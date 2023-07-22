using System.Reflection;

namespace wan24.StreamSerializerExtensions
{
    /// <summary>
    /// Base class for an automatic serializable type
    /// </summary>
    /// <typeparam name="T">Final type</typeparam>
    public abstract class AutoStreamSerializerBase<T> : StreamSerializerBase, IAutoStreamSerializer where T : AutoStreamSerializerBase<T>
    {
        /// <summary>
        /// Auto stream serializer configuration
        /// </summary>
        private static AutoStreamSerializerConfig<T> _AutoStreamSerializerConfig = null!;

        /// <summary>
        /// Static constructor
        /// </summary>
        static AutoStreamSerializerBase()
        {
            if (typeof(T).GetCustomAttribute<StreamSerializerAttribute>() == null)
                throw new InvalidProgramException($"{typeof(T)} needs a {typeof(StreamSerializerAttribute)}");
        }

        /// <summary>
        /// Constructor
        /// </summary>
        protected AutoStreamSerializerBase() : base(AutoStreamSerializerConfig.Attribute.Version) { }

        /// <summary>
        /// Auto stream serializer configuration
        /// </summary>
        protected static AutoStreamSerializerConfig<T> AutoStreamSerializerConfig => _AutoStreamSerializerConfig ??= new();

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
