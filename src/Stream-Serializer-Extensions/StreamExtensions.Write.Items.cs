using System.Collections;
using System.Runtime.CompilerServices;
using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    // Items
    public static partial class StreamExtensions
    {
        /// <summary>
        /// Write an item using a specified serializer
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="item">Item</param>
        /// <param name="nullable">Nullable?</param>
        /// <param name="serializer">Serializer type</param>
        /// <param name="syncSerializer">Synchronous serializer</param>
        /// <returns>Stream</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream WriteItem(
            Stream stream,
            dynamic item,
            bool nullable,
            SerializerTypes serializer,
            StreamSerializer.Serialize_Delegate? syncSerializer = null
            )
        {
            if (!nullable)
            {
                switch (serializer)
                {
                    case SerializerTypes.StreamSerializer: return WriteSerialized(stream, (IStreamSerializer)item);
                    case SerializerTypes.Serializer:
                        {
                            SerializerException.Wrap(() => ArgumentValidationHelper.EnsureValidArgument(nameof(syncSerializer), syncSerializer));
                            object obj = item;
                            SerializerException.Wrap(() => syncSerializer!(stream, obj));
                            return stream;
                        }
                    case SerializerTypes.Any: return WriteAny(stream, item);
                    case SerializerTypes.AnyObject: return WriteAnyObject(stream, item);
                    case SerializerTypes.Bool: return Write(stream, (bool)item);
                    case SerializerTypes.Number: return WriteNumber(stream, item);
                    case SerializerTypes.Enum: return WriteEnum(stream, (Enum)item);
                    case SerializerTypes.String: return WriteString(stream, (string)item);
                    case SerializerTypes.String16: return WriteString16(stream, (string)item);
                    case SerializerTypes.String32: return WriteString32(stream, (string)item);
                    case SerializerTypes.Bytes: return WriteBytes(stream, (byte[])item);
                    case SerializerTypes.Array: return WriteArray(stream, (Array)item);
                    case SerializerTypes.List: return WriteList(stream, (IList)item);
                    case SerializerTypes.Dictionary: return WriteDict(stream, (IDictionary)item);
                    case SerializerTypes.Struct: return WriteStruct(stream, item);
                    case SerializerTypes.Stream: return WriteStream(stream, (Stream)item);
                    default: throw SerializerException.From(new ArgumentException($"Unknown serializer type {serializer}", nameof(serializer)));
                }
            }
            switch (serializer)
            {
                case SerializerTypes.StreamSerializer: return WriteSerializedNullable(stream, (IStreamSerializer?)item);
                case SerializerTypes.Serializer:
                    {
                        SerializerException.Wrap(() => ArgumentValidationHelper.EnsureValidArgument(nameof(syncSerializer), syncSerializer));
                        object? obj = item;
                        return WriteIfNotNull(stream, obj, () => syncSerializer!(stream, obj));
                    }
                case SerializerTypes.Any: return WriteAnyNullable(stream, item);
                case SerializerTypes.AnyObject: return WriteAnyObjectNullable(stream, item);
                case SerializerTypes.Bool: return WriteNullable(stream, (bool?)item);
                case SerializerTypes.Number: return WriteNumberNullable(stream, item);
                case SerializerTypes.Enum: return WriteEnumNullable(stream, (Enum?)item);
                case SerializerTypes.String: return WriteStringNullable(stream, (string?)item);
                case SerializerTypes.String16: return WriteString16Nullable(stream, (string?)item);
                case SerializerTypes.String32: return WriteString32Nullable(stream, (string?)item);
                case SerializerTypes.Bytes: return WriteBytesNullable(stream, (byte[]?)item);
                case SerializerTypes.Array: return WriteArrayNullable(stream, (Array?)item);
                case SerializerTypes.List: return WriteListNullable(stream, (IList?)item);
                case SerializerTypes.Dictionary: return WriteDictNullable(stream, (IDictionary?)item);
                case SerializerTypes.Struct: return WriteStructNullable(stream, item);
                case SerializerTypes.Stream: return WriteStreamNullable(stream, (Stream?)item);
                default: throw SerializerException.From(new ArgumentException($"Unknown serializer type {serializer}", nameof(serializer)));
            }
        }

        /// <summary>
        /// Write an item using a specified serializer
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="item">Item</param>
        /// <param name="nullable">Nullable?</param>
        /// <param name="serializer">Serializer type</param>
        /// <param name="syncSerializer">Synchronous serializer</param>
        /// <param name="asyncSerializer">Asynchronous serializer</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<Stream> WriteItemAsync(
            Stream stream,
            dynamic item,
            bool nullable,
            SerializerTypes serializer,
            StreamSerializer.Serialize_Delegate? syncSerializer = null,
            StreamSerializer.AsyncSerialize_Delegate? asyncSerializer = null,
            CancellationToken cancellationToken = default
            )
        {
            if (!nullable)
            {
                switch (serializer)
                {
                    case SerializerTypes.StreamSerializer: return await WriteSerializedAsync(stream, (IStreamSerializer)item, cancellationToken).DynamicContext();
                    case SerializerTypes.Serializer:
                        {
                            object obj = item;
                            if (asyncSerializer == null)
                            {
                                await Task.Yield();
                                SerializerException.Wrap(() => ArgumentValidationHelper.EnsureValidArgument(nameof(syncSerializer), syncSerializer));
                                SerializerException.Wrap(() => syncSerializer!(stream, obj));
                            }
                            else
                            {
                                SerializerException.Wrap(() => ArgumentValidationHelper.EnsureValidArgument(nameof(asyncSerializer), asyncSerializer));
                                await SerializerException.WrapAsync(() => asyncSerializer(stream, obj, cancellationToken)).DynamicContext();
                            }
                            return stream;
                        }
                    case SerializerTypes.Any: return await WriteAnyAsync(stream, (object)item, cancellationToken).DynamicContext();
                    case SerializerTypes.AnyObject: return await WriteAnyObjectAsync(stream, (object)item, cancellationToken).DynamicContext();
                    case SerializerTypes.Bool: return await WriteAsync(stream, (bool)item, cancellationToken).DynamicContext();
                    case SerializerTypes.Number: return await WriteNumberAsync(stream, (object)item, cancellationToken).DynamicContext();
                    case SerializerTypes.Enum: return await WriteEnumAsync(stream, (Enum)item, cancellationToken).DynamicContext();
                    case SerializerTypes.String: return await WriteStringAsync(stream, (string)item, cancellationToken).DynamicContext();
                    case SerializerTypes.String16: return await WriteString16Async(stream, (string)item, cancellationToken).DynamicContext();
                    case SerializerTypes.String32: return await WriteString32Async(stream, (string)item, cancellationToken).DynamicContext();
                    case SerializerTypes.Bytes: return await WriteBytesAsync(stream, (byte[])item, cancellationToken).DynamicContext();
                    case SerializerTypes.Array: return await WriteArrayAsync(stream, (Array)item, cancellationToken: cancellationToken).DynamicContext();
                    case SerializerTypes.List: return await WriteListAsync(stream, (IList)item, cancellationToken: cancellationToken).DynamicContext();
                    case SerializerTypes.Dictionary: return await WriteDictAsync(stream, (IDictionary)item, cancellationToken: cancellationToken).DynamicContext();
                    case SerializerTypes.Struct: return await WriteStructAsync(stream, (object)item, cancellationToken: cancellationToken).DynamicContext();
                    case SerializerTypes.Stream: return await WriteStreamAsync(stream, (Stream)item, cancellationToken: cancellationToken).DynamicContext();
                    default: throw SerializerException.From(new ArgumentException($"Unknown serializer type {serializer}", nameof(serializer)));
                }
            }
            switch (serializer)
            {
                case SerializerTypes.StreamSerializer: return await WriteSerializedNullableAsync(stream, (IStreamSerializer?)item, cancellationToken).DynamicContext();
                case SerializerTypes.Serializer:
                    {
                        object? obj = item!;
                        if (asyncSerializer == null)
                        {
                            SerializerException.Wrap(() => ArgumentValidationHelper.EnsureValidArgument(nameof(syncSerializer), syncSerializer));
                            Task action()
                            {
                                syncSerializer!(stream, obj);
                                return Task.CompletedTask;
                            }
                            return await WriteIfNotNullAsync(stream, obj, action, cancellationToken).DynamicContext();
                        }
                        else
                        {
                            SerializerException.Wrap(() => ArgumentValidationHelper.EnsureValidArgument(nameof(asyncSerializer), asyncSerializer));
                            return await WriteIfNotNullAsync(stream, obj, () => asyncSerializer!(stream, obj, cancellationToken), cancellationToken).DynamicContext();
                        }
                    }
                case SerializerTypes.Any: return await WriteAnyNullableAsync(stream, (object?)item, cancellationToken).DynamicContext();
                case SerializerTypes.AnyObject: return await WriteAnyObjectNullableAsync(stream, (object?)item, cancellationToken).DynamicContext();
                case SerializerTypes.Bool: return await WriteNullableAsync(stream, (bool?)item, cancellationToken).DynamicContext();
                case SerializerTypes.Number: return await WriteNumberNullableAsync(stream, (object?)item, cancellationToken).DynamicContext();
                case SerializerTypes.Enum: return await WriteEnumNullableAsync(stream, (Enum?)item, cancellationToken).DynamicContext();
                case SerializerTypes.String: return await WriteStringNullableAsync(stream, (string?)item, cancellationToken).DynamicContext();
                case SerializerTypes.String16: return await WriteString16NullableAsync(stream, (string?)item, cancellationToken).DynamicContext();
                case SerializerTypes.String32: return await WriteString32NullableAsync(stream, (string?)item, cancellationToken).DynamicContext();
                case SerializerTypes.Bytes: return await WriteBytesNullableAsync(stream, (byte[]?)item, cancellationToken).DynamicContext();
                case SerializerTypes.Array: return await WriteArrayNullableAsync(stream, (Array?)item, cancellationToken: cancellationToken).DynamicContext();
                case SerializerTypes.List: return await WriteListNullableAsync(stream, (IList?)item, cancellationToken: cancellationToken).DynamicContext();
                case SerializerTypes.Dictionary: return await WriteDictNullableAsync(stream, (IDictionary?)item, cancellationToken: cancellationToken).DynamicContext();
                case SerializerTypes.Struct: return await WriteStructNullableAsync(stream, (object?)item, cancellationToken: cancellationToken).DynamicContext();
                case SerializerTypes.Stream: return await WriteStreamNullableAsync(stream, (Stream?)item, cancellationToken: cancellationToken).DynamicContext();
                default: throw SerializerException.From(new ArgumentException($"Unknown serializer type {serializer}", nameof(serializer)));
            }
        }

        /// <summary>
        /// Write an item header, if the used item type isn't specified
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="item">Item</param>
        /// <param name="itemType">Item type</param>
        /// <param name="typeCache">Type cache</param>
        /// <param name="objectCache">Object cache</param>
        /// <param name="lastItemType">Last item type</param>
        /// <param name="itemSerializer">Item serializer type</param>
        /// <param name="itemSyncSerializer">Item synchronous serializer</param>
        /// <param name="objType">Object type</param>
        /// <param name="writeObject">Write the object?</param>
        /// <returns>All information written?</returns>
        public static bool WriteAnyItemHeader(
            Stream stream,
            dynamic item,
            Type itemType,
            Span<int> typeCache,
            Span<int> objectCache,
            ref Type? lastItemType,
            ref SerializerTypes itemSerializer,
            ref StreamSerializer.Serialize_Delegate? itemSyncSerializer,
            ref ObjectTypes objType,
            ref bool writeObject
            )
        {
            // Use the object cache
            var info = ((object)item).GetObjectSerializerInfo();
            if (info.WriteObject)
            {
                int ohc = item.GetHashCode(),
                objIndex = objectCache.IndexOf(ohc);
                if (objIndex != -1)
                {
                    objType = ObjectTypes.Cached;
                    Write(stream, (byte)objType);
                    Write(stream, (byte)objIndex);
                    objType = info.ObjectType;
                    writeObject = info.WriteObject;
                    (itemSerializer, itemSyncSerializer, _) = itemType.GetItemSerializerInfo(isAsync: false);
                    lastItemType = writeObject ? itemType : null;
                    return true;
                }
                else
                {
                    objIndex = objectCache.IndexOf(0);
                    if (objIndex != -1) objectCache[objIndex] = ohc;
                }
            }
            // Write the type information
            if (itemType == lastItemType)
            {
                // Use the last object type
                Write(stream, (byte)ObjectTypes.LastItemType);
            }
            else
            {
                // Write object type details
                objType = info.ObjectType;
                writeObject = info.WriteObject;
                (itemSerializer, itemSyncSerializer, _) = itemType.GetItemSerializerInfo(isAsync: false);
                if (info.WriteType)
                {
                    // Write type detail informations
                    int thc = itemType.GetHashCode(),
                        typeIndex = typeCache.IndexOf(thc);
                    if (typeIndex != -1)
                    {
                        // Use the cached type
                        objType |= ObjectTypes.Cached;
                        Write(stream, (byte)objType);
                        Write(stream, (byte)typeIndex);
                    }
                    else
                    {
                        // Update the cache
                        typeIndex = typeCache.IndexOf(0);
                        if (typeIndex != -1) typeCache[typeIndex] = thc;
                        // Write the type informations
                        Write(stream, (byte)objType);
                        Write(stream, itemType);
                    }
                }
                else
                {
                    // Write the type informations
                    Write(stream, (byte)objType);
                    itemSerializer = SerializerTypes.Any;
                }
                lastItemType = writeObject ? itemType : null;
            }
            return !writeObject;
        }

        /// <summary>
        /// Write an item header, if the used item type isn't specified
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="item">Item</param>
        /// <param name="itemType">Item type</param>
        /// <param name="typeCache">Type cache</param>
        /// <param name="objectCache">Object cache</param>
        /// <param name="lastItemType">Last item type</param>
        /// <param name="itemSerializer">Item serializer type</param>
        /// <param name="itemSyncSerializer">Item synchronous serializer</param>
        /// <param name="itemAsyncSerializer">Item asynchronous serializer</param>
        /// <param name="objType">Object type</param>
        /// <param name="writeObject">Write the object?</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static async Task<(
            bool Complete,
            Type? LastItemType,
            SerializerTypes ItemSerializer,
            StreamSerializer.Serialize_Delegate? ItemSyncSerializer,
            StreamSerializer.AsyncSerialize_Delegate? ItemAsyncSerializer,
            ObjectTypes ObjType,
            bool WriteObject
            )> WriteAnyItemHeaderAsync(
            Stream stream,
            dynamic item,
            Type itemType,
            Memory<int> typeCache,
            Memory<int> objectCache,
            Type? lastItemType,
            SerializerTypes itemSerializer,
            StreamSerializer.Serialize_Delegate? itemSyncSerializer,
            StreamSerializer.AsyncSerialize_Delegate? itemAsyncSerializer,
            ObjectTypes objType,
            bool writeObject,
            CancellationToken cancellationToken
            )
        {
            // Use the object cache
            var info = ((object)item).GetObjectSerializerInfo();
            if (info.WriteObject)
            {
                int ohc = item.GetHashCode(),
                    objIndex = objectCache.IndexOf(ohc);
                if (objIndex != -1)
                {
                    objType = ObjectTypes.Cached;
                    await WriteAsync(stream, (byte)objType, cancellationToken).DynamicContext();
                    await WriteAsync(stream, (byte)objIndex, cancellationToken).DynamicContext();
                    objType = info.ObjectType;
                    writeObject = info.WriteObject;
                    (itemSerializer, itemSyncSerializer, itemAsyncSerializer) = itemType.GetItemSerializerInfo(isAsync: true);
                    lastItemType = writeObject ? itemType : null;
                    return (Complete: true, lastItemType, itemSerializer, itemSyncSerializer, itemAsyncSerializer, objType, writeObject);
                }
                else
                {
                    objIndex = objectCache.Span.IndexOf(0);
                    if (objIndex != -1) objectCache.Span[objIndex] = ohc;
                }
            }
            // Write the type information
            if (itemType == lastItemType)
            {
                // Use the last object type
                await WriteAsync(stream, (byte)ObjectTypes.LastItemType, cancellationToken).DynamicContext();
            }
            else
            {
                // Write object type details
                objType = info.ObjectType;
                writeObject = info.WriteObject;
                (itemSerializer, itemSyncSerializer, itemAsyncSerializer) = itemType.GetItemSerializerInfo(isAsync: true);
                if (info.WriteType)
                {
                    // Write type detail informations
                    int thc = itemType.GetHashCode(),
                        typeIndex = typeCache.Span.IndexOf(thc);
                    if (typeIndex != -1)
                    {
                        // Use the cached type
                        objType |= ObjectTypes.Cached;
                        await WriteAsync(stream, (byte)objType, cancellationToken).DynamicContext();
                        await WriteAsync(stream, (byte)typeIndex, cancellationToken).DynamicContext();
                    }
                    else
                    {
                        // Update the cache
                        typeIndex = typeCache.Span.IndexOf(0);
                        if (typeIndex != -1) typeCache.Span[typeIndex] = thc;
                        // Write the type informations
                        await WriteAsync(stream, (byte)objType, cancellationToken).DynamicContext();
                        await WriteAsync(stream, itemType, cancellationToken).DynamicContext();
                    }
                }
                else
                {
                    // Write the type informations
                    await WriteAsync(stream, (byte)objType, cancellationToken).DynamicContext();
                    itemSerializer = SerializerTypes.Any;
                }
                lastItemType = writeObject ? itemType : null;
            }
            return (Complete: !writeObject, lastItemType, itemSerializer, itemSyncSerializer, itemAsyncSerializer, objType, writeObject);
        }
    }
}
