using System.Reflection;
using wan24.Core;

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
        protected static readonly AutoStreamSerializerConfig<T> AutoStreamSerializerConfig;

        /// <summary>
        /// Static constructor
        /// </summary>
        static AutoStreamSerializerBase()
        {
            if (typeof(T).GetCustomAttribute<StreamSerializerAttribute>() == null) throw new InvalidProgramException($"{typeof(T)} needs a {typeof(StreamSerializerAttribute)}");
            AutoStreamSerializerConfig = new();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        protected AutoStreamSerializerBase() : base(AutoStreamSerializerConfig.Attribute.Version) { }

        /// <inheritdoc/>
        IAutoStreamSerializerConfig IAutoStreamSerializer.AutoStreamSerializerConfig => AutoStreamSerializerConfig;

        /// <inheritdoc/>
        protected override void Serialize(Stream stream) => AutoStreamSerializerConfig.Serialize((T)this, stream);

        /// <inheritdoc/>
        protected override async Task SerializeAsync(Stream stream, CancellationToken cancellationToken)
            => await AutoStreamSerializerConfig.SerializeAsync((T)this, stream, cancellationToken).DynamicContext();

        /// <inheritdoc/>
        protected override void Deserialize(Stream stream, int version) => AutoStreamSerializerConfig.Deserialize((T)this, stream, version);

        /// <inheritdoc/>
        protected override async Task DeserializeAsync(Stream stream, int version, CancellationToken cancellationToken)
            => await AutoStreamSerializerConfig.DeserializeAsync((T)this, stream, version, cancellationToken).DynamicContext();
    }
}
