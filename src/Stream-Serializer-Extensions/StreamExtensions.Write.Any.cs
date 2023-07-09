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
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Just a method adapter")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Stream WriteAny(this Stream stream, object obj, ISerializationContext context) => WriteAny(stream, obj, objType: null, writeObject: true, context);

        /// <summary>
        /// Write any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="objType">Object type (if not <see langword="null"/>, no header will be written)</param>
        /// <param name="writeObject">Write the object? (may be overridden, if writing a header)</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream WriteAny(this Stream stream, object obj, ObjectTypes? objType, bool writeObject, ISerializationContext context)
        {
            if (objType == null)
            {
                (Type type, objType, bool writeType, writeObject) = obj.GetObjectSerializerInfo();
                Write(stream, (byte)objType, context);
                if (writeType) Write(stream, type, context);
            }
            if (!writeObject) return stream;
            return objType.Value switch
            {
                ObjectTypes.Byte => Write(stream, (sbyte)obj, context),
                ObjectTypes.Byte | ObjectTypes.Unsigned => Write(stream, (byte)obj, context),
                ObjectTypes.Short => Write(stream, (short)obj, context),
                ObjectTypes.Short | ObjectTypes.Unsigned => Write(stream, (ushort)obj, context),
                ObjectTypes.String16 => WriteString16(stream, (string)obj, context),
                ObjectTypes.String32 => WriteString32(stream, (string)obj, context),
                ObjectTypes.Serializable | ObjectTypes.CachedSerializable => Write(stream, obj.GetHashCode(), context),
                _ => objType.Value.IsNumber()
                    ? WriteNumber(stream, obj, context)
                    : WriteObject(stream, obj, context)
            };
        }

        /// <summary>
        /// Write any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Just a method adapter")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteAnyAsync(
            this Stream stream,
            object obj,
            ISerializationContext context
            )
            => WriteAnyAsync(stream, obj, objType: null, writeObject: true, context);

        /// <summary>
        /// Write any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="objType">Object type (if not <see langword="null"/>, no header will be written)</param>
        /// <param name="writeObject">Write the object? (may be overridden, if writing a header)</param>
        /// <param name="context">Context</param>
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
            ISerializationContext context
            )
        {
            if (objType == null)
            {
                (Type type, objType, bool writeType, writeObject) = obj.GetObjectSerializerInfo();
                await WriteAsync(stream, (byte)objType, context).DynamicContext();
                if (writeType) await WriteAsync(stream, type, context).DynamicContext();
            }
            if (!writeObject) return stream;
            return objType.Value switch
            {
                ObjectTypes.Byte => await WriteAsync(stream, (sbyte)obj, context).DynamicContext(),
                ObjectTypes.Byte | ObjectTypes.Unsigned => await WriteAsync(stream, (byte)obj, context).DynamicContext(),
                ObjectTypes.Short => await WriteAsync(stream, (short)obj, context).DynamicContext(),
                ObjectTypes.Short | ObjectTypes.Unsigned => await WriteAsync(stream, (ushort)obj, context).DynamicContext(),
                ObjectTypes.String16 => await WriteString16Async(stream, (string)obj, context).DynamicContext(),
                ObjectTypes.String32 => await WriteString32Async(stream, (string)obj, context).DynamicContext(),
                ObjectTypes.Serializable | ObjectTypes.CachedSerializable => await WriteAsync(stream, obj.GetHashCode(), context).DynamicContext(),
                _ => objType.Value.IsNumber()
                    ? await WriteNumberAsync(stream, obj, context).DynamicContext()
                    : await WriteObjectAsync(stream, obj, context).DynamicContext()
            };
        }

        /// <summary>
        /// Write any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteAnyAsync(
            this Task<Stream> stream,
            object obj,
            ISerializationContext context
            )
            => AsyncHelper.FluentAsync(stream, obj, context, WriteAnyAsync);

        /// <summary>
        /// Write any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="objType">Object type (if not <see langword="null"/>, no header will be written)</param>
        /// <param name="writeObject">Write the object? (may be overridden, if writing a header)</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteAnyAsync(
            this Task<Stream> stream, 
            object obj, 
            ObjectTypes? objType, 
            bool writeObject, 
            ISerializationContext context
            )
            => AsyncHelper.FluentAsync(stream, obj, objType, writeObject, context, WriteAnyAsync);

        /// <summary>
        /// Write any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream WriteAnyNullable(this Stream stream, object? obj, ISerializationContext context)
            => obj == null ? Write(stream, (byte)ObjectTypes.Null, context) : WriteAny(stream, obj, context);

        /// <summary>
        /// Write any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="objType">Object type (if not <see langword="null"/>, no header will be written)</param>
        /// <param name="writeObject">Write the object? (may be overridden, if writing a header)</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream WriteAnyNullable(this Stream stream, object? obj, ObjectTypes? objType, bool writeObject, ISerializationContext context)
            => obj == null ? Write(stream, (byte)ObjectTypes.Null, context) : WriteAny(stream, obj, objType, writeObject, context);

        /// <summary>
        /// Write any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<Stream> WriteAnyNullableAsync(
            this Stream stream,
            object? obj,
            ISerializationContext context
            )
            => obj == null
                ? await WriteAsync(stream, (byte)ObjectTypes.Null, context).DynamicContext()
                : await WriteAnyAsync(stream, obj, context).DynamicContext();

        /// <summary>
        /// Write any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="objType">Object type (if not <see langword="null"/>, no header will be written)</param>
        /// <param name="writeObject">Write the object? (may be overridden, if writing a header)</param>
        /// <param name="context">Context</param>
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
            ISerializationContext context
            )
            => obj == null 
                ? await WriteAsync(stream, (byte)ObjectTypes.Null, context).DynamicContext() 
                : await WriteAnyAsync(stream, obj, objType, writeObject, context).DynamicContext();

        /// <summary>
        /// Write any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteAnyNullableAsync(
            this Task<Stream> stream,
            object? obj,
            ISerializationContext context
            )
            => AsyncHelper.FluentAsync(stream, obj, context, WriteAnyNullableAsync);

        /// <summary>
        /// Write any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="objType">Object type (if not <see langword="null"/>, no header will be written)</param>
        /// <param name="writeObject">Write the object? (may be overridden, if writing a header)</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteAnyNullableAsync(
            this Task<Stream> stream, 
            object? obj, 
            ObjectTypes? objType, 
            bool writeObject, 
            ISerializationContext context
            )
            => AsyncHelper.FluentAsync(stream, obj, objType, writeObject, context, WriteAnyNullableAsync);
    }
}
