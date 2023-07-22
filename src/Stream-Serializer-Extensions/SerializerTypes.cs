namespace wan24.StreamSerializerExtensions
{
    /// <summary>
    /// Serializer types enumeration
    /// </summary>
    public enum SerializerTypes
    {
        /// <summary>
        /// Any type serializer (uses <see cref="StreamExtensions.WriteAny(System.IO.Stream, object, ISerializationContext)"/>)
        /// </summary>
        Any,
        /// <summary>
        /// Any object (uses <see cref="StreamExtensions.WriteAnyObject(System.IO.Stream, object, ISerializationContext)"/>)
        /// </summary>
        AnyObject,
        /// <summary>
        /// Registered serializer (<see cref="StreamSerializer.SyncSerializer"/> f.e.)
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
        /// CLR type (using <see cref="SerializedTypeInfo"/>)
        /// </summary>
        Type
    }
}
