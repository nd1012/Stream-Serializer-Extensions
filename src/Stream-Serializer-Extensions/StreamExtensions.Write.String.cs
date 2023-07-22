using System.Runtime;
using System.Runtime.CompilerServices;
using System.Text;
using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    // String
    public static partial class StreamExtensions
    {
        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static Stream WriteString(this Stream stream, string value, ISerializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            => WriteString(context, value, lenShift: 2, (buffer) => value.GetBytes(buffer));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        public static Task<Stream> WriteStringAsync(this Stream stream, string value, ISerializationContext context)
            => WriteStringAsync(context, value, lenShift: 2, (buffer) => value.GetBytes(buffer));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteStringAsync(this Task<Stream> stream, string value, ISerializationContext context)
            => AsyncHelper.FluentAsync(stream, value, context, WriteStringAsync);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static Stream WriteStringNullable(this Stream stream, string? value, ISerializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            => WriteNullableString(context, value, lenShift: 2, (buffer) => value!.GetBytes(buffer));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        public static Task<Stream> WriteStringNullableAsync(this Stream stream, string? value, ISerializationContext context)
            => WriteNullableStringAsync(context, value, lenShift: 2, (buffer) => value!.GetBytes(buffer));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteStringNullableAsync(this Task<Stream> stream, string? value, ISerializationContext context)
            => AsyncHelper.FluentAsync(stream, value, context, WriteStringNullableAsync);

        /// <summary>
        /// Write UTF-16 (little endian) string
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static Stream WriteString16(this Stream stream, string value, ISerializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            => WriteString(context, value, lenShift: 1, (buffer) => value.GetBytes16(buffer));

        /// <summary>
        /// Write UTF-16 (little endian) string
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        public static Task<Stream> WriteString16Async(this Stream stream, string value, ISerializationContext context)
            => WriteStringAsync(context, value, lenShift: 1, (buffer) => value.GetBytes16(buffer));

        /// <summary>
        /// Write UTF-16 (little endian) string
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteString16Async(this Task<Stream> stream, string value, ISerializationContext context)
            => AsyncHelper.FluentAsync(stream, value, context, WriteString16Async);

        /// <summary>
        /// Write UTF-16 (little endian) string
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static Stream WriteString16Nullable(this Stream stream, string? value, ISerializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            => WriteNullableString(context, value, lenShift: 1, (buffer) => value!.GetBytes16(buffer));

        /// <summary>
        /// Write UTF-16 (little endian) string
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        public static Task<Stream> WriteString16NullableAsync(this Stream stream, string? value, ISerializationContext context)
            => WriteNullableStringAsync(context, value, lenShift: 1, (buffer) => value!.GetBytes16(buffer));

        /// <summary>
        /// Write UTF-16 (little endian) string
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteString16NullableAsync(this Task<Stream> stream, string? value, ISerializationContext context)
            => AsyncHelper.FluentAsync(stream, value, context, WriteString16NullableAsync);

        /// <summary>
        /// Write UTF-32 (little endian) string
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static Stream WriteString32(this Stream stream, string value, ISerializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            => WriteString(context, value, lenShift: 2, (buffer) => value.GetBytes32(buffer));

        /// <summary>
        /// Write UTF-32 (little endian) string
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        public static Task<Stream> WriteString32Async(this Stream stream, string value, ISerializationContext context)
            => WriteStringAsync(context, value, lenShift: 2, (buffer) => value.GetBytes32(buffer));

        /// <summary>
        /// Write UTF-32 (little endian) string
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteString32Async(this Task<Stream> stream, string value, ISerializationContext context)
            => AsyncHelper.FluentAsync(stream, value, context, WriteString32Async);

        /// <summary>
        /// Write UTF-32 (little endian) string
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static Stream WriteString32Nullable(this Stream stream, string? value, ISerializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            => WriteNullableString(context, value, lenShift: 2, (buffer) => value!.GetBytes32(buffer));

        /// <summary>
        /// Write UTF-32 (little endian) string
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        public static Task<Stream> WriteString32NullableAsync(this Stream stream, string? value, ISerializationContext context)
            => WriteNullableStringAsync(context, value, lenShift: 2, (buffer) => value!.GetBytes32(buffer));

        /// <summary>
        /// Write UTF-32 (little endian) string
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteString32NullableAsync(this Task<Stream> stream, string? value, ISerializationContext context)
            => AsyncHelper.FluentAsync(stream, value, context, WriteString32NullableAsync);

        /// <summary>
        /// Write a string
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="value">Value</param>
        /// <param name="lenShift">Buffer length bit-shifting value</param>
        /// <param name="action">Writing action to execute</param>
        /// <returns>Stream</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [SkipLocalsInit]
        private static Stream WriteString(ISerializationContext context, string value, int lenShift, StringWriter_Delegate action)
        {
            int len = value == null ? 0 : value.Length << lenShift;
            if (len <= Settings.StackAllocBorder)
            {
                Span<byte> buffer = (value?.Length ?? 0) == 0 ? Array.Empty<byte>() : stackalloc byte[len];
                len = value == null ? 0 : action(buffer);
                if (context.TryWriteCachedCountable(value, value == null ? null : len)) return context.Stream;
                context.Stream.Write(buffer[..action(buffer)]);
                return context.Stream;
            }
            else
            {
                byte[] data = StreamSerializer.BufferPool.Rent(len);
                try
                {
                    len = action(data);
                    return context.TryWriteCachedCountable(value, len)
                        ? context.Stream
                        : WriteSerializedData(context, data, len);
                }
                finally
                {
                    StreamSerializer.BufferPool.Return(data);
                }
            }
        }

        /// <summary>
        /// Write a string
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="value">Value</param>
        /// <param name="lenShift">Buffer length bit-shifting value</param>
        /// <param name="action">Writing action to execut</param>
        /// <returns>Stream</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static async Task<Stream> WriteStringAsync(ISerializationContext context, string value, int lenShift, StringWriter_Delegate action)
        {
            byte[] data = (value?.Length ?? 0) == 0 ? Array.Empty<byte>() : StreamSerializer.BufferPool.Rent(value!.Length << lenShift);
            try
            {
                int len = value == null ? 0 : action(data);
                if (await context.TryWriteCachedCountableAsync(value, value == null ? null : len).DynamicContext()) return context.Stream;
                await WriteSerializedDataAsync(context, data, len).DynamicContext();
            }
            finally
            {
                if (data.Length != 0) StreamSerializer.BufferPool.Return(data);
            }
            return context.Stream;
        }

        /// <summary>
        /// Write a nullable string
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="value">Value</param>
        /// <param name="lenShift">Buffer length bit-shifting value</param>
        /// <param name="action">Writing action to execute if the string isn't <see langword="null"/></param>
        /// <returns>Stream</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [SkipLocalsInit]
        private static Stream WriteNullableString(ISerializationContext context, string? value, int lenShift, StringWriter_Delegate action)
        {
            int len = value == null ? 0 : value.Length << lenShift;
            if (len <= Settings.StackAllocBorder)
            {
                Span<byte> buffer = (value?.Length ?? 0) == 0 ? Array.Empty<byte>() : stackalloc byte[len];
                len = value == null ? 0 : action(buffer);
                if (context.TryWriteCachedCountable(value, value == null ? null : len)) return context.Stream;
                context.Stream.Write(buffer[..len]);
                return context.Stream;
            }
            else
            {
                byte[] data = StreamSerializer.BufferPool.Rent(len);
                try
                {
                    len = action(data);
                    return context.TryWriteCachedCountable(value, len)
                        ? context.Stream
                        : WriteSerializedData(context, data, len);
                }
                finally
                {
                    StreamSerializer.BufferPool.Return(data);
                }
            }
        }

        /// <summary>
        /// Write a nullable string
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="value">Value</param>
        /// <param name="lenShift">Buffer length bit-shifting value</param>
        /// <param name="action">Writing action to execute if the string isn't <see langword="null"/></param>
        /// <returns>Stream</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static async Task<Stream> WriteNullableStringAsync(ISerializationContext context, string? value, int lenShift, StringWriter_Delegate action)
        {
            byte[] data = (value?.Length ?? 0) == 0 ? Array.Empty<byte>() : StreamSerializer.BufferPool.Rent(value!.Length << lenShift);
            try
            {
                int len = (value?.Length ?? 0) == 0 ? 0 : action(data);
                if (await context.TryWriteCachedCountableAsync(value, value == null ? null : len).DynamicContext()) return context.Stream;
                await WriteSerializedDataAsync(context, data, len).DynamicContext();
            }
            finally
            {
                if (data.Length != 0) StreamSerializer.BufferPool.Return(data);
            }
            return context.Stream;
        }

        /// <summary>
        /// Delegate for a string writer
        /// </summary>
        /// <param name="buffer">Buffer</param>
        /// <returns>Number of used bytes</returns>
        private delegate int StringWriter_Delegate(Span<byte> buffer);
    }
}
