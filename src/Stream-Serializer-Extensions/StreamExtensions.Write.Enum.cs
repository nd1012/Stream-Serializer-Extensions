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
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream WriteEnum<T>(this Stream stream, T value) where T : struct, Enum
        {
            if (ObjectHelper.AreEqual(value, default(T))) return Write(stream, (byte)NumberTypes.Default);
            return WriteNumber(stream, Convert.ChangeType(value, typeof(T).GetEnumUnderlyingType()));
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
        public static Stream WriteEnum(this Stream stream, object value)
        {
            Type enumType = value.GetType();
            SerializerException.Wrap(() => ArgumentValidationHelper.EnsureValidArgument(nameof(value), enumType.IsEnum, "Not an enumeration value"));
            if (ObjectHelper.AreEqual(value, Activator.CreateInstance(enumType))) return Write(stream, (byte)NumberTypes.Default);
            return WriteNumber(stream, Convert.ChangeType(value, enumType.GetEnumUnderlyingType()));
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Enumeration type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task WriteEnumAsync<T>(this Stream stream, T value, CancellationToken cancellationToken = default) where T : struct, Enum
        {
            if (ObjectHelper.AreEqual(value, default(T)))
            {
                await WriteAsync(stream, (byte)NumberTypes.Default, cancellationToken).DynamicContext();
                return;
            }
            await WriteNumberAsync(stream, Convert.ChangeType(value, value.GetType().GetEnumUnderlyingType()), cancellationToken).DynamicContext();
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task WriteEnumAsync(this Stream stream, object value, CancellationToken cancellationToken = default)
        {
            Type enumType = value.GetType();
            SerializerException.Wrap(() => ArgumentValidationHelper.EnsureValidArgument(nameof(value), enumType.IsEnum, "Not an enumeration value"));
            if (ObjectHelper.AreEqual(value, Activator.CreateInstance(enumType)))
            {
                await WriteAsync(stream, (byte)NumberTypes.Default, cancellationToken).DynamicContext();
            }
            else
            {
                await WriteNumberAsync(stream, Convert.ChangeType(value, enumType.GetEnumUnderlyingType()), cancellationToken).DynamicContext();
            }
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Enumeration type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream WriteEnumNullable<T>(this Stream stream, T? value) where T : struct, Enum
            => value == null ? Write(stream, (byte)NumberTypes.Null) : WriteEnum(stream, value.Value);

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
        public static Stream WriteEnumNullable(this Stream stream, object? value)
            => value == null ? Write(stream, (byte)NumberTypes.Null) : WriteEnum(stream, value);

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Enumeration type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task WriteEnumNullableAsync<T>(this Stream stream, T? value, CancellationToken cancellationToken = default) where T : struct, Enum
            => value == null
                ? WriteAsync(stream, (byte)NumberTypes.Null, cancellationToken)
                : WriteEnumAsync(stream, value.Value, cancellationToken);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task WriteEnumNullableAsync(this Stream stream, object? value, CancellationToken cancellationToken = default)
            => value == null
                ? WriteAsync(stream, (byte)NumberTypes.Null, cancellationToken)
                : WriteEnumAsync(stream, value, cancellationToken);
    }
}
