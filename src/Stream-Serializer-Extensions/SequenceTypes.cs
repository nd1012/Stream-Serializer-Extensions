namespace wan24.StreamSerializerExtensions
{
    /// <summary>
    /// Binary sequence types
    /// </summary>
    public enum SequenceTypes : byte
    {
        /// <summary>
        /// The object will be <see langword="null"/>
        /// </summary>
        Null = 0,
        /// <summary>
        /// The object was cached
        /// </summary>
        Cached = 32,
        /// <summary>
        /// Using a small index (lower or equal to <see cref="byte.MaxValue"/>)
        /// </summary>
        SmallIndex = 64,
        /// <summary>
        /// The object wasn't cached
        /// </summary>
        NotCached = 128,
        /// <summary>
        /// All sequence type values (for removing them from a <see cref="NumberTypes"/> or <see cref="ObjectTypes"/> value)
        /// </summary>
        ALL_VALUES = Cached | NotCached,
        /// <summary>
        /// All sequence type flags (including <see cref="SmallIndex"/>)
        /// </summary>
        ALL_FLAGS = ALL_VALUES | SmallIndex
    }
}
