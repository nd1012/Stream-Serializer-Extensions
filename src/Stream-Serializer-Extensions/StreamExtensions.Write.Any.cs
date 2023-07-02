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
        [TargetedPatchingOptOut("Kust a method adapter")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Stream WriteAny(this Stream stream, object obj) => WriteAny(stream, obj, objType: null, writeObject: true);

        /// <summary>
        /// Write any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="objType">Object type (if not <see langword="null"/>, no header will be written)</param>
        /// <param name="writeObject">Write the object? (may be overridden, if writing a header)</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream WriteAny(this Stream stream, object obj, ObjectTypes? objType, bool writeObject)
        {
            if (objType == null)
            {
                (Type type, objType, bool writeType, writeObject) = obj.GetObjectSerializerInfo();
                Write(stream, (byte)objType);
                if (writeType) WriteString(stream, type.ToString());
            }
            if (!writeObject) return stream;
            return objType.Value switch
            {
                ObjectTypes.Byte => Write(stream, (sbyte)obj),
                ObjectTypes.Byte | ObjectTypes.Unsigned => Write(stream, (byte)obj),
                ObjectTypes.Short => Write(stream, (short)obj),
                ObjectTypes.Short | ObjectTypes.Unsigned => Write(stream, (ushort)obj),
                ObjectTypes.String16 => WriteString16(stream, (string)obj),
                ObjectTypes.String32 => WriteString32(stream, (string)obj),
                _ => objType.Value.IsNumber()
                    ? WriteNumber(stream, obj)
                    : WriteObject(stream, obj)
            };
        }

        /// <summary>
        /// Write any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Just a method adapter")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteAnyAsync(
            this Stream stream,
            object obj,
            CancellationToken cancellationToken = default
            )
            => WriteAnyAsync(stream, obj, objType: null, writeObject: true, cancellationToken);

        /// <summary>
        /// Write any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="objType">Object type (if not <see langword="null"/>, no header will be written)</param>
        /// <param name="writeObject">Write the object? (may be overridden, if writing a header)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<Stream> WriteAnyAsync(
            this Stream stream,
            object obj,
            ObjectTypes? objType,
            bool writeObject,
            CancellationToken cancellationToken = default
            )
        {
            if (objType == null)
            {
                (Type type, objType, bool writeType, writeObject) = obj.GetObjectSerializerInfo();
                await WriteAsync(stream, (byte)objType, cancellationToken).DynamicContext();
                if (writeType) await WriteStringAsync(stream, type.ToString(), cancellationToken).DynamicContext();
            }
            if (!writeObject) return stream;
            return objType.Value switch
            {
                ObjectTypes.Byte => await WriteAsync(stream, (sbyte)obj, cancellationToken).DynamicContext(),
                ObjectTypes.Byte | ObjectTypes.Unsigned => await WriteAsync(stream, (byte)obj, cancellationToken).DynamicContext(),
                ObjectTypes.Short => await WriteAsync(stream, (short)obj, cancellationToken).DynamicContext(),
                ObjectTypes.Short | ObjectTypes.Unsigned => await WriteAsync(stream, (ushort)obj, cancellationToken).DynamicContext(),
                ObjectTypes.String16 => await WriteString16Async(stream, (string)obj, cancellationToken).DynamicContext(),
                ObjectTypes.String32 => await WriteString32Async(stream, (string)obj, cancellationToken).DynamicContext(),
                _ => objType.Value.IsNumber()
                    ? await WriteNumberAsync(stream, obj, cancellationToken).DynamicContext()
                    : await WriteObjectAsync(stream, obj, cancellationToken).DynamicContext()
            };
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
        public static Task<Stream> WriteAnyAsync(
            this Task<Stream> stream,
            object obj,
            CancellationToken cancellationToken = default
            )
            => AsyncHelper.FluentAsync(stream, obj, cancellationToken, WriteAnyAsync);

        /// <summary>
        /// Write any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="objType">Object type (if not <see langword="null"/>, no header will be written)</param>
        /// <param name="writeObject">Write the object? (may be overridden, if writing a header)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteAnyAsync(
            this Task<Stream> stream, 
            object obj, 
            ObjectTypes? objType, 
            bool writeObject, 
            CancellationToken cancellationToken = default
            )
            => AsyncHelper.FluentAsync(stream, obj, objType, writeObject, cancellationToken, WriteAnyAsync);

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
        /// <param name="objType">Object type (if not <see langword="null"/>, no header will be written)</param>
        /// <param name="writeObject">Write the object? (may be overridden, if writing a header)</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream WriteAnyNullable(this Stream stream, object? obj, ObjectTypes? objType, bool writeObject)
            => obj == null ? Write(stream, (byte)ObjectTypes.Null) : WriteAny(stream, obj, objType, writeObject);

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
        public static async Task<Stream> WriteAnyNullableAsync(
            this Stream stream,
            object? obj,
            CancellationToken cancellationToken = default
            )
            => obj == null
                ? await WriteAsync(stream, (byte)ObjectTypes.Null, cancellationToken).DynamicContext()
                : await WriteAnyAsync(stream, obj, cancellationToken).DynamicContext();

        /// <summary>
        /// Write any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="objType">Object type (if not <see langword="null"/>, no header will be written)</param>
        /// <param name="writeObject">Write the object? (may be overridden, if writing a header)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<Stream> WriteAnyNullableAsync(
            this Stream stream, 
            object? obj, 
            ObjectTypes? objType, 
            bool writeObject, 
            CancellationToken cancellationToken = default
            )
            => obj == null 
                ? await WriteAsync(stream, (byte)ObjectTypes.Null, cancellationToken).DynamicContext() 
                : await WriteAnyAsync(stream, obj, objType, writeObject, cancellationToken).DynamicContext();

        /// <summary>
        /// Write any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteAnyNullableAsync(
            this Task<Stream> stream,
            object? obj,
            CancellationToken cancellationToken = default
            )
            => AsyncHelper.FluentAsync(stream, obj, cancellationToken, WriteAnyNullableAsync);

        /// <summary>
        /// Write any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="objType">Object type (if not <see langword="null"/>, no header will be written)</param>
        /// <param name="writeObject">Write the object? (may be overridden, if writing a header)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteAnyNullableAsync(
            this Task<Stream> stream, 
            object? obj, 
            ObjectTypes? objType, 
            bool writeObject, 
            CancellationToken cancellationToken = default
            )
            => AsyncHelper.FluentAsync(stream, obj, objType, writeObject, cancellationToken, WriteAnyNullableAsync);
    }
}
