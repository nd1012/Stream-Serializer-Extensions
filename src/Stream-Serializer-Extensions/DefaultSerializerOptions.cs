using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    /// <summary>
    /// Default serializer options
    /// </summary>
    public class DefaultSerializerOptions : SerializerOptionsBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="property">Target property</param>
        /// <param name="attr">Stream serializer attribute (required, if <c>property</c> is <see langword="null"/>)</param>
        public DefaultSerializerOptions(PropertyInfoExt? property, StreamSerializerAttribute? attr = null) : base(property, attr) { }
    }
}
