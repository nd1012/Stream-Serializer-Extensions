using System.Buffers;

namespace wan24.StreamSerializerExtensions.Enumerator
{
    /// <summary>
    /// Stream string enumerator
    /// </summary>
    public class StreamStringAsyncEnumerator : StreamAsyncEnumeratorBase<string>
    {
        /// <summary>
        /// Array pool
        /// </summary>
        protected readonly ArrayPool<byte> Pool;
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
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public StreamStringAsyncEnumerator(Stream stream, int? version = null, CancellationToken cancellationToken = default) : base(stream, version, cancellationToken)
            => Pool = StreamSerializer.BufferPool;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum UTF-8 string bytes length</param>
        /// <param name="maxLen">Maximum UTF-8 string bytes length</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public StreamStringAsyncEnumerator(
            Stream stream, 
            int? version = null, 
            ArrayPool<byte>? pool = null, 
            int minLen = 0, 
            int maxLen = int.MaxValue, 
            CancellationToken cancellationToken = default
            )
            : base(stream, version, cancellationToken)
        {
            Pool = pool ?? StreamSerializer.BufferPool;
            MinLen = minLen;
            MaxLen = maxLen;
        }

        /// <inheritdoc/>
        protected override Task<string> ReadObjectAsync() => Stream.ReadStringAsync(SerializerVersion, Pool, MinLen, MaxLen, Cancellation);
    }
}
