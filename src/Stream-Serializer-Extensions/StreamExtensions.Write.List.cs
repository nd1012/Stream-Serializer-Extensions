using System.Collections;
using System.Runtime;
using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    // List
    public static partial class StreamExtensions
    {
        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static Stream WriteList(this Stream stream, IList value)
            => SerializerException.Wrap(() =>
            {
                if (value.GetType().GetGenericArgumentsCached()[0] == typeof(byte)) return WriteBytes(stream, (value as byte[])!);
                WriteNumber(stream, value.Count);
                if (value.Count == 0) return stream;
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
        public static Task WriteListAsync(this Stream stream, IList value, CancellationToken cancellationToken = default)
            => SerializerException.WrapAsync(async () =>
            {
                if (value.GetType().GetGenericArgumentsCached()[0] == typeof(byte))
                {
                    await WriteBytesAsync(stream, (value as byte[])!, cancellationToken).DynamicContext();
                    return;
                }
                await WriteNumberAsync(stream, value.Count, cancellationToken).DynamicContext();
                if (value.Count == 0) return;
                foreach (object element in value) await WriteObjectAsync(stream, element, cancellationToken).DynamicContext();
            });

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static Stream WriteListNullable(this Stream stream, IList? value)
            => WriteIfNull(stream, value, () => WriteList(stream, value!));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        [TargetedPatchingOptOut("Tiny method")]
        public static Task WriteListNullableAsync(this Stream stream, IList? value, CancellationToken cancellationToken = default)
            => WriteIfNullAsync(stream, value, () => WriteListAsync(stream, value!, cancellationToken), cancellationToken);
    }
}
