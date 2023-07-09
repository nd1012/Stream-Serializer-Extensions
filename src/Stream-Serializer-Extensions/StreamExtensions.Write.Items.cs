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
        /// <param name="context">Context</param>
        /// <param name="item">Item</param>
        /// <returns>Stream</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream WriteItem(ItemSerializerContext context, dynamic item)
        {
            ISerializationContext sc = context.Context;
            if (!context.Nullable)
            {
                switch (context.ItemSerializer)
                {
                    case SerializerTypes.StreamSerializer: return WriteSerialized(sc.Stream, (IStreamSerializer)item, sc);
                    case SerializerTypes.Serializer:
                        {
                            SerializerException.Wrap(() =>
                                ArgumentValidationHelper.EnsureValidArgument(nameof(context), context.ItemSyncSerializer, () => "Missing synchronous serializer")
                                );
                            object obj = item;
                            SerializerException.Wrap(() => context.ItemSyncSerializer!(sc, obj));
                            return sc.Stream;
                        }
                    case SerializerTypes.Any: return WriteAny(sc.Stream, item, sc);
                    case SerializerTypes.AnyObject: return WriteAnyObject(sc.Stream, item, sc);
                    case SerializerTypes.Bool: return Write(sc.Stream, (bool)item, sc);
                    case SerializerTypes.Number: return WriteNumber(sc.Stream, item, sc);
                    case SerializerTypes.Enum: return WriteEnum(sc.Stream, (Enum)item, sc);
                    case SerializerTypes.String: return WriteString(sc.Stream, (string)item, sc);
                    case SerializerTypes.String16: return WriteString16(sc.Stream, (string)item, sc);
                    case SerializerTypes.String32: return WriteString32(sc.Stream, (string)item, sc);
                    case SerializerTypes.Bytes: return WriteBytes(sc.Stream, (byte[])item, sc);
                    case SerializerTypes.Array: return WriteArray(sc.Stream, (Array)item, sc);
                    case SerializerTypes.List: return WriteList(sc.Stream, (IList)item, sc);
                    case SerializerTypes.Dictionary: return WriteDict(sc.Stream, (IDictionary)item, sc);
                    case SerializerTypes.Struct: return WriteStruct(sc.Stream, item, sc);
                    case SerializerTypes.Stream: return WriteStream(sc.Stream, (Stream)item, sc);
                    case SerializerTypes.Type: return Write(sc.Stream, (Type)item, sc);
                    default: throw SerializerException.From(new ArgumentException($"Unknown serializer type {context.ItemSerializer}", nameof(context)));
                }
            }
            switch (context.ItemSerializer)
            {
                case SerializerTypes.StreamSerializer: return WriteSerializedNullable(sc.Stream, (IStreamSerializer?)item, sc);
                case SerializerTypes.Serializer:
                    {
                        SerializerException.Wrap(() => ArgumentValidationHelper.EnsureValidArgument(nameof(context), context.ItemSyncSerializer, () => "Missing synchronous serializer"));
                        object? obj = item;
                        return WriteIfNotNull(sc.Stream, obj, () => context.ItemSyncSerializer!(sc, obj), sc);
                    }
                case SerializerTypes.Any: return WriteAnyNullable(sc.Stream, item, sc);
                case SerializerTypes.AnyObject: return WriteAnyObjectNullable(sc.Stream, item, sc);
                case SerializerTypes.Bool: return WriteNullable(sc.Stream, (bool?)item, sc);
                case SerializerTypes.Number: return WriteNumberNullable(sc.Stream, item, sc);
                case SerializerTypes.Enum: return WriteEnumNullable(sc.Stream, (Enum?)item, sc);
                case SerializerTypes.String: return WriteStringNullable(sc.Stream, (string?)item, sc);
                case SerializerTypes.String16: return WriteString16Nullable(sc.Stream, (string?)item, sc);
                case SerializerTypes.String32: return WriteString32Nullable(sc.Stream, (string?)item, sc);
                case SerializerTypes.Bytes: return WriteBytesNullable(sc.Stream, (byte[]?)item, sc);
                case SerializerTypes.Array: return WriteArrayNullable(sc.Stream, (Array?)item, sc);
                case SerializerTypes.List: return WriteListNullable(sc.Stream, (IList?)item, sc);
                case SerializerTypes.Dictionary: return WriteDictNullable(sc.Stream, (IDictionary?)item, sc);
                case SerializerTypes.Struct: return WriteStructNullable(sc.Stream, item, sc);
                case SerializerTypes.Stream: return WriteStreamNullable(sc.Stream, (Stream?)item, sc);
                case SerializerTypes.Type: return WriteNullable(sc.Stream, (Type)item, sc);
                default: throw SerializerException.From(new ArgumentException($"Unknown serializer type {context.ItemSerializer}", nameof(context)));
            }
        }

        /// <summary>
        /// Write an item using a specified serializer
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="item">Item</param>
        /// <returns>Stream</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<Stream> WriteItemAsync(ItemSerializerContext context, dynamic item)
        {
            ISerializationContext sc = context.Context;
            if (!context.Nullable)
            {
                switch (context.ItemSerializer)
                {
                    case SerializerTypes.StreamSerializer: return await WriteSerializedAsync(sc.Stream, (IStreamSerializer)item, sc).DynamicContext();
                    case SerializerTypes.Serializer:
                        {
                            object obj = item;
                            if (context.ItemAsyncSerializer == null)
                            {
                                await Task.Yield();
                                SerializerException.Wrap(() =>
                                    ArgumentValidationHelper.EnsureValidArgument(nameof(context), context.ItemSyncSerializer, () => "Missing synchronous erializer")
                                    );
                                SerializerException.Wrap(() => context.ItemSyncSerializer!(sc, obj));
                            }
                            else
                            {
                                await SerializerException.WrapAsync(() => context.ItemAsyncSerializer(sc, obj)).DynamicContext();
                            }
                            return sc.Stream;
                        }
                    case SerializerTypes.Any: return await WriteAnyAsync(sc.Stream, (object)item, sc).DynamicContext();
                    case SerializerTypes.AnyObject: return await WriteAnyObjectAsync(sc.Stream, (object)item, sc).DynamicContext();
                    case SerializerTypes.Bool: return await WriteAsync(sc.Stream, (bool)item, sc).DynamicContext();
                    case SerializerTypes.Number: return await WriteNumberAsync(sc.Stream, (object)item, sc).DynamicContext();
                    case SerializerTypes.Enum: return await WriteEnumAsync(sc.Stream, (Enum)item, sc).DynamicContext();
                    case SerializerTypes.String: return await WriteStringAsync(sc.Stream, (string)item, sc).DynamicContext();
                    case SerializerTypes.String16: return await WriteString16Async(sc.Stream, (string)item, sc).DynamicContext();
                    case SerializerTypes.String32: return await WriteString32Async(sc.Stream, (string)item, sc).DynamicContext();
                    case SerializerTypes.Bytes: return await WriteBytesAsync(sc.Stream, (byte[])item, sc).DynamicContext();
                    case SerializerTypes.Array: return await WriteArrayAsync(sc.Stream, (Array)item, sc).DynamicContext();
                    case SerializerTypes.List: return await WriteListAsync(sc.Stream, (IList)item, sc).DynamicContext();
                    case SerializerTypes.Dictionary: return await WriteDictAsync(sc.Stream, (IDictionary)item, sc).DynamicContext();
                    case SerializerTypes.Struct: return await WriteStructAsync(sc.Stream, (object)item, sc).DynamicContext();
                    case SerializerTypes.Stream: return await WriteStreamAsync(sc.Stream, (Stream)item, sc).DynamicContext();
                    case SerializerTypes.Type: return await WriteAsync(sc.Stream, (Type)item, sc).DynamicContext();
                    default: throw SerializerException.From(new ArgumentException($"Unknown serializer type {context.ItemSerializer}", nameof(context)));
                }
            }
            switch (context.ItemSerializer)
            {
                case SerializerTypes.StreamSerializer: return await WriteSerializedNullableAsync(sc.Stream, (IStreamSerializer?)item, sc).DynamicContext();
                case SerializerTypes.Serializer:
                    {
                        object? obj = item!;
                        if (context.ItemAsyncSerializer == null)
                        {
                            SerializerException.Wrap(() =>
                                ArgumentValidationHelper.EnsureValidArgument(nameof(context), context.ItemSyncSerializer, () => "Missing synchronous serializer")
                                );
                            Task action()
                            {
                                context.ItemSyncSerializer!(sc, obj);
                                return Task.CompletedTask;
                            }
                            return await WriteIfNotNullAsync(sc.Stream, obj, action, sc).DynamicContext();
                        }
                        else
                        {
                            return await WriteIfNotNullAsync(sc.Stream, obj, () => context.ItemAsyncSerializer!(sc, obj), sc).DynamicContext();
                        }
                    }
                case SerializerTypes.Any: return await WriteAnyNullableAsync(sc.Stream, (object?)item, sc).DynamicContext();
                case SerializerTypes.AnyObject: return await WriteAnyObjectNullableAsync(sc.Stream, (object?)item, sc).DynamicContext();
                case SerializerTypes.Bool: return await WriteNullableAsync(sc.Stream, (bool?)item, sc).DynamicContext();
                case SerializerTypes.Number: return await WriteNumberNullableAsync(sc.Stream, (object?)item, sc).DynamicContext();
                case SerializerTypes.Enum: return await WriteEnumNullableAsync(sc.Stream, (Enum?)item, sc).DynamicContext();
                case SerializerTypes.String: return await WriteStringNullableAsync(sc.Stream, (string?)item, sc).DynamicContext();
                case SerializerTypes.String16: return await WriteString16NullableAsync(sc.Stream, (string?)item, sc).DynamicContext();
                case SerializerTypes.String32: return await WriteString32NullableAsync(sc.Stream, (string?)item, sc).DynamicContext();
                case SerializerTypes.Bytes: return await WriteBytesNullableAsync(sc.Stream, (byte[]?)item, sc).DynamicContext();
                case SerializerTypes.Array: return await WriteArrayNullableAsync(sc.Stream, (Array?)item, sc).DynamicContext();
                case SerializerTypes.List: return await WriteListNullableAsync(sc.Stream, (IList?)item, sc).DynamicContext();
                case SerializerTypes.Dictionary: return await WriteDictNullableAsync(sc.Stream, (IDictionary?)item, sc).DynamicContext();
                case SerializerTypes.Struct: return await WriteStructNullableAsync(sc.Stream, (object?)item, sc).DynamicContext();
                case SerializerTypes.Stream: return await WriteStreamNullableAsync(sc.Stream, (Stream?)item, sc).DynamicContext();
                case SerializerTypes.Type: return await WriteNullableAsync(sc.Stream, (Type)item, sc).DynamicContext();
                default: throw SerializerException.From(new ArgumentException($"Unknown serializer type {context.ItemSerializer}", nameof(context)));
            }
        }

        /// <summary>
        /// Write an item header, if the used item type isn't specified
        /// </summary>
        /// <typeparam name="T">Item type</typeparam>
        /// <param name="context">Context</param>
        /// <param name="item">Item</param>
        /// <param name="itemType">Item type</param>
        /// <returns>All information written?</returns>
        public static bool WriteAnyItemHeader<T>(ItemSerializerContext context, T item, Type itemType)
        {
            ISerializationContext sc = context.Context;
            // Use the object cache
            var info = item!.GetObjectSerializerInfo();
            if (info.WriteObject)
            {
                int idx = context.GetObjectCacheIndex(item);
                if (idx != -1)
                {
                    Write(sc.Stream, (byte)ObjectTypes.Cached, sc);
                    Write(sc.Stream, (byte)idx, sc);
                    context.ObjectType = info.ObjectType;
                    context.WriteObject = info.WriteObject;
                    (context.ItemSerializer, context.ItemSyncSerializer, _) = itemType.GetItemSerializerInfo(info.ObjectType, isAsync: false);
                    context.LastItemType = context.WriteObject ? itemType : null;
                    return true;
                }
                else
                {
                    context.AddObject(item);
                }
            }
            // Write the type information
            if (itemType == context.LastItemType)
            {
                // Use the last object type
                Write(sc.Stream, (byte)ObjectTypes.LastItemType, sc);
            }
            else
            {
                // Write object type details
                context.ObjectType = info.ObjectType;
                context.WriteObject = info.WriteObject;
                (context.ItemSerializer, context.ItemSyncSerializer, _) = itemType.GetItemSerializerInfo(info.ObjectType, isAsync: false);
                if (info.WriteType)
                {
                    //TODO For array/list/dictionary write the key/value type(s) only - when reading, use them to construct the target type
                    // Write type detail informations
                    SerializedTypeInfo ti = itemType;
                    if (ti.IsBasicType)
                    {
                        // Don't use the type cache
                        Write(sc.Stream, (byte)(context.ObjectType | ObjectTypes.BasicTypeInfo), sc);
                        Write(sc.Stream, (byte)ti.ObjectType, sc);
                    }
                    else
                    {
                        int idx = context.GetTypeCacheIndex(itemType);
                        if (idx != -1)
                        {
                            // Use the cached type
                            context.ObjectType |= ObjectTypes.Cached;
                            Write(sc.Stream, (byte)context.ObjectType, sc);
                            Write(sc.Stream, (byte)idx, sc);
                        }
                        else
                        {
                            // Update the cache
                            context.AddType(itemType);
                            // Write the type informations
                            Write(sc.Stream, (byte)context.ObjectType, sc);
                            Write(sc.Stream, itemType, sc);
                        }
                    }
                }
                else
                {
                    // Write the type informations
                    Write(sc.Stream, (byte)context.ObjectType, sc);
                    context.ItemSerializer = SerializerTypes.Any;
                }
                context.LastItemType = context.WriteObject ? itemType : null;
            }
            return !context.WriteObject;
        }

        /// <summary>
        /// Write an item header, if the used item type isn't specified
        /// </summary>
        /// <typeparam name="T">Item type</typeparam>
        /// <param name="context">Context</param>
        /// <param name="item">Item</param>
        /// <param name="itemType">Item type</param>
        public static async Task<bool> WriteAnyItemHeaderAsync<T>(ItemSerializerContext context, T item, Type itemType)
        {
            ISerializationContext sc = context.Context;
            // Use the object cache
            var info = item!.GetObjectSerializerInfo();
            if (info.WriteObject)
            {
                int idx = context.GetObjectCacheIndex(item);
                if (idx != -1)
                {
                    await WriteAsync(sc.Stream, (byte)ObjectTypes.Cached, sc).DynamicContext();
                    await WriteAsync(sc.Stream, (byte)idx, sc).DynamicContext();
                    context.ObjectType = info.ObjectType;
                    context.WriteObject = info.WriteObject;
                    (context.ItemSerializer, context.ItemSyncSerializer, context.ItemAsyncSerializer) =
                        itemType.GetItemSerializerInfo(context.ObjectType, isAsync: true);
                    context.LastItemType = context.WriteObject ? itemType : null;
                    return true;
                }
                else
                {
                    context.AddObject(item);
                }
            }
            // Write the type information
            if (itemType == context.LastItemType)
            {
                // Use the last object type
                await WriteAsync(sc.Stream, (byte)ObjectTypes.LastItemType, sc).DynamicContext();
            }
            else
            {
                // Write object type details
                context.ObjectType = info.ObjectType;
                context.WriteObject = info.WriteObject;
                (context.ItemSerializer, context.ItemSyncSerializer, context.ItemAsyncSerializer) =
                    itemType.GetItemSerializerInfo(context.ObjectType, isAsync: true);
                if (info.WriteType)
                {
                    // Write type detail informations
                    SerializedTypeInfo ti = itemType;
                    if (ti.IsBasicType)
                    {
                        // Don't use the type cache
                        await WriteAsync(sc.Stream, (byte)(context.ObjectType | ObjectTypes.BasicTypeInfo), sc).DynamicContext();
                        await WriteAsync(sc.Stream, (byte)ti.ObjectType, sc).DynamicContext();
                    }
                    else
                    {
                        int idx = context.GetTypeCacheIndex(itemType);
                        if (idx != -1)
                        {
                            // Use the cached type
                            context.ObjectType |= ObjectTypes.Cached;
                            await WriteAsync(sc.Stream, (byte)context.ObjectType, sc).DynamicContext();
                            await WriteAsync(sc.Stream, (byte)idx, sc).DynamicContext();
                        }
                        else
                        {
                            // Update the cache
                            context.AddType(itemType);
                            // Write the type informations
                            await WriteAsync(sc.Stream, (byte)context.ObjectType, sc).DynamicContext();
                            await WriteAsync(sc.Stream, itemType, sc).DynamicContext();
                        }
                    }
                }
                else
                {
                    // Write the type informations
                    await WriteAsync(sc.Stream, (byte)context.ObjectType, sc).DynamicContext();
                    context.ItemSerializer = SerializerTypes.Any;
                }
                context.LastItemType = context.WriteObject ? itemType : null;
            }
            return !context.WriteObject;
        }
    }
}
