namespace wan24.StreamSerializerExtensions
{
    /// <summary>
    /// Serializer types enumeration
    /// </summary>
    public enum SerializerTypes
    {
        /// <summary>
        /// Any type serializer
        /// </summary>
        Any,
        /// <summary>
        /// Any object
        /// </summary>
        AnyObject,
        /// <summary>
        /// Registered serializer
        /// </summary>
        Serializer,
        /// <summary>
        /// <see cref="IStreamSerializer"/> object
        /// </summary>
        StreamSerializer,
        /// <summary>
        /// Boolaen
        /// </summary>
        Bool,
        /// <summary>
        /// Number
        /// </summary>
        Number,
        /// <summary>
        /// Enumeration
        /// </summary>
        Enum,
        /// <summary>
        /// String (UTF-8)
        /// </summary>
        String,
        /// <summary>
        /// String (UTF-16)
        /// </summary>
        String16,
        /// <summary>
        /// String (UTF-32)
        /// </summary>
        String32,
        /// <summary>
        /// Byte array
        /// </summary>
        Bytes,
        /// <summary>
        /// Object array
        /// </summary>
        Array,
        /// <summary>
        /// List
        /// </summary>
        List,
        /// <summary>
        /// Dictionary
        /// </summary>
        Dictionary,
        /// <summary>
        /// Structure (using <c>Marshal</c>)
        /// </summary>
        Struct,
        /// <summary>
        /// Stream
        /// </summary>
        Stream,
        /// <summary>
        /// CLR type
        /// </summary>
        Type
    }
}
