using System.Buffers;
using System.ComponentModel.DataAnnotations;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Text;
using wan24.Core;
using wan24.ObjectValidation;

namespace wan24.StreamSerializerExtensions
{
    // Object
    public static partial class StreamExtensions
    {
        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="options">Options</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T ReadObject<T>(this Stream stream, int? version = null, ISerializerOptions? options = null)
            => SerializerException.Wrap(() =>
            {
                if (typeof(IStreamSerializer).IsAssignableFrom(typeof(T))) return (T)ReadSerializedObject(stream, typeof(T), version);
                return StreamSerializer.FindDeserializer(typeof(T)) is StreamSerializer.Deserialize_Delegate deserializer
                    ? (T)(deserializer(stream, typeof(T), version ?? StreamSerializer.Version, options) ?? throw new SerializerException($"{typeof(T)} deserialized to NULL"))
                    : (T)ReadAnyObject(stream, typeof(T), version);
            });

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Type</param>
        /// <param name="version">Serializer version</param>
        /// <param name="options">Options</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object ReadObject(this Stream stream, Type type, int? version = null, ISerializerOptions? options = null)
            => SerializerException.Wrap(() =>
            {
                if (typeof(IStreamSerializer).IsAssignableFrom(type)) return ReadSerializedObject(stream, type, version);
                return StreamSerializer.FindDeserializer(type) is StreamSerializer.Deserialize_Delegate deserializer
                    ? (deserializer(stream, type, version ?? StreamSerializer.Version, options) ?? throw new SerializerException($"{type} deserialized to NULL"))
                    : ReadAnyObject(stream, type, version);
            });

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="options">Options</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<T> ReadObjectAsync<T>(this Stream stream, int? version = null, ISerializerOptions? options = null, CancellationToken cancellationToken = default)
            => SerializerException.WrapAsync(async () =>
            {
                if (typeof(IStreamSerializer).IsAssignableFrom(typeof(T)))
                    return (T)await ReadSerializedObjectAsync(stream, typeof(T), version, cancellationToken).DynamicContext();
                if (StreamSerializer.FindAsyncDeserializer(typeof(T)) is not StreamSerializer.AsyncDeserialize_Delegate deserializer)
                    return StreamSerializer.FindDeserializer(typeof(T)) is not null
                        ? ReadObject<T>(stream, version, options)
                        : (T)ReadAnyObject(stream, typeof(T), version);
                Task task = deserializer(stream, typeof(T), version ?? StreamSerializer.Version, options, cancellationToken);
                await task.DynamicContext();
                return task.GetResult<T>();
            });

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Type</param>
        /// <param name="version">Serializer version</param>
        /// <param name="options">Options</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<object> ReadObjectAsync(
            this Stream stream,
            Type type,
            int? version = null,
            ISerializerOptions? options = null,
            CancellationToken cancellationToken = default
            )
            => SerializerException.WrapAsync(async () =>
            {
                if (typeof(IStreamSerializer).IsAssignableFrom(type))
                    return await ReadSerializedObjectAsync(stream, type, version, cancellationToken).DynamicContext();
                if (StreamSerializer.FindAsyncDeserializer(type) is not StreamSerializer.AsyncDeserialize_Delegate deserializer)
                    return StreamSerializer.FindDeserializer(type) is not null
                        ? ReadObject(stream, type, version, options)
                        : ReadAnyObject(stream, type, version);
                Task task = deserializer(stream, type, version ?? StreamSerializer.Version, options, cancellationToken);
                await task.DynamicContext();
                return task.GetResult(type);
            });

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="options">Options</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T? ReadObjectNullable<T>(this Stream stream, int? version = null, ISerializerOptions? options = null)
#pragma warning disable IDE0034 // default expression can be simplified
            => ReadBool(stream, version) ? ReadObject<T>(stream, version, options) : default(T?);
#pragma warning restore IDE0034 // default expression can be simplified

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Object type</param>
        /// <param name="version">Serializer version</param>
        /// <param name="options">Options</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object? ReadObjectNullable(this Stream stream, Type type, int? version = null, ISerializerOptions? options = null)
            => ReadBool(stream, version) ? ReadObject(stream, type, version, options) : null;

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="options">Options</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<T?> ReadObjectNullableAsync<T>(
            this Stream stream,
            int? version = null,
            ISerializerOptions? options = null,
            CancellationToken cancellationToken = default
            )
            => await ReadBoolAsync(stream, version, cancellationToken: cancellationToken).DynamicContext()
                ? await ReadObjectAsync<T>(stream, version, options, cancellationToken).DynamicContext()
#pragma warning disable IDE0034 // default expression can be simplified
                : default(T?);
#pragma warning restore IDE0034 // default expression can be simplified

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Object type</param>
        /// <param name="version">Serializer version</param>
        /// <param name="options">Options</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<object?> ReadObjectNullableAsync(
            this Stream stream,
            Type type,
            int? version = null,
            ISerializerOptions? options = null,
            CancellationToken cancellationToken = default
            )
            => await ReadBoolAsync(stream, version, cancellationToken: cancellationToken).DynamicContext()
                ? await ReadObjectAsync(stream, type, version, options, cancellationToken).DynamicContext()
                : null;

        /// <summary>
        /// Read any object
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <returns>Object</returns>
        public static T ReadAnyObject<T>(this Stream stream, int? version = null) where T : class, new()
        {
            version ??= StreamSerializer.Version;
            // Handle serializable type
            Type type = typeof(T);
            if (typeof(IStreamSerializer).IsAssignableFrom(type)) return (T)ReadSerializedObject(stream, type, version);
            // Find the stream serializer attribute
            StreamSerializerAttribute? attr = type.GetCustomAttributeCached<StreamSerializerAttribute>();
            if (AnyObjectAttributeRequired && attr == null) throw new SerializerException($"Deserialization of {type} requires the {typeof(StreamSerializerAttribute)}");
            // Get properties to read
            PropertyInfoExt[] pis = StreamSerializerAttribute.GetReadProperties(type, ReadNumberNullable<int>(stream, version)).ToArray();
            int count = ReadNumber<int>(stream, version);
            if (count != pis.Length) throw new SerializerException($"The serialized type has {count} properties, while {type} has {pis.Length} properties");
            // Deserialize property values
            bool useChecksum = !(attr?.SkipPropertyNameChecksum ?? false);
            PropertyInfoExt pi;
            T res = new();
            Type? itemType = null;
            ObjectTypes objType = default,
                lastObjType = default;
            SerializerTypes itemSerializer = default;
            StreamSerializer.Deserialize_Delegate? itemSyncDeserializer = null;
            Type[]? typeCache = null;
            object[]? objectCache = null;
            Span<Type> typeCacheSpan;
            ReadOnlySpan<object> objectCacheSpan;
            object? obj;
            int objIndex;
            try
            {
                typeCache = ArrayPool<Type>.Shared.RentClean(byte.MaxValue);
                typeCacheSpan = typeCache.AsSpan(0, byte.MaxValue);
                objectCache = ArrayPool<object>.Shared.RentClean(byte.MaxValue);
                objectCacheSpan = objectCache.AsSpan(0, byte.MaxValue);
                for (int i = 0; i < count; i++)
                {
                    pi = pis[i];
                    // Validate the property name
                    if (
                        useChecksum &&
                        !(pi.Property.GetCustomAttributeCached<StreamSerializerAttribute>()?.SkipPropertyNameChecksum ?? false) &&
                        ReadOneByte(stream, version) != pi.Property.Name.GetBytes().Aggregate((c, b) => (byte)(c ^ b))
                        )
                        throw new SerializerException($"{type}.{pi.Property.Name} property name checksum mismatch");
                    // Deserialize the property value
                    obj = ReadAnyItemHeader(
                        stream,
                        version.Value,
                        pi.PropertyType,
                        i,
                        typeCache,
                        objectCache,
                        ref objType,
                        ref lastObjType,
                        ref itemType,
                        ref itemSerializer,
                        ref itemSyncDeserializer
                        );
                    if (obj == null && objType == ObjectTypes.Null)
                    {
                        if (!pi.PropertyType.IsNullable())
                            throw new SerializerException($"Deserialized NULL for non-NULL property {type}.{pi.Property.Name}", new InvalidDataException());
                        pi.Setter!(res, null);
                    }
                    else if (obj == null)
                    {
                        pi.Setter!(res, itemSerializer == SerializerTypes.Serializer
                            ? obj = ReadItem(stream, version.Value, nullable: false, itemSerializer, itemType, pool: null, options: null, itemSyncDeserializer)
                            : obj = ReadAnyInt(stream, version.Value, objType, itemType, options: null));
                        objIndex = objectCache.IndexOf(null);
                        if (objIndex != -1) objectCache[objIndex] = obj!;
                    }
                    else
                    {
                        pi.Setter!(res, obj);
                    }
                }
            }
            finally
            {
                if (typeCache != null) ArrayPool<Type>.Shared.Return(typeCache);
                if (objectCache != null) ArrayPool<object>.Shared.Return(objectCache);
            }
            // Validate the resulting object
            if (!res.TryValidateObject(out List<ValidationResult> results))
                throw new SerializerException(
                    $"The deserialized object contains {results.Count} errors: {results[0].ErrorMessage} ({string.Join(',', results[0].MemberNames)})",
                    new ObjectValidationException(results)
                    );
            return res;
        }

        /// <summary>
        /// Read any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Object type</param>
        /// <param name="version">Serializer version</param>
        /// <returns>Object</returns>
        public static object ReadAnyObject(this Stream stream, Type type, int? version = null)
        {
            version ??= StreamSerializer.Version;
            // Handle serializable type
            if (typeof(IStreamSerializer).IsAssignableFrom(type)) return ReadSerializedObject(stream, type, version);
            // Find the stream serializer attribute
            StreamSerializerAttribute? attr = type.GetCustomAttributeCached<StreamSerializerAttribute>();
            if (AnyObjectAttributeRequired && attr == null) throw new SerializerException($"Deserialization of {type} requires the {typeof(StreamSerializerAttribute)}");
            // Get properties to read
            PropertyInfoExt[] pis = StreamSerializerAttribute.GetReadProperties(type, ReadNumberNullable<int>(stream, version)).ToArray();
            int count = ReadNumber<int>(stream, version);
            if (count != pis.Length) throw new SerializerException($"The serialized type has {count} properties, while {type} has {pis.Length} properties");
            // Deserialize property values
            bool useChecksum = !(attr?.SkipPropertyNameChecksum ?? false);
            PropertyInfoExt pi;
            object res = Activator.CreateInstance(type) ?? throw new SerializerException($"Failed to instance {type}");
            Type? itemType = null;
            ObjectTypes objType = default,
                lastObjType = default;
            SerializerTypes itemSerializer = default;
            StreamSerializer.Deserialize_Delegate? itemSyncDeserializer = null;
            Type[]? typeCache = null;
            object[]? objectCache = null;
            Span<Type> typeCacheSpan;
            ReadOnlySpan<object> objectCacheSpan;
            object? obj;
            int objIndex;
            try
            {
                typeCache = ArrayPool<Type>.Shared.RentClean(byte.MaxValue);
                typeCacheSpan = typeCache.AsSpan(0, byte.MaxValue);
                objectCache = ArrayPool<object>.Shared.RentClean(byte.MaxValue);
                objectCacheSpan = objectCache.AsSpan(0, byte.MaxValue);
                for (int i = 0; i < count; i++)
                {
                    pi = pis[i];
                    // Validate the property name
                    if (
                        useChecksum &&
                        !(pi.Property.GetCustomAttributeCached<StreamSerializerAttribute>()?.SkipPropertyNameChecksum ?? false) &&
                        ReadOneByte(stream, version) != pi.Property.Name.GetBytes().Aggregate((c, b) => (byte)(c ^ b))
                        )
                        throw new SerializerException($"{type}.{pi.Property.Name} property name checksum mismatch");
                    // Deserialize the property value
                    obj = ReadAnyItemHeader(
                        stream,
                        version.Value,
                        pi.PropertyType,
                        i,
                        typeCache,
                        objectCache,
                        ref objType,
                        ref lastObjType,
                        ref itemType,
                        ref itemSerializer,
                        ref itemSyncDeserializer
                        );
                    if (obj == null && objType == ObjectTypes.Null)
                    {
                        if (!pi.PropertyType.IsNullable())
                            throw new SerializerException($"Deserialized NULL for non-NULL property {type}.{pi.Property.Name}", new InvalidDataException());
                        pi.Setter!(res, null);
                    }
                    else if (obj == null)
                    {
                        pi.Setter!(res, itemSerializer == SerializerTypes.Serializer
                            ? obj = ReadItem(stream, version.Value, nullable: false, itemSerializer, itemType, pool: null, options: null, itemSyncDeserializer)
                            : obj = ReadAnyInt(stream, version.Value, objType, itemType, options: null));
                        objIndex = objectCache.IndexOf(null);
                        if (objIndex != -1) objectCache[objIndex] = obj!;
                    }
                    else
                    {
                        pi.Setter!(res, obj);
                    }
                }
            }
            finally
            {
                if (typeCache != null) ArrayPool<Type>.Shared.Return(typeCache);
                if (objectCache != null) ArrayPool<object>.Shared.Return(objectCache);
            }
            // Validate the resulting object
            if (!res.TryValidateObject(out List<ValidationResult> results))
                throw new SerializerException(
                    $"The deserialized object contains {results.Count} errors: {results[0].ErrorMessage} ({string.Join(',', results[0].MemberNames)})",
                    new ObjectValidationException(results)
                    );
            return res;
        }

        /// <summary>
        /// Read any object
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Object</returns>
        public static async Task<T> ReadAnyObjectAsync<T>(this Stream stream, int? version = null, CancellationToken cancellationToken = default) where T : class, new()
        {
            version ??= StreamSerializer.Version;
            // Handle serializable type
            Type type = typeof(T);
            if (typeof(IStreamSerializer).IsAssignableFrom(type))
                return (T)await ReadSerializedObjectAsync(stream, type, version, cancellationToken).DynamicContext();
            // Find the stream serializer attribute
            StreamSerializerAttribute? attr = type.GetCustomAttributeCached<StreamSerializerAttribute>();
            if (AnyObjectAttributeRequired && attr == null) throw new SerializerException($"Deserialization of {type} requires the {typeof(StreamSerializerAttribute)}");
            // Get properties to read
            PropertyInfoExt[] pis = StreamSerializerAttribute.GetReadProperties(
                type,
                await ReadNumberNullableAsync<int>(stream, version, cancellationToken: cancellationToken).DynamicContext()
                ).ToArray();
            int count = await ReadNumberAsync<int>(stream, version, cancellationToken: cancellationToken).DynamicContext();
            if (count != pis.Length) throw new SerializerException($"The serialized type has {count} properties, while {type} has {pis.Length} properties");
            // Deserialize property values
            bool useChecksum = !(attr?.SkipPropertyNameChecksum ?? false);
            PropertyInfoExt pi;
            T res = new();
            Type? itemType = null;
            ObjectTypes objType = default,
                lastObjType = default;
            SerializerTypes itemSerializer = default;
            StreamSerializer.Deserialize_Delegate? itemSyncDeserializer = null;
            StreamSerializer.AsyncDeserialize_Delegate? itemAsyncDeserializer = null;
            Type[]? typeCache = null;
            object[]? objectCache = null;
            Memory<Type> typeCacheMem;
            ReadOnlyMemory<object> objectCacheMem;
            object? obj;
            int objIndex;
            try
            {
                typeCache = ArrayPool<Type>.Shared.RentClean(byte.MaxValue);
                typeCacheMem = typeCache.AsMemory(0, byte.MaxValue);
                objectCache = ArrayPool<object>.Shared.RentClean(byte.MaxValue);
                objectCacheMem = objectCache.AsMemory(0, byte.MaxValue);
                for (int i = 0; i < count; i++)
                {
                    pi = pis[i];
                    // Validate the property name
                    if (
                        useChecksum &&
                        !(pi.Property.GetCustomAttributeCached<StreamSerializerAttribute>()?.SkipPropertyNameChecksum ?? false) &&
                        await ReadOneByteAsync(stream, version, cancellationToken).DynamicContext() != pi.Property.Name.GetBytes().Aggregate((c, b) => (byte)(c ^ b))
                        )
                        throw new SerializerException($"{type}.{pi.Property.Name} property name checksum mismatch");
                    // Deserialize the property value
                    (obj, objType, lastObjType,  itemType, itemSerializer, itemSyncDeserializer, itemAsyncDeserializer) =
                        await ReadAnyItemHeaderAsync(
                            stream,
                            version.Value,
                            pi.PropertyType,
                            i,
                            typeCacheMem,
                            objectCacheMem,
                            lastObjType,
                            itemType,
                            itemSerializer,
                            itemSyncDeserializer,
                            itemAsyncDeserializer,
                            cancellationToken
                            ).DynamicContext();
                    if (obj == null && objType == ObjectTypes.Null)
                    {
                        if (!pi.PropertyType.IsNullable())
                            throw new SerializerException($"Deserialized NULL for non-NULL property {type}.{pi.Property.Name}", new InvalidDataException());
                        pi.Setter!(res, null);
                    }
                    else if (obj == null)
                    {
                        pi.Setter!(res, obj = itemSerializer == SerializerTypes.Serializer
                            ? await ReadItemAsync(
                                stream,
                                version.Value,
                                nullable: false,
                                itemSerializer,
                                itemType,
                                pool: null,
                                options: null,
                                itemSyncDeserializer,
                                itemAsyncDeserializer,
                                cancellationToken
                                ).DynamicContext()
                            : await ReadAnyIntAsync(stream, version.Value, objType, itemType, options: null, cancellationToken).DynamicContext());
                        objIndex = objectCache.IndexOf(null);
                        if (objIndex != -1) objectCache[objIndex] = obj!;
                    }
                    else
                    {
                        pi.Setter!(res, obj);
                    }
                }
            }
            finally
            {
                if (typeCache != null) ArrayPool<Type>.Shared.Return(typeCache);
                if (objectCache != null) ArrayPool<object>.Shared.Return(objectCache);
            }
            // Validate the resulting object
            if (!res.TryValidateObject(out List<ValidationResult> results))
                throw new SerializerException(
                    $"The deserialized object contains {results.Count} errors: {results[0].ErrorMessage} ({string.Join(',', results[0].MemberNames)})",
                    new ObjectValidationException(results)
                    );
            return res;
        }

        /// <summary>
        /// Read any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Object type</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Object</returns>
        public static async Task<object> ReadAnyObjectAsync(this Stream stream, Type type, int? version = null, CancellationToken cancellationToken = default)
        {
            version ??= StreamSerializer.Version;
            // Handle serializable type
            if (typeof(IStreamSerializer).IsAssignableFrom(type))
                return await ReadSerializedObjectAsync(stream, type, version, cancellationToken).DynamicContext();
            // Find the stream serializer attribute
            StreamSerializerAttribute? attr = type.GetCustomAttributeCached<StreamSerializerAttribute>();
            if (AnyObjectAttributeRequired && attr == null) throw new SerializerException($"Deserialization of {type} requires the {typeof(StreamSerializerAttribute)}");
            // Get properties to read
            PropertyInfoExt[] pis = StreamSerializerAttribute.GetReadProperties(
                type,
                await ReadNumberNullableAsync<int>(stream, version, cancellationToken: cancellationToken).DynamicContext()
                ).ToArray();
            int count = await ReadNumberAsync<int>(stream, version, cancellationToken: cancellationToken).DynamicContext();
            if (count != pis.Length) throw new SerializerException($"The serialized type has {count} properties, while {type} has {pis.Length} properties");
            // Deserialize property values
            bool useChecksum = !(attr?.SkipPropertyNameChecksum ?? false);
            PropertyInfoExt pi;
            object res = Activator.CreateInstance(type) ?? throw new SerializerException($"Failed to instance {type}");
            Type? itemType = null;
            ObjectTypes objType = default,
                lastObjType = default;
            SerializerTypes itemSerializer = default;
            StreamSerializer.Deserialize_Delegate? itemSyncDeserializer = null;
            StreamSerializer.AsyncDeserialize_Delegate? itemAsyncDeserializer = null;
            Type[]? typeCache = null;
            object[]? objectCache = null;
            Memory<Type> typeCacheMem;
            ReadOnlyMemory<object> objectCacheMem;
            object? obj;
            int objIndex;
            try
            {
                typeCache = ArrayPool<Type>.Shared.RentClean(byte.MaxValue);
                typeCacheMem = typeCache.AsMemory(0, byte.MaxValue);
                objectCache = ArrayPool<object>.Shared.RentClean(byte.MaxValue);
                objectCacheMem = objectCache.AsMemory(0, byte.MaxValue);
                for (int i = 0; i < count; i++)
                {
                    pi = pis[i];
                    // Validate the property name
                    if (
                        useChecksum &&
                        !(pi.Property.GetCustomAttributeCached<StreamSerializerAttribute>()?.SkipPropertyNameChecksum ?? false) &&
                        await ReadOneByteAsync(stream, version, cancellationToken).DynamicContext() != pi.Property.Name.GetBytes().Aggregate((c, b) => (byte)(c ^ b))
                        )
                        throw new SerializerException($"{type}.{pi.Property.Name} property name checksum mismatch");
                    // Deserialize the property value
                    (obj, objType, lastObjType, itemType, itemSerializer, itemSyncDeserializer, itemAsyncDeserializer) =
                        await ReadAnyItemHeaderAsync(
                            stream,
                            version.Value,
                            pi.PropertyType,
                            i,
                            typeCacheMem,
                            objectCacheMem,
                            lastObjType,
                            itemType,
                            itemSerializer,
                            itemSyncDeserializer,
                            itemAsyncDeserializer,
                            cancellationToken
                            ).DynamicContext();
                    if (obj == null && objType == ObjectTypes.Null)
                    {
                        if (!pi.PropertyType.IsNullable())
                            throw new SerializerException($"Deserialized NULL for non-NULL property {type}.{pi.Property.Name}", new InvalidDataException());
                        pi.Setter!(res, null);
                    }
                    else if (obj == null)
                    {
                        pi.Setter!(res, obj = itemSerializer == SerializerTypes.Serializer
                            ? await ReadItemAsync(
                                stream,
                                version.Value,
                                nullable: false,
                                itemSerializer,
                                itemType,
                                pool: null,
                                options: null,
                                itemSyncDeserializer,
                                itemAsyncDeserializer,
                                cancellationToken
                                ).DynamicContext()
                            : await ReadAnyIntAsync(stream, version.Value, objType, itemType, options: null, cancellationToken).DynamicContext());
                        objIndex = objectCache.IndexOf(null);
                        if (objIndex != -1) objectCache[objIndex] = obj!;
                    }
                    else
                    {
                        pi.Setter!(res, obj);
                    }
                }
            }
            finally
            {
                if (typeCache != null) ArrayPool<Type>.Shared.Return(typeCache);
                if (objectCache != null) ArrayPool<object>.Shared.Return(objectCache);
            }
            // Validate the resulting object
            if (!res.TryValidateObject(out List<ValidationResult> results))
                throw new SerializerException(
                    $"The deserialized object contains {results.Count} errors: {results[0].ErrorMessage} ({string.Join(',', results[0].MemberNames)})",
                    new ObjectValidationException(results)
                    );
            return res;
        }

        /// <summary>
        /// Read any object
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <returns>Object</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T? ReadAnyObjectNullable<T>(this Stream stream, int? version = null) where T : class, new()
            => ReadBool(stream, version) ? ReadAnyObject<T>(stream, version) : null;

        /// <summary>
        /// Read any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Object type</param>
        /// <param name="version">Serializer version</param>
        /// <returns>Object</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object? ReadAnyObjectNullable(this Stream stream, Type type, int? version = null)
            => ReadBool(stream, version) ? ReadAnyObject(stream, type, version) : null;

        /// <summary>
        /// Read any object
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Object</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<T?> ReadAnyObjectNullableAsync<T>(this Stream stream, int? version = null, CancellationToken cancellationToken = default)
            where T : class, new()
            => await ReadBoolAsync(stream, version, cancellationToken: cancellationToken).DynamicContext()
                ? await ReadAnyObjectAsync<T>(stream, version, cancellationToken).DynamicContext()
                : null;

        /// <summary>
        /// Read any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Object type</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Object</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<object?> ReadAnyObjectNullableAsync(this Stream stream, Type type, int? version = null, CancellationToken cancellationToken = default)
            => await ReadBoolAsync(stream, version, cancellationToken: cancellationToken).DynamicContext()
                ? await ReadAnyObjectAsync(stream, type, version, cancellationToken).DynamicContext()
                : null;
    }
}
