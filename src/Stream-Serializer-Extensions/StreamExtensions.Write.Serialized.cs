using System.Runtime;
using System.Runtime.CompilerServices;
using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    // Serialized
    public static partial class StreamExtensions
    {
        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream WriteSerialized(this Stream stream, IStreamSerializer obj)
            => SerializerException.Wrap(() =>
            {
                obj.Serialize(stream);
                return stream;
            });

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream WriteSerialized<T>(this Stream stream, T obj) where T : class, IStreamSerializer
            => SerializerException.Wrap(() =>
            {
                obj.Serialize(stream);
                return stream;
            });

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Structure type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream WriteSerializedStruct<T>(this Stream stream, T obj) where T : struct, IStreamSerializer
            => SerializerException.Wrap(() =>
            {
                obj.Serialize(stream);
                return stream;
            });

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<Stream> WriteSerializedAsync(this Stream stream, IStreamSerializer obj, CancellationToken cancellationToken = default)
            => SerializerException.Wrap(async () =>
            {
                await obj.SerializeAsync(stream, cancellationToken).DynamicContext();
                return stream;
            });

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteSerializedAsync(this Task<Stream> stream, IStreamSerializer obj, CancellationToken cancellationToken = default)
            => AsyncHelper.FluentAsync(stream, obj, cancellationToken, WriteSerializedAsync);

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<Stream> WriteSerializedAsync<T>(this Stream stream, T obj, CancellationToken cancellationToken = default) where T : class, IStreamSerializer
            => SerializerException.Wrap(async () =>
            {
                await obj.SerializeAsync(stream, cancellationToken).DynamicContext();
                return stream;
            });

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteSerializedAsync<T>(this Task<Stream> stream, T obj, CancellationToken cancellationToken = default) where T : class, IStreamSerializer
            => AsyncHelper.FluentAsync(stream, obj, cancellationToken, WriteSerializedAsync);

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Structure type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<Stream> WriteSerializedStructAsync<T>(this Stream stream, T obj, CancellationToken cancellationToken = default) where T : struct, IStreamSerializer
            => SerializerException.Wrap(async () =>
            {
                await obj.SerializeAsync(stream, cancellationToken).DynamicContext();
                return stream;
            });

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Structure type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteSerializedStructAsync<T>(this Task<Stream> stream, T obj, CancellationToken cancellationToken = default) where T : struct, IStreamSerializer
            => AsyncHelper.FluentAsync(stream, obj, cancellationToken, WriteSerializedStructAsync);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream WriteSerializedNullable(this Stream stream, IStreamSerializer? obj)
            => WriteIfNotNull(stream, obj, () => WriteSerialized(stream, obj!));

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream WriteSerializedNullable<T>(this Stream stream, T? obj) where T : class, IStreamSerializer
            => WriteIfNotNull(stream, obj, () => WriteSerialized(stream, obj!));

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Structure type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream WriteSerializedNullableStruct<T>(this Stream stream, T? obj) where T : struct, IStreamSerializer
            => WriteIfNotNull(stream, obj, () => WriteSerialized(stream, obj!.Value));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<Stream> WriteSerializedNullableAsync(this Stream stream, IStreamSerializer? obj, CancellationToken cancellationToken = default)
            => WriteIfNotNullAsync(stream, obj, () => WriteSerializedAsync(stream, obj!, cancellationToken), cancellationToken);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteSerializedNullableAsync(this Task<Stream> stream, IStreamSerializer? obj, CancellationToken cancellationToken = default)
            => AsyncHelper.FluentAsync(stream, obj, cancellationToken, WriteSerializedNullableAsync);

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<Stream> WriteSerializedNullableAsync<T>(this Stream stream, T? obj, CancellationToken cancellationToken = default) where T : class, IStreamSerializer
            => WriteIfNotNullAsync(stream, obj, () => WriteSerializedAsync(stream, obj!, cancellationToken), cancellationToken);

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteSerializedNullableAsync<T>(this Task<Stream> stream, T? obj, CancellationToken cancellationToken = default)
            where T : class, IStreamSerializer
            => AsyncHelper.FluentAsync(stream, obj, cancellationToken, WriteSerializedNullableAsync);

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Structure type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<Stream> WriteSerializedNullableStructAsync<T>(this Stream stream, T? obj, CancellationToken cancellationToken = default)
            where T : struct, IStreamSerializer
            => WriteIfNotNullAsync(stream, obj, () => WriteSerializedAsync(stream, obj!.Value, cancellationToken), cancellationToken);

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Structure type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteSerializedNullableStructAsync<T>(this Task<Stream> stream, T? obj, CancellationToken cancellationToken = default)
            where T : struct, IStreamSerializer
            => AsyncHelper.FluentAsync(stream, obj, cancellationToken, WriteSerializedNullableStructAsync);
    }
}
