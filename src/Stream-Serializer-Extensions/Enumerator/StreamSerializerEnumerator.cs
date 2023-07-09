namespace wan24.StreamSerializerExtensions.Enumerator
{
    /// <summary>
    /// Stream object enumerator
    /// </summary>
    /// <typeparam name="T">Object type</typeparam>
    public class StreamSerializerEnumerator<T> : StreamEnumeratorBase<T> where T : class, IStreamSerializer
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">Context</param>
        public StreamSerializerEnumerator(IDeserializationContext context) : base(context) { }

        /// <summary>
        /// Read object method
        /// </summary>
        /// <returns>Object</returns>
        protected override T ReadObject() => Context.Stream.ReadSerialized<T>(Context);

        /// <inheritdoc/>
        protected override void Dispose(bool disposing) { }
    }
}
