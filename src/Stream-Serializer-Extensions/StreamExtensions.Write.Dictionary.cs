using System.Collections;
using System.Runtime;
using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    // Dictionary
    public static partial class StreamExtensions
    {
        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static Stream WriteDict(this Stream stream, IDictionary value)
            => SerializerException.Wrap(() =>
            {
                WriteNumber(stream, value.Count);
                if (value.Count == 0) return stream;
                foreach (object key in value.Keys)
                {
                    WriteObject(stream, key);
                    WriteObject(stream, value[key]!);
                }
                return stream;
            });

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        [TargetedPatchingOptOut("Tiny method")]
        public static Task WriteDictAsync(this Stream stream, IDictionary value, CancellationToken cancellationToken = default)
            => SerializerException.WrapAsync(async () =>
            {
                await WriteNumberAsync(stream, value.Count, cancellationToken).DynamicContext();
                if (value.Count == 0) return;
                foreach (object key in value.Keys)
                {
                    await WriteObjectAsync(stream, key, cancellationToken).DynamicContext();
                    await WriteObjectAsync(stream, value[key]!, cancellationToken).DynamicContext();
                }
            });

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static Stream WriteDictNullable(this Stream stream, IDictionary? value)
            => WriteIfNull(stream, value, () => WriteDict(stream, value!));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        [TargetedPatchingOptOut("Tiny method")]
        public static Task WriteDictNullableAsync(this Stream stream, IDictionary? value, CancellationToken cancellationToken = default)
            => WriteIfNullAsync(stream, value, () => WriteDictAsync(stream, value!, cancellationToken), cancellationToken);
    }
}
