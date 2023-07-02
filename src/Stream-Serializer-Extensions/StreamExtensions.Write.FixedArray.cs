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
        /// Write
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="valuesNullable">Are the values nullable?</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Just a method adapter")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Stream WriteFixedArray<T>(this Stream stream, Span<T> value, bool valuesNullable = false)
            => WriteFixedArray(stream, (ReadOnlySpan<T>)value, valuesNullable);

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="valuesNullable">Are the values nullable?</param>
        /// <returns>Stream</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream WriteFixedArray<T>(this Stream stream, ReadOnlySpan<T> value, bool valuesNullable = false)
        {
            try
            {
                Type elementType = typeof(T);
                (SerializerTypes serializer, StreamSerializer.Serialize_Delegate? syncSerializer, _) = elementType.GetItemSerializerInfo(isAsync: false);
                if (serializer == SerializerTypes.Any)
                {
                    ObjectTypes objType = default;
                    Type? lastItemType = null;
                    Type itemType;
                    T? item;
                    SerializerTypes itemSerializer = default;
                    StreamSerializer.Serialize_Delegate? itemSyncSerializer = null;
                    bool writeObject = false;
                    int[] cache = ArrayPool<int>.Shared.RentClean(byte.MaxValue << 1);
                    try
                    {
                        Span<int> typeCache = cache.AsSpan(0, byte.MaxValue),
                            objectCache = cache.AsSpan(byte.MaxValue, byte.MaxValue);
                        for (int i = 0, len = value.Length; i < len; i++)
                        {
                            item = value[i];
                            if (item == null)
                            {
                                Write(stream, (byte)ObjectTypes.Null);
                                continue;
                            }
                            itemType = item!.GetType();
                            WriteAnyItemHeader(
                                stream,
                                item,
                                itemType,
                                typeCache,
                                objectCache,
                                ref lastItemType,
                                ref itemSerializer,
                                ref itemSyncSerializer,
                                ref objType,
                                ref writeObject
                                );
                            if (writeObject && objType != ObjectTypes.Cached)
                                if (itemSerializer == SerializerTypes.Serializer)
                                {
                                    WriteItem(stream, item, nullable: false, itemSerializer, itemSyncSerializer);
                                }
                                else
                                {
                                    WriteAny(stream, item, objType, writeObject);
                                }
                        }
                    }
                    finally
                    {
                        ArrayPool<int>.Shared.Return(cache);
                    }
                }
                else
                {
                    for (int i = 0, len = value.Length; i < len; WriteItem(stream, value[i]!, valuesNullable, serializer, syncSerializer), i++) ;
                }
                return stream;
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
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="valuesNullable">Are the values nullable?</param>
        /// <returns>Stream</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream WriteFixedArray(this Stream stream, Array value, bool valuesNullable = false)
            => SerializerException.Wrap(() =>
            {
                Type elementType = value.GetType().GetElementType()!;
                (SerializerTypes serializer, StreamSerializer.Serialize_Delegate? syncSerializer, _) = elementType.GetItemSerializerInfo(isAsync: false);
                if (serializer == SerializerTypes.Any)
                {
                    ObjectTypes objType = default;
                    Type? lastItemType = null;
                    Type itemType;
                    object? item;
                    SerializerTypes itemSerializer = default;
                    StreamSerializer.Serialize_Delegate? itemSyncSerializer = null;
                    bool writeObject = false,
                        isComplete;
                    int[] cache = ArrayPool<int>.Shared.RentClean(byte.MaxValue << 1);
                    try
                    {
                        Span<int> typeCache = cache.AsSpan(0, byte.MaxValue),
                            objectCache = cache.AsSpan(byte.MaxValue, byte.MaxValue);
                        for (int i = 0, len = value.Length; i < len; i++)
                        {
                            item = value.GetValue(i);
                            if (item == null)
                            {
                                Write(stream, (byte)ObjectTypes.Null);
                                continue;
                            }
                            itemType = item.GetType();
                            isComplete = WriteAnyItemHeader(
                                stream,
                                item,
                                itemType,
                                typeCache,
                                objectCache,
                                ref lastItemType,
                                ref itemSerializer,
                                ref itemSyncSerializer,
                                ref objType,
                                ref writeObject
                                );
                            Logging.WriteInfo($"WRITE {i} {stream.Position} {objType} {(int)objType} {itemType}");
                            if (!isComplete)
                                if (itemSerializer == SerializerTypes.Serializer)
                                {
                                    WriteItem(stream, item, nullable: false, itemSerializer, itemSyncSerializer);
                                }
                                else
                                {
                                    WriteAny(stream, item, objType, writeObject);
                                }
                        }
                    }
                    finally
                    {
                        ArrayPool<int>.Shared.Return(cache);
                    }
                }
                else
                {
                    for (int i = 0, len = value.Length; i < len; WriteItem(stream, value.GetValue(i)!, valuesNullable, serializer, syncSerializer), i++) ;
                }
                return stream;
            });

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="valuesNullable">Are the values nullable?</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Just a method adapter")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteFixedArrayAsync<T>(this Stream stream, Memory<T> value, bool valuesNullable = false, CancellationToken cancellationToken = default)
            => WriteFixedArrayAsync(stream, (ReadOnlyMemory<T>)value, valuesNullable, cancellationToken);

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="valuesNullable">Are the values nullable?</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Just a method adapter")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteFixedArrayAsync<T>(this Task<Stream> stream, Memory<T> value, bool valuesNullable = false, CancellationToken cancellationToken = default)
            => AsyncHelper.FluentAsync(stream, value, valuesNullable, cancellationToken, WriteFixedArrayAsync);

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="valuesNullable">Are the values nullable?</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<Stream> WriteFixedArrayAsync<T>(this Stream stream, ReadOnlyMemory<T> value, bool valuesNullable = false, CancellationToken cancellationToken = default)
            => SerializerException.WrapAsync(async () =>
            {
                Type elementType = typeof(T);
                (SerializerTypes serializer, StreamSerializer.Serialize_Delegate? syncSerializer, StreamSerializer.AsyncSerialize_Delegate? asyncSerializer) =
                    elementType.GetItemSerializerInfo(isAsync: true);
                if (serializer == SerializerTypes.Any)
                {
                    ObjectTypes objType = default;
                    Type? lastItemType = null;
                    Type itemType;
                    T? item;
                    SerializerTypes itemSerializer = default;
                    StreamSerializer.Serialize_Delegate? itemSyncSerializer = null;
                    StreamSerializer.AsyncSerialize_Delegate? itemAsyncSerializer = null;
                    bool writeObject = false,
                        isComplete;
                    int[] cache = ArrayPool<int>.Shared.RentClean(byte.MaxValue << 1);
                    try
                    {
                        Memory<int> typeCache = cache.AsMemory(0, byte.MaxValue),
                            objectCache = cache.AsMemory(byte.MaxValue, byte.MaxValue);
                        for (int i = 0, len = value.Length; i < len; i++)
                        {
                            item = value.Span[i];
                            if (item == null)
                            {
                                await WriteAsync(stream, (byte)ObjectTypes.Null, cancellationToken).DynamicContext();
                                continue;
                            }
                            itemType = item.GetType();
                            (isComplete, lastItemType, itemSerializer, itemSyncSerializer, itemAsyncSerializer, objType, writeObject) = await WriteAnyItemHeaderAsync(
                                stream,
                                item,
                                itemType,
                                typeCache,
                                objectCache,
                                lastItemType,
                                itemSerializer,
                                itemSyncSerializer,
                                itemAsyncSerializer,
                                objType,
                                writeObject,
                                cancellationToken
                                ).DynamicContext();
                            if (!isComplete && writeObject)
                                if (itemSerializer == SerializerTypes.Serializer)
                                {
                                    await WriteItemAsync(stream, item, nullable: false, itemSerializer, itemSyncSerializer, itemAsyncSerializer, cancellationToken).DynamicContext();
                                }
                                else
                                {
                                    await WriteAnyAsync(stream, item, objType, writeObject, cancellationToken).DynamicContext();
                                }
                        }
                    }
                    finally
                    {
                        ArrayPool<int>.Shared.Return(cache);
                    }
                }
                else
                {
                    for (
                        int i = 0, len = value.Length;
                        i < len;
                        await WriteItemAsync(stream, value.Span[i]!, valuesNullable, serializer, syncSerializer, asyncSerializer, cancellationToken).DynamicContext(), i++
                        ) ;
                }
                return stream;
            });

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="valuesNullable">Are the values nullable?</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteFixedArrayAsync<T>(
            this Task<Stream> stream,
            ReadOnlyMemory<T> value,
            bool valuesNullable = false,
            CancellationToken cancellationToken = default
            )
            => AsyncHelper.FluentAsync(stream, value, valuesNullable, cancellationToken, WriteFixedArrayAsync);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="valuesNullable">Are the values nullable?</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<Stream> WriteFixedArrayAsync(this Stream stream, Array value, bool valuesNullable = false, CancellationToken cancellationToken = default)
            => SerializerException.WrapAsync(async () =>
            {
                Type elementType = value.GetType().GetElementType()!;
                (SerializerTypes serializer, StreamSerializer.Serialize_Delegate? syncSerializer, StreamSerializer.AsyncSerialize_Delegate? asyncSerializer) =
                    elementType.GetItemSerializerInfo(isAsync: true);
                if (serializer == SerializerTypes.Any)
                {
                    ObjectTypes objType = default;
                    Type? lastItemType = null;
                    Type itemType;
                    object? item;
                    SerializerTypes itemSerializer = default;
                    StreamSerializer.Serialize_Delegate? itemSyncSerializer = null;
                    StreamSerializer.AsyncSerialize_Delegate? itemAsyncSerializer = null;
                    bool writeObject = false,
                        isComplete;
                    int[] cache = ArrayPool<int>.Shared.RentClean(byte.MaxValue << 1);
                    try
                    {
                        Memory<int> typeCache = cache.AsMemory(0, byte.MaxValue),
                            objectCache = cache.AsMemory(byte.MaxValue, byte.MaxValue);
                        for (int i = 0, len = value.Length; i < len; i++)
                        {
                            item = value.GetValue(i);
                            if (item == null)
                            {
                                await WriteAsync(stream, (byte)ObjectTypes.Null, cancellationToken).DynamicContext();
                                continue;
                            }
                            itemType = item.GetType();
                            (isComplete, lastItemType, itemSerializer, itemSyncSerializer, itemAsyncSerializer, objType, writeObject) = await WriteAnyItemHeaderAsync(
                                stream,
                                item,
                                itemType,
                                typeCache,
                                objectCache,
                                lastItemType,
                                itemSerializer,
                                itemSyncSerializer,
                                itemAsyncSerializer,
                                objType,
                                writeObject,
                                cancellationToken
                                ).DynamicContext();
                            if (!isComplete && writeObject)
                                if (itemSerializer == SerializerTypes.Serializer)
                                {
                                    await WriteItemAsync(stream, item, nullable: false, itemSerializer, itemSyncSerializer, itemAsyncSerializer, cancellationToken).DynamicContext();
                                }
                                else
                                {
                                    await WriteAnyAsync(stream, item, objType, writeObject, cancellationToken).DynamicContext();
                                }
                        }
                    }
                    finally
                    {
                        ArrayPool<int>.Shared.Return(cache);
                    }
                }
                else
                {
                    for (
                        int i = 0, len = value.Length;
                        i < len;
                        await WriteItemAsync(stream, value.GetValue(i)!, valuesNullable, serializer, syncSerializer, asyncSerializer, cancellationToken).DynamicContext(), i++
                        ) ;
                }
                return stream;
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteFixedArrayAsync(this Task<Stream> stream, Array value, bool valuesNullable = false, CancellationToken cancellationToken = default)
            => AsyncHelper.FluentAsync(stream, value, valuesNullable, cancellationToken, WriteFixedArrayAsync);
    }
}
