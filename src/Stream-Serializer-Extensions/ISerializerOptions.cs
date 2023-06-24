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
