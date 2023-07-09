namespace wan24.StreamSerializerExtensions.Enumerator
{
    /// <summary>
    /// Stream string enumerator
    /// </summary>
    public class StreamStringEnumerator : StreamEnumeratorBase<string>
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
        public StreamStringEnumerator(IDeserializationContext context) : base(context) { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="minLen">Minimum UTF-8 string bytes length</param>
        /// <param name="maxLen">Maximum UTF-8 string bytes length</param>
        public StreamStringEnumerator(IDeserializationContext context, int minLen = 0, int maxLen = int.MaxValue) : base(context)
        {
            MinLen = minLen;
            MaxLen = maxLen;
        }

        /// <inheritdoc/>
        protected override string ReadObject() => Context.Stream.ReadString(Context, MinLen, MaxLen);
    }
}
