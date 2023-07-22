using System.Runtime;
using System.Runtime.CompilerServices;
using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    // Basic
    public static partial class StreamExtensions
    {
        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static bool ReadBool(this Stream stream, IDeserializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            => ReadNumeric(context, sizeof(bool), (data) => data[0] == 1);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static Task<bool> ReadBoolAsync(this Stream stream, IDeserializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            => ReadNumericAsync(context, sizeof(bool), (data) => data[0] == 1);

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
        public static bool? ReadBoolNullable(this Stream stream, IDeserializationContext context)
        {
            switch (context.SerializerVersion)// Serializer version switch
            {
                case 1:
                case 2:
                    {
                        return ReadBool(stream, context) ? ReadBool(stream, context) : null;
                    }
                default:
                    {
                        ObjectTypes type = (ObjectTypes)ReadOneByte(stream, context);
                        return type switch
                        {
                            ObjectTypes.Null => null,
                            ObjectTypes.True => true,
                            ObjectTypes.False => false,
                            _ => throw new SerializerException($"Invalid boolean {type}", new InvalidDataException())
                        };
                    }
            }
        }

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
        public static async Task<bool?> ReadBoolNullableAsync(this Stream stream, IDeserializationContext context)
        {
            switch (context.SerializerVersion)// Serializer version switch
            {
                case 1:
                case 2:
                    {
                        return await ReadBoolAsync(stream, context).DynamicContext()
                            ? await ReadBoolAsync(stream, context).DynamicContext()
                            : null;
                    }
                default:
                    {
                        ObjectTypes type = (ObjectTypes)await ReadOneByteAsync(stream, context).DynamicContext();
                        return type switch
                        {
                            ObjectTypes.Null => null,
                            ObjectTypes.True => true,
                            ObjectTypes.False => false,
                            _ => throw new SerializerException($"Invalid boolean {type}", new InvalidDataException())
                        };
                    }
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte ReadOneSByte(this Stream stream, IDeserializationContext context) => (sbyte)ReadOneByte(stream, context);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<sbyte> ReadOneSByteAsync(this Stream stream, IDeserializationContext context) => Task.FromResult(ReadOneSByte(stream, context));

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static sbyte? ReadOneSByteNullable(this Stream stream, IDeserializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            => ReadNullableNumeric(context, sizeof(sbyte), sbyte.MinValue, sbyte.MaxValue, (data) => (sbyte)data[0]);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static Task<sbyte?> ReadOneSByteNullableAsync(this Stream stream, IDeserializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            => ReadNullableNumericAsync(context, sizeof(sbyte), sbyte.MinValue, sbyte.MaxValue, (data) => (sbyte)data[0]);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused parameter
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte ReadOneByte(this Stream stream, IDeserializationContext context)
            => SerializerException.Wrap(() =>
            {
                int res = stream.ReadByte();
                if (res < 0) throw new IOException("Failed to read one byte from stream");
                return (byte)res;
            });

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<byte> ReadOneByteAsync(this Stream stream, IDeserializationContext context) => Task.FromResult(ReadOneByte(stream, context));

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static ushort? ReadOneByteNullable(this Stream stream, IDeserializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            => ReadNullableNumeric(context, sizeof(byte), byte.MinValue, byte.MaxValue, (data) => data[0]);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static Task<byte?> ReadOneByteNullableAsync(this Stream stream, IDeserializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            => ReadNullableNumericAsync(context, sizeof(byte), byte.MinValue, byte.MaxValue, (data) => data[0]);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static short ReadShort(this Stream stream, IDeserializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            => ReadNumeric(context, sizeof(short), (data) => data.ToShort());

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static Task<short> ReadShortAsync(this Stream stream, IDeserializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            => ReadNumericAsync(context, sizeof(short), (data) => data.ToShort());

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static short? ReadShortNullable(this Stream stream, IDeserializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            => ReadNullableNumeric(context, sizeof(short), short.MinValue, short.MaxValue, (data) => data.ToShort());

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static Task<short?> ReadShortNullableAsync(this Stream stream, IDeserializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            => ReadNullableNumericAsync(context, sizeof(short), short.MinValue, short.MaxValue, (data) => data.ToShort());

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static ushort ReadUShort(this Stream stream, IDeserializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            => ReadNumeric(context, sizeof(ushort), (data) => data.ToUShort());

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static Task<ushort> ReadUShortAsync(this Stream stream, IDeserializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            => ReadNumericAsync(context, sizeof(ushort), (data) => data.ToUShort());

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static ushort? ReadUShortNullable(this Stream stream, IDeserializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            => ReadNullableNumeric(context, sizeof(ushort), ushort.MinValue, ushort.MaxValue, (data) => data.ToUShort());

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static Task<ushort?> ReadUShortNullableAsync(this Stream stream, IDeserializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            => ReadNullableNumericAsync(context, sizeof(ushort), ushort.MinValue, ushort.MaxValue, (data) => data.ToUShort());

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static int ReadInt(this Stream stream, IDeserializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            => ReadNumeric(context, sizeof(int), (data) => data.ToInt());

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static Task<int> ReadIntAsync(this Stream stream, IDeserializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            => ReadNumericAsync(context, sizeof(int), (data) => data.ToInt());

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static int? ReadIntNullable(this Stream stream, IDeserializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            => ReadNullableNumeric(context, sizeof(int), int.MinValue, int.MaxValue, (data) => data.ToInt());

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static Task<int?> ReadIntNullableAsync(this Stream stream, IDeserializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            => ReadNullableNumericAsync(context, sizeof(int), int.MinValue, int.MaxValue, (data) => data.ToInt());

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static uint ReadUInt(this Stream stream, IDeserializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            => ReadNumeric(context, sizeof(uint), (data) => data.ToUInt());

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static Task<uint> ReadUIntAsync(this Stream stream, IDeserializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            => ReadNumericAsync(context, sizeof(uint), (data) => data.ToUInt());

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static uint? ReadUIntNullable(this Stream stream, IDeserializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            => ReadNullableNumeric(context, sizeof(uint), uint.MinValue, uint.MaxValue, (data) => data.ToUInt());

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static Task<uint?> ReadUIntNullableAsync(this Stream stream, IDeserializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            => ReadNullableNumericAsync(context, sizeof(uint), uint.MinValue, uint.MaxValue, (data) => data.ToUInt());

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static long ReadLong(this Stream stream, IDeserializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            => ReadNumeric(context, sizeof(long), (data) => data.ToLong());

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static Task<long> ReadLongAsync(this Stream stream, IDeserializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            => ReadNumericAsync(context, sizeof(long), (data) => data.ToLong());

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static long? ReadLongNullable(this Stream stream, IDeserializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            => ReadNullableNumeric(context, sizeof(long), long.MinValue, long.MaxValue, (data) => data.ToLong());

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static Task<long?> ReadLongNullableAsync(this Stream stream, IDeserializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            => ReadNullableNumericAsync(context, sizeof(long), long.MinValue, long.MaxValue, (data) => data.ToLong());

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static ulong ReadULong(this Stream stream, IDeserializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            => ReadNumeric(context, sizeof(ulong), (data) => data.ToULong());

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static Task<ulong> ReadULongAsync(this Stream stream, IDeserializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            => ReadNumericAsync(context, sizeof(ulong), (data) => data.ToULong());

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static ulong? ReadULongNullable(this Stream stream, IDeserializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            => ReadNullableNumeric(context, sizeof(ulong), ulong.MinValue, ulong.MaxValue, (data) => data.ToULong());

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static Task<ulong?> ReadULongNullableAsync(this Stream stream, IDeserializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            => ReadNullableNumericAsync(context, sizeof(ulong), ulong.MinValue, ulong.MaxValue, (data) => data.ToULong());

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static float ReadFloat(this Stream stream, IDeserializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            => ReadNumeric(context, sizeof(float), (data) => data.ToFloat());

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static Task<float> ReadFloatAsync(this Stream stream, IDeserializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            => ReadNumericAsync(context, sizeof(float), (data) => data.ToFloat());

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static float? ReadFloatNullable(this Stream stream, IDeserializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            => ReadNullableNumeric(context, sizeof(float), float.MinValue, float.MaxValue, (data) => data.ToFloat());

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static Task<float?> ReadFloatNullableAsync(this Stream stream, IDeserializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            => ReadNullableNumericAsync(context, sizeof(float), float.MinValue, float.MaxValue, (data) => data.ToFloat());

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static double ReadDouble(this Stream stream, IDeserializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            => ReadNumeric(context, sizeof(double), (data) => data.ToDouble());

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static Task<double> ReadDoubleAsync(this Stream stream, IDeserializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            => ReadNumericAsync(context, sizeof(double), (data) => data.ToDouble());

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static double? ReadDoubleNullable(this Stream stream, IDeserializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            => ReadNullableNumeric(context, sizeof(double), double.MinValue, double.MaxValue, (data) => data.ToDouble());

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static Task<double?> ReadDoubleNullableAsync(this Stream stream, IDeserializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            => ReadNullableNumericAsync(context, sizeof(double), double.MinValue, double.MaxValue, (data) => data.ToDouble());

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static decimal ReadDecimal(this Stream stream, IDeserializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            => ReadNumeric(context, sizeof(decimal), (data) => data.ToDecimal());

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static Task<decimal> ReadDecimalAsync(this Stream stream, IDeserializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            => ReadNumericAsync(context, sizeof(decimal), (data) => data.ToDecimal());

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static decimal? ReadDecimalNullable(this Stream stream, IDeserializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            => ReadNullableNumeric(context, sizeof(decimal), decimal.MinValue, decimal.MaxValue, (data) => data.ToDecimal());

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static Task<decimal?> ReadDecimalNullableAsync(this Stream stream, IDeserializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            => ReadNullableNumericAsync(context, sizeof(decimal), decimal.MinValue, decimal.MaxValue, (data) => data.ToDecimal());

        /// <summary>
        /// Read a numeric value
        /// </summary>
        /// <typeparam name="T">Numeric type</typeparam>
        /// <param name="context">Context</param>
        /// <param name="size">Serialized data size in bytes</param>
        /// <param name="deserializer">Deserializer action</param>
        /// <returns>Value</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [SkipLocalsInit]
        private static T ReadNumeric<T>(IDeserializationContext context, int size, NumericReader_Delegate<T> deserializer)
        {
            Span<byte> buffer = stackalloc byte[size];
            if (context.Stream.Read(buffer) != size) throw new SerializerException($"Failed to read {size} bytes serialized data", new IOException());
            return deserializer(buffer);
        }

        /// <summary>
        /// Read a numeric value
        /// </summary>
        /// <typeparam name="T">Numeric type</typeparam>
        /// <param name="context">Context</param>
        /// <param name="size">Serialized data size in bytes</param>
        /// <param name="deserializer">Deserializer action</param>
        /// <returns>Value</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static async Task<T> ReadNumericAsync<T>(IDeserializationContext context, int size, NumericReader_Delegate<T> deserializer)
        {
            byte[] data = await ReadSerializedDataAsync(context.Stream, size, context).DynamicContext();
            try
            {
                return deserializer(data.AsSpan(0, size));
            }
            finally
            {
                context.BufferPool.Return(data, clearArray: false);
            }
        }

        /// <summary>
        /// Read a nullable numeric
        /// </summary>
        /// <typeparam name="T">Numeric type</typeparam>
        /// <param name="context">Context</param>
        /// <param name="size">Serialized data size in bytes</param>
        /// <param name="min">Minimum</param>
        /// <param name="max">Maximum</param>
        /// <param name="deserializer">Deserializer function to execute, if the red value isn't a default handleable numeric value</param>
        /// <returns>Value</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [SkipLocalsInit]
        private static T? ReadNullableNumeric<T>(IDeserializationContext context, int size, T min, T max, NumericReader_Delegate<T> deserializer)
            where T : struct, IConvertible
        {
            switch (context.SerializerVersion)// Serializer version switch
            {
                case 1:
                case 2:
                    {
                        if (!ReadBool(context.Stream, context)) return null;
                    }
                    break;
                default:
                    {
                        if (size > 2 && context.TryReadCachedNumber(out T? res, readType: true)) return res;
                        switch (size > 2 && context.IsCacheEnabled 
                            ? context.LastNumberType!.Value 
                            : (NumberTypes)ReadOneByte(context.Stream, context))
                        {
                            case NumberTypes.IsNull: return null;
                            case NumberTypes.MinValue: return min;
                            case NumberTypes.MaxValue: return max;
                            case NumberTypes.Zero: return default(T);
                        }
                    }
                    break;
            }
            Span<byte> buffer = stackalloc byte[size];
            if (context.Stream.Read(buffer) != size) throw new SerializerException($"Failed to read {size} bytes serialized data", new IOException());
            return deserializer(buffer);
        }

        /// <summary>
        /// Read a nullable numeric
        /// </summary>
        /// <typeparam name="T">Numeric type</typeparam>
        /// <param name="context">Context</param>
        /// <param name="size">Serialized data size in bytes</param>
        /// <param name="min">Minimum</param>
        /// <param name="max">Maximum</param>
        /// <param name="deserializer">Deserializer function to execute, if the red value isn't a default handleable numeric value</param>
        /// <returns>Value</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static async Task<T?> ReadNullableNumericAsync<T>(IDeserializationContext context, int size, T min, T max, NumericReader_Delegate<T> deserializer)
            where T : struct, IConvertible
        {
            switch (context.SerializerVersion)// Serializer version switch
            {
                case 1:
                case 2:
                    {
                        if (!await ReadBoolAsync(context.Stream, context).DynamicContext()) return null;
                    }
                    break;
                default:
                    {
                        if (size > 2)
                        {
                            (bool cached, T? res) = await context.TryReadCachedNumberAsync<T>(readType: true).DynamicContext();
                            if (cached) return res;
                        }
                        switch (size > 2 && context.IsCacheEnabled 
                            ? context.LastNumberType!.Value 
                            : (NumberTypes)await ReadOneByteAsync(context.Stream, context).DynamicContext())
                        {
                            case NumberTypes.IsNull: return null;
                            case NumberTypes.MinValue: return min;
                            case NumberTypes.MaxValue: return max;
                            case NumberTypes.Zero: return default(T);
                        }
                    }
                    break;
            }
            byte[] data = await ReadSerializedDataAsync(context.Stream, size, context).DynamicContext();
            try
            {
                return deserializer(data);
            }
            finally
            {
                context.BufferPool.Return(data, clearArray: false);
            }
        }

        /// <summary>
        /// Delegate for a numeric reader
        /// </summary>
        /// <typeparam name="T">Number type</typeparam>
        /// <param name="data">Serialized data</param>
        /// <returns>Number</returns>
        private delegate T NumericReader_Delegate<T>(Span<byte> data);
    }
}
