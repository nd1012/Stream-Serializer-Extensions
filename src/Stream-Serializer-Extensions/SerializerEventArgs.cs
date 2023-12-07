namespace wan24.StreamSerializerExtensions
{
    /// <summary>
    /// Serializer event arguments
    /// </summary>
    /// <typeparam name="T">Delegate type</typeparam>
    /// <remarks>
    /// Constructor
    /// </remarks>
    /// <param name="type">Type</param>
    public class SerializerEventArgs<T>(Type type) : EventArgs() where T : Delegate
    {
        /// <summary>
        /// Type
        /// </summary>
        public Type Type { get; } = type;

        /// <summary>
        /// Delegate
        /// </summary>
        public T? Delegate { get; set; }
    }
}
