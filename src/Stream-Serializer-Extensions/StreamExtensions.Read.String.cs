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
        /// <param name="context">Context</param>
        /// <param name="minLen">Minimum length in bytes</param>
        /// <param name="maxLen">Maximum length in bytes</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static string ReadString(this Stream stream, IDeserializationContext context, int minLen = 0, int maxLen = int.MaxValue)
#pragma warning restore IDE0060 // Remove unused argument
            => ReadString(context, minLen, maxLen, (data) => data.ToUtf8String());

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <param name="minLen">Minimum length in bytes</param>
        /// <param name="maxLen">Maximum length in bytes</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static Task<string> ReadStringAsync(this Stream stream, IDeserializationContext context, int minLen = 0, int maxLen = int.MaxValue)
#pragma warning restore IDE0060 // Remove unused argument
            => ReadStringAsync(context, minLen, maxLen, (data) => data.ToUtf8String());

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <param name="minLen">Minimum length in bytes</param>
        /// <param name="maxLen">Maximum length in bytes</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static string? ReadStringNullable(this Stream stream, IDeserializationContext context, int minLen = 0, int maxLen = int.MaxValue)
#pragma warning restore IDE0060 // Remove unused argument
            => ReadNullableString(context, minLen, maxLen, (data) => data.ToUtf8String());

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <param name="minLen">Minimum length in bytes</param>
        /// <param name="maxLen">Maximum length in bytes</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static Task<string?> ReadStringNullableAsync(this Stream stream, IDeserializationContext context, int minLen = 0, int maxLen = int.MaxValue)
#pragma warning restore IDE0060 // Remove unused argument
            => ReadNullableStringAsync(context, minLen, maxLen, (data) => data.ToUtf8String());

        /// <summary>
        /// Read UTF-16 (little endian) string
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <param name="minLen">Minimum length in bytes</param>
        /// <param name="maxLen">Maximum length in bytes</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static string ReadString16(this Stream stream, IDeserializationContext context, int minLen = 0, int maxLen = int.MaxValue)
#pragma warning restore IDE0060 // Remove unused argument
            => ReadString(context, minLen, maxLen, (data) => data.ToUtf16String());

        /// <summary>
        /// Read UTF-16 (little endian) string
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <param name="minLen">Minimum length in bytes</param>
        /// <param name="maxLen">Maximum length in bytes</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static Task<string> ReadString16Async(this Stream stream, IDeserializationContext context, int minLen = 0, int maxLen = int.MaxValue)
#pragma warning restore IDE0060 // Remove unused argument
            => ReadStringAsync(context, minLen, maxLen, (data) => data.ToUtf16String());

        /// <summary>
        /// Read UTF-16 (little endian) string
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <param name="minLen">Minimum length in bytes</param>
        /// <param name="maxLen">Maximum length in bytes</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static string? ReadString16Nullable(this Stream stream, IDeserializationContext context, int minLen = 0, int maxLen = int.MaxValue)
#pragma warning restore IDE0060 // Remove unused argument
            => ReadNullableString(context, minLen, maxLen, (data) => data.ToUtf16String());

        /// <summary>
        /// Read UTF-16 (little endian) string
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <param name="minLen">Minimum length in bytes</param>
        /// <param name="maxLen">Maximum length in bytes</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static Task<string?> ReadString16NullableAsync(this Stream stream, IDeserializationContext context, int minLen = 0, int maxLen = int.MaxValue)
#pragma warning restore IDE0060 // Remove unused argument
            => ReadNullableStringAsync(context, minLen, maxLen, (data) => data.ToUtf16String());

        /// <summary>
        /// Read UTF-32 (little endian) string
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <param name="minLen">Minimum length in bytes</param>
        /// <param name="maxLen">Maximum length in bytes</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static string ReadString32(this Stream stream, IDeserializationContext context, int minLen = 0, int maxLen = int.MaxValue)
#pragma warning restore IDE0060 // Remove unused argument
            => ReadString(context, minLen, maxLen, (data) => data.ToUtf32String());

        /// <summary>
        /// Read UTF-32 (little endian) string
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <param name="minLen">Minimum length in bytes</param>
        /// <param name="maxLen">Maximum length in bytes</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static Task<string> ReadString32Async(this Stream stream, IDeserializationContext context, int minLen = 0, int maxLen = int.MaxValue)
#pragma warning restore IDE0060 // Remove unused argument
            => ReadStringAsync(context, minLen, maxLen, (data) => data.ToUtf32String());

        /// <summary>
        /// Read UTF-32 (little endian) string
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <param name="minLen">Minimum length in bytes</param>
        /// <param name="maxLen">Maximum length in bytes</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static string? ReadString32Nullable(this Stream stream, IDeserializationContext context, int minLen = 0, int maxLen = int.MaxValue)
#pragma warning restore IDE0060 // Remove unused argument
            => ReadNullableString(context, minLen, maxLen, (data) => data.ToUtf32String());

        /// <summary>
        /// Read UTF-32 (little endian) string
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <param name="minLen">Minimum length in bytes</param>
        /// <param name="maxLen">Maximum length in bytes</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static Task<string?> ReadString32NullableAsync(this Stream stream, IDeserializationContext context, int minLen = 0, int maxLen = int.MaxValue)
