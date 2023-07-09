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
        /// <param name="context">Context</param>
        /// <param name="minLen">Minimum length in bytes</param>
        /// <param name="maxLen">Maximum length in bytes</param>
        /// <returns>Value and length</returns>
        public static (byte[] Value, int Length) ReadBytes(this Stream stream, IDeserializationContext context, int minLen = 0, int maxLen = int.MaxValue)
        {
            byte[] buffer = Array.Empty<byte>();
            try
            {
                int len = ReadNumber<int>(stream, context);
                SerializerHelper.EnsureValidLength(len, minLen, maxLen);
                if (len != 0)
                {
                    if (len != 0) buffer = context.BufferPool.Rent(len);
                    int red = stream.Read(buffer.AsSpan(0, len));
                    if (red != len) throw new SerializerException($"Failed to read serialized data ({len} bytes expected, {red} bytes red)");
                }
                return (buffer, len);
            }
            catch (SerializerException)
            {
                if (buffer.Length != 0) context.BufferPool.Return(buffer!);
                throw;
            }
            catch (Exception ex)
            {
                if (buffer.Length != 0) context.BufferPool.Return(buffer!);
                throw SerializerException.From(ex);
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <param name="minLen">Minimum length in bytes</param>
        /// <param name="maxLen">Maximum length in bytes</param>
        /// <returns>Value and length</returns>
        public static async Task<(byte[] Value, int Length)> ReadBytesAsync(this Stream stream, IDeserializationContext context, int minLen = 0, int maxLen = int.MaxValue)
        {
            byte[] buffer = Array.Empty<byte>();
            try
            {
                int len = await ReadNumberAsync<int>(stream, context).DynamicContext();
                SerializerHelper.EnsureValidLength(len, minLen, maxLen);
                if (len != 0)
                {
                    if (len != 0) buffer = context.BufferPool.Rent(len);
                    int red = await stream.ReadAsync(buffer.AsMemory(0, len), context.Cancellation).DynamicContext();
                    if (red != len) throw new SerializerException($"Failed to read serialized data ({len} bytes expected, {red} bytes red)");
                }
                return (buffer, len);
            }
            catch (SerializerException)
            {
                if (buffer.Length != 0) context.BufferPool.Return(buffer!);
                throw;
            }
            catch (Exception ex)
            {
                if (buffer.Length != 0) context.BufferPool.Return(buffer!);
                throw SerializerException.From(ex);
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <param name="minLen">Minimum length in bytes</param>
        /// <param name="maxLen">Maximum length in bytes</param>
        /// <returns>Value and length</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static (byte[] Value, int Length)? ReadBytesNullable(this Stream stream, IDeserializationContext context, int minLen = 0, int maxLen = int.MaxValue)
        {
            switch (context.SerializerVersion)// Serializer version switch
            {
                case 1:
                case 2:
                    {
                        return ReadBool(stream, context) ? ReadBytes(stream, context, minLen, maxLen) : null;
                    }
                default:
                    {
                        byte[] buffer = Array.Empty<byte>();
                        try
                        {
                            if (ReadNumberNullable<int>(stream, context) is not int len) return null;
                            SerializerHelper.EnsureValidLength(len, minLen, maxLen);
                            if (len != 0)
                            {
                                if (len != 0) buffer = context.BufferPool.Rent(len);
                                int red = stream.Read(buffer.AsSpan(0, len));
                                if (red != len) throw new SerializerException($"Failed to read serialized data ({len} bytes expected, {red} bytes red)");
                            }
                            return (buffer, len);
                        }
                        catch (SerializerException)
                        {
                            if (buffer.Length != 0) context.BufferPool.Return(buffer!);
                            throw;
                        }
                        catch (Exception ex)
                        {
                            if (buffer.Length != 0) context.BufferPool.Return(buffer!);
                            throw SerializerException.From(ex);
                        }
                    }
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <param name="minLen">Minimum length in bytes</param>
        /// <param name="maxLen">Maximum length in bytes</param>
        /// <returns>Value and length</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static async Task<(byte[] Value, int Length)?> ReadBytesNullableAsync(this Stream stream, IDeserializationContext context, int minLen = 0, int maxLen = int.MaxValue)
        {
            switch (context.SerializerVersion)// Serializer version switch
            {
                case 1:
                case 2:
                    {
                        return await ReadBoolAsync(stream, context).DynamicContext()
                            ? await ReadBytesAsync(stream, context, minLen, maxLen).DynamicContext()
                            : null;
                    }
                default:
                    {
                        byte[] buffer = Array.Empty<byte>();
                        try
                        {
                            if (await ReadNumberNullableAsync<int>(stream, context).DynamicContext() is not int len) return null;
                            SerializerHelper.EnsureValidLength(len, minLen, maxLen);
                            if (len != 0)
                            {
                                if (len != 0) buffer = context.BufferPool.Rent(len);
                                int red = await stream.ReadAsync(buffer.AsMemory(0, len), context.Cancellation).DynamicContext();
                                if (red != len) throw new SerializerException($"Failed to read serialized data ({len} bytes expected, {red} bytes red)");
                            }
                            return (buffer, len);
                        }
                        catch (SerializerException)
                        {
                            if (buffer.Length != 0) context.BufferPool.Return(buffer!);
                            throw;
                        }
                        catch (Exception ex)
                        {
                            if (buffer.Length != 0) context.BufferPool.Return(buffer!);
                            throw SerializerException.From(ex);
                        }
                    }
            }
        }
    }
}
