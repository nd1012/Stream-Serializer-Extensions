using System.Buffers;
using System.Runtime;
using System.Runtime.CompilerServices;
using wan24.Core;

//TODO Use Predicate?

namespace wan24.StreamSerializerExtensions
{
    // Basic
    public static partial class StreamExtensions
    {
        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        public static bool ReadBool(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
            => ReadNumeric(stream, sizeof(bool), version, pool, (data) => data[0] == 1);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static Task<bool> ReadBoolAsync(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
            => ReadNumericAsync(stream, sizeof(bool), version, pool, (data) => data[0] == 1, cancellationToken);

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
        public static bool? ReadBoolNullable(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
        {
            switch ((version ??= StreamSerializer.VERSION) & byte.MaxValue)// Serializer version switch
            {
                case 1:
                case 2:
                    {
                        return ReadBool(stream, version, pool) ? ReadBool(stream, version, pool) : null;
                    }
                default:
                    {
                        ObjectTypes type = (ObjectTypes)ReadOneByte(stream, version);
                        return type switch
                        {
                            ObjectTypes.Null => null,
                            ObjectTypes.Bool => true,
                            ObjectTypes.Empty => false,
                            _ => throw new SerializerException($"Invalid boolean {type}", new InvalidDataException())
                        };
                    }
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<bool?> ReadBoolNullableAsync(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
        {
            switch ((version ??= StreamSerializer.VERSION) & byte.MaxValue)// Serializer version switch
            {
                case 1:
                case 2:
                    {
                        return await ReadBoolAsync(stream, version, pool, cancellationToken).DynamicContext()
                            ? await ReadBoolAsync(stream, version, pool, cancellationToken).DynamicContext()
                            : null;
                    }
                default:
                    {
                        ObjectTypes type = (ObjectTypes)await ReadOneByteAsync(stream, version, cancellationToken).DynamicContext();
                        return type switch
                        {
                            ObjectTypes.Null => null,
                            ObjectTypes.Bool => true,
                            ObjectTypes.Empty => false,
                            _ => throw new SerializerException($"Invalid boolean {type}", new InvalidDataException())
                        };
                    }
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte ReadOneSByte(this Stream stream, int? version = null) => (sbyte)ReadOneByte(stream, version);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused parameter
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<sbyte> ReadOneSByteAsync(this Stream stream, int? version = null, CancellationToken cancellationToken = default)
#pragma warning restore IDE0060 // Remove unused parameter
            => Task.FromResult(ReadOneSByte(stream, version));

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <returns>Value</returns>
        public static sbyte? ReadOneSByteNullable(this Stream stream, int? version = null)
            => ReadNullableNumeric(stream, sizeof(sbyte), sbyte.MinValue, sbyte.MaxValue, version, pool: null, (data) => (sbyte)data[0]);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static Task<sbyte?> ReadOneSByteNullableAsync(this Stream stream, int? version = null, CancellationToken cancellationToken = default)
            => ReadNullableNumericAsync(stream, sizeof(sbyte), sbyte.MinValue, sbyte.MaxValue, version, pool: null, (data) => (sbyte)data[0], cancellationToken);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused parameter
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte ReadOneByte(this Stream stream, int? version = null)
#pragma warning restore IDE0060 // Remove unused parameter
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
        /// <param name="version">Serializer version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused parameter
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<byte> ReadOneByteAsync(this Stream stream, int? version = null, CancellationToken cancellationToken = default)
#pragma warning restore IDE0060 // Remove unused parameter
            => Task.FromResult(ReadOneByte(stream, version));

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <returns>Value</returns>
        public static ushort? ReadOneByteNullable(this Stream stream, int? version = null)
            => ReadNullableNumeric(stream, sizeof(byte), byte.MinValue, byte.MaxValue, version, pool: null, (data) => data[0]);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static Task<byte?> ReadOneByteNullableAsync(this Stream stream, int? version = null, CancellationToken cancellationToken = default)
            => ReadNullableNumericAsync(stream, sizeof(byte), byte.MinValue, byte.MaxValue, version, pool: null, (data) => data[0], cancellationToken);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        public static short ReadShort(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
            => ReadNumeric(stream, sizeof(short), version, pool, (data) => data.AsSpan().ToShort());

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static Task<short> ReadShortAsync(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
            => ReadNumericAsync(stream, sizeof(short), version, pool, (data) => data.AsSpan().ToShort(), cancellationToken);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        public static short? ReadShortNullable(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
            => ReadNullableNumeric(stream, sizeof(short), short.MinValue, short.MaxValue, version, pool, (data) => data.AsSpan().ToShort());

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static Task<short?> ReadShortNullableAsync(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
            => ReadNullableNumericAsync(stream, sizeof(short), short.MinValue, short.MaxValue, version, pool, (data) => data.AsSpan().ToShort(), cancellationToken);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        public static ushort ReadUShort(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
            => ReadNumeric(stream, sizeof(ushort), version, pool, (data) => data.AsSpan().ToUShort());

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static Task<ushort> ReadUShortAsync(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
            => ReadNumericAsync(stream, sizeof(ushort), version, pool, (data) => data.AsSpan().ToUShort(), cancellationToken);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        public static ushort? ReadUShortNullable(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
            => ReadNullableNumeric(stream, sizeof(ushort), ushort.MinValue, ushort.MaxValue, version, pool, (data) => data.AsSpan().ToUShort());

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static Task<ushort?> ReadUShortNullableAsync(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
            => ReadNullableNumericAsync(stream, sizeof(ushort), ushort.MinValue, ushort.MaxValue, version, pool, (data) => data.AsSpan().ToUShort(), cancellationToken);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        public static int ReadInt(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
            => ReadNumeric(stream, sizeof(int), version, pool, (data) => data.AsSpan().ToInt());

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static Task<int> ReadIntAsync(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
            => ReadNumericAsync(stream, sizeof(int), version, pool, (data) => data.AsSpan().ToInt(), cancellationToken);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        public static int? ReadIntNullable(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
            => ReadNullableNumeric(stream, sizeof(int), int.MinValue, int.MaxValue, version, pool, (data) => data.AsSpan().ToInt());

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static Task<int?> ReadIntNullableAsync(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
            => ReadNullableNumericAsync(stream, sizeof(int), int.MinValue, int.MaxValue, version, pool, (data) => data.AsSpan().ToInt(), cancellationToken);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        public static uint ReadUInt(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
            => ReadNumeric(stream, sizeof(uint), version, pool, (data) => data.AsSpan().ToUInt());

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static Task<uint> ReadUIntAsync(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
            => ReadNumericAsync(stream, sizeof(uint), version, pool, (data) => data.AsSpan().ToUInt(), cancellationToken);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        public static uint? ReadUIntNullable(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
            => ReadNullableNumeric(stream, sizeof(uint), uint.MinValue, uint.MaxValue, version, pool, (data) => data.AsSpan().ToUInt());

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static Task<uint?> ReadUIntNullableAsync(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
            => ReadNullableNumericAsync(stream, sizeof(uint), uint.MinValue, uint.MaxValue, version, pool, (data) => data.AsSpan().ToUInt(), cancellationToken);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        public static long ReadLong(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
            => ReadNumeric(stream, sizeof(long), version, pool, (data) => data.AsSpan().ToLong());

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static Task<long> ReadLongAsync(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
            => ReadNumericAsync(stream, sizeof(long), version, pool, (data) => data.AsSpan().ToLong(), cancellationToken);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        public static long? ReadLongNullable(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
            => ReadNullableNumeric(stream, sizeof(long), long.MinValue, long.MaxValue, version, pool, (data) => data.AsSpan().ToLong());

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static Task<long?> ReadLongNullableAsync(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
            => ReadNullableNumericAsync(stream, sizeof(long), long.MinValue, long.MaxValue, version, pool, (data) => data.AsSpan().ToLong(), cancellationToken);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        public static ulong ReadULong(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
            => ReadNumeric(stream, sizeof(ulong), version, pool, (data) => data.AsSpan().ToULong());

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static Task<ulong> ReadULongAsync(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
            => ReadNumericAsync(stream, sizeof(ulong), version, pool, (data) => data.AsSpan().ToULong(), cancellationToken);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        public static ulong? ReadULongNullable(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
            => ReadNullableNumeric(stream, sizeof(ulong), ulong.MinValue, ulong.MaxValue, version, pool, (data) => data.AsSpan().ToULong());

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static Task<ulong?> ReadULongNullableAsync(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
            => ReadNullableNumericAsync(stream, sizeof(ulong), ulong.MinValue, ulong.MaxValue, version, pool, (data) => data.AsSpan().ToULong(), cancellationToken);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        public static float ReadFloat(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
            => ReadNumeric(stream, sizeof(float), version, pool, (data) => data.AsSpan().ToFloat());

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static Task<float> ReadFloatAsync(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
            => ReadNumericAsync(stream, sizeof(float), version, pool, (data) => data.AsSpan().ToFloat(), cancellationToken);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        public static float? ReadFloatNullable(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
            => ReadNullableNumeric(stream, sizeof(float), float.MinValue, float.MaxValue, version, pool, (data) => data.AsSpan().ToFloat());

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static Task<float?> ReadFloatNullableAsync(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
            => ReadNullableNumericAsync(stream, sizeof(float), float.MinValue, float.MaxValue, version, pool, (data) => data.AsSpan().ToFloat(), cancellationToken);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        public static double ReadDouble(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
            => ReadNumeric(stream, sizeof(double), version, pool, (data) => data.AsSpan().ToDouble());

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static Task<double> ReadDoubleAsync(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
            => ReadNumericAsync(stream, sizeof(double), version, pool, (data) => data.AsSpan().ToDouble(), cancellationToken);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        public static double? ReadDoubleNullable(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
            => ReadNullableNumeric(stream, sizeof(double), double.MinValue, double.MaxValue, version, pool, (data) => data.AsSpan().ToDouble());

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static Task<double?> ReadDoubleNullableAsync(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
            => ReadNullableNumericAsync(stream, sizeof(double), double.MinValue, double.MaxValue, version, pool, (data) => data.AsSpan().ToDouble(), cancellationToken);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        public static decimal ReadDecimal(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
            => ReadNumeric(stream, sizeof(decimal), version, pool, (data) => data.AsSpan().ToDecimal());

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static Task<decimal> ReadDecimalAsync(
            this Stream stream,
            int? version = null,
            ArrayPool<byte>? pool = null,
            CancellationToken cancellationToken = default
            )
            => ReadNumericAsync(stream, sizeof(decimal), version, pool, (data) => data.AsSpan().ToDecimal(), cancellationToken);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        public static decimal? ReadDecimalNullable(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
            => ReadNullableNumeric(stream, sizeof(decimal), decimal.MinValue, decimal.MaxValue, version, pool, (data) => data.AsSpan().ToDecimal());

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static Task<decimal?> ReadDecimalNullableAsync(
            this Stream stream,
            int? version = null,
            ArrayPool<byte>? pool = null,
            CancellationToken cancellationToken = default
            )
            => ReadNullableNumericAsync(
                stream,
                sizeof(decimal),
                decimal.MinValue,
                decimal.MaxValue,
                version,
                pool,
                (data) => data.AsSpan().ToDecimal(),
                cancellationToken
                );

        /// <summary>
        /// Read a numeric value
        /// </summary>
        /// <typeparam name="T">Numeric type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="size">Serialized data size in bytes</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="deserializer">Deserializer action</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused parameter
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static T ReadNumeric<T>(Stream stream, int size, int? version, ArrayPool<byte>? pool, Func<byte[], T> deserializer) where T : struct, IConvertible
#pragma warning restore IDE0060 // Remove unused parameter
        {
            pool ??= StreamSerializer.BufferPool;
            byte[] data = ReadSerializedData(stream, size, pool);
            try
            {
                return deserializer(data);
            }
            finally
            {
                pool.Return(data, clearArray: false);
            }
        }

        /// <summary>
        /// Read a numeric value
        /// </summary>
        /// <typeparam name="T">Numeric type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="size">Serialized data size in bytes</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="deserializer">Deserializer action</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused parameter
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static async Task<T> ReadNumericAsync<T>(
            Stream stream, 
            int size, 
            int? version, 
            ArrayPool<byte>? pool, 
            Func<byte[], T> deserializer, 
            CancellationToken cancellationToken
            )
#pragma warning restore IDE0060 // Remove unused parameter
            where T : struct, IConvertible
        {
            pool ??= StreamSerializer.BufferPool;
            byte[] data = await ReadSerializedDataAsync(stream, size, pool, cancellationToken).DynamicContext();
            try
            {
                return deserializer(data);
            }
            finally
            {
                pool.Return(data, clearArray: false);
            }
        }

        /// <summary>
        /// Read a nullable numeric
        /// </summary>
        /// <typeparam name="T">Numeric type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="size">Serialized data size in bytes</param>
        /// <param name="min">Minimum</param>
        /// <param name="max">Maximum</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="deserializer">Deserializer function to execute, if the red value isn't a default handleable numeric value</param>
        /// <returns>Value</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static T? ReadNullableNumeric<T>(Stream stream, int size, T min, T max, int? version, ArrayPool<byte>? pool, Func<byte[], T> deserializer)
            where T : struct, IConvertible
        {
            pool ??= StreamSerializer.BufferPool;
            switch ((version ??= StreamSerializer.VERSION) & byte.MaxValue)// Serializer version switch
            {
                case 1:
                case 2:
                    {
                        if (!ReadBool(stream, version, pool)) return null;
                    }
                    break;
                default:
                    {
                        switch ((NumberTypes)ReadOneByte(stream, version))
                        {
                            case NumberTypes.Null: return null;
                            case NumberTypes.MinValue: return min;
                            case NumberTypes.MaxValue: return max;
                            case NumberTypes.Zero: return default(T);
                        }
                    }
                    break;
            }
            byte[] data = ReadSerializedData(stream, size, pool);
            try
            {
                return deserializer(data);
            }
            finally
            {
                pool.Return(data, clearArray: false);
            }
        }

        /// <summary>
        /// Read a nullable numeric
        /// </summary>
        /// <typeparam name="T">Numeric type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="size">Serialized data size in bytes</param>
        /// <param name="min">Minimum</param>
        /// <param name="max">Maximum</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="deserializer">Deserializer function to execute, if the red value isn't a default handleable numeric value</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static async Task<T?> ReadNullableNumericAsync<T>(
            Stream stream,
            int size,
            T min,
            T max,
            int? version,
            ArrayPool<byte>? pool,
            Func<byte[], T> deserializer,
            CancellationToken cancellationToken
            )
            where T : struct, IConvertible
        {
            pool ??= StreamSerializer.BufferPool;
            switch ((version ??= StreamSerializer.VERSION) & byte.MaxValue)// Serializer version switch
            {
                case 1:
                case 2:
                    {
                        if (!await ReadBoolAsync(stream, version, pool, cancellationToken).DynamicContext()) return null;
                    }
                    break;
                default:
                    {
                        switch ((NumberTypes)await ReadOneByteAsync(stream, version, cancellationToken).DynamicContext())
                        {
                            case NumberTypes.Null: return null;
                            case NumberTypes.MinValue: return min;
                            case NumberTypes.MaxValue: return max;
                            case NumberTypes.Zero: return default(T);
                        }
                    }
                    break;
            }
            byte[] data = await ReadSerializedDataAsync(stream, size, pool, cancellationToken).DynamicContext();
            try
            {
                return deserializer(data);
            }
            finally
            {
                pool.Return(data, clearArray: false);
            }
        }
    }
}
