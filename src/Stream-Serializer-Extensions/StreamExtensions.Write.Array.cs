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
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream WriteArray(this Stream stream, Array value)
            => SerializerException.Wrap(() =>
            {
                WriteNumber(stream, value.Length);
                if (value.LongLength == 0) return stream;
                foreach (object element in value) WriteObject(stream, element);
                return stream;
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
        public static Task<Stream> WriteArrayAsync(this Stream stream, Array value, CancellationToken cancellationToken = default)
            => SerializerException.WrapAsync(async () =>
            {
                await WriteNumberAsync(stream, value.Length, cancellationToken).DynamicContext();
                if (value.Length == 0) return stream;
                foreach (object element in value) await WriteObjectAsync(stream, element, cancellationToken).DynamicContext();
                return stream;
            });

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteArrayAsync(this Task<Stream> stream, Array value, CancellationToken cancellationToken = default)
            => AsyncHelper.FluentAsync(stream, value, cancellationToken, WriteArrayAsync);

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
        public static Stream WriteArrayNullable(this Stream stream, Array? value)
            => WriteNullableCount(stream, value?.Length, () => WriteFixedArray(stream, value!));

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
        public static Task<Stream> WriteArrayNullableAsync(this Stream stream, Array? value, CancellationToken cancellationToken = default)
            => WriteNullableCountAsync(stream, value?.Length, () => WriteFixedArrayAsync(stream, value!, cancellationToken), cancellationToken);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteArrayNullableAsync(this Task<Stream> stream, Array? value, CancellationToken cancellationToken = default)
            => AsyncHelper.FluentAsync(stream, value, cancellationToken, WriteArrayNullableAsync);

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Just a method adapter")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Stream WriteFixedArray<T>(this Stream stream, Span<T> value) => WriteFixedArray(stream, (ReadOnlySpan<T>)value);

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream WriteFixedArray<T>(this Stream stream, ReadOnlySpan<T> value)
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
                throw SerializerException.From(ex);
            }
        }

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
        public static Stream WriteFixedArray(this Stream stream, Array value)
            => SerializerException.Wrap(() =>
            {
                foreach (object element in value)
                    WriteObject(stream, element!);
                return stream;
            });

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Just a method adapter")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteFixedArrayAsync<T>(this Stream stream, Memory<T> value, CancellationToken cancellationToken = default)
            => WriteFixedArrayAsync(stream, (ReadOnlyMemory<T>)value, cancellationToken);

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Just a method adapter")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteFixedArrayAsync<T>(this Task<Stream> stream, Memory<T> value, CancellationToken cancellationToken = default)
            => AsyncHelper.FluentAsync(stream, value, cancellationToken, WriteFixedArrayAsync);

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<Stream> WriteFixedArrayAsync<T>(this Stream stream, ReadOnlyMemory<T> value, CancellationToken cancellationToken = default)
            => SerializerException.WrapAsync(async () =>
            {
                for (int i = 0; i < value.Length; i++) await WriteObjectAsync(stream, value.Span[i]!, cancellationToken).DynamicContext();
                return stream;
            });

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteFixedArrayAsync<T>(this Task<Stream> stream, ReadOnlyMemory<T> value, CancellationToken cancellationToken = default)
            => AsyncHelper.FluentAsync(stream, value, cancellationToken, WriteFixedArrayAsync);

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
        public static Task<Stream> WriteFixedArrayAsync(this Stream stream, Array value, CancellationToken cancellationToken = default)
            => SerializerException.WrapAsync(async () =>
            {
                foreach (object element in value)
                    await WriteObjectAsync(stream, element!, cancellationToken).DynamicContext();
                return stream;
            });

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteFixedArrayAsync(this Task<Stream> stream, Array value, CancellationToken cancellationToken = default)
            => AsyncHelper.FluentAsync(stream, value, cancellationToken, WriteFixedArrayAsync);
    }
}
