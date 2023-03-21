namespace wan24.StreamSerializerExtensions
{
    /// <summary>
    /// Type loader event arguments
    /// </summary>
    public class TypeLoadEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public TypeLoadEventArgs(string name) : base() => Name = name;

        /// <summary>
        /// Requested type name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Type
        /// </summary>
        public Type? Type { get; set; }
    }
}
