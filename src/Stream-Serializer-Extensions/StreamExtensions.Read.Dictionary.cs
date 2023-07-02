using System.Buffers;
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
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <param name="keyOptions">Key serializer options</param>
        /// <param name="valueOptions">Value serializer options</param>
        /// <param name="valuesNullable">Are the values nullable?</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Dictionary<tKey, tValue> ReadDict<tKey, tValue>(
            this Stream stream,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            ISerializerOptions? keyOptions = null,
            ISerializerOptions? valueOptions = null,
            bool valuesNullable = false
            )
            where tKey : notnull
        {
            version ??= StreamSerializer.Version;
            pool ??= StreamSerializer.BufferPool;
            int len = ReadNumber<int>(stream, version, pool);
            SerializerHelper.EnsureValidLength(len, minLen, maxLen);
            Dictionary<tKey, tValue> res = new(len);
            ReadDictInt(stream, res, typeof(tKey), typeof(tValue), len, version.Value, keyOptions, valueOptions, valuesNullable);
            return res;
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Dictionary type</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <param name="keyOptions">Key serializer options</param>
        /// <param name="valueOptions">Value serializer options</param>
        /// <param name="valuesNullable">Are the values nullable?</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static IDictionary ReadDict(
            this Stream stream,
            Type type,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            ISerializerOptions? keyOptions = null,
            ISerializerOptions? valueOptions = null,
            bool valuesNullable = false
            )
        {
            SerializerException.Wrap(() => ArgumentValidationHelper.EnsureValidArgument(
                nameof(type),
                type.IsGenericType || type.IsGenericTypeDefinition || !typeof(Dictionary<,>).IsAssignableFrom(type.GetGenericTypeDefinition()),
                () => "Not a dictionary type"
                ));
            version ??= StreamSerializer.Version;
            pool ??= StreamSerializer.BufferPool;
            int len = ReadNumber<int>(stream, version, pool);
            SerializerHelper.EnsureValidLength(len, minLen, maxLen);
            Type[] types = type.GetGenericArguments();
            IDictionary res = (IDictionary)(Activator.CreateInstance(type, len) ?? throw new SerializerException($"Failed to instance {type}"));
            ReadDictInt(stream, res, type.GenericTypeArguments[0], type.GenericTypeArguments[1], len, version.Value, keyOptions, valueOptions, valuesNullable);
            return res;
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="tKey">Key type</typeparam>
        /// <typeparam name="tValue">Value type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <param name="keyOptions">Key serializer options</param>
        /// <param name="valueOptions">Value serializer options</param>
        /// <param name="valuesNullable">Are the values nullable?</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<Dictionary<tKey, tValue>> ReadDictAsync<tKey, tValue>(
            this Stream stream,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            ISerializerOptions? keyOptions = null,
            ISerializerOptions? valueOptions = null,
            bool valuesNullable = false,
            CancellationToken cancellationToken = default
            )
            where tKey : notnull
        {
            version ??= StreamSerializer.Version;
            pool ??= StreamSerializer.BufferPool;
            int len = await ReadNumberAsync<int>(stream, version, pool, cancellationToken).DynamicContext();
            SerializerHelper.EnsureValidLength(len, minLen, maxLen);
            Dictionary<tKey, tValue> res = new(len);
            await ReadDictIntAsync(stream, res, typeof(tKey), typeof(tValue), len, version.Value, keyOptions, valueOptions, valuesNullable, cancellationToken).DynamicContext();
            return res;
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Dictionary type</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <param name="keyOptions">Key serializer options</param>
        /// <param name="valueOptions">Value serializer options</param>
        /// <param name="valuesNullable">Are the values nullable?</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<IDictionary> ReadDictAsync(
            this Stream stream,
            Type type,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            ISerializerOptions? keyOptions = null,
            ISerializerOptions? valueOptions = null,
            bool valuesNullable = false,
            CancellationToken cancellationToken = default
            )
        {
            SerializerException.Wrap(() => ArgumentValidationHelper.EnsureValidArgument(
                nameof(type),
                type.IsGenericType || type.IsGenericTypeDefinition || !typeof(Dictionary<,>).IsAssignableFrom(type.GetGenericTypeDefinition()),
                () => "Not a dictionary type"
                ));
            version ??= StreamSerializer.Version;
            pool ??= StreamSerializer.BufferPool;
            int len = await ReadNumberAsync<int>(stream, version, pool, cancellationToken).DynamicContext();
            SerializerHelper.EnsureValidLength(len, minLen, maxLen);
            Type[] types = type.GetGenericArguments();
            IDictionary res = (IDictionary)(Activator.CreateInstance(type, len) ?? throw new SerializerException($"Failed to instance {type}"));
            await ReadDictIntAsync(
                stream, 
                res, 
                type.GenericTypeArguments[0], 
                type.GenericTypeArguments[1], 
                len, 
                version.Value, 
                keyOptions, 
                valueOptions, 
                valuesNullable, 
                cancellationToken
                )
                .DynamicContext();
            return res;
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="tKey">Key type</typeparam>
        /// <typeparam name="tValue">Value type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <param name="keyOptions">Key serializer options</param>
        /// <param name="valueOptions">Value serializer options</param>
        /// <param name="valuesNullable">Are the values nullable?</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Dictionary<tKey, tValue>? ReadDictNullable<tKey, tValue>(
            this Stream stream,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            ISerializerOptions? keyOptions = null,
            ISerializerOptions? valueOptions = null,
            bool valuesNullable = false
            )
            where tKey : notnull
        {
            pool ??= StreamSerializer.BufferPool;
            switch ((version ??= StreamSerializer.Version) & byte.MaxValue)// Serializer version switch
            {
                case 1:
                case 2:
                    return ReadBool(stream, version, pool) ? ReadDict<tKey, tValue>(stream, version, pool, minLen, maxLen, keyOptions, valueOptions) : null;
                default:
                    {
                        if (ReadNumberNullable<int>(stream, version, pool) is not int len) return null;
                        SerializerHelper.EnsureValidLength(len, minLen, maxLen);
                        Dictionary<tKey, tValue> res = new(len);
                        ReadDictInt(stream, res, typeof(tKey), typeof(tValue), len, version.Value, keyOptions, valueOptions, valuesNullable);
                        return res;
                    }
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Dictionary type</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <param name="keyOptions">Key serializer options</param>
        /// <param name="valueOptions">Value serializer options</param>
        /// <param name="valuesNullable">Are the values nullable?</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static IDictionary? ReadDictNullable(
            this Stream stream,
            Type type,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            ISerializerOptions? keyOptions = null,
            ISerializerOptions? valueOptions = null,
            bool valuesNullable = false
            )
        {
            SerializerException.Wrap(() => ArgumentValidationHelper.EnsureValidArgument(
                nameof(type),
                type.IsGenericType || type.IsGenericTypeDefinition || !typeof(Dictionary<,>).IsAssignableFrom(type.GetGenericTypeDefinition()),
                () => "Not a dictionary type"
                ));
            pool ??= StreamSerializer.BufferPool;
            switch ((version ??= StreamSerializer.Version) & byte.MaxValue)// Serializer version switch
            {
                case 1:
                case 2:
                    {
                        return ReadBool(stream, version, pool) ? ReadDict(stream, type, version, pool, minLen, maxLen, keyOptions, valueOptions) : null;
                    }
                default:
                    {
                        if (ReadNumberNullable<int>(stream, version, pool) is not int len) return null;
                        SerializerHelper.EnsureValidLength(len, minLen, maxLen);
                        Type[] types = type.GetGenericArguments();
                        IDictionary res = (IDictionary)(Activator.CreateInstance(type, len) ?? throw new SerializerException($"Failed to instance {type}"));
                        ReadDictInt(stream, res, type.GenericTypeArguments[0], type.GenericTypeArguments[1], len, version.Value, keyOptions, valueOptions, valuesNullable);
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
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <param name="keyOptions">Key serializer options</param>
        /// <param name="valueOptions">Value serializer options</param>
        /// <param name="valuesNullable">Are the values nullable?</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<Dictionary<tKey, tValue>?> ReadDictNullableAsync<tKey, tValue>(
            this Stream stream,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            ISerializerOptions? keyOptions = null,
            ISerializerOptions? valueOptions = null,
            bool valuesNullable = false,
            CancellationToken cancellationToken = default
            )
            where tKey : notnull
        {
            pool ??= StreamSerializer.BufferPool;
            switch ((version ??= StreamSerializer.Version) & byte.MaxValue)// Serializer version switch
            {
                case 1:
                case 2:
                    {
                        return await ReadBoolAsync(stream, version, pool, cancellationToken).DynamicContext()
                            ? await ReadDictAsync<tKey, tValue>(stream, version, pool, minLen, maxLen, keyOptions, valueOptions, cancellationToken: cancellationToken)
                                .DynamicContext()
                            : null;
                    }
                default:
                    {
                        if (await ReadNumberNullableAsync<int>(stream, version, pool, cancellationToken).DynamicContext() is not int len) return null;
                        SerializerHelper.EnsureValidLength(len, minLen, maxLen);
                        Dictionary<tKey, tValue> res = new(len);
                        await ReadDictIntAsync(stream, res, typeof(tKey), typeof(tValue), len, version.Value, keyOptions, valueOptions, valuesNullable, cancellationToken)
                            .DynamicContext();
                        return res;
                    }
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Dictionary type</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <param name="keyOptions">Key serializer options</param>
        /// <param name="valueOptions">Value serializer options</param>
        /// <param name="valuesNullable">Are the values nullable?</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<IDictionary?> ReadDictNullableAsync(
            this Stream stream,
            Type type,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            ISerializerOptions? keyOptions = null,
            ISerializerOptions? valueOptions = null,
            bool valuesNullable = false,
            CancellationToken cancellationToken = default
            )
        {
            SerializerException.Wrap(() => ArgumentValidationHelper.EnsureValidArgument(
                nameof(type),
                type.IsGenericType || type.IsGenericTypeDefinition || !typeof(Dictionary<,>).IsAssignableFrom(type.GetGenericTypeDefinition()),
                () => "Not a dictionary type"
                ));
            pool ??= StreamSerializer.BufferPool;
            switch ((version ??= StreamSerializer.Version) & byte.MaxValue)// Serializer version switch
            {
                case 1:
                case 2:
                    {
                        return await ReadBoolAsync(stream, version, pool, cancellationToken).DynamicContext()
                            ? await ReadDictAsync(stream, type, version, pool, minLen, maxLen, keyOptions, valueOptions, cancellationToken: cancellationToken).DynamicContext()
                            : null;
                    }
                default:
                    {
                        if (await ReadNumberNullableAsync<int>(stream, version, pool, cancellationToken).DynamicContext() is not int len) return null;
                        SerializerHelper.EnsureValidLength(len, minLen, maxLen);
                        Type[] types = type.GetGenericArguments();
                        IDictionary res = (IDictionary)(Activator.CreateInstance(type, len) ?? throw new SerializerException($"Failed to instance {type}"));
                        await ReadDictIntAsync(
                            stream,
                            res,
                            type.GenericTypeArguments[0],
                            type.GenericTypeArguments[1],
                            len,
                            version.Value,
                            keyOptions,
                            valueOptions,
                            valuesNullable,
                            cancellationToken
                            ).DynamicContext();
                        return res;
                    }
            }
        }

        /// <summary>
        /// Read dictionary items
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="dict">Dictionary</param>
        /// <param name="keyType">Key type</param>
        /// <param name="valueType">Value type</param>
        /// <param name="count">Number of items</param>
        /// <param name="version">Serializer version</param>
        /// <param name="keyOptions">Key serializer options</param>
        /// <param name="valueOptions">Value serializer options</param>
        /// <param name="valuesNullable">Are the values nullable?</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ReadDictInt(
            Stream stream,
            IDictionary dict,
            Type keyType,
            Type valueType,
            int count,
            int version,
            ISerializerOptions? keyOptions,
            ISerializerOptions? valueOptions,
            bool valuesNullable
            )
        {
            switch (version & byte.MaxValue)// Serializer version switch
            {
                case 1:
                case 2:
                    {
                        for (int i = 0; i < count; i++) dict[ReadObject(stream, keyType, version, keyOptions)] = ReadObject(stream, valueType, version, valueOptions);
                    }
                    break;
                default:
                    {
                        object key;
                        object? value;
                        (SerializerTypes keySerializer, StreamSerializer.Deserialize_Delegate? keySyncDeserializer, _) = keyType.GetItemDeserializerInfo(isAsync: false);
                        (SerializerTypes valueSerializer, StreamSerializer.Deserialize_Delegate? valueSyncDeserializer, _) = valueType.GetItemDeserializerInfo(isAsync: false);
                        Type? keyItemType = null,
                            valueItemType = null;
                        ObjectTypes keyObjType = default,
                            valueObjType = default,
                            lastKeyObjType = default,
                            lastValueObjType = default;
                        SerializerTypes keyItemSerializer = default,
                            valueItemSerializer = default;
                        StreamSerializer.Deserialize_Delegate? keyItemSyncDeserializer = null,
                            valueItemSyncDeserializer = null;
                        Type[]? keyTypeCache = null,
                            typeCache = null;
                        object[]? objectCache = null;
                        Span<Type> keyTypeCacheSpan,
                            typeCacheSpan;
                        ReadOnlySpan<object> keyObjectCacheSpan,
                            objectCacheSpan;
                        object? obj;
                        int objIndex;
                        if (valueOptions != null && !valuesNullable) valuesNullable = valueOptions.IsNullable;
                        try
                        {
                            if (keySerializer == SerializerTypes.Any)
                            {
                                keyTypeCache = ArrayPool<Type>.Shared.RentClean(byte.MaxValue);
                                keyTypeCacheSpan = typeCache.AsSpan(0, byte.MaxValue);
                                keyObjectCacheSpan = Array.Empty<object>().AsSpan();
                            }
                            else
                            {
                                keyTypeCacheSpan = Array.Empty<Type>().AsSpan();
                                keyObjectCacheSpan = Array.Empty<object>().AsSpan();
                            }
                            if (valueSerializer == SerializerTypes.Any)
                            {
                                typeCache = ArrayPool<Type>.Shared.RentClean(byte.MaxValue);
                                typeCacheSpan = typeCache.AsSpan(0, byte.MaxValue);
                                objectCache = ArrayPool<object>.Shared.RentClean(byte.MaxValue);
                                objectCacheSpan = objectCache.AsSpan(0, byte.MaxValue);
                            }
                            else
                            {
                                typeCacheSpan = Array.Empty<Type>().AsSpan();
                                objectCacheSpan = Array.Empty<object>().AsSpan();
                            }
                            for (int i = 0; i < count; i++)
                            {
                                if (keySerializer == SerializerTypes.Any)
                                {
                                    ReadAnyItemHeader(
                                        stream,
                                        version,
                                        keyType,
                                        i,
                                        keyTypeCacheSpan,
                                        keyObjectCacheSpan,
                                        ref keyObjType,
                                        ref lastKeyObjType,
                                        ref keyItemType,
                                        ref keyItemSerializer,
                                        ref keyItemSyncDeserializer
                                        );
                                    key = (keyItemSerializer == SerializerTypes.Serializer
                                        ? ReadItem(stream, version, nullable: false, keyItemSerializer, keyType, pool: null, keyOptions, keyItemSyncDeserializer)
                                        : ReadAnyInt(stream, version, keyObjType, keyItemType, keyOptions))!;
                                }
                                else
                                {
                                    key = ReadItem(stream, version, nullable: false, keySerializer, keyType, pool: null, keyOptions, keySyncDeserializer)!;
                                }
                                if (valueSerializer == SerializerTypes.Any)
                                {
                                    obj = ReadAnyItemHeader(
                                        stream,
                                        version,
                                        valueType,
                                        i,
                                        typeCacheSpan,
                                        objectCacheSpan,
                                        ref valueObjType,
                                        ref lastValueObjType,
                                        ref valueItemType,
                                        ref valueItemSerializer,
                                        ref valueItemSyncDeserializer
                                        );
                                    if (obj == null && valueObjType == ObjectTypes.Null)
                                    {
                                        if (!valuesNullable) throw new SerializerException($"Deserialized NULL value #{i}", new InvalidDataException());
                                        value = null;
                                    }
                                    if (obj == null)
                                    {
                                        value = (valueItemSerializer == SerializerTypes.Serializer
                                            ? ReadItem(stream, version, nullable: false, valueItemSerializer, valueType, pool: null, valueOptions, valueItemSyncDeserializer)
                                            : ReadAnyInt(stream, version, valueObjType, valueItemType, valueOptions))!;
                                        objIndex = objectCache!.IndexOf(null);
                                        if (objIndex != -1) objectCache![objIndex] = value!;
                                    }
                                    else
                                    {
                                        value = obj;
                                    }
                                }
                                else
                                {
                                    value = ReadItem(stream, version, valuesNullable, valueSerializer, valueType, pool: null, valueOptions, valueSyncDeserializer)!;
                                }
                                dict[key] = value;
                            }
                        }
                        finally
                        {
                            if (keyTypeCache != null) ArrayPool<Type>.Shared.Return(keyTypeCache);
                            if (typeCache != null) ArrayPool<Type>.Shared.Return(typeCache);
                            if (objectCache != null) ArrayPool<object>.Shared.Return(objectCache);
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// Read dictionary items
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="dict">Dictionary</param>
        /// <param name="keyType">Key type</param>
        /// <param name="valueType">Value type</param>
        /// <param name="count">Number of items</param>
        /// <param name="version">Serializer version</param>
        /// <param name="keyOptions">Key serializer options</param>
        /// <param name="valueOptions">Value serializer options</param>
        /// <param name="valuesNullable">Are the values nullable?</param>
        /// <param name="cancellationToken">Cancellation token</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static async Task ReadDictIntAsync(
            Stream stream,
            IDictionary dict,
            Type keyType,
            Type valueType,
            int count,
            int version,
            ISerializerOptions? keyOptions,
            ISerializerOptions? valueOptions,
            bool valuesNullable,
            CancellationToken cancellationToken
            )
        {
            switch (version & byte.MaxValue)// Serializer version switch
            {
                case 1:
                case 2:
                    {
                        for (int i = 0; i < count; i++)
                            dict[await ReadObjectAsync(stream, keyType, version, keyOptions, cancellationToken).DynamicContext()] =
                                await ReadObjectAsync(stream, valueType, version, valueOptions, cancellationToken).DynamicContext();
                    }
                    break;
                default:
                    {
                        object key;
                        object? value;
                        (
                            SerializerTypes keySerializer,
                            StreamSerializer.Deserialize_Delegate? keySyncDeserializer,
                            StreamSerializer.AsyncDeserialize_Delegate? keyAsyncDeserializer
                            ) =
                            keyType.GetItemDeserializerInfo(isAsync: true);
                        (
                            SerializerTypes valueSerializer,
                            StreamSerializer.Deserialize_Delegate? valueSyncDeserializer,
                            StreamSerializer.AsyncDeserialize_Delegate? valueAsyncDeserializer
                            ) =
                            valueType.GetItemDeserializerInfo(isAsync: true);
                        Type? keyItemType = null,
                            valueItemType = null;
                        ObjectTypes keyObjType = default,
                            valueObjType = default,
                            lastKeyObjType = default,
                            lastValueObjType = default;
                        SerializerTypes keyItemSerializer = default,
                            valueItemSerializer = default;
                        StreamSerializer.Deserialize_Delegate? keyItemSyncDeserializer = null,
                            valueItemSyncDeserializer = null;
                        StreamSerializer.AsyncDeserialize_Delegate? keyItemAsyncDeserializer = null,
                            valueItemAsyncDeserializer = null;
                        Type[]? keyTypeCache = null,
                            typeCache = null;
                        object[]? objectCache = null;
                        Memory<Type> keyTypeCacheMem,
                            typeCacheMem;
                        ReadOnlyMemory<object> keyObjectCacheMem,
                            objectCacheMem;
                        object? obj;
                        int objIndex;
                        if (valueOptions != null && !valuesNullable) valuesNullable = valueOptions.IsNullable;
                        try
                        {
                            if (keySerializer == SerializerTypes.Any)
                            {
                                keyTypeCache = ArrayPool<Type>.Shared.RentClean(byte.MaxValue);
                                keyTypeCacheMem = typeCache.AsMemory(0, byte.MaxValue);
                                keyObjectCacheMem = Array.Empty<object>().AsMemory();
                            }
                            else
                            {
                                keyTypeCacheMem = Array.Empty<Type>().AsMemory();
                                keyObjectCacheMem = Array.Empty<object>().AsMemory();
                            }
                            if (valueSerializer == SerializerTypes.Any)
                            {
                                typeCache = ArrayPool<Type>.Shared.RentClean(byte.MaxValue);
                                typeCacheMem = typeCache.AsMemory(0, byte.MaxValue);
                                objectCache = ArrayPool<object>.Shared.RentClean(byte.MaxValue);
                                objectCacheMem = objectCache.AsMemory(0, byte.MaxValue);
                            }
                            else
                            {
                                typeCacheMem = Array.Empty<Type>().AsMemory();
                                objectCacheMem = Array.Empty<object>().AsMemory();
                            }
                            for (int i = 0; i < count; i++)
                            {
                                if (keySerializer == SerializerTypes.Any)
                                {
                                    (obj, keyObjType, lastKeyObjType, keyItemType, keyItemSerializer, keyItemSyncDeserializer, keyItemAsyncDeserializer) =
                                        await ReadAnyItemHeaderAsync(
                                            stream,
                                            version,
                                            keyType,
                                            i,
                                            keyTypeCacheMem,
                                            keyObjectCacheMem,
                                            lastKeyObjType,
                                            keyItemType,
                                            keyItemSerializer,
                                            keyItemSyncDeserializer,
                                            keyItemAsyncDeserializer,
                                            cancellationToken
                                            );
                                    key = (keyItemSerializer == SerializerTypes.Serializer
                                        ? await ReadItemAsync(
                                            stream,
                                            version,
                                            nullable: false,
                                            keyItemSerializer,
                                            keyItemType,
                                            pool: null,
                                            keyOptions,
                                            keyItemSyncDeserializer,
                                            keyItemAsyncDeserializer,
                                            cancellationToken
                                            ).DynamicContext()
                                        : await ReadAnyIntAsync(stream, version, keyObjType, keyItemType, keyOptions, cancellationToken).DynamicContext())!;
                                }
                                else
                                {
                                    key = ReadItem(stream, version, nullable: false, keySerializer, keyType, pool: null, keyOptions, keySyncDeserializer)!;
                                }
                                if (valueSerializer == SerializerTypes.Any)
                                {
                                    (obj, valueObjType, lastValueObjType, valueItemType, valueItemSerializer, valueItemSyncDeserializer, valueItemAsyncDeserializer) =
                                        await ReadAnyItemHeaderAsync(
                                            stream,
                                            version,
                                            valueType,
                                            i,
                                            typeCacheMem,
                                            objectCacheMem,
                                            lastValueObjType,
                                            valueItemType,
                                            valueItemSerializer,
                                            valueItemSyncDeserializer,
                                            valueItemAsyncDeserializer,
                                            cancellationToken
                                            ).DynamicContext();
                                    if (obj == null && valueObjType == ObjectTypes.Null)
                                    {
                                        if (!valuesNullable) throw new SerializerException($"Deserialized NULL value #{i}", new InvalidDataException());
                                        value = null;
                                    }
                                    else if (obj == null)
                                    {
                                        value = (valueItemSerializer == SerializerTypes.Serializer
                                            ? await ReadItemAsync(
                                                stream,
                                                version,
                                                nullable: false,
                                                valueItemSerializer,
                                                valueType,
                                                pool: null,
                                                valueOptions,
                                                valueItemSyncDeserializer,
                                                valueItemAsyncDeserializer,
                                                cancellationToken
                                                ).DynamicContext()
                                            : await ReadAnyIntAsync(stream, version, valueObjType, valueItemType, valueOptions, cancellationToken).DynamicContext())!;
                                        objIndex = objectCache!.IndexOf(null);
                                        if (objIndex != -1) objectCache![objIndex] = value!;
                                    }
                                    else
                                    {
                                        value = obj;
                                    }
                                }
                                else
                                {
                                    value = (await ReadItemAsync(
                                        stream,
                                        version,
                                        valuesNullable,
                                        valueSerializer,
                                        valueType,
                                        pool: null,
                                        valueOptions,
                                        valueSyncDeserializer,
                                        valueAsyncDeserializer,
                                        cancellationToken
                                        ).DynamicContext())!;
                                }
                                dict[key] = value;
                            }
                        }
                        finally
                        {
                            if (keyTypeCache != null) ArrayPool<Type>.Shared.Return(keyTypeCache);
                            if (typeCache != null) ArrayPool<Type>.Shared.Return(typeCache);
                            if (objectCache != null) ArrayPool<object>.Shared.Return(objectCache);
                        }
                        break;
                    }
            }
        }
    }
}