#pragma warning restore IDE0060 // Remove unused argument
            => ReadNullableStringAsync(context, minLen, maxLen, (data) => data.ToUtf32String());

        /// <summary>
        /// Read a string
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="minLen">Minimum length in bytes</param>
        /// <param name="maxLen">Maximum length in bytes</param>
        /// <param name="action">Action to execute for decoding the resulting string</param>
        /// <returns>String</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [SkipLocalsInit]
        private static string ReadString(IDeserializationContext context, int minLen, int maxLen, StringReader_Delegate action)
        {
            if (context.TryReadCachedObjectCountable(out string? res, out long l))
                //FIXME How to validate the correct length in this case?
                return res ?? throw new SerializerException("Non-nullable string expected, but NULL deserialized", new InvalidDataException());
            int len = (int)l;
            SerializerHelper.EnsureValidLength(len, minLen, maxLen);
            if (len == 0)
            {
                if (context.SerializerVersion > 2) throw new SerializerException($"Deserialized empty string", new InvalidDataException());
                return string.Empty;// Compatibility with serializer version 2
            }
            if (len <= Settings.StackAllocBorder)
            {
                Span<byte> buffer = stackalloc byte[len];
                if (context.Stream.Read(buffer) != len) throw new SerializerException($"Failed to read {len} bytes serialized data", new IOException());
                return action(buffer);
            }
            else
            {
                byte[] buffer = ReadSerializedData(context.Stream, len, context);
                try
                {
                    return action(buffer.AsSpan(0, len));
                }
                finally
                {
                    context.BufferPool.Return(buffer);
                }
            }
        }

        /// <summary>
        /// Read a string
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="minLen">Minimum length in bytes</param>
        /// <param name="maxLen">Maximum length in bytes</param>
        /// <param name="action">Action to execute for decoding the resulting string</param>
        /// <returns>String</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static async Task<string> ReadStringAsync(IDeserializationContext context, int minLen, int maxLen, StringReader_Delegate action)
        {
            (bool cached, string? res, long l) = await context.TryReadCachedObjectCountableAsync<string>().DynamicContext();
            if (cached) return res ?? throw new SerializerException("Non-nullable string expected, but NULL deserialized", new InvalidDataException());
            int len = (int)l;
            SerializerHelper.EnsureValidLength(len, minLen, maxLen);
            if (len == 0)
            {
                if (context.SerializerVersion > 2) throw new SerializerException($"Deserialized empty string", new InvalidDataException());
                return string.Empty;// Compatibility with serializer version 2
            }
            byte[] buffer = await ReadSerializedDataAsync(context.Stream, len, context).DynamicContext();
            try
            {
                return action(buffer.AsSpan(0, len));
            }
            finally
            {
                context.BufferPool.Return(buffer);
            }
        }

        /// <summary>
        /// Read a string
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="minLen">Minimum length in bytes</param>
        /// <param name="maxLen">Maximum length in bytes</param>
        /// <param name="action">Action to execute for decoding the resulting string</param>
        /// <returns>String</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [SkipLocalsInit]
        private static string? ReadNullableString(IDeserializationContext context, int minLen, int maxLen, StringReader_Delegate action)
        {
            int len;
            switch (context.SerializerVersion)// Serializer version switch
            {
                case 1:
                case 2:
                    {
                        if (!ReadBool(context.Stream, context)) return null;
                        len = ReadNumber<int>(context.Stream, context);
                    }
                    break;
                default:
                    {
                        if (context.TryReadCachedObjectCountable(out string? res, out long l)) return res;
                        len = (int)l;
                    }
                    break;
            }
            SerializerHelper.EnsureValidLength(len, minLen, maxLen);
            if (len == 0)
            {
                if (context.SerializerVersion > 2) throw new SerializerException($"Deserialized empty string", new InvalidDataException());
                return string.Empty;// Compatibility with serializer version 2
            }
            if (len <= Settings.StackAllocBorder)
            {
                Span<byte> buffer = stackalloc byte[len];
                if (context.Stream.Read(buffer) != len) throw new SerializerException($"Failed to read {len} bytes serialized data", new IOException());
                return action(buffer);
            }
            else
            {
                byte[] buffer = ReadSerializedData(context.Stream, len, context);
                try
                {
                    return action(buffer.AsSpan(0, len));
                }
                finally
                {
                    context.BufferPool.Return(buffer);
                }
            }
        }

        /// <summary>
        /// Read a string
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="minLen">Minimum length in bytes</param>
        /// <param name="maxLen">Maximum length in bytes</param>
        /// <param name="action">Action to execute for decoding the resulting string</param>
        /// <returns>String</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static async Task<string?> ReadNullableStringAsync(IDeserializationContext context, int minLen, int maxLen, StringReader_Delegate action)
        {
            int len;
            switch (context.SerializerVersion)// Serializer version switch
            {
                case 1:
                case 2:
                    {
                        if (!await ReadBoolAsync(context.Stream, context).DynamicContext()) return null;
                        len = await ReadNumberAsync<int>(context.Stream, context).DynamicContext();
                    }
                    break;
                default:
                    {
                        (bool cached, string? res, long l) = await context.TryReadCachedObjectCountableAsync<string>().DynamicContext();
                        if (cached) return res;
                        len = (int)l;
                    }
                    break;
            }
            SerializerHelper.EnsureValidLength(len, minLen, maxLen);
            if (len == 0)
            {
                if (context.SerializerVersion > 2) throw new SerializerException($"Deserialized empty string", new InvalidDataException());
                return string.Empty;// Compatibility with serializer version 2
            }
            byte[] buffer = await ReadSerializedDataAsync(context.Stream, len, context).DynamicContext();
            try
            {
                return action(buffer.AsSpan(0, len));
            }
            finally
            {
                context.BufferPool.Return(buffer);
            }
        }

        /// <summary>
        /// Delegate for a string reader
        /// </summary>
        /// <param name="buffer">Buffer</param>
        /// <returns>String</returns>
        private delegate string StringReader_Delegate(Span<byte> buffer);
    }
}
