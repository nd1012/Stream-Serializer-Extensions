using System.Buffers;

namespace wan24.StreamSerializerExtensions.Enumerator
{
    /// <summary>
    /// Stream number enumerator
    /// </summary>
    /// <typeparam name="T">Numeric type</typeparam>
    public class StreamNumberAsyncEnumerator<T> : StreamAsyncEnumeratorBase<T> where T : struct, IConvertible
    {
        /// <summary>
        /// Array pool
        /// </summary>
        protected readonly ArrayPool<byte> Pool;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public StreamNumberAsyncEnumerator(Stream stream, int? version = null, CancellationToken cancellationToken = default) : base(stream, version, cancellationToken)
            => Pool = StreamSerializer.BufferPool;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public StreamNumberAsyncEnumerator(Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default) 
            : base(stream, version, cancellationToken)
            => Pool = pool ?? StreamSerializer.BufferPool;

        /// <inheritdoc/>
        protected override Task<T> ReadObjectAsync() => Stream.ReadNumberAsync<T>(SerializerVersion, Pool, Cancellation);
    }
}
