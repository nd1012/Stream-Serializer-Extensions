namespace wan24.StreamSerializerExtensions
{
    /// <summary>
    /// Thrown on serialization errors
    /// </summary>
    public class SerializerException : Exception
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public SerializerException() : base() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Message</param>
        public SerializerException(string? message) : base(message) { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="inner">Inner exception</param>
        public SerializerException(string? message, Exception inner) : base(message, inner) { }
    }
}
