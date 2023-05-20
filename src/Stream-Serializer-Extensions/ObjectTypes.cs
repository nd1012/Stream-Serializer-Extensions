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
        /// Boolean
        /// </summary>
        Bool = 1,
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
        /// Unsigned number
        /// </summary>
        Unsigned = 64,
        /// <summary>
        /// Empty (or zero)
        /// </summary>
        Empty = 128,
        /// <summary>
        /// All flags
        /// </summary>
        FLAGS = Unsigned | Empty
    }
}
