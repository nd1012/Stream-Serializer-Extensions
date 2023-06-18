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
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static Stream WriteString(this Stream stream, string value)
            => SerializerException.Wrap(() =>
            {
                using RentedArray<byte> buffer = new(value.Length << 2, clean: false);
                WriteBytes(stream, buffer.Span[..value.GetBytes(buffer)]);
                return stream;
            });

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        [TargetedPatchingOptOut("Tiny method")]
        public static Task WriteStringAsync(this Stream stream, string value, CancellationToken cancellationToken = default)
            => SerializerException.WrapAsync(async () =>
            {
                using RentedArray<byte> buffer = new(value.Length << 2, clean: false);
                await WriteBytesAsync(stream, buffer.Memory[..value.GetBytes(buffer)], cancellationToken).DynamicContext();
            });

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static Stream WriteStringNullable(this Stream stream, string? value)
            => WriteIfNull(stream, value, () => WriteString(stream, value!));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        [TargetedPatchingOptOut("Tiny method")]
        public static Task WriteStringNullableAsync(this Stream stream, string? value, CancellationToken cancellationToken = default)
            => WriteIfNullAsync(stream, value, async () => await WriteStringAsync(stream, value!, cancellationToken).DynamicContext(), cancellationToken);

        /// <summary>
        /// Write UTF-16 (little endian) string
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static Stream WriteString16(this Stream stream, string value)
            => SerializerException.Wrap(() =>
            {
                using RentedArray<byte> buffer = new(value.Length << 2, clean: false);
                WriteBytes(stream, buffer.Span[..value.GetBytes16(buffer)]);
                return stream;
            });

        /// <summary>
        /// Write UTF-16 (little endian) string
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        [TargetedPatchingOptOut("Tiny method")]
        public static Task WriteString16Async(this Stream stream, string value, CancellationToken cancellationToken = default)
            => SerializerException.WrapAsync(async () =>
            {
                using RentedArray<byte> buffer = new(value.Length << 2, clean: false);
                await WriteBytesAsync(stream, buffer.Memory[..value.GetBytes16(buffer)], cancellationToken).DynamicContext();
            });

        /// <summary>
        /// Write UTF-16 (little endian) string
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static Stream WriteString16Nullable<T>(this Stream stream, string? value)
            => WriteIfNull(stream, value, () => WriteString16(stream, value!));

        /// <summary>
        /// Write UTF-16 (little endian) string
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        [TargetedPatchingOptOut("Tiny method")]
        public static Task WriteString16NullableAsync(this Stream stream, string? value, CancellationToken cancellationToken = default)
            => WriteIfNullAsync(stream, value, async () => await WriteString16Async(stream, value!, cancellationToken).DynamicContext(), cancellationToken);

        /// <summary>
        /// Write UTF-32 (little endian) string
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static Stream WriteString32(this Stream stream, string value)
            => SerializerException.Wrap(() =>
            {
                using RentedArray<byte> buffer = new(value.Length << 2, clean: false);
                WriteBytes(stream, buffer.Span[..value.GetBytes32(buffer)]);
                return stream;
            });

        /// <summary>
        /// Write UTF-32 (little endian) string
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        [TargetedPatchingOptOut("Tiny method")]
        public static Task WriteString32Async(this Stream stream, string value, CancellationToken cancellationToken = default)
            => SerializerException.WrapAsync(async () =>
            {
                using RentedArray<byte> buffer = new(value.Length << 2, clean: false);
                await WriteBytesAsync(stream, buffer.Memory[..value.GetBytes32(buffer)], cancellationToken).DynamicContext();
            });

        /// <summary>
        /// Write UTF-32 (little endian) string
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static Stream WriteString32Nullable(this Stream stream, string? value)
            => WriteIfNull(stream, value, () => WriteString32(stream, value!));

        /// <summary>
        /// Write UTF-32 (little endian) string
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        [TargetedPatchingOptOut("Tiny method")]
        public static Task WriteString32NullableAsync(this Stream stream, string? value, CancellationToken cancellationToken = default)
            => WriteIfNullAsync(stream, value, async () => await WriteString32Async(stream, value!, cancellationToken).DynamicContext(), cancellationToken);
    }
}
