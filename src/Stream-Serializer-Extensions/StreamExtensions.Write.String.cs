using System.Runtime.CompilerServices;
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
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        public static Stream WriteString(this Stream stream, string value)
            => WriteString(stream, value, lenShift: 2, (buffer) => value.GetBytes(buffer));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static Task WriteStringAsync(this Stream stream, string value, CancellationToken cancellationToken = default)
            => WriteStringAsync(stream, value, lenShift: 2, (buffer) => value.GetBytes(buffer), cancellationToken);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        public static Stream WriteStringNullable(this Stream stream, string? value)
            => WriteNullableString(stream, value, lenShift: 2, (buffer) => value!.GetBytes(buffer));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static Task WriteStringNullableAsync(this Stream stream, string? value, CancellationToken cancellationToken = default)
            => WriteNullableStringAsync(stream, value, lenShift: 2, (buffer) => value!.GetBytes(buffer), cancellationToken);

        /// <summary>
        /// Write UTF-16 (little endian) string
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        public static Stream WriteString16(this Stream stream, string value)
            => WriteString(stream, value, lenShift: 1, (buffer) => value.GetBytes16(buffer));

        /// <summary>
        /// Write UTF-16 (little endian) string
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static Task WriteString16Async(this Stream stream, string value, CancellationToken cancellationToken = default)
            => WriteStringAsync(stream, value, lenShift: 1, (buffer) => value.GetBytes16(buffer), cancellationToken);

        /// <summary>
        /// Write UTF-16 (little endian) string
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        public static Stream WriteString16Nullable<T>(this Stream stream, string? value)
            => WriteNullableString(stream, value, lenShift: 1, (buffer) => value!.GetBytes16(buffer));

        /// <summary>
        /// Write UTF-16 (little endian) string
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static Task WriteString16NullableAsync(this Stream stream, string? value, CancellationToken cancellationToken = default)
            => WriteNullableStringAsync(stream, value, lenShift: 1, (buffer) => value!.GetBytes16(buffer), cancellationToken);

        /// <summary>
        /// Write UTF-32 (little endian) string
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        public static Stream WriteString32(this Stream stream, string value)
            => WriteString(stream, value, lenShift: 2, (buffer) => value.GetBytes32(buffer));

        /// <summary>
        /// Write UTF-32 (little endian) string
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static Task WriteString32Async(this Stream stream, string value, CancellationToken cancellationToken = default)
            => WriteStringAsync(stream, value, lenShift: 2, (buffer) => value.GetBytes32(buffer), cancellationToken);

        /// <summary>
        /// Write UTF-32 (little endian) string
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        public static Stream WriteString32Nullable(this Stream stream, string? value)
            => WriteNullableString(stream, value, lenShift: 2, (buffer) => value!.GetBytes32(buffer));

        /// <summary>
        /// Write UTF-32 (little endian) string
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static Task WriteString32NullableAsync(this Stream stream, string? value, CancellationToken cancellationToken = default)
            => WriteNullableStringAsync(stream, value, lenShift: 2, (buffer) => value!.GetBytes32(buffer), cancellationToken);

        /// <summary>
        /// Write a string
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value</param>
        /// <param name="lenShift">Buffer length bit-shifting value</param>
        /// <param name="action">Writing action to execute</param>
        /// <returns>Stream</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static Stream WriteString(Stream stream, string value, int lenShift, Func<byte[], int> action)
        {
            if (value.Length == 0)
            {
                Write(stream, (byte)NumberTypes.Zero);
                return stream;
            }
            byte[] data = StreamSerializer.BufferPool.Rent(value.Length << lenShift);
            try
            {
                int len = action(data);
                WriteNumber(stream, len);
                return WriteSerializedData(stream, data, len);
            }
            catch
            {
                StreamSerializer.BufferPool.Return(data);
                throw;
            }
        }

        /// <summary>
        /// Write a string
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value</param>
        /// <param name="lenShift">Buffer length bit-shifting value</param>
        /// <param name="action">Writing action to execut</param>
        /// <param name="cancellationToken">Cancellation token</param>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static async Task WriteStringAsync(Stream stream, string value, int lenShift, Func<byte[], int> action, CancellationToken cancellationToken)
        {
            if (value.Length == 0)
            {
                await WriteAsync(stream, (byte)NumberTypes.Zero, cancellationToken).DynamicContext();
                return;
            }
            byte[] data = StreamSerializer.BufferPool.Rent(value.Length << lenShift);
            try
            {
                int len = action(data);
                await WriteNumberAsync(stream, len, cancellationToken).DynamicContext();
                await WriteSerializedDataAsync(stream, data, len, cancellationToken: cancellationToken).DynamicContext();
            }
            catch
            {
                StreamSerializer.BufferPool.Return(data);
                throw;
            }
        }

        /// <summary>
        /// Write a nullable string
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value</param>
        /// <param name="lenShift">Buffer length bit-shifting value</param>
        /// <param name="action">Writing action to execute if the string isn't <see langword="null"/></param>
        /// <returns>Stream</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static Stream WriteNullableString(Stream stream, string? value, int lenShift, Func<byte[], int> action)
        {
            if (value == null)
            {
                Write(stream, (byte)NumberTypes.Null);
                return stream;
            }
            if (value.Length == 0)
            {
                Write(stream, (byte)NumberTypes.Zero);
                return stream;
            }
            byte[] data = StreamSerializer.BufferPool.Rent(value.Length << lenShift);
            try
            {
                int len = action(data);
                WriteNumber(stream, len);
                return WriteSerializedData(stream, data, action(data), StreamSerializer.BufferPool);
            }
            catch
            {
                StreamSerializer.BufferPool.Return(data);
                throw;
            }
        }

        /// <summary>
        /// Write a nullable string
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value</param>
        /// <param name="lenShift">Buffer length bit-shifting value</param>
        /// <param name="action">Writing action to execute if the string isn't <see langword="null"/></param>
        /// <param name="cancellationToken">Cancellation token</param>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static async Task WriteNullableStringAsync(Stream stream, string? value, int lenShift, Func<byte[], int> action, CancellationToken cancellationToken)
        {
            if (value == null)
            {
                await WriteAsync(stream, (byte)NumberTypes.Null, cancellationToken).DynamicContext();
                return;
            }
            if (value.Length == 0)
            {
                await WriteAsync(stream, (byte)NumberTypes.Zero, cancellationToken).DynamicContext();
                return;
            }
            byte[] data = StreamSerializer.BufferPool.Rent(value.Length << lenShift);
            try
            {
                int len = action(data);
                await WriteNumberAsync(stream, len, cancellationToken).DynamicContext();
                await WriteSerializedDataAsync(stream, data, action(data), StreamSerializer.BufferPool, cancellationToken).DynamicContext();
            }
            catch
            {
                StreamSerializer.BufferPool.Return(data);
                throw;
            }
        }
    }
}
