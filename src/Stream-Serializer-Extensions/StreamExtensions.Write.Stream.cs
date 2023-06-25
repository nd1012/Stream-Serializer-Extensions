using System.Buffers;
using System.Runtime;
using System.Runtime.CompilerServices;
using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    // Stream
    public static partial class StreamExtensions
    {
        /// <summary>
        /// Write a stream
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="source">Source stream</param>
        /// <param name="pool">Array pool</param>
        /// <param name="chunkLength">Chunk length in bytes</param>
        /// <returns>Stream</returns>
        public static Stream WriteStream(this Stream stream, Stream source, ArrayPool<byte>? pool = null, int? chunkLength = null)
            => SerializerException.Wrap(() =>
            {
                if (chunkLength != null && chunkLength.Value < 1) throw new ArgumentOutOfRangeException(nameof(chunkLength));
                long len = source.CanSeek ? source.Length - source.Position : 0 - (chunkLength ?? Settings.BufferSize);
                WriteNumber(stream, len);
                if (len != 0)
                    if (source.CanSeek)
                    {
                        source.CopyTo(stream, bufferSize: chunkLength ?? Settings.BufferSize);
                    }
                    else
                    {
                        using RentedArray<byte> buffer = new(len: chunkLength ?? Settings.BufferSize, pool ?? StreamSerializer.BufferPool, clean: false);
                        for (int red = buffer.Length; red == buffer.Length;)
                        {
                            red = stream.Read(buffer.Span);
                            if (red < 1)
                            {
                                stream.WriteBytes(Array.Empty<byte>());
                                break;
                            }
                            stream.WriteBytes(buffer.Span);
                        }
                    }
                return stream;
            });

        /// <summary>
        /// Write a stream
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="source">Source stream</param>
        /// <param name="pool">Array pool</param>
        /// <param name="chunkLength">Chunk length in bytes</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream WriteStreamNullable(this Stream stream, Stream? source, ArrayPool<byte>? pool = null, int? chunkLength = null)
            => source == null ? WriteNumber(stream, long.MinValue) : WriteStream(stream, source, pool, chunkLength);

        /// <summary>
        /// Write a stream
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="source">Source stream</param>
        /// <param name="pool">Array pool</param>
        /// <param name="chunkLength">Chunk length in bytes</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        public static Task<Stream> WriteStreamAsync(
            this Stream stream,
            Stream source,
            ArrayPool<byte>? pool = null,
            int? chunkLength = null,
            CancellationToken cancellationToken = default
            )
            => SerializerException.WrapAsync(async () =>
            {
                if (chunkLength != null && chunkLength.Value < 1) throw new ArgumentOutOfRangeException(nameof(chunkLength));
                long len = source.CanSeek ? source.Length - source.Position : 0 - (chunkLength ?? Settings.BufferSize);
                await WriteNumberAsync(stream, len, cancellationToken).DynamicContext();
                if (len != 0)
                    if (source.CanSeek)
                    {
                        await source.CopyToAsync(stream, bufferSize: chunkLength ?? Settings.BufferSize, cancellationToken).DynamicContext();
                    }
                    else
                    {
                        using RentedArray<byte> buffer = new(len: chunkLength ?? Settings.BufferSize, pool ?? StreamSerializer.BufferPool, clean: false);
                        for (int red = buffer.Length; red == buffer.Length;)
                        {
                            red = await stream.ReadAsync(buffer.Memory, cancellationToken).DynamicContext();
                            if (red < 1)
                            {
                                await stream.WriteBytesAsync(Array.Empty<byte>(), cancellationToken).DynamicContext();
                                break;
                            }
                            await stream.WriteBytesAsync(buffer.Memory, cancellationToken).DynamicContext();
                        }
                    }
                return stream;
            });

        /// <summary>
        /// Write a stream
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="source">Source stream</param>
        /// <param name="pool">Array pool</param>
        /// <param name="chunkLength">Chunk length in bytes</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteStreamAsync(
            this Task<Stream> stream,
            Stream source,
            ArrayPool<byte>? pool = null,
            int? chunkLength = null,
            CancellationToken cancellationToken = default
            )
            => FluentAsync(stream, (s) => WriteStreamAsync(s, source, pool, chunkLength, cancellationToken));

        /// <summary>
        /// Write a stream
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="source">Source stream</param>
        /// <param name="pool">Array pool</param>
        /// <param name="chunkLength">Chunk length in bytes</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<Stream> WriteStreamNullableAsync(
            this Stream stream,
            Stream? source,
            ArrayPool<byte>? pool = null,
            int? chunkLength = null,
            CancellationToken cancellationToken = default
            )
            => source == null
                ? WriteNumberAsync(stream, long.MinValue, cancellationToken)
                : WriteStreamAsync(stream, source, pool, chunkLength, cancellationToken);

        /// <summary>
        /// Write a stream
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="source">Source stream</param>
        /// <param name="pool">Array pool</param>
        /// <param name="chunkLength">Chunk length in bytes</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteStreamNullableAsync(
            this Task<Stream> stream,
            Stream? source,
            ArrayPool<byte>? pool = null,
            int? chunkLength = null,
            CancellationToken cancellationToken = default
            )
            => FluentAsync(stream, (s) => WriteStreamNullableAsync(s, source, pool, chunkLength, cancellationToken));
    }
}
