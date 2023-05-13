using System.ComponentModel.DataAnnotations;
using System.Reflection;
using wan24.Core;
using wan24.ObjectValidation;

namespace wan24.StreamSerializerExtensions
{
    /// <summary>
    /// Base class for a disposable stream serializing type
    /// </summary>
    public abstract class DisposableStreamSerializerBase : DisposableBase, IStreamSerializerVersion, IObjectValidatable
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
        protected DisposableStreamSerializerBase(int? objectVersion = null) : base() => _ObjectVersion = objectVersion ?? GetType().GetCustomAttribute<StreamSerializerAttribute>()?.Version;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="objectVersion">Object version</param>
        protected DisposableStreamSerializerBase(Stream stream, int version, int? objectVersion = null) : base()
        {
            _ObjectVersion = objectVersion;
            Deserialize(stream, version);
        }

        /// <inheritdoc/>
        int? IStreamSerializerVersion.ObjectVersion => IfUndisposed(_ObjectVersion);

        /// <inheritdoc/>
        int? IStreamSerializerVersion.SerializedObjectVersion => IfUndisposed(_SerializedObjectVersion);

        /// <inheritdoc/>
        int? IStreamSerializerVersion.SerializerVersion => IfUndisposed(_SerializerVersion);

        /// <summary>
        /// Serialize
        /// </summary>
        /// <param name="stream">Stream</param>
        protected abstract void Serialize(Stream stream);

        /// <summary>
        /// Serialize
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="cancellationToken">Cancellation token</param>
        protected virtual async Task SerializeAsync(Stream stream, CancellationToken cancellationToken)
        {
            await Task.Yield();
            Serialize(stream);
        }

        /// <summary>
        /// Serialize
        /// </summary>
        /// <param name="stream">Stream</param>
        private void SerializeInt(Stream stream)
        {
            stream.WriteSerializerVersion()
                .WriteNumber(BASE_VERSION);
            if (_ObjectVersion != null) stream.WriteNumber(_ObjectVersion.Value);
            Serialize(stream);
        }

        /// <summary>
        /// Serialize
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="cancellationToken">Cancellation token</param>
        private async Task SerializeIntAsync(Stream stream, CancellationToken cancellationToken)
        {
            await stream.WriteSerializerVersionAsync(cancellationToken).DynamicContext();
            await stream.WriteNumberAsync(BASE_VERSION, cancellationToken).DynamicContext();
            if (_ObjectVersion != null) await stream.WriteNumberAsync(_ObjectVersion.Value, cancellationToken).DynamicContext();
            await SerializeAsync(stream, cancellationToken).DynamicContext();
        }

        /// <summary>
        /// Deserialize
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        protected abstract void Deserialize(Stream stream, int version);

        /// <summary>
        /// Deserialize
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        protected virtual async Task DeserializeAsync(Stream stream, int version, CancellationToken cancellationToken)
        {
            await Task.Yield();
            Deserialize(stream, version);
        }

        /// <summary>
        /// Deserialize
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
#pragma warning disable IDE0060 // Remove unused parameter (may be used later)
        private void DeserializeInt(Stream stream, int version)
#pragma warning restore IDE0060 // Remove unused parameter (may be used later)
        {
            _SerializerVersion = stream.ReadSerializerVersion();
            int bv = stream.ReadNumber<int>(_SerializerVersion.Value);
            if (bv < 1 || bv > BASE_VERSION) throw new SerializerException($"Invalid base object version {bv}", new InvalidDataException());
            if (_ObjectVersion != null) _SerializedObjectVersion = StreamSerializerAdapter.ReadSerializedObjectVersion(stream, _SerializerVersion.Value, _ObjectVersion.Value);
            Deserialize(stream, _SerializerVersion.Value);
        }

        /// <summary>
        /// Deserialize
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cancellationToken">Cancellation token</param>
#pragma warning disable IDE0060 // Remove unused parameter (may be used later)
        private async Task DeserializeIntAsync(Stream stream, int version, CancellationToken cancellationToken)
#pragma warning restore IDE0060 // Remove unused parameter (may be used later)
        {
            _SerializerVersion = await stream.ReadSerializerVersionAsync(cancellationToken).DynamicContext();
            int bv = await stream.ReadNumberAsync<int>(_SerializerVersion.Value, cancellationToken: cancellationToken).DynamicContext();
            if (bv < 1 || bv > BASE_VERSION) throw new SerializerException($"Invalid base object version {bv}", new InvalidDataException());
            if (_ObjectVersion != null)
                _SerializedObjectVersion = await StreamSerializerAdapter.ReadSerializedObjectVersionAsync(stream, _SerializerVersion.Value, _ObjectVersion.Value, cancellationToken)
                    .DynamicContext();
            await DeserializeAsync(stream, _SerializerVersion.Value, cancellationToken).DynamicContext();
        }

        /// <inheritdoc/>
        void IStreamSerializer.Serialize(Stream stream) => IfUndisposed(() => SerializeInt(stream));

        /// <inheritdoc/>
        Task IStreamSerializer.SerializeAsync(Stream stream, CancellationToken cancellationToken) => IfUndisposed(() => SerializeIntAsync(stream, cancellationToken));

        /// <inheritdoc/>
        void IStreamSerializer.Deserialize(Stream stream, int version) => IfUndisposed(() => DeserializeInt(stream, version));

        /// <inheritdoc/>
        Task IStreamSerializer.DeserializeAsync(Stream stream, int version, CancellationToken cancellationToken) => IfUndisposed(() => DeserializeIntAsync(stream, version, cancellationToken));

        /// <inheritdoc/>
        IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext) => IfUndisposed(() => ValidatableObject.ObjectValidatable(this));
    }
}
