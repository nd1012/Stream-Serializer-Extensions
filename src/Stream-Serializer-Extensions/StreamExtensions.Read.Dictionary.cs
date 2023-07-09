using System.Collections;
using System.Runtime;
using System.Runtime.CompilerServices;
using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    // Dictionary
    public static partial class StreamExtensions
    {
        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="tKey">Key type</typeparam>
        /// <typeparam name="tValue">Value type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Dictionary<tKey, tValue> ReadDict<tKey, tValue>(this Stream stream, IDeserializationContext context, int minLen = 0, int maxLen = int.MaxValue)
            where tKey : notnull
        {
            int len = ReadNumber<int>(stream, context);
            SerializerHelper.EnsureValidLength(len, minLen, maxLen);
            Dictionary<tKey, tValue> res = new(len);
            ReadDictInt(context, res, typeof(tKey), typeof(tValue), len);
            return res;
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Dictionary type</param>
        /// <param name="context">Context</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static IDictionary ReadDict(this Stream stream, Type type, IDeserializationContext context, int minLen = 0, int maxLen = int.MaxValue)
        {
            SerializerException.Wrap(() => ArgumentValidationHelper.EnsureValidArgument(
                nameof(type),
                type.IsGenericType || type.IsGenericTypeDefinition || !typeof(Dictionary<,>).IsAssignableFrom(type.GetGenericTypeDefinition()),
                () => "Not a dictionary type"
                ));
            int len = ReadNumber<int>(stream, context);
            SerializerHelper.EnsureValidLength(len, minLen, maxLen);
            Type[] types = type.GetGenericArguments();
            IDictionary res = (IDictionary)(Activator.CreateInstance(type, len) ?? throw new SerializerException($"Failed to instance {type}"));
            ReadDictInt(context, res, type.GenericTypeArguments[0], type.GenericTypeArguments[1], len);
            return res;
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="tKey">Key type</typeparam>
        /// <typeparam name="tValue">Value type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<Dictionary<tKey, tValue>> ReadDictAsync<tKey, tValue>(this Stream stream, IDeserializationContext context, int minLen = 0, int maxLen = int.MaxValue)
            where tKey : notnull
        {
            int len = await ReadNumberAsync<int>(stream, context).DynamicContext();
            SerializerHelper.EnsureValidLength(len, minLen, maxLen);
            Dictionary<tKey, tValue> res = new(len);
            await ReadDictIntAsync(context, res, typeof(tKey), typeof(tValue), len).DynamicContext();
            return res;
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Dictionary type</param>
        /// <param name="context">Context</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<IDictionary> ReadDictAsync(this Stream stream, Type type, IDeserializationContext context, int minLen = 0, int maxLen = int.MaxValue)
        {
            SerializerException.Wrap(() => ArgumentValidationHelper.EnsureValidArgument(
                nameof(type),
                type.IsGenericType || type.IsGenericTypeDefinition || !typeof(Dictionary<,>).IsAssignableFrom(type.GetGenericTypeDefinition()),
                () => "Not a dictionary type"
                ));
            int len = await ReadNumberAsync<int>(stream, context).DynamicContext();
            SerializerHelper.EnsureValidLength(len, minLen, maxLen);
            Type[] types = type.GetGenericArguments();
            IDictionary res = (IDictionary)(Activator.CreateInstance(type, len) ?? throw new SerializerException($"Failed to instance {type}"));
            await ReadDictIntAsync(context, res, type.GenericTypeArguments[0], type.GenericTypeArguments[1], len).DynamicContext();
            return res;
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="tKey">Key type</typeparam>
        /// <typeparam name="tValue">Value type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Dictionary<tKey, tValue>? ReadDictNullable<tKey, tValue>(this Stream stream, IDeserializationContext context, int minLen = 0, int maxLen = int.MaxValue)
            where tKey : notnull
        {
            switch (context.SerializerVersion)// Serializer version switch
            {
                case 1:
                case 2:
                    return ReadBool(stream, context) ? ReadDict<tKey, tValue>(stream, context, minLen, maxLen) : null;
                default:
                    {
                        if (ReadNumberNullable<int>(stream, context) is not int len) return null;
                        SerializerHelper.EnsureValidLength(len, minLen, maxLen);
                        Dictionary<tKey, tValue> res = new(len);
                        ReadDictInt(context, res, typeof(tKey), typeof(tValue), len);
                        return res;
                    }
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Dictionary type</param>
        /// <param name="context">Context</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static IDictionary? ReadDictNullable(this Stream stream, Type type, IDeserializationContext context, int minLen = 0, int maxLen = int.MaxValue)
        {
            SerializerException.Wrap(() => ArgumentValidationHelper.EnsureValidArgument(
                nameof(type),
                type.IsGenericType || type.IsGenericTypeDefinition || !typeof(Dictionary<,>).IsAssignableFrom(type.GetGenericTypeDefinition()),
                () => "Not a dictionary type"
                ));
            switch (context.SerializerVersion)// Serializer version switch
            {
                case 1:
                case 2:
                    {
                        return ReadBool(stream, context) ? ReadDict(stream, type, context, minLen, maxLen) : null;
                    }
                default:
                    {
                        if (ReadNumberNullable<int>(stream, context) is not int len) return null;
                        SerializerHelper.EnsureValidLength(len, minLen, maxLen);
                        Type[] types = type.GetGenericArguments();
                        IDictionary res = (IDictionary)(Activator.CreateInstance(type, len) ?? throw new SerializerException($"Failed to instance {type}"));
                        ReadDictInt(context, res, type.GenericTypeArguments[0], type.GenericTypeArguments[1], len);
                        return res;
                    }
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="tKey">Key type</typeparam>
        /// <typeparam name="tValue">Value type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<Dictionary<tKey, tValue>?> ReadDictNullableAsync<tKey, tValue>(this Stream stream, IDeserializationContext context, int minLen = 0, int maxLen = int.MaxValue)
            where tKey : notnull
        {
            switch (context.SerializerVersion)// Serializer version switch
            {
                case 1:
                case 2:
                    {
                        return await ReadBoolAsync(stream, context).DynamicContext()
                            ? await ReadDictAsync<tKey, tValue>(stream, context, minLen, maxLen).DynamicContext()
                            : null;
                    }
                default:
                    {
                        if (await ReadNumberNullableAsync<int>(stream, context).DynamicContext() is not int len) return null;
                        SerializerHelper.EnsureValidLength(len, minLen, maxLen);
                        Dictionary<tKey, tValue> res = new(len);
                        await ReadDictIntAsync(context, res, typeof(tKey), typeof(tValue), len).DynamicContext();
                        return res;
                    }
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Dictionary type</param>
        /// <param name="context">Context</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<IDictionary?> ReadDictNullableAsync(this Stream stream, Type type, IDeserializationContext context, int minLen = 0, int maxLen = int.MaxValue)
        {
            SerializerException.Wrap(() => ArgumentValidationHelper.EnsureValidArgument(
                nameof(type),
                type.IsGenericType || type.IsGenericTypeDefinition || !typeof(Dictionary<,>).IsAssignableFrom(type.GetGenericTypeDefinition()),
                () => "Not a dictionary type"
                ));
            switch (context.SerializerVersion)// Serializer version switch
            {
                case 1:
                case 2:
                    {
                        return await ReadBoolAsync(stream, context).DynamicContext()
                            ? await ReadDictAsync(stream, type, context, minLen, maxLen).DynamicContext()
                            : null;
                    }
                default:
                    {
                        if (await ReadNumberNullableAsync<int>(stream, context).DynamicContext() is not int len) return null;
                        SerializerHelper.EnsureValidLength(len, minLen, maxLen);
                        Type[] types = type.GetGenericArguments();
                        IDictionary res = (IDictionary)(Activator.CreateInstance(type, len) ?? throw new SerializerException($"Failed to instance {type}"));
                        await ReadDictIntAsync(context, res, type.GenericTypeArguments[0], type.GenericTypeArguments[1], len).DynamicContext();
                        return res;
                    }
            }
        }

        /// <summary>
        /// Read dictionary items
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="dict">Dictionary</param>
        /// <param name="keyType">Key type</param>
        /// <param name="valueType">Value type</param>
        /// <param name="count">Number of items</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ReadDictInt(IDeserializationContext context, IDictionary dict, Type keyType, Type valueType, int count)
        {
            using ContextRecursion cr = new(context);
            switch (context.SerializerVersion)// Serializer version switch
            {
                case 1:
                case 2:
                    {
                        for (int i = 0; i < count; i++) dict[ReadObject(context.Stream, keyType, context)] = ReadObject(context.Stream, valueType, context);
                    }
                    break;
                default:
                    try
                    {
                        ISerializerOptions? options = context.Options,
                            keyOptions = options?.KeyOptions,
                            valueOptions = options?.ValueOptions;
                        bool hasOptions = keyOptions != null || valueOptions != null;
                        using ItemDeserializerContext keyContext = new(context, objectCache: false);
                        using ItemDeserializerContext valueContext = new(context)
                        {
                            Nullable = valueOptions?.IsNullable ?? context.Nullable
                        };
                        object key;
                        object? value;
                        (keyContext.ItemSerializer, keyContext.ItemSyncDeserializer, _) = keyType.GetItemDeserializerInfo(ObjectTypes.Null, isAsync: false);
                        (valueContext.ItemSerializer, valueContext.ItemSyncDeserializer, _) = valueType.GetItemDeserializerInfo(ObjectTypes.Null, isAsync: false);
                        object? obj;
                        for (int i = 0; i < count; i++)
                        {
                            if (hasOptions) context.WithOptions(keyOptions);
                            if (keyContext.ItemSerializer == SerializerTypes.Any)
                            {
                                ReadAnyItemHeader(keyContext, i, keyType);
                                key = (keyContext.ItemSerializer == SerializerTypes.Serializer
                                    ? ReadItem(keyContext)
                                    : ReadAnyInt(context, keyContext.ObjectType, keyContext.ItemType))!;
                            }
                            else
                            {
                                key = ReadItem(keyContext)!;
                            }
                            if (hasOptions) context.WithOptions(valueOptions);
                            if (valueContext.ItemSerializer == SerializerTypes.Any)
                            {
                                obj = ReadAnyItemHeader(valueContext, i, valueType);
                                if (obj == null && valueContext.ObjectType == ObjectTypes.Null)
                                {
                                    if (!context.Nullable) throw new SerializerException($"Deserialized NULL value #{i}", new InvalidDataException());
                                    value = null;
                                }
                                if (obj == null)
                                {
                                    value = (valueContext.ItemSerializer == SerializerTypes.Serializer
                                        ? ReadItem(valueContext)
                                        : ReadAnyInt(context, valueContext.ObjectType, valueContext.ItemType))!;
                                    if (valueContext.ObjectType.RequiresObjectWriting()) valueContext.AddObject(value);
                                }
                                else
                                {
                                    value = obj;
                                }
                            }
                            else
                            {
                                value = ReadItem(valueContext)!;
                            }
                            dict[key] = value;
                        }
                        break;
                    }
                    finally
                    {
                        context.WithoutOptions();
                    }
            }
        }

        /// <summary>
        /// Read dictionary items
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="dict">Dictionary</param>
        /// <param name="keyType">Key type</param>
        /// <param name="valueType">Value type</param>
        /// <param name="count">Number of items</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static async Task ReadDictIntAsync(IDeserializationContext context, IDictionary dict, Type keyType, Type valueType, int count)
        {
            using ContextRecursion cr = new(context);
            switch (context.SerializerVersion)// Serializer version switch
            {
                case 1:
                case 2:
                    {
                        for (int i = 0; i < count; i++)
                            dict[await ReadObjectAsync(context.Stream, keyType, context).DynamicContext()] =
                                await ReadObjectAsync(context.Stream, valueType, context).DynamicContext();
                    }
                    break;
                default:
                    try
                    {
                        ISerializerOptions? options = context.Options,
                            keyOptions = options?.KeyOptions,
                            valueOptions = options?.ValueOptions;
                        bool hasOptions = keyOptions != null || valueOptions != null;
                        using ItemDeserializerContext keyContext = new(context, objectCache: false);
                        using ItemDeserializerContext valueContext = new(context)
                        {
                            Nullable = valueOptions?.IsNullable ?? context.Nullable
                        };
                        object key;
                        object? value;
                        (keyContext.ItemSerializer, keyContext.ItemSyncDeserializer, keyContext.ItemAsyncDeserializer) = 
                            keyType.GetItemDeserializerInfo(ObjectTypes.Null, isAsync: true);
                        (valueContext.ItemSerializer, valueContext.ItemSyncDeserializer, valueContext.ItemAsyncDeserializer) = 
                            valueType.GetItemDeserializerInfo(ObjectTypes.Null, isAsync: true);
                        object? obj;
                        for (int i = 0; i < count; i++)
                        {
                            if (hasOptions) context.WithOptions(keyOptions);
                            if (keyContext.ItemSerializer == SerializerTypes.Any)
                            {
                                await ReadAnyItemHeaderAsync(keyContext, i, keyType).DynamicContext();
                                key = (keyContext.ItemSerializer == SerializerTypes.Serializer
                                    ? await ReadItemAsync(keyContext).DynamicContext()
                                    : await ReadAnyIntAsync(context, keyContext.ObjectType, keyContext.ItemType).DynamicContext())!;
                            }
                            else
                            {
                                key = (await ReadItemAsync(keyContext).DynamicContext())!;
                            }
                            if (hasOptions) context.WithOptions(valueOptions);
                            if (valueContext.ItemSerializer == SerializerTypes.Any)
                            {
                                obj = await ReadAnyItemHeaderAsync(valueContext, i, valueType).DynamicContext();
                                if (obj == null && valueContext.ObjectType == ObjectTypes.Null)
                                {
                                    if (!context.Nullable) throw new SerializerException($"Deserialized NULL value #{i}", new InvalidDataException());
                                    value = null;
                                }
                                if (obj == null)
                                {
                                    value = (valueContext.ItemSerializer == SerializerTypes.Serializer
                                        ? await ReadItemAsync(valueContext).DynamicContext()
                                        : await ReadAnyIntAsync(context, valueContext.ObjectType, valueContext.ItemType).DynamicContext());
                                    if (valueContext.ObjectType.RequiresObjectWriting()) valueContext.AddObject(value);
                                }
                                else
                                {
                                    value = obj;
                                }
                            }
                            else
                            {
                                value = await ReadItemAsync(valueContext).DynamicContext();
                            }
                            dict[key] = value;
                        }
                        break;
                    }
                    finally
                    {
                        context.WithoutOptions();
                    }
            }
        }
    }
}
