using System.Buffers;
using System.Runtime;
using System.Runtime.CompilerServices;
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
        /// <param name="context">Contxt</param>
        /// <returns>Struct</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T ReadStruct<T>(this Stream stream, IDeserializationContext context)
            where T : struct
            => SerializerException.Wrap(() =>
            {
                int len = Marshal.SizeOf(typeof(T));
                byte[] data = context.BufferPool.Rent(len);
                stream.ReadBytes(context, len, len);
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
                    context.BufferPool.Return(data);
                }
            });

        /// <summary>
        /// Read a struct
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Struct</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<T> ReadStructAsync<T>(this Stream stream, IDeserializationContext context) where T : struct
            => SerializerException.WrapAsync(async () =>
            {
                int len = Marshal.SizeOf(typeof(T));
                byte[] data = context.BufferPool.Rent(len);
                await stream.ReadBytesAsync(context, len, len).DynamicContext();
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
                    context.BufferPool.Return(data);
                }
            });

        /// <summary>
        /// Read a struct
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Struct</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T? ReadStructNullable<T>(this Stream stream, IDeserializationContext context) where T : struct
            => ReadBool(stream, context) ? ReadStruct<T>(stream, context) : null;

        /// <summary>
        /// Read a struct
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Struct</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<T?> ReadStructNullableAsync<T>(this Stream stream, IDeserializationContext context) where T : struct
            => await ReadBoolAsync(stream, context).DynamicContext() ? await ReadStructAsync<T>(stream, context).DynamicContext() : null;
    }
}
