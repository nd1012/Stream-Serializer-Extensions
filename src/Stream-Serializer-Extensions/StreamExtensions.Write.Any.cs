using System.Runtime;
using System.Runtime.CompilerServices;
using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    // Any
    public static partial class StreamExtensions
    {
        /// <summary>
        /// Write any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream WriteAny(this Stream stream, object obj)
        {
            (Type type, ObjectTypes objType, bool writeType, bool writeObject) = obj.GetObjectSerializerInfo();
            Write(stream, (byte)objType);
            if (writeType) WriteString(stream, type.ToString());
            if (!writeObject) return stream;
            return objType.IsNumber()
                ? WriteNumber(stream, obj)
                : WriteObject(stream, obj);
        }

        /// <summary>
        /// Write any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<Stream> WriteAnyAsync(this Stream stream, object obj, CancellationToken cancellationToken = default)
        {
            (Type type, ObjectTypes objType, bool writeType, bool writeObject) = obj.GetObjectSerializerInfo();
            await WriteAsync(stream, (byte)objType, cancellationToken).DynamicContext();
            if (writeType) await WriteStringAsync(stream, type.ToString(), cancellationToken).DynamicContext();
            if (!writeObject) return stream;
            return objType.IsNumber()
                ? await WriteNumberAsync(stream, obj, cancellationToken).DynamicContext()
                : await WriteObjectAsync(stream, obj, cancellationToken).DynamicContext();
        }

        /// <summary>
        /// Write any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteAnyAsync(this Task<Stream> stream, object obj, CancellationToken cancellationToken = default)
            => AsyncHelper.FluentAsync(stream, obj, cancellationToken, WriteAnyAsync);

        /// <summary>
        /// Write any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream WriteAnyNullable(this Stream stream, object? obj)
            => obj == null ? Write(stream, (byte)ObjectTypes.Null) : WriteAny(stream, obj);

        /// <summary>
        /// Write any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<Stream> WriteAnyNullableAsync(this Stream stream, object? obj, CancellationToken cancellationToken = default)
            => obj == null 
                ? await WriteAsync(stream, (byte)ObjectTypes.Null, cancellationToken).DynamicContext() 
                : await WriteAnyAsync(stream, obj, cancellationToken).DynamicContext();

        /// <summary>
        /// Write any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteAnyNullableAsync(this Task<Stream> stream, object? obj, CancellationToken cancellationToken = default)
            => AsyncHelper.FluentAsync(stream, obj, cancellationToken, WriteAnyNullableAsync);
    }
}
