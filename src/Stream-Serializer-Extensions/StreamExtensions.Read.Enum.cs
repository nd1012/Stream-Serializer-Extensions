using System.Runtime;
using System.Runtime.CompilerServices;
using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    // Enumeration
    public static partial class StreamExtensions
    {
        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Enumeration type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
#pragma warning disable IDE0060 // Remove unused argument
        public static T ReadEnum<T>(this Stream stream, IDeserializationContext context) where T : struct, Enum
#pragma warning restore IDE0060 // Remove unused argument
            => (T)ReadEnumInt(context, typeof(T), numberType: null);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Enumeration type</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
#pragma warning disable IDE0060 // Remove unused argument
        public static Enum ReadEnum(this Stream stream, Type type, IDeserializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            => ReadEnumInt(context, type, numberType: null);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="type">Enumeration type</param>
        /// <param name="numberType">Number type</param>
        /// <returns>Value</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static Enum ReadEnumInt(IDeserializationContext context, Type type, NumberTypes? numberType)
            => SerializerException.Wrap(() =>
            {
                IEnumInfo info = type.GetEnumInfo();
                numberType ??= (NumberTypes)ReadOneByte(context.Stream, context);
                if (numberType == NumberTypes.Default) return info.DefaultValue;
                Enum res = (Enum)Enum.ToObject(type, ReadNumberInt(context, type.GetEnumUnderlyingType()!, numberType));
                if (!info.IsValidValue(res)) throw new SerializerException($"Unknown enumeration value {res} for {type}");
                return res;
            });

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Enumeration type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
#pragma warning disable IDE0060 // Remove unused argument
        public static async Task<T> ReadEnumAsync<T>(this Stream stream, IDeserializationContext context) where T : struct, Enum
#pragma warning restore IDE0060 // Remove unused argument
            => (T)await ReadEnumIntAsync(context, typeof(T), numberType: null).DynamicContext();

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Enumeration type</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
#pragma warning disable IDE0060 // Remove unused argument
        public static Task<Enum> ReadEnumAsync(this Stream stream, Type type, IDeserializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            => ReadEnumIntAsync(context, type, numberType: null);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="type">Enumeration type</param>
        /// <param name="numberType">Number type</param>
        /// <returns>Value</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static Task<Enum> ReadEnumIntAsync(IDeserializationContext context, Type type, NumberTypes? numberType)
            => SerializerException.WrapAsync(async () =>
            {
                IEnumInfo info = type.GetEnumInfo();
                numberType ??= (NumberTypes)await ReadOneByteAsync(context.Stream, context).DynamicContext();
                if (numberType == NumberTypes.Default) return info.DefaultValue;
                Enum res = (Enum)Enum.ToObject(type, await ReadNumberIntAsync(context, type.GetEnumUnderlyingType(), numberType).DynamicContext());
                if (!info.IsValidValue(res)) throw new SerializerException($"Unknown enumeration value {res} for {type}");
                return res;
            });

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Enumeration type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T? ReadEnumNullable<T>(this Stream stream, IDeserializationContext context) where T : struct, Enum
        {
            switch (context.SerializerVersion)// Serializer version switch
            {
                case 1:
                    {
                        return ReadBool(stream, context) ? ReadEnum<T>(stream, context) : null;
                    }
                default:
                    {
                        NumberTypes numberType = (NumberTypes)ReadOneByte(stream, context);
                        return numberType == NumberTypes.IsNull ? null : (T?)ReadEnumInt(context, typeof(T), numberType);
                    }
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Enumeration type</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Enum? ReadEnumNullable(this Stream stream, Type type, IDeserializationContext context)
        {
            switch (context.SerializerVersion)// Serializer version switch
            {
                case 1:
                    {
                        return ReadBool(stream, context)
                            ? ReadEnum(stream, type, context)
                            : null;
                    }
                default:
                    {
                        NumberTypes numberType = (NumberTypes)ReadOneByte(stream, context);
                        return numberType == NumberTypes.IsNull
                            ? null
                            : ReadEnumInt(context, type, numberType);
                    }
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Enumeration type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<T?> ReadEnumNullableAsync<T>(this Stream stream, IDeserializationContext context)
            where T : struct, Enum
        {
            switch (context.SerializerVersion)// Serializer version switch
            {
                case 1:
                    {
                        return await ReadBoolAsync(stream, context).DynamicContext()
                            ? await ReadEnumAsync<T>(stream, context).DynamicContext()
                            : null;
                    }
                default:
                    {
                        NumberTypes numberType = (NumberTypes)await ReadOneByteAsync(stream, context).DynamicContext();
                        return numberType == NumberTypes.IsNull
                            ? null
                            : (T)await ReadEnumIntAsync(context, typeof(T), numberType).DynamicContext();
                    }
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Enumeration type</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<Enum?> ReadEnumNullableAsync(this Stream stream, Type type, IDeserializationContext context)
        {
            switch (context.SerializerVersion)// Serializer version switch
            {
                case 1:
                    {
                        return await ReadBoolAsync(stream, context).DynamicContext()
                            ? await ReadEnumAsync(stream, type, context).DynamicContext()
                            : null;
                    }
                default:
                    {
                        NumberTypes numberType = (NumberTypes)await ReadOneByteAsync(stream, context).DynamicContext();
                        return numberType == NumberTypes.IsNull
                            ? null
                            : await ReadEnumIntAsync(context, type, numberType).DynamicContext();
                    }
            }
        }
    }
}
