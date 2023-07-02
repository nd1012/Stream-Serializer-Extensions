using System.Buffers;
using System.Reflection;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Text;
using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    // Object
    public static partial class StreamExtensions
    {
        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object to write</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream WriteObject(this Stream stream, object obj)
        {
            SerializerException.Wrap(() => ArgumentValidationHelper.EnsureValidArgument(nameof(obj), obj));
            if (obj is IStreamSerializer streamSerializer)
                return WriteSerialized(stream, streamSerializer);
            else if (StreamSerializer.FindSerializer(obj.GetType()) is not StreamSerializer.Serialize_Delegate serializer)
                return WriteAnyObject(stream, obj);
            else
                SerializerException.Wrap(() => serializer(stream, obj));
            return stream;
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<Stream> WriteObjectAsync(this Stream stream, object obj, CancellationToken cancellationToken = default)
        {
            SerializerException.Wrap(() => ArgumentValidationHelper.EnsureValidArgument(nameof(obj), obj));
            if (obj is IStreamSerializer streamSerializer)
                await WriteSerializedAsync(stream, streamSerializer, cancellationToken).DynamicContext();
            else if (StreamSerializer.FindAsyncSerializer(obj.GetType()) is not StreamSerializer.AsyncSerialize_Delegate serializer)
                await WriteAnyObjectAsync(stream, obj, cancellationToken).DynamicContext();
            else
                await SerializerException.WrapAsync(async () => await serializer(stream, obj, cancellationToken).DynamicContext()).DynamicContext();
            return stream;
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteObjectAsync(this Task<Stream> stream, object obj, CancellationToken cancellationToken = default)
            => AsyncHelper.FluentAsync(stream, obj, cancellationToken, WriteObjectAsync);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object to write</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream WriteObjectNullable(this Stream stream, object? obj)
            => WriteIfNotNull(stream, obj, () => WriteObject(stream, obj!));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<Stream> WriteObjectNullableAsync(this Stream stream, object? obj, CancellationToken cancellationToken = default)
            => WriteIfNotNullAsync(stream, obj, () => WriteObjectAsync(stream, obj!, cancellationToken), cancellationToken);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteObjectNullableAsync(this Task<Stream> stream, object? obj, CancellationToken cancellationToken = default)
            => AsyncHelper.FluentAsync(stream, obj, cancellationToken, WriteObjectNullableAsync);

        /// <summary>
        /// Write any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <returns>Stream</returns>
        public static Stream WriteAnyObject(this Stream stream, object obj)
        {
            if (obj is IStreamSerializer serializable) return WriteSerialized(stream, serializable);
            Type type = obj.GetType();
            PropertyInfoExt[] pis = StreamSerializerAttribute.GetWriteProperties(type).ToArray();
            StreamSerializerAttribute? attr = type.GetCustomAttributeCached<StreamSerializerAttribute>();
            bool useChecksum = !(attr?.SkipPropertyNameChecksum ?? false);
            WriteNumberNullable(stream, attr?.Version);
            WriteNumber(stream, pis.Length);
            ObjectTypes objType = default;
            Type? lastItemType = null;
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
                foreach (PropertyInfoExt pi in pis)
                {
                    if (useChecksum && !(pi.Property.GetCustomAttributeCached<StreamSerializerAttribute>()?.SkipPropertyNameChecksum ?? false))
                        Write(stream, pi.Property.Name.GetBytes().Aggregate((c, b) => (byte)(c ^ b)));
                    item = pi.Getter!(obj);
                    if (item == null)
                    {
                        Write(stream, (byte)ObjectTypes.Null);
                        continue;
                    }
                    isComplete = WriteAnyItemHeader(
                        stream,
                        item!,
                        pi.Property.PropertyType,
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
                            WriteItem(stream, item!, nullable: false, itemSerializer, itemSyncSerializer);
                        }
                        else
                        {
                            WriteAny(stream, item!, objType, writeObject);
                        }
                }
            }
            finally
            {
                ArrayPool<int>.Shared.Return(cache);
            }
            return stream;
        }

        /// <summary>
        /// Write any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        public static async Task<Stream> WriteAnyObjectAsync(this Stream stream, object obj, CancellationToken cancellationToken = default)
        {
            if (obj is IStreamSerializer serializable) return await WriteSerializedAsync(stream, serializable, cancellationToken).DynamicContext();
            Type type = obj.GetType();
            PropertyInfoExt[] pis = StreamSerializerAttribute.GetWriteProperties(type).ToArray();
            StreamSerializerAttribute? attr = type.GetCustomAttributeCached<StreamSerializerAttribute>();
            bool useChecksum = !(attr?.SkipPropertyNameChecksum ?? false);
            await WriteNumberNullableAsync(stream, attr?.Version, cancellationToken).DynamicContext();
            await WriteNumberAsync(stream, pis.Length, cancellationToken).DynamicContext();
            ObjectTypes objType = default;
            Type? lastItemType = null;
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
                foreach (PropertyInfoExt pi in pis)
                {
                    if (useChecksum && !(pi.Property.GetCustomAttribute<StreamSerializerAttribute>()?.SkipPropertyNameChecksum ?? false))
                        await WriteAsync(stream, pi.Property.Name.GetBytes().Aggregate((c, b) => (byte)(c ^ b)), cancellationToken).DynamicContext();
                    item = pi.Getter!(obj);
                    if (item == null)
                    {
                        await WriteAsync(stream, (byte)ObjectTypes.Null, cancellationToken).DynamicContext();
                        continue;
                    }
                    (isComplete, lastItemType, itemSerializer, itemSyncSerializer, itemAsyncSerializer, objType, writeObject) = await WriteAnyItemHeaderAsync(
                        stream,
                        item!,
                        pi.Property.PropertyType,
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
                            await WriteItemAsync(stream, item!, nullable: false, itemSerializer, itemSyncSerializer, itemAsyncSerializer, cancellationToken).DynamicContext();
                        }
                        else
                        {
                            await WriteAnyAsync(stream, item!, objType, writeObject, cancellationToken).DynamicContext();
                        }
                }
            }
            finally
            {
                ArrayPool<int>.Shared.Return(cache);
            }
            return stream;
        }

        /// <summary>
        /// Write any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteAnyObjectAsync(this Task<Stream> stream, object obj, CancellationToken cancellationToken = default)
            => AsyncHelper.FluentAsync(stream, obj, cancellationToken, WriteAnyObjectAsync);

        /// <summary>
        /// Write any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream WriteAnyObjectNullable(this Stream stream, object? obj)
            => WriteIfNotNull(stream, obj, () => WriteAnyObject(stream, obj!));

        /// <summary>
        /// Write any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<Stream> WriteAnyObjectNullableAsync(this Stream stream, object? obj, CancellationToken cancellationToken = default)
            => WriteIfNotNullAsync(stream, obj, () => WriteObjectAsync(stream, obj!, cancellationToken), cancellationToken);

        /// <summary>
        /// Write any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteAnyObjectNullableAsync(this Task<Stream> stream, object? obj, CancellationToken cancellationToken = default)
            => AsyncHelper.FluentAsync(stream, obj, cancellationToken, WriteAnyObjectNullableAsync);
    }
}
