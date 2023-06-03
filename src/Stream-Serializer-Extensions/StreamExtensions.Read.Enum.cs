using System.Buffers;
using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    // Enumeration
    public static partial class StreamExtensions
    {
        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Enumeration type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        public static T ReadEnum<T>(this Stream stream, int? version = null, ArrayPool<byte>? pool = null) where T : struct, Enum
        {
            try
            {
                T res = (T)Enum.ToObject(typeof(T), ReadNumberMethod.MakeGenericMethod(typeof(T).GetEnumUnderlyingType()).InvokeAuto(obj: null, stream, version, pool)!);
                if (!res.IsValid()) throw new SerializerException($"Unknown enumeration value {res} for {typeof(T)}");
                return res;
            }
            catch (SerializerException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Enumeration type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static async Task<T> ReadEnumAsync<T>(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default) where T : struct, Enum
        {
            try
            {
                Type type = typeof(T).GetEnumUnderlyingType();
                Task task = (Task)ReadNumberAsyncMethod.MakeGenericMethod(type).InvokeAuto(obj: null, stream, version, pool, cancellationToken)!;
                await task.DynamicContext();
                T res = (T)Enum.ToObject(typeof(T), task.GetResult(type));
                if (!res.IsValid()) throw new SerializerException($"Unknown enumeration value {res} for {typeof(T)}");
                return res;
            }
            catch (SerializerException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Enumeration type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        public static T? ReadEnumNullable<T>(this Stream stream, int? version = null, ArrayPool<byte>? pool = null) where T : struct, Enum
            => ReadBool(stream, version, pool) ? ReadEnum<T>(stream, version, pool) : default(T?);

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Enumeration type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static async Task<T?> ReadEnumNullableAsync<T>(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
            where T : struct, Enum
            => await ReadBoolAsync(stream, version, pool, cancellationToken).DynamicContext()
                ? await ReadEnumAsync<T>(stream, version, pool, cancellationToken).DynamicContext()
                : default(T?);
    }
}
