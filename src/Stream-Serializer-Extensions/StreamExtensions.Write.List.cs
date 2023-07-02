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
        public static Stream WriteList(this Stream stream, IList value, bool valuesNullable = false)
        {
            Type type = value.GetType();
            SerializerException.Wrap(() => ArgumentValidationHelper.EnsureValidArgument(
                nameof(value),
                type.IsGenericType || type.IsGenericTypeDefinition || !typeof(List<>).IsAssignableFrom(type.GetGenericTypeDefinition()),
                () => "Not a list type"
                ));
            WriteNumber(stream, value.Count);
            if (value.Count == 0) return stream;
            WriteListInt(stream, value, valuesNullable);
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
        public static async Task<Stream> WriteListAsync(this Stream stream, IList value, bool valuesNullable = false, CancellationToken cancellationToken = default)
        {
            Type type = value.GetType();
            SerializerException.Wrap(() => ArgumentValidationHelper.EnsureValidArgument(
                nameof(value),
                type.IsGenericType || type.IsGenericTypeDefinition || !typeof(List<>).IsAssignableFrom(type.GetGenericTypeDefinition()),
                () => "Not a list type"
                ));
            await WriteNumberAsync(stream, value.Count, cancellationToken).DynamicContext();
            if (value.Count == 0) return stream;
            await WriteListIntAsync(stream, value, valuesNullable, cancellationToken).DynamicContext();
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
        public static Task<Stream> WriteListAsync(this Task<Stream> stream, IList value, bool valuesNullable = false, CancellationToken cancellationToken = default)
            => AsyncHelper.FluentAsync(stream, value, valuesNullable, cancellationToken, WriteListAsync);

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
        public static Stream WriteListNullable(this Stream stream, IList? value, bool valuesNullable = false)
            => WriteNullableCount(stream, value?.Count, () =>
            {
                Type type = value!.GetType();
                SerializerException.Wrap(() => ArgumentValidationHelper.EnsureValidArgument(
                    nameof(value),
                    type.IsGenericType || type.IsGenericTypeDefinition || !typeof(List<>).IsAssignableFrom(type.GetGenericTypeDefinition()),
                    () => "Not a list type"
                    ));
                if (value!.Count == 0) return;
                WriteListInt(stream, value, valuesNullable);
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
        public static Task<Stream> WriteListNullableAsync(this Stream stream, IList? value, bool valuesNullable = false, CancellationToken cancellationToken = default)
            => WriteNullableCountAsync(stream, value?.Count, async () =>
            {
                Type type = value!.GetType();
                SerializerException.Wrap(() => ArgumentValidationHelper.EnsureValidArgument(
                    nameof(value),
                    type.IsGenericType || type.IsGenericTypeDefinition || !typeof(List<>).IsAssignableFrom(type.GetGenericTypeDefinition()),
                    () => "Not a list type"
                    ));
                if (value!.Count == 0) return;
                await WriteListIntAsync(stream, value, valuesNullable, cancellationToken).DynamicContext();
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
        public static Task<Stream> WriteListNullableAsync(this Task<Stream> stream, IList? value, bool valuesNullable = false, CancellationToken cancellationToken = default)
            => AsyncHelper.FluentAsync(stream, value, valuesNullable, cancellationToken, WriteListNullableAsync);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="valuesNullable">Are the values nullable?</param>
        /// <returns>Stream</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void WriteListInt(Stream stream, IList value, bool valuesNullable)
        {
            Type valueType = value.GetType(),
                elementType = valueType.IsGenericType && valueType.GenericTypeArguments.Length == 1
                    ? valueType.GenericTypeArguments[0]
                    : typeof(object);
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
                    for (int i = 0, len = value.Count; i < len; i++)
                    {
                        item = value[i];
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
                        if (!isComplete && writeObject)
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
                for (int i = 0, len = value.Count; i < len; WriteItem(stream, value[i]!, valuesNullable, serializer, syncSerializer), i++) ;
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
        private static async Task WriteListIntAsync(Stream stream, IList value, bool valuesNullable, CancellationToken cancellationToken)
        {
            Type valueType = value.GetType(),
                elementType = valueType.IsGenericType && valueType.GenericTypeArguments.Length == 1
                    ? valueType.GenericTypeArguments[0]
                    : typeof(object);
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
                    for (int i = 0, len = value.Count; i < len; i++)
                    {
                        item = value[i];
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
                    int i = 0, len = value.Count;
                    i < len;
                    await WriteItemAsync(stream, value[i]!, valuesNullable, serializer, syncSerializer, asyncSerializer, cancellationToken).DynamicContext(), i++
                    ) ;
            }
        }
    }
}
