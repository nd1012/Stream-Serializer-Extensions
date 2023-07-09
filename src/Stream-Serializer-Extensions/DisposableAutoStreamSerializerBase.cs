using System.Reflection;

namespace wan24.StreamSerializerExtensions
{
    /// <summary>
    /// Base class for an automatic serializable type
    /// </summary>
    /// <typeparam name="T">Final type</typeparam>
    public abstract class DisposableAutoStreamSerializerBase<T> : DisposableStreamSerializerBase, IAutoStreamSerializer where T : DisposableAutoStreamSerializerBase<T>
    {
        /// <summary>
        /// Auto stream serializer configuration
        /// </summary>
        protected static readonly AutoStreamSerializerConfig<T> AutoStreamSerializerConfig;

        /// <summary>
        /// Static constructor
        /// </summary>
        static DisposableAutoStreamSerializerBase()
        {
            if (typeof(T).GetCustomAttribute<StreamSerializerAttribute>() == null) throw new InvalidProgramException($"{typeof(T)} needs a {typeof(StreamSerializerAttribute)}");
            AutoStreamSerializerConfig = new(initDefaultValues: false);
            AutoStreamSerializerConfig.InitDefaultValues();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        protected DisposableAutoStreamSerializerBase() : base(AutoStreamSerializerConfig.Attribute.Version) { }

        /// <inheritdoc/>
        IAutoStreamSerializerConfig IAutoStreamSerializer.AutoStreamSerializerConfig => AutoStreamSerializerConfig;

        /// <inheritdoc/>
        protected override void Serialize(ISerializationContext context) => AutoStreamSerializerConfig.Serialize((T)this, context);

        /// <inheritdoc/>
        protected override Task SerializeAsync(ISerializationContext context) => AutoStreamSerializerConfig.SerializeAsync((T)this, context);

        /// <inheritdoc/>
        protected override void Deserialize(IDeserializationContext context) => AutoStreamSerializerConfig.Deserialize((T)this, context);

        /// <inheritdoc/>
        protected override Task DeserializeAsync(IDeserializationContext context) => AutoStreamSerializerConfig.DeserializeAsync((T)this, context);
    }
}
