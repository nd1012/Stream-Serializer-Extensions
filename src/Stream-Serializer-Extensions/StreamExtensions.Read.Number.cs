using System.Buffers;
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
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        public static T ReadNumber<T>(this Stream stream, int? version = null, ArrayPool<byte>? pool = null) where T : struct, IConvertible
            => (T)ReadNumberInt(stream, typeof(T), version, numberType: null, pool);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Number type</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        public static object ReadNumber(this Stream stream, Type type, int? version = null, ArrayPool<byte>? pool = null)
            => ReadNumberInt(stream, type, version, numberType: null, pool);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="resType">Resulting number type</param>
        /// <param name="version">Serializer version</param>
        /// <param name="numberType">Number type</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static object ReadNumberInt(Stream stream, Type resType, int? version, NumberTypes? numberType, ArrayPool<byte>? pool)
            => SerializerException.Wrap(() =>
            {
                ArgumentValidationHelper.EnsureValidArgument(nameof(resType), resType.IsValueType && typeof(IConvertible).IsAssignableFrom(resType), () => "Not a valid number type");
                byte[] data = numberType == null ? ReadSerializedData(stream, len: 1, pool) : (pool ?? StreamSerializer.BufferPool).Rent(minimumLength: 1);
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
                            if (stream.Read(data.AsSpan(0, 1)) != 1) throw new SerializerException("Failed to read serialized data (1 bytes)");
                            return Convert.ChangeType(data[0], resType);
                        case NumberTypes.Byte | NumberTypes.Unsigned:
                            switch (type)
                            {
                                case NumberTypes.Byte | NumberTypes.MaxValue | NumberTypes.Unsigned:
                                    return Convert.ChangeType(byte.MaxValue, resType);
                            }
                            if (stream.Read(data.AsSpan(0, 1)) != 1) throw new SerializerException("Failed to read serialized data (1 bytes)");
                            return Convert.ChangeType(data[0], resType);
                        case NumberTypes.Short:
                            return type switch
                            {
                                NumberTypes.Short | NumberTypes.MinValue => Convert.ChangeType(short.MinValue, resType),
                                NumberTypes.Short | NumberTypes.MaxValue => Convert.ChangeType(short.MaxValue, resType),
                                _ => Convert.ChangeType(ReadShort(stream, version, pool), resType)
                            };
                        case NumberTypes.Short | NumberTypes.Unsigned:
                            return type switch
                            {
                                NumberTypes.Short | NumberTypes.MaxValue | NumberTypes.Unsigned => Convert.ChangeType(ushort.MaxValue, resType),
                                _ => Convert.ChangeType(ReadUShort(stream, version, pool), resType)
                            };
                        case NumberTypes.Int:
                            return type switch
                            {
                                NumberTypes.Int | NumberTypes.MinValue => Convert.ChangeType(int.MinValue, resType),
                                NumberTypes.Int | NumberTypes.MaxValue => Convert.ChangeType(int.MaxValue, resType),
                                _ => Convert.ChangeType(ReadInt(stream, version, pool), resType)
                            };
                        case NumberTypes.Int | NumberTypes.Unsigned:
                            return type switch
                            {
                                NumberTypes.Int | NumberTypes.MaxValue | NumberTypes.Unsigned => Convert.ChangeType(uint.MaxValue, resType),
                                _ => Convert.ChangeType(ReadUInt(stream, version, pool), resType)
                            };
                        case NumberTypes.Long:
                            return type switch
                            {
                                NumberTypes.Long | NumberTypes.MinValue => Convert.ChangeType(long.MinValue, resType),
                                NumberTypes.Long | NumberTypes.MaxValue => Convert.ChangeType(long.MaxValue, resType),
                                _ => Convert.ChangeType(ReadLong(stream, version, pool), resType)
                            };
                        case NumberTypes.Long | NumberTypes.Unsigned:
                            return type switch
                            {
                                NumberTypes.Long | NumberTypes.MaxValue | NumberTypes.Unsigned => Convert.ChangeType(ulong.MaxValue, resType),
                                _ => Convert.ChangeType(ReadULong(stream, version, pool), resType)
                            };
                        case NumberTypes.Float:
                            return type switch
                            {
                                NumberTypes.Float | NumberTypes.MinValue => Convert.ChangeType(float.MinValue, resType),
                                NumberTypes.Float | NumberTypes.MaxValue => Convert.ChangeType(float.MaxValue, resType),
                                _ => Convert.ChangeType(ReadFloat(stream, version, pool), resType)
                            };
                        case NumberTypes.Double:
                            return type switch
                            {
                                NumberTypes.Double | NumberTypes.MinValue => Convert.ChangeType(double.MinValue, resType),
                                NumberTypes.Double | NumberTypes.MaxValue => Convert.ChangeType(double.MaxValue, resType),
                                _ => Convert.ChangeType(ReadDouble(stream, version, pool), resType)
                            };
                        case NumberTypes.Decimal:
                            return type switch
                            {
                                NumberTypes.Decimal | NumberTypes.MinValue => Convert.ChangeType(decimal.MinValue, resType),
                                NumberTypes.Decimal | NumberTypes.MaxValue => Convert.ChangeType(decimal.MaxValue, resType),
                                _ => Convert.ChangeType(ReadDecimal(stream, version, pool), resType)
                            };
                        default:
                            throw new SerializerException($"Unknown numeric type {type}");
                    }
                }
                finally
                {
                    (pool ?? StreamSerializer.BufferPool).Return(data);
                }
            });

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static async Task<T> ReadNumberAsync<T>(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
            where T : struct, IConvertible
            => (T)await ReadNumberIntAsync(stream, typeof(T), version, numberType: null, pool, cancellationToken).DynamicContext();

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Number type</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static Task<object> ReadNumberAsync(this Stream stream, Type type, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
            => ReadNumberIntAsync(stream, type, version, numberType: null, pool, cancellationToken);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="resType">Resulting number type</param>
        /// <param name="version">Serializer version</param>
        /// <param name="numberType">Number type</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static Task<object> ReadNumberIntAsync(
            Stream stream,
            Type resType,
            int? version,
            NumberTypes? numberType,
            ArrayPool<byte>? pool,
            CancellationToken cancellationToken
            )
            => SerializerException.WrapAsync(async () =>
            {
                ArgumentValidationHelper.EnsureValidArgument(nameof(resType), resType.IsValueType && typeof(IConvertible).IsAssignableFrom(resType), () => "Not a valid number type");
                byte[] data = numberType == null
                    ? await ReadSerializedDataAsync(stream, len: 1, pool, cancellationToken).DynamicContext()
                    : (pool ?? StreamSerializer.BufferPool).Rent(minimumLength: 1);
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
                            if (await stream.ReadAsync(data.AsMemory(0, 1), cancellationToken).DynamicContext() != 1)
                                throw new SerializerException("Failed to read serialized data (1 bytes)");
                            return Convert.ChangeType(data[0], resType);
                        case NumberTypes.Byte | NumberTypes.Unsigned:
                            switch (type)
                            {
                                case NumberTypes.Byte | NumberTypes.MaxValue | NumberTypes.Unsigned:
                                    return Convert.ChangeType(byte.MaxValue, resType);
                            }
                            if (await stream.ReadAsync(data.AsMemory(0, 1), cancellationToken).DynamicContext() != 1)
                                throw new SerializerException("Failed to read serialized data (1 bytes)");
                            return Convert.ChangeType(data[0], resType);
                        case NumberTypes.Short:
                            return type switch
                            {
                                NumberTypes.Short | NumberTypes.MinValue => Convert.ChangeType(short.MinValue, resType),
                                NumberTypes.Short | NumberTypes.MaxValue => Convert.ChangeType(short.MaxValue, resType),
                                _ => Convert.ChangeType(await ReadShortAsync(stream, version, pool, cancellationToken).DynamicContext(), resType)
                            };
                        case NumberTypes.Short | NumberTypes.Unsigned:
                            return type switch
                            {
                                NumberTypes.Short | NumberTypes.MaxValue | NumberTypes.Unsigned => Convert.ChangeType(ushort.MaxValue, resType),
                                _ => Convert.ChangeType(await ReadUShortAsync(stream, version, pool, cancellationToken).DynamicContext(), resType)
                            };
                        case NumberTypes.Int:
                            return type switch
                            {
                                NumberTypes.Int | NumberTypes.MinValue => Convert.ChangeType(int.MinValue, resType),
                                NumberTypes.Int | NumberTypes.MaxValue => Convert.ChangeType(int.MaxValue, resType),
                                _ => Convert.ChangeType(await ReadIntAsync(stream, version, pool, cancellationToken).DynamicContext(), resType)
                            };
                        case NumberTypes.Int | NumberTypes.Unsigned:
                            return type switch
                            {
                                NumberTypes.Int | NumberTypes.MaxValue | NumberTypes.Unsigned => Convert.ChangeType(uint.MaxValue, resType),
                                _ => Convert.ChangeType(await ReadUIntAsync(stream, version, pool, cancellationToken).DynamicContext(), resType)
                            };
                        case NumberTypes.Long:
                            return type switch
                            {
                                NumberTypes.Long | NumberTypes.MinValue => Convert.ChangeType(long.MinValue, resType),
                                NumberTypes.Long | NumberTypes.MaxValue => Convert.ChangeType(long.MaxValue, resType),
                                _ => Convert.ChangeType(await ReadLongAsync(stream, version, pool, cancellationToken).DynamicContext(), resType)
                            };
                        case NumberTypes.Long | NumberTypes.Unsigned:
                            return type switch
                            {
                                NumberTypes.Long | NumberTypes.MaxValue | NumberTypes.Unsigned => Convert.ChangeType(ulong.MaxValue, resType),
                                _ => Convert.ChangeType(await ReadULongAsync(stream, version, pool, cancellationToken).DynamicContext(), resType)
                            };
                        case NumberTypes.Float:
                            return type switch
                            {
                                NumberTypes.Float | NumberTypes.MinValue => Convert.ChangeType(float.MinValue, resType),
                                NumberTypes.Float | NumberTypes.MaxValue => Convert.ChangeType(float.MaxValue, resType),
                                _ => Convert.ChangeType(await ReadFloatAsync(stream, version, pool, cancellationToken).DynamicContext(), resType)
                            };
                        case NumberTypes.Double:
                            return type switch
                            {
                                NumberTypes.Double | NumberTypes.MinValue => Convert.ChangeType(double.MinValue, resType),
                                NumberTypes.Double | NumberTypes.MaxValue => Convert.ChangeType(double.MaxValue, resType),
                                _ => Convert.ChangeType(await ReadDoubleAsync(stream, version, pool, cancellationToken).DynamicContext(), resType)
                            };
                        case NumberTypes.Decimal:
                            return type switch
                            {
                                NumberTypes.Decimal | NumberTypes.MinValue => Convert.ChangeType(decimal.MinValue, resType),
                                NumberTypes.Decimal | NumberTypes.MaxValue => Convert.ChangeType(decimal.MaxValue, resType),
                                _ => Convert.ChangeType(await ReadDecimalAsync(stream, version, pool, cancellationToken).DynamicContext(), resType)
                            };
                        default:
                            throw new SerializerException($"Unknown numeric type {type}");
                    }
                }
                finally
                {
                    (pool ?? StreamSerializer.BufferPool).Return(data);
                }
            });

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T? ReadNumberNullable<T>(this Stream stream, int? version = null, ArrayPool<byte>? pool = null) where T : struct, IConvertible
            => SerializerException.Wrap(() =>
            {
                switch ((version ?? StreamSerializer.Version) & byte.MaxValue)
                {
                    case 1:
                        {
                            return ReadBool(stream, version, pool) ? ReadNumber<T>(stream, version, pool) : null;
                        }
                    case 2:
                        {
                            NumberTypes numberType = ReadEnum<NumberTypes>(stream, version, pool);
                            return numberType == NumberTypes.Null ? null : (T?)ReadNumberInt(stream, typeof(T), version, numberType, pool);
                        }
                    default:
                        {
                            NumberTypes numberType = (NumberTypes)ReadOneByte(stream, version);
                            return numberType == NumberTypes.Null ? null : (T?)ReadNumberInt(stream, typeof(T), version, numberType, pool);
                        }
                }
            });

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Number type</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object? ReadNumberNullable(this Stream stream, Type type, int? version = null, ArrayPool<byte>? pool = null)
            => SerializerException.Wrap(() =>
            {
                switch ((version ?? StreamSerializer.Version) & byte.MaxValue)
                {
                    case 1:
                        {
                            return ReadBool(stream, version, pool) ? ReadNumber(stream, type, version, pool) : null;
                        }
                    case 2:
                        {
                            NumberTypes numberType = ReadEnum<NumberTypes>(stream, version, pool);
                            return numberType == NumberTypes.Null ? null : ReadNumberInt(stream, type, version, numberType, pool);
                        }
                    default:
                        {
                            NumberTypes numberType = (NumberTypes)ReadOneByte(stream, version);
                            return numberType == NumberTypes.Null ? null : ReadNumberInt(stream, type, version, numberType, pool);
                        }
                }
            });

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Number type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<T?> ReadNumberNullableAsync<T>(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
            where T : struct, IConvertible
            => SerializerException.WrapAsync(async () =>
            {
                switch ((version ?? StreamSerializer.Version) & byte.MaxValue)
                {
                    case 1:
                        {
                            return await ReadBoolAsync(stream, version, pool, cancellationToken).DynamicContext()
                                ? await ReadNumberAsync<T>(stream, version, pool, cancellationToken).DynamicContext()
                                : null;
                        }
                    case 2:
                        {
                            NumberTypes numberType = await ReadEnumAsync<NumberTypes>(stream, version, pool, cancellationToken).DynamicContext();
                            return numberType == NumberTypes.Null ? null : (T?)await ReadNumberIntAsync(stream, typeof(T), version, numberType, pool, cancellationToken)
                                .DynamicContext();
                        }
                    default:
                        {
                            NumberTypes numberType = (NumberTypes)await ReadOneByteAsync(stream, version, cancellationToken).DynamicContext();
                            return numberType == NumberTypes.Null ? null : (T?)await ReadNumberIntAsync(stream, typeof(T), version, numberType, pool, cancellationToken)
                                .DynamicContext();
                        }
                }
            });

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Number type</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<object?> ReadNumberNullableAsync(
            this Stream stream,
            Type type,
            int? version = null,
            ArrayPool<byte>? pool = null,
            CancellationToken cancellationToken = default
            )
            => SerializerException.WrapAsync(async () =>
            {
                switch ((version ?? StreamSerializer.Version) & byte.MaxValue)
                {
                    case 1:
                        {
                            return await ReadBoolAsync(stream, version, pool, cancellationToken).DynamicContext()
                                ? await ReadNumberAsync(stream, type, version, pool, cancellationToken).DynamicContext()
                                : null;
                        }
                    case 2:
                        {
                            NumberTypes numberType = await ReadEnumAsync<NumberTypes>(stream, version, pool, cancellationToken).DynamicContext();
                            return numberType == NumberTypes.Null ? null : await ReadNumberIntAsync(stream, type, version, numberType, pool, cancellationToken)
                                .DynamicContext();
                        }
                    default:
                        {
                            NumberTypes numberType = (NumberTypes)await ReadOneByteAsync(stream, version, cancellationToken).DynamicContext();
                            return numberType == NumberTypes.Null ? null : await ReadNumberIntAsync(stream, type, version, numberType, pool, cancellationToken).DynamicContext();
                        }
                }
            });
    }
}
