using System.Collections;
using System.Runtime;
using System.Runtime.CompilerServices;
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
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream WriteList(this Stream stream, IList value)
        {
            WriteNumber(stream, value.Count);
            if (value.Count == 0) return stream;
            foreach (object element in value) WriteObject(stream, element);
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
        public static async Task<Stream> WriteListAsync(this Stream stream, IList value, CancellationToken cancellationToken = default)
        {
            await WriteNumberAsync(stream, value.Count, cancellationToken).DynamicContext();
            if (value.Count == 0) return stream;
            foreach (object element in value) await WriteObjectAsync(stream, element, cancellationToken).DynamicContext();
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
        public static Task<Stream> WriteListAsync(this Task<Stream> stream, IList value, CancellationToken cancellationToken = default)
            => AsyncHelper.FluentAsync(stream, value, cancellationToken, WriteListAsync);

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
        public static Stream WriteListNullable(this Stream stream, IList? value)
            => WriteNullableCount(stream, value?.Count, () =>
            {
                if (value!.Count == 0) return;
                foreach (object element in value) WriteObject(stream, element);
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
        public static Task<Stream> WriteListNullableAsync(this Stream stream, IList? value, CancellationToken cancellationToken = default)
            => WriteNullableCountAsync(stream, value?.Count, async () =>
            {
                if (value!.Count == 0) return;
                foreach (object element in value) await WriteObjectAsync(stream, element, cancellationToken).DynamicContext();
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
        public static Task<Stream> WriteListNullableAsync(this Task<Stream> stream, IList? value, CancellationToken cancellationToken = default)
            => AsyncHelper.FluentAsync(stream, value, cancellationToken, WriteListNullableAsync);
    }
}
