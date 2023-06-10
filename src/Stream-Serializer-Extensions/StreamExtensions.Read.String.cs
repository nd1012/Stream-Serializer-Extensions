using System.Buffers;
using System.Runtime;
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
        [TargetedPatchingOptOut("Tiny method")]
        public static string ReadString(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, int minLen = 0, int maxLen = int.MaxValue)
        {
            (byte[] data, int len) = ReadBytes(stream, version, buffer: null, pool ?? StreamSerializer.BufferPool, minLen, maxLen);
            try
            {
                return data.AsSpan(0, len).ToUtf8String();
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
        /// <param name="minLen">Minimum length in bytes</param>
        /// <param name="maxLen">Maximum length in bytes</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static async Task<string> ReadStringAsync(
            this Stream stream,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            CancellationToken cancellationToken = default
            )
        {
            (byte[] data, int len) = await ReadBytesAsync(stream, version, buffer: null, pool ?? StreamSerializer.BufferPool, minLen, maxLen, cancellationToken).DynamicContext();
            try
            {
                return data.AsSpan(0, len).ToUtf8String();
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
        /// <param name="minLen">Minimum length in bytes</param>
        /// <param name="maxLen">Maximum length in bytes</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static string? ReadStringNullable(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, int minLen = 0, int maxLen = int.MaxValue)
#pragma warning disable IDE0034 // default expression can be simplified
            => ReadBool(stream, version, pool) ? ReadString(stream, version, pool, minLen, maxLen) : default(string?);
#pragma warning restore IDE0034 // default expression can be simplified

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
        [TargetedPatchingOptOut("Tiny method")]
        public static async Task<string?> ReadStringNullableAsync(
            this Stream stream,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            CancellationToken cancellationToken = default
            )
            => await ReadBoolAsync(stream, version, pool, cancellationToken).DynamicContext()
                ? await ReadStringAsync(stream, version, pool, minLen, maxLen, cancellationToken).DynamicContext()
#pragma warning disable IDE0034 // default expression can be simplified
                : default(string?);
#pragma warning restore IDE0034 // default expression can be simplified

        /// <summary>
        /// Read UTF-16 (little endian) string
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length in bytes</param>
        /// <param name="maxLen">Maximum length in bytes</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static string ReadString16(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, int minLen = 0, int maxLen = int.MaxValue)
        {
            (byte[] data, int len) = ReadBytes(stream, version, buffer: null, pool ?? StreamSerializer.BufferPool, minLen, maxLen);
            try
            {
                return data.AsSpan(0, len).ToUtf16String();
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
        /// Read UTF-16 (little endian) string
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length in bytes</param>
        /// <param name="maxLen">Maximum length in bytes</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static async Task<string> ReadString16Async(
            this Stream stream,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            CancellationToken cancellationToken = default
            )
        {
            (byte[] data, int len) = await ReadBytesAsync(stream, version, buffer: null, pool ?? StreamSerializer.BufferPool, minLen, maxLen, cancellationToken).DynamicContext();
            try
            {
                return data.AsSpan(0, len).ToUtf16String();
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
        /// Read UTF-16 (little endian) string
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length in bytes</param>
        /// <param name="maxLen">Maximum length in bytes</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static string? ReadString16Nullable(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, int minLen = 0, int maxLen = int.MaxValue)
#pragma warning disable IDE0034 // default expression can be simplified
            => ReadBool(stream, version, pool) ? ReadString16(stream, version, pool, minLen, maxLen) : default(string?);
#pragma warning restore IDE0034 // default expression can be simplified

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
        [TargetedPatchingOptOut("Tiny method")]
        public static async Task<string?> ReadString16NullableAsync(
            this Stream stream,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            CancellationToken cancellationToken = default
            )
            => await ReadBoolAsync(stream, version, pool, cancellationToken).DynamicContext()
                ? await ReadString16Async(stream, version, pool, minLen, maxLen, cancellationToken).DynamicContext()
#pragma warning disable IDE0034 // default expression can be simplified
                : default(string?);
#pragma warning restore IDE0034 // default expression can be simplified

        /// <summary>
        /// Read UTF-32 (little endian) string
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length in bytes</param>
        /// <param name="maxLen">Maximum length in bytes</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static string ReadString32(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, int minLen = 0, int maxLen = int.MaxValue)
        {
            (byte[] data, int len) = ReadBytes(stream, version, buffer: null, pool ?? StreamSerializer.BufferPool, minLen, maxLen);
            try
            {
                return data.AsSpan(0, len).ToUtf32String();
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
        /// Read UTF-32 (little endian) string
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length in bytes</param>
        /// <param name="maxLen">Maximum length in bytes</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static async Task<string> ReadString32Async(
            this Stream stream,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            CancellationToken cancellationToken = default
            )
        {
            (byte[] data, int len) = await ReadBytesAsync(stream, version, buffer: null, pool ?? StreamSerializer.BufferPool, minLen, maxLen, cancellationToken).DynamicContext();
            try
            {
                return data.AsSpan(0, len).ToUtf32String();
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
        /// Read UTF-32 (little endian) string
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length in bytes</param>
        /// <param name="maxLen">Maximum length in bytes</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static string? ReadString32Nullable(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, int minLen = 0, int maxLen = int.MaxValue)
#pragma warning disable IDE0034 // default expression can be simplified
            => ReadBool(stream, version, pool) ? ReadString32(stream, version, pool, minLen, maxLen) : default(string?);
#pragma warning restore IDE0034 // default expression can be simplified

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
        [TargetedPatchingOptOut("Tiny method")]
        public static async Task<string?> ReadString32NullableAsync(
            this Stream stream,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            CancellationToken cancellationToken = default
            )
            => await ReadBoolAsync(stream, version, pool, cancellationToken).DynamicContext()
                ? await ReadString32Async(stream, version, pool, minLen, maxLen, cancellationToken).DynamicContext()
#pragma warning disable IDE0034 // default expression can be simplified
                : default(string?);
#pragma warning restore IDE0034 // default expression can be simplified
    }
}
