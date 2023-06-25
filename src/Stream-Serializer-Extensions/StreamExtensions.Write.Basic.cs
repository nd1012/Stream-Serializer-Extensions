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
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream Write(this Stream stream, bool value)
        {
            byte[] buffer = StreamSerializer.BufferPool.Rent(minimumLength: 1);
            buffer[0] = (byte)(value ? 1 : 0);
            return WriteSerializedData(stream, buffer, len: 1);
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<Stream> WriteAsync(this Stream stream, bool value, CancellationToken cancellationToken = default)
        {
            byte[] buffer = StreamSerializer.BufferPool.Rent(minimumLength: 1);
            buffer[0] = (byte)(value ? 1 : 0);
            return await WriteSerializedDataAsync(stream, buffer, len: 1, cancellationToken: cancellationToken).DynamicContext();
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteAsync(this Task<Stream> stream, bool value, CancellationToken cancellationToken = default)
            => FluentAsync(stream, (s) => WriteAsync(s, value, cancellationToken));

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
        public static Stream WriteNullable(this Stream stream, bool? value)
        {
            ObjectTypes type;
            if (value == null)
            {
                type = ObjectTypes.Null;
            }
            else if (value.Value)
            {
                type = ObjectTypes.Bool;
            }
            else
            {
                type = ObjectTypes.Empty;
            }
            return Write(stream, (byte)type);
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<Stream> WriteNullableAsync(this Stream stream, bool? value, CancellationToken cancellationToken = default)
        {
            ObjectTypes type;
            if (value == null)
            {
                type = ObjectTypes.Null;
            }
            else if (value.Value)
            {
                type = ObjectTypes.Bool;
            }
            else
            {
                type = ObjectTypes.Empty;
            }
            return WriteAsync(stream, (byte)type, cancellationToken);
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteNullableAsync(this Task<Stream> stream, bool? value, CancellationToken cancellationToken = default)
            => FluentAsync(stream, (s) => WriteNullableAsync(s, value, cancellationToken));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Stream Write(this Stream stream, sbyte value) => Write(stream, (byte)value);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteAsync(this Stream stream, sbyte value, CancellationToken cancellationToken = default)
            => WriteAsync(stream, (byte)value, cancellationToken);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteAsync(this Task<Stream> stream, sbyte value, CancellationToken cancellationToken = default)
            => FluentAsync(stream, (s) => WriteAsync(s, value, cancellationToken));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        public static Stream WriteNullable(this Stream stream, sbyte? value)
            => WriteNullableNumeric(stream, value, sbyte.MinValue, sbyte.MaxValue, () => Write(stream, value!.Value));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        public static Task<Stream> WriteNullableAsync(this Stream stream, sbyte? value, CancellationToken cancellationToken = default)
            => WriteNullableNumericAsync(stream, value, sbyte.MinValue, sbyte.MaxValue, () => WriteAsync(stream, value!.Value, cancellationToken), cancellationToken);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteNullableAsync(this Task<Stream> stream, sbyte? value, CancellationToken cancellationToken = default)
            => FluentAsync(stream, (s) => WriteNullableAsync(s, value, cancellationToken));

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
        public static Stream Write(this Stream stream, byte value)
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
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
#pragma warning disable IDE0060 // Remove unused parameter
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<Stream> WriteAsync(this Stream stream, byte value, CancellationToken cancellationToken = default)
#pragma warning restore IDE0060
        {
            await Task.Yield();
            return Write(stream, value);
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteAsync(this Task<Stream> stream, byte value, CancellationToken cancellationToken = default)
            => FluentAsync(stream, (s) => WriteAsync(s, value, cancellationToken));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        public static Stream WriteNullable(this Stream stream, byte? value)
            => WriteNullableNumeric(stream, value, byte.MinValue, byte.MaxValue, () => Write(stream, value!.Value));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        public static Task<Stream> WriteNullableAsync(this Stream stream, byte? value, CancellationToken cancellationToken = default)
            => WriteNullableNumericAsync(stream, value, byte.MinValue, byte.MaxValue, () => WriteAsync(stream, value!.Value, cancellationToken), cancellationToken);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteNullableAsync(this Task<Stream> stream, byte? value, CancellationToken cancellationToken = default)
            => FluentAsync(stream, (s) => WriteNullableAsync(s, value, cancellationToken));

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
        public static Stream Write(this Stream stream, short value)
            => WriteSerializedData(stream, value.GetBytes(StreamSerializer.BufferPool.Rent(sizeof(short))), sizeof(short));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<Stream> WriteAsync(this Stream stream, short value, CancellationToken cancellationToken = default)
            => WriteSerializedDataAsync(stream, value.GetBytes(StreamSerializer.BufferPool.Rent(sizeof(short))), sizeof(short), cancellationToken: cancellationToken);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteAsync(this Task<Stream> stream, short value, CancellationToken cancellationToken = default)
            => FluentAsync(stream, (s) => WriteAsync(s, value, cancellationToken));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        public static Stream WriteNullable(this Stream stream, short? value)
            => WriteNullableNumeric(stream, value, short.MinValue, short.MaxValue, () => Write(stream, value!.Value));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        public static Task<Stream> WriteNullableAsync(this Stream stream, short? value, CancellationToken cancellationToken = default)
            => WriteNullableNumericAsync(stream, value, short.MinValue, short.MaxValue, () => WriteAsync(stream, value!.Value, cancellationToken), cancellationToken);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteNullableAsync(this Task<Stream> stream, short? value, CancellationToken cancellationToken = default)
            => FluentAsync(stream, (s) => WriteNullableAsync(s, value, cancellationToken));

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
        public static Stream Write(this Stream stream, ushort value)
            => WriteSerializedData(stream, value.GetBytes(StreamSerializer.BufferPool.Rent(sizeof(ushort))), sizeof(ushort));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<Stream> WriteAsync(this Stream stream, ushort value, CancellationToken cancellationToken = default)
            => WriteSerializedDataAsync(stream, value.GetBytes(StreamSerializer.BufferPool.Rent(sizeof(ushort))), sizeof(ushort), cancellationToken: cancellationToken);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteAsync(this Task<Stream> stream, ushort value, CancellationToken cancellationToken = default)
            => FluentAsync(stream, (s) => WriteAsync(s, value, cancellationToken));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        public static Stream WriteNullable(this Stream stream, ushort? value)
            => WriteNullableNumeric(stream, value, ushort.MinValue, ushort.MaxValue, () => Write(stream, value!.Value));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        public static Task<Stream> WriteNullableAsync(this Stream stream, ushort? value, CancellationToken cancellationToken = default)
            => WriteNullableNumericAsync(stream, value, ushort.MinValue, ushort.MaxValue, () => WriteAsync(stream, value!.Value, cancellationToken), cancellationToken);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteNullableAsync(this Task<Stream> stream, ushort? value, CancellationToken cancellationToken = default)
            => FluentAsync(stream, (s) => WriteNullableAsync(s, value, cancellationToken));

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
        public static Stream Write(this Stream stream, int value)
            => WriteSerializedData(stream, value.GetBytes(StreamSerializer.BufferPool.Rent(sizeof(int))), sizeof(int));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<Stream> WriteAsync(this Stream stream, int value, CancellationToken cancellationToken = default)
            => WriteSerializedDataAsync(stream, value.GetBytes(StreamSerializer.BufferPool.Rent(sizeof(int))), sizeof(int), cancellationToken: cancellationToken);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteAsync(this Task<Stream> stream, int value, CancellationToken cancellationToken = default)
            => FluentAsync(stream, (s) => WriteAsync(s, value, cancellationToken));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        public static Stream WriteNullable(this Stream stream, int? value)
            => WriteNullableNumeric(stream, value, int.MinValue, int.MaxValue, () => Write(stream, value!.Value));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        public static Task<Stream> WriteNullableAsync(this Stream stream, int? value, CancellationToken cancellationToken = default)
            => WriteNullableNumericAsync(stream, value, int.MinValue, int.MaxValue, () => WriteAsync(stream, value!.Value, cancellationToken), cancellationToken);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteNullableAsync(this Task<Stream> stream, int? value, CancellationToken cancellationToken = default)
            => FluentAsync(stream, (s) => WriteNullableAsync(s, value, cancellationToken));

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
        public static Stream Write(this Stream stream, uint value)
            => WriteSerializedData(stream, value.GetBytes(StreamSerializer.BufferPool.Rent(sizeof(uint))), sizeof(uint));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<Stream> WriteAsync(this Stream stream, uint value, CancellationToken cancellationToken = default)
            => WriteSerializedDataAsync(stream, value.GetBytes(StreamSerializer.BufferPool.Rent(sizeof(uint))), sizeof(uint), cancellationToken: cancellationToken);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteAsync(this Task<Stream> stream, uint value, CancellationToken cancellationToken = default)
            => FluentAsync(stream, (s) => WriteAsync(s, value, cancellationToken));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        public static Stream WriteNullable(this Stream stream, uint? value)
            => WriteNullableNumeric(stream, value, uint.MinValue, uint.MaxValue, () => Write(stream, value!.Value));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        public static Task<Stream> WriteNullableAsync(this Stream stream, uint? value, CancellationToken cancellationToken = default)
            => WriteNullableNumericAsync(stream, value, uint.MinValue, uint.MaxValue, () => WriteAsync(stream, value!.Value, cancellationToken), cancellationToken);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteNullableAsync(this Task<Stream> stream, uint? value, CancellationToken cancellationToken = default)
            => FluentAsync(stream, (s) => WriteNullableAsync(s, value, cancellationToken));

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
        public static Stream Write(this Stream stream, long value)
            => WriteSerializedData(stream, value.GetBytes(StreamSerializer.BufferPool.Rent(sizeof(long))), sizeof(long));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<Stream> WriteAsync(this Stream stream, long value, CancellationToken cancellationToken = default)
            => WriteSerializedDataAsync(stream, value.GetBytes(StreamSerializer.BufferPool.Rent(sizeof(long))), sizeof(long), cancellationToken: cancellationToken);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteAsync(this Task<Stream> stream, long value, CancellationToken cancellationToken = default)
            => FluentAsync(stream, (s) => WriteAsync(s, value, cancellationToken));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        public static Stream WriteNullable(this Stream stream, long? value)
            => WriteNullableNumeric(stream, value, long.MinValue, long.MaxValue, () => Write(stream, value!.Value));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        public static Task<Stream> WriteNullableAsync(this Stream stream, long? value, CancellationToken cancellationToken = default)
            => WriteNullableNumericAsync(stream, value, long.MinValue, long.MaxValue, () => WriteAsync(stream, value!.Value, cancellationToken), cancellationToken);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteNullableAsync(this Task<Stream> stream, long? value, CancellationToken cancellationToken = default)
            => FluentAsync(stream, (s) => WriteNullableAsync(s, value, cancellationToken));

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
        public static Stream Write(this Stream stream, ulong value)
            => WriteSerializedData(stream, value.GetBytes(StreamSerializer.BufferPool.Rent(sizeof(ulong))), sizeof(ulong));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<Stream> WriteAsync(this Stream stream, ulong value, CancellationToken cancellationToken = default)
            => WriteSerializedDataAsync(stream, value.GetBytes(StreamSerializer.BufferPool.Rent(sizeof(ulong))), sizeof(ulong), cancellationToken: cancellationToken);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteAsync(this Task<Stream> stream, ulong value, CancellationToken cancellationToken = default)
            => FluentAsync(stream, (s) => WriteAsync(s, value, cancellationToken));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        public static Stream WriteNullable(this Stream stream, ulong? value)
            => WriteNullableNumeric(stream, value, ulong.MinValue, ulong.MaxValue, () => Write(stream, value!.Value));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        public static Task<Stream> WriteNullableAsync(this Stream stream, ulong? value, CancellationToken cancellationToken = default)
            => WriteNullableNumericAsync(stream, value, ulong.MinValue, ulong.MaxValue, () => WriteAsync(stream, value!.Value, cancellationToken), cancellationToken);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteNullableAsync(this Task<Stream> stream, ulong? value, CancellationToken cancellationToken = default)
            => FluentAsync(stream, (s) => WriteNullableAsync(s, value, cancellationToken));

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
        public static Stream Write(this Stream stream, float value)
            => WriteSerializedData(stream, value.GetBytes(StreamSerializer.BufferPool.Rent(sizeof(float))), sizeof(float));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<Stream> WriteAsync(this Stream stream, float value, CancellationToken cancellationToken = default)
            => WriteSerializedDataAsync(stream, value.GetBytes(StreamSerializer.BufferPool.Rent(sizeof(float))), sizeof(float), cancellationToken: cancellationToken);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteAsync(this Task<Stream> stream, float value, CancellationToken cancellationToken = default)
            => FluentAsync(stream, (s) => WriteAsync(s, value, cancellationToken));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        public static Stream WriteNullable(this Stream stream, float? value)
            => WriteNullableNumeric(stream, value, float.MinValue, float.MaxValue, () => Write(stream, value!.Value));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        public static Task<Stream> WriteNullableAsync(this Stream stream, float? value, CancellationToken cancellationToken = default)
            => WriteNullableNumericAsync(stream, value, float.MinValue, float.MaxValue, () => WriteAsync(stream, value!.Value, cancellationToken), cancellationToken);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteNullableAsync(this Task<Stream> stream, float? value, CancellationToken cancellationToken = default)
            => FluentAsync(stream, (s) => WriteNullableAsync(s, value, cancellationToken));

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
        public static Stream Write(this Stream stream, double value)
            => WriteSerializedData(stream, value.GetBytes(StreamSerializer.BufferPool.Rent(sizeof(double))), sizeof(double));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<Stream> WriteAsync(this Stream stream, double value, CancellationToken cancellationToken = default)
            => WriteSerializedDataAsync(stream, value.GetBytes(StreamSerializer.BufferPool.Rent(sizeof(double))), sizeof(double), cancellationToken: cancellationToken);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteAsync(this Task<Stream> stream, double value, CancellationToken cancellationToken = default)
            => FluentAsync(stream, (s) => WriteAsync(s, value, cancellationToken));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        public static Stream WriteNullable(this Stream stream, double? value)
            => WriteNullableNumeric(stream, value, double.MinValue, double.MaxValue, () => Write(stream, value!.Value));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        public static Task<Stream> WriteNullableAsync(this Stream stream, double? value, CancellationToken cancellationToken = default)
            => WriteNullableNumericAsync(stream, value, double.MinValue, double.MaxValue, () => WriteAsync(stream, value!.Value, cancellationToken), cancellationToken);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteNullableAsync(this Task<Stream> stream, double? value, CancellationToken cancellationToken = default)
            => FluentAsync(stream, (s) => WriteNullableAsync(s, value, cancellationToken));

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
        public static Stream Write(this Stream stream, decimal value)
            => WriteSerializedData(stream, value.GetBytes(StreamSerializer.BufferPool.Rent(sizeof(decimal))), sizeof(decimal));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<Stream> WriteAsync(this Stream stream, decimal value, CancellationToken cancellationToken = default)
            => WriteSerializedDataAsync(stream, value.GetBytes(StreamSerializer.BufferPool.Rent(sizeof(decimal))), sizeof(decimal), cancellationToken: cancellationToken);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteAsync(this Task<Stream> stream, decimal value, CancellationToken cancellationToken = default)
            => FluentAsync(stream, (s) => WriteAsync(s, value, cancellationToken));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        public static Stream WriteNullable(this Stream stream, decimal? value)
            => WriteNullableNumeric(stream, value, decimal.MinValue, decimal.MaxValue, () => Write(stream, value!.Value));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        public static Task<Stream> WriteNullableAsync(this Stream stream, decimal? value, CancellationToken cancellationToken = default)
            => WriteNullableNumericAsync(stream, value, decimal.MinValue, decimal.MaxValue, () => WriteAsync(stream, value!.Value, cancellationToken), cancellationToken);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteNullableAsync(this Task<Stream> stream, decimal? value, CancellationToken cancellationToken = default)
            => FluentAsync(stream, (s) => WriteNullableAsync(s, value, cancellationToken));

        /// <summary>
        /// Write a nullable numeric value
        /// </summary>
        /// <typeparam name="T">Numeric type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value</param>
        /// <param name="min">Minimum</param>
        /// <param name="max">Maximum</param>
        /// <param name="action">Action to execute if the value can't be handled using a default handler</param>
        /// <returns>Stream</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static Stream WriteNullableNumeric<T>(Stream stream, T? value, T min, T max, Action action) where T:struct, IConvertible
        {
            NumberTypes type;
            if (value == null) type = NumberTypes.Null;
            else if (value.Equals(default(T))) type = NumberTypes.Zero;
            else if (value.Equals(min)) type = NumberTypes.MinValue;
            else if (value.Equals(max)) type = NumberTypes.MaxValue;
            else type = NumberTypes.Default;
            Write(stream, (byte)type);
            if (type == NumberTypes.Default) action();
            return stream;
        }

        /// <summary>
        /// Write a nullable numeric value
        /// </summary>
        /// <typeparam name="T">Numeric type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value</param>
        /// <param name="min">Minimum</param>
        /// <param name="max">Maximum</param>
        /// <param name="action">Action to execute if the value can't be handled using a default handler</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static async Task<Stream> WriteNullableNumericAsync<T>(Stream stream, T? value, T min, T max, Func<Task> action, CancellationToken cancellationToken)
            where T : struct, IConvertible
        {
            NumberTypes type;
            if (value == null) type = NumberTypes.Null;
            else if (value.Equals(default(T))) type = NumberTypes.Zero;
            else if (value.Equals(min)) type = NumberTypes.MinValue;
            else if (value.Equals(max)) type = NumberTypes.MaxValue;
            else type = NumberTypes.Default;
            await WriteAsync(stream, (byte)type, cancellationToken).DynamicContext();
            if (type == NumberTypes.Default) await action().DynamicContext();
            return stream;
        }
    }
}
