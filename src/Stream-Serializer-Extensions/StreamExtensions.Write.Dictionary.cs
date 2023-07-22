using System.Buffers;
using System.Collections;
using System.Data;
using System.Runtime;
using System.Runtime.CompilerServices;
using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    // Dictionary
    public static partial class StreamExtensions
    {
        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream WriteDict(this Stream stream, IDictionary value, ISerializationContext context)
        {
            WriteNumber(stream, value.Count, context);
            if (value.Count == 0) return stream;
            WriteDictInt(stream, value, context);
            return stream;
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<Stream> WriteDictAsync(this Stream stream, IDictionary value, ISerializationContext context)
        {
            await WriteNumberAsync(stream, value.Count, context).DynamicContext();
            if (value.Count == 0) return stream;
            await WriteDictIntAsync(stream, value, context).DynamicContext();
            return stream;
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteDictAsync(this Task<Stream> stream, IDictionary value, ISerializationContext context)
            => AsyncHelper.FluentAsync(stream, value, context, WriteDictAsync);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream WriteDictNullable(this Stream stream, IDictionary? value, ISerializationContext context)
            => WriteNullableCount(context, value?.Count, () =>
            {
                if (value!.Count == 0) return;
                WriteDictInt(stream, value, context);
            });

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<Stream> WriteDictNullableAsync(this Stream stream, IDictionary? value, ISerializationContext context)
            => WriteNullableCountAsync(context, value?.Count, async () =>
            {
                if (value!.Count == 0) return;
                await WriteDictIntAsync(stream, value, context).DynamicContext();
            });

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteDictNullableAsync(this Task<Stream> stream, IDictionary? value, ISerializationContext context)
            => AsyncHelper.FluentAsync(stream, value, context, WriteDictNullableAsync);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void WriteDictInt(Stream stream, IDictionary value, ISerializationContext context)
        {
            using ContextRecursion cr = new(context);
            Type valueType = value.GetType(),
                keyType = valueType.GenericTypeArguments[0],
                itemType = valueType.GenericTypeArguments[1];
            using ItemSerializerContext keyContext = new(context, objectCache: false);
            using ItemSerializerContext valueContext = new(context);
            (keyContext.ItemSerializer, keyContext.ItemSyncSerializer, _) = keyType.GetItemSerializerInfo(ObjectTypes.Null, isAsync: false);
            (valueContext.ItemSerializer, valueContext.ItemSyncSerializer, _) = itemType.GetItemSerializerInfo(ObjectTypes.Null, isAsync: false);
            object key;
            object? item;
            object[] keys = value.Keys.Cast<object>().ToArray();
            for (int i = 0, len = value.Count; i < len; i++)
            {
                if (keyContext.ItemSerializer == SerializerTypes.Any)
                {
                    key = keys[i];
                    if (WriteAnyItemHeader(keyContext, key, key.GetType())) continue;
                    if (keyContext.WriteObject)
                        if (keyContext.ItemSerializer == SerializerTypes.Serializer)
                        {
                            WriteItem(keyContext, key);
                        }
                        else
                        {
                            WriteAny(context.Stream, key, keyContext.ObjectType, keyContext.WriteObject, context);
                        }
                }
                else
                {
                    WriteItem(keyContext, keys[i]);
                }
                if (valueContext.ItemSerializer == SerializerTypes.Any)
                {
                    item = value[keys[i]];
                    if (item == null)
                    {
                        Write(stream, (byte)ObjectTypes.Null, context);
                        continue;
                    }
                    if (WriteAnyItemHeader(valueContext, item, item.GetType())) continue;
                    if (valueContext.WriteObject)
                        if (valueContext.ItemSerializer == SerializerTypes.Serializer)
                        {
                            WriteItem(valueContext, item);
                        }
                        else
                        {
                            WriteAny(context.Stream, item, valueContext.ObjectType, valueContext.WriteObject, context);
                        }
                }
                else
                {
                    WriteItem(valueContext, value[keys[i]]!);
                }
            }
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static async Task WriteDictIntAsync(Stream stream, IDictionary value, ISerializationContext context)
        {
            using ContextRecursion cr = new(context);
            Type valueType = value.GetType(),
                keyType = valueType.GenericTypeArguments[0],
                itemType = valueType.GenericTypeArguments[1];
            using ItemSerializerContext keyContext = new(context, objectCache: false);
            using ItemSerializerContext valueContext = new(context);
            (keyContext.ItemSerializer, keyContext.ItemSyncSerializer, keyContext.ItemAsyncSerializer) = 
                keyType.GetItemSerializerInfo(ObjectTypes.Null, isAsync: true);
            (valueContext.ItemSerializer, valueContext.ItemSyncSerializer, valueContext.ItemAsyncSerializer) = 
                itemType.GetItemSerializerInfo(ObjectTypes.Null, isAsync: true);
            object key;
            object? item;
            object[] keys = value.Keys.Cast<object>().ToArray();
            for (int i = 0, len = value.Count; i < len; i++)
            {
                if (keyContext.ItemSerializer == SerializerTypes.Any)
                {
                    key = keys[i];
                    if (await WriteAnyItemHeaderAsync(keyContext, key, key.GetType()).DynamicContext()) continue;
                    if (keyContext.WriteObject)
                        if (keyContext.ItemSerializer == SerializerTypes.Serializer)
                        {
                            await WriteItemAsync(keyContext, key).DynamicContext();
                        }
                        else
                        {
                            await WriteAnyAsync(context.Stream, key, keyContext.ObjectType, keyContext.WriteObject, context).DynamicContext();
                        }
                }
                else
                {
                    await WriteItemAsync(keyContext, keys[i]).DynamicContext();
                }
                if (valueContext.ItemSerializer == SerializerTypes.Any)
                {
                    item = value[keys[i]];
                    if (item == null)
                    {
                        await WriteAsync(stream, (byte)ObjectTypes.Null, context).DynamicContext();
                        continue;
                    }
                    if (await WriteAnyItemHeaderAsync(valueContext, item, item.GetType()).DynamicContext()) continue;
                    if (valueContext.WriteObject)
                        if (valueContext.ItemSerializer == SerializerTypes.Serializer)
                        {
                            await WriteItemAsync(valueContext, item).DynamicContext();
                        }
                        else
                        {
                            await WriteAnyAsync(context.Stream, item, valueContext.ObjectType, valueContext.WriteObject, context).DynamicContext();
                        }
                }
                else
                {
                    await WriteItemAsync(valueContext, value[keys[i]]!).DynamicContext();
                }
            }
        }
    }
}
