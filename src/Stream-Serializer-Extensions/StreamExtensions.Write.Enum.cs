using System.Runtime;
using System.Runtime.CompilerServices;
using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    // Enumeration
    public static partial class StreamExtensions
    {
        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Enumeration type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream WriteEnum<T>(this Stream stream, T value, ISerializationContext context) where T : struct, Enum
        {
            if (ObjectHelper.AreEqual(value, default(T))) return Write(stream, (byte)NumberTypes.Default, context);
            return WriteNumber(stream, Convert.ChangeType(value, typeof(T).GetEnumUnderlyingType()), context);
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream WriteEnum(this Stream stream, object value, ISerializationContext context)
        {
            Type enumType = value.GetType();
            SerializerException.Wrap(() => ArgumentValidationHelper.EnsureValidArgument(nameof(value), enumType.IsEnum, () => "Not an enumeration value"));
            if (ObjectHelper.AreEqual(value, Activator.CreateInstance(enumType))) return Write(stream, (byte)NumberTypes.Default, context);
            return WriteNumber(stream, Convert.ChangeType(value, enumType.GetEnumUnderlyingType()), context);
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Enumeration type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<Stream> WriteEnumAsync<T>(this Stream stream, T value, ISerializationContext context) where T : struct, Enum
        {
            if (ObjectHelper.AreEqual(value, default(T)))
            {
                await WriteAsync(stream, (byte)NumberTypes.Default, context).DynamicContext();
            }
            else
            {
                await WriteNumberAsync(stream, Convert.ChangeType(value, value.GetType().GetEnumUnderlyingType()), context).DynamicContext();
            }
            return stream;
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Enumeration type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteEnumAsync<T>(this Task<Stream> stream, T value, ISerializationContext context) where T : struct, Enum
            => AsyncHelper.FluentAsync(stream, value, context, WriteEnumAsync);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<Stream> WriteEnumAsync(this Stream stream, object value, ISerializationContext context)
        {
            Type enumType = value.GetType();
            SerializerException.Wrap(() => ArgumentValidationHelper.EnsureValidArgument(nameof(value), enumType.IsEnum, () => "Not an enumeration value"));
            if (ObjectHelper.AreEqual(value, Activator.CreateInstance(enumType)))
            {
                await WriteAsync(stream, (byte)NumberTypes.Default, context).DynamicContext();
            }
            else
            {
                await WriteNumberAsync(stream, Convert.ChangeType(value, enumType.GetEnumUnderlyingType()), context).DynamicContext();
            }
            return stream;
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteEnumAsync(this Task<Stream> stream, object value, ISerializationContext context)
            => AsyncHelper.FluentAsync(stream, value, context, WriteEnumAsync);

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Enumeration type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream WriteEnumNullable<T>(this Stream stream, T? value, ISerializationContext context) where T : struct, Enum
            => value == null ? Write(stream, (byte)NumberTypes.IsNull, context) : WriteEnum(stream, value.Value, context);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream WriteEnumNullable(this Stream stream, object? value, ISerializationContext context)
            => value == null ? Write(stream, (byte)NumberTypes.IsNull, context) : WriteEnum(stream, value, context);

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Enumeration type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<Stream> WriteEnumNullableAsync<T>(this Stream stream, T? value, ISerializationContext context) where T : struct, Enum
            => value == null
                ? WriteAsync(stream, (byte)NumberTypes.IsNull, context)
                : WriteEnumAsync(stream, value.Value, context);

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Enumeration type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteEnumNullableAsync<T>(this Task<Stream> stream, T? value, ISerializationContext context) where T : struct, Enum
            => AsyncHelper.FluentAsync(stream, value, context, WriteEnumNullableAsync);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<Stream> WriteEnumNullableAsync(this Stream stream, object? value, ISerializationContext context)
            => value == null
                ? WriteAsync(stream, (byte)NumberTypes.IsNull, context)
                : WriteEnumAsync(stream, value, context);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteEnumNullableAsync(this Task<Stream> stream, object? value, ISerializationContext context)
            => AsyncHelper.FluentAsync(stream, value, context, WriteEnumNullableAsync);
    }
}
