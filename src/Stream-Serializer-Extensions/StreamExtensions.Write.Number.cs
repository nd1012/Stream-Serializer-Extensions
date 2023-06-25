using System.Runtime;
using System.Runtime.CompilerServices;
using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    // Number
    public static partial class StreamExtensions
    {
        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Number type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        public static Stream WriteNumber<T>(this Stream stream, T value) where T : struct, IConvertible
            => WriteNumberInt(stream, value, type: null);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        public static Stream WriteNumber(this Stream stream, object value) => WriteNumberInt(stream, value, type: null);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="type">Number type</param>
        /// <returns>Stream</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static Stream WriteNumberInt(Stream stream, object value, NumberTypes? type)
            => SerializerException.Wrap(() =>
            {
                if (type == null) (_, type) = value.GetNumberAndType();
                using RentedArray<byte> poolData = new(1, clean: false);
                poolData[0] = (byte)type;
                stream.Write(poolData.Span);
                if (!type.Value.IsZero() && !type.Value.HasValueFlags())
                    switch (type)
                    {
                        case NumberTypes.Byte:
                        case NumberTypes.Byte | NumberTypes.Unsigned:
                            poolData[0] = value.ConvertType<byte>();
                            stream.Write(poolData.Span);
                            break;
                        case NumberTypes.Short:
                            Write(stream, value.ConvertType<short>());
                            break;
                        case NumberTypes.Short | NumberTypes.Unsigned:
                            Write(stream, value.ConvertType<ushort>());
                            break;
                        case NumberTypes.Int:
                            Write(stream, value.ConvertType<int>());
                            break;
                        case NumberTypes.Int | NumberTypes.Unsigned:
                            Write(stream, value.ConvertType<uint>());
                            break;
                        case NumberTypes.Long:
                            Write(stream, value.ConvertType<long>());
                            break;
                        case NumberTypes.Long | NumberTypes.Unsigned:
                            Write(stream, value.ConvertType<ulong>());
                            break;
                        case NumberTypes.Float:
                            Write(stream, value.ConvertType<float>());
                            break;
                        case NumberTypes.Double:
                            Write(stream, value.ConvertType<double>());
                            break;
                        case NumberTypes.Decimal:
                            Write(stream, value.ConvertType<decimal>());
                            break;
                    }
                return stream;
            });

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Number type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        public static Task<Stream> WriteNumberAsync<T>(this Stream stream, T value, CancellationToken cancellationToken = default) where T : struct, IConvertible
            => WriteNumberIntAsync(stream, value, type: null, cancellationToken);

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Number type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteNumberAsync<T>(this Task<Stream> stream, T value, CancellationToken cancellationToken = default) where T : struct, IConvertible
            => FluentAsync(stream, (s) => WriteNumberAsync(s, value, cancellationToken));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        public static Task<Stream> WriteNumberAsync(this Stream stream, object value, CancellationToken cancellationToken = default)
            => WriteNumberIntAsync(stream, value, type: null, cancellationToken);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteNumberAsync(this Task<Stream> stream, object value, CancellationToken cancellationToken = default)
            => FluentAsync(stream, (s) => WriteNumberAsync(s, value, cancellationToken));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="type">Number type</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static Task<Stream> WriteNumberIntAsync(Stream stream, object value, NumberTypes? type, CancellationToken cancellationToken)
            => SerializerException.WrapAsync(async () =>
            {
                if (type == null) (_, type) = value.GetNumberAndType();
                using RentedArray<byte> poolData = new(1, clean: false);
                poolData[0] = (byte)type;
                await stream.WriteAsync(poolData.Memory, cancellationToken).DynamicContext();
                if (!type.Value.IsZero() && !type.Value.HasValueFlags())
                    switch (type)
                    {
                        case NumberTypes.Byte:
                        case NumberTypes.Byte | NumberTypes.Unsigned:
                            poolData[0] = value.ConvertType<byte>();
                            await stream.WriteAsync(poolData.Memory, cancellationToken).DynamicContext();
                            break;
                        case NumberTypes.Short:
                            await WriteAsync(stream, value.ConvertType<short>(), cancellationToken).DynamicContext();
                            break;
                        case NumberTypes.Short | NumberTypes.Unsigned:
                            await WriteAsync(stream, value.ConvertType<ushort>(), cancellationToken).DynamicContext();
                            break;
                        case NumberTypes.Int:
                            await WriteAsync(stream, value.ConvertType<int>(), cancellationToken).DynamicContext();
                            break;
                        case NumberTypes.Int | NumberTypes.Unsigned:
                            await WriteAsync(stream, value.ConvertType<uint>(), cancellationToken).DynamicContext();
                            break;
                        case NumberTypes.Long:
                            await WriteAsync(stream, value.ConvertType<long>(), cancellationToken).DynamicContext();
                            break;
                        case NumberTypes.Long | NumberTypes.Unsigned:
                            await WriteAsync(stream, value.ConvertType<ulong>(), cancellationToken).DynamicContext();
                            break;
                        case NumberTypes.Float:
                            await WriteAsync(stream, value.ConvertType<float>(), cancellationToken).DynamicContext();
                            break;
                        case NumberTypes.Double:
                            await WriteAsync(stream, value.ConvertType<double>(), cancellationToken).DynamicContext();
                            break;
                        case NumberTypes.Decimal:
                            await WriteAsync(stream, value.ConvertType<decimal>(), cancellationToken).DynamicContext();
                            break;
                    }
                return stream;
            });

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Number type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream WriteNumberNullable<T>(this Stream stream, T? value) where T : struct, IConvertible
            => value == null ? Write(stream, (byte)NumberTypes.Null) : WriteNumber(stream, value.Value);

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
        public static Stream WriteNumberNullable(this Stream stream, object? value)
            => value == null ? Write(stream, (byte)NumberTypes.Null) : WriteNumber(stream, value);

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Number type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<Stream> WriteNumberNullableAsync<T>(this Stream stream, T? value, CancellationToken cancellationToken = default)
            where T : struct, IConvertible
            => value == null
                ? WriteAsync(stream, (byte)NumberTypes.Null, cancellationToken)
                : WriteNumberAsync(stream, value.Value, cancellationToken);

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Number type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteNumberNullableAsync<T>(this Task<Stream> stream, T? value, CancellationToken cancellationToken = default)
            where T : struct, IConvertible
            => FluentAsync(stream, (s) => WriteNumberNullableAsync(s, value, cancellationToken));

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
        public static Task<Stream> WriteNumberNullableAsync(this Stream stream, object? value, CancellationToken cancellationToken = default)
            => value == null
                ? WriteAsync(stream, (byte)NumberTypes.Null, cancellationToken)
                : WriteNumberAsync(stream, value, cancellationToken);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteNumberNullableAsync(this Task<Stream> stream, object? value, CancellationToken cancellationToken = default)
            => FluentAsync(stream, (s) => WriteNumberNullableAsync(s, value, cancellationToken));
    }
}
