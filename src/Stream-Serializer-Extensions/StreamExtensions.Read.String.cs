using System.Buffers;
using System.Runtime.CompilerServices;
using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    // String
    public static partial class StreamExtensions
    {
        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length in bytes</param>
        /// <param name="maxLen">Maximum length in bytes</param>
        /// <returns>Value</returns>
        public static string ReadString(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, int minLen = 0, int maxLen = int.MaxValue)
            => ReadString(stream, version, minLen, maxLen, pool, (data, len) => data.AsSpan(0, len).ToUtf8String());

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length in bytes</param>
        /// <param name="maxLen">Maximum length in bytes</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static Task<string> ReadStringAsync(
            this Stream stream,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            CancellationToken cancellationToken = default
            )
            => ReadStringAsync(stream, version, minLen, maxLen, pool, (data, len) => data.AsSpan(0, len).ToUtf8String(), cancellationToken);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length in bytes</param>
        /// <param name="maxLen">Maximum length in bytes</param>
        /// <returns>Value</returns>
        public static string? ReadStringNullable(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, int minLen = 0, int maxLen = int.MaxValue)
            => ReadNullableString(stream, version, minLen, maxLen, pool, (data, len) => data.AsSpan(0, len).ToUtf8String());

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length in bytes</param>
        /// <param name="maxLen">Maximum length in bytes</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static Task<string?> ReadStringNullableAsync(
            this Stream stream,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            CancellationToken cancellationToken = default
            )
            => ReadNullableStringAsync(stream, version, minLen, maxLen, pool, (data, len) => data.AsSpan(0, len).ToUtf8String(), cancellationToken);

        /// <summary>
        /// Read UTF-16 (little endian) string
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length in bytes</param>
        /// <param name="maxLen">Maximum length in bytes</param>
        /// <returns>Value</returns>
        public static string ReadString16(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, int minLen = 0, int maxLen = int.MaxValue)
            => ReadString(stream, version, minLen, maxLen, pool, (data, len) => data.AsSpan(0, len).ToUtf16String());

        /// <summary>
        /// Read UTF-16 (little endian) string
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length in bytes</param>
        /// <param name="maxLen">Maximum length in bytes</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static Task<string> ReadString16Async(
            this Stream stream,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            CancellationToken cancellationToken = default
            )
            => ReadStringAsync(stream, version, minLen, maxLen, pool, (data, len) => data.AsSpan(0, len).ToUtf16String(), cancellationToken);

        /// <summary>
        /// Read UTF-16 (little endian) string
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length in bytes</param>
        /// <param name="maxLen">Maximum length in bytes</param>
        /// <returns>Value</returns>
        public static string? ReadString16Nullable(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, int minLen = 0, int maxLen = int.MaxValue)
            => ReadNullableString(stream, version, minLen, maxLen, pool, (data, len) => data.AsSpan(0, len).ToUtf16String());

        /// <summary>
        /// Read UTF-16 (little endian) string
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length in bytes</param>
        /// <param name="maxLen">Maximum length in bytes</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static Task<string?> ReadString16NullableAsync(
            this Stream stream,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            CancellationToken cancellationToken = default
            )
            => ReadNullableStringAsync(stream, version, minLen, maxLen, pool, (data, len) => data.AsSpan(0, len).ToUtf16String(), cancellationToken);

        /// <summary>
        /// Read UTF-32 (little endian) string
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length in bytes</param>
        /// <param name="maxLen">Maximum length in bytes</param>
        /// <returns>Value</returns>
        public static string ReadString32(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, int minLen = 0, int maxLen = int.MaxValue)
            => ReadString(stream, version, minLen, maxLen, pool, (data, len) => data.AsSpan(0, len).ToUtf32String());

        /// <summary>
        /// Read UTF-32 (little endian) string
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length in bytes</param>
        /// <param name="maxLen">Maximum length in bytes</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static Task<string> ReadString32Async(
            this Stream stream,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            CancellationToken cancellationToken = default
            )
            => ReadStringAsync(stream, version, minLen, maxLen, pool, (data, len) => data.AsSpan(0, len).ToUtf32String(), cancellationToken);

        /// <summary>
        /// Read UTF-32 (little endian) string
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length in bytes</param>
        /// <param name="maxLen">Maximum length in bytes</param>
        /// <returns>Value</returns>
        public static string? ReadString32Nullable(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, int minLen = 0, int maxLen = int.MaxValue)
            => ReadNullableString(stream, version, minLen, maxLen, pool, (data, len) => data.AsSpan(0, len).ToUtf32String());

        /// <summary>
        /// Read UTF-32 (little endian) string
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length in bytes</param>
        /// <param name="maxLen">Maximum length in bytes</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static Task<string?> ReadString32NullableAsync(
            this Stream stream,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            CancellationToken cancellationToken = default
            )
            => ReadNullableStringAsync(stream, version, minLen, maxLen, pool, (data, len) => data.AsSpan(0, len).ToUtf32String(), cancellationToken);

        /// <summary>
        /// Read a string
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="minLen">Minimum length in bytes</param>
        /// <param name="maxLen">Maximum length in bytes</param>
        /// <param name="pool">Array pool</param>
        /// <param name="action">Action to execute for decoding the resulting string</param>
        /// <returns>String</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static string ReadString(Stream stream, int? version, int minLen, int maxLen, ArrayPool<byte>? pool, Func<byte[], int, string> action)
        {
            version ??= StreamSerializer.VERSION;
            pool ??= StreamSerializer.BufferPool;
            int len = ReadNumber<int>(stream, version, pool);
            SerializerHelper.EnsureValidLength(len, minLen, maxLen);
            if (len == 0) return string.Empty;
            byte[] buffer = ReadSerializedData(stream, len, pool);
            try
            {
                return action(buffer, len);
            }
            finally
            {
                pool.Return(buffer);
            }
        }

        /// <summary>
        /// Read a string
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="minLen">Minimum length in bytes</param>
        /// <param name="maxLen">Maximum length in bytes</param>
        /// <param name="pool">Array pool</param>
        /// <param name="action">Action to execute for decoding the resulting string</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>String</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static async Task<string> ReadStringAsync(
            Stream stream,
            int? version,
            int minLen,
            int maxLen,
            ArrayPool<byte>? pool,
            Func<byte[], int, string> action,
            CancellationToken cancellationToken
            )
        {
            version ??= StreamSerializer.VERSION;
            pool ??= StreamSerializer.BufferPool;
            int len = await ReadNumberAsync<int>(stream, version, pool, cancellationToken).DynamicContext();
            SerializerHelper.EnsureValidLength(len, minLen, maxLen);
            if (len == 0) return string.Empty;
            byte[] buffer = await ReadSerializedDataAsync(stream, len, pool, cancellationToken).DynamicContext();
            try
            {
                return action(buffer, len);
            }
            finally
            {
                pool.Return(buffer);
            }
        }

        /// <summary>
        /// Read a string
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="minLen">Minimum length in bytes</param>
        /// <param name="maxLen">Maximum length in bytes</param>
        /// <param name="pool">Array pool</param>
        /// <param name="action">Action to execute for decoding the resulting string</param>
        /// <returns>String</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static string? ReadNullableString(Stream stream, int? version, int minLen, int maxLen, ArrayPool<byte>? pool, Func<byte[], int, string> action)
        {
            pool ??= StreamSerializer.BufferPool;
            int? len;
            switch ((version ??= StreamSerializer.VERSION) & byte.MaxValue)// Serializer version switch
            {
                case 1:
                case 2:
                    {
                        if (!ReadBool(stream, version, pool)) return null;
                        len = ReadNumber<int>(stream, version, pool);
                    }
                    break;
                default:
                    {
                        len = ReadNumberNullable<int>(stream, version, pool);
                        if (len == null) return null;
                    }
                    break;
            }
            SerializerHelper.EnsureValidLength(len.Value, minLen, maxLen);
            if (len == 0) return string.Empty;
            byte[] buffer = ReadSerializedData(stream, len.Value, pool);
            try
            {
                return action(buffer, len.Value);
            }
            finally
            {
                pool.Return(buffer);
            }
        }

        /// <summary>
        /// Read a string
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="minLen">Minimum length in bytes</param>
        /// <param name="maxLen">Maximum length in bytes</param>
        /// <param name="pool">Array pool</param>
        /// <param name="action">Action to execute for decoding the resulting string</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>String</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static async Task<string?> ReadNullableStringAsync(
            Stream stream,
            int? version,
            int minLen,
            int maxLen,
            ArrayPool<byte>? pool,
            Func<byte[], int, string> action,
            CancellationToken cancellationToken
            )
        {
            pool ??= StreamSerializer.BufferPool;
            int? len;
            switch ((version ??= StreamSerializer.VERSION) & byte.MaxValue)// Serializer version switch
            {
                case 1:
                case 2:
                    {
                        if (!await ReadBoolAsync(stream, version, pool, cancellationToken).DynamicContext()) return null;
                        len = await ReadNumberAsync<int>(stream, version, pool, cancellationToken).DynamicContext();
                    }
                    break;
                default:
                    {
                        len = await ReadNumberNullableAsync<int>(stream, version, pool, cancellationToken).DynamicContext();
                        if (len == null) return null;
                    }
                    break;
            }
            SerializerHelper.EnsureValidLength(len.Value, minLen, maxLen);
            if (len == 0) return string.Empty;
            byte[] buffer = await ReadSerializedDataAsync(stream, len.Value, pool, cancellationToken).DynamicContext();
            try
            {
                return action(buffer, len.Value);
            }
            finally
            {
                pool.Return(buffer);
            }
        }
    }
}
