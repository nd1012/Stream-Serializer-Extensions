using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    /// <summary>
    /// Interface for serializer options
    /// </summary>
    public interface ISerializerOptions
    {
        /// <summary>
        /// Target property
        /// </summary>
        PropertyInfoExt? Property { get; }
        /// <summary>
        /// Stream serializer attribute
        /// </summary>
        StreamSerializerAttribute Attribute { get; }
        /// <summary>
        /// Serializer type
        /// </summary>
        SerializerTypes? Serializer { get; set; }
        /// <summary>
        /// Is the value nullable?
        /// </summary>
        bool IsNullable { get; set; }
        /// <summary>
        /// Key serializer options
        /// </summary>
        ISerializerOptions? KeyOptions { get; set; }
        /// <summary>
        /// Value serializer options
        /// </summary>
        ISerializerOptions? ValueOptions { get; set; }
        /// <summary>
        /// Get the minimum length
        /// </summary>
        /// <param name="defaultValue">Default value</param>
        /// <returns>Minimum length</returns>
        int GetMinLen(int defaultValue);
        /// <summary>
        /// Get the maximum length
        /// </summary>
        /// <param name="defaultValue">Default value</param>
        /// <returns>Maximum length</returns>
        int GetMaxLen(int defaultValue);
        /// <summary>
        /// Get the minimum length
        /// </summary>
        /// <param name="defaultValue">Default value</param>
        /// <returns>Minimum length</returns>
        long GetMinLen(long defaultValue);
        /// <summary>
        /// Get the maximum length
        /// </summary>
        /// <param name="defaultValue">Default value</param>
        /// <returns>Maximum length</returns>
        long GetMaxLen(long defaultValue);
    }
}
