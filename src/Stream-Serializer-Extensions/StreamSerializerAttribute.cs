using System.Reflection;

namespace wan24.StreamSerializerExtensions
{
    /// <summary>
    /// Attribute for stream serializable classes and properties
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class StreamSerializerAttribute : Attribute
    {
        /// <summary>
        /// Constructor (used for a type)
        /// </summary>
        /// <param name="mode">Type serializer mode (can't be <see cref="StreamSerializerModes.Auto"/>)</param>
        /// <param name="version">Object version</param>
        /// <param name="skipPropertyNameChecksum">Skip the property name checksum?</param>
        public StreamSerializerAttribute(StreamSerializerModes mode = StreamSerializerModes.OptOut, int version = 0, bool skipPropertyNameChecksum = false) : base()
        {
            if (mode == StreamSerializerModes.Auto) throw new ArgumentException($"Type serializer mode can't be {StreamSerializerModes.Auto}", nameof(mode));
            Mode = mode;
            Version = version == 0 ? null : version;
            SkipPropertyNameChecksum = skipPropertyNameChecksum;
        }

        /// <summary>
        /// Constructor (used for a property)
        /// </summary>
        /// <param name="fromVersion">Start object version</param>
        /// <param name="version">Last supported object version</param>
        /// <param name="position">Position</param>
        /// <param name="skipPropertyNameChecksum">Skip the property name checksum?</param>
        /// <param name="mode">Property serializer mode (use <see cref="StreamSerializerModes.Auto"/> to inherit from the type serializer mode)</param>
        public StreamSerializerAttribute(int fromVersion, int version = 0, int position = 0, bool skipPropertyNameChecksum = false, StreamSerializerModes mode = StreamSerializerModes.Auto)
            : base()
        {
            Mode = mode;
            SkipPropertyNameChecksum = skipPropertyNameChecksum;
            Version = version == 0 ? null : version;
            FromVersion = fromVersion == 0 ? null : fromVersion;
            Position = position == 0 ? null : position;
        }

        /// <summary>
        /// Serializer mode
        /// </summary>
        public StreamSerializerModes Mode { get; }

        /// <summary>
        /// Skip the property name checksum?
        /// </summary>
        public bool SkipPropertyNameChecksum { get; }

        /// <summary>
        /// Object version
        /// </summary>
        public int? Version { get; }

        /// <summary>
        /// Starting from object version
        /// </summary>
        public int? FromVersion { get; set; }

        /// <summary>
        /// Value sequence position
        /// </summary>
        public int? Position { get; }

        /// <summary>
        /// Determine if the property value is included in a specific object version
        /// </summary>
        /// <param name="mode">Type serializer mode mode</param>
        /// <param name="version">Object version (<c>0</c> to skip object version compliance checking)</param>
        /// <returns>Value is included?</returns>
        public virtual bool IsIncluded(StreamSerializerModes mode, int version)
        {
            if (version == 0 || (FromVersion == null && Version == null))
            {
                switch (mode)
                {
                    case StreamSerializerModes.OptIn:
                        if (Mode == StreamSerializerModes.OptOut) return false;
                        break;
                    case StreamSerializerModes.OptOut:
                        if (Mode != StreamSerializerModes.OptIn) return false;
                        break;
                    default:
                        throw new ArgumentException($"Type serializer mode can't be {StreamSerializerModes.Auto}", nameof(mode));
                }
            }
            else
            {
                if (FromVersion != null && version < FromVersion) return false;
                if (Version != null && version > Version) return false;
            }
            return true;
        }

        /// <summary>
        /// Get properties to write
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Properties</returns>
        public static IEnumerable<PropertyInfo> GetWriteProperties(Type type)
        {
            StreamSerializerAttribute? attr = type.GetCustomAttribute<StreamSerializerAttribute>(),
                objAttr;
            StreamSerializerModes mode = attr?.Mode ?? StreamSerializerModes.OptOut;
            return attr?.Version is int version
                ? mode switch
                {
                    StreamSerializerModes.OptIn => from pi in type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                                   where (pi.GetMethod?.IsPublic ?? false) &&
                                                    (pi.SetMethod?.IsPublic ?? false) &&
                                                    (objAttr = pi.GetCustomAttribute<StreamSerializerAttribute>()) != null &&
                                                    objAttr.IsIncluded(mode, version)
                                                   orderby pi.GetCustomAttribute<StreamSerializerAttribute>()!.Position, pi.Name
                                                   select pi,
                    StreamSerializerModes.OptOut => from pi in type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                                    where (pi.GetMethod?.IsPublic ?? false) &&
                                                     (pi.SetMethod?.IsPublic ?? false) &&
                                                     (
                                                         (objAttr = pi.GetCustomAttribute<StreamSerializerAttribute>()) == null ||
                                                         objAttr.IsIncluded(mode, version)
                                                     )
                                                    orderby pi.GetCustomAttribute<StreamSerializerAttribute>()?.Position, pi.Name
                                                    select pi,
                    _ => throw new InvalidProgramException($"Type serializer mode can't be {StreamSerializerModes.Auto}")
                }
                : mode switch
                {
                    StreamSerializerModes.OptIn => from pi in type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                                   where (pi.GetMethod?.IsPublic ?? false) &&
                                                    (pi.SetMethod?.IsPublic ?? false) &&
                                                    (objAttr = pi.GetCustomAttribute<StreamSerializerAttribute>()) != null &&
                                                    objAttr.Mode != StreamSerializerModes.OptOut
                                                   orderby pi.GetCustomAttribute<StreamSerializerAttribute>()!.Position, pi.Name
                                                   select pi,
                    StreamSerializerModes.OptOut => from pi in type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                                    where (pi.GetMethod?.IsPublic ?? false) &&
                                                     (pi.SetMethod?.IsPublic ?? false) &&
                                                     (
                                                         (objAttr = pi.GetCustomAttribute<StreamSerializerAttribute>()) == null ||
                                                         objAttr.Mode == StreamSerializerModes.OptIn
                                                     )
                                                    orderby pi.GetCustomAttribute<StreamSerializerAttribute>()!.Position, pi.Name
                                                    select pi,
                    _ => throw new InvalidProgramException($"Type serializer mode can't be {StreamSerializerModes.Auto}")
                };
        }

        /// <summary>
        /// Get properties to read
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="version">Object version</param>
        /// <returns>Properties</returns>
        public static IEnumerable<PropertyInfo> GetReadProperties(Type type, int? version)
        {
            StreamSerializerAttribute? attr = type.GetCustomAttribute<StreamSerializerAttribute>(),
                objAttr;
            StreamSerializerModes mode = attr?.Mode ?? StreamSerializerModes.OptOut;
            return version != null
                ? mode switch
                {
                    StreamSerializerModes.OptIn => from pi in type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                                   where (pi.GetMethod?.IsPublic ?? false) &&
                                                    (pi.SetMethod?.IsPublic ?? false) &&
                                                    (objAttr = pi.GetCustomAttribute<StreamSerializerAttribute>()) != null &&
                                                    objAttr.IsIncluded(mode, version.Value)
                                                   orderby pi.GetCustomAttribute<StreamSerializerAttribute>()?.Position ?? 0, pi.Name
                                                   select pi,
                    StreamSerializerModes.OptOut => from pi in type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                                    where (pi.GetMethod?.IsPublic ?? false) &&
                                                     (pi.SetMethod?.IsPublic ?? false) &&
                                                     (
                                                         (objAttr = pi.GetCustomAttribute<StreamSerializerAttribute>()) == null ||
                                                         objAttr.IsIncluded(mode, version.Value)
                                                     )
                                                    orderby pi.GetCustomAttribute<StreamSerializerAttribute>()?.Position ?? 0, pi.Name
                                                    select pi,
                    _ => throw new InvalidProgramException($"Type serializer mode can't be {StreamSerializerModes.Auto}")
                }
                : mode switch
                {
                    StreamSerializerModes.OptIn => from pi in type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                                   where (pi.GetMethod?.IsPublic ?? false) &&
                                                    (pi.SetMethod?.IsPublic ?? false) &&
                                                    (objAttr = pi.GetCustomAttribute<StreamSerializerAttribute>()) != null &&
                                                    objAttr.Mode != StreamSerializerModes.OptOut
                                                   orderby pi.GetCustomAttribute<StreamSerializerAttribute>()?.Position ?? 0, pi.Name
                                                   select pi,
                    StreamSerializerModes.OptOut => from pi in type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                                    where (pi.GetMethod?.IsPublic ?? false) &&
                                                     (pi.SetMethod?.IsPublic ?? false) &&
                                                     (
                                                         (objAttr = pi.GetCustomAttribute<StreamSerializerAttribute>()) == null ||
                                                         objAttr.Mode == StreamSerializerModes.OptIn
                                                     )
                                                    orderby pi.GetCustomAttribute<StreamSerializerAttribute>()?.Position ?? 0, pi.Name
                                                    select pi,
                    _ => throw new InvalidProgramException($"Type serializer mode can't be {StreamSerializerModes.Auto}")
                };
        }
    }
}
