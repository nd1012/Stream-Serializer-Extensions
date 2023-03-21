namespace wan24.StreamSerializerExtensions
{
    /// <summary>
    /// Number type
    /// </summary>
    [Flags]
    public enum NumberTypes : byte
    {
        /// <summary>
        /// None
        /// </summary>
        None = 0,
        /// <summary>
        /// Byte
        /// </summary>
        Byte = 1,
        /// <summary>
        /// Short
        /// </summary>
        Short = 2,
        /// <summary>
        /// Integer
        /// </summary>
        Int = 3,
        /// <summary>
        /// Long
        /// </summary>
        Long = 4,
        /// <summary>
        /// Float
        /// </summary>
        Float = 5,
        /// <summary>
        /// Double
        /// </summary>
        Double = 6,
        /// <summary>
        /// Decimal
        /// </summary>
        Decimal = 7,
        /// <summary>
        /// Min. value
        /// </summary>
        MinValue = 32,
        /// <summary>
        /// Max. value
        /// </summary>
        MaxValue = 64,
        /// <summary>
        /// Unsigned?
        /// </summary>
        Unsigned = 128,
        /// <summary>
        /// Is zero?
        /// </summary>
        Zero = 128,
        /// <summary>
        /// All flags
        /// </summary>
        FLAGS = Unsigned,
        /// <summary>
        /// Value flags
        /// </summary>
        VALUE_FLAGS = MinValue | MaxValue
    }
}
