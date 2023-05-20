using System.Buffers;

namespace wan24.StreamSerializerExtensions.Enumerator
{
    /// <summary>
    /// Stream number enumerator
    /// </summary>
    /// <typeparam name="T">Numeric type</typeparam>
    public class StreamNumberEnumerator<T> : StreamEnumeratorBase<T> where T : struct, IConvertible
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
        public StreamNumberEnumerator(Stream stream, int? version = null) : base(stream, version) => Pool = StreamSerializer.BufferPool;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        public StreamNumberEnumerator(Stream stream, int? version = null, ArrayPool<byte>? pool = null) : base(stream, version) => Pool = pool ?? StreamSerializer.BufferPool;

        /// <inheritdoc/>
        protected override T ReadObject() => Stream.ReadNumber<T>(SerializerVersion, Pool);
    }
}
