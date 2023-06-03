using System.Buffers;
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
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused parameter
        public static bool ReadBool(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
#pragma warning restore IDE0060
        {
            byte[] data = ReadSerializedData(stream, len: 1, pool);
            try
            {
                return data[0] == 1;
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
            finally
            {
                (pool ?? StreamSerializer.BufferPool).Return(data);
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
#pragma warning disable IDE0060 // Remove unused parameter
        public static async Task<bool> ReadBoolAsync(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            byte[] data = await ReadSerializedDataAsync(stream, len: 1, pool, cancellationToken: cancellationToken).DynamicContext();
            try
            {
                return data[0] == 1;
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
            finally
            {
                (pool ?? StreamSerializer.BufferPool).Return(data);
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        public static bool? ReadBoolNullable(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
            => ReadBool(stream, version, pool) ? ReadBool(stream, version, pool) : default(bool?);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static async Task<bool?> ReadBoolNullableAsync(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
            => await ReadBoolAsync(stream, version, pool, cancellationToken).DynamicContext()
                ? await ReadBoolAsync(stream, version, pool, cancellationToken).DynamicContext()
                : default(bool?);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <returns>Value</returns>
        public static sbyte ReadOneSByte(this Stream stream, int? version = null)
        {
            try
            {
                return (sbyte)ReadOneByte(stream, version);
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused parameter
        public static async Task<sbyte> ReadOneSByteAsync(this Stream stream, int? version = null, CancellationToken cancellationToken = default)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            await Task.Yield();
            return ReadOneSByte(stream, version);
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <returns>Value</returns>
        public static sbyte? ReadOneSByteNullable(this Stream stream, int? version = null)
            => ReadBool(stream, version) ? ReadOneSByte(stream, version) : default(sbyte?);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static async Task<sbyte?> ReadOneSByteNullableAsync(this Stream stream, int? version = null, CancellationToken cancellationToken = default)
            => await ReadBoolAsync(stream, version, cancellationToken: cancellationToken).DynamicContext()
                ? await ReadOneSByteAsync(stream, version, cancellationToken).DynamicContext()
                : default(sbyte?);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused parameter
        public static byte ReadOneByte(this Stream stream, int? version = null)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            try
            {
                int res = stream.ReadByte();
                if (res < 0) throw new SerializerException("Failed to read one byte from stream");
                return (byte)res;
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused parameter
        public static async Task<byte> ReadOneByteAsync(this Stream stream, int? version = null, CancellationToken cancellationToken = default)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            await Task.Yield();
            return ReadOneByte(stream, version);
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <returns>Value</returns>
        public static ushort? ReadOneByteNullable(this Stream stream, int? version = null)
            => ReadBool(stream, version) ? ReadOneByte(stream, version) : default(ushort?);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static async Task<ushort?> ReadOneByteNullableAsync(this Stream stream, int? version = null, CancellationToken cancellationToken = default)
            => await ReadBoolAsync(stream, version, cancellationToken: cancellationToken).DynamicContext()
                ? await ReadOneByteAsync(stream, version, cancellationToken).DynamicContext()
                : default(ushort?);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused parameter
        public static short ReadShort(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            byte[] data = ReadSerializedData(stream, len: sizeof(short), pool);
            try
            {
                return data.AsSpan().ToShort();
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
            finally
            {
                (pool ?? StreamSerializer.BufferPool).Return(data);
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
#pragma warning disable IDE0060 // Remove unused parameter
        public static async Task<short> ReadShortAsync(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            byte[] data = await ReadSerializedDataAsync(stream, len: sizeof(short), pool, cancellationToken).DynamicContext();
            try
            {
                return data.AsSpan().ToShort();
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
            finally
            {
                (pool ?? StreamSerializer.BufferPool).Return(data);
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        public static short? ReadShortNullable(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
            => ReadBool(stream, version, pool) ? ReadShort(stream, version, pool) : default(short?);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static async Task<short?> ReadShortNullableAsync(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
            => await ReadBoolAsync(stream, version, pool, cancellationToken).DynamicContext()
                ? await ReadShortAsync(stream, version, pool, cancellationToken).DynamicContext()
                : default(short?);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused parameter
        public static ushort ReadUShort(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            byte[] data = ReadSerializedData(stream, len: sizeof(ushort), pool);
            try
            {
                return data.AsSpan().ToUShort();
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
            finally
            {
                (pool ?? StreamSerializer.BufferPool).Return(data);
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
#pragma warning disable IDE0060 // Remove unused parameter
        public static async Task<ushort> ReadUShortAsync(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            byte[] data = await ReadSerializedDataAsync(stream, len: sizeof(ushort), pool, cancellationToken).DynamicContext();
            try
            {
                return data.AsSpan().ToUShort();
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
            finally
            {
                (pool ?? StreamSerializer.BufferPool).Return(data);
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        public static ushort? ReadUShortNullable(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
            => ReadBool(stream, version, pool) ? ReadUShort(stream, version, pool) : default(ushort?);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static async Task<ushort?> ReadUShortNullableAsync(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
            => await ReadBoolAsync(stream, version, pool, cancellationToken).DynamicContext()
                ? await ReadUShortAsync(stream, version, pool, cancellationToken).DynamicContext()
                : default(ushort?);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused parameter
        public static int ReadInt(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            byte[] data = ReadSerializedData(stream, len: sizeof(int), pool);
            try
            {
                return data.AsSpan().ToInt();
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
            finally
            {
                (pool ?? StreamSerializer.BufferPool).Return(data);
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
#pragma warning disable IDE0060 // Remove unused parameter
        public static async Task<int> ReadIntAsync(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            byte[] data = await ReadSerializedDataAsync(stream, len: sizeof(int), pool, cancellationToken).DynamicContext();
            try
            {
                return data.AsSpan().ToInt();
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
            finally
            {
                (pool ?? StreamSerializer.BufferPool).Return(data);
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        public static int? ReadIntNullable(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
            => ReadBool(stream, version, pool) ? ReadInt(stream, version, pool) : default(int?);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static async Task<int?> ReadIntNullableAsync(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
            => await ReadBoolAsync(stream, version, pool, cancellationToken).DynamicContext()
                ? await ReadIntAsync(stream, version, pool, cancellationToken).DynamicContext()
                : default(int?);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused parameter
        public static uint ReadUInt(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            byte[] data = ReadSerializedData(stream, len: sizeof(uint), pool);
            try
            {
                return data.AsSpan().ToUInt();
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
            finally
            {
                (pool ?? StreamSerializer.BufferPool).Return(data);
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
#pragma warning disable IDE0060 // Remove unused parameter
        public static async Task<uint> ReadUIntAsync(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            byte[] data = await ReadSerializedDataAsync(stream, len: sizeof(uint), pool, cancellationToken).DynamicContext();
            try
            {
                return data.AsSpan().ToUInt();
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
            finally
            {
                (pool ?? StreamSerializer.BufferPool).Return(data);
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        public static uint? ReadUIntNullable(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
            => ReadBool(stream, version, pool) ? ReadUInt(stream, version, pool) : default(uint?);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static async Task<uint?> ReadUIntNullableAsync(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
            => await ReadBoolAsync(stream, version, pool, cancellationToken).DynamicContext()
                ? await ReadUIntAsync(stream, version, pool, cancellationToken).DynamicContext()
                : default(uint?);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused parameter
        public static long ReadLong(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            byte[] data = ReadSerializedData(stream, len: sizeof(long), pool);
            try
            {
                return data.AsSpan().ToLong();
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
            finally
            {
                (pool ?? StreamSerializer.BufferPool).Return(data);
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
#pragma warning disable IDE0060 // Remove unused parameter
        public static async Task<long> ReadLongAsync(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            byte[] data = await ReadSerializedDataAsync(stream, len: sizeof(long), pool, cancellationToken).DynamicContext();
            try
            {
                return data.AsSpan().ToLong();
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
            finally
            {
                (pool ?? StreamSerializer.BufferPool).Return(data);
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        public static long? ReadLongNullable(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
            => ReadBool(stream, version, pool) ? ReadLong(stream, version, pool) : default(long?);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static async Task<long?> ReadLongNullableAsync(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
            => await ReadBoolAsync(stream, version, pool, cancellationToken).DynamicContext()
                ? await ReadLongAsync(stream, version, pool, cancellationToken).DynamicContext()
                : default(long?);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused parameter
        public static ulong ReadULong(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            byte[] data = ReadSerializedData(stream, len: sizeof(ulong), pool);
            try
            {
                return data.AsSpan().ToULong();
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
            finally
            {
                (pool ?? StreamSerializer.BufferPool).Return(data);
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
#pragma warning disable IDE0060 // Remove unused parameter
        public static async Task<ulong> ReadULongAsync(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            byte[] data = await ReadSerializedDataAsync(stream, len: sizeof(ulong), pool, cancellationToken).DynamicContext();
            try
            {
                return data.AsSpan().ToULong();
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
            finally
            {
                (pool ?? StreamSerializer.BufferPool).Return(data);
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        public static ulong? ReadULongNullable(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
            => ReadBool(stream, version, pool) ? ReadULong(stream, version, pool) : default(ulong?);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static async Task<ulong?> ReadULongNullableAsync(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
            => await ReadBoolAsync(stream, version, pool, cancellationToken).DynamicContext()
                ? await ReadULongAsync(stream, version, pool, cancellationToken).DynamicContext()
                : default(ulong?);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused parameter
        public static float ReadFloat(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            byte[] data = ReadSerializedData(stream, len: sizeof(float), pool);
            try
            {
                return data.AsSpan().ToFloat();
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
            finally
            {
                (pool ?? StreamSerializer.BufferPool).Return(data);
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
#pragma warning disable IDE0060 // Remove unused parameter
        public static async Task<float> ReadFloatAsync(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            byte[] data = await ReadSerializedDataAsync(stream, len: sizeof(float), pool, cancellationToken).DynamicContext();
            try
            {
                return data.AsSpan().ToFloat();
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
            finally
            {
                (pool ?? StreamSerializer.BufferPool).Return(data);
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        public static float? ReadFloatNullable(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
            => ReadBool(stream, version, pool) ? ReadFloat(stream, version, pool) : default(float?);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static async Task<float?> ReadFloatNullableAsync(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
            => await ReadBoolAsync(stream, version, pool, cancellationToken).DynamicContext()
                ? await ReadFloatAsync(stream, version, pool, cancellationToken).DynamicContext()
                : default(float?);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused parameter
        public static double ReadDouble(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            byte[] data = ReadSerializedData(stream, len: sizeof(double), pool);
            try
            {
                return data.AsSpan().ToDouble();
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
            finally
            {
                (pool ?? StreamSerializer.BufferPool).Return(data);
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
#pragma warning disable IDE0060 // Remove unused parameter
        public static async Task<double> ReadDoubleAsync(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            byte[] data = await ReadSerializedDataAsync(stream, len: sizeof(double), pool, cancellationToken).DynamicContext();
            try
            {
                return data.AsSpan().ToDouble();
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
            finally
            {
                (pool ?? StreamSerializer.BufferPool).Return(data);
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        public static double? ReadDoubleNullable(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
            => ReadBool(stream, version, pool) ? ReadDouble(stream, version, pool) : default(double?);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static async Task<double?> ReadDoubleNullableAsync(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
            => await ReadBoolAsync(stream, version, pool, cancellationToken).DynamicContext()
                ? await ReadDoubleAsync(stream, version, pool, cancellationToken).DynamicContext()
                : default(double?);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused parameter
        public static decimal ReadDecimal(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            byte[] data = ReadSerializedData(stream, len: sizeof(int) << 2, pool);
            try
            {
                return data.AsSpan().ToDecimal();
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
            finally
            {
                (pool ?? StreamSerializer.BufferPool).Return(data);
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
#pragma warning disable IDE0060 // Remove unused parameter
        public static async Task<decimal> ReadDecimalAsync(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            byte[] data = await ReadSerializedDataAsync(stream, len: sizeof(int) << 2, pool, cancellationToken).DynamicContext();
            try
            {
                return data.AsSpan().ToDecimal();
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
            finally
            {
                (pool ?? StreamSerializer.BufferPool).Return(data);
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        public static decimal? ReadDecimalNullable(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
            => ReadBool(stream, version, pool) ? ReadDecimal(stream, version, pool) : default(decimal?);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static async Task<decimal?> ReadDecimalNullableAsync(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
            => await ReadBoolAsync(stream, version, pool, cancellationToken).DynamicContext()
                ? await ReadDecimalAsync(stream, version, pool, cancellationToken).DynamicContext()
                : default(decimal?);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        public static T ReadNumber<T>(this Stream stream, int? version = null, ArrayPool<byte>? pool = null) where T : struct, IConvertible
            => ReadNumberInt<T>(stream, version, numberType: null, pool);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="numberType">Number type</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        private static T ReadNumberInt<T>(Stream stream, int? version, NumberTypes? numberType, ArrayPool<byte>? pool) where T : struct, IConvertible
        {
            byte[] data = numberType == null ? ReadSerializedData(stream, len: 1, pool) : (pool ?? StreamSerializer.BufferPool).Rent(minimumLength: 1);
            try
            {
                NumberTypes type = numberType ?? (NumberTypes)data[0];
                if (type.IsZero()) return (T)Activator.CreateInstance(typeof(T))!;
                switch (type.RemoveValueFlags())
                {
                    case NumberTypes.Byte:
                        switch (type)
                        {
                            case NumberTypes.Byte | NumberTypes.MinValue:
                                return (T)(object)sbyte.MinValue;
                            case NumberTypes.Byte | NumberTypes.MaxValue:
                                return (T)(object)sbyte.MaxValue;
                        }
                        if (stream.Read(data.AsSpan(0, 1)) != 1) throw new SerializerException("Failed to read serialized data (1 bytes)");
                        return data[0].ConvertType<T>();
                    case NumberTypes.Byte | NumberTypes.Unsigned:
                        switch (type)
                        {
                            case NumberTypes.Byte | NumberTypes.MaxValue | NumberTypes.Unsigned:
                                return (T)(object)byte.MaxValue;
                        }
                        if (stream.Read(data.AsSpan(0, 1)) != 1) throw new SerializerException("Failed to read serialized data (1 bytes)");
                        return data[0].ConvertType<T>();
                    case NumberTypes.Short:
                        return type switch
                        {
                            NumberTypes.Short | NumberTypes.MinValue => (T)(object)short.MinValue,
                            NumberTypes.Short | NumberTypes.MaxValue => (T)(object)short.MaxValue,
                            _ => ReadShort(stream, version, pool).ConvertType<T>()
                        };
                    case NumberTypes.Short | NumberTypes.Unsigned:
                        return type switch
                        {
                            NumberTypes.Short | NumberTypes.MaxValue | NumberTypes.Unsigned => (T)(object)ushort.MaxValue,
                            _ => ReadUShort(stream, version, pool).ConvertType<T>()
                        };
                    case NumberTypes.Int:
                        return type switch
                        {
                            NumberTypes.Int | NumberTypes.MinValue => (T)(object)int.MinValue,
                            NumberTypes.Int | NumberTypes.MaxValue => (T)(object)int.MaxValue,
                            _ => ReadInt(stream, version, pool).ConvertType<T>()
                        };
                    case NumberTypes.Int | NumberTypes.Unsigned:
                        return type switch
                        {
                            NumberTypes.Int | NumberTypes.MaxValue | NumberTypes.Unsigned => (T)(object)uint.MaxValue,
                            _ => ReadUInt(stream, version, pool).ConvertType<T>()
                        };
                    case NumberTypes.Long:
                        return type switch
                        {
                            NumberTypes.Long | NumberTypes.MinValue => (T)(object)long.MinValue,
                            NumberTypes.Long | NumberTypes.MaxValue => (T)(object)long.MaxValue,
                            _ => ReadLong(stream, version, pool).ConvertType<T>()
                        };
                    case NumberTypes.Long | NumberTypes.Unsigned:
                        return type switch
                        {
                            NumberTypes.Long | NumberTypes.MaxValue | NumberTypes.Unsigned => (T)(object)ulong.MaxValue,
                            _ => ReadULong(stream, version, pool).ConvertType<T>()
                        };
                    case NumberTypes.Float:
                        return type switch
                        {
                            NumberTypes.Float | NumberTypes.MinValue => (T)(object)float.MinValue,
                            NumberTypes.Float | NumberTypes.MaxValue => (T)(object)float.MaxValue,
                            _ => ReadFloat(stream, version, pool).ConvertType<T>()
                        };
                    case NumberTypes.Double:
                        return type switch
                        {
                            NumberTypes.Double | NumberTypes.MinValue => (T)(object)double.MinValue,
                            NumberTypes.Double | NumberTypes.MaxValue => (T)(object)double.MaxValue,
                            _ => ReadDouble(stream, version, pool).ConvertType<T>()
                        };
                    case NumberTypes.Decimal:
                        return type switch
                        {
                            NumberTypes.Decimal | NumberTypes.MinValue => (T)(object)decimal.MinValue,
                            NumberTypes.Decimal | NumberTypes.MaxValue => (T)(object)decimal.MaxValue,
                            _ => ReadDecimal(stream, version, pool).ConvertType<T>()
                        };
                    default:
                        throw new SerializerException($"Unknown numeric type {type}");
                }
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
            finally
            {
                (pool ?? StreamSerializer.BufferPool).Return(data);
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
        public static Task<T> ReadNumberAsync<T>(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
            where T : struct, IConvertible
            => ReadNumberIntAsync<T>(stream, version, numberType: null, pool, cancellationToken);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="numberType">Number type</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        private static async Task<T> ReadNumberIntAsync<T>(Stream stream, int? version, NumberTypes? numberType, ArrayPool<byte>? pool, CancellationToken cancellationToken)
            where T : struct, IConvertible
        {
            byte[] data = numberType == null
                ? await ReadSerializedDataAsync(stream, len: 1, pool, cancellationToken).DynamicContext()
                : (pool ?? StreamSerializer.BufferPool).Rent(minimumLength: 1);
            try
            {
                NumberTypes type = numberType ?? (NumberTypes)data[0];
                if (type.IsZero()) return (T)Activator.CreateInstance(typeof(T))!;
                switch (type.RemoveValueFlags())
                {
                    case NumberTypes.Byte:
                        switch (type)
                        {
                            case NumberTypes.Byte | NumberTypes.MinValue:
                                return (T)(object)sbyte.MinValue;
                            case NumberTypes.Byte | NumberTypes.MaxValue:
                                return (T)(object)sbyte.MaxValue;
                        }
                        if (await stream.ReadAsync(data.AsMemory(0, 1), cancellationToken).DynamicContext() != 1)
                            throw new SerializerException("Failed to read serialized data (1 bytes)");
                        return data[0].ConvertType<T>();
                    case NumberTypes.Byte | NumberTypes.Unsigned:
                        switch (type)
                        {
                            case NumberTypes.Byte | NumberTypes.MaxValue | NumberTypes.Unsigned:
                                return (T)(object)byte.MaxValue;
                        }
                        if (await stream.ReadAsync(data.AsMemory(0, 1), cancellationToken).DynamicContext() != 1)
                            throw new SerializerException("Failed to read serialized data (1 bytes)");
                        return data[0].ConvertType<T>();
                    case NumberTypes.Short:
                        return type switch
                        {
                            NumberTypes.Short | NumberTypes.MinValue => (T)(object)short.MinValue,
                            NumberTypes.Short | NumberTypes.MaxValue => (T)(object)short.MaxValue,
                            _ => (await ReadShortAsync(stream, version, pool, cancellationToken).DynamicContext()).ConvertType<T>()
                        };
                    case NumberTypes.Short | NumberTypes.Unsigned:
                        return type switch
                        {
                            NumberTypes.Short | NumberTypes.MaxValue | NumberTypes.Unsigned => (T)(object)ushort.MaxValue,
                            _ => (await ReadUShortAsync(stream, version, pool, cancellationToken).DynamicContext()).ConvertType<T>()
                        };
                    case NumberTypes.Int:
                        return type switch
                        {
                            NumberTypes.Int | NumberTypes.MinValue => (T)(object)int.MinValue,
                            NumberTypes.Int | NumberTypes.MaxValue => (T)(object)int.MaxValue,
                            _ => (await ReadIntAsync(stream, version, pool, cancellationToken).DynamicContext()).ConvertType<T>()
                        };
                    case NumberTypes.Int | NumberTypes.Unsigned:
                        return type switch
                        {
                            NumberTypes.Int | NumberTypes.MaxValue | NumberTypes.Unsigned => (T)(object)uint.MaxValue,
                            _ => (await ReadUIntAsync(stream, version, pool, cancellationToken).DynamicContext()).ConvertType<T>()
                        };
                    case NumberTypes.Long:
                        return type switch
                        {
                            NumberTypes.Long | NumberTypes.MinValue => (T)(object)long.MinValue,
                            NumberTypes.Long | NumberTypes.MaxValue => (T)(object)long.MaxValue,
                            _ => (await ReadLongAsync(stream, version, pool, cancellationToken).DynamicContext()).ConvertType<T>()
                        };
                    case NumberTypes.Long | NumberTypes.Unsigned:
                        return type switch
                        {
                            NumberTypes.Long | NumberTypes.MaxValue | NumberTypes.Unsigned => (T)(object)ulong.MaxValue,
                            _ => (await ReadULongAsync(stream, version, pool, cancellationToken).DynamicContext()).ConvertType<T>()
                        };
                    case NumberTypes.Float:
                        return type switch
                        {
                            NumberTypes.Float | NumberTypes.MinValue => (T)(object)float.MinValue,
                            NumberTypes.Float | NumberTypes.MaxValue => (T)(object)float.MaxValue,
                            _ => (await ReadFloatAsync(stream, version, pool, cancellationToken).DynamicContext()).ConvertType<T>()
                        };
                    case NumberTypes.Double:
                        return type switch
                        {
                            NumberTypes.Double | NumberTypes.MinValue => (T)(object)double.MinValue,
                            NumberTypes.Double | NumberTypes.MaxValue => (T)(object)double.MaxValue,
                            _ => (await ReadDoubleAsync(stream, version, pool, cancellationToken).DynamicContext()).ConvertType<T>()
                        };
                    case NumberTypes.Decimal:
                        return type switch
                        {
                            NumberTypes.Decimal | NumberTypes.MinValue => (T)(object)decimal.MinValue,
                            NumberTypes.Decimal | NumberTypes.MaxValue => (T)(object)decimal.MaxValue,
                            _ => (await ReadDecimalAsync(stream, version, pool, cancellationToken).DynamicContext()).ConvertType<T>()
                        };
                    default:
                        throw new SerializerException($"Unknown numeric type {type}");
                }
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
            finally
            {
                (pool ?? StreamSerializer.BufferPool).Return(data);
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        public static T? ReadNumberNullable<T>(this Stream stream, int? version = null, ArrayPool<byte>? pool = null) where T : struct, IConvertible
        {
            switch ((version ?? StreamSerializer.VERSION) & byte.MaxValue)
            {
                case 1:
                    return ReadBool(stream, version, pool) ? ReadNumber<T>(stream, version, pool) : null;
                default:
                    {
                        using RentedArray<byte> buffer = new(len: 1, pool, clean: false);
                        NumberTypes numberType = (NumberTypes)ReadOneByte(stream, version);
                        return numberType == NumberTypes.Null ? default(T?) : ReadNumberInt<T>(stream, version, numberType, pool);
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
        public static async Task<T?> ReadNumberNullableAsync<T>(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
            where T : struct, IConvertible
        {
            switch ((version ?? StreamSerializer.VERSION) & byte.MaxValue)
            {
                case 1:
                    return await ReadBoolAsync(stream, version, pool, cancellationToken).DynamicContext() 
                        ? await ReadNumberAsync<T>(stream, version, pool, cancellationToken).DynamicContext() 
                        : null;
                default:
                    {
                        using RentedArray<byte> buffer = new(len: 1, pool, clean: false);
                        NumberTypes numberType = (NumberTypes)await ReadOneByteAsync(stream, version, cancellationToken).DynamicContext();
                        return numberType == NumberTypes.Null ? null : await ReadNumberIntAsync<T>(stream, version, numberType, pool, cancellationToken).DynamicContext(); ;
                    }
            }
        }
    }
}
