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
        /// <param name="context">Context</param>
        /// <param name="chunkLength">Chunk length in bytes</param>
        /// <returns>Stream</returns>
        public static Stream WriteStream(this Stream stream, Stream source, ISerializationContext context, int? chunkLength = null)
            => SerializerException.Wrap(() =>
            {
                if (chunkLength != null) ArgumentValidationHelper.EnsureValidArgument(nameof(chunkLength), 1, int.MaxValue, chunkLength.Value);
                long len = source.CanSeek ? source.Length - source.Position : 0 - (chunkLength ?? Settings.BufferSize);
                WriteNumber(stream, len, context);
                if (len != 0)
                    if (source.CanSeek)
                    {
                        source.CopyTo(stream, bufferSize: chunkLength ?? Settings.BufferSize);
                    }
                    else
                    {
                        using RentedArrayStruct<byte> buffer = new(len: chunkLength ?? Settings.BufferSize, StreamSerializer.BufferPool, clean: false);
                        for (int red = buffer.Length; red == buffer.Length;)
                        {
                            red = stream.Read(buffer.Span);
                            if (red < 1)
                            {
                                stream.WriteBytes(Array.Empty<byte>(), context);
                                break;
                            }
                            stream.WriteBytes(buffer.Span, context);
                        }
                    }
                return stream;
            });

        /// <summary>
        /// Write a stream
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="source">Source stream</param>
        /// <param name="context">Context</param>
        /// <param name="chunkLength">Chunk length in bytes</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream WriteStreamNullable(this Stream stream, Stream? source, ISerializationContext context, int? chunkLength = null)
            => source == null ? WriteNumber(stream, long.MinValue, context) : WriteStream(stream, source, context, chunkLength);

        /// <summary>
        /// Write a stream
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="source">Source stream</param>
        /// <param name="context">Context</param>
        /// <param name="chunkLength">Chunk length in bytes</param>
        /// <returns>Stream</returns>
        public static Task<Stream> WriteStreamAsync(
            this Stream stream,
            Stream source,
            ISerializationContext context,
            int? chunkLength = null
            )
            => SerializerException.WrapAsync(async () =>
            {
                if (chunkLength != null) ArgumentValidationHelper.EnsureValidArgument(nameof(chunkLength), 1, int.MaxValue, chunkLength.Value);
                long len = source.CanSeek ? source.Length - source.Position : 0 - (chunkLength ?? Settings.BufferSize);
                await WriteNumberAsync(stream, len, context).DynamicContext();
                if (len != 0)
                    if (source.CanSeek)
                    {
                        await source.CopyToAsync(stream, bufferSize: chunkLength ?? Settings.BufferSize, context.Cancellation).DynamicContext();
                    }
                    else
                    {
                        using RentedArrayStruct<byte> buffer = new(len: chunkLength ?? Settings.BufferSize, StreamSerializer.BufferPool, clean: false);
                        for (int red = buffer.Length; red == buffer.Length;)
                        {
                            red = await stream.ReadAsync(buffer.Memory, context.Cancellation).DynamicContext();
                            if (red < 1)
                            {
                                await stream.WriteBytesAsync(Array.Empty<byte>(), context).DynamicContext();
                                break;
                            }
                            await stream.WriteBytesAsync(buffer.Memory, context).DynamicContext();
                        }
                    }
                return stream;
            });

        /// <summary>
        /// Write a stream
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="source">Source stream</param>
        /// <param name="context">Context</param>
        /// <param name="chunkLength">Chunk length in bytes</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteStreamAsync(
            this Task<Stream> stream,
            Stream source,
            ISerializationContext context,
            int? chunkLength = null
            )
            => AsyncHelper.FluentAsync(stream, source, context, chunkLength, WriteStreamAsync);

        /// <summary>
        /// Write a stream
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="source">Source stream</param>
        /// <param name="context">Context</param>
        /// <param name="chunkLength">Chunk length in bytes</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<Stream> WriteStreamNullableAsync(
            this Stream stream,
            Stream? source,
            ISerializationContext context,
            int? chunkLength = null
            )
            => source == null
                ? WriteNumberAsync(stream, long.MinValue, context)
                : WriteStreamAsync(stream, source, context, chunkLength);

        /// <summary>
        /// Write a stream
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="source">Source stream</param>
        /// <param name="context">Context</param>
        /// <param name="chunkLength">Chunk length in bytes</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteStreamNullableAsync(
            this Task<Stream> stream,
            Stream? source,
            ISerializationContext context,
            int? chunkLength = null
            )
            => AsyncHelper.FluentAsync(stream, source, context, chunkLength, WriteStreamNullableAsync);
    }
}
