namespace wan24.StreamSerializerExtensions.Enumerator
{
    /// <summary>
    /// Stream number enumerator
    /// </summary>
    /// <typeparam name="T">Numeric type</typeparam>
    public class StreamNumberEnumerator<T> : StreamEnumeratorBase<T> where T : struct, IConvertible
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">Context</param>
        public StreamNumberEnumerator(IDeserializationContext context) : base(context) { }

        /// <inheritdoc/>
        protected override T ReadObject() => Context.Stream.ReadNumber<T>(Context);
    }
}
