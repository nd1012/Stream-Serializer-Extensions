using System.Buffers;
using System.Runtime;
using System.Runtime.InteropServices;
using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    // Structure
    public static partial class StreamExtensions
    {
        /// <summary>
        /// Read a struct
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="buffer">Buffer</param>
        /// <param name="pool">Buffer pool</param>
        /// <returns>Struct</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static T ReadStruct<T>(
            this Stream stream,
            int? version = null,
            byte[]? buffer = null,
            ArrayPool<byte>? pool = null
            )
            where T : struct
        {
            int len = Marshal.SizeOf(typeof(T));
            byte[] data = stream.ReadBytes(version, buffer, pool, len, len).Value;
            try
            {
                GCHandle gch = GCHandle.Alloc(data, GCHandleType.Pinned);
                try
                {
                    return Marshal.PtrToStructure<T>(gch.AddrOfPinnedObject());
                }
                finally
                {
                    gch.Free();
                }
            }
            finally
            {
                if (buffer == null && pool != null) pool.Return(data);
            }
        }

        /// <summary>
        /// Read a struct
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="buffer">Buffer</param>
        /// <param name="pool">Buffer pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Struct</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static async Task<T> ReadStructAsync<T>(
            this Stream stream,
            int? version = null,
            byte[]? buffer = null,
            ArrayPool<byte>? pool = null,
            CancellationToken cancellationToken = default
            )
            where T : struct
        {
            int len = Marshal.SizeOf(typeof(T));
            byte[] data = (await stream.ReadBytesAsync(version, buffer, pool, len, len, cancellationToken).DynamicContext()).Value;
            try
            {
                GCHandle gch = GCHandle.Alloc(data, GCHandleType.Pinned);
                try
                {
                    return Marshal.PtrToStructure<T>(gch.AddrOfPinnedObject());
                }
                finally
                {
                    gch.Free();
                }
            }
            finally
            {
                if (buffer == null && pool != null) pool.Return(data);
            }
        }

        /// <summary>
        /// Read a struct
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="buffer">Buffer</param>
        /// <param name="pool">Buffer pool</param>
        /// <returns>Struct</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static T? ReadStructNullable<T>(
            this Stream stream,
            int? version = null,
            byte[]? buffer = null,
            ArrayPool<byte>? pool = null
            )
            where T : struct
            => ReadBool(stream, version, pool) ? ReadStruct<T>(stream, version, buffer, pool) : null;

        /// <summary>
        /// Read a struct
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="buffer">Buffer</param>
        /// <param name="pool">Buffer pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Struct</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static async Task<T?> ReadStructNullableAsync<T>(
            this Stream stream,
            int? version = null,
            byte[]? buffer = null,
            ArrayPool<byte>? pool = null,
            CancellationToken cancellationToken = default
            )
            where T : struct
            => await ReadBoolAsync(stream, version, pool, cancellationToken).DynamicContext()
                ? await ReadStructAsync<T>(stream, version, buffer, pool, cancellationToken).DynamicContext()
                : null;
    }
}
