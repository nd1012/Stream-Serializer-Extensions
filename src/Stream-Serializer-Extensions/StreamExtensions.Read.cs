using System.Buffers;
using System.Reflection;
using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    // Read
    public static partial class StreamExtensions
    {
        /// <summary>
        /// Read object method
        /// </summary>
        public static readonly MethodInfo ReadObjectMethod;
        /// <summary>
        /// Read object method
        /// </summary>
        public static readonly MethodInfo ReadObjectAsyncMethod;
        /// <summary>
        /// Read struct method
        /// </summary>
        public static readonly MethodInfo ReadStructMethod;
        /// <summary>
        /// Read struct method
        /// </summary>
        public static readonly MethodInfo ReadStructAsyncMethod;
        /// <summary>
        /// Read object method
        /// </summary>
        public static readonly MethodInfo ReadObjectNullableMethod;
        /// <summary>
        /// Read object method
        /// </summary>
        public static readonly MethodInfo ReadObjectNullableAsyncMethod;
        /// <summary>
        /// Read any method
        /// </summary>
        public static readonly MethodInfo ReadAnyMethod;
        /// <summary>
        /// Read any method
        /// </summary>
        public static readonly MethodInfo ReadAnyAsyncMethod;
        /// <summary>
        /// Read any method
        /// </summary>
        public static readonly MethodInfo ReadAnyNullableMethod;
        /// <summary>
        /// Read any method
        /// </summary>
        public static readonly MethodInfo ReadAnyNullableAsyncMethod;
        /// <summary>
        /// Read serialized method
        /// </summary>
        public static readonly MethodInfo ReadSerializedMethod;
        /// <summary>
        /// Read serialized method
        /// </summary>
        public static readonly MethodInfo ReadSerializedAsyncMethod;
        /// <summary>
        /// Read number method
        /// </summary>
        public static readonly MethodInfo ReadNumberMethod;
        /// <summary>
        /// Read number method
        /// </summary>
        public static readonly MethodInfo ReadNumberAsyncMethod;
        /// <summary>
        /// Read enumeration method
        /// </summary>
        public static readonly MethodInfo ReadEnumMethod;
        /// <summary>
        /// Read enumeration method
        /// </summary>
        public static readonly MethodInfo ReadEnumAsyncMethod;
        /// <summary>
        /// Read array method
        /// </summary>
        public static readonly MethodInfo ReadArrayMethod;
        /// <summary>
        /// Read array method
        /// </summary>
        public static readonly MethodInfo ReadArrayAsyncMethod;
        /// <summary>
        /// Read list method
        /// </summary>
        public static readonly MethodInfo ReadListMethod;
        /// <summary>
        /// Read list method
        /// </summary>
        public static readonly MethodInfo ReadListAsyncMethod;
        /// <summary>
        /// Read dictionary method
        /// </summary>
        public static readonly MethodInfo ReadDictMethod;
        /// <summary>
        /// Read dictionary method
        /// </summary>
        public static readonly MethodInfo ReadDictAsyncMethod;
        /// <summary>
        /// Array empty method
        /// </summary>
        public static readonly MethodInfo ArrayEmptyMethod;

        /// <summary>
        /// Require the <see cref="StreamSerializerAttribute"/> attribute when using <see cref="ReadAnyObject{T}(Stream, int?)"/> etc.?
        /// </summary>
        public static bool AnyObjectAttributeRequired { get; set; } = true;

        /// <summary>
        /// Read the serializer version
        /// </summary>
        /// <param name="stream">Steam</param>
        /// <returns>Serializer version</returns>
        public static int ReadSerializerVersion(this Stream stream)
        {
            int res = ReadNumber<int>(stream, version: 1);
            if (res < 1 || res > StreamSerializer.VERSION) throw new InvalidDataException($"Invalid or unsupported stream serializer version #{res}");
            return res;
        }

        /// <summary>
        /// Read the serializer version
        /// </summary>
        /// <param name="stream">Steam</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Serializer version</returns>
        public static async Task<int> ReadSerializerVersionAsync(this Stream stream, CancellationToken cancellationToken = default)
        {
            int res = await ReadNumberAsync<int>(stream, version: 1, cancellationToken: cancellationToken).DynamicContext();
            if (res < 1 || res > StreamSerializer.VERSION) throw new InvalidDataException($"Invalid or unsupported stream serializer version #{res}");
            return res;
        }

        /// <summary>
        /// Read serialized data
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="len">Length in bytes</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Serialized data (a pool array which needs to be returned to the pool after use and might be larger than the given length!)</returns>
        public static byte[] ReadSerializedData(this Stream stream, int len, ArrayPool<byte>? pool = null)
        {
            byte[] res = (pool ?? StreamSerializer.BufferPool).Rent(len);
            try
            {
                if (stream.Read(res.AsSpan(0, len)) != len)
                    throw new SerializerException($"Failed to read serialized data ({len} bytes)");
                return res;
            }
            catch
            {
                (pool ?? StreamSerializer.BufferPool).Return(res);
                throw;
            }
        }

        /// <summary>
        /// Read serialized data
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="len">Length in bytes</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Serialized data (a pool array which needs to be returned to the pool after use and might be larger than the given length!)</returns>
        public static async Task<byte[]> ReadSerializedDataAsync(this Stream stream, int len, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
        {
            byte[] res = (pool ?? StreamSerializer.BufferPool).Rent(len);
            try
            {
                if (await stream.ReadAsync(res.AsMemory(0, len), cancellationToken).DynamicContext() != len)
                    throw new SerializerException($"Failed to read serialized data ({len} bytes)");
                return res;
            }
            catch
            {
                (pool ?? StreamSerializer.BufferPool).Return(res);
                throw;
            }
        }
    }
}
