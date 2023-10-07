using System.Reflection;

namespace wan24.StreamSerializerExtensions
{
    /// <summary>
    /// Base class for an automatic serializable type
    /// </summary>
    /// <typeparam name="T">Final type</typeparam>
    public abstract record class DisposableAutoStreamSerializerRecordBase<T>
        : DisposableStreamSerializerRecordBase, IAutoStreamSerializer
        where T : DisposableAutoStreamSerializerRecordBase<T>
    {
        /// <summary>
        /// Auto stream serializer configuration
        /// </summary>
        protected static readonly AutoStreamSerializerConfig<T> AutoStreamSerializerConfig;

        /// <summary>
        /// Static constructor
        /// </summary>
        static DisposableAutoStreamSerializerRecordBase()
        {
            if (typeof(T).GetCustomAttribute<StreamSerializerAttribute>() == null) throw new InvalidProgramException($"{typeof(T)} needs a {typeof(StreamSerializerAttribute)}");
            AutoStreamSerializerConfig = new(initDefaultValues: false);
            AutoStreamSerializerConfig.InitDefaultValues();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        protected DisposableAutoStreamSerializerRecordBase() : base(AutoStreamSerializerConfig.Attribute.Version) { }

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
