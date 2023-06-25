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
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Just a method adapter")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Stream WriteBytes(this Stream stream, Span<byte> value) => WriteBytes(stream, (ReadOnlySpan<byte>)value);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream WriteBytes(this Stream stream, ReadOnlySpan<byte> value)
        {
            try
            {
                WriteNumber(stream, value.Length);
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
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Just a method adapter")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteBytesAsync(this Stream stream, Memory<byte> value, CancellationToken cancellationToken = default)
            => WriteBytesAsync(stream, (ReadOnlyMemory<byte>)value, cancellationToken);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Just a method adapter")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteBytesAsync(this Task<Stream> stream, Memory<byte> value, CancellationToken cancellationToken = default)
            => FluentAsync(stream, (s) => WriteBytesAsync(s, value, cancellationToken));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<Stream> WriteBytesAsync(this Stream stream, ReadOnlyMemory<byte> value, CancellationToken cancellationToken = default)
            => SerializerException.WrapAsync(async () =>
            {
                await WriteNumberAsync(stream, value.Length, cancellationToken).DynamicContext();
                if (value.Length > 0) await stream.WriteAsync(value, cancellationToken).DynamicContext();
                return stream;
            });

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteBytesAsync(this Task<Stream> stream, ReadOnlyMemory<byte> value, CancellationToken cancellationToken = default)
            => FluentAsync(stream, (s) => WriteBytesAsync(s, value, cancellationToken));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream WriteBytesNullable(this Stream stream, byte[]? value)
            => WriteNullableCount(stream, value?.Length, () => SerializerException.Wrap(() => stream.Write(value)));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<Stream> WriteBytesNullableAsync(this Stream stream, byte[]? value, CancellationToken cancellationToken = default)
            => WriteNullableCountAsync(
                stream,
                value?.Length,
                () => SerializerException.WrapAsync(() => stream.WriteAsync(value, cancellationToken).AsTask()),
                cancellationToken
                );

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteBytesNullableAsync(this Task<Stream> stream, byte[]? value, CancellationToken cancellationToken = default)
            => FluentAsync(stream, (s) => WriteBytesNullableAsync(s, value, cancellationToken));
    }
}
