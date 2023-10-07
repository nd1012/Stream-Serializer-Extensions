using System.Reflection;

namespace wan24.StreamSerializerExtensions
{
    /// <summary>
    /// Base class for an automatic serializable type
    /// </summary>
    /// <typeparam name="T">Final type</typeparam>
    public abstract record class AutoStreamSerializerRecordBase<T> : StreamSerializerRecordBase, IAutoStreamSerializer where T : AutoStreamSerializerRecordBase<T>
    {
        /// <summary>
        /// Auto stream serializer configuration
        /// </summary>
        private static AutoStreamSerializerConfig<T> _AutoStreamSerializerConfig = null!;

        /// <summary>
        /// Static constructor
        /// </summary>
        static AutoStreamSerializerRecordBase()
        {
            if (typeof(T).GetCustomAttribute<StreamSerializerAttribute>() == null) throw new InvalidProgramException($"{typeof(T)} needs a {typeof(StreamSerializerAttribute)}");
        }

        /// <summary>
        /// Constructor
        /// </summary>
        protected AutoStreamSerializerRecordBase() : base(AutoStreamSerializerConfig.Attribute.Version) { }

        /// <summary>
        /// Auto stream serializer configuration
        /// </summary>
        protected static AutoStreamSerializerConfig<T> AutoStreamSerializerConfig => _AutoStreamSerializerConfig ??= new();

        /// <inheritdoc/>
        IAutoStreamSerializerConfig IAutoStreamSerializer.AutoStreamSerializerConfig => AutoStreamSerializerConfig;

        /// <inheritdoc/>
        protected override void Serialize(Stream stream) => AutoStreamSerializerConfig.Serialize((T)this, stream);

        /// <inheritdoc/>
        protected override Task SerializeAsync(Stream stream, CancellationToken cancellationToken)
            => AutoStreamSerializerConfig.SerializeAsync((T)this, stream, cancellationToken);

        /// <inheritdoc/>
        protected override void Deserialize(Stream stream, int version) => AutoStreamSerializerConfig.Deserialize((T)this, stream, version);

        /// <inheritdoc/>
        protected override Task DeserializeAsync(Stream stream, int version, CancellationToken cancellationToken)
            => AutoStreamSerializerConfig.DeserializeAsync((T)this, stream, version, cancellationToken);
    }
}
