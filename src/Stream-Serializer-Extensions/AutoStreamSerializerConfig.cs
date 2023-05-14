using System.Reflection;
using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    /// <summary>
    /// Auto stream serializer configuration
    /// </summary>
    /// <typeparam name="T">Serialized object type</typeparam>
    public sealed class AutoStreamSerializerConfig<T> : IAutoStreamSerializerConfig where T : class, IAutoStreamSerializer
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public AutoStreamSerializerConfig()
        {
            Type = typeof(T);
            Attribute = Type.GetCustomAttribute<StreamSerializerAttribute>() ?? throw new InvalidProgramException($"{typeof(T)} is missing a {typeof(StreamSerializerAttribute)}");
            if (Attribute.Version == null)
                throw new InvalidProgramException($"{typeof(StreamSerializerAttribute)}.{nameof(StreamSerializerAttribute.Version)} of {typeof(T)} requires a value");
            if (Attribute.Mode == StreamSerializerModes.Auto)
                throw new InvalidProgramException($"{typeof(StreamSerializerAttribute)}.{nameof(StreamSerializerAttribute.Mode)} ({Attribute.Mode}) of {typeof(T)} not acceptable");
            StreamSerializerAttribute? objAttr;
            Infos = new(from pi in Type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                        where (pi.GetMethod?.IsPublic ?? false) &&
                         (pi.SetMethod?.IsPublic ?? false) &&
                         (objAttr = pi.GetCustomAttribute<StreamSerializerAttribute>()) != null &&
                         objAttr.IsIncluded(Attribute.Mode, version: 0)
                        orderby pi.GetCustomAttribute<StreamSerializerAttribute>()?.Position ?? 0, pi.Name
                        select new KeyValuePair<string, AutoStreamSerializerInfo>(pi.Name, new(pi)));
        }

        /// <inheritdoc/>
        public Type Type { get; }

        /// <inheritdoc/>
        public StreamSerializerAttribute Attribute { get; }

        /// <inheritdoc/>
        public OrderedDictionary<string, AutoStreamSerializerInfo> Infos { get; }

        /// <summary>
        /// Serialize
        /// </summary>
        /// <param name="obj">Object</param>
        /// <param name="stream">Stream</param>
        public void Serialize(T obj, Stream stream)
        {
            foreach (PropertyInfo pi in StreamSerializerAttribute.GetWriteProperties(obj.GetType()))
            {
                if (!Infos.TryGetValue(pi.Name, out AutoStreamSerializerInfo? info))
                    throw new SerializerException($"Missing auto stream serializer information for {obj.GetType()}.{pi.Name}", new InvalidProgramException());
                info.Serialize(this, obj, stream);
            }
        }

        /// <summary>
        /// Serialize
        /// </summary>
        /// <param name="obj">Object</param>
        /// <param name="stream">Stream</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public async Task SerializeAsync(T obj, Stream stream, CancellationToken cancellationToken)
        {
            foreach (PropertyInfo pi in StreamSerializerAttribute.GetWriteProperties(obj.GetType()))
            {
                if (!Infos.TryGetValue(pi.Name, out AutoStreamSerializerInfo? info))
                    throw new SerializerException($"Missing auto stream serializer information for {obj.GetType()}.{pi.Name}", new InvalidProgramException());
                await info.SerializeAsync(this, obj, stream, cancellationToken).DynamicContext();
            }
        }

        /// <summary>
        /// Deserialize
        /// </summary>
        /// <param name="obj">Object</param>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        public void Deserialize(T obj, Stream stream, int version)
        {
            foreach (PropertyInfo pi in StreamSerializerAttribute.GetReadProperties(obj.GetType(), version))
            {
                if (!Infos.TryGetValue(pi.Name, out AutoStreamSerializerInfo? info))
                    throw new SerializerException($"Missing auto stream serializer information for {obj.GetType()}.{pi.Name}", new InvalidProgramException());
                info.Deserialize(this, obj, stream, version);
            }
        }

        /// <summary>
        /// Deserialize
        /// </summary>
        /// <param name="obj">Object</param>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public async Task DeserializeAsync(T obj, Stream stream, int version, CancellationToken cancellationToken)
        {
            foreach (PropertyInfo pi in StreamSerializerAttribute.GetReadProperties(obj.GetType(), version))
            {
                if (!Infos.TryGetValue(pi.Name, out AutoStreamSerializerInfo? info))
                    throw new SerializerException($"Missing auto stream serializer information for {obj.GetType()}.{pi.Name}", new InvalidProgramException());
                await info.DeserializeAsync(this, obj, stream, version, cancellationToken).DynamicContext();
            }
        }
    }
}
