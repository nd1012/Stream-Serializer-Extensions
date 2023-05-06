using System.Buffers;
using wan24.Core;

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
        protected readonly ArrayPool<byte>? Pool;
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
        public StreamStringAsyncEnumerator(Stream stream, int? version = null, CancellationToken cancellationToken = default) : base(stream, version, cancellationToken) => Pool = null;

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
            Pool = pool;
            MinLen = minLen;
            MaxLen = maxLen;
        }

        /// <inheritdoc/>
        protected override async Task<string> ReadObjectAsync() => await Stream.ReadStringAsync(SerializerVersion, Pool, MinLen, MaxLen, Cancellation).DynamicContext();
    }
}
