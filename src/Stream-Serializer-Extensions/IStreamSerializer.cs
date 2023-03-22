namespace wan24.StreamSerializerExtensions
{
    /// <summary>
    /// Interface for a stream serializer object
    /// </summary>
    public interface IStreamSerializer
    {
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
