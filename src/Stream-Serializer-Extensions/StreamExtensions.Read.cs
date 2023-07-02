﻿using System.Buffers;
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
        /// Require the <see cref="StreamSerializerAttribute"/> attribute when using <see cref="ReadAnyObject{T}(Stream, int?)"/> etc.?
        /// </summary>
        public static bool AnyObjectAttributeRequired { get; set; } = true;

        /// <summary>
        /// Read the serializer version
        /// </summary>
        /// <param name="stream">Steam</param>
        /// <returns>Serializer version</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int ReadSerializerVersion(this Stream stream)
        {
            int res = ReadNumber<int>(stream, version: 1);
            if (res < 1 || (res & byte.MaxValue) > StreamSerializer.VERSION)
                throw new SerializerException($"Invalid or unsupported stream serializer version #{res}", new InvalidDataException());
            return res;
        }

        /// <summary>
        /// Read the serializer version
        /// </summary>
        /// <param name="stream">Steam</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Serializer version</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<int> ReadSerializerVersionAsync(this Stream stream, CancellationToken cancellationToken = default)
        {
            int res = await ReadNumberAsync<int>(stream, version: 1, cancellationToken: cancellationToken).DynamicContext();
            if (res < 1 || (res & byte.MaxValue) > StreamSerializer.VERSION)
                throw new SerializerException($"Invalid or unsupported stream serializer version #{res}", new InvalidDataException());
            return res;
        }

        /// <summary>
        /// Read serialized data
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="len">Length in bytes</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Serialized data (a pool array which needs to be returned to the pool (<see cref="StreamSerializer.BufferPool"/> will be used per default) after use and might 
        /// be larger than the given length!)</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[] ReadSerializedData(this Stream stream, int len, ArrayPool<byte>? pool = null)
        {
            pool ??= StreamSerializer.BufferPool;
            byte[] res = pool.Rent(len);
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
                pool.Return(res, clearArray: false);
                throw;
            }
        }

        /// <summary>
        /// Read serialized data
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="len">Length in bytes</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Serialized data (a pool array which needs to be returned to the pool (<see cref="StreamSerializer.BufferPool"/> will be used per default) after use and might 
        /// be larger than the given length!)</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<byte[]> ReadSerializedDataAsync(this Stream stream, int len, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
        {
            pool ??= StreamSerializer.BufferPool;
            byte[] res = pool.Rent(len);
            try
            {
                return await SerializerException.WrapAsync(async () =>
                {
                    int red = await stream.ReadAsync(res.AsMemory(0, len), cancellationToken).DynamicContext();
                    if (red != len) throw new IOException($"Failed to read serialized data ({len} bytes expected, {red} bytes red)");
                    return res;
                }).DynamicContext();
            }
            catch
            {
                pool.Return(res, clearArray: false);
                throw;
            }
        }
    }
}
