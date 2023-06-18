using System.Runtime;
using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    // Array
    public static partial class StreamExtensions
    {
        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static Stream WriteArray(this Stream stream, Array value)
            => SerializerException.Wrap(() =>
            {
                if (value.GetType().GetElementType() == typeof(byte)) return WriteBytes(stream, (value as byte[])!);
                WriteNumber(stream, value.Length);
                if (value.Length == 0) return stream;
                foreach (object element in value) WriteObject(stream, element);
                return stream;
            });

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        [TargetedPatchingOptOut("Tiny method")]
        public static Task WriteArrayAsync(this Stream stream, Array value, CancellationToken cancellationToken = default)
            => SerializerException.WrapAsync(async () =>
            {
                if (value.GetType().GetElementType() == typeof(byte))
                {
                    await WriteBytesAsync(stream, (value as byte[])!, cancellationToken).DynamicContext();
                    return;
                }
                await WriteNumberAsync(stream, value.Length, cancellationToken).DynamicContext();
                if (value.Length == 0) return;
                foreach (object element in value) await WriteObjectAsync(stream, element, cancellationToken).DynamicContext();
            });

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static Stream WriteArrayNullable(this Stream stream, Array? value)
            => WriteIfNull(stream, value, () => WriteArray(stream, value!));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        [TargetedPatchingOptOut("Tiny method")]
        public static Task WriteArrayNullableAsync(this Stream stream, Array? value, CancellationToken cancellationToken = default)
            => WriteIfNullAsync(stream, value, () => WriteArrayAsync(stream, value!, cancellationToken), cancellationToken);

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static Stream WriteFixedArray<T>(this Stream stream, Span<T> value)
        {
            try
            {
                foreach (T element in value)
                    WriteObject(stream, element!);
                return stream;
            }
            catch (SerializerException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        [TargetedPatchingOptOut("Tiny method")]
        public static Task WriteFixedArrayAsync<T>(this Stream stream, Memory<T> value, CancellationToken cancellationToken = default)
            => SerializerException.WrapAsync(async () =>
            {
                for (int i = 0; i < value.Length; i++) await WriteObjectAsync(stream, value.Span[i]!, cancellationToken).DynamicContext();
            });
    }
}
