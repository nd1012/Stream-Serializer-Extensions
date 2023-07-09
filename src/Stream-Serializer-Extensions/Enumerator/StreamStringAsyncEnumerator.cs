namespace wan24.StreamSerializerExtensions.Enumerator
{
    /// <summary>
    /// Stream string enumerator
    /// </summary>
    public class StreamStringAsyncEnumerator : StreamAsyncEnumeratorBase<string>
    {
        /// <summary>
        /// Minimum UTF-8 string bytes length
        /// </summary>
        protected readonly int MinLen = 0;
        /// <summary>
        /// Maximum UTF-8 string bytes length
        /// </summary>
        protected readonly int MaxLen = int.MaxValue;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">Context</param>
        public StreamStringAsyncEnumerator(IDeserializationContext context) : base(context) { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="minLen">Minimum UTF-8 string bytes length</param>
        /// <param name="maxLen">Maximum UTF-8 string bytes length</param>
        public StreamStringAsyncEnumerator(IDeserializationContext context, int minLen = 0, int maxLen = int.MaxValue) : base(context)
        {
            MinLen = minLen;
            MaxLen = maxLen;
        }

        /// <inheritdoc/>
        protected override Task<string> ReadObjectAsync() => Context.Stream.ReadStringAsync(Context, MinLen, MaxLen);
    }
}
