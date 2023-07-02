using System.Buffers;
using System.Runtime.CompilerServices;
using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    // Items
    public static partial class StreamExtensions
    {
        /// <summary>
        /// Read an item using a specified serializer
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="nullable">Nullable?</param>
        /// <param name="serializer">Serializer type</param>
        /// <param name="type">Type</param>
        /// <param name="pool">Array pool</param>
        /// <param name="options">Serializer options</param>
        /// <param name="syncDeserializer">Synchronous deserializer</param>
        /// <returns>Stream</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static dynamic? ReadItem(
            Stream stream,
            int version,
            bool nullable,
            SerializerTypes serializer,
            Type? type = null,
            ArrayPool<byte>? pool = null,
            ISerializerOptions? options = null,
            StreamSerializer.Deserialize_Delegate? syncDeserializer = null
            )
        {
            if (!nullable)
            {
                switch (serializer)
                {
                    case SerializerTypes.StreamSerializer: return ReadSerializedObject(stream, SerializerHelper.EnsureNotNull(type, nameof(type)), version);
                    case SerializerTypes.Serializer:
                        {
                            SerializerException.Wrap(() => ArgumentValidationHelper.EnsureValidArgument(nameof(syncDeserializer), syncDeserializer));
                            return SerializerException.Wrap(() => syncDeserializer!(stream, SerializerHelper.EnsureNotNull(type, nameof(type)), version, options))
                                ?? throw new SerializerException($"{type} deserialized to NULL");
                        }
                    case SerializerTypes.Any: return ReadAny(stream, version, options);
                    case SerializerTypes.AnyObject: return ReadAnyObject(stream, SerializerHelper.EnsureNotNull(type, nameof(type)), version);
                    case SerializerTypes.Bool: return ReadBool(stream, version, pool);
                    case SerializerTypes.Number: return ReadNumber(stream, SerializerHelper.EnsureNotNull(type, nameof(type)), version, pool);
                    case SerializerTypes.Enum: return ReadEnum(stream, SerializerHelper.EnsureNotNull(type, nameof(type)), version, pool);
                    case SerializerTypes.String: return ReadString(stream, version, pool, options?.GetMinLen(0) ?? 0, options?.GetMaxLen(int.MaxValue) ?? int.MaxValue);
                    case SerializerTypes.String16: return ReadString16(stream, version, pool, options?.GetMinLen(0) ?? 0, options?.GetMaxLen(int.MaxValue) ?? int.MaxValue);
                    case SerializerTypes.String32: return ReadString32(stream, version, pool, options?.GetMinLen(0) ?? 0, options?.GetMaxLen(int.MaxValue) ?? int.MaxValue);
                    case SerializerTypes.Bytes: return ReadBytes(stream, version, buffer: null, pool, options?.GetMinLen(0) ?? 0, options?.GetMaxLen(int.MaxValue) ?? int.MaxValue).Value;
                    case SerializerTypes.Array: return ReadArray(stream, SerializerHelper.EnsureNotNull(type, nameof(type)), version, pool, options?.GetMinLen(0) ?? 0, options?.GetMaxLen(int.MaxValue) ?? int.MaxValue, options?.ValueOptions);
                    case SerializerTypes.List: return ReadList(stream, SerializerHelper.EnsureNotNull(type, nameof(type)), version, pool, options?.GetMinLen(0) ?? 0, options?.GetMaxLen(int.MaxValue) ?? int.MaxValue, options?.ValueOptions);
                    case SerializerTypes.Dictionary: return ReadDict(stream, SerializerHelper.EnsureNotNull(type, nameof(type)), version, pool, options?.GetMinLen(0) ?? 0, options?.GetMaxLen(int.MaxValue) ?? int.MaxValue, options?.KeyOptions, options?.ValueOptions);
                    case SerializerTypes.Struct:
                        return ReadStructMethod.MakeGenericMethod(SerializerHelper.EnsureNotNull(type, nameof(type))).InvokeAuto(obj: null, stream, version, null, pool)
                            ?? throw new SerializerException($"{nameof(ReadStruct)} serialized {type} to NULL");
                    case SerializerTypes.Stream: return ReadStream(stream, SerializerHelper.EnsureNotNull(options?.Attribute?.GetStream(null, null, stream, version), nameof(ISerializerOptions.Attribute)), version, pool, maxBufferSize: null, options?.GetMinLen(0) ?? 0, options?.GetMaxLen(long.MaxValue) ?? long.MaxValue);
                    default: throw SerializerException.From(new ArgumentException($"Unknown serializer type {serializer}", nameof(serializer)));
                }
            }
            switch (serializer)
            {
                case SerializerTypes.StreamSerializer: return ReadSerializedObjectNullable(stream, SerializerHelper.EnsureNotNull(type, nameof(type)), version);
                case SerializerTypes.Serializer:
                    {
                        if (!ReadBool(stream, version, pool)) return null;
                        SerializerException.Wrap(() => ArgumentValidationHelper.EnsureValidArgument(nameof(syncDeserializer), syncDeserializer));
                        return SerializerException.Wrap(() => syncDeserializer!(stream, SerializerHelper.EnsureNotNull(type, nameof(type)), version, options))
                            ?? throw new SerializerException($"{type} deserialized to NULL");
                    }
                case SerializerTypes.Any: return ReadAnyNullable(stream, version, options);
                case SerializerTypes.AnyObject: return ReadAnyObjectNullable(stream, SerializerHelper.EnsureNotNull(type, nameof(type)), version);
                case SerializerTypes.Bool: return ReadBoolNullable(stream, version, pool);
                case SerializerTypes.Number: return ReadNumberNullable(stream, SerializerHelper.EnsureNotNull(type, nameof(type)), version, pool);
                case SerializerTypes.Enum: return ReadEnumNullable(stream, SerializerHelper.EnsureNotNull(type, nameof(type)), version, pool);
                case SerializerTypes.String: return ReadStringNullable(stream, version, pool, options?.GetMinLen(0) ?? 0, options?.GetMaxLen(int.MaxValue) ?? int.MaxValue);
                case SerializerTypes.String16: return ReadString16Nullable(stream, version, pool, options?.GetMinLen(0) ?? 0, options?.GetMaxLen(int.MaxValue) ?? int.MaxValue);
                case SerializerTypes.String32: return ReadString32Nullable(stream, version, pool, options?.GetMinLen(0) ?? 0, options?.GetMaxLen(int.MaxValue) ?? int.MaxValue);
                case SerializerTypes.Bytes: return ReadBytesNullable(stream, version, buffer: null, pool, options?.GetMinLen(0) ?? 0, options?.GetMaxLen(int.MaxValue) ?? int.MaxValue)?.Value;
                case SerializerTypes.Array: return ReadArrayNullable(stream, SerializerHelper.EnsureNotNull(type, nameof(type)), version, pool, options?.GetMinLen(0) ?? 0, options?.GetMaxLen(int.MaxValue) ?? int.MaxValue, options?.ValueOptions);
                case SerializerTypes.List: return ReadListNullable(stream, SerializerHelper.EnsureNotNull(type, nameof(type)), version, pool, options?.GetMinLen(0) ?? 0, options?.GetMaxLen(int.MaxValue) ?? int.MaxValue, options?.ValueOptions);
                case SerializerTypes.Dictionary: return ReadDictNullable(stream, SerializerHelper.EnsureNotNull(type, nameof(type)), version, pool, options?.GetMinLen(0) ?? 0, options?.GetMaxLen(int.MaxValue) ?? int.MaxValue, options?.KeyOptions, options?.ValueOptions);
                case SerializerTypes.Struct: return ReadStructNullableMethod.MakeGenericMethod(SerializerHelper.EnsureNotNull(type, nameof(type))).InvokeAuto(obj: null, stream, version, null, pool);
                case SerializerTypes.Stream: return ReadStreamNullable(stream, SerializerHelper.EnsureNotNull(options?.Attribute?.GetStream(null, null, stream, version), nameof(ISerializerOptions.Attribute)), version, pool, maxBufferSize: null, options?.GetMinLen(0) ?? 0, options?.GetMaxLen(long.MaxValue) ?? long.MaxValue);
                default: throw SerializerException.From(new ArgumentException($"Unknown serializer type {serializer}", nameof(serializer)));
            }
        }

        /// <summary>
        /// Read an item using a specified serializer
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="nullable">Nullable?</param>
        /// <param name="serializer">Serializer type</param>
        /// <param name="type">Type</param>
        /// <param name="pool">Array pool</param>
        /// <param name="options">Serializer options</param>
        /// <param name="syncDeserializer">Synchronous deserializer</param>
        /// <param name="asyncDeserializer">Asynchronous deserializer</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<dynamic?> ReadItemAsync(
            Stream stream,
            int version,
            bool nullable,
            SerializerTypes serializer,
            Type? type = null,
            ArrayPool<byte>? pool = null,
            ISerializerOptions? options = null,
            StreamSerializer.Deserialize_Delegate? syncDeserializer = null,
            StreamSerializer.AsyncDeserialize_Delegate? asyncDeserializer = null,
            CancellationToken cancellationToken = default
            )
        {
            if (!nullable)
            {
                switch (serializer)
                {
                    case SerializerTypes.StreamSerializer: return await ReadSerializedObjectAsync(stream, SerializerHelper.EnsureNotNull(type, nameof(type)), version, cancellationToken).DynamicContext();
                    case SerializerTypes.Serializer:
                        {
                            if (asyncDeserializer == null)
                            {
                                SerializerException.Wrap(() => ArgumentValidationHelper.EnsureValidArgument(nameof(syncDeserializer), syncDeserializer));
                                return SerializerException.Wrap(() => syncDeserializer!(stream, SerializerHelper.EnsureNotNull(type, nameof(type)), version, options))
                                    ?? throw new SerializerException($"{type} deserialized to NULL");
                            }
                            else
                            {
                                Task task = SerializerException.WrapAsync(() => asyncDeserializer!(stream, SerializerHelper.EnsureNotNull(type, nameof(type)), version, options, cancellationToken));
                                await task.DynamicContext();
                                return task.GetResult(type!);
                            }
                        }
                    case SerializerTypes.Any: return await ReadAnyAsync(stream, version, options, cancellationToken).DynamicContext();
                    case SerializerTypes.AnyObject: return await ReadAnyObjectAsync(stream, SerializerHelper.EnsureNotNull(type, nameof(type)), version, cancellationToken).DynamicContext();
                    case SerializerTypes.Bool: return await ReadBoolAsync(stream, version, pool, cancellationToken).DynamicContext();
                    case SerializerTypes.Number: return await ReadNumberAsync(stream, SerializerHelper.EnsureNotNull(type, nameof(type)), version, pool, cancellationToken).DynamicContext();
                    case SerializerTypes.Enum: return await ReadEnumAsync(stream, SerializerHelper.EnsureNotNull(type, nameof(type)), version, pool, cancellationToken).DynamicContext();
                    case SerializerTypes.String: return await ReadStringAsync(stream, version, pool, options?.GetMinLen(0) ?? 0, options?.GetMaxLen(int.MaxValue) ?? int.MaxValue, cancellationToken).DynamicContext();
                    case SerializerTypes.String16: return await ReadString16Async(stream, version, pool, options?.GetMinLen(0) ?? 0, options?.GetMaxLen(int.MaxValue) ?? int.MaxValue, cancellationToken).DynamicContext();
                    case SerializerTypes.String32: return await ReadString32Async(stream, version, pool, options?.GetMinLen(0) ?? 0, options?.GetMaxLen(int.MaxValue) ?? int.MaxValue, cancellationToken).DynamicContext();
                    case SerializerTypes.Bytes: return (await ReadBytesAsync(stream, version, buffer: null, pool, options?.GetMinLen(0) ?? 0, options?.GetMaxLen(int.MaxValue) ?? int.MaxValue, cancellationToken).DynamicContext()).Value;
                    case SerializerTypes.Array: return await ReadArrayAsync(stream, SerializerHelper.EnsureNotNull(type, nameof(type)), version, pool, options?.GetMinLen(0) ?? 0, options?.GetMaxLen(int.MaxValue) ?? int.MaxValue, options?.ValueOptions, cancellationToken: cancellationToken).DynamicContext();
                    case SerializerTypes.List: return await ReadListAsync(stream, SerializerHelper.EnsureNotNull(type, nameof(type)), version, pool, options?.GetMinLen(0) ?? 0, options?.GetMaxLen(int.MaxValue) ?? int.MaxValue, options?.ValueOptions, cancellationToken: cancellationToken).DynamicContext();
                    case SerializerTypes.Dictionary: return await ReadDictAsync(stream, SerializerHelper.EnsureNotNull(type, nameof(type)), version, pool, options?.GetMinLen(0) ?? 0, options?.GetMaxLen(int.MaxValue) ?? int.MaxValue, options?.KeyOptions, options?.ValueOptions, cancellationToken: cancellationToken).DynamicContext();
                    case SerializerTypes.Struct:
                        return ReadStructMethod.MakeGenericMethod(SerializerHelper.EnsureNotNull(type, nameof(type))).InvokeAuto(obj: null, stream, version, null, pool)
                            ?? throw new SerializerException($"{nameof(ReadStruct)} serialized {type} to NULL");
                    case SerializerTypes.Stream: return await ReadStreamAsync(stream, SerializerHelper.EnsureNotNull(options?.Attribute?.GetStream(null, null, stream, version, cancellationToken), nameof(ISerializerOptions.Attribute)), version, pool, maxBufferSize: null, options?.GetMinLen(0) ?? 0, options?.GetMaxLen(long.MaxValue) ?? long.MaxValue, cancellationToken).DynamicContext();
                    default: throw SerializerException.From(new ArgumentException($"Unknown serializer type {serializer}", nameof(serializer)));
                }
            }
            switch (serializer)
            {
                case SerializerTypes.StreamSerializer: return await ReadSerializedObjectNullableAsync(stream, SerializerHelper.EnsureNotNull(type, nameof(type)), version, cancellationToken).DynamicContext();
                case SerializerTypes.Serializer:
                    {
                        if (!await ReadBoolAsync(stream, version, pool, cancellationToken: cancellationToken).DynamicContext()) return null;
                        if (asyncDeserializer == null)
                        {
                            SerializerException.Wrap(() => ArgumentValidationHelper.EnsureValidArgument(nameof(syncDeserializer), syncDeserializer));
                            return SerializerException.Wrap(() => syncDeserializer!(stream, SerializerHelper.EnsureNotNull(type, nameof(type)), version, options))
                                ?? throw new SerializerException($"{type} deserialized to NULL");
                        }
                        else
                        {
                            Task task = SerializerException.WrapAsync(() => asyncDeserializer!(stream, SerializerHelper.EnsureNotNull(type, nameof(type)), version, options, cancellationToken));
                            await task.DynamicContext();
                            return task.GetResult(type!);
                        }
                    }
                case SerializerTypes.Any: return await ReadAnyNullableAsync(stream, version, options, cancellationToken).DynamicContext();
                case SerializerTypes.AnyObject: return await ReadAnyObjectNullableAsync(stream, SerializerHelper.EnsureNotNull(type, nameof(type)), version, cancellationToken).DynamicContext();
                case SerializerTypes.Bool: return await ReadBoolNullableAsync(stream, version, pool, cancellationToken).DynamicContext();
                case SerializerTypes.Number: return await ReadNumberNullableAsync(stream, SerializerHelper.EnsureNotNull(type, nameof(type)), version, pool, cancellationToken).DynamicContext();
                case SerializerTypes.Enum: return await ReadEnumNullableAsync(stream, SerializerHelper.EnsureNotNull(type, nameof(type)), version, pool, cancellationToken).DynamicContext();
                case SerializerTypes.String: return await ReadStringNullableAsync(stream, version, pool, options?.GetMinLen(0) ?? 0, options?.GetMaxLen(int.MaxValue) ?? int.MaxValue, cancellationToken).DynamicContext();
                case SerializerTypes.String16: return await ReadString16NullableAsync(stream, version, pool, options?.GetMinLen(0) ?? 0, options?.GetMaxLen(int.MaxValue) ?? int.MaxValue, cancellationToken).DynamicContext();
                case SerializerTypes.String32: return await ReadString32NullableAsync(stream, version, pool, options?.GetMinLen(0) ?? 0, options?.GetMaxLen(int.MaxValue) ?? int.MaxValue, cancellationToken).DynamicContext();
                case SerializerTypes.Bytes: return (await ReadBytesNullableAsync(stream, version, buffer: null, pool, options?.GetMinLen(0) ?? 0, options?.GetMaxLen(int.MaxValue) ?? int.MaxValue, cancellationToken).DynamicContext())?.Value;
                case SerializerTypes.Array: return await ReadArrayNullableAsync(stream, SerializerHelper.EnsureNotNull(type, nameof(type)), version, pool, options?.GetMinLen(0) ?? 0, options?.GetMaxLen(int.MaxValue) ?? int.MaxValue, options?.ValueOptions, cancellationToken: cancellationToken).DynamicContext();
                case SerializerTypes.List: return await ReadListNullableAsync(stream, SerializerHelper.EnsureNotNull(type, nameof(type)), version, pool, options?.GetMinLen(0) ?? 0, options?.GetMaxLen(int.MaxValue) ?? int.MaxValue, options?.ValueOptions, cancellationToken: cancellationToken).DynamicContext();
                case SerializerTypes.Dictionary: return await ReadDictNullableAsync(stream, SerializerHelper.EnsureNotNull(type, nameof(type)), version, pool, options?.GetMinLen(0) ?? 0, options?.GetMaxLen(int.MaxValue) ?? int.MaxValue, options?.KeyOptions, options?.ValueOptions, cancellationToken: cancellationToken).DynamicContext();
                case SerializerTypes.Struct:
                    return ReadStructNullableMethod.MakeGenericMethod(SerializerHelper.EnsureNotNull(type, nameof(type))).InvokeAuto(obj: null, stream, version, null, pool)
                        ?? throw new SerializerException($"{nameof(ReadStructNullable)} serialized {type} to NULL");
                case SerializerTypes.Stream: return await ReadStreamNullableAsync(stream, SerializerHelper.EnsureNotNull(options?.Attribute?.GetStream(null, null, stream, version, cancellationToken), nameof(ISerializerOptions.Attribute)), version, pool, maxBufferSize: null, options?.GetMinLen(0) ?? 0, options?.GetMaxLen(long.MaxValue) ?? long.MaxValue, cancellationToken).DynamicContext();
                default: throw SerializerException.From(new ArgumentException($"Unknown serializer type {serializer}", nameof(serializer)));
            }
        }

        /// <summary>
        /// Read an item header, if the final item type is not specified
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="elementType">Item element type</param>
        /// <param name="index">Item index</param>
        /// <param name="typeCache">Type cache</param>
        /// <param name="objectCache">Object cache</param>
        /// <param name="objType">Object type</param>
        /// <param name="lastObjType">Last object type</param>
        /// <param name="itemType">Item type</param>
        /// <param name="itemSerializer">Item serializer type</param>
        /// <param name="itemSyncDeserializer">Synchronous item deserializer</param>
        /// <returns>Cached object</returns>
        public static object? ReadAnyItemHeader(
            Stream stream,
            int version,
            Type elementType,
            int index,
            Span<Type> typeCache,
            ReadOnlySpan<object> objectCache,
            ref ObjectTypes objType,
            ref ObjectTypes lastObjType,
            ref Type? itemType,
            ref SerializerTypes itemSerializer,
            ref StreamSerializer.Deserialize_Delegate? itemSyncDeserializer
            )
        {
            bool requireType;
            // Read the object type
            objType = (ObjectTypes)ReadOneByte(stream, version);
            if (objType == ObjectTypes.Null) return null;
            // Use the object cache
            if (objType == ObjectTypes.Cached)
            {
                int objIndex = ReadOneByte(stream, version);
                object? res = objectCache[objIndex] ?? throw new SerializerException($"Invalid object cache index #{objIndex}", new InvalidDataException());
                itemType = res.GetType();
                objType = itemType.GetObjectSerializerInfo().ObjectType;
                (itemSerializer, itemSyncDeserializer, _) = itemType.GetItemDeserializerInfo(isAsync: false);
                lastObjType = objType;
                return res;
            }
            // Prepare the deserialization
            lastObjType = objType.RequiresObjectWriting() ? objType : default;
            if (objType == ObjectTypes.LastItemType)
            {
                // Use the last object type
                if (index == 0) throw new SerializerException($"Invalid object type for item #{index}", new InvalidDataException());
                requireType = false;
            }
            else
            {
                // Ensure correct deserializer informations
                requireType = objType.RequiresTypeName();
                if (requireType)
                {
                    // An object type is required
                    if (objType.IsCached())
                    {
                        // Use a previously cached object type
                        int typeIndex = ReadOneByte(stream, version);
                        itemType = typeCache[typeIndex] ?? throw new SerializerException($"No type at cache index #{typeIndex}", new InvalidDataException());
                        objType &= ~ObjectTypes.Cached;
                    }
                    else
                    {
                        // Read the object type
                        itemType = ReadSerializableType(stream, version);
                        if (!elementType.IsAssignableFrom(itemType) || itemType.IsAbstract || itemType.IsInterface || itemType == typeof(object))
                            throw new SerializerException($"Invalid item type {itemType} for item #{index} ({elementType})", new InvalidCastException());
                        int typeIndex = typeCache.AsReadOnly().IndexOf(null!);
                        if (typeIndex != -1) typeCache[typeIndex] = itemType;
                        (itemSerializer, itemSyncDeserializer, _) = itemType.GetItemDeserializerInfo(isAsync: false);
                    }
                }
                else
                {
                    // No object type is required
                    itemType = null;
                    itemSerializer = SerializerTypes.Any;
                }
            }
            // Ensure having a valid type serializer configuration
            if (requireType && itemType == null)
                throw new SerializerException($"Serialized type name expected for item #{index}", new InvalidDataException());
            return null;
        }

        /// <summary>
        /// Read an item header, if the final item type is not specified
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="elementType">Item element type</param>
        /// <param name="index">Item index</param>
        /// <param name="typeCache">Type cache</param>
        /// <param name="objectCache">Object cache</param>
        /// <param name="lastObjType">Last object type</param>
        /// <param name="itemType">Item type</param>
        /// <param name="itemSerializer">Item serializer type</param>
        /// <param name="itemSyncDeserializer">Synchronous item deserializer</param>
        /// <param name="itemAsyncDeserializer">Asynchronous item deserializer</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Item informations</returns>
        public static async Task<(
            object? Object,
            ObjectTypes ObjectType,
            ObjectTypes LastObjType,
            Type? ItemType,
            SerializerTypes ItemSerializer,
            StreamSerializer.Deserialize_Delegate? ItemSyncDesSerializer,
            StreamSerializer.AsyncDeserialize_Delegate? ItemAsyncDeserializer
            )> ReadAnyItemHeaderAsync(
            Stream stream,
            int version,
            Type elementType,
            int index,
            Memory<Type> typeCache,
            ReadOnlyMemory<object> objectCache,
            ObjectTypes lastObjType,
            Type? itemType,
            SerializerTypes itemSerializer,
            StreamSerializer.Deserialize_Delegate? itemSyncDeserializer,
            StreamSerializer.AsyncDeserialize_Delegate? itemAsyncDeserializer,
            CancellationToken cancellationToken
            )
        {
            bool requireType;
            // Read the object type
            ObjectTypes objType = (ObjectTypes)await ReadOneByteAsync(stream, version, cancellationToken).DynamicContext();
            if (objType == ObjectTypes.Null) return (Object: null, objType, lastObjType, itemType, itemSerializer, itemSyncDeserializer, itemAsyncDeserializer);
            // Use the object cache
            if (objType == ObjectTypes.Cached)
            {
                int objIndex = await ReadOneByteAsync(stream, version, cancellationToken).DynamicContext();
                object? res = objectCache.Span[objIndex] ?? throw new SerializerException($"Invalid object cache index #{objIndex}", new InvalidDataException());
                itemType = res.GetType();
                objType = itemType.GetObjectSerializerInfo().ObjectType;
                (itemSerializer, itemSyncDeserializer, itemAsyncDeserializer) = itemType.GetItemDeserializerInfo(isAsync: false);
                lastObjType = objType;
                return (res, objType, lastObjType, itemType, itemSerializer, itemSyncDeserializer, itemAsyncDeserializer);
            }
            // Prepare the deserialization
            lastObjType = objType.RequiresObjectWriting() ? objType : default;
            if (objType == ObjectTypes.LastItemType)
            {
                // Use the last object type
                if (index == 0) throw new SerializerException($"Invalid object type for item #{index}", new InvalidDataException());
                requireType = false;
            }
            else
            {
                // Ensure correct deserializer informations
                requireType = objType.RequiresTypeName();
                if (requireType)
                {
                    // An object type (name) is required
                    if (objType.IsCached())
                    {
                        // Use a previously cached object type
                        int typeIndex = await ReadOneByteAsync(stream, version, cancellationToken).DynamicContext();
                        itemType = typeCache.Span[typeIndex] ?? throw new SerializerException($"No type at cache index #{typeIndex}", new InvalidDataException());
                        objType &= ~ObjectTypes.Cached;
                    }
                    else
                    {
                        // Read the object type name
                        itemType = await ReadSerializableTypeAsync(stream, version, cancellationToken).DynamicContext();
                        if (!elementType.IsAssignableFrom(itemType) || itemType.IsAbstract || itemType.IsInterface || itemType == typeof(object))
                            throw new SerializerException($"Invalid item type {itemType} for item #{index} ({elementType})", new InvalidCastException());
                        int typeIndex = typeCache.AsReadOnly().IndexOf(null!);
                        if (typeIndex != -1) typeCache.Span[typeIndex] = itemType;
                        (itemSerializer, itemSyncDeserializer, itemAsyncDeserializer) = itemType.GetItemDeserializerInfo(isAsync: true);
                    }
                }
                else
                {
                    // No object type name is required
                    itemType = null;
                    itemSerializer = SerializerTypes.Any;
                }
            }
            // Ensure having a valid type serializer configuration
            if (requireType && itemType == null)
                throw new SerializerException($"Serialized type expected for item #{index}", new InvalidDataException());
            return (Object: null, objType, lastObjType, itemType, itemSerializer, itemSyncDeserializer, itemAsyncDeserializer);
        }
    }
}
