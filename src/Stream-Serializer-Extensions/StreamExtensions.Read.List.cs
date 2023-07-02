using System.Buffers;
using System.Collections;
using System.Runtime;
using System.Runtime.CompilerServices;
using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    // List
    public static partial class StreamExtensions
    {
        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <param name="valueOptions">Value serializer options</param>
        /// <param name="valuesNullable">Are the values nullable?</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static List<T> ReadList<T>(
            this Stream stream,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            ISerializerOptions? valueOptions = null,
            bool valuesNullable = false
            )
        {
            version ??= StreamSerializer.Version;
            int len = ReadNumber<int>(stream, version, pool);
            SerializerHelper.EnsureValidLength(len, minLen, maxLen);
            List<T> res = new(len);
            ReadListInt(stream, res, typeof(T), len, version.Value, valueOptions, valuesNullable);
            return res;
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">List type</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <param name="valueOptions">Value serializer options</param>
        /// <param name="valuesNullable">Are the values nullable?</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static IList ReadList(
            this Stream stream,
            Type type,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            ISerializerOptions? valueOptions = null,
            bool valuesNullable = false
            )
        {
            SerializerException.Wrap(() => ArgumentValidationHelper.EnsureValidArgument(
                nameof(type),
                type.IsGenericType || type.IsGenericTypeDefinition || !typeof(List<>).IsAssignableFrom(type.GetGenericTypeDefinition()),
                () => "Not a list type"
                ));
            version ??= StreamSerializer.Version;
            int len = ReadNumber<int>(stream, version, pool);
            SerializerHelper.EnsureValidLength(len, minLen, maxLen);
            Type itemType = type.GetGenericArgumentsCached()[0];
            IList res = (IList)(Activator.CreateInstance(type, len) ?? throw new SerializerException($"Failed to instance {type}"));
            ReadListInt(stream, res, itemType, len, version.Value, valueOptions, valuesNullable);
            return res;
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <param name="valueOptions">Value serializer options</param>
        /// <param name="valuesNullable">Are the values nullable?</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<List<T>> ReadListAsync<T>(
            this Stream stream,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            ISerializerOptions? valueOptions = null,
            bool valuesNullable = false,
            CancellationToken cancellationToken = default
            )
        {
            version ??= StreamSerializer.Version;
            int len = await ReadNumberAsync<int>(stream, version, pool, cancellationToken).DynamicContext();
            SerializerHelper.EnsureValidLength(len, minLen, maxLen);
            List<T> res = new(len);
            await ReadListIntAsync(stream, res, typeof(T), len, version.Value, valueOptions, valuesNullable, cancellationToken).DynamicContext();
            return res;
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">List type</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <param name="valueOptions">Value serializer options</param>
        /// <param name="valuesNullable">Are the values nullable?</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<IList> ReadListAsync(
            this Stream stream,
            Type type,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            ISerializerOptions? valueOptions = null,
            bool valuesNullable = false,
            CancellationToken cancellationToken = default
            )
        {
            SerializerException.Wrap(() => ArgumentValidationHelper.EnsureValidArgument(
                nameof(type),
                type.IsGenericType || type.IsGenericTypeDefinition || !typeof(List<>).IsAssignableFrom(type.GetGenericTypeDefinition()),
                () => "Not a list type"
                ));
            version ??= StreamSerializer.Version;
            int len = await ReadNumberAsync<int>(stream, version, pool, cancellationToken).DynamicContext();
            SerializerHelper.EnsureValidLength(len, minLen, maxLen);
            Type itemType = type.GetGenericArgumentsCached()[0];
            IList res = (IList)(Activator.CreateInstance(type, len) ?? throw new SerializerException($"Failed to instance {type}"));
            await ReadListIntAsync(stream, res, type.GenericTypeArguments[0], len, version.Value, valueOptions, valuesNullable, cancellationToken).DynamicContext();
            return res;
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <param name="valueOptions">Value serializer options</param>
        /// <param name="valuesNullable">Are the values nullable?</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static List<T>? ReadListNullable<T>(
            this Stream stream,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            ISerializerOptions? valueOptions = null,
            bool valuesNullable = false
            )
        {
            switch ((version ??= StreamSerializer.Version) & byte.MaxValue)// Serializer version switch
            {
                case 1:
                case 2:
                    {
                        return ReadBool(stream, version, pool) ? ReadList<T>(stream, version, pool, minLen, maxLen, valueOptions) : null;
                    }
                default:
                    {
                        version ??= StreamSerializer.Version;
                        if (ReadNumberNullable<int>(stream, version, pool) is not int len) return null;
                        SerializerHelper.EnsureValidLength(len, minLen, maxLen);
                        List<T> res = new(len);
                        ReadListInt(stream, res, typeof(T), len, version.Value, valueOptions, valuesNullable);
                        return res;
                    }
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">List type</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <param name="valueOptions">Value serializer options</param>
        /// <param name="valuesNullable">Are the values nullable?</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static IList? ReadListNullable(
            this Stream stream,
            Type type,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            ISerializerOptions? valueOptions = null,
            bool valuesNullable = false
            )
        {
            SerializerException.Wrap(() => ArgumentValidationHelper.EnsureValidArgument(
                nameof(type),
                type.IsGenericType || type.IsGenericTypeDefinition || !typeof(List<>).IsAssignableFrom(type.GetGenericTypeDefinition()),
                () => "Not a list type"
                ));
            switch ((version ??= StreamSerializer.Version) & byte.MaxValue)// Serializer version switch
            {
                case 1:
                case 2:
                    {
                        return ReadBool(stream, version, pool) ? ReadList(stream, type, version, pool, minLen, maxLen, valueOptions) : null;
                    }
                default:
                    {
                        version ??= StreamSerializer.Version;
                        if (ReadNumberNullable<int>(stream, version, pool) is not int len) return null;
                        SerializerHelper.EnsureValidLength(len, minLen, maxLen);
                        Type itemType = type.GetGenericArgumentsCached()[0];
                        IList res = (IList)(Activator.CreateInstance(type, len) ?? throw new SerializerException($"Failed to instance {type}"));
                        ReadListInt(stream, res, itemType, len, version.Value, valueOptions, valuesNullable);
                        return res;
                    }
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <param name="valueOptions">Value serializer options</param>
        /// <param name="valuesNullable">Are the values nullable?</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<List<T>?> ReadListNullableAsync<T>(
            this Stream stream,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            ISerializerOptions? valueOptions = null,
            bool valuesNullable = false,
            CancellationToken cancellationToken = default
            )
        {
            switch ((version ??= StreamSerializer.Version) & byte.MaxValue)// Serializer version switch
            {
                case 1:
                case 2:
                    {
                        return await ReadBoolAsync(stream, version, pool, cancellationToken).DynamicContext()
                            ? await ReadListAsync<T>(stream, version, pool, minLen, maxLen, valueOptions, cancellationToken: cancellationToken).DynamicContext()
                            : null;
                    }
                default:
                    {
                        version ??= StreamSerializer.Version;
                        if (await ReadNumberNullableAsync<int>(stream, version, pool, cancellationToken).DynamicContext() is not int len) return null;
                        SerializerHelper.EnsureValidLength(len, minLen, maxLen);
                        List<T> res = new(len);
                        await ReadListIntAsync(stream, res, typeof(T), len, version.Value, valueOptions, valuesNullable, cancellationToken).DynamicContext();
                        return res;
                    }
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">List type</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <param name="valueOptions">Value serializer options</param>
        /// <param name="valuesNullable">Are the values nullable?</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<IList?> ReadListNullableAsync(
            this Stream stream,
            Type type,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            ISerializerOptions? valueOptions = null,
            bool valuesNullable = false,
            CancellationToken cancellationToken = default
            )
        {
            SerializerException.Wrap(() => ArgumentValidationHelper.EnsureValidArgument(
                nameof(type),
                type.IsGenericType || type.IsGenericTypeDefinition || !typeof(List<>).IsAssignableFrom(type.GetGenericTypeDefinition()),
                () => "Not a list type"
                ));
            switch ((version ??= StreamSerializer.Version) & byte.MaxValue)// Serializer version switch
            {
                case 1:
                case 2:
                    {
                        return await ReadBoolAsync(stream, version, pool, cancellationToken).DynamicContext()
                            ? await ReadListAsync(stream, type, version, pool, minLen, maxLen, valueOptions, cancellationToken: cancellationToken).DynamicContext()
                            : null;
                    }
                default:
                    {
                        version ??= StreamSerializer.Version;
                        if (await ReadNumberNullableAsync<int>(stream, version, pool, cancellationToken).DynamicContext() is not int len) return null;
                        SerializerHelper.EnsureValidLength(len, minLen, maxLen);
                        Type itemType = type.GetGenericArgumentsCached()[0];
                        IList res = (IList)(Activator.CreateInstance(type, len) ?? throw new SerializerException($"Failed to instance {type}"));
                        await ReadListIntAsync(stream, res, itemType, len, version.Value, valueOptions, valuesNullable, cancellationToken).DynamicContext();
                        return res;
                    }
            }
        }

        /// <summary>
        /// Read list items
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="list">List</param>
        /// <param name="type">Item type</param>
        /// <param name="count">Number of items</param>
        /// <param name="version">Serializer version</param>
        /// <param name="valueOptions">Value serializer options</param>
        /// <param name="valuesNullable">Are the values nullable?</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ReadListInt(Stream stream, IList list, Type type, int count, int version, ISerializerOptions? valueOptions, bool valuesNullable)
        {
            switch (version & byte.MaxValue)// Serializer version switch
            {
                case 1:
                case 2:
                    {
                        for (int i = 0; i < count; list.Add(ReadObject(stream, type, version, valueOptions)), i++) ;
                        break;
                    }
                default:
                    {
                        (SerializerTypes serializer, StreamSerializer.Deserialize_Delegate? syncDeserializer, _) = type.GetItemDeserializerInfo(isAsync: false);
                        if (valueOptions != null && !valuesNullable) valuesNullable = valueOptions.IsNullable;
                        if (serializer == SerializerTypes.Any)
                        {
                            Type? itemType = null;
                            ObjectTypes objType = default,
                                lastObjType = default;
                            SerializerTypes itemSerializer = default;
                            StreamSerializer.Deserialize_Delegate? itemSyncDeserializer = null;
                            Type[]? typeCache = null;
                            object[]? objectCache = null;
                            Span<Type> typeCacheSpan;
                            ReadOnlySpan<object> objectCacheSpan;
                            object? obj;
                            int objIndex;
                            try
                            {
                                typeCache = ArrayPool<Type>.Shared.RentClean(byte.MaxValue);
                                typeCacheSpan = typeCache.AsSpan(0, byte.MaxValue);
                                objectCache = ArrayPool<object>.Shared.RentClean(byte.MaxValue);
                                objectCacheSpan = objectCache.AsSpan(0, byte.MaxValue);
                                for (int i = 0; i < count; i++)
                                {
                                    obj = ReadAnyItemHeader(
                                        stream,
                                        version,
                                        type,
                                        i,
                                        typeCacheSpan,
                                        objectCacheSpan,
                                        ref objType,
                                        ref lastObjType,
                                        ref itemType,
                                        ref itemSerializer,
                                        ref itemSyncDeserializer
                                        );
                                    if (obj == null && objType == ObjectTypes.Null)
                                    {
                                        if (!valuesNullable) throw new SerializerException($"Deserialized NULL value #{i}", new InvalidDataException());
                                        list.Add(null);
                                    }
                                    else if (obj == null)
                                    {
                                        list.Add((obj = itemSerializer == SerializerTypes.Serializer
                                            ? ReadItem(stream, version, nullable: false, itemSerializer, itemType, pool: null, valueOptions, itemSyncDeserializer)
                                            : ReadAnyInt(stream, version, objType, itemType, valueOptions))!);
                                        objIndex = objectCache.IndexOf(null);
                                        if (objIndex != -1) objectCache[objIndex] = obj!;
                                    }
                                    else
                                    {
                                        list.Add(obj);
                                    }
                                }
                            }
                            finally
                            {
                                if (typeCache != null) ArrayPool<Type>.Shared.Return(typeCache);
                                if (objectCache != null) ArrayPool<object>.Shared.Return(objectCache);
                            }
                        }
                        else
                        {
                            for (int i = 0; i < count; i++)
                                list.Add(ReadItem(stream, version, valuesNullable, serializer, type, pool: null, valueOptions, syncDeserializer)!);
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// Read list items
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="list">List</param>
        /// <param name="type">Item type</param>
        /// <param name="count">Number of items</param>
        /// <param name="version">Serializer version</param>
        /// <param name="valueOptions">Value serializer options</param>
        /// <param name="valuesNullable">Are the values nullable?</param>
        /// <param name="cancellationToken">Cancellation token</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static async Task ReadListIntAsync(
            Stream stream,
            IList list,
            Type type,
            int count,
            int version,
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
                        for (int i = 0; i < count; list.Add(await ReadObjectAsync(stream, type, version, valueOptions, cancellationToken).DynamicContext()), i++) ;
                        break;
                    }
                default:
                    {
                        (SerializerTypes serializer, StreamSerializer.Deserialize_Delegate? syncDeserializer, StreamSerializer.AsyncDeserialize_Delegate? asyncDeserializer) =
                            type.GetItemDeserializerInfo(isAsync: true);
                        if (valueOptions != null && !valuesNullable) valuesNullable = valueOptions.IsNullable;
                        if (serializer == SerializerTypes.Any)
                        {
                            Type? itemType = null;
                            ObjectTypes objType = default,
                                lastObjType = default;
                            SerializerTypes itemSerializer = default;
                            StreamSerializer.Deserialize_Delegate? itemSyncDeserializer = null;
                            StreamSerializer.AsyncDeserialize_Delegate? itemAsyncDeserializer = null;
                            Type[]? typeCache = null;
                            object[]? objectCache = null;
                            Memory<Type> typeCacheMem;
                            ReadOnlyMemory<object> objectCacheMem;
                            object? obj;
                            int objIndex;
                            try
                            {
                                typeCache = ArrayPool<Type>.Shared.RentClean(byte.MaxValue);
                                typeCacheMem = typeCache.AsMemory(0, byte.MaxValue);
                                objectCache = ArrayPool<object>.Shared.RentClean(byte.MaxValue);
                                objectCacheMem = objectCache.AsMemory(0, byte.MaxValue);
                                for (int i = 0; i < count; i++)
                                {
                                    (obj, objType, lastObjType, itemType, itemSerializer, itemSyncDeserializer, itemAsyncDeserializer) =
                                        await ReadAnyItemHeaderAsync(
                                            stream,
                                            version,
                                            type,
                                            i,
                                            typeCacheMem,
                                            objectCacheMem,
                                            lastObjType,
                                            itemType,
                                            itemSerializer,
                                            itemSyncDeserializer,
                                            itemAsyncDeserializer,
                                            cancellationToken
                                            ).DynamicContext();
                                    if (obj == null && objType == ObjectTypes.Null)
                                    {
                                        if (!valuesNullable) throw new SerializerException($"Deserialized NULL value #{i}", new InvalidDataException());
                                        list.Add(null);
                                    }
                                    else if (obj == null)
                                    {
                                        list.Add(obj = itemSerializer == SerializerTypes.Serializer
                                            ? await ReadItemAsync(
                                                stream,
                                                version,
                                                nullable: false,
                                                itemSerializer,
                                                itemType,
                                                pool: null,
                                                valueOptions,
                                                itemSyncDeserializer,
                                                itemAsyncDeserializer,
                                                cancellationToken
                                                ).DynamicContext()
                                            : await ReadAnyIntAsync(stream, version, lastObjType, itemType, valueOptions, cancellationToken).DynamicContext());
                                        objIndex = objectCache.IndexOf(null);
                                        if (objIndex != -1) objectCache[objIndex] = obj!;
                                    }
                                    else
                                    {
                                        list.Add(obj);
                                    }
                                }
                            }
                            finally
                            {
                                if (typeCache != null) ArrayPool<Type>.Shared.Return(typeCache);
                                if (objectCache != null) ArrayPool<object>.Shared.Return(objectCache);
                            }
                        }
                        else
                        {
                            for (int i = 0; i < count; i++)
                                list.Add(await ReadItemAsync(
                                    stream,
                                    version,
                                    valuesNullable,
                                    serializer,
                                    type,
                                    pool: null,
                                    valueOptions,
                                    syncDeserializer,
                                    asyncDeserializer,
                                    cancellationToken
                                    ).DynamicContext());
                        }
                        break;
                    }
            }
        }
    }
}
