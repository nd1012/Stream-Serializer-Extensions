namespace wan24.StreamSerializerExtensions
{
    /// <summary>
    /// Object types
    /// </summary>
    [Flags]
    public enum ObjectTypes : byte
    {
        /// <summary>
        /// Null
        /// </summary>
        Null = 0,
        /// <summary>
        /// Boolean (boolean <see langword="true"/>)
        /// </summary>
        Bool = 1,
#pragma warning disable CA1069 // Double value
        /// <summary>
        /// <see langword="true"/>
        /// </summary>
        True = 1,
#pragma warning restore CA1069 // Double value
        /// <summary>
        /// Byte
        /// </summary>
        Byte = 2,
        /// <summary>
        /// Int16
        /// </summary>
        Short = 3,
        /// <summary>
        /// Int32
        /// </summary>
        Int = 4,
        /// <summary>
        /// Int64
        /// </summary>
        Long = 5,
        /// <summary>
        /// Single
        /// </summary>
        Float = 6,
        /// <summary>
        /// Double
        /// </summary>
        Double = 7,
        /// <summary>
        /// Decimal
        /// </summary>
        Decimal = 8,
        /// <summary>
        /// String
        /// </summary>
        String = 9,
        /// <summary>
        /// Array
        /// </summary>
        Array = 10,
        /// <summary>
        /// List
        /// </summary>
        List = 11,
        /// <summary>
        /// Dictionary
        /// </summary>
        Dict = 12,
        /// <summary>
        /// Object
        /// </summary>
        Object = 13,
        /// <summary>
        /// Serializable object
        /// </summary>
        Serializable = 14,
        /// <summary>
        /// Bytes
        /// </summary>
        Bytes = 15,
        /// <summary>
        /// Embedded stream
        /// </summary>
        Stream = 16,
        /// <summary>
        /// Struct
        /// </summary>
        Struct = 17,
        /// <summary>
        /// String UTF-16
        /// </summary>
        String16 = 18,
        /// <summary>
        /// String UTF-32
        /// </summary>
        String32 = 19,
        /// <summary>
        /// <see cref="Type"/>
        /// </summary>
        ClrType = 20,
        /// <summary>
        /// Using the object type from the cache, or the object from the object cache (if this flag is being used alone)
        /// </summary>
        Cached = 32,
        /// <summary>
        /// Unsigned number
        /// </summary>
        Unsigned = 64,
#pragma warning disable CA1069 // Double value
        /// <summary>
        /// No array rank
        /// </summary>
        NoRank = 64,
        /// <summary>
        /// Cached serializeable <see cref="IStreamSerializer"/> assignable <see cref="Type"/>
        /// </summary>
        CachedSerializable = 64,
        /// <summary>
        /// Is a basic <see cref="SerializedTypeInfo"/> (serialized to only one byte using <see cref="ObjectTypes"/>)
        /// </summary>
        BasicTypeInfo = 64,
#pragma warning restore CA1069 // Double value
        /// <summary>
        /// Empty (or zero or boolean <see langword="false"/>; or enumeration termination or last item type, if this flag is being used alone)
        /// </summary>
        Empty = 128,
#pragma warning disable CA1069 // Double value
        /// <summary>
        /// <see langword="false"/>
        /// </summary>
        False = 128,
        /// <summary>
        /// Use the last item type
        /// </summary>
        LastItemType = 128,
        /// <summary>
        /// Enumeration termination
        /// </summary>
        Break = 128,
        /// <summary>
        /// Generic type
        /// </summary>
        Generic = 128,
#pragma warning restore CA1069 // Double value
        /// <summary>
        /// All flags
        /// </summary>
        FLAGS = Unsigned | Empty | Cached
    }
}
