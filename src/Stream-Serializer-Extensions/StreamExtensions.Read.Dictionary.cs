using System.Buffers;
using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    // Dictionary
    public static partial class StreamExtensions
    {
        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="tKey">Key type</typeparam>
        /// <typeparam name="tValue">Value type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <param name="keyOptions">Key serializer options</param>
        /// <param name="valueOptions">Value serializer options</param>
        /// <returns>Value</returns>
        public static Dictionary<tKey, tValue> ReadDict<tKey, tValue>(
            this Stream stream,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            ISerializerOptions? keyOptions = null,
            ISerializerOptions? valueOptions = null
            )
            where tKey : notnull
        {
            try
            {
                int len = ReadNumber<int>(stream, version, pool);
                SerializerHelper.EnsureValidLength(len, minLen, maxLen);
                Dictionary<tKey, tValue> res = new(len);
                for (int i = 0; i < len; i++)
                    res[ReadObject<tKey>(stream, version, keyOptions)]
                        = ReadObject<tValue>(stream, version, valueOptions);
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
        /// <typeparam name="tKey">Key type</typeparam>
        /// <typeparam name="tValue">Value type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <param name="keyOptions">Key serializer options</param>
        /// <param name="valueOptions">Value serializer options</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static async Task<Dictionary<tKey, tValue>> ReadDictAsync<tKey, tValue>(
            this Stream stream,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            ISerializerOptions? keyOptions = null,
            ISerializerOptions? valueOptions = null,
            CancellationToken cancellationToken = default
            )
            where tKey : notnull
        {
            try
            {
                int len = await ReadNumberAsync<int>(stream, version, pool, cancellationToken).DynamicContext();
                SerializerHelper.EnsureValidLength(len, minLen, maxLen);
                Dictionary<tKey, tValue> res = new(len);
                for (int i = 0; i < len; i++)
                    res[await ReadObjectAsync<tKey>(stream, version, keyOptions, cancellationToken).DynamicContext()]
                        = await ReadObjectAsync<tValue>(stream, version, valueOptions, cancellationToken).DynamicContext();
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
        /// <typeparam name="tKey">Key type</typeparam>
        /// <typeparam name="tValue">Value type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <param name="keyOptions">Key serializer options</param>
        /// <param name="valueOptions">Value serializer options</param>
        /// <returns>Value</returns>
        public static Dictionary<tKey, tValue>? ReadDictNullable<tKey, tValue>(
            this Stream stream,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            ISerializerOptions? keyOptions = null,
            ISerializerOptions? valueOptions = null
            )
            where tKey : notnull
#pragma warning disable IDE0034 // default expression can be simplified
            => ReadBool(stream, version, pool) ? ReadDict<tKey, tValue>(stream, version, pool, minLen, maxLen, keyOptions, valueOptions) : default(Dictionary<tKey, tValue>?);
#pragma warning restore IDE0034 // default expression can be simplified

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="tKey">Key type</typeparam>
        /// <typeparam name="tValue">Value type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <param name="keyOptions">Key serializer options</param>
        /// <param name="valueOptions">Value serializer options</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static async Task<Dictionary<tKey, tValue>?> ReadDictNullableAsync<tKey, tValue>(
            this Stream stream,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            ISerializerOptions? keyOptions = null,
            ISerializerOptions? valueOptions = null,
            CancellationToken cancellationToken = default
            )
            where tKey : notnull
            => await ReadBoolAsync(stream, version, pool, cancellationToken).DynamicContext()
                ? await ReadDictAsync<tKey, tValue>(stream, version, pool, minLen, maxLen, keyOptions, valueOptions, cancellationToken).DynamicContext()
                : default(Dictionary<tKey, tValue>?);
    }
}
