﻿using System.Buffers;
using System.Runtime;
using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    // Bytes
    public static partial class StreamExtensions
    {
        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="buffer">Result buffer to use</param>
        /// <param name="pool">Array pool (if given, and <c>buffer</c> is <see langword="null"/>, the returned value is a pool array which needs to be returned to the pool after use!)</param>
        /// <param name="minLen">Minimum length in bytes</param>
        /// <param name="maxLen">Maximum length in bytes</param>
        /// <returns>Value and length</returns>
        public static (byte[] Value, int Length) ReadBytes(
            this Stream stream,
            int? version = null,
            byte[]? buffer = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue
            )
        {
            bool rented = false;
            try
            {
                int len = ReadNumber<int>(stream, version, pool);
                SerializerHelper.EnsureValidLength(len, minLen, maxLen);
                if (len == 0 && buffer == null) buffer = Array.Empty<byte>();
                rented = buffer == null && pool != null;
                buffer ??= rented ? pool!.Rent(len) : new byte[len];
                if (buffer.Length < len) throw new SerializerException($"Buffer too small ({len} bytes required)", new ArgumentOutOfRangeException(nameof(buffer)));
                if (len != 0)
                {
                    int red = stream.Read(buffer.AsSpan(0, len));
                    if (red != len) throw new SerializerException($"Failed to read serialized data ({len} bytes expected, {red} bytes red)");
                }
                return (buffer, len);
            }
            catch (SerializerException)
            {
                if (rented) pool!.Return(buffer!);
                throw;
            }
            catch (Exception ex)
            {
                if (rented) pool!.Return(buffer!);
                throw SerializerException.From(ex);
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="buffer">Result buffer to use</param>
        /// <param name="pool">Array pool (if given, and <c>buffer</c> is <see langword="null"/>, the returned value is a pool array which needs to be returned to the pool after use!)</param>
        /// <param name="minLen">Minimum length in bytes</param>
        /// <param name="maxLen">Maximum length in bytes</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value and length</returns>
        public static async Task<(byte[] Value, int Length)> ReadBytesAsync(
            this Stream stream,
            int? version = null,
            byte[]? buffer = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            CancellationToken cancellationToken = default
            )
        {
            bool rented = false;
            try
            {
                return await SerializerException.WrapAsync(async () =>
                {
                    int len = await ReadNumberAsync<int>(stream, version, pool, cancellationToken).DynamicContext();
                    SerializerHelper.EnsureValidLength(len, minLen, maxLen);
                    if (len == 0 && buffer == null) buffer = Array.Empty<byte>();
                    rented = buffer == null && pool != null;
                    buffer ??= rented ? pool!.Rent(len) : new byte[len];
                    if (buffer.Length < len) throw new SerializerException($"Buffer too small ({len} bytes required)", new ArgumentOutOfRangeException(nameof(buffer)));
                    if (len != 0)
                    {
                        int red = await stream.ReadAsync(buffer.AsMemory(0, len), cancellationToken).DynamicContext();
                        if (red != len) throw new SerializerException($"Failed to read serialized data ({len} bytes expected, {red} bytes red)");
                    }
                    return (buffer, len);
                }).DynamicContext();
            }
            catch (SerializerException)
            {
                if (rented) pool!.Return(buffer!);
                throw;
            }
            catch (Exception ex)
            {
                if (rented) pool!.Return(buffer!);
                throw SerializerException.From(ex);
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="buffer">Result buffer to use</param>
        /// <param name="pool">Array pool (if given, and <c>buffer</c> is <see langword="null"/>, the returned value is a pool array which needs to be returned to the 
        /// pool after use!)</param>
        /// <param name="minLen">Minimum length in bytes</param>
        /// <param name="maxLen">Maximum length in bytes</param>
        /// <returns>Value and length</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static (byte[] Value, int Length)? ReadBytesNullable(
            this Stream stream,
            int? version = null,
            byte[]? buffer = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue
            )
        {
            switch ((version ??= StreamSerializer.Version) & byte.MaxValue)// Serializer version switch
            {
                case 1:
                case 2:
                    {
                        return ReadBool(stream, version, pool) ? ReadBytes(stream, version, buffer, pool, minLen, maxLen) : null;
                    }
                default:
                    {
                        bool rented = false;
                        try
                        {
                            if (ReadNumberNullable<int>(stream, version, pool) is not int len) return null;
                            SerializerHelper.EnsureValidLength(len, minLen, maxLen);
                            if (len == 0 && buffer == null && pool == null) buffer = Array.Empty<byte>();
                            rented = buffer == null && pool != null;
                            buffer ??= rented ? pool!.Rent(len) : new byte[len];
                            if (buffer.Length < len) throw new SerializerException($"Buffer too small ({len} bytes required)", new ArgumentOutOfRangeException(nameof(buffer)));
                            if (len != 0)
                            {
                                int red = stream.Read(buffer.AsSpan(0, len));
                                if (red != len) throw new SerializerException($"Failed to read serialized data ({len} bytes expected, {red} bytes red)");
                            }
                            return (buffer, len);
                        }
                        catch (SerializerException)
                        {
                            if (rented) pool!.Return(buffer!);
                            throw;
                        }
                        catch (Exception ex)
                        {
                            if (rented) pool!.Return(buffer!);
                            throw SerializerException.From(ex);
                        }
                    }
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="buffer">Result buffer to use</param>
        /// <param name="pool">Array pool (if given, and <c>buffer</c> is <see langword="null"/>, the returned value is a pool array which needs to be returned to the 
        /// pool after use!)</param>
        /// <param name="minLen">Minimum length in bytes</param>
        /// <param name="maxLen">Maximum length in bytes</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value and length</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static async Task<(byte[] Value, int Length)?> ReadBytesNullableAsync(
            this Stream stream,
            int? version = null,
            byte[]? buffer = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            CancellationToken cancellationToken = default
            )
        {
            switch ((version ??= StreamSerializer.Version) & byte.MaxValue)// Serializer version switch
            {
                case 1:
                case 2:
                    {
                        return await ReadBoolAsync(stream, version, pool, cancellationToken).DynamicContext()
                            ? await ReadBytesAsync(stream, version, buffer, pool, minLen, maxLen, cancellationToken).DynamicContext()
                            : null;
                    }
                default:
                    {
                        bool rented = false;
                        try
                        {
                            if (await ReadNumberNullableAsync<int>(stream, version, pool, cancellationToken).DynamicContext() is not int len) return null;
                            SerializerHelper.EnsureValidLength(len, minLen, maxLen);
                            if (len == 0 && buffer == null && pool == null) buffer = Array.Empty<byte>();
                            rented = buffer == null && pool != null;
                            buffer ??= rented ? pool!.Rent(len) : new byte[len];
                            if (buffer.Length < len)
                                throw new SerializerException($"Buffer too small ({len} bytes required)", new ArgumentOutOfRangeException(nameof(buffer)));
                            if (len != 0)
                            {
                                int red = await stream.ReadAsync(buffer.AsMemory(0, len), cancellationToken).DynamicContext();
                                if (red != len) throw new SerializerException($"Failed to read serialized data ({len} bytes expected, {red} bytes red)");
                            }
                            return (buffer, len);
                        }
                        catch (SerializerException)
                        {
                            if (rented) pool!.Return(buffer!);
                            throw;
                        }
                        catch (Exception ex)
                        {
                            if (rented) pool!.Return(buffer!);
                            throw SerializerException.From(ex);
                        }
                    }
            }
        }
    }
}
