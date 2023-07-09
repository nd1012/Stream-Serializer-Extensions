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
        /// <param name="context">Context</param>
        /// <returns>Item</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static dynamic? ReadItem(ItemDeserializerContext context)
        {
            IDeserializationContext dc = context.Context;
            Type? type = context.ItemType;
            if (!context.Nullable)
            {
                switch (context.ItemSerializer)
                {
                    case SerializerTypes.StreamSerializer: return ReadSerializedObject(dc.Stream, SerializerHelper.EnsureNotNull(type, nameof(type)), dc);
                    case SerializerTypes.Serializer:
                        {
                            SerializerException.Wrap(() => ArgumentValidationHelper.EnsureValidArgument(nameof(context), context.ItemSyncDeserializer, "Missing serializer"));
                            return SerializerException.Wrap(() => context.ItemSyncDeserializer!(dc, SerializerHelper.EnsureNotNull(type, nameof(type))))
                                ?? throw new SerializerException($"{type} deserialized to NULL");
                        }
                    case SerializerTypes.Any: return ReadAny(dc.Stream, dc);
                    case SerializerTypes.AnyObject: return ReadAnyObject(dc.Stream, SerializerHelper.EnsureNotNull(type, nameof(type)), dc);
                    case SerializerTypes.Bool: return ReadBool(dc.Stream, dc);
                    case SerializerTypes.Number: return ReadNumber(dc.Stream, SerializerHelper.EnsureNotNull(type, nameof(type)), dc);
                    case SerializerTypes.Enum: return ReadEnum(dc.Stream, SerializerHelper.EnsureNotNull(type, nameof(type)), dc);
                    case SerializerTypes.String: return ReadString(dc.Stream, dc, dc.Options?.GetMinLen(0) ?? 0, dc.Options?.GetMaxLen(int.MaxValue) ?? int.MaxValue);
                    case SerializerTypes.String16: return ReadString16(dc.Stream, dc, dc.Options?.GetMinLen(0) ?? 0, dc.Options?.GetMaxLen(int.MaxValue) ?? int.MaxValue);
                    case SerializerTypes.String32: return ReadString32(dc.Stream, dc, dc.Options?.GetMinLen(0) ?? 0, dc.Options?.GetMaxLen(int.MaxValue) ?? int.MaxValue);
                    case SerializerTypes.Bytes: return ReadBytes(dc.Stream, dc, dc.Options?.GetMinLen(0) ?? 0, dc.Options?.GetMaxLen(int.MaxValue) ?? int.MaxValue).Value;
                    case SerializerTypes.Array: return ReadArray(dc.Stream, SerializerHelper.EnsureNotNull(type, nameof(type)), dc, dc.Options?.GetMinLen(0) ?? 0, dc.Options?.GetMaxLen(int.MaxValue) ?? int.MaxValue);
                    case SerializerTypes.List: return ReadList(dc.Stream, SerializerHelper.EnsureNotNull(type, nameof(type)), dc, dc.Options?.GetMinLen(0) ?? 0, dc.Options?.GetMaxLen(int.MaxValue) ?? int.MaxValue);
                    case SerializerTypes.Dictionary: return ReadDict(dc.Stream, SerializerHelper.EnsureNotNull(type, nameof(type)), dc, dc.Options?.GetMinLen(0) ?? 0, dc.Options?.GetMaxLen(int.MaxValue) ?? int.MaxValue);
                    case SerializerTypes.Struct:
                        return ReadStructMethod.MakeGenericMethod(SerializerHelper.EnsureNotNull(type, nameof(type))).InvokeAuto(obj: null, dc.Stream, dc)
                            ?? throw new SerializerException($"{nameof(ReadStruct)} serialized {type} to NULL");
                    case SerializerTypes.Stream: return ReadStream(dc.Stream, SerializerHelper.EnsureNotNull(dc.Options?.Attribute?.GetStream(null, null, dc), nameof(ISerializerOptions.Attribute)), dc, maxBufferSize: null, dc.Options?.GetMinLen(0) ?? 0, dc.Options?.GetMaxLen(long.MaxValue) ?? long.MaxValue);
                    case SerializerTypes.Type: return ReadType(dc.Stream, dc);
                    default: throw SerializerException.From(new ArgumentException($"Unknown serializer type {context.ItemSerializer}", nameof(context)));
                }
            }
            switch (context.ItemSerializer)
            {
                case SerializerTypes.StreamSerializer: return ReadSerializedObjectNullable(dc.Stream, SerializerHelper.EnsureNotNull(type, nameof(type)), dc);
                case SerializerTypes.Serializer:
                    {
                        if (!ReadBool(dc.Stream, dc)) return null;
                        SerializerException.Wrap(() => ArgumentValidationHelper.EnsureValidArgument(nameof(context), context.ItemSyncDeserializer, "Missing serializer"));
                        return SerializerException.Wrap(() => context.ItemSyncDeserializer!(dc, SerializerHelper.EnsureNotNull(type, nameof(type))))
                            ?? throw new SerializerException($"{type} deserialized to NULL");
                    }
                case SerializerTypes.Any: return ReadAnyNullable(dc.Stream, dc);
                case SerializerTypes.AnyObject: return ReadAnyObjectNullable(dc.Stream, SerializerHelper.EnsureNotNull(type, nameof(type)), dc);
                case SerializerTypes.Bool: return ReadBoolNullable(dc.Stream, dc);
                case SerializerTypes.Number: return ReadNumberNullable(dc.Stream, SerializerHelper.EnsureNotNull(type, nameof(type)), dc);
                case SerializerTypes.Enum: return ReadEnumNullable(dc.Stream, SerializerHelper.EnsureNotNull(type, nameof(type)), dc);
                case SerializerTypes.String: return ReadStringNullable(dc.Stream, dc, dc.Options?.GetMinLen(0) ?? 0, dc.Options?.GetMaxLen(int.MaxValue) ?? int.MaxValue);
                case SerializerTypes.String16: return ReadString16Nullable(dc.Stream, dc, dc.Options?.GetMinLen(0) ?? 0, dc.Options?.GetMaxLen(int.MaxValue) ?? int.MaxValue);
                case SerializerTypes.String32: return ReadString32Nullable(dc.Stream, dc, dc.Options?.GetMinLen(0) ?? 0, dc.Options?.GetMaxLen(int.MaxValue) ?? int.MaxValue);
                case SerializerTypes.Bytes: return ReadBytesNullable(dc.Stream, dc, dc.Options?.GetMinLen(0) ?? 0, dc.Options?.GetMaxLen(int.MaxValue) ?? int.MaxValue)?.Value;
                case SerializerTypes.Array: return ReadArrayNullable(dc.Stream, SerializerHelper.EnsureNotNull(type, nameof(type)), dc, dc.Options?.GetMinLen(0) ?? 0, dc.Options?.GetMaxLen(int.MaxValue) ?? int.MaxValue);
                case SerializerTypes.List: return ReadListNullable(dc.Stream, SerializerHelper.EnsureNotNull(type, nameof(type)), dc, dc.Options?.GetMinLen(0) ?? 0, dc.Options?.GetMaxLen(int.MaxValue) ?? int.MaxValue);
                case SerializerTypes.Dictionary: return ReadDictNullable(dc.Stream, SerializerHelper.EnsureNotNull(type, nameof(type)), dc, dc.Options?.GetMinLen(0) ?? 0, dc.Options?.GetMaxLen(int.MaxValue) ?? int.MaxValue);
                case SerializerTypes.Struct: return ReadStructNullableMethod.MakeGenericMethod(SerializerHelper.EnsureNotNull(type, nameof(type))).InvokeAuto(obj: null, dc.Stream, dc);
                case SerializerTypes.Stream: return ReadStreamNullable(dc.Stream, SerializerHelper.EnsureNotNull(dc.Options?.Attribute?.GetStream(null, null, dc), nameof(ISerializerOptions.Attribute)), dc, maxBufferSize: null, dc.Options?.GetMinLen(0) ?? 0, dc.Options?.GetMaxLen(long.MaxValue) ?? long.MaxValue);
                case SerializerTypes.Type: return ReadTypeNullable(dc.Stream, dc);
                default: throw SerializerException.From(new ArgumentException($"Unknown serializer type {context.ItemSerializer}", nameof(context)));
            }
        }

        /// <summary>
        /// Read an item using a specified serializer
        /// </summary>
        /// <param name="context">Context</param>
        /// <returns>Item</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<dynamic?> ReadItemAsync(ItemDeserializerContext context)
        {
            IDeserializationContext dc = context.Context;
            Type? type = context.ItemType;
            if (!context.Nullable)
            {
                switch (context.ItemSerializer)
                {
                    case SerializerTypes.StreamSerializer: return await ReadSerializedObjectAsync(dc.Stream, SerializerHelper.EnsureNotNull(type, nameof(type)), dc).DynamicContext();
                    case SerializerTypes.Serializer:
                        {
                            if (context.ItemAsyncDeserializer == null)
                            {
                                SerializerException.Wrap(() => ArgumentValidationHelper.EnsureValidArgument(nameof(context.ItemSyncDeserializer), context, "Missng serializer"));
                                return SerializerException.Wrap(() => context.ItemSyncDeserializer!(dc, SerializerHelper.EnsureNotNull(type, nameof(type))))
                                    ?? throw new SerializerException($"{type} deserialized to NULL");
                            }
                            else
                            {
                                Task task = SerializerException.WrapAsync(() => context.ItemAsyncDeserializer!(dc, SerializerHelper.EnsureNotNull(type, nameof(type))));
                                await task.DynamicContext();
                                return task.GetResult(type!);
                            }
                        }
                    case SerializerTypes.Any: return await ReadAnyAsync(dc.Stream, dc).DynamicContext();
                    case SerializerTypes.AnyObject: return await ReadAnyObjectAsync(dc.Stream, SerializerHelper.EnsureNotNull(type, nameof(type)), dc).DynamicContext();
                    case SerializerTypes.Bool: return await ReadBoolAsync(dc.Stream, dc).DynamicContext();
                    case SerializerTypes.Number: return await ReadNumberAsync(dc.Stream, SerializerHelper.EnsureNotNull(type, nameof(type)), dc).DynamicContext();
                    case SerializerTypes.Enum: return await ReadEnumAsync(dc.Stream, SerializerHelper.EnsureNotNull(type, nameof(type)), dc).DynamicContext();
                    case SerializerTypes.String: return await ReadStringAsync(dc.Stream, dc, dc.Options?.GetMinLen(0) ?? 0, dc.Options?.GetMaxLen(int.MaxValue) ?? int.MaxValue).DynamicContext();
                    case SerializerTypes.String16: return await ReadString16Async(dc.Stream, dc, dc.Options?.GetMinLen(0) ?? 0, dc.Options?.GetMaxLen(int.MaxValue) ?? int.MaxValue).DynamicContext();
                    case SerializerTypes.String32: return await ReadString32Async(dc.Stream, dc, dc.Options?.GetMinLen(0) ?? 0, dc.Options?.GetMaxLen(int.MaxValue) ?? int.MaxValue).DynamicContext();
                    case SerializerTypes.Bytes: return (await ReadBytesAsync(dc.Stream, dc, dc.Options?.GetMinLen(0) ?? 0, dc.Options?.GetMaxLen(int.MaxValue) ?? int.MaxValue).DynamicContext()).Value;
                    case SerializerTypes.Array: return await ReadArrayAsync(dc.Stream, SerializerHelper.EnsureNotNull(type, nameof(type)), dc, dc.Options?.GetMinLen(0) ?? 0, dc.Options?.GetMaxLen(int.MaxValue) ?? int.MaxValue).DynamicContext();
                    case SerializerTypes.List: return await ReadListAsync(dc.Stream, SerializerHelper.EnsureNotNull(type, nameof(type)), dc, dc.Options?.GetMinLen(0) ?? 0, dc.Options?.GetMaxLen(int.MaxValue) ?? int.MaxValue).DynamicContext();
                    case SerializerTypes.Dictionary: return await ReadDictAsync(dc.Stream, SerializerHelper.EnsureNotNull(type, nameof(type)), dc, dc.Options?.GetMinLen(0) ?? 0, dc.Options?.GetMaxLen(int.MaxValue) ?? int.MaxValue).DynamicContext();
                    case SerializerTypes.Struct:
                        return ReadStructMethod.MakeGenericMethod(SerializerHelper.EnsureNotNull(type, nameof(type))).InvokeAuto(obj: null, dc.Stream, dc)
                            ?? throw new SerializerException($"{nameof(ReadStruct)} serialized {type} to NULL");
                    case SerializerTypes.Stream: return await ReadStreamAsync(dc.Stream, SerializerHelper.EnsureNotNull(dc.Options?.Attribute?.GetStream(null, null, dc), nameof(ISerializerOptions.Attribute)), dc, maxBufferSize: null, dc.Options?.GetMinLen(0) ?? 0, dc.Options?.GetMaxLen(long.MaxValue) ?? long.MaxValue).DynamicContext();
                    case SerializerTypes.Type: return await ReadTypeAsync(dc.Stream, dc).DynamicContext();
                    default: throw SerializerException.From(new ArgumentException($"Unknown serializer type {context.ItemSerializer}", nameof(context)));
                }
            }
            switch (context.ItemSerializer)
            {
                case SerializerTypes.StreamSerializer: return await ReadSerializedObjectNullableAsync(dc.Stream, SerializerHelper.EnsureNotNull(type, nameof(type)), dc).DynamicContext();
                case SerializerTypes.Serializer:
                    {
                        if (!await ReadBoolAsync(dc.Stream, dc).DynamicContext()) return null;
                        if (context.ItemAsyncDeserializer == null)
                        {
                            SerializerException.Wrap(() => ArgumentValidationHelper.EnsureValidArgument(nameof(context), context.ItemSyncDeserializer, "Missing serializer"));
                            return SerializerException.Wrap(() => context.ItemSyncDeserializer!(dc, SerializerHelper.EnsureNotNull(type, nameof(type))))
                                ?? throw new SerializerException($"{type} deserialized to NULL");
                        }
                        else
                        {
                            Task task = SerializerException.WrapAsync(() => context.ItemAsyncDeserializer(dc, SerializerHelper.EnsureNotNull(type, nameof(type))));
                            await task.DynamicContext();
                            return task.GetResult(type!);
                        }
                    }
                case SerializerTypes.Any: return await ReadAnyNullableAsync(dc.Stream, dc).DynamicContext();
                case SerializerTypes.AnyObject: return await ReadAnyObjectNullableAsync(dc.Stream, SerializerHelper.EnsureNotNull(type, nameof(type)), dc).DynamicContext();
                case SerializerTypes.Bool: return await ReadBoolNullableAsync(dc.Stream, dc).DynamicContext();
                case SerializerTypes.Number: return await ReadNumberNullableAsync(dc.Stream, SerializerHelper.EnsureNotNull(type, nameof(type)), dc).DynamicContext();
                case SerializerTypes.Enum: return await ReadEnumNullableAsync(dc.Stream, SerializerHelper.EnsureNotNull(type, nameof(type)), dc).DynamicContext();
                case SerializerTypes.String: return await ReadStringNullableAsync(dc.Stream, dc, dc.Options?.GetMinLen(0) ?? 0, dc.Options?.GetMaxLen(int.MaxValue) ?? int.MaxValue).DynamicContext();
                case SerializerTypes.String16: return await ReadString16NullableAsync(dc.Stream, dc, dc.Options?.GetMinLen(0) ?? 0, dc.Options?.GetMaxLen(int.MaxValue) ?? int.MaxValue).DynamicContext();
                case SerializerTypes.String32: return await ReadString32NullableAsync(dc.Stream, dc, dc.Options?.GetMinLen(0) ?? 0, dc.Options?.GetMaxLen(int.MaxValue) ?? int.MaxValue).DynamicContext();
                case SerializerTypes.Bytes: return (await ReadBytesNullableAsync(dc.Stream, dc, dc.Options?.GetMinLen(0) ?? 0, dc.Options?.GetMaxLen(int.MaxValue) ?? int.MaxValue).DynamicContext())?.Value;
                case SerializerTypes.Array: return await ReadArrayNullableAsync(dc.Stream, SerializerHelper.EnsureNotNull(type, nameof(type)), dc, dc.Options?.GetMinLen(0) ?? 0, dc.Options?.GetMaxLen(int.MaxValue) ?? int.MaxValue).DynamicContext();
                case SerializerTypes.List: return await ReadListNullableAsync(dc.Stream, SerializerHelper.EnsureNotNull(type, nameof(type)), dc, dc.Options?.GetMinLen(0) ?? 0, dc.Options?.GetMaxLen(int.MaxValue) ?? int.MaxValue).DynamicContext();
                case SerializerTypes.Dictionary: return await ReadDictNullableAsync(dc.Stream, SerializerHelper.EnsureNotNull(type, nameof(type)), dc, dc.Options?.GetMinLen(0) ?? 0, dc.Options?.GetMaxLen(int.MaxValue) ?? int.MaxValue).DynamicContext();
                case SerializerTypes.Struct:
                    return ReadStructNullableMethod.MakeGenericMethod(SerializerHelper.EnsureNotNull(type, nameof(type))).InvokeAuto(obj: null, dc.Stream, dc)
                        ?? throw new SerializerException($"{nameof(ReadStructNullable)} serialized {type} to NULL");
                case SerializerTypes.Stream: return await ReadStreamNullableAsync(dc.Stream, SerializerHelper.EnsureNotNull(dc.Options?.Attribute?.GetStream(null, null, dc), nameof(ISerializerOptions.Attribute)), dc, maxBufferSize: null, dc.Options?.GetMinLen(0) ?? 0, dc.Options?.GetMaxLen(long.MaxValue) ?? long.MaxValue).DynamicContext();
                case SerializerTypes.Type: return await ReadTypeNullableAsync(dc.Stream, dc).DynamicContext();
                default: throw SerializerException.From(new ArgumentException($"Unknown serializer type {context.ItemSerializer}", nameof(context)));
            }
        }

        /// <summary>
        /// Read an item header, if the final item type is not specified
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="index">Item index</param>
        /// <param name="elementType">Element type</param>
        /// <returns>Cached object</returns>
        public static object? ReadAnyItemHeader(ItemDeserializerContext context, int index, Type elementType)
        {
            IDeserializationContext dc = context.Context;
            bool requireType;
            // Read the object type
            context.ObjectType = (ObjectTypes)ReadOneByte(dc.Stream, dc);
            if (context.ObjectType == ObjectTypes.Null) return null;
            // Use the object cache
            if (context.ObjectType == ObjectTypes.Cached)
            {
                object res = context.GetCachedObject(ReadOneByte(dc.Stream, dc));
                context.ItemType = res.GetType();
                context.ObjectType = res.GetObjectSerializerInfo().ObjectType;
                (context.ItemSerializer, context.ItemSyncDeserializer, _) = context.ItemType.GetItemDeserializerInfo(context.ObjectType, isAsync: false);
                context.LastObjectType = context.ObjectType;
                return res;
            }
            // Prepare the deserialization
            if (context.ObjectType == ObjectTypes.LastItemType)
            {
                // Use the last object type
                if (index == 0) throw new SerializerException($"Invalid object type {context.ObjectType} for item #{index}", new InvalidDataException());
                requireType = false;
            }
            else
            {
                // Ensure correct deserializer informations
                context.LastObjectType = context.ObjectType.RequiresObjectWriting() ? context.ObjectType : default;
                requireType = context.ObjectType.RequiresType();
                if (requireType)
                {
                    // An object type is required
                    if (context.ObjectType.IsBasicTypeInfo())
                    {
                        // Read a basic type information
                        context.ItemType = new SerializedTypeInfo((ObjectTypes)dc.Stream.ReadOneByte(dc)).ToSerializableType();
                        context.ObjectType &= ~ObjectTypes.BasicTypeInfo;
                    }
                    if (context.ObjectType.IsCached())
                    {
                        // Use a previously cached object type
                        context.ItemType = context.GetCachedType(ReadOneByte(dc.Stream, dc));
                        context.ObjectType &= ~ObjectTypes.Cached;
                    }
                    else
                    {
                        // Read the object type
                        context.ItemType = ReadSerializableType(dc.Stream, dc);
                        if (!elementType.IsAssignableFrom(context.ItemType) || context.ItemType.IsAbstract || context.ItemType.IsInterface || context.ItemType == typeof(object))
                            throw new SerializerException($"Invalid item type {context.ItemType} for item #{index} ({elementType})", new InvalidCastException());
                        context.AddType(context.ItemType);
                    }
                    (context.ItemSerializer, context.ItemSyncDeserializer, _) = context.ItemType.GetItemDeserializerInfo(context.ObjectType, isAsync: false);
                }
                else
                {
                    // No object type is required
                    context.ItemType = null;
                    context.ItemSerializer = SerializerTypes.Any;
                }
            }
            // Ensure having a valid type serializer configuration
            if (requireType && context.ItemType == null)
                throw new SerializerException($"Serialized type name expected for item #{index}", new InvalidDataException());
            return null;
        }

        /// <summary>
        /// Read an item header, if the final item type is not specified
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="index">Item index</param>
        /// <param name="elementType">Item element type</param>
        /// <returns>Item informations</returns>
        public static async Task<object?> ReadAnyItemHeaderAsync(ItemDeserializerContext context, int index, Type elementType)
        {
            IDeserializationContext dc = context.Context;
            bool requireType;
            // Read the object type
            context.ObjectType = (ObjectTypes)await ReadOneByteAsync(dc.Stream, dc).DynamicContext();
            if (context.ObjectType == ObjectTypes.Null) return null;
            // Use the object cache
            if (context.ObjectType == ObjectTypes.Cached)
            {
                object res = context.GetCachedObject(ReadOneByte(dc.Stream, dc));
                context.ItemType = res.GetType();
                context.ObjectType = res.GetObjectSerializerInfo().ObjectType;
                (context.ItemSerializer, context.ItemSyncDeserializer, context.ItemAsyncDeserializer) =
                    context.ItemType.GetItemDeserializerInfo(context.ObjectType, isAsync: true);
                context.LastObjectType = context.ObjectType;
                return res;
            }
            // Prepare the deserialization
            if (context.ObjectType == ObjectTypes.LastItemType)
            {
                // Use the last object type
                if (index == 0) throw new SerializerException($"Invalid object type {context.ObjectType} for item #{index}", new InvalidDataException());
                requireType = false;
            }
            else
            {
                // Ensure correct deserializer informations
                context.LastObjectType = context.ObjectType.RequiresObjectWriting() ? context.ObjectType : default;
                requireType = context.ObjectType.RequiresType();
                if (requireType)
                {
                    // An object type is required
                    if (context.ObjectType.IsBasicTypeInfo())
                    {
                        // Read a basic type information
                        context.ItemType = new SerializedTypeInfo((ObjectTypes)await dc.Stream.ReadOneByteAsync(dc).DynamicContext()).ToSerializableType();
                        context.ObjectType &= ~ObjectTypes.BasicTypeInfo;
                    }
                    if (context.ObjectType.IsCached())
                    {
                        // Use a previously cached object type
                        context.ItemType = context.GetCachedType(await ReadOneByteAsync(dc.Stream, dc).DynamicContext());
                        context.ObjectType &= ~ObjectTypes.Cached;
                    }
                    else
                    {
                        // Read the object type
                        context.ItemType = await ReadSerializableTypeAsync(dc.Stream, dc).DynamicContext();
                        if (!elementType.IsAssignableFrom(context.ItemType) || context.ItemType.IsAbstract || context.ItemType.IsInterface || context.ItemType == typeof(object))
                            throw new SerializerException($"Invalid item type {context.ItemType} for item #{index} ({elementType})", new InvalidCastException());
                        context.AddType(context.ItemType);
                    }
                    (context.ItemSerializer, context.ItemSyncDeserializer, context.ItemAsyncDeserializer) =
                        context.ItemType.GetItemDeserializerInfo(context.ObjectType, isAsync: true);
                }
                else
                {
                    // No object type is required
                    context.ItemType = null;
                    context.ItemSerializer = SerializerTypes.Any;
                }
            }
            // Ensure having a valid type serializer configuration
            if (requireType && context.ItemType == null)
                throw new SerializerException($"Serialized type name expected for item #{index}", new InvalidDataException());
            return null;
        }
    }
}
