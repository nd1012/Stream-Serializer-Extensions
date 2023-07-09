using wan24.Core;
using wan24.ObjectValidation;

namespace wan24.StreamSerializerExtensions
{
    /// <summary>
    /// Base class for a stream serializing type
    /// </summary>
    public abstract class StreamSerializerBase : ValidatableObjectBase, IStreamSerializerVersion
    {
        /// <summary>
        /// Base object version
        /// </summary>
        public const int BASE_VERSION = 1;

        /// <summary>
        /// Object version
        /// </summary>
        private readonly int? _ObjectVersion;
        /// <summary>
        /// Serialized object version
        /// </summary>
        private int? _SerializedObjectVersion = null;
        /// <summary>
        /// Serializer version
        /// </summary>
        private int? _SerializerVersion = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="objectVersion">Object version</param>
        protected StreamSerializerBase(int? objectVersion = null) : base()
            => _ObjectVersion = objectVersion ?? GetType().GetCustomAttributeCached<StreamSerializerAttribute>()?.Version;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="objectVersion">Object version</param>
        protected StreamSerializerBase(IDeserializationContext context, int? objectVersion = null) : base()
        {
            _ObjectVersion = objectVersion;
            DeserializeInt(context);
        }

        /// <inheritdoc/>
        int? IStreamSerializerVersion.ObjectVersion => _ObjectVersion;

        /// <inheritdoc/>
        int? IStreamSerializerVersion.SerializedObjectVersion => _SerializedObjectVersion;

        /// <inheritdoc/>
        int? IStreamSerializerVersion.SerializerVersion => _SerializerVersion;

        /// <summary>
        /// Serialize
        /// </summary>
        /// <param name="context">Context</param>
        protected abstract void Serialize(ISerializationContext context);

        /// <summary>
        /// Serialize
        /// </summary>
        /// <param name="context">Context</param>
        protected virtual async Task SerializeAsync(ISerializationContext context)
        {
            await Task.Yield();
            Serialize(context);
        }

        /// <summary>
        /// Serialize
        /// </summary>
        /// <param name="context">Context</param>
        private void SerializeInt(ISerializationContext context)
        {
            context.Stream.WriteSerializerVersion(context)
                .WriteNumber(BASE_VERSION, context);
            if (_ObjectVersion != null) context.Stream.WriteNumber(_ObjectVersion.Value, context);
            Serialize(context);
        }

        /// <summary>
        /// Serialize
        /// </summary>
        /// <param name="context">Context</param>
        private async Task SerializeIntAsync(ISerializationContext context)
        {
            await context.Stream.WriteSerializerVersionAsync(context).DynamicContext();
            await context.Stream.WriteNumberAsync(BASE_VERSION, context).DynamicContext();
            if (_ObjectVersion != null)
                await context.Stream.WriteNumberAsync(_ObjectVersion.Value, context).DynamicContext();
            await SerializeAsync(context).DynamicContext();
        }

        /// <summary>
        /// Deserialize
        /// </summary>
        /// <param name="context">Context</param>
        protected abstract void Deserialize(IDeserializationContext context);

        /// <summary>
        /// Deserialize
        /// </summary>
        /// <param name="context">Context</param>
        protected virtual async Task DeserializeAsync(IDeserializationContext context)
        {
            await Task.Yield();
            Deserialize(context);
        }

        /// <summary>
        /// Deserialize
        /// </summary>
        /// <param name="context">Context</param>
        private void DeserializeInt(IDeserializationContext context)
        {
            _SerializerVersion = context.Stream.ReadSerializerVersion(context);
            using DeserializerContext objContext = new(context.Stream, _SerializedObjectVersion, context.CacheSize, context.Cancellation);
            int bv = context.Stream.ReadNumber<int>(objContext);
            if (bv < 1 || bv > BASE_VERSION) throw new SerializerException($"Invalid base object version {bv}", new InvalidDataException());
            if (_ObjectVersion != null) _SerializedObjectVersion = StreamSerializerAdapter.ReadSerializedObjectVersion(objContext, _ObjectVersion.Value);
            Deserialize(objContext);
        }

        /// <summary>
        /// Deserialize
        /// </summary>
        /// <param name="context">Context</param>
        private async Task DeserializeIntAsync(IDeserializationContext context)
        {
            _SerializerVersion = await context.Stream.ReadSerializerVersionAsync(context).DynamicContext();
            using DeserializerContext objContext = new(context.Stream, _SerializedObjectVersion, context.CacheSize, context.Cancellation);
            int bv = await context.Stream.ReadNumberAsync<int>(objContext).DynamicContext();
            if (bv < 1 || bv > BASE_VERSION) throw new SerializerException($"Invalid base object version {bv}", new InvalidDataException());
            if (_ObjectVersion != null)
                _SerializedObjectVersion = await StreamSerializerAdapter.ReadSerializedObjectVersionAsync(objContext, _ObjectVersion.Value)
                    .DynamicContext();
            await DeserializeAsync(objContext).DynamicContext();
        }

        /// <inheritdoc/>
        void IStreamSerializer.Serialize(ISerializationContext context) => SerializeInt(context);

        /// <inheritdoc/>
        Task IStreamSerializer.SerializeAsync(ISerializationContext context) => SerializeIntAsync(context);

        /// <inheritdoc/>
        void IStreamSerializer.Deserialize(IDeserializationContext context) => DeserializeInt(context);

        /// <inheritdoc/>
        Task IStreamSerializer.DeserializeAsync(IDeserializationContext context) => DeserializeIntAsync(context);
    }
}
