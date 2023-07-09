using System.Runtime;
using System.Runtime.CompilerServices;
using System.Text;
using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    // Basic types
    public static partial class StreamExtensions
    {
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
#pragma warning disable IDE0060 // Remove unused argument
        public static Stream Write(this Stream stream, bool value, ISerializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
        {
            stream.WriteByte((byte)(value ? 1 : 0));
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
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<Stream> WriteAsync(this Stream stream, bool value, ISerializationContext context)
        {
            stream.WriteByte((byte)(value ? 1 : 0));
            return Task.FromResult(stream);
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
        public static Task<Stream> WriteAsync(this Task<Stream> stream, bool value, ISerializationContext context)
            => AsyncHelper.FluentAsync(stream, value, context, WriteAsync);

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
#pragma warning disable IDE0060 // Remove unused argument
        public static Stream WriteNullable(this Stream stream, bool? value, ISerializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
        {
            ObjectTypes type;
            if (value == null)
            {
                type = ObjectTypes.Null;
            }
            else if (value.Value)
            {
                type = ObjectTypes.True;
            }
            else
            {
                type = ObjectTypes.False;
            }
            stream.WriteByte((byte)type);
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
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<Stream> WriteNullableAsync(this Stream stream, bool? value, ISerializationContext context)
        {
            ObjectTypes type;
            if (value == null)
            {
                type = ObjectTypes.Null;
            }
            else if (value.Value)
            {
                type = ObjectTypes.True;
            }
            else
            {
                type = ObjectTypes.False;
            }
            stream.WriteByte((byte)type);
            return Task.FromResult(stream);
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
        public static Task<Stream> WriteNullableAsync(this Task<Stream> stream, bool? value, ISerializationContext context)
            => AsyncHelper.FluentAsync(stream, value, context, WriteNullableAsync);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Stream Write(this Stream stream, sbyte value, ISerializationContext context) => Write(stream, (byte)value, context);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteAsync(this Stream stream, sbyte value, ISerializationContext context)
            => WriteAsync(stream, (byte)value, context);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteAsync(this Task<Stream> stream, sbyte value, ISerializationContext context)
            => AsyncHelper.FluentAsync(stream, value, context, WriteAsync);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        public static Stream WriteNullable(this Stream stream, sbyte? value, ISerializationContext context)
            => WriteNullableNumeric(context, sizeof(sbyte), value, sbyte.MinValue, sbyte.MaxValue, () => Write(stream, value!.Value, context));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        public static Task<Stream> WriteNullableAsync(this Stream stream, sbyte? value, ISerializationContext context)
            => WriteNullableNumericAsync(context, sizeof(sbyte), value, sbyte.MinValue, sbyte.MaxValue, () => WriteAsync(stream, value!.Value, context));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteNullableAsync(this Task<Stream> stream, sbyte? value, ISerializationContext context)
            => AsyncHelper.FluentAsync(stream, value, context, WriteNullableAsync);

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
#pragma warning disable IDE0060 // Remove unused argument
        public static Stream Write(this Stream stream, byte value, ISerializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            => SerializerException.Wrap(() =>
            {
                stream.WriteByte(value);
                return stream;
            });

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
        public static Task<Stream> WriteAsync(this Stream stream, byte value, ISerializationContext context)
            => SerializerException.Wrap(() =>
            {
                stream.WriteByte(value);
                return Task.FromResult(stream);
            });

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteAsync(this Task<Stream> stream, byte value, ISerializationContext context)
            => AsyncHelper.FluentAsync(stream, value, context, WriteAsync);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        public static Stream WriteNullable(this Stream stream, byte? value, ISerializationContext context)
            => WriteNullableNumeric(context, sizeof(byte), value, byte.MinValue, byte.MaxValue, () => Write(stream, value!.Value, context));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        public static Task<Stream> WriteNullableAsync(this Stream stream, byte? value, ISerializationContext context)
            => WriteNullableNumericAsync(context, sizeof(byte), value, byte.MinValue, byte.MaxValue, () => WriteAsync(stream, value!.Value, context));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteNullableAsync(this Task<Stream> stream, byte? value, ISerializationContext context)
            => AsyncHelper.FluentAsync(stream, value, context, WriteNullableAsync);

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
#pragma warning disable // Remove unused argument
        public static Stream Write(this Stream stream, short value, ISerializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            => WriteNumeric(context, (buffer) => value.GetBytes(buffer), sizeof(short));

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
        public static Task<Stream> WriteAsync(this Stream stream, short value, ISerializationContext context)
            => WriteSerializedDataAsync(context, value.GetBytes(StreamSerializer.BufferPool.Rent(sizeof(short))), sizeof(short));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteAsync(this Task<Stream> stream, short value, ISerializationContext context)
            => AsyncHelper.FluentAsync(stream, value, context, WriteAsync);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        public static Stream WriteNullable(this Stream stream, short? value, ISerializationContext context)
            => WriteNullableNumeric(context, sizeof(short), value, short.MinValue, short.MaxValue, () => Write(stream, value!.Value, context));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        public static Task<Stream> WriteNullableAsync(this Stream stream, short? value, ISerializationContext context)
            => WriteNullableNumericAsync(context, sizeof(short), value, short.MinValue, short.MaxValue, () => WriteAsync(stream, value!.Value, context));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteNullableAsync(this Task<Stream> stream, short? value, ISerializationContext context)
            => AsyncHelper.FluentAsync(stream, value, context, WriteNullableAsync);

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
#pragma warning disable IDE0060 // Remove unused argument
        public static Stream Write(this Stream stream, ushort value, ISerializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            => WriteNumeric(context, (buffer) => value.GetBytes(buffer), sizeof(ushort));

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
        public static Task<Stream> WriteAsync(this Stream stream, ushort value, ISerializationContext context)
            => WriteSerializedDataAsync(context, value.GetBytes(StreamSerializer.BufferPool.Rent(sizeof(ushort))), sizeof(ushort));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteAsync(this Task<Stream> stream, ushort value, ISerializationContext context)
            => AsyncHelper.FluentAsync(stream, value, context, WriteAsync);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        public static Stream WriteNullable(this Stream stream, ushort? value, ISerializationContext context)
            => WriteNullableNumeric(context, sizeof(ushort), value, ushort.MinValue, ushort.MaxValue, () => Write(stream, value!.Value, context));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        public static Task<Stream> WriteNullableAsync(this Stream stream, ushort? value, ISerializationContext context)
            => WriteNullableNumericAsync(context, sizeof(ushort), value, ushort.MinValue, ushort.MaxValue, () => WriteAsync(stream, value!.Value, context));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteNullableAsync(this Task<Stream> stream, ushort? value, ISerializationContext context)
            => AsyncHelper.FluentAsync(stream, value, context, WriteNullableAsync);

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
#pragma warning disable IDE0060 // Remove unused argument
        public static Stream Write(this Stream stream, int value, ISerializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            => WriteNumeric(context, (buffer) => value.GetBytes(buffer), sizeof(int));

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
        public static Task<Stream> WriteAsync(this Stream stream, int value, ISerializationContext context)
            => WriteSerializedDataAsync(context, value.GetBytes(StreamSerializer.BufferPool.Rent(sizeof(int))), sizeof(int));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteAsync(this Task<Stream> stream, int value, ISerializationContext context)
            => AsyncHelper.FluentAsync(stream, value, context, WriteAsync);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        public static Stream WriteNullable(this Stream stream, int? value, ISerializationContext context)
            => WriteNullableNumeric(context, sizeof(int), value, int.MinValue, int.MaxValue, () => Write(stream, value!.Value, context));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        public static Task<Stream> WriteNullableAsync(this Stream stream, int? value, ISerializationContext context)
            => WriteNullableNumericAsync(context, sizeof(int), value, int.MinValue, int.MaxValue, () => WriteAsync(stream, value!.Value, context));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteNullableAsync(this Task<Stream> stream, int? value, ISerializationContext context)
            => AsyncHelper.FluentAsync(stream, value, context, WriteNullableAsync);

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
#pragma warning disable IDE0060 // Remove unused argument
        public static Stream Write(this Stream stream, uint value, ISerializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            => WriteNumeric(context, (buffer) => value.GetBytes(buffer), sizeof(uint));

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
        public static Task<Stream> WriteAsync(this Stream stream, uint value, ISerializationContext context)
            => WriteSerializedDataAsync(context, value.GetBytes(StreamSerializer.BufferPool.Rent(sizeof(uint))), sizeof(uint));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteAsync(this Task<Stream> stream, uint value, ISerializationContext context)
            => AsyncHelper.FluentAsync(stream, value, context, WriteAsync);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        public static Stream WriteNullable(this Stream stream, uint? value, ISerializationContext context)
            => WriteNullableNumeric(context, sizeof(uint), value, uint.MinValue, uint.MaxValue, () => Write(stream, value!.Value, context));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        public static Task<Stream> WriteNullableAsync(this Stream stream, uint? value, ISerializationContext context)
            => WriteNullableNumericAsync(context, sizeof(uint), value, uint.MinValue, uint.MaxValue, () => WriteAsync(stream, value!.Value, context));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteNullableAsync(this Task<Stream> stream, uint? value, ISerializationContext context)
            => AsyncHelper.FluentAsync(stream, value, context, WriteNullableAsync);

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
#pragma warning disable IDE0060 // Remove unused argument
        public static Stream Write(this Stream stream, long value, ISerializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            => WriteNumeric(context, (buffer) => value.GetBytes(buffer), sizeof(long));

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
        public static Task<Stream> WriteAsync(this Stream stream, long value, ISerializationContext context)
            => WriteSerializedDataAsync(context, value.GetBytes(StreamSerializer.BufferPool.Rent(sizeof(long))), sizeof(long));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteAsync(this Task<Stream> stream, long value, ISerializationContext context)
            => AsyncHelper.FluentAsync(stream, value, context, WriteAsync);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        public static Stream WriteNullable(this Stream stream, long? value, ISerializationContext context)
            => WriteNullableNumeric(context, sizeof(long), value, long.MinValue, long.MaxValue, () => Write(stream, value!.Value, context));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        public static Task<Stream> WriteNullableAsync(this Stream stream, long? value, ISerializationContext context)
            => WriteNullableNumericAsync(context, sizeof(long), value, long.MinValue, long.MaxValue, () => WriteAsync(stream, value!.Value, context));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteNullableAsync(this Task<Stream> stream, long? value, ISerializationContext context)
            => AsyncHelper.FluentAsync(stream, value, context, WriteNullableAsync);

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
#pragma warning disable IDE0060 // Remove unused argument
        public static Stream Write(this Stream stream, ulong value, ISerializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            => WriteNumeric(context, (buffer) => value.GetBytes(buffer), sizeof(ulong));

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
        public static Task<Stream> WriteAsync(this Stream stream, ulong value, ISerializationContext context)
            => WriteSerializedDataAsync(context, value.GetBytes(StreamSerializer.BufferPool.Rent(sizeof(ulong))), sizeof(ulong));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteAsync(this Task<Stream> stream, ulong value, ISerializationContext context)
            => AsyncHelper.FluentAsync(stream, value, context, WriteAsync);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        public static Stream WriteNullable(this Stream stream, ulong? value, ISerializationContext context)
            => WriteNullableNumeric(context, sizeof(ulong), value, ulong.MinValue, ulong.MaxValue, () => Write(stream, value!.Value, context));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        public static Task<Stream> WriteNullableAsync(this Stream stream, ulong? value, ISerializationContext context)
            => WriteNullableNumericAsync(context, sizeof(ulong), value, ulong.MinValue, ulong.MaxValue, () => WriteAsync(stream, value!.Value, context));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteNullableAsync(this Task<Stream> stream, ulong? value, ISerializationContext context)
            => AsyncHelper.FluentAsync(stream, value, context, WriteNullableAsync);

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
#pragma warning disable IDE0060 // Remove unused argument
        public static Stream Write(this Stream stream, float value, ISerializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            => WriteNumeric(context, (buffer) => value.GetBytes(buffer), sizeof(float));

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
        public static Task<Stream> WriteAsync(this Stream stream, float value, ISerializationContext context)
            => WriteSerializedDataAsync(context, value.GetBytes(StreamSerializer.BufferPool.Rent(sizeof(float))), sizeof(float));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteAsync(this Task<Stream> stream, float value, ISerializationContext context)
            => AsyncHelper.FluentAsync(stream, value, context, WriteAsync);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        public static Stream WriteNullable(this Stream stream, float? value, ISerializationContext context)
            => WriteNullableNumeric(context, sizeof(float), value, float.MinValue, float.MaxValue, () => Write(stream, value!.Value, context));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        public static Task<Stream> WriteNullableAsync(this Stream stream, float? value, ISerializationContext context)
            => WriteNullableNumericAsync(context, sizeof(float), value, float.MinValue, float.MaxValue, () => WriteAsync(stream, value!.Value, context));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteNullableAsync(this Task<Stream> stream, float? value, ISerializationContext context)
            => AsyncHelper.FluentAsync(stream, value, context, WriteNullableAsync);

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
#pragma warning disable IDE0060 // Remove unused argument
        public static Stream Write(this Stream stream, double value, ISerializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            => WriteNumeric(context, (buffer) => value.GetBytes(buffer), sizeof(double));

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
        public static Task<Stream> WriteAsync(this Stream stream, double value, ISerializationContext context)
            => WriteSerializedDataAsync(context, value.GetBytes(StreamSerializer.BufferPool.Rent(sizeof(double))), sizeof(double));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteAsync(this Task<Stream> stream, double value, ISerializationContext context)
            => AsyncHelper.FluentAsync(stream, value, context, WriteAsync);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        public static Stream WriteNullable(this Stream stream, double? value, ISerializationContext context)
            => WriteNullableNumeric(context, sizeof(double), value, double.MinValue, double.MaxValue, () => Write(stream, value!.Value, context));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        public static Task<Stream> WriteNullableAsync(this Stream stream, double? value, ISerializationContext context)
            => WriteNullableNumericAsync(context, sizeof(double), value, double.MinValue, double.MaxValue, () => WriteAsync(stream, value!.Value, context));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteNullableAsync(this Task<Stream> stream, double? value, ISerializationContext context)
            => AsyncHelper.FluentAsync(stream, value, context, WriteNullableAsync);

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
#pragma warning disable IDE0060 // Remove unused argument
        public static Stream Write(this Stream stream, decimal value, ISerializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            => WriteNumeric(context, (buffer) => value.GetBytes(buffer), sizeof(decimal));

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
        public static Task<Stream> WriteAsync(this Stream stream, decimal value, ISerializationContext context)
            => WriteSerializedDataAsync(context, value.GetBytes(StreamSerializer.BufferPool.Rent(sizeof(decimal))), sizeof(decimal));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteAsync(this Task<Stream> stream, decimal value, ISerializationContext context)
            => AsyncHelper.FluentAsync(stream, value, context, WriteAsync);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        public static Stream WriteNullable(this Stream stream, decimal? value, ISerializationContext context)
            => WriteNullableNumeric(context, sizeof(decimal), value, decimal.MinValue, decimal.MaxValue, () => Write(stream, value!.Value, context));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        public static Task<Stream> WriteNullableAsync(this Stream stream, decimal? value, ISerializationContext context)
            => WriteNullableNumericAsync(context, sizeof(decimal), value, decimal.MinValue, decimal.MaxValue, () => WriteAsync(stream, value!.Value, context));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteNullableAsync(this Task<Stream> stream, decimal? value, ISerializationContext context)
            => AsyncHelper.FluentAsync(stream, value, context, WriteNullableAsync);

        /// <summary>
        /// Write a numeric value
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="action">Action to write the values bytes into a buffer</param>
        /// <param name="size">Type size in bytes</param>
        /// <returns>Stream</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [SkipLocalsInit]
        private static Stream WriteNumeric(ISerializationContext context, NumericWriter_Delegate action, int size)
        {
            Span<byte> buffer = stackalloc byte[size];
            action(buffer);
            context.Stream.Write(buffer);
            return context.Stream;
        }

        /// <summary>
        /// Write a nullable numeric value
        /// </summary>
        /// <typeparam name="T">Numeric type</typeparam>
        /// <param name="context">Context</param>
        /// <param name="size">Type size in bytes</param>
        /// <param name="value">Value</param>
        /// <param name="min">Minimum</param>
        /// <param name="max">Maximum</param>
        /// <param name="action">Action to execute if the value can't be handled using a default handler</param>
        /// <returns>Stream</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static Stream WriteNullableNumeric<T>(ISerializationContext context, int size, T? value, T min, T max, NullableNumericWriter_Delegate action)
            where T : struct, IConvertible
        {
            NumberTypes type;
            if (value == null) type = NumberTypes.IsNull;
            else if (value.Equals(default(T))) type = NumberTypes.Zero;
            else if (value.Equals(min)) type = NumberTypes.MinValue;
            else if (value.Equals(max)) type = NumberTypes.MaxValue;
            else type = NumberTypes.Default;
            if (size > 2 && context.TryWriteCached(value, type)) return context.Stream;
            if (size < 3) Write(context.Stream, (byte)type, context);
            if (type == NumberTypes.Default) action();
            return context.Stream;
        }

        /// <summary>
        /// Write a nullable numeric value
        /// </summary>
        /// <typeparam name="T">Numeric type</typeparam>
        /// <param name="context">Context</param>
        /// <param name="size">Type size in bytes</param>
        /// <param name="value">Value</param>
        /// <param name="min">Minimum</param>
        /// <param name="max">Maximum</param>
        /// <param name="action">Action to execute if the value can't be handled using a default handler</param>
        /// <returns>Stream</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static async Task<Stream> WriteNullableNumericAsync<T>(ISerializationContext context, int size, T? value, T min, T max, AsyncNullableNumericWriter_Delegate action)
            where T : struct, IConvertible
        {
            NumberTypes type;
            if (value == null) type = NumberTypes.IsNull;
            else if (value.Equals(default(T))) type = NumberTypes.Zero;
            else if (value.Equals(min)) type = NumberTypes.MinValue;
            else if (value.Equals(max)) type = NumberTypes.MaxValue;
            else type = NumberTypes.Default;
            if (size > 2 && await context.TryWriteCachedAsync(value, type).DynamicContext()) return context.Stream;
            if (size < 3) await WriteAsync(context.Stream, (byte)type, context).DynamicContext();
            if (type == NumberTypes.Default) await action().DynamicContext();
            return context.Stream;
        }

        /// <summary>
        /// Delegate for a numeric writer
        /// </summary>
        /// <param name="buffer">Buffer</param>
        private delegate void NumericWriter_Delegate(Span<byte> buffer);

        /// <summary>
        /// Delegate for a nullable numeric writer
        /// </summary>
        private delegate void NullableNumericWriter_Delegate();

        /// <summary>
        /// Delegate for a nullable numeric writer
        /// </summary>
        /// <returns></returns>
        private delegate Task AsyncNullableNumericWriter_Delegate();
    }
}
