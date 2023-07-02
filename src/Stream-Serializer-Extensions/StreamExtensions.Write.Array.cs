using System.Runtime;
using System.Runtime.CompilerServices;
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
        /// <param name="valuesNullable">Are values nullable?</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream WriteArray(this Stream stream, Array value, bool valuesNullable = false)
            => SerializerException.Wrap(() =>
            {
                WriteNumber(stream, value.Length);
                return value.Length == 0 ? stream : WriteFixedArray(stream, value, valuesNullable);
            });

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="valuesNullable">Are values nullable?</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<Stream> WriteArrayAsync(this Stream stream, Array value, bool valuesNullable = false, CancellationToken cancellationToken = default)
            => SerializerException.WrapAsync(async () =>
            {
                await WriteNumberAsync(stream, value.Length, cancellationToken).DynamicContext();
                return value.Length == 0 ? stream : await WriteFixedArrayAsync(stream, value, valuesNullable, cancellationToken).DynamicContext();
            });

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="valuesNullable">Are values nullable?</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteArrayAsync(this Task<Stream> stream, Array value, bool valuesNullable = false, CancellationToken cancellationToken = default)
            => AsyncHelper.FluentAsync(stream, value, valuesNullable, cancellationToken, WriteArrayAsync);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="valuesNullable">Are values nullable?</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream WriteArrayNullable(this Stream stream, Array? value, bool valuesNullable = false)
            => WriteNullableCount(stream, value?.Length, () => WriteFixedArray(stream, value!, valuesNullable));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="valuesNullable">Are values nullable?</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<Stream> WriteArrayNullableAsync(this Stream stream, Array? value, bool valuesNullable = false, CancellationToken cancellationToken = default)
            => WriteNullableCountAsync(stream, value?.Length, () => WriteFixedArrayAsync(stream, value!, valuesNullable, cancellationToken), cancellationToken);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="valuesNullable">Are values nullable?</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteArrayNullableAsync(this Task<Stream> stream, Array? value, bool valuesNullable = false, CancellationToken cancellationToken = default)
            => AsyncHelper.FluentAsync(stream, value, valuesNullable, cancellationToken, WriteArrayNullableAsync);
    }
}
