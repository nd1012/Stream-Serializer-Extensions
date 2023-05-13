namespace wan24.StreamSerializerExtensions
{
    /// <summary>
    /// Interface for an auto stream serializer serializable object
    /// </summary>
    public interface IAutoStreamSerializer : IStreamSerializerVersion
    {
        /// <summary>
        /// Autostream serializer configuration
        /// </summary>
        IAutoStreamSerializerConfig AutoStreamSerializerConfig { get; }
    }
}
