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
        /// Read a stream
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="target">Target stream (the position won't be reset)</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="maxBufferSize">Maximum buffer size in bytes</param>
        /// <param name="minLen">Minimum stream length</param>
        /// <param name="maxLen">Maximum stream length in bytes</param>
        /// <returns>Target stream</returns>
        public static Stream ReadStream(
            this Stream stream,
            Stream target,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int? maxBufferSize = null,
            long minLen = 0,
            long maxLen = long.MaxValue
            )
            => ReadStreamInt(stream, target, version, pool, maxBufferSize, minLen, maxLen, len: null);

        /// <summary>
        /// Read a stream
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="target">Target stream (the position won't be reset)</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="maxBufferSize">Maximum buffer size in bytes</param>
        /// <param name="minLen">Minimum stream length</param>
        /// <param name="maxLen">Maximum stream length in bytes</param>
        /// <param name="len">Stream/chunk length in bytes (chunk length is negative)</param>
        /// <returns>Target stream</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static Stream ReadStreamInt(
            this Stream stream,
            Stream target,
            int? version,
            ArrayPool<byte>? pool,
            int? maxBufferSize,
            long minLen,
            long maxLen,
            long? len
            )
            => SerializerException.Wrap(() =>
            {
                if (!target.CanWrite) throw new ArgumentException("Writable stream required", nameof(target));
                if (maxBufferSize != null && maxBufferSize.Value < 1) throw new ArgumentOutOfRangeException(nameof(maxBufferSize));
                if (minLen < 0) throw new ArgumentOutOfRangeException(nameof(minLen));
                if (maxLen < 0 || maxLen < minLen) throw new ArgumentOutOfRangeException(nameof(maxLen));
                len ??= stream.ReadNumber<long>(version, pool);
                if (len == 0)
                {
                    if (len < minLen) throw new SerializerException($"The stream length doesn't fit the minimum length of {minLen} bytes", new InvalidDataException());
                    return target;
                }
                if (len < 0)
                {
                    // Chunked
                    len = Math.Abs(len.Value);
                    if (len > int.MaxValue) throw new SerializerException($"Invalid chunk length {len}", new InvalidDataException());
                    if (len > (maxBufferSize ?? Settings.BufferSize))
                        throw new SerializerException($"Chunk length of {len} bytes exceeds max. buffer size of {maxBufferSize ?? Settings.BufferSize}", new InvalidDataException());
                    using RentedArray<byte> buffer = new((int)len, pool ?? StreamSerializer.BufferPool, clean: false);
                    long total = 0;
                    for (int red = (int)len; red == len; total += red)
                    {
                        red = stream.ReadBytes(version, buffer.Array, maxLen: buffer.Length).Length;
                        if (total + red > maxLen) throw new SerializerException($"The embedded stream length exceeds the maximum of {maxLen} bytes", new OverflowException());
                        if (red < 1) break;
                        target.Write(buffer.Span[..red]);
                    }
                    if (total < minLen) throw new SerializerException($"The stream length doesn't fit the minimum length of {minLen} bytes", new InvalidDataException());
                }
                else
                {
                    // Fixed length
                    if (len < minLen) throw new SerializerException($"The stream length doesn't fit the minimum length of {minLen} bytes", new InvalidDataException());
                    if (len > maxLen)
                        throw new SerializerException($"Embedded stream length of {len} bytes exceeds the maximum stream length of {maxLen} bytes", new OverflowException());
                    using RentedArray<byte> buffer = new(maxBufferSize ?? Settings.BufferSize, pool ?? StreamSerializer.BufferPool, clean: false);
                    long total = 0;
                    for (int red = buffer.Length; red == buffer.Length && total < len; total += red)
                    {
                        red = stream.Read(buffer.Span[..(int)Math.Min(buffer.Length, len.Value - total)]);
                        if (red < 1) break;
                        target.Write(buffer.Span[..red]);
                    }
                    if (total != len) throw new SerializerException($"Invalid stream length ({len} bytes proposed, {total} bytes red)", new IOException());
                }
                return target;
            });

        /// <summary>
        /// Read a stream
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="target">Target stream (the position won't be reset; will be disposed, if the value is <see langword="null"/>)</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="maxBufferSize">Maximum buffer size in bytes</param>
        /// <param name="minLen">Minimum stream length</param>
        /// <param name="maxLen">Maximum stream length in bytes</param>
        /// <returns>Target stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream? ReadStreamNullable(
            this Stream stream,
            Stream target,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int? maxBufferSize = null,
            long minLen = 0,
            long maxLen = long.MaxValue
            )
            => SerializerException.Wrap(() =>
            {
                switch ((version ??= StreamSerializer.VERSION) & byte.MaxValue)// Serializer version switch
                {
                    case 1:
                        {
                            if (!ReadBool(stream, version, pool))
                            {
                                target.Dispose();
                                return null;
                            }
                            return ReadStream(stream, target, version, pool, maxBufferSize, minLen, maxLen);
                        }
                    default:
                        {
                            long len = ReadNumber<long>(stream, version, pool);
                            if (len == long.MinValue)
                            {
                                target.Dispose();
                                return null;
                            }
                            return ReadStreamInt(stream, target, version, pool, maxBufferSize, minLen, maxLen, len);
                        }
                }
            });

        /// <summary>
        /// Read a stream
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="target">Target stream (the position won't be reset)</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="maxBufferSize">Maximum buffer size in bytes</param>
        /// <param name="minLen">Minimum stream length</param>
        /// <param name="maxLen">Maximum stream length in bytes</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Target stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<Stream> ReadStreamAsync(
            this Stream stream,
            Stream target,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int? maxBufferSize = null,
            long minLen = 0,
            long maxLen = long.MaxValue,
            CancellationToken cancellationToken = default
            )
            => ReadStreamIntAsync(stream, target, version, pool, maxBufferSize, minLen, maxLen, len: null, cancellationToken);

        /// <summary>
        /// Read a stream
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="target">Target stream (the position won't be reset)</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="maxBufferSize">Maximum buffer size in bytes</param>
        /// <param name="minLen">Minimum stream length</param>
        /// <param name="maxLen">Maximum stream length in bytes</param>
        /// <param name="len">Stream/chunk length in bytes (chunk length is negative)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Target stream</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static Task<Stream> ReadStreamIntAsync(
            Stream stream,
            Stream target,
            int? version,
            ArrayPool<byte>? pool,
            int? maxBufferSize,
            long minLen,
            long maxLen,
            long? len,
            CancellationToken cancellationToken
            )
            => SerializerException.WrapAsync(async () =>
            {
                if (!target.CanWrite) throw new ArgumentException("Writable stream required", nameof(target));
                if (maxBufferSize != null && maxBufferSize.Value < 1) throw new ArgumentOutOfRangeException(nameof(maxBufferSize));
                if (minLen < 0) throw new ArgumentOutOfRangeException(nameof(minLen));
                if (maxLen < 0 || maxLen < minLen) throw new ArgumentOutOfRangeException(nameof(maxLen));
                len ??= await stream.ReadNumberAsync<long>(version, pool, cancellationToken).DynamicContext();
                if (len == 0)
                {
                    if (len < minLen) throw new SerializerException($"The stream length doesn't fit the minimum length of {minLen} bytes", new InvalidDataException());
                    return target;
                }
                if (len < 0)
                {
                    // Chunked
                    len = Math.Abs(len.Value);
                    if (len > int.MaxValue) throw new SerializerException($"Invalid chunk length {len}", new InvalidDataException());
                    if (len > (maxBufferSize ?? Settings.BufferSize))
                        throw new SerializerException($"Chunk length of {len} bytes exceeds max. buffer size of {maxBufferSize ?? Settings.BufferSize}", new InvalidDataException());
                    using RentedArray<byte> buffer = new((int)len, pool ?? StreamSerializer.BufferPool, clean: false);
                    long total = 0;
                    for (int red = (int)len; red == len; total += red)
                    {
                        red = (await stream.ReadBytesAsync(version, buffer.Array, maxLen: buffer.Length, cancellationToken: cancellationToken).DynamicContext()).Length;
                        if (total + red > maxLen) throw new SerializerException($"The embedded stream length exceeds the maximum of {maxLen} bytes", new OverflowException());
                        if (red < 1) break;
                        await target.WriteAsync(buffer.Memory[..red], cancellationToken).DynamicContext();
                    }
                    if (total < minLen) throw new SerializerException($"The stream length doesn't fit the minimum length of {minLen} bytes", new InvalidDataException());
                }
                else
                {
                    // Fixed length
                    if (len < minLen) throw new SerializerException($"The stream length doesn't fit the minimum length of {minLen} bytes", new InvalidDataException());
                    if (len > maxLen)
                        throw new SerializerException($"Embedded stream length of {len} bytes exceeds the maximum stream length of {maxLen} bytes", new OverflowException());
                    using RentedArray<byte> buffer = new(maxBufferSize ?? Settings.BufferSize, pool ?? StreamSerializer.BufferPool, clean: false);
                    long total = 0;
                    for (int red = buffer.Length; red == buffer.Length && total < len; total += red)
                    {
                        red = await stream.ReadAsync(buffer.Memory[..(int)Math.Min(buffer.Length, len.Value - total)], cancellationToken: cancellationToken).DynamicContext();
                        if (red < 1) break;
                        await target.WriteAsync(buffer.Memory[..red], cancellationToken).DynamicContext();
                    }
                    if (total != len) throw new SerializerException($"Invalid stream length ({len} bytes proposed, {total} bytes red)", new IOException());
                }
                return target;
            });

        /// <summary>
        /// Read a stream
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="target">Target stream (the position won't be reset; will be disposed, if the value is <see langword="null"/>)</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="maxBufferSize">Maximum buffer size in bytes</param>
        /// <param name="minLen">Minimum stream length</param>
        /// <param name="maxLen">Maximum stream length in bytes</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Target stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<Stream?> ReadStreamNullableAsync<T>(
            this Stream stream,
            Stream target,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int? maxBufferSize = null,
            long minLen = 0,
            long maxLen = long.MaxValue,
            CancellationToken cancellationToken = default
            )
            => SerializerException.WrapAsync(async () =>
            {
                switch ((version ??= StreamSerializer.VERSION) & byte.MaxValue)// Serializer version switch
                {
                    case 1:
                        {
                            if (!await ReadBoolAsync(stream, version, pool, cancellationToken).DynamicContext())
                            {
                                await target.DisposeAsync().DynamicContext();
                                return null;
                            }
                            return await ReadStreamAsync(stream, target, version, pool, maxBufferSize, minLen, maxLen, cancellationToken).DynamicContext();
                        }
                    default:
                        {
                            long len = await ReadNumberAsync<long>(stream, version, pool, cancellationToken).DynamicContext();
                            if (len == long.MinValue)
                            {
                                await target.DisposeAsync().DynamicContext();
                                return null;
                            }
                            return await ReadStreamIntAsync(stream, target, version, pool, maxBufferSize, minLen, maxLen, len, cancellationToken).DynamicContext();
                        }
                }
            });
    }
}
