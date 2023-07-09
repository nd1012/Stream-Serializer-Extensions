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
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream WriteSerialized(this Stream stream, IStreamSerializer obj, ISerializationContext context)
            => SerializerException.Wrap(() =>
            {
                using ContextRecursion cr = new(context);
                obj.Serialize(context);
                return stream;
            });

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream WriteSerialized<T>(this Stream stream, T obj, ISerializationContext context) where T : class, IStreamSerializer
            => SerializerException.Wrap(() =>
            {
                using ContextRecursion cr = new(context);
                obj.Serialize(context);
                return stream;
            });

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Structure type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream WriteSerializedStruct<T>(this Stream stream, T obj, ISerializationContext context) where T : struct, IStreamSerializer
            => SerializerException.Wrap(() =>
            {
                using ContextRecursion cr = new(context);
                obj.Serialize(context);
                return stream;
            });

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<Stream> WriteSerializedAsync(this Stream stream, IStreamSerializer obj, ISerializationContext context)
            => SerializerException.Wrap(async () =>
            {
                using ContextRecursion cr = new(context);
                await obj.SerializeAsync(context).DynamicContext();
                return stream;
            });

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteSerializedAsync(this Task<Stream> stream, IStreamSerializer obj, ISerializationContext context)
            => AsyncHelper.FluentAsync(stream, obj, context, WriteSerializedAsync);

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<Stream> WriteSerializedAsync<T>(this Stream stream, T obj, ISerializationContext context) where T : class, IStreamSerializer
            => SerializerException.Wrap(async () =>
            {
                using ContextRecursion cr = new(context);
                await obj.SerializeAsync(context).DynamicContext();
                return stream;
            });

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteSerializedAsync<T>(this Task<Stream> stream, T obj, ISerializationContext context) where T : class, IStreamSerializer
            => AsyncHelper.FluentAsync(stream, obj, context, WriteSerializedAsync);

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Structure type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<Stream> WriteSerializedStructAsync<T>(this Stream stream, T obj, ISerializationContext context) where T : struct, IStreamSerializer
            => SerializerException.Wrap(async () =>
            {
                using ContextRecursion cr = new(context);
                await obj.SerializeAsync(context).DynamicContext();
                return stream;
            });

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Structure type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteSerializedStructAsync<T>(this Task<Stream> stream, T obj, ISerializationContext context) where T : struct, IStreamSerializer
            => AsyncHelper.FluentAsync(stream, obj, context, WriteSerializedStructAsync);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream WriteSerializedNullable(this Stream stream, IStreamSerializer? obj, ISerializationContext context)
            => WriteIfNotNull(stream, obj, () => WriteSerialized(stream, obj!, context), context);

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream WriteSerializedNullable<T>(this Stream stream, T? obj, ISerializationContext context) where T : class, IStreamSerializer
            => WriteIfNotNull(stream, obj, () => WriteSerialized(stream, obj!, context), context);

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Structure type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream WriteSerializedNullableStruct<T>(this Stream stream, T? obj, ISerializationContext context) where T : struct, IStreamSerializer
            => WriteIfNotNull(stream, obj, () => WriteSerialized(stream, obj!.Value, context), context);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<Stream> WriteSerializedNullableAsync(this Stream stream, IStreamSerializer? obj, ISerializationContext context)
            => WriteIfNotNullAsync(stream, obj, () => WriteSerializedAsync(stream, obj!, context), context);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteSerializedNullableAsync(this Task<Stream> stream, IStreamSerializer? obj, ISerializationContext context)
            => AsyncHelper.FluentAsync(stream, obj, context, WriteSerializedNullableAsync);

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<Stream> WriteSerializedNullableAsync<T>(this Stream stream, T? obj, ISerializationContext context) where T : class, IStreamSerializer
            => WriteIfNotNullAsync(stream, obj, () => WriteSerializedAsync(stream, obj!, context), context);

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteSerializedNullableAsync<T>(this Task<Stream> stream, T? obj, ISerializationContext context)
            where T : class, IStreamSerializer
            => AsyncHelper.FluentAsync(stream, obj, context, WriteSerializedNullableAsync);

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Structure type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<Stream> WriteSerializedNullableStructAsync<T>(this Stream stream, T? obj, ISerializationContext context)
            where T : struct, IStreamSerializer
            => WriteIfNotNullAsync(stream, obj, () => WriteSerializedAsync(stream, obj!.Value, context), context);

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Structure type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteSerializedNullableStructAsync<T>(this Task<Stream> stream, T? obj, ISerializationContext context)
            where T : struct, IStreamSerializer
            => AsyncHelper.FluentAsync(stream, obj, context, WriteSerializedNullableStructAsync);
    }
}
