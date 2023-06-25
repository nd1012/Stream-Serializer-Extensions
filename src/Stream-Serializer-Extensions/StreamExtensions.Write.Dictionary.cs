using System.Collections;
using System.Runtime;
using System.Runtime.CompilerServices;
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
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream WriteDict(this Stream stream, IDictionary value)
        {
            WriteNumber(stream, value.Count);
            if (value.Count == 0) return stream;
            foreach (object key in value.Keys)
            {
                WriteObject(stream, key);
                WriteObject(stream, value[key]!);
            }
            return stream;
        }

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
        public static async Task<Stream> WriteDictAsync(this Stream stream, IDictionary value, CancellationToken cancellationToken = default)
        {
            await WriteNumberAsync(stream, value.Count, cancellationToken).DynamicContext();
            if (value.Count == 0) return stream;
            foreach (object key in value.Keys)
            {
                await WriteObjectAsync(stream, key, cancellationToken).DynamicContext();
                await WriteObjectAsync(stream, value[key]!, cancellationToken).DynamicContext();
            }
            return stream;
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteDictAsync(this Task<Stream> stream, IDictionary value, CancellationToken cancellationToken = default)
            => AsyncHelper.FluentAsync(stream, value, cancellationToken, WriteDictAsync);

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
        public static Stream WriteDictNullable(this Stream stream, IDictionary? value)
            => WriteNullableCount(stream, value?.Count, () =>
            {
                if (value!.Count == 0) return;
                foreach (object key in value.Keys)
                {
                    WriteObject(stream, key);
                    WriteObject(stream, value[key]!);
                }
            });

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
        public static Task<Stream> WriteDictNullableAsync(this Stream stream, IDictionary? value, CancellationToken cancellationToken = default)
            => WriteNullableCountAsync(stream, value?.Count, async () =>
            {
                if (value!.Count == 0) return;
                foreach (object key in value.Keys)
                {
                    await WriteObjectAsync(stream, key, cancellationToken).DynamicContext();
                    await WriteObjectAsync(stream, value[key]!, cancellationToken).DynamicContext();
                }
            }, cancellationToken);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteDictNullableAsync(this Task<Stream> stream, IDictionary? value, CancellationToken cancellationToken = default)
            => AsyncHelper.FluentAsync(stream, value, cancellationToken, WriteDictNullableAsync);
    }
}
