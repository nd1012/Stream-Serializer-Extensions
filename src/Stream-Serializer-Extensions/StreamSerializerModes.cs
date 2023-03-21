namespace wan24.StreamSerializerExtensions
{
    /// <summary>
    /// Stream serializer modes
    /// </summary>
    public enum StreamSerializerModes
    {
        /// <summary>
        /// Automatic (depending on the type attributes serializer mode setting)
        /// </summary>
        Auto = 0,
        /// <summary>
        /// Opt-in
        /// </summary>
        OptOut = 1,
        /// <summary>
        /// Opt-out
        /// </summary>
        OptIn = 2
    }
}
