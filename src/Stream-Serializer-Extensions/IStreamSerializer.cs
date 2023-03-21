namespace wan24.StreamSerializerExtensions
{
    /// <summary>
    /// Interface for a stream serializer object
    /// </summary>
    public interface IStreamSerializer
    {
        /// <summary>
        /// Object version (if <see langword="null"/>, the object serializer doesn't support object versioning)
        /// </summary>
        int? ObjectVersion { get; }
        /// <summary>
        /// Serialized object version (if <see langword="null"/>, the object serializer doesn't support object versioning, or the instance wasn't deserialized)
        /// </summary>
        int? SerializedObjectVersion { get; }
        /// <summary>
        /// Serialize
        /// </summary>
        /// <param name="stream">Stream</param>
        void Serialize(Stream stream);
        /// <summary>
        /// Serialize
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="cancellationToken">Cancellation token</param>
        Task SerializeAsync(Stream stream, CancellationToken cancellationToken);
        /// <summary>
        /// Deserialize
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        void Deserialize(Stream stream, int version);
        /// <summary>
        /// Deserialize
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        Task DeserializeAsync(Stream stream, int version, CancellationToken cancellationToken);
    }
}
