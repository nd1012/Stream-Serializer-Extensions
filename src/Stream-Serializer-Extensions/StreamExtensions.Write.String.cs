using System.Runtime;
using System.Text;
using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    // String
    public static partial class StreamExtensions
    {
        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Stream type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static T WriteString<T>(this T stream, string value) where T : Stream
        {
            try
            {
                using RentedArray<byte> buffer = new(value.Length << 2, clean: false);
                WriteBytes(stream, buffer.Span[..value.GetBytes(buffer)]);
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
        [TargetedPatchingOptOut("Tiny method")]
        public static async Task WriteStringAsync(this Stream stream, string value, CancellationToken cancellationToken = default)
        {
            try
            {
                using RentedArray<byte> buffer = new(value.Length << 2, clean: false);
                await WriteBytesAsync(stream, buffer.Memory[..value.GetBytes(buffer)], cancellationToken).DynamicContext();
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
        public static T WriteStringNullable<T>(this T stream, string? value) where T : Stream
        {
            Write(stream, value != null);
            if (value != null) WriteString(stream, value);
            return stream;
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        [TargetedPatchingOptOut("Tiny method")]
        public static async Task WriteStringNullableAsync(this Stream stream, string? value, CancellationToken cancellationToken = default)
        {
            await WriteAsync(stream, value != null, cancellationToken).DynamicContext();
            if (value != null) await WriteStringAsync(stream, value, cancellationToken).DynamicContext();
        }

        /// <summary>
        /// Write UTF-16 (little endian) string
        /// </summary>
        /// <typeparam name="T">Stream type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static T WriteString16<T>(this T stream, string value) where T : Stream
        {
            try
            {
                using RentedArray<byte> buffer = new(value.Length << 2, clean: false);
                WriteBytes(stream, buffer.Span[..value.GetBytes16(buffer)]);
                return stream;
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Write UTF-16 (little endian) string
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        [TargetedPatchingOptOut("Tiny method")]
        public static async Task WriteString16Async(this Stream stream, string value, CancellationToken cancellationToken = default)
        {
            try
            {
                using RentedArray<byte> buffer = new(value.Length << 2, clean: false);
                await WriteBytesAsync(stream, buffer.Memory[..value.GetBytes16(buffer)], cancellationToken).DynamicContext();
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Write UTF-16 (little endian) string
        /// </summary>
        /// <typeparam name="T">Stream type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static T WriteString16Nullable<T>(this T stream, string? value) where T : Stream
        {
            Write(stream, value != null);
            if (value != null) WriteString16(stream, value);
            return stream;
        }

        /// <summary>
        /// Write UTF-16 (little endian) string
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        [TargetedPatchingOptOut("Tiny method")]
        public static async Task WriteString16NullableAsync(this Stream stream, string? value, CancellationToken cancellationToken = default)
        {
            await WriteAsync(stream, value != null, cancellationToken).DynamicContext();
            if (value != null) await WriteString16Async(stream, value, cancellationToken).DynamicContext();
        }

        /// <summary>
        /// Write UTF-32 (little endian) string
        /// </summary>
        /// <typeparam name="T">Stream type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static T WriteString32<T>(this T stream, string value) where T : Stream
        {
            try
            {
                using RentedArray<byte> buffer = new(value.Length << 2, clean: false);
                WriteBytes(stream, buffer.Span[..value.GetBytes32(buffer)]);
                return stream;
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Write UTF-32 (little endian) string
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        [TargetedPatchingOptOut("Tiny method")]
        public static async Task WriteString32Async(this Stream stream, string value, CancellationToken cancellationToken = default)
        {
            try
            {
                using RentedArray<byte> buffer = new(value.Length << 2, clean: false);
                await WriteBytesAsync(stream, buffer.Memory[..value.GetBytes32(buffer)], cancellationToken).DynamicContext();
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Write UTF-32 (little endian) string
        /// </summary>
        /// <typeparam name="T">Stream type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static T WriteString32Nullable<T>(this T stream, string? value) where T : Stream
        {
            Write(stream, value != null);
            if (value != null) WriteString32(stream, value);
            return stream;
        }

        /// <summary>
        /// Write UTF-32 (little endian) string
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        [TargetedPatchingOptOut("Tiny method")]
        public static async Task WriteString32NullableAsync(this Stream stream, string? value, CancellationToken cancellationToken = default)
        {
            await WriteAsync(stream, value != null, cancellationToken).DynamicContext();
            if (value != null) await WriteString32Async(stream, value, cancellationToken).DynamicContext();
        }
    }
}
