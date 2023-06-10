using System.Runtime;
using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    // Bytes
    public static partial class StreamExtensions
    {
        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Stream type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Just a method adapter")]
        public static T WriteBytes<T>(this T stream, Span<byte> value) where T : Stream => WriteBytes(stream, (ReadOnlySpan<byte>)value);

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Stream type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static T WriteBytes<T>(this T stream, ReadOnlySpan<byte> value) where T : Stream
        {
            try
            {
                WriteNumber(stream, value.Length);
                if (value.Length > 0) stream.Write(value);
                return stream;
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        [TargetedPatchingOptOut("Just a method adapter")]
        public static Task WriteBytesAsync(this Stream stream, Memory<byte> value, CancellationToken cancellationToken = default)
            => WriteBytesAsync(stream, (ReadOnlyMemory<byte>)value, cancellationToken);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        [TargetedPatchingOptOut("Tiny method")]
        public static async Task WriteBytesAsync(this Stream stream, ReadOnlyMemory<byte> value, CancellationToken cancellationToken = default)
        {
            try
            {
                await WriteNumberAsync(stream, value.Length, cancellationToken).DynamicContext();
                if (value.Length > 0) await stream.WriteAsync(value, cancellationToken).DynamicContext();
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Stream type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static T WriteBytesNullable<T>(this T stream, byte[]? value) where T : Stream
        {
            Write(stream, value != null);
            if (value != null) WriteBytes(stream, value);
            return stream;
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        [TargetedPatchingOptOut("Tiny method")]
        public static async Task WriteBytesNullableAsync(this Stream stream, byte[]? value, CancellationToken cancellationToken = default)
        {
            await WriteAsync(stream, value != null, cancellationToken).DynamicContext();
            if (value != null) await WriteBytesAsync(stream, value, cancellationToken).DynamicContext();
        }
    }
}
