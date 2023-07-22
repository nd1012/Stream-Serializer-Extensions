namespace wan24.StreamSerializerExtensions.Enumerator
{
    /// <summary>
    /// Stream object enumerator
    /// </summary>
    /// <typeparam name="T">Object type</typeparam>
    public class StreamSerializerAsyncEnumerator<T> : StreamAsyncEnumeratorBase<T> where T : class, IStreamSerializer
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">Context</param>
        public StreamSerializerAsyncEnumerator(IDeserializationContext context) : base(context) { }

        /// <summary>
        /// Read object method
        /// </summary>
        /// <returns>Object</returns>
        protected override Task<T> ReadObjectAsync() => Context.Stream.ReadSerializedAsync<T>(Context);
    }
}
