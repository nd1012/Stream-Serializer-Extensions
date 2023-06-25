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
            if (writeObject)
                if (objType.IsNumber())
                {
                    WriteNumber(stream, obj);
                }
                else
                {
                    WriteObject(stream, obj);
                }
            return stream;
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
            if (writeObject)
                if (objType.IsNumber())
                {
                    await WriteNumberAsync(stream, obj, cancellationToken).DynamicContext();
                }
                else
                {
                    await WriteObjectAsync(stream, obj, cancellationToken).DynamicContext();
                }
            return stream;
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
            => FluentAsync(stream, (s) => WriteAnyAsync(s, obj, cancellationToken));

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
        {
            if (obj == null)
            {
                Write(stream, (byte)ObjectTypes.Null);
            }
            else
            {
                WriteAny(stream, obj);
            }
            return stream;
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
        public static async Task<Stream> WriteAnyNullableAsync(this Stream stream, object? obj, CancellationToken cancellationToken = default)
        {
            if (obj == null)
            {
                await WriteAsync(stream, (byte)ObjectTypes.Null, cancellationToken).DynamicContext();
            }
            else
            {
                await WriteAnyAsync(stream, obj, cancellationToken).DynamicContext();
            }
            return stream;
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
        public static Task<Stream> WriteAnyNullableAsync(this Task<Stream> stream, object? obj, CancellationToken cancellationToken = default)
            => FluentAsync(stream, (s) => WriteAnyNullableAsync(s, obj, cancellationToken));
    }
}
