namespace wan24.StreamSerializerExtensions
{
    /// <summary>
    /// Base class for a stream serializing type
    /// </summary>
    public abstract class StreamSerializerBase : IStreamSerializer
    {
        /// <summary>
        /// Object version
        /// </summary>
        private readonly int? _ObjectVersion;
        /// <summary>
        /// Serialized object version
        /// </summary>
        private int? _SerializedObjectVersion = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="objectVersion">Object version</param>
        protected StreamSerializerBase(int? objectVersion = null) => _ObjectVersion = objectVersion;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="objectVersion">Object version</param>
        protected StreamSerializerBase(Stream stream, int version, int? objectVersion = null)
        {
            _ObjectVersion = objectVersion;
            Deserialize(stream, version);
        }

        /// <inheritdoc/>
        int? IStreamSerializer.ObjectVersion => _ObjectVersion;

        /// <inheritdoc/>
        int? IStreamSerializer.SerializedObjectVersion => _SerializedObjectVersion;

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
            if (_ObjectVersion != null) await stream.WriteNumberAsync(_ObjectVersion.Value, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
            await SerializeAsync(stream, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
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
        private void DeserializeInt(Stream stream, int version)
        {
            if (_ObjectVersion != null)
            {
                _SerializedObjectVersion = stream.ReadNumber<int>(version);
                if (_SerializedObjectVersion > _ObjectVersion) throw new SerializerException($"Unsupported object version {_SerializedObjectVersion} for {GetType()} version {_ObjectVersion}");
            }
            Deserialize(stream, version);
        }

        /// <summary>
        /// Deserialize
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        private async Task DeserializeIntAsync(Stream stream, int version, CancellationToken cancellationToken)
        {
            if (_ObjectVersion != null)
            {
                _SerializedObjectVersion = await stream.ReadNumberAsync<int>(version, cancellationToken: cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
                if (_SerializedObjectVersion > _ObjectVersion) throw new SerializerException($"Unsupported object version {_SerializedObjectVersion} for {GetType()} version {_ObjectVersion}");
            }
            await DeserializeAsync(stream, version, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
        }

        /// <inheritdoc/>
        void IStreamSerializer.Serialize(Stream stream) => SerializeInt(stream);

        /// <inheritdoc/>
        Task IStreamSerializer.SerializeAsync(Stream stream, CancellationToken cancellationToken) => SerializeIntAsync(stream, cancellationToken);

        /// <inheritdoc/>
        void IStreamSerializer.Deserialize(Stream stream, int version) => DeserializeInt(stream, version);

        /// <inheritdoc/>
        Task IStreamSerializer.DeserializeAsync(Stream stream, int version, CancellationToken cancellationToken) => DeserializeIntAsync(stream, version, cancellationToken);
    }
}
