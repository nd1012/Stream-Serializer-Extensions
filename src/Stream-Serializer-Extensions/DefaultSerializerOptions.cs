using System.Reflection;

namespace wan24.StreamSerializerExtensions
{
    /// <summary>
    /// Default serializer options
    /// </summary>
    /// <remarks>
    /// Constructor
    /// </remarks>
    /// <param name="property">Target property</param>
    /// <param name="attr">Stream serializer attribute (required, if <c>property</c> is <see langword="null"/>)</param>
    public class DefaultSerializerOptions(PropertyInfo? property, StreamSerializerAttribute? attr = null) : SerializerOptionsBase(property, attr)
    {
    }
}
