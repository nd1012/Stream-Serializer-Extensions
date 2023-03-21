namespace wan24.StreamSerializerExtensions
{
    /// <summary>
    /// Serializer event arguments
    /// </summary>
    /// <typeparam name="T">Delegate type</typeparam>
    public class SerializerEventArgs<T> : EventArgs where T : Delegate
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">Type</param>
        public SerializerEventArgs(Type type) : base() => Type = type;

        /// <summary>
        /// Type
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Delegate
        /// </summary>
        public T? Delegate { get; set; }
    }
}
