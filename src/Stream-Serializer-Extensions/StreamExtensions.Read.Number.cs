using System.Runtime;
using System.Runtime.CompilerServices;
using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    // Number
    public static partial class StreamExtensions
    {
        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static T ReadNumber<T>(this Stream stream, IDeserializationContext context) where T : struct, IConvertible
#pragma warning restore // Remove unused argument
            => (T)ReadNumberInt(context, typeof(T), numberType: null);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Number type</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static object ReadNumber(this Stream stream, Type type, IDeserializationContext context)
#pragma warning restore // Remove unused argument
            => ReadNumberInt(context, type, numberType: null);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="resType">Resulting number type</param>
        /// <param name="numberType">Number type</param>
        /// <returns>Value</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [SkipLocalsInit]
        public static object ReadNumberInt(IDeserializationContext context, Type resType, NumberTypes? numberType)
            => SerializerException.Wrap(() =>
            {
                ArgumentValidationHelper.EnsureValidArgument(nameof(resType), resType.IsValueType && typeof(IConvertible).IsAssignableFrom(resType), () => "Not a valid number type");
                Span<byte> data = stackalloc byte[1];
                if (numberType == null && context.Stream.Read(data) != 1) throw new SerializerException("Failed to read one byte", new IOException());
                NumberTypes type = numberType ?? (NumberTypes)data[0];
                if (type.IsZero()) return Activator.CreateInstance(resType)!;
                switch (type.RemoveValueFlags())
                {
                    case NumberTypes.Byte:
                        switch (type)
                        {
                            case NumberTypes.Byte | NumberTypes.MinValue:
                                return Convert.ChangeType(sbyte.MinValue, resType);
                            case NumberTypes.Byte | NumberTypes.MaxValue:
                                return Convert.ChangeType(sbyte.MaxValue, resType);
                        }
                        if (context.Stream.Read(data) != 1)// Compatibility with serializer version 2
                            throw new SerializerException("Failed to read serialized data (1 bytes)");
                        return Convert.ChangeType(data[0], resType);
                    case NumberTypes.Byte | NumberTypes.Unsigned:
                        switch (type)
                        {
                            case NumberTypes.Byte | NumberTypes.MaxValue | NumberTypes.Unsigned:
                                return Convert.ChangeType(byte.MaxValue, resType);
                        }
                        if (context.Stream.Read(data) != 1)// Compatibility with serializer version 2
                            throw new SerializerException("Failed to read serialized data (1 bytes)");
                        return Convert.ChangeType(data[0], resType);
                    case NumberTypes.Short:
                        return type switch
                        {
                            NumberTypes.Short | NumberTypes.MinValue => Convert.ChangeType(short.MinValue, resType),
                            NumberTypes.Short | NumberTypes.MaxValue => Convert.ChangeType(short.MaxValue, resType),
                            _ => Convert.ChangeType(ReadShort(context.Stream, context), resType)// Compatibility with serializer version 2
                        };
                    case NumberTypes.Short | NumberTypes.Unsigned:
                        return type switch
                        {
                            NumberTypes.Short | NumberTypes.MaxValue | NumberTypes.Unsigned => Convert.ChangeType(ushort.MaxValue, resType),
                            _ => Convert.ChangeType(ReadUShort(context.Stream, context), resType)// Compatibility with serializer version 2
                        };
                    case NumberTypes.Int:
                        return type switch
                        {
                            NumberTypes.Int | NumberTypes.MinValue => Convert.ChangeType(int.MinValue, resType),
                            NumberTypes.Int | NumberTypes.MaxValue => Convert.ChangeType(int.MaxValue, resType),
                            _ => Convert.ChangeType(ReadInt(context.Stream, context), resType)
                        };
                    case NumberTypes.Int | NumberTypes.Unsigned:
                        return type switch
                        {
                            NumberTypes.Int | NumberTypes.MaxValue | NumberTypes.Unsigned => Convert.ChangeType(uint.MaxValue, resType),
                            _ => Convert.ChangeType(ReadUInt(context.Stream, context), resType)
                        };
                    case NumberTypes.Long:
                        return type switch
                        {
                            NumberTypes.Long | NumberTypes.MinValue => Convert.ChangeType(long.MinValue, resType),
                            NumberTypes.Long | NumberTypes.MaxValue => Convert.ChangeType(long.MaxValue, resType),
                            _ => Convert.ChangeType(ReadLong(context.Stream, context), resType)
                        };
                    case NumberTypes.Long | NumberTypes.Unsigned:
                        return type switch
                        {
                            NumberTypes.Long | NumberTypes.MaxValue | NumberTypes.Unsigned => Convert.ChangeType(ulong.MaxValue, resType),
                            _ => Convert.ChangeType(ReadULong(context.Stream, context), resType)
                        };
                    case NumberTypes.Float:
                        return type switch
                        {
                            NumberTypes.Float | NumberTypes.MinValue => Convert.ChangeType(float.MinValue, resType),
                            NumberTypes.Float | NumberTypes.MaxValue => Convert.ChangeType(float.MaxValue, resType),
                            _ => Convert.ChangeType(ReadFloat(context.Stream, context), resType)
                        };
                    case NumberTypes.Double:
                        return type switch
                        {
                            NumberTypes.Double | NumberTypes.MinValue => Convert.ChangeType(double.MinValue, resType),
                            NumberTypes.Double | NumberTypes.MaxValue => Convert.ChangeType(double.MaxValue, resType),
                            _ => Convert.ChangeType(ReadDouble(context.Stream, context), resType)
                        };
                    case NumberTypes.Decimal:
                        return type switch
                        {
                            NumberTypes.Decimal | NumberTypes.MinValue => Convert.ChangeType(decimal.MinValue, resType),
                            NumberTypes.Decimal | NumberTypes.MaxValue => Convert.ChangeType(decimal.MaxValue, resType),
                            _ => Convert.ChangeType(ReadDecimal(context.Stream, context), resType)
                        };
                    default:
                        throw new SerializerException($"Unknown numeric type {type}");
                }
            });

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static async Task<T> ReadNumberAsync<T>(this Stream stream, IDeserializationContext context)
#pragma warning restore // Remove unused argument
            where T : struct, IConvertible
            => (T)await ReadNumberIntAsync(context, typeof(T), numberType: null).DynamicContext();

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Number type</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static Task<object> ReadNumberAsync(this Stream stream, Type type, IDeserializationContext context)
#pragma warning restore // Remove unused argument
            => ReadNumberIntAsync(context, type, numberType: null);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="resType">Resulting number type</param>
        /// <param name="numberType">Number type</param>
        /// <returns>Value</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<object> ReadNumberIntAsync(IDeserializationContext context, Type resType, NumberTypes? numberType)
            => SerializerException.WrapAsync(async () =>
            {
                ArgumentValidationHelper.EnsureValidArgument(nameof(resType), resType.IsValueType && typeof(IConvertible).IsAssignableFrom(resType), () => "Not a valid number type");
                byte[] data = numberType == null
                    ? await ReadSerializedDataAsync(context.Stream, len: 1, context).DynamicContext()
                    : context.BufferPool.Rent(minimumLength: 1);
                try
                {
                    NumberTypes type = numberType ?? (NumberTypes)data[0];
                    if (type.IsZero()) return Activator.CreateInstance(resType)!;
                    switch (type.RemoveValueFlags())
                    {
                        case NumberTypes.Byte:
                            switch (type)
                            {
                                case NumberTypes.Byte | NumberTypes.MinValue:
                                    return Convert.ChangeType(sbyte.MinValue, resType);
                                case NumberTypes.Byte | NumberTypes.MaxValue:
                                    return Convert.ChangeType(sbyte.MaxValue, resType);
                            }
                            if (await context.Stream.ReadAsync(data.AsMemory(0, 1), context.Cancellation).DynamicContext() != 1)// Compatibility with serializer version 2
                                throw new SerializerException("Failed to read serialized data (1 bytes)");
                            return Convert.ChangeType(data[0], resType);
                        case NumberTypes.Byte | NumberTypes.Unsigned:
                            switch (type)
                            {
                                case NumberTypes.Byte | NumberTypes.MaxValue | NumberTypes.Unsigned:
                                    return Convert.ChangeType(byte.MaxValue, resType);
                            }
                            if (await context.Stream.ReadAsync(data.AsMemory(0, 1), context.Cancellation).DynamicContext() != 1)// Compatibility with serializer version 2
                                throw new SerializerException("Failed to read serialized data (1 bytes)");
                            return Convert.ChangeType(data[0], resType);
                        case NumberTypes.Short:
                            return type switch
                            {
                                NumberTypes.Short | NumberTypes.MinValue => Convert.ChangeType(short.MinValue, resType),
                                NumberTypes.Short | NumberTypes.MaxValue => Convert.ChangeType(short.MaxValue, resType),
                                _ => Convert.ChangeType(await ReadShortAsync(context.Stream, context).DynamicContext(), resType)// Compatibility with serializer version 2
                            };
                        case NumberTypes.Short | NumberTypes.Unsigned:
                            return type switch
                            {
                                NumberTypes.Short | NumberTypes.MaxValue | NumberTypes.Unsigned => Convert.ChangeType(ushort.MaxValue, resType),
                                _ => Convert.ChangeType(await ReadUShortAsync(context.Stream, context).DynamicContext(), resType)// Compatibility with serializer version 2
                            };
                        case NumberTypes.Int:
                            return type switch
                            {
                                NumberTypes.Int | NumberTypes.MinValue => Convert.ChangeType(int.MinValue, resType),
                                NumberTypes.Int | NumberTypes.MaxValue => Convert.ChangeType(int.MaxValue, resType),
                                _ => Convert.ChangeType(await ReadIntAsync(context.Stream, context).DynamicContext(), resType)
                            };
                        case NumberTypes.Int | NumberTypes.Unsigned:
                            return type switch
                            {
                                NumberTypes.Int | NumberTypes.MaxValue | NumberTypes.Unsigned => Convert.ChangeType(uint.MaxValue, resType),
                                _ => Convert.ChangeType(await ReadUIntAsync(context.Stream, context).DynamicContext(), resType)
                            };
                        case NumberTypes.Long:
                            return type switch
                            {
                                NumberTypes.Long | NumberTypes.MinValue => Convert.ChangeType(long.MinValue, resType),
                                NumberTypes.Long | NumberTypes.MaxValue => Convert.ChangeType(long.MaxValue, resType),
                                _ => Convert.ChangeType(await ReadLongAsync(context.Stream, context).DynamicContext(), resType)
                            };
                        case NumberTypes.Long | NumberTypes.Unsigned:
                            return type switch
                            {
                                NumberTypes.Long | NumberTypes.MaxValue | NumberTypes.Unsigned => Convert.ChangeType(ulong.MaxValue, resType),
                                _ => Convert.ChangeType(await ReadULongAsync(context.Stream, context).DynamicContext(), resType)
                            };
                        case NumberTypes.Float:
                            return type switch
                            {
                                NumberTypes.Float | NumberTypes.MinValue => Convert.ChangeType(float.MinValue, resType),
                                NumberTypes.Float | NumberTypes.MaxValue => Convert.ChangeType(float.MaxValue, resType),
                                _ => Convert.ChangeType(await ReadFloatAsync(context.Stream, context).DynamicContext(), resType)
                            };
                        case NumberTypes.Double:
                            return type switch
                            {
                                NumberTypes.Double | NumberTypes.MinValue => Convert.ChangeType(double.MinValue, resType),
                                NumberTypes.Double | NumberTypes.MaxValue => Convert.ChangeType(double.MaxValue, resType),
                                _ => Convert.ChangeType(await ReadDoubleAsync(context.Stream, context).DynamicContext(), resType)
                            };
                        case NumberTypes.Decimal:
                            return type switch
                            {
                                NumberTypes.Decimal | NumberTypes.MinValue => Convert.ChangeType(decimal.MinValue, resType),
                                NumberTypes.Decimal | NumberTypes.MaxValue => Convert.ChangeType(decimal.MaxValue, resType),
                                _ => Convert.ChangeType(await ReadDecimalAsync(context.Stream, context).DynamicContext(), resType)
                            };
                        default:
                            throw new SerializerException($"Unknown numeric type {type}");
                    }
                }
                finally
                {
                    context.BufferPool.Return(data);
                }
            });

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T? ReadNumberNullable<T>(this Stream stream, IDeserializationContext context) where T : struct, IConvertible
            => SerializerException.Wrap(() =>
            {
                switch (context.SerializerVersion)// Serializer version switch
                {
                    case 1:
                        {
                            return ReadBool(stream, context) ? ReadNumber<T>(stream, context) : null;
                        }
                    case 2:
                        {
                            NumberTypes numberType = ReadEnum<NumberTypes>(stream, context);
                            return numberType == NumberTypes.IsNull ? null : (T?)ReadNumberInt(context, typeof(T), numberType);
                        }
                    default:
                        {
                            NumberTypes numberType = (NumberTypes)ReadOneByte(stream, context);
                            return numberType == NumberTypes.IsNull ? null : (T?)ReadNumberInt(context, typeof(T), numberType);
                        }
                }
            });

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Number type</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object? ReadNumberNullable(this Stream stream, Type type, IDeserializationContext context)
            => SerializerException.Wrap(() =>
            {
                switch (context.SerializerVersion)// Serializer version switch
                {
                    case 1:
                        {
                            return ReadBool(stream, context) ? ReadNumber(stream, type, context) : null;
                        }
                    case 2:
                        {
                            NumberTypes numberType = ReadEnum<NumberTypes>(stream, context);
                            return numberType == NumberTypes.IsNull ? null : ReadNumberInt(context, type, numberType);
                        }
                    default:
                        {
                            NumberTypes numberType = (NumberTypes)ReadOneByte(stream, context);
                            return numberType == NumberTypes.IsNull ? null : ReadNumberInt(context, type, numberType);
                        }
                }
            });

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Number type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<T?> ReadNumberNullableAsync<T>(this Stream stream, IDeserializationContext context)
            where T : struct, IConvertible
            => SerializerException.WrapAsync(async () =>
            {
                switch (context.SerializerVersion)// Serializer version switch
                {
                    case 1:
                        {
                            return await ReadBoolAsync(stream, context).DynamicContext()
                                ? await ReadNumberAsync<T>(stream, context).DynamicContext()
                                : null;
                        }
                    case 2:
                        {
                            NumberTypes numberType = await ReadEnumAsync<NumberTypes>(stream, context).DynamicContext();
                            return numberType == NumberTypes.IsNull ? null : (T?)await ReadNumberIntAsync(context, typeof(T), numberType).DynamicContext();
                        }
                    default:
                        {
                            NumberTypes numberType = (NumberTypes)await ReadOneByteAsync(stream, context).DynamicContext();
                            return numberType == NumberTypes.IsNull ? null : (T?)await ReadNumberIntAsync(context, typeof(T), numberType).DynamicContext();
                        }
                }
            });

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Number type</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<object?> ReadNumberNullableAsync(this Stream stream, Type type, IDeserializationContext context)
            => SerializerException.WrapAsync(async () =>
            {
                switch (context.SerializerVersion)// Serializer version switch
                {
                    case 1:
                        {
                            return await ReadBoolAsync(stream, context).DynamicContext()
                                ? await ReadNumberAsync(stream, type, context).DynamicContext()
                                : null;
                        }
                    case 2:
                        {
                            NumberTypes numberType = await ReadEnumAsync<NumberTypes>(stream, context).DynamicContext();
                            return numberType == NumberTypes.IsNull ? null : await ReadNumberIntAsync(context, type, numberType).DynamicContext();
                        }
                    default:
                        {
                            NumberTypes numberType = (NumberTypes)await ReadOneByteAsync(stream, context).DynamicContext();
                            return numberType == NumberTypes.IsNull ? null : await ReadNumberIntAsync(context, type, numberType).DynamicContext();
                        }
                }
            });
    }
}
