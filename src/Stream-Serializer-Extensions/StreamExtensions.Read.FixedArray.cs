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
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] ReadFixedArray<T>(this Stream stream, T[] arr, IDeserializationContext context)
        {
            ReadFixedArray(stream, arr.AsSpan(), context);
            return arr;
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="arr">Array</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Span<T> ReadFixedArray<T>(this Stream stream, Span<T> arr, IDeserializationContext context)
        {
            using ContextRecursion cr = new(context);
            try
            {
                switch (context.SerializerVersion)// Serializer version switch
                {
                    case 1:
                    case 2:
                        {
                            for (int i = 0, len = arr.Length; i < len; arr[i] = ReadObject<T>(stream, context), i++) ;
                            return arr;
                        }
                    default:
                        try
                        {
                            Type type = typeof(T);
                            using ItemDeserializerContext itemContext = new(context)
                            {
                                Nullable = context.Options?.IsNullable ?? context.Nullable
                            };
                            (itemContext.ItemSerializer, itemContext.ItemSyncDeserializer, _) = type.GetItemDeserializerInfo(ObjectTypes.Null, isAsync: false);
                            if (itemContext.ItemSerializer == SerializerTypes.Any)
                            {
                                object? obj;
                                for (int i = 0, len = arr.Length; i < len; i++)
                                {
                                    obj = ReadAnyItemHeader(itemContext, i, type);
                                    Logging.WriteInfo($"READ {i} {stream.Position} {itemContext.ObjectType} {(int)itemContext.ObjectType} {itemContext.ItemType} {obj}");
                                    if (obj == null && itemContext.ObjectType == ObjectTypes.Null)
                                    {
                                        if (!itemContext.Nullable) throw new SerializerException($"Deserialized NULL value #{i}", new InvalidDataException());
                                        arr[i] = (T?)obj!;
                                    }
                                    else if (obj == null)
                                    {
                                        arr[i] = (itemContext.ItemSerializer == SerializerTypes.Serializer
                                            ? (T?)(obj = ReadItem(itemContext))
                                            : (T?)(obj = ReadAnyInt(context, itemContext.ObjectType, itemContext.ItemType)))!;
                                        if (itemContext.ObjectType.RequiresObjectWriting()) itemContext.AddObject(obj);
                                    }
                                    else
                                    {
                                        arr[i] = (T)obj;
                                    }
                                }
                            }
                            else
                            {
                                for (int i = 0, len = arr.Length; i < len; arr[i] = (T)ReadItem(itemContext)!, i++) ;
                            }
                            return arr;
                        }
                        finally
                        {
                            context.WithoutOptions();
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
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Array ReadFixedArray(this Stream stream, Array arr, IDeserializationContext context)
        {
            using ContextRecursion cr = new(context);
            Type elementType = arr.GetType().GetElementType()!;
            switch (context.SerializerVersion)// Serializer version switch
            {
                case 1:
                case 2:
                    {
                        for (int i = 0, len = arr.Length; i < len; arr.SetValue(ReadObject(stream, elementType, context), i), i++) ;
                        return arr;
                    }
                default:
                    try
                    {
                        Type type = arr.GetType().GetElementType()!;
                        using ItemDeserializerContext itemContext = new(context)
                        {
                            Nullable = context.Options?.IsNullable ?? context.Nullable
                        };
                        (itemContext.ItemSerializer, itemContext.ItemSyncDeserializer, _) = type.GetItemDeserializerInfo(ObjectTypes.Null, isAsync: false);
                        if (itemContext.ItemSerializer == SerializerTypes.Any)
                        {
                            object? obj;
                            for (int i = 0, len = arr.Length; i < len; i++)
                            {
                                obj = ReadAnyItemHeader(itemContext, i, type);
                                if (obj == null && itemContext.ObjectType == ObjectTypes.Null)
                                {
                                    if (!itemContext.Nullable) throw new SerializerException($"Deserialized NULL value #{i}", new InvalidDataException());
                                    arr.SetValue(obj, i);
                                }
                                else if (obj == null)
                                {
                                    arr.SetValue((itemContext.ItemSerializer == SerializerTypes.Serializer
                                        ? obj = ReadItem(itemContext)
                                        : obj = ReadAnyInt(context, itemContext.ObjectType, itemContext.ItemType))!, i);
                                    if (itemContext.ObjectType.RequiresObjectWriting()) itemContext.AddObject(obj);
                                }
                                else
                                {
                                    arr.SetValue(obj, i);
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0, len = arr.Length; i < len; arr.SetValue(ReadItem(itemContext), i), i++) ;
                        }
                        return arr;
                    }
                    finally
                    {
                        context.WithoutOptions();
                    }
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="arr">Array</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<T[]> ReadFixedArrayAsync<T>(this Stream stream, T[] arr, IDeserializationContext context)
        {
            await ReadFixedArrayAsync(stream, arr.AsMemory(), context).DynamicContext();
            return arr;
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="arr">Array</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<Memory<T>> ReadFixedArrayAsync<T>(this Stream stream, Memory<T> arr, IDeserializationContext context)
        {
            using ContextRecursion cr = new(context);
            T? item;
            switch (context.SerializerVersion)// Serializer version switch
            {
                case 1:
                case 2:
                    {
                        for (int i = 0, len = arr.Length; i < len; i++)
                        {
                            item = await ReadObjectAsync<T>(stream, context).DynamicContext();
                            arr.Span[i] = item;
                        }
                        return arr;
                    }
                default:
                    try
                    {
                        Type type = typeof(T);
                        using ItemDeserializerContext itemContext = new(context)
                        {
                            Nullable = context.Options?.IsNullable ?? context.Nullable
                        };
                        (itemContext.ItemSerializer, itemContext.ItemSyncDeserializer, itemContext.ItemAsyncDeserializer) = 
                            type.GetItemDeserializerInfo(ObjectTypes.Null, isAsync: true);
                        if (itemContext.ItemSerializer == SerializerTypes.Any)
                        {
                            object? obj;
                            for (int i = 0, len = arr.Length; i < len; i++)
                            {
                                obj = await ReadAnyItemHeaderAsync(itemContext, i, type).DynamicContext();
                                if (obj == null && itemContext.ObjectType == ObjectTypes.Null)
                                {
                                    if (!itemContext.Nullable) throw new SerializerException($"Deserialized NULL value #{i}", new InvalidDataException());
                                    arr.Span[i] = (T?)obj!;
                                }
                                else if (obj == null)
                                {
                                    obj = itemContext.ItemSerializer == SerializerTypes.Serializer
                                        ? await ReadItemAsync(itemContext).DynamicContext()
                                        : await ReadAnyIntAsync(context, itemContext.ObjectType, itemContext.ItemType).DynamicContext();
                                    arr.Span[i] = (T?)obj!;
                                    if (itemContext.ObjectType.RequiresObjectWriting()) itemContext.AddObject(obj);
                                }
                                else
                                {
                                    arr.Span[i] = (T)obj;
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0, len = arr.Length; i < len; item = (T)(await ReadItemAsync(itemContext).DynamicContext())!, arr.Span[i] = item, i++) ;
                        }
                        return arr;
                    }
                    finally
                    {
                        context.WithoutOptions();
                    }
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="arr">Array</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<Array> ReadFixedArrayAsync(this Stream stream, Array arr, IDeserializationContext context)
        {
            using ContextRecursion cr = new(context);
            Type elementType = arr.GetType().GetElementType()!;
            switch (context.SerializerVersion)// Serializer version switch
            {
                case 1:
                case 2:
                    {
                        for (
                            int i = 0, len = arr.Length;
                            i < len;
                            arr.SetValue(await ReadObjectAsync(stream, elementType, context).DynamicContext(), i), i++
                            ) ;
                        return arr;
                    }
                default:
                    try
                    {
                        Type type = arr.GetType().GetElementType()!;
                        using ItemDeserializerContext itemContext = new(context)
                        {
                            Nullable = context.Options?.IsNullable ?? context.Nullable
                        };
                        (itemContext.ItemSerializer, itemContext.ItemSyncDeserializer, itemContext.ItemAsyncDeserializer) = 
                            type.GetItemDeserializerInfo(ObjectTypes.Null, isAsync: true);
                        if (itemContext.ItemSerializer == SerializerTypes.Any)
                        {
                            object? obj;
                            for (int i = 0, len = arr.Length; i < len; i++)
                            {
                                obj = await ReadAnyItemHeaderAsync(itemContext, i, type).DynamicContext();
                                if (obj == null && itemContext.ObjectType == ObjectTypes.Null)
                                {
                                    if (!itemContext.Nullable) throw new SerializerException($"Deserialized NULL value #{i}", new InvalidDataException());
                                    arr.SetValue(obj, i);
                                }
                                else if (obj == null)
                                {
                                    arr.SetValue((itemContext.ItemSerializer == SerializerTypes.Serializer
                                        ? obj = await ReadItemAsync(itemContext).DynamicContext()
                                        : obj = await ReadAnyIntAsync(context, itemContext.ObjectType, itemContext.ItemType).DynamicContext()), i);
                                    if (itemContext.ObjectType.RequiresObjectWriting()) itemContext.AddObject(obj);
                                }
                                else
                                {
                                    arr.SetValue(obj, i);
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0, len = arr.Length; i < len; arr.SetValue(await ReadItemAsync(itemContext).DynamicContext(), i), i++) ;
                        }
                        return arr;
                    }
                    finally
                    {
                        context.WithoutOptions();
                    }
            }
        }
    }
}
