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
        /// <param name="valuesNullable">Are the values nullable?</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream WriteDict(this Stream stream, IDictionary value, bool valuesNullable = false)
        {
            WriteNumber(stream, value.Count);
            if (value.Count == 0) return stream;
            WriteDictInt(stream, value, valuesNullable);
            return stream;
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="valuesNullable">Are the values nullable?</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<Stream> WriteDictAsync(this Stream stream, IDictionary value, bool valuesNullable = false, CancellationToken cancellationToken = default)
        {
            await WriteNumberAsync(stream, value.Count, cancellationToken).DynamicContext();
            if (value.Count == 0) return stream;
            await WriteDictIntAsync(stream, value, valuesNullable, cancellationToken).DynamicContext();
            return stream;
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="valuesNullable">Are the values nullable?</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteDictAsync(this Task<Stream> stream, IDictionary value, bool valuesNullable = false, CancellationToken cancellationToken = default)
            => AsyncHelper.FluentAsync(stream, value, valuesNullable, cancellationToken, WriteDictAsync);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="valuesNullable">Are the values nullable?</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream WriteDictNullable(this Stream stream, IDictionary? value, bool valuesNullable = false)
            => WriteNullableCount(stream, value?.Count, () =>
            {
                if (value!.Count == 0) return;
                WriteDictInt(stream, value, valuesNullable);
            });

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="valuesNullable">Are the values nullable?</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<Stream> WriteDictNullableAsync(this Stream stream, IDictionary? value, bool valuesNullable = false, CancellationToken cancellationToken = default)
            => WriteNullableCountAsync(stream, value?.Count, async () =>
            {
                if (value!.Count == 0) return;
                await WriteDictIntAsync(stream, value, valuesNullable, cancellationToken).DynamicContext();
            }, cancellationToken);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="valuesNullable">Are the values nullable?</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteDictNullableAsync(this Task<Stream> stream, IDictionary? value, bool valuesNullable = false, CancellationToken cancellationToken = default)
            => AsyncHelper.FluentAsync(stream, value, valuesNullable, cancellationToken, WriteDictNullableAsync);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="valuesNullable">Are the values nullable?</param>
        /// <returns>Stream</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void WriteDictInt(Stream stream, IDictionary value, bool valuesNullable)
        {
            Type valueType = value.GetType(),
                keyType = valueType.GenericTypeArguments[0],
                itemType = valueType.GenericTypeArguments[1],
                keyItemType,
                valueItemType;
            (SerializerTypes keySerializer, StreamSerializer.Serialize_Delegate? keySyncSerializer, _) = keyType.GetItemSerializerInfo(isAsync: false);
            (SerializerTypes itemSerializer, StreamSerializer.Serialize_Delegate? itemSyncSerializer, _) = itemType.GetItemSerializerInfo(isAsync: false);
            ObjectTypes keyObjType = default,
                valueObjType = default;
            Type? lastKeyItemType = null,
                lastValueItemType = null;
            object key;
            object? item;
            SerializerTypes keyItemSerializer = default,
                valueItemSerializer = default;
            StreamSerializer.Serialize_Delegate? keyItemSyncSerializer = null,
                valueItemSyncSerializer = null;
            bool writeKey = false,
                writeValue = false,
                isComplete;
            object[] keys = value.Keys.Cast<object>().ToArray();
            int[] cache;
            if (keySerializer == SerializerTypes.Any && itemSerializer == SerializerTypes.Any)
            {
                cache = ArrayPool<int>.Shared.RentClean(byte.MaxValue * 3);
            }
            else if (keySerializer == SerializerTypes.Any)
            {
                cache = ArrayPool<int>.Shared.RentClean(byte.MaxValue);
            }
            else if (itemSerializer == SerializerTypes.Any)
            {
                cache = ArrayPool<int>.Shared.RentClean(byte.MaxValue << 1);
            }
            else
            {
                cache = Array.Empty<int>();
            }
            try
            {
                Span<int> keyTypeCache,
                    keyObjectCache,
                    typeCache,
                    objectCache;
                if (keySerializer == SerializerTypes.Any)
                {
                    keyTypeCache = cache.AsSpan(0, byte.MaxValue);
                    keyObjectCache = cache.AsSpan(0, 0);
                }
                else
                {
                    keyTypeCache = keyObjectCache = cache.AsSpan(0, 0);
                }
                if (itemSerializer == SerializerTypes.Any)
                {
                    typeCache = cache.AsSpan(keySerializer == SerializerTypes.Any ? byte.MaxValue : 0, byte.MaxValue);
                    objectCache = cache.AsSpan(keySerializer == SerializerTypes.Any ? byte.MaxValue << 1 : byte.MaxValue, byte.MaxValue);
                }
                else
                {
                    typeCache = objectCache = cache.AsSpan(0, 0);
                }
                for (int i = 0, len = value.Count; i < len; i++)
                {
                    if (keySerializer == SerializerTypes.Any)
                    {
                        key = keys[i];
                        keyItemType = key.GetType();
                        isComplete = WriteAnyItemHeader(
                            stream,
                            key,
                            keyItemType,
                            keyTypeCache,
                            keyObjectCache,
                            ref lastKeyItemType,
                            ref keyItemSerializer,
                            ref keyItemSyncSerializer,
                            ref keyObjType,
                            ref writeKey
                            );
                        if (!isComplete && writeKey)
                            if (keyItemSerializer == SerializerTypes.Serializer)
                            {
                                WriteItem(stream, key, nullable: false, keyItemSerializer, keyItemSyncSerializer);
                            }
                            else
                            {
                                WriteAny(stream, key, keyObjType, writeKey);
                            }
                    }
                    else
                    {
                        WriteItem(stream, keys[i], nullable: false, keySerializer, keySyncSerializer);
                    }
                    if (keySerializer == SerializerTypes.Any)
                    {
                        item = value[keys[i]];
                        if (item == null)
                        {
                            Write(stream, (byte)ObjectTypes.Null);
                            continue;
                        }
                        valueItemType = value.GetType();
                        isComplete = WriteAnyItemHeader(
                            stream,
                            item,
                            valueItemType,
                            typeCache,
                            objectCache,
                            ref lastValueItemType,
                            ref valueItemSerializer,
                            ref valueItemSyncSerializer,
                            ref valueObjType,
                            ref writeValue
                            );
                        if (!isComplete && writeValue)
                            if (valueItemSerializer == SerializerTypes.Serializer)
                            {
                                WriteItem(stream, item, nullable: false, valueItemSerializer, valueItemSyncSerializer);
                            }
                            else
                            {
                                WriteAny(stream, item, valueObjType, writeValue);
                            }
                    }
                    else
                    {
                        WriteItem(stream, value[keys[i]]!, valuesNullable, itemSerializer, itemSyncSerializer);
                    }
                }
            }
            finally
            {
                ArrayPool<int>.Shared.Return(cache);
            }
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="valuesNullable">Are the values nullable?</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static async Task WriteDictIntAsync(Stream stream, IDictionary value, bool valuesNullable, CancellationToken cancellationToken)
        {
            Type valueType = value.GetType(),
                keyType = valueType.GenericTypeArguments[0],
                itemType = valueType.GenericTypeArguments[1],
                keyItemType,
                valueItemType;
            (SerializerTypes keySerializer, StreamSerializer.Serialize_Delegate? keySyncSerializer, StreamSerializer.AsyncSerialize_Delegate? keyAsyncSerializer) =
                keyType.GetItemSerializerInfo(isAsync: true);
            (SerializerTypes itemSerializer, StreamSerializer.Serialize_Delegate? itemSyncSerializer, StreamSerializer.AsyncSerialize_Delegate? itemAsyncSerializer) =
                itemType.GetItemSerializerInfo(isAsync: true);
            ObjectTypes keyObjType = default,
                valueObjType = default;
            Type? lastKeyItemType = null,
                lastValueItemType = null;
            object key;
            object? item;
            SerializerTypes keyItemSerializer = default,
                valueItemSerializer = default;
            StreamSerializer.Serialize_Delegate? keyItemSyncSerializer = null,
                valueItemSyncSerializer = null;
            StreamSerializer.AsyncSerialize_Delegate? keyItemAsyncSerializer = null,
                valueItemAsyncSerializer = null;
            bool writeKey = false,
                writeValue = false,
                isComplete;
            object[] keys = value.Keys.Cast<object>().ToArray();
            int[] cache;
            if (keySerializer == SerializerTypes.Any && itemSerializer == SerializerTypes.Any)
            {
                cache = ArrayPool<int>.Shared.RentClean(byte.MaxValue * 3);
            }
            else if (keySerializer == SerializerTypes.Any)
            {
                cache = ArrayPool<int>.Shared.RentClean(byte.MaxValue);
            }
            else if (itemSerializer == SerializerTypes.Any)
            {
                cache = ArrayPool<int>.Shared.RentClean(byte.MaxValue << 1);
            }
            else
            {
                cache = Array.Empty<int>();
            }
            try
            {
                Memory<int> keyTypeCache,
                    keyObjectCache,
                    typeCache,
                    objectCache;
                if (keySerializer == SerializerTypes.Any)
                {
                    keyTypeCache = cache.AsMemory(0, byte.MaxValue);
                    keyObjectCache = cache.AsMemory(0, 0);
                }
                else
                {
                    keyTypeCache = keyObjectCache = cache.AsMemory(0, 0);
                }
                if (itemSerializer == SerializerTypes.Any)
                {
                    typeCache = cache.AsMemory(keySerializer == SerializerTypes.Any ? byte.MaxValue : 0, byte.MaxValue);
                    objectCache = cache.AsMemory(keySerializer == SerializerTypes.Any ? byte.MaxValue << 1 : byte.MaxValue, byte.MaxValue);
                }
                else
                {
                    typeCache = objectCache = cache.AsMemory(0, 0);
                }
                for (int i = 0, len = value.Count; i < len; i++)
                {
                    if (keySerializer == SerializerTypes.Any)
                    {
                        key = keys[i];
                        keyItemType = key.GetType();
                        (isComplete, lastKeyItemType, keyItemSerializer, keyItemSyncSerializer, keyItemAsyncSerializer, keyObjType, writeKey) =
                            await WriteAnyItemHeaderAsync(
                                stream,
                                key,
                                keyItemType,
                                keyTypeCache,
                                keyObjectCache,
                                lastKeyItemType,
                                keyItemSerializer,
                                keyItemSyncSerializer,
                                keyItemAsyncSerializer,
                                keyObjType,
                                writeKey,
                                cancellationToken
                                ).DynamicContext();
                        if (!isComplete && writeKey)
                            if (keyItemSerializer == SerializerTypes.Serializer)
                            {
                                await WriteItemAsync(stream, key, nullable: false, keyItemSerializer, keyItemSyncSerializer, keyItemAsyncSerializer, cancellationToken)
                                    .DynamicContext();
                            }
                            else
                            {
                                await WriteAnyAsync(stream, key, keyObjType, writeKey, cancellationToken).DynamicContext();
                            }
                    }
                    else
                    {
                        await WriteItemAsync(stream, keys[i], nullable: false, keySerializer, keySyncSerializer, keyAsyncSerializer, cancellationToken).DynamicContext();
                    }
                    if (keySerializer == SerializerTypes.Any)
                    {
                        item = value[keys[i]];
                        if (item == null)
                        {
                            await WriteAsync(stream, (byte)ObjectTypes.Null, cancellationToken).DynamicContext();
                            continue;
                        }
                        valueItemType = value.GetType();
                        (isComplete, lastKeyItemType, keyItemSerializer, keyItemSyncSerializer, keyItemAsyncSerializer, keyObjType, writeKey) =
                            await WriteAnyItemHeaderAsync(
                                stream,
                                item,
                                valueItemType,
                                typeCache,
                                objectCache,
                                lastValueItemType,
                                valueItemSerializer,
                                valueItemSyncSerializer,
                                valueItemAsyncSerializer,
                                valueObjType,
                                writeValue,
                                cancellationToken
                                ).DynamicContext();
                        if (!isComplete && writeValue)
                            if (valueItemSerializer == SerializerTypes.Serializer)
                            {
                                await WriteItemAsync(stream, item, nullable: false, valueItemSerializer, valueItemSyncSerializer, valueItemAsyncSerializer, cancellationToken)
                                    .DynamicContext();
                            }
                            else
                            {
                                await WriteAnyAsync(stream, item, valueObjType, writeValue, cancellationToken).DynamicContext();
                            }
                    }
                    else
                    {
                        await WriteItemAsync(stream, value[keys[i]]!, valuesNullable, itemSerializer, itemSyncSerializer, itemAsyncSerializer, cancellationToken).DynamicContext();
                    }
                }
            }
            finally
            {
                ArrayPool<int>.Shared.Return(cache);
            }
        }
    }
}
