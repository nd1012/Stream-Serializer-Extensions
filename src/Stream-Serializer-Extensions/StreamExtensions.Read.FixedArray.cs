using System.Buffers;
using System.Runtime;
using System.Runtime.CompilerServices;
using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    // Fixed array
    public static partial class StreamExtensions
    {
        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="arr">Array</param>
        /// <param name="version">Serializer version</param>
        /// <param name="valueOptions">Value deserializer options</param>
        /// <param name="valuesNullable">Are the values nullable?</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] ReadFixedArray<T>(
            this Stream stream, 
            T[] arr, 
            int? version = null, 
            ISerializerOptions? valueOptions = null, 
            bool valuesNullable = false,
            ArrayPool<byte>? pool = null
            )
        {
            ReadFixedArray(stream, arr.AsSpan(), version, valueOptions, valuesNullable, pool);
            return arr;
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="arr">Array</param>
        /// <param name="version">Serializer version</param>
        /// <param name="valueOptions">Value deserializer options</param>
        /// <param name="valuesNullable">Are the values nullable?</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Span<T> ReadFixedArray<T>(
            this Stream stream, 
            Span<T> arr, 
            int? version = null, 
            ISerializerOptions? valueOptions = null,
            bool valuesNullable = false,
            ArrayPool<byte>? pool = null
            )
        {
            try
            {
                switch ((version ??= StreamSerializer.Version) & byte.MaxValue)// Serializer version switch
                {
                    case 1:
                    case 2:
                        {
                            for (int i = 0, len = arr.Length; i < len; arr[i] = ReadObject<T>(stream, version, valueOptions), i++) ;
                            return arr;
                        }
                    default:
                        {
                            Type type = typeof(T);
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
                                    for (int i = 0, len = arr.Length; i < len; i++)
                                    {
                                        obj = ReadAnyItemHeader(
                                            stream,
                                            version.Value,
                                            type,
                                            i,
                                            typeCache,
                                            objectCache,
                                            ref objType,
                                            ref lastObjType,
                                            ref itemType,
                                            ref itemSerializer,
                                            ref itemSyncDeserializer
                                            );
                                        Logging.WriteInfo($"READ {i} {stream.Position} {objType} {(int)objType} {itemType} {obj}");
                                        if (obj == null && objType == ObjectTypes.Null)
                                        {
                                            if (!valuesNullable) throw new SerializerException($"Deserialized NULL value #{i}", new InvalidDataException());
                                            arr[i] = (T?)obj!;
                                        }
                                        else if (obj == null)
                                        {
                                            arr[i] = (itemSerializer == SerializerTypes.Serializer
                                                ? (T?)(obj = ReadItem(stream, version.Value, nullable: false, itemSerializer, itemType, pool, valueOptions, itemSyncDeserializer))
                                                : (T?)(obj = ReadAnyInt(stream, version.Value, objType, itemType, valueOptions)))!;
                                            objIndex = objectCache.IndexOf(null);
                                            if (objIndex != -1) objectCache[objIndex] = obj!;
                                            Logging.WriteInfo($"RED {obj}");
                                        }
                                        else
                                        {
                                            arr[i] = (T)obj;
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
                                for (int i = 0, len = arr.Length; i < len; i++)
                                    arr[i] = (T)ReadItem(stream, version.Value, valuesNullable, serializer, type, pool, valueOptions, syncDeserializer)!;
                            }
                            return arr;
                        }
                }
            }
            catch (SerializerException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw SerializerException.From(ex);
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="arr">Array</param>
        /// <param name="version">Serializer version</param>
        /// <param name="valueOptions">Value deserializer options</param>
        /// <param name="valuesNullable">Are the values nullable?</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Array ReadFixedArray(
            this Stream stream, 
            Array arr, 
            int? version = null, 
            ISerializerOptions? valueOptions = null,
            bool valuesNullable = false,
            ArrayPool<byte>? pool = null
            )
        {
            Type elementType = arr.GetType().GetElementType()!;
            switch ((version ??= StreamSerializer.Version) & byte.MaxValue)// Serializer version switch
            {
                case 1:
                case 2:
                    {
                        for (int i = 0, len = arr.Length; i < len; arr.SetValue(ReadObject(stream, elementType, version, valueOptions), i), i++) ;
                        return arr;
                    }
                default:
                    {
                        Type type = arr.GetType().GetElementType()!;
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
                                for (int i = 0, len = arr.Length; i < len; i++)
                                {
                                    obj = ReadAnyItemHeader(
                                        stream, 
                                        version.Value, 
                                        type, 
                                        i, 
                                        typeCache,
                                        objectCache,
                                        ref objType,
                                        ref lastObjType, 
                                        ref itemType, 
                                        ref itemSerializer, 
                                        ref itemSyncDeserializer
                                        );
                                    if (obj == null && objType == ObjectTypes.Null)
                                    {
                                        if (!valuesNullable) throw new SerializerException($"Deserialized NULL value #{i}", new InvalidDataException());
                                        arr.SetValue(null, i);
                                    }
                                    else if (obj == null)
                                    {
                                        arr.SetValue(obj = itemSerializer == SerializerTypes.Serializer
                                            ? ReadItem(stream, version.Value, nullable: false, itemSerializer, itemType, pool, valueOptions, itemSyncDeserializer)
                                            : ReadAnyInt(stream, version.Value, objType, itemType, valueOptions),
                                            i);
                                        objIndex = objectCache.IndexOf(null);
                                        if (objIndex != -1) objectCache[objIndex] = obj!;
                                    }
                                    else
                                    {
                                        arr.SetValue(obj, i);
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
                            for (int i = 0, len = arr.Length; i < len; i++)
                                arr.SetValue(ReadItem(stream, version.Value, valuesNullable, serializer, type, pool, valueOptions, syncDeserializer)!, i);
                        }
                        return arr;
                    }
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="arr">Array</param>
        /// <param name="version">Serializer version</param>
        /// <param name="valueOptions">Value deserializer options</param>
        /// <param name="valuesNullable">Are the values nullable?</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<T[]> ReadFixedArrayAsync<T>(
            this Stream stream,
            T[] arr,
            int? version = null,
            ISerializerOptions? valueOptions = null,
            bool valuesNullable = false,
            ArrayPool<byte>? pool = null,
            CancellationToken cancellationToken = default
            )
        {
            await ReadFixedArrayAsync(stream, arr.AsMemory(), version, valueOptions, valuesNullable, pool, cancellationToken).DynamicContext();
            return arr;
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="arr">Array</param>
        /// <param name="version">Serializer version</param>
        /// <param name="valueOptions">Value deserializer options</param>
        /// <param name="valuesNullable">Are the values nullable?</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<Memory<T>> ReadFixedArrayAsync<T>(
            this Stream stream,
            Memory<T> arr,
            int? version = null,
            ISerializerOptions? valueOptions = null,
            bool valuesNullable = false,
            ArrayPool<byte>? pool = null,
            CancellationToken cancellationToken = default
            )
        {
            T? item;
            switch ((version ??= StreamSerializer.Version) & byte.MaxValue)// Serializer version switch
            {
                case 1:
                case 2:
                    {
                        for (int i = 0, len = arr.Length; i < len; i++)
                        {
                            item = await ReadObjectAsync<T>(stream, version, valueOptions, cancellationToken).DynamicContext();
                            arr.Span[i] = item;
                        }
                        return arr;
                    }
                default:
                    {
                        Type type = typeof(T);
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
                                for (int i = 0, len = arr.Length; i < len; i++)
                                {
                                    (obj, objType, lastObjType, itemType, itemSerializer, itemSyncDeserializer, itemAsyncDeserializer) =
                                        await ReadAnyItemHeaderAsync(
                                            stream,
                                            version.Value,
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
                                        item = default;
                                    }
                                    else if (obj == null)
                                    {
                                        item = (itemSerializer == SerializerTypes.Serializer
                                            ? (T?)await ReadItemAsync(
                                                stream,
                                                version.Value,
                                                nullable: false,
                                                itemSerializer,
                                                itemType,
                                                pool,
                                                valueOptions,
                                                itemSyncDeserializer,
                                                itemAsyncDeserializer,
                                                cancellationToken
                                                ).DynamicContext()
                                            : (T?)await ReadAnyIntAsync(stream, version.Value, lastObjType, itemType, valueOptions, cancellationToken).DynamicContext())!;
                                        objIndex = objectCache.IndexOf(null);
                                        if (objIndex != -1) objectCache[objIndex] = item!;
                                    }
                                    else
                                    {
                                        item = (T)obj;
                                    }
                                    arr.Span[i] = item!;
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
                            for (int i = 0, len = arr.Length; i < len; i++)
                            {
                                item = (T)(await ReadItemAsync(
                                    stream,
                                    version.Value,
                                    valuesNullable,
                                    serializer,
                                    type,
                                    pool,
                                    valueOptions,
                                    syncDeserializer,
                                    asyncDeserializer,
                                    cancellationToken
                                    ).DynamicContext())!;
                                arr.Span[i] = item;
                            }
                        }
                        return arr;
                    }
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="arr">Array</param>
        /// <param name="version">Serializer version</param>
        /// <param name="valueOptions">Value deserializer options</param>
        /// <param name="valuesNullable">Are the values nullable?</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<Array> ReadFixedArrayAsync(
            this Stream stream,
            Array arr,
            int? version = null,
            ISerializerOptions? valueOptions = null,
            bool valuesNullable = false,
            ArrayPool<byte>? pool = null,
            CancellationToken cancellationToken = default
            )
        {
            Type elementType = arr.GetType().GetElementType()!;
            switch ((version ??= StreamSerializer.Version) & byte.MaxValue)// Serializer version switch
            {
                case 1:
                case 2:
                    {
                        for (
                            int i = 0, len = arr.Length;
                            i < len;
                            arr.SetValue(await ReadObjectAsync(stream, elementType, version, valueOptions, cancellationToken).DynamicContext(), i), i++
                            ) ;
                        return arr;
                    }
                default:
                    {
                        Type type = arr.GetType().GetElementType()!;
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
                                for (int i = 0, len = arr.Length; i < len; i++)
                                {
                                    (obj, objType, lastObjType, itemType, itemSerializer, itemSyncDeserializer, itemAsyncDeserializer) =
                                        await ReadAnyItemHeaderAsync(
                                            stream,
                                            version.Value,
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
                                        arr.SetValue(null, i);
                                    }
                                    else if (obj == null)
                                    {
                                        arr.SetValue(obj = itemSerializer == SerializerTypes.Serializer
                                            ? await ReadItemAsync(
                                                stream,
                                                version.Value,
                                                nullable: false,
                                                itemSerializer,
                                                itemType,
                                                pool,
                                                valueOptions,
                                                itemSyncDeserializer,
                                                itemAsyncDeserializer,
                                                cancellationToken
                                                ).DynamicContext()
                                            : await ReadAnyIntAsync(stream, version.Value, objType, itemType, valueOptions, cancellationToken).DynamicContext(),
                                            i);
                                        objIndex = objectCache.IndexOf(null);
                                        if (objIndex != -1) objectCache[objIndex] = obj!;
                                    }
                                    else
                                    {
                                        arr.SetValue(obj, i);
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
                            for (int i = 0, len = arr.Length; i < len; i++)
                                arr.SetValue(await ReadItemAsync(
                                    stream,
                                    version.Value,
                                    valuesNullable,
                                    serializer,
                                    type,
                                    pool,
                                    valueOptions,
                                    syncDeserializer,
                                    asyncDeserializer,
                                    cancellationToken
                                    ).DynamicContext(), i);
                        }
                        return arr;
                    }
            }
        }
    }
}
