using System.Buffers;
using System.Runtime;
using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    // List
    public static partial class StreamExtensions
    {
        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <param name="valueOptions">Value serializer options</param>
        /// <returns>Value</returns>
        public static List<T> ReadList<T>(
            this Stream stream,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            ISerializerOptions? valueOptions = null
            )
        {
            if (typeof(T) == typeof(byte)) return new List<T>((ReadBytes(stream, version, buffer: null, pool, minLen, maxLen) as T[])!);
            try
            {
                int len = ReadNumber<int>(stream, version, pool);
                SerializerHelper.EnsureValidLength(len, minLen, maxLen);
                List<T> res = new(len);
                for (int i = 0; i < len; res.Add(ReadObject<T>(stream, version, valueOptions)), i++) ;
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
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <param name="valueOptions">Value serializer options</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static async Task<List<T>> ReadListAsync<T>(
            this Stream stream,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            ISerializerOptions? valueOptions = null,
            CancellationToken cancellationToken = default
            )
        {
            if (typeof(T) == typeof(byte))
                return new List<T>((await ReadBytesAsync(stream, version, buffer: null, pool, minLen, maxLen, cancellationToken).DynamicContext() as T[])!);
            try
            {
                int len = await ReadNumberAsync<int>(stream, version, pool, cancellationToken).DynamicContext();
                SerializerHelper.EnsureValidLength(len, minLen, maxLen);
                List<T> res = new(len);
                for (int i = 0; i < len; res.Add(await ReadObjectAsync<T>(stream, version, valueOptions, cancellationToken).DynamicContext()), i++) ;
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
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <param name="valueOptions">Value serializer options</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static List<T>? ReadListNullable<T>(
            this Stream stream,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            ISerializerOptions? valueOptions = null
            )
#pragma warning disable IDE0034 // default expression can be simplified
            => ReadBool(stream, version, pool) ? ReadList<T>(stream, version, pool, minLen, maxLen, valueOptions) : default(List<T>?);
#pragma warning restore IDE0034 // default expression can be simplified

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <param name="valueOptions">Value serializer options</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static async Task<List<T>?> ReadListNullableAsync<T>(
            this Stream stream,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            ISerializerOptions? valueOptions = null,
            CancellationToken cancellationToken = default
            )
            => await ReadBoolAsync(stream, version, pool, cancellationToken).DynamicContext()
                ? await ReadListAsync<T>(stream, version, pool, minLen, maxLen, valueOptions, cancellationToken).DynamicContext()
                : default(List<T>?);
    }
}
