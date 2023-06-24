using System.Runtime;
using System.Runtime.CompilerServices;

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
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task WriteSerializedAsync(this Stream stream, IStreamSerializer obj, CancellationToken cancellationToken = default)
            => SerializerException.Wrap(() => obj.SerializeAsync(stream, cancellationToken));

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="cancellationToken">Cancellation token</param>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task WriteSerializedAsync<T>(this Stream stream, T obj, CancellationToken cancellationToken = default) where T : class, IStreamSerializer
            => SerializerException.Wrap(() => obj.SerializeAsync(stream, cancellationToken));

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Structure type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="cancellationToken">Cancellation token</param>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task WriteSerializedStructAsync<T>(this Stream stream, T obj, CancellationToken cancellationToken = default) where T : struct, IStreamSerializer
            => SerializerException.Wrap(() => obj.SerializeAsync(stream, cancellationToken));

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
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task WriteSerializedNullableAsync(this Stream stream, IStreamSerializer? obj, CancellationToken cancellationToken = default)
            => WriteIfNotNullAsync(stream, obj, () => WriteSerializedAsync(stream, obj!, cancellationToken), cancellationToken);

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="cancellationToken">Cancellation token</param>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task WriteSerializedNullableAsync<T>(this Stream stream, T? obj, CancellationToken cancellationToken = default) where T : class, IStreamSerializer
            => WriteIfNotNullAsync(stream, obj, () => WriteSerializedAsync(stream, obj!, cancellationToken), cancellationToken);

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Structure type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="cancellationToken">Cancellation token</param>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task WriteSerializedNullableAsync<T>(this Stream stream, T? obj, CancellationToken cancellationToken = default) where T : struct, IStreamSerializer
            => WriteIfNotNullAsync(stream, obj, () => WriteSerializedAsync(stream, obj!.Value, cancellationToken), cancellationToken);
    }
}
