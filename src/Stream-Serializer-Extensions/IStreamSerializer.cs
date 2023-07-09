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
        /// <param name="context">Context</param>
        void Serialize(ISerializationContext context);
        /// <summary>
        /// Serialize
        /// </summary>
        /// <param name="context">Context</param>
        Task SerializeAsync(ISerializationContext context);
        /// <summary>
        /// Deserialize
        /// </summary>
        /// <param name="context">Context</param>
        void Deserialize(IDeserializationContext context);
        /// <summary>
        /// Deserialize
        /// </summary>
        /// <param name="context">Context</param>
        Task DeserializeAsync(IDeserializationContext context);
    }
}
