using System.Reflection;
using System.Runtime;
using System.Runtime.CompilerServices;
using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    // Read
    public static partial class StreamExtensions
    {
        /// <summary>
        /// Array empty method
        /// </summary>
        public static readonly MethodInfo ArrayEmptyMethod;
        /// <summary>
        /// Read struct method
        /// </summary>
        public static readonly MethodInfo ReadStructMethod;
        /// <summary>
        /// Read struct method
        /// </summary>
        public static readonly MethodInfo ReadStructAsyncMethod;
        /// <summary>
        /// Read nullable struct method
        /// </summary>
        public static readonly MethodInfo ReadStructNullableMethod;
        /// <summary>
        /// Read nullable struct method
        /// </summary>
        public static readonly MethodInfo ReadStructNullableAsyncMethod;

        /// <summary>
        /// Require the <see cref="StreamSerializerAttribute"/> attribute when using <see cref="ReadAnyObject(Stream, Type, IDeserializationContext)"/> etc.?
        /// </summary>
        public static bool AnyObjectAttributeRequired { get; set; } = true;

        /// <summary>
        /// Read the serializer version
        /// </summary>
        /// <param name="stream">Steam</param>
        /// <param name="context">Context</param>
        /// <returns>Serializer version</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int ReadSerializerVersion(this Stream stream, IDeserializationContext context)
        {
            int res = ReadNumber<int>(stream, context);
            if (res < 1 || (res & byte.MaxValue) > StreamSerializer.VERSION)
                throw new SerializerException($"Invalid or unsupported stream serializer version #{res}", new InvalidDataException());
            return res;
        }

        /// <summary>
        /// Read the serializer version
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Serializer version</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<int> ReadSerializerVersionAsync(this Stream stream, IDeserializationContext context)
        {
            int res = await ReadNumberAsync<int>(stream, context).DynamicContext();
            if (res < 1 || (res & byte.MaxValue) > StreamSerializer.VERSION)
                throw new SerializerException($"Invalid or unsupported stream serializer version #{res}", new InvalidDataException());
            return res;
        }

        /// <summary>
        /// Read serialized data
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="len">Length in bytes</param>
        /// <param name="context">Context</param>
        /// <returns>Serialized data (a pool array which needs to be returned to the context buffer pool after use and might be larger than the given length!)</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[] ReadSerializedData(this Stream stream, int len, IDeserializationContext context)
        {
            byte[] res = context.BufferPool.Rent(len);
            try
            {
                return SerializerException.Wrap(() =>
                {
                    int red = stream.Read(res.AsSpan(0, len));
                    if (red != len) throw new IOException($"Failed to read serialized data ({len} bytes expected, {red} bytes red)");
                    return res;
                });
            }
            catch
            {
                context.BufferPool.Return(res, clearArray: false);
                throw;
            }
        }

        /// <summary>
        /// Read serialized data
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="len">Length in bytes</param>
        /// <param name="context">Context</param>
        /// <returns>Serialized data (a pool array which needs to be returned to the context buffer pool after use and might be larger than the given length!)</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<byte[]> ReadSerializedDataAsync(this Stream stream, int len, IDeserializationContext context)
        {
            byte[] res = context.BufferPool.Rent(len);
            try
            {
                return await SerializerException.WrapAsync(async () =>
                {
                    int red = await stream.ReadAsync(res.AsMemory(0, len), context.Cancellation).DynamicContext();
                    if (red != len) throw new IOException($"Failed to read serialized data ({len} bytes expected, {red} bytes red)");
                    return res;
                }).DynamicContext();
            }
            catch
            {
                context.BufferPool.Return(res, clearArray: false);
                throw;
            }
        }
    }
}
