namespace wan24.StreamSerializerExtensions
{
    /// <summary>
    /// Type loader event arguments
    /// </summary>
    /// <remarks>
    /// Constructor
    /// </remarks>
    public class TypeLoadEventArgs(string name) : EventArgs()
    {
        /// <summary>
        /// Requested type name
        /// </summary>
        public string Name { get; } = name;

        /// <summary>
        /// Type
        /// </summary>
        public Type? Type { get; set; }
    }
}
