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
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
#pragma warning disable // Remove unused argument
        public static Stream WriteNumber<T>(this Stream stream, T value, ISerializationContext context) where T : struct, IConvertible
#pragma warning restore IDE0060 // Remove unused argument
            => WriteNumberInt(context, value, type: null);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static Stream WriteNumber(this Stream stream, object value, ISerializationContext context) => WriteNumberInt(context, value, type: null);
#pragma warning restore IDE0060 // Remove unused argument

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="value">Value to write</param>
        /// <param name="type">Number type</param>
        /// <returns>Stream</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream WriteNumberInt(ISerializationContext context, object value, NumberTypes? type, bool writeType = true)
            => SerializerException.Wrap(() =>
            {
                if (type == null) (_, type) = value.GetNumberAndType();
                using RentedArrayStruct<byte> poolData = new(1, clean: false);
                if (writeType)
                {
                    poolData[0] = (byte)type;
                    context.Stream.Write(poolData.Span);
                }
                if (!type.Value.IsZero() && !type.Value.HasValueFlags())
                    switch (type)
                    {
                        case NumberTypes.Byte:
                        case NumberTypes.Byte | NumberTypes.Unsigned:
                            poolData[0] = value.ConvertType<byte>();
                            context.Stream.Write(poolData.Span);// Compatibility with serializer version 2
                            break;
                        case NumberTypes.Short:
                            Write(context.Stream, value.ConvertType<short>(), context);// Compatibility with serializer version 2
                            break;
                        case NumberTypes.Short | NumberTypes.Unsigned:
                            Write(context.Stream, value.ConvertType<ushort>(), context);// Compatibility with serializer version 2
                            break;
                        case NumberTypes.Int:
                            Write(context.Stream, value.ConvertType<int>(), context);
                            break;
                        case NumberTypes.Int | NumberTypes.Unsigned:
                            Write(context.Stream, value.ConvertType<uint>(), context);
                            break;
                        case NumberTypes.Long:
                            Write(context.Stream, value.ConvertType<long>(), context);
                            break;
                        case NumberTypes.Long | NumberTypes.Unsigned:
                            Write(context.Stream, value.ConvertType<ulong>(), context);
                            break;
                        case NumberTypes.Float:
                            Write(context.Stream, value.ConvertType<float>(), context);
                            break;
                        case NumberTypes.Double:
                            Write(context.Stream, value.ConvertType<double>(), context);
                            break;
                        case NumberTypes.Decimal:
                            Write(context.Stream, value.ConvertType<decimal>(), context);
                            break;
                    }
                return context.Stream;
            });

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Number type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        public static Task<Stream> WriteNumberAsync<T>(this Stream stream, T value, ISerializationContext context) where T : struct, IConvertible
            => WriteNumberIntAsync(context, value, type: null);

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Number type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteNumberAsync<T>(this Task<Stream> stream, T value, ISerializationContext context) where T : struct, IConvertible
            => AsyncHelper.FluentAsync(stream, value, context, WriteNumberAsync);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        public static Task<Stream> WriteNumberAsync(this Stream stream, object value, ISerializationContext context)
            => WriteNumberIntAsync(context, value, type: null);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteNumberAsync(this Task<Stream> stream, object value, ISerializationContext context)
            => AsyncHelper.FluentAsync(stream, value, context, WriteNumberAsync);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="value">Value to write</param>
        /// <param name="type">Number type</param>
        /// <returns>Stream</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<Stream> WriteNumberIntAsync(ISerializationContext context, object value, NumberTypes? type, bool writeType = true)
            => SerializerException.WrapAsync(async () =>
            {
                if (type == null) (_, type) = value.GetNumberAndType();
                using RentedArrayStruct<byte> poolData = new(1, clean: false);
                if (writeType)
                {
                    poolData[0] = (byte)type;
                    await context.Stream.WriteAsync(poolData.Memory, context.Cancellation).DynamicContext();
                }
                if (!type.Value.IsZero() && !type.Value.HasValueFlags())
                    switch (type)
                    {
                        case NumberTypes.Byte:
                        case NumberTypes.Byte | NumberTypes.Unsigned:
                            poolData[0] = value.ConvertType<byte>();
                            await context.Stream.WriteAsync(poolData.Memory, context.Cancellation).DynamicContext();// Compatibility with serializer version 2
                            break;
                        case NumberTypes.Short:
                            await WriteAsync(context.Stream, value.ConvertType<short>(), context).DynamicContext();// Compatibility with serializer version 2
                            break;
                        case NumberTypes.Short | NumberTypes.Unsigned:
                            await WriteAsync(context.Stream, value.ConvertType<ushort>(), context).DynamicContext();// Compatibility with serializer version 2
                            break;
                        case NumberTypes.Int:
                            await WriteAsync(context.Stream, value.ConvertType<int>(), context).DynamicContext();
                            break;
                        case NumberTypes.Int | NumberTypes.Unsigned:
                            await WriteAsync(context.Stream, value.ConvertType<uint>(), context).DynamicContext();
                            break;
                        case NumberTypes.Long:
                            await WriteAsync(context.Stream, value.ConvertType<long>(), context).DynamicContext();
                            break;
                        case NumberTypes.Long | NumberTypes.Unsigned:
                            await WriteAsync(context.Stream, value.ConvertType<ulong>(), context).DynamicContext();
                            break;
                        case NumberTypes.Float:
                            await WriteAsync(context.Stream, value.ConvertType<float>(), context).DynamicContext();
                            break;
                        case NumberTypes.Double:
                            await WriteAsync(context.Stream, value.ConvertType<double>(), context).DynamicContext();
                            break;
                        case NumberTypes.Decimal:
                            await WriteAsync(context.Stream, value.ConvertType<decimal>(), context).DynamicContext();
                            break;
                    }
                return context.Stream;
            });

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Number type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream WriteNumberNullable<T>(this Stream stream, T? value, ISerializationContext context) where T : struct, IConvertible
            => value == null ? Write(stream, (byte)NumberTypes.IsNull, context) : WriteNumber(stream, value.Value, context);

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
        public static Stream WriteNumberNullable(this Stream stream, object? value, ISerializationContext context)
            => value == null ? Write(stream, (byte)NumberTypes.IsNull, context) : WriteNumber(stream, value, context);

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Number type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<Stream> WriteNumberNullableAsync<T>(this Stream stream, T? value, ISerializationContext context)
            where T : struct, IConvertible
            => value == null
                ? WriteAsync(stream, (byte)NumberTypes.IsNull, context)
                : WriteNumberAsync(stream, value.Value, context);

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Number type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteNumberNullableAsync<T>(this Task<Stream> stream, T? value, ISerializationContext context)
            where T : struct, IConvertible
            => AsyncHelper.FluentAsync(stream, value, context, WriteNumberNullableAsync);

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
        public static Task<Stream> WriteNumberNullableAsync(this Stream stream, object? value, ISerializationContext context)
            => value == null
                ? WriteAsync(stream, (byte)NumberTypes.IsNull, context)
                : WriteNumberAsync(stream, value, context);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteNumberNullableAsync(this Task<Stream> stream, object? value, ISerializationContext context)
            => AsyncHelper.FluentAsync(stream, value, context, WriteNumberNullableAsync);
    }
}
