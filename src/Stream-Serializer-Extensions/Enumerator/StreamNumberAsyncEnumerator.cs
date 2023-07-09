namespace wan24.StreamSerializerExtensions.Enumerator
{
    /// <summary>
    /// Stream number enumerator
    /// </summary>
    /// <typeparam name="T">Numeric type</typeparam>
    public class StreamNumberAsyncEnumerator<T> : StreamAsyncEnumeratorBase<T> where T : struct, IConvertible
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">Context</param>
        public StreamNumberAsyncEnumerator(IDeserializationContext context) : base(context) { }

        /// <inheritdoc/>
        protected override Task<T> ReadObjectAsync() => Context.Stream.ReadNumberAsync<T>(Context);
    }
}
