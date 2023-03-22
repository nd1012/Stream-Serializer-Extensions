namespace wan24.StreamSerializerExtensions
{
    /// <summary>
    /// Interface for a stream serializer enabled object which contains versioning information
    /// </summary>
    public interface IStreamSerializerVersion : IStreamSerializer
    {
        /// <summary>
        /// Object version (if <see langword="null"/>, the object serializer doesn't support object versioning)
        /// </summary>
        int? ObjectVersion { get; }
        /// <summary>
        /// Serialized object version (if <see langword="null"/>, the object serializer doesn't support object versioning, or the instance wasn't deserialized)
        /// </summary>
        int? SerializedObjectVersion { get; }
    }
}
