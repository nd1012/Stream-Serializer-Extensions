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
        /// <param name="context">Context</param>
        /// <param name="maxBufferSize">Maximum buffer size in bytes</param>
        /// <param name="minLen">Minimum stream length</param>
        /// <param name="maxLen">Maximum stream length in bytes</param>
        /// <returns>Target stream</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static Stream ReadStream(
            this Stream stream,
            Stream target,
            IDeserializationContext context,
            int? maxBufferSize = null,
            long minLen = 0,
            long maxLen = long.MaxValue
            )
#pragma warning restore IDE0060 // Remove unused argument
            => ReadStreamInt(context, target, maxBufferSize, minLen, maxLen, len: null);

        /// <summary>
        /// Read a stream
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="target">Target stream (the position won't be reset)</param>
        /// <param name="maxBufferSize">Maximum buffer size in bytes</param>
        /// <param name="minLen">Minimum stream length</param>
        /// <param name="maxLen">Maximum stream length in bytes</param>
        /// <param name="len">Stream/chunk length in bytes (chunk length is negative)</param>
        /// <returns>Target stream</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static Stream ReadStreamInt(
            IDeserializationContext context,
            Stream target,
            int? maxBufferSize,
            long minLen,
            long maxLen,
            long? len
            )
            => SerializerException.Wrap(() =>
            {
                ArgumentValidationHelper.EnsureValidArgument(nameof(target), target.CanWrite, () => "Writable stream required");
                if (maxBufferSize != null) ArgumentValidationHelper.EnsureValidArgument(nameof(maxBufferSize), 1, int.MaxValue, maxBufferSize.Value);
                ArgumentValidationHelper.EnsureValidArgument(nameof(minLen), 0, long.MaxValue, minLen);
                ArgumentValidationHelper.EnsureValidArgument(nameof(maxLen), minLen, long.MaxValue, maxLen);
                len ??= context.Stream.ReadNumber<long>(context);
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
                    using RentedArrayStruct<byte> buffer = new((int)len, context.BufferPool, clean: false);
                    long total = 0;
                    for (int red = (int)len; red == len; total += red)
                    {
                        red = context.Stream.ReadBytes(context, maxLen: buffer.Length).Length;
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
                    using RentedArrayStruct<byte> buffer = new(maxBufferSize ?? Settings.BufferSize, context.BufferPool, clean: false);
                    long total = 0;
                    for (int red = buffer.Length; red == buffer.Length && total < len; total += red)
                    {
                        red = context.Stream.Read(buffer.Span[..(int)Math.Min(buffer.Length, len.Value - total)]);
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
        /// <param name="context">Context</param>
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
            IDeserializationContext context,
            int? maxBufferSize = null,
            long minLen = 0,
            long maxLen = long.MaxValue
            )
            => SerializerException.Wrap(() =>
            {
                switch (context.SerializerVersion)// Serializer version switch
                {
                    case 1:
                        {
                            if (!ReadBool(stream, context))
                            {
                                target.Dispose();
                                return null;
                            }
                            return ReadStream(stream, target, context, maxBufferSize, minLen, maxLen);
                        }
                    default:
                        {
                            long len = ReadNumber<long>(stream, context);
                            if (len == long.MinValue)
                            {
                                target.Dispose();
                                return null;
                            }
                            return ReadStreamInt(context, target, maxBufferSize, minLen, maxLen, len);
                        }
                }
            });

        /// <summary>
        /// Read a stream
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="target">Target stream (the position won't be reset)</param>
        /// <param name="context">Context</param>
        /// <param name="maxBufferSize">Maximum buffer size in bytes</param>
        /// <param name="minLen">Minimum stream length</param>
        /// <param name="maxLen">Maximum stream length in bytes</param>
        /// <returns>Target stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
#pragma warning disable IDE0060 // Remove unused argument
        public static Task<Stream> ReadStreamAsync(
            this Stream stream,
            Stream target,
            IDeserializationContext context,
            int? maxBufferSize = null,
            long minLen = 0,
            long maxLen = long.MaxValue
            )
#pragma warning restore IDE0060 // Remove unused argument
            => ReadStreamIntAsync(context, target, maxBufferSize, minLen, maxLen, len: null);

        /// <summary>
        /// Read a stream
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="target">Target stream (the position won't be reset)</param>
        /// <param name="maxBufferSize">Maximum buffer size in bytes</param>
        /// <param name="minLen">Minimum stream length</param>
        /// <param name="maxLen">Maximum stream length in bytes</param>
        /// <param name="len">Stream/chunk length in bytes (chunk length is negative)</param>
        /// <returns>Target stream</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static Task<Stream> ReadStreamIntAsync(
            IDeserializationContext context,
            Stream target,
            int? maxBufferSize,
            long minLen,
            long maxLen,
            long? len
            )
            => SerializerException.WrapAsync(async () =>
            {
                ArgumentValidationHelper.EnsureValidArgument(nameof(target), target.CanWrite, () => "Writable stream required");
                if (maxBufferSize != null) ArgumentValidationHelper.EnsureValidArgument(nameof(maxBufferSize), 1, int.MaxValue, maxBufferSize.Value);
                ArgumentValidationHelper.EnsureValidArgument(nameof(minLen), 0, long.MaxValue, minLen);
                ArgumentValidationHelper.EnsureValidArgument(nameof(maxLen), minLen, long.MaxValue, maxLen);
                len ??= await context.Stream.ReadNumberAsync<long>(context).DynamicContext();
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
                    using RentedArrayStruct<byte> buffer = new((int)len, context.BufferPool, clean: false);
                    long total = 0;
                    for (int red = (int)len; red == len; total += red)
                    {
                        red = (await context.Stream.ReadBytesAsync(context, maxLen: buffer.Length).DynamicContext()).Length;
                        if (total + red > maxLen) throw new SerializerException($"The embedded stream length exceeds the maximum of {maxLen} bytes", new OverflowException());
                        if (red < 1) break;
                        await target.WriteAsync(buffer.Memory[..red], context.Cancellation).DynamicContext();
                    }
                    if (total < minLen) throw new SerializerException($"The stream length doesn't fit the minimum length of {minLen} bytes", new InvalidDataException());
                }
                else
                {
                    // Fixed length
                    if (len < minLen) throw new SerializerException($"The stream length doesn't fit the minimum length of {minLen} bytes", new InvalidDataException());
                    if (len > maxLen)
                        throw new SerializerException($"Embedded stream length of {len} bytes exceeds the maximum stream length of {maxLen} bytes", new OverflowException());
                    using RentedArrayStruct<byte> buffer = new(maxBufferSize ?? Settings.BufferSize, context.BufferPool, clean: false);
                    long total = 0;
                    for (int red = buffer.Length; red == buffer.Length && total < len; total += red)
                    {
                        red = await context.Stream.ReadAsync(buffer.Memory[..(int)Math.Min(buffer.Length, len.Value - total)], context.Cancellation).DynamicContext();
                        if (red < 1) break;
                        await target.WriteAsync(buffer.Memory[..red], context.Cancellation).DynamicContext();
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
        /// <param name="context">Context</param>
        /// <param name="maxBufferSize">Maximum buffer size in bytes</param>
        /// <param name="minLen">Minimum stream length</param>
        /// <param name="maxLen">Maximum stream length in bytes</param>
        /// <returns>Target stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<Stream?> ReadStreamNullableAsync(
            this Stream stream,
            Stream target,
            IDeserializationContext context,
            int? maxBufferSize = null,
            long minLen = 0,
            long maxLen = long.MaxValue
            )
            => SerializerException.WrapAsync(async () =>
            {
                switch (context.SerializerVersion)// Serializer version switch
                {
                    case 1:
                        {
                            if (!await ReadBoolAsync(stream, context).DynamicContext())
                            {
                                await target.DisposeAsync().DynamicContext();
                                return null;
                            }
                            return await ReadStreamAsync(stream, target, context, maxBufferSize, minLen, maxLen).DynamicContext();
                        }
                    default:
                        {
                            long len = await ReadNumberAsync<long>(stream, context).DynamicContext();
                            if (len == long.MinValue)
                            {
                                await target.DisposeAsync().DynamicContext();
                                return null;
                            }
                            return await ReadStreamIntAsync(context, target, maxBufferSize, minLen, maxLen, len).DynamicContext();
                        }
                }
            });
    }
}
