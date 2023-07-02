using System.Reflection;
using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    /// <summary>
    /// Attribute for stream serializable classes and properties
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Constructor)]
    public class StreamSerializerAttribute : Attribute
    {
        /// <summary>
        /// Structure fields which require endianess conversion
        /// </summary>
        protected List<FieldInfo>? NumericStructureFields = null;

        /// <summary>
        /// Constructor (used for a type)
        /// </summary>
        /// <param name="mode">Type serializer mode (can't be <see cref="StreamSerializerModes.Auto"/>)</param>
        /// <param name="version">Object version</param>
        /// <param name="skipPropertyNameChecksum">Skip the property name checksum?</param>
        public StreamSerializerAttribute(StreamSerializerModes mode = StreamSerializerModes.OptOut, int version = 0, bool skipPropertyNameChecksum = false) : base()
        {
            ArgumentValidationHelper.EnsureValidArgument(nameof(mode), mode != StreamSerializerModes.Auto, () => $"Type serializer mode can't be {StreamSerializerModes.Auto}");
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
        /// Use default value(s) for automatic configured serialization?
        /// </summary>
        public bool UseDefaultValues { get; set; } = true;

        /// <summary>
        /// <see cref="ISerializerOptions"/> type
        /// </summary>
        public Type? OptionsType { get; set; }

        /// <summary>
        /// Key <see cref="ISerializerOptions"/> type
        /// </summary>
        public Type? KeyOptionsType { get; set; }

        /// <summary>
        /// Value <see cref="ISerializerOptions"/> type
        /// </summary>
        public Type? ValueOptionsType { get; set; }

        /// <summary>
        /// Minimum length
        /// </summary>
        public long? MinLen { get; set; }

        /// <summary>
        /// Maximum length
        /// </summary>
        public long? MaxLen { get; set; }

        /// <summary>
        /// Stream factory
        /// </summary>
        public StreamFactory_Delegate? StreamFactory { get; set; }

        /// <summary>
        /// Stream factory type
        /// </summary>
        public Type? StreamFactoryType { get; set; }

        /// <summary>
        /// Stream factory method (a <see cref="StreamFactory_Delegate"/> delegate)
        /// </summary>
        public string? StreamFactoryMethod { get; set; }

        /// <summary>
        /// Serializer options
        /// </summary>
        public ISerializerOptions? SerializerOptions { get; set; }

        /// <summary>
        /// Serializer options factory type
        /// </summary>
        public Type? SerializerOptionsFactoryType { get; set; }

        /// <summary>
        /// Serializer options factory method (a <see cref="SerializerOptionsFactory_Delegate"/> delegate)
        /// </summary>
        public string? SerializerOptionsFactoryMethod { get; set; }

        /// <summary>
        /// Key serializer options
        /// </summary>
        public ISerializerOptions? KeySerializerOptions { get; set; }

        /// <summary>
        /// Key serializer options factory type (for deserializing dictionaries)
        /// </summary>
        public Type? KeySerializerOptionsFactoryType { get; set; }

        /// <summary>
        /// Key serializer options factory method (a <see cref="SerializerOptionsFactory_Delegate"/> delegate)
        /// </summary>
        public string? KeySerializerOptionsFactoryMethod { get; set; }

        /// <summary>
        /// Value serializer options
        /// </summary>
        public ISerializerOptions? ValueSerializerOptions { get; set; }

        /// <summary>
        /// Value serializer options factory type (for deserializing arrays, lists and dictionaries)
        /// </summary>
        public Type? ValueSerializerOptionsFactoryType { get; set; }

        /// <summary>
        /// Value serializer options factory method (a <see cref="SerializerOptionsFactory_Delegate"/> delegate)
        /// </summary>
        public string? ValueSerializerOptionsFactoryMethod { get; set; }

        /// <summary>
        /// Serializer type
        /// </summary>
        public SerializerTypes? Serializer { get; set; }

        /// <summary>
        /// Get a stream from the stream factory for deserializing an embedded stream
        /// </summary>
        /// <param name="obj">Deserializing object</param>
        /// <param name="property">Target property</param>
        /// <param name="stream">Source stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream to use for deserializing an embedded stream</returns>
        public virtual Stream? GetStream(object? obj, PropertyInfo? property, Stream stream, int version, CancellationToken cancellationToken = default)
        {
            if (obj == null) ArgumentValidationHelper.EnsureValidArgument(nameof(obj), property == null);
            if (property == null) ArgumentValidationHelper.EnsureValidArgument(nameof(property), obj == null);
            if (StreamFactory == null)
            {
                if (StreamFactoryType == null) return null;
                if (StreamFactoryMethod == null)
                    throw new SerializerException(
                        obj == null
                            ? "Stream serializer attribute defines a stream factory type, but no method"
                            : $"{obj.GetType()}.{property!.Name} stream serializer attribute defines a stream factory type, but no method"
                        , new InvalidProgramException()
                        );
                MethodInfo delegateInfo = typeof(StreamFactory_Delegate).GetMethodCached(nameof(StreamFactory_Delegate.Invoke), BindingFlags.Public)!,
                    mi = StreamFactoryType.GetMethod(
                        StreamFactoryMethod,
                        BindingFlags.Public | BindingFlags.Static,
                        filter: null,
                        genericArgumentCount: null,
                        exactTypes: false,
                        delegateInfo.ReturnType,
                        delegateInfo.GetParametersCached().Select(p => p.ParameterType).ToArray()
                        ) ??
                        throw new SerializerException(
                            obj == null
                                ? $"Stream serializer attribute defined stream factory {StreamFactoryType}.{StreamFactoryMethod} not found"
                                : $"{obj.GetType()}.{property!.Name} stream serializer attribute defined stream factory {StreamFactoryType}.{StreamFactoryMethod} not found",
                            new InvalidProgramException()
                            );
                StreamFactory = mi.CreateDelegate<StreamFactory_Delegate>();
            }
            return StreamFactory(obj, property, this, stream, version, cancellationToken);
        }

        /// <summary>
        /// Get serializer options from the serializer options factory, or the default
        /// </summary>
        /// <param name="property">Target property</param>
        /// <param name="stream">Source stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Serializer options</returns>
        public virtual ISerializerOptions GetSerializerOptions(PropertyInfoExt? property, Stream stream, int version, CancellationToken cancellationToken = default)
        {
            try
            {
                if (SerializerOptions != null) return SerializerOptions;
                if (SerializerOptionsFactoryType == null) return SerializerOptions ??= CreateSerializerOptions(OptionsType, property);
                if (SerializerOptionsFactoryMethod == null)
                    throw new SerializerException(
                        property == null
                            ? "Stream serializer attribute defines a serializer options factory type, but no method"
                            : $"{property.DeclaringType}.{property!.Name} stream serializer attribute defines a serializer options factory type, but no method",
                        new InvalidProgramException()
                        );
                MethodInfo delegateInfo = typeof(SerializerOptionsFactory_Delegate).GetMethodCached("Invoke")!,
                    mi = SerializerOptionsFactoryType.GetMethod(
                        SerializerOptionsFactoryMethod,
                        BindingFlags.Public | BindingFlags.Static,
                        filter: null,
                        genericArgumentCount: null,
                        exactTypes: false,
                        delegateInfo.ReturnType,
                        delegateInfo.GetParametersCached().Select(p => p.ParameterType).ToArray()
                        ) ??
                        throw new SerializerException(
                            property == null
                                ? $"Stream serializer attribute defined serializer options factory {SerializerOptionsFactoryType}.{SerializerOptionsFactoryMethod} not found"
                                : $"{property.DeclaringType}.{property!.Name} stream serializer attribute defined serializer options factory {SerializerOptionsFactoryType}.{SerializerOptionsFactoryMethod} not found",
                            new InvalidProgramException()
                            );
                return (ISerializerOptions)mi.Invoke(obj: null, new object?[] { property, this, stream, version, cancellationToken })!;
            }
            finally
            {
                KeySerializerOptions ??= GetKeySerializerOptions(property, stream, version, cancellationToken);
                ValueSerializerOptions ??= GetValueSerializerOptions(property, stream, version, cancellationToken);
            }
        }

        /// <summary>
        /// Get key serializer options from the serializer options factory, or the default
        /// </summary>
        /// <param name="property">Target property</param>
        /// <param name="stream">Source stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Serializer options</returns>
        public virtual ISerializerOptions GetKeySerializerOptions(PropertyInfoExt? property, Stream stream, int version, CancellationToken cancellationToken = default)
        {
            if (KeySerializerOptions != null) return KeySerializerOptions;
            if (KeySerializerOptionsFactoryType == null) return KeySerializerOptions ??= CreateSerializerOptions(KeyOptionsType, property);
            if (KeySerializerOptionsFactoryMethod == null)
                throw new SerializerException(
                    property == null
                        ? "Stream serializer attribute defines a key serializer options factory type, but no method"
                        : $"{property.DeclaringType}.{property!.Name} stream serializer attribute defines a key serializer options factory type, but no method",
                    new InvalidProgramException()
                    );
            MethodInfo delegateInfo = typeof(SerializerOptionsFactory_Delegate).GetMethodCached("Invoke")!,
                mi = KeySerializerOptionsFactoryType.GetMethod(
                    KeySerializerOptionsFactoryMethod,
                    BindingFlags.Public | BindingFlags.Static,
                    filter: null,
                    genericArgumentCount: null,
                    exactTypes: false,
                    delegateInfo.ReturnType,
                    delegateInfo.GetParametersCached().Select(p => p.ParameterType).ToArray()
                    ) ??
                    throw new SerializerException(
                        property == null
                            ? $"Stream serializer attribute defined serializer options factory {KeySerializerOptionsFactoryType}.{KeySerializerOptionsFactoryMethod} not found"
                            : $"{property.DeclaringType}.{property!.Name} stream serializer attribute defined serializer options factory {KeySerializerOptionsFactoryType}.{KeySerializerOptionsFactoryMethod} not found",
                        new InvalidProgramException()
                        );
            return (ISerializerOptions)mi.Invoke(obj: null, new object?[] { property, this, stream, version, cancellationToken })!;
        }

        /// <summary>
        /// Get value serializer options from the serializer options factory, or the default
        /// </summary>
        /// <param name="property">Target property</param>
        /// <param name="stream">Source stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Serializer options</returns>
        public virtual ISerializerOptions GetValueSerializerOptions(PropertyInfoExt? property, Stream stream, int version, CancellationToken cancellationToken = default)
        {
            if (ValueSerializerOptions != null) return ValueSerializerOptions;
            if (ValueSerializerOptionsFactoryType == null) return ValueSerializerOptions ??= CreateSerializerOptions(ValueOptionsType, property);
            if (ValueSerializerOptionsFactoryMethod == null)
                throw new SerializerException(
                    property == null
                        ? "Stream serializer attribute defines a value serializer options factory type, but no method"
                        : $"{property.DeclaringType}.{property!.Name} stream serializer attribute defines a value serializer options factory type, but no method",
                    new InvalidProgramException()
                    );
            MethodInfo delegateInfo = typeof(SerializerOptionsFactory_Delegate).GetMethodCached("Invoke")!,
                mi = ValueSerializerOptionsFactoryType.GetMethod(
                    ValueSerializerOptionsFactoryMethod,
                    BindingFlags.Public | BindingFlags.Static,
                    filter: null,
                    genericArgumentCount: null,
                    exactTypes: false,
                    delegateInfo.ReturnType,
                    delegateInfo.GetParametersCached().Select(p => p.ParameterType).ToArray()
                    ) ??
                    throw new SerializerException(
                        property == null
                            ? $"Stream serializer attribute defined serializer options factory {ValueSerializerOptionsFactoryType}.{ValueSerializerOptionsFactoryMethod} not found"
                            : $"{property.DeclaringType}.{property!.Name} stream serializer attribute defined serializer options factory {ValueSerializerOptionsFactoryType}.{ValueSerializerOptionsFactoryMethod} not found",
                        new InvalidProgramException()
                        );
            return (ISerializerOptions)mi.Invoke(obj: null, new object?[] { property, this, stream, version, cancellationToken })!;
        }

        /// <summary>
        /// Determine if the property value is included in a specific object version
        /// </summary>
        /// <param name="mode">Type serializer mode</param>
        /// <param name="version">Object version (<c>0</c> to skip object version compliance checking)</param>
        /// <returns>Value is included?</returns>
        public virtual bool IsIncluded(StreamSerializerModes mode, int version)
        {
            ArgumentValidationHelper.EnsureValidArgument(nameof(mode), mode != StreamSerializerModes.Auto, () => $"Type serializer mode can't be {StreamSerializerModes.Auto}");
            return (version == 0 || (FromVersion == null && Version == null) || (FromVersion == null || version >= FromVersion) && (Version == null || version <= Version)) &&
                (
                    (mode == StreamSerializerModes.OptOut && Mode.In(StreamSerializerModes.OptOut, StreamSerializerModes.Auto)) ||
                    (Mode == StreamSerializerModes.Auto ? mode : Mode) == StreamSerializerModes.OptIn
                );
        }

        /// <summary>
        /// Get if using the default value(s) for automatic configured serializing
        /// </summary>
        /// <param name="version">Object version</param>
        /// <returns>Use the default value(s)?</returns>
        public virtual bool GetUseDefaultValue(int version) => UseDefaultValues;

        /// <summary>
        /// Get the structure fields which require endianess conversion
        /// </summary>
        /// <param name="type">Structure type</param>
        /// <returns>Fields</returns>
        public virtual List<FieldInfo> GetNumericStructureFields(Type type)
        {
            ArgumentValidationHelper.EnsureValidArgument(nameof(type), type.IsValueType, () => "Structure type required");
            return NumericStructureFields ??= new(from fi in type.GetFieldsCached(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                                           where !fi.IsStatic &&
                                              fi.GetCustomAttributeCached<StreamSerializerAttribute>() is not null
                                           select fi);
        }

        /// <summary>
        /// Create serializer options
        /// </summary>
        /// <param name="type">Custom serializer options type</param>
        /// <param name="property">Property</param>
        /// <returns>Serializer options</returns>
        protected virtual ISerializerOptions CreateSerializerOptions(Type? type, PropertyInfoExt? property)
            => type == null ? new DefaultSerializerOptions(property, this) : type.ConstructAuto(usePrivate: true, property, this) as ISerializerOptions
                ?? throw new SerializerException($"Invalid serializer options type {type} (must implement {typeof(ISerializerOptions)})", new InvalidProgramException());

        /// <summary>
        /// Delegate for a stream factory
        /// </summary>
        /// <param name="obj">Deserializing object</param>
        /// <param name="property">Target property</param>
        /// <param name="attr">Stream serializer attribute</param>
        /// <param name="stream">Source stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream to use for deserializing an embedded stream</returns>
        public delegate Stream StreamFactory_Delegate(
            object? obj,
            PropertyInfo? property,
            StreamSerializerAttribute attr,
            Stream stream,
            int version,
            CancellationToken cancellationToken = default
            );

        /// <summary>
        /// Delegate for a serializer options factory
        /// </summary>
        /// <param name="obj">Deserializing object</param>
        /// <param name="property">Target property</param>
        /// <param name="attr">Stream serializer attribute</param>
        /// <param name="stream">Source stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Serializer options</returns>
        public delegate ISerializerOptions SerializerOptionsFactory_Delegate(
            object? obj,
            PropertyInfo? property,
            StreamSerializerAttribute attr,
            Stream stream,
            int version,
            CancellationToken cancellationToken = default
            );

        /// <summary>
        /// Delegate for a stream factory
        /// </summary>
        /// <param name="obj">Deserializing object</param>
        /// <param name="property">Target property</param>
        /// <param name="attr">Stream serializer attribute</param>
        /// <param name="stream">Source stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream to use for deserializing an embedded stream</returns>
#pragma warning disable IDE0060 // Remove unused parameter
        public static Stream MemoryStreamFactory(
            object? obj,
            PropertyInfo? property,
            StreamSerializerAttribute attr,
            Stream stream,
            int version,
            CancellationToken cancellationToken = default
            )
#pragma warning restore IDE0060 // Remove unused parameter
            => new MemoryStream();

        /// <summary>
        /// Get properties to write
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="version">Object version</param>
        /// <returns>Properties</returns>
        public static IEnumerable<PropertyInfoExt> GetWriteProperties(Type type, int? version = null)
        {
            StreamSerializerAttribute? attr = type.GetCustomAttributeCached<StreamSerializerAttribute>();
            StreamSerializerModes mode = attr?.Mode ?? StreamSerializerModes.OptOut;
            version ??= attr?.Version ?? 0;
            return mode switch
            {
                StreamSerializerModes.OptIn => from pi in type.GetPropertiesCached(BindingFlags.Instance | BindingFlags.Public)
                                               where (pi.Property.GetMethod?.IsPublic ?? false) &&
                                                (pi.Property.SetMethod?.IsPublic ?? false) &&
                                                pi.Property.GetCustomAttributeCached<StreamSerializerAttribute>() is StreamSerializerAttribute objAttr &&
                                                objAttr.IsIncluded(mode, version.Value)
                                               orderby pi.Property.GetCustomAttributeCached<StreamSerializerAttribute>()!.Position, pi.Property.Name
                                               select pi,
                StreamSerializerModes.OptOut => from pi in type.GetPropertiesCached(BindingFlags.Instance | BindingFlags.Public)
                                                where (pi.Property.GetMethod?.IsPublic ?? false) &&
                                                 (pi.Property.SetMethod?.IsPublic ?? false) &&
                                                 (
                                                     pi.Property.GetCustomAttributeCached<StreamSerializerAttribute>() is not StreamSerializerAttribute objAttr ||
                                                     objAttr.IsIncluded(mode, version.Value)
                                                 )
                                                orderby pi.Property.GetCustomAttributeCached<StreamSerializerAttribute>()?.Position, pi.Property.Name
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
        public static IEnumerable<PropertyInfoExt> GetReadProperties(Type type, int? version)
        {
            StreamSerializerAttribute? attr = type.GetCustomAttributeCached<StreamSerializerAttribute>();
            StreamSerializerModes mode = attr?.Mode ?? StreamSerializerModes.OptOut;
            version = attr?.Version ?? 0;
            return mode switch
            {
                StreamSerializerModes.OptIn => from pi in type.GetPropertiesCached(BindingFlags.Instance | BindingFlags.Public)
                                               where (pi.Property.GetMethod?.IsPublic ?? false) &&
                                                (pi.Property.SetMethod?.IsPublic ?? false) &&
                                                pi.Property.GetCustomAttributeCached<StreamSerializerAttribute>() is StreamSerializerAttribute objAttr &&
                                                objAttr.IsIncluded(mode, version.Value)
                                               orderby pi.Property.GetCustomAttributeCached<StreamSerializerAttribute>()?.Position ?? 0, pi.Property.Name
                                               select pi,
                StreamSerializerModes.OptOut => from pi in type.GetPropertiesCached(BindingFlags.Instance | BindingFlags.Public)
                                                where (pi.Property.GetMethod?.IsPublic ?? false) &&
                                                 (pi.Property.SetMethod?.IsPublic ?? false) &&
                                                 (
                                                     pi.Property.GetCustomAttributeCached<StreamSerializerAttribute>() is not StreamSerializerAttribute objAttr ||
                                                     objAttr.IsIncluded(mode, version.Value)
                                                 )
                                                orderby pi.Property.GetCustomAttributeCached<StreamSerializerAttribute>()?.Position ?? 0, pi.Property.Name
                                                select pi,
                _ => throw new InvalidProgramException($"Type serializer mode can't be {StreamSerializerModes.Auto}")
            };
        }
    }
}
