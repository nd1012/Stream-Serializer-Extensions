using System.Runtime;
using System.Runtime.CompilerServices;
using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    // Bytes
    public static partial class StreamExtensions
    {
        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Just a method adapter")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Stream WriteBytes(this Stream stream, Span<byte> value, ISerializationContext context) => WriteBytes(stream, (ReadOnlySpan<byte>)value, context);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream WriteBytes(this Stream stream, ReadOnlySpan<byte> value, ISerializationContext context)
        {
            try
            {
                WriteNumber(stream, value.Length, context);
                if (value.Length > 0) stream.Write(value);
                return stream;
            }
            catch (SerializerException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw SerializerException.From(ex);
            }
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Just a method adapter")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteBytesAsync(this Stream stream, Memory<byte> value, ISerializationContext context)
            => WriteBytesAsync(stream, (ReadOnlyMemory<byte>)value, context);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Just a method adapter")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteBytesAsync(this Task<Stream> stream, Memory<byte> value, ISerializationContext context)
            => AsyncHelper.FluentAsync(stream, value, context, WriteBytesAsync);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<Stream> WriteBytesAsync(this Stream stream, ReadOnlyMemory<byte> value, ISerializationContext context)
            => SerializerException.WrapAsync(async () =>
            {
                await WriteNumberAsync(stream, value.Length, context).DynamicContext();
                if (value.Length > 0) await stream.WriteAsync(value, context.Cancellation).DynamicContext();
                return stream;
            });

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteBytesAsync(this Task<Stream> stream, ReadOnlyMemory<byte> value, ISerializationContext context)
            => AsyncHelper.FluentAsync(stream, value, context, WriteBytesAsync);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream WriteBytesNullable(this Stream stream, byte[]? value, ISerializationContext context)
            => WriteNullableCount(context, value?.Length, () => SerializerException.Wrap(() => stream.Write(value)));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<Stream> WriteBytesNullableAsync(this Stream stream, byte[]? value, ISerializationContext context)
            => WriteNullableCountAsync(
                context,
                value?.Length,
                () => SerializerException.WrapAsync(() => stream.WriteAsync(value, context.Cancellation).AsTask())
                );

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteBytesNullableAsync(this Task<Stream> stream, byte[]? value, ISerializationContext context)
            => AsyncHelper.FluentAsync(stream, value, context, WriteBytesNullableAsync);
    }
}
