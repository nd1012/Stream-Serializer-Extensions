using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    /// <summary>
    /// Interface for an auto stream serializer configuration
    /// </summary>
    public interface IAutoStreamSerializerConfig
    {
        /// <summary>
        /// Serialized object type
        /// </summary>
        Type Type { get; }
        /// <summary>
        /// Stream serializer attribute
        /// </summary>
        StreamSerializerAttribute Attribute { get; }
        /// <summary>
        /// Auto stream serializer informations (key is the property name)
        /// </summary>
        OrderedDictionary<string, AutoStreamSerializerInfo> Infos { get; }
    }
}
