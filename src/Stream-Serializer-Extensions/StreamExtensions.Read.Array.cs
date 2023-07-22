using System.Runtime;
using System.Runtime.CompilerServices;
using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    // Array
    public static partial class StreamExtensions
    {
        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[] ReadArray<T>(this Stream stream, IDeserializationContext context, int minLen = 0, int maxLen = int.MaxValue)
        {
            int len = ReadNumber<int>(stream, context);
            SerializerHelper.EnsureValidLength(len, minLen, maxLen);
            if (len == 0) return Array.Empty<T>();
            T[] res = new T[len];
            return ReadFixedArray(stream, res, context);
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Array type</param>
        /// <param name="context">Context</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Array ReadArray(this Stream stream, Type type, IDeserializationContext context, int minLen = 0, int maxLen = int.MaxValue)
        {
            SerializerException.Wrap(() => ArgumentValidationHelper.EnsureValidArgument(nameof(type), type.IsArray, () => "Not an array type"));
            int len = ReadNumber<int>(stream, context);
            SerializerHelper.EnsureValidLength(len, minLen, maxLen);
            Type elementType = type.GetElementType()!;
            Array res = Array.CreateInstance(elementType, len);
            return ReadFixedArray(stream, res, context);
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<T[]> ReadArrayAsync<T>(this Stream stream, IDeserializationContext context, int minLen = 0, int maxLen = int.MaxValue)
        {
            int len = await ReadNumberAsync<int>(stream, context).DynamicContext();
            SerializerHelper.EnsureValidLength(len, minLen, maxLen);
            if (len == 0) return Array.Empty<T>();
            T[] res = new T[len];
            return await ReadFixedArrayAsync(stream, res, context).DynamicContext();
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Array type</param>
        /// <param name="context">Context</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<Array> ReadArrayAsync(this Stream stream, Type type, IDeserializationContext context, int minLen = 0, int maxLen = int.MaxValue)
        {
            SerializerException.Wrap(() => ArgumentValidationHelper.EnsureValidArgument(nameof(type), type.IsArray, () => "Not an array type"));
            int len = await ReadNumberAsync<int>(stream, context).DynamicContext();
            SerializerHelper.EnsureValidLength(len, minLen, maxLen);
            Type elementType = type.GetElementType()!;
            Array res = Array.CreateInstance(elementType, len);
            return await ReadFixedArrayAsync(stream, res, context).DynamicContext();
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[]? ReadArrayNullable<T>(this Stream stream, IDeserializationContext context, int minLen = 0, int maxLen = int.MaxValue)
        {
            switch (context.SerializerVersion)// Serializer version switch
            {
                case 1:
                case 2:
                    {
                        return ReadBool(stream, context) ? ReadArray<T>(stream, context, minLen, maxLen) : null;
                    }
                default:
                    {
                        if (ReadNumberNullable<int>(stream, context) is not int len) return null;
                        SerializerHelper.EnsureValidLength(len, minLen, maxLen);
                        if (len == 0) return Array.Empty<T>();
                        T[] res = new T[len];
                        ReadFixedArray(stream, res.AsSpan(), context);
                        return res;
                    }
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Array type</param>
        /// <param name="context">Context</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Array? ReadArrayNullable(this Stream stream, Type type, IDeserializationContext context, int minLen = 0, int maxLen = int.MaxValue)
        {
            switch (context.SerializerVersion)// Serializer version switch
            {
                case 1:
                case 2:
                    {
                        return ReadBool(stream, context) ? ReadArray(stream, type, context, minLen, maxLen) : null;
                    }
                default:
                    {
                        if (ReadNumberNullable<int>(stream, context) is not int len) return null;
                        return len < 1
                            ? Array.CreateInstance(type.GetElementType()!, length: 0)
                            : ReadFixedArray(
                                stream,
                                Array.CreateInstance(type.GetElementType()!, SerializerHelper.EnsureValidLength(len, minLen, maxLen)),
                                context
                                );
                    }
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<T[]?> ReadArrayNullableAsync<T>(this Stream stream, IDeserializationContext context, int minLen = 0, int maxLen = int.MaxValue)
        {
            switch (context.SerializerVersion)// Serializer version switch
            {
                case 1:
                case 2:
                    {
                        return await ReadBoolAsync(stream, context).DynamicContext()
                            ? await ReadArrayAsync<T>(stream, context, minLen, maxLen).DynamicContext()
                            : null;
                    }
                default:
                    {
                        if (await ReadNumberNullableAsync<int>(stream, context).DynamicContext() is not int len) return null;
                        SerializerHelper.EnsureValidLength(len, minLen, maxLen);
                        if (len == 0) return Array.Empty<T>();
                        T[] res = new T[len];
                        await ReadFixedArrayAsync(stream, res.AsMemory(), context).DynamicContext();
                        return res;
                    }
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Array type</param>
        /// <param name="context">Context</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<Array?> ReadArrayNullableAsync(this Stream stream, Type type, IDeserializationContext context, int minLen = 0, int maxLen = int.MaxValue)
        {
            switch (context.SerializerVersion)// Serializer version switch
            {
                case 1:
                case 2:
                    {
                        return await ReadBoolAsync(stream, context).DynamicContext()
                            ? await ReadArrayAsync(stream, type, context, minLen, maxLen).DynamicContext()
                            : null;
                    }
                default:
                    {
                        if (await ReadNumberNullableAsync<int>(stream, context).DynamicContext() is not int len) return null;
                        return len < 1
                            ? Array.CreateInstance(type.GetElementType()!, length: 0)
                            : await ReadFixedArrayAsync(
                                stream,
                                Array.CreateInstance(type.GetElementType()!, SerializerHelper.EnsureValidLength(len, minLen, maxLen)),
                                context
                                ).DynamicContext();
                    }
            }
        }
    }
}
