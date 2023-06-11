using System.Buffers;
using System.Runtime;
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
            => ReadEnumInt<T>(stream, version, numberType: null, pool);

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Enumeration type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="numberType">Number type</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        private static T ReadEnumInt<T>(Stream stream, int? version, NumberTypes? numberType, ArrayPool<byte>? pool) where T : struct, Enum
        {
            try
            {
                if ((numberType ?? NumberTypes.None) == NumberTypes.Default) return default;
                numberType ??= (NumberTypes)ReadOneByte(stream, version);
                if ((version ?? StreamSerializer.VERSION) > 1 && numberType == NumberTypes.Default) return default;
                T res = (T)Enum.ToObject(
                    typeof(T),
                    ReadNumberIntMethod.MakeGenericMethod(typeof(T).GetEnumUnderlyingType()).InvokeAuto(obj: null, stream, version, numberType, pool)!
                    );
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
        public static Task<T> ReadEnumAsync<T>(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
            where T : struct, Enum
            => ReadEnumIntAsync<T>(stream, version, numberType: null, pool, cancellationToken);

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Enumeration type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="numberType">Number type</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        private static async Task<T> ReadEnumIntAsync<T>(Stream stream, int? version, NumberTypes? numberType, ArrayPool<byte>? pool, CancellationToken cancellationToken)
            where T : struct, Enum
        {
            try
            {
                if ((numberType ?? NumberTypes.None) == NumberTypes.Default) return default;
                numberType ??= (NumberTypes)await ReadOneByteAsync(stream, version, cancellationToken).DynamicContext();
                if ((version ?? StreamSerializer.VERSION) > 1 && numberType == NumberTypes.Default) return default;
                Type type = typeof(T).GetEnumUnderlyingType();
                Task task = (Task)ReadNumberIntAsyncMethod.MakeGenericMethod(type).InvokeAuto(obj: null, stream, version, numberType, pool, cancellationToken)!;
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
        [TargetedPatchingOptOut("Tiny method")]
        public static T? ReadEnumNullable<T>(this Stream stream, int? version = null, ArrayPool<byte>? pool = null) where T : struct, Enum
        {
            switch ((version ?? StreamSerializer.VERSION) & byte.MaxValue)
            {
                case 1:
                    return ReadBool(stream, version, pool) ? ReadEnum<T>(stream, version, pool) : default(T?);
                default:
                    NumberTypes numberType = (NumberTypes)ReadOneByte(stream, version);
                    return numberType == NumberTypes.Null ? default(T?) : ReadEnumInt<T>(stream, version, numberType, pool);
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
        [TargetedPatchingOptOut("Tiny method")]
        public static async Task<T?> ReadEnumNullableAsync<T>(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
            where T : struct, Enum
        {
            switch ((version ?? StreamSerializer.VERSION) & byte.MaxValue)
            {
                case 1:
                    return await ReadBoolAsync(stream, version, pool, cancellationToken).DynamicContext()
                        ? await ReadEnumAsync<T>(stream, version, pool, cancellationToken).DynamicContext()
                        : default(T?);
                default:
                    NumberTypes numberType = (NumberTypes)await ReadOneByteAsync(stream, version, cancellationToken).DynamicContext();
                    return numberType == NumberTypes.Null ? default(T?) : await ReadEnumIntAsync<T>(stream, version, numberType, pool, cancellationToken).DynamicContext();
            }
        }
    }
}
