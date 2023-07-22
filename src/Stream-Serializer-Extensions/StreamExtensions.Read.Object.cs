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
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T ReadObject<T>(this Stream stream, IDeserializationContext context)
            => SerializerException.Wrap(() =>
            {
                if (typeof(IStreamSerializer).IsAssignableFrom(typeof(T))) return (T)ReadSerializedObject(stream, typeof(T), context);
                return StreamSerializer.FindDeserializer(typeof(T)) is StreamSerializer.Deserializer_Delegate deserializer
                    ? (T)(deserializer(context, typeof(T)) ?? throw new SerializerException($"{typeof(T)} deserialized to NULL"))
                    : (T)ReadAnyObject(stream, typeof(T), context);
            });

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Type</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object ReadObject(this Stream stream, Type type, IDeserializationContext context)
            => SerializerException.Wrap(() =>
            {
                if (typeof(IStreamSerializer).IsAssignableFrom(type)) return ReadSerializedObject(stream, type, context);
                return StreamSerializer.FindDeserializer(type) is StreamSerializer.Deserializer_Delegate deserializer
                    ? (deserializer(context, type) ?? throw new SerializerException($"{type} deserialized to NULL"))
                    : ReadAnyObject(stream, type, context);
            });

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<T> ReadObjectAsync<T>(this Stream stream, IDeserializationContext context)
            => SerializerException.WrapAsync(async () =>
            {
                if (typeof(IStreamSerializer).IsAssignableFrom(typeof(T)))
                    return (T)await ReadSerializedObjectAsync(stream, typeof(T), context).DynamicContext();
                if (StreamSerializer.FindAsyncDeserializer(typeof(T)) is not StreamSerializer.AsyncDeserializer_Delegate deserializer)
                    return StreamSerializer.FindDeserializer(typeof(T)) is not null
                        ? ReadObject<T>(stream, context)
                        : (T)ReadAnyObject(stream, typeof(T), context);
                Task task = deserializer(context, typeof(T));
                await task.DynamicContext();
                return task.GetResult<T>();
            });

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Type</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<object> ReadObjectAsync(this Stream stream, Type type, IDeserializationContext context)
            => SerializerException.WrapAsync(async () =>
            {
                if (typeof(IStreamSerializer).IsAssignableFrom(type))
                    return await ReadSerializedObjectAsync(stream, type, context).DynamicContext();
                if (StreamSerializer.FindAsyncDeserializer(type) is not StreamSerializer.AsyncDeserializer_Delegate deserializer)
                    return StreamSerializer.FindDeserializer(type) is not null
                        ? ReadObject(stream, type, context)
                        : ReadAnyObject(stream, type, context);
                Task task = deserializer(context, type);
                await task.DynamicContext();
                return task.GetResult(type);
            });

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T? ReadObjectNullable<T>(this Stream stream, IDeserializationContext context)
#pragma warning disable IDE0034 // default expression can be simplified
            => ReadBool(stream, context) ? ReadObject<T>(stream, context) : default(T?);
#pragma warning restore IDE0034 // default expression can be simplified

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Object type</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object? ReadObjectNullable(this Stream stream, Type type, IDeserializationContext context)
            => ReadBool(stream, context) ? ReadObject(stream, type, context) : null;

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<T?> ReadObjectNullableAsync<T>(this Stream stream, IDeserializationContext context)
            => await ReadBoolAsync(stream, context).DynamicContext()
                ? await ReadObjectAsync<T>(stream, context).DynamicContext()
#pragma warning disable IDE0034 // default expression can be simplified
                : default(T?);
#pragma warning restore IDE0034 // default expression can be simplified

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Object type</param>
        /// <param name="context">Context</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<object?> ReadObjectNullableAsync(this Stream stream, Type type, IDeserializationContext context)
            => await ReadBoolAsync(stream, context).DynamicContext()
                ? await ReadObjectAsync(stream, type, context).DynamicContext()
                : null;

        /// <summary>
        /// Read any object
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Object</returns>
        public static T ReadAnyObject<T>(this Stream stream, IDeserializationContext context) where T : class, new()
        {
            using ContextRecursion cr = new(context);
            // Handle serializable type
            Type type = typeof(T);
            if (typeof(IStreamSerializer).IsAssignableFrom(type)) return (T)ReadSerializedObject(stream, type, context);
            // Find the stream serializer attribute
            StreamSerializerAttribute? attr = type.GetCustomAttributeCached<StreamSerializerAttribute>();
            if (AnyObjectAttributeRequired && attr == null) throw new SerializerException($"Deserialization of {type} requires the {typeof(StreamSerializerAttribute)}");
            // Get properties to read
            PropertyInfoExt[] pis = StreamSerializerAttribute.GetReadProperties(type, ReadNumberNullable<int>(stream, context)).ToArray();
            int count = ReadNumber<int>(stream, context);
            if (count != pis.Length) throw new SerializerException($"The serialized type has {count} properties, while {type} has {pis.Length} properties");
            // Deserialize property values
            bool useChecksum = !(attr?.SkipPropertyNameChecksum ?? false);
            PropertyInfoExt pi;
            T res = new();
            object? obj;
            using ItemDeserializerContext itemContext = new(context);
            ISerializerOptions? options;
            for (int i = 0; i < count; i++)
            {
                pi = pis[i];
                attr = pi.Property.GetCustomAttributeCached<StreamSerializerAttribute>();
                options = attr?.GetSerializerOptions(pi, context);
                context.WithOptions(options);
                itemContext.Nullable = options?.IsNullable ?? pi.Property.IsNullable();
                // Validate the property name
                if (
                    useChecksum &&
                    !(
                        attr?.SkipPropertyNameChecksum ?? false) &&
                        ReadOneByte(stream, context) != pi.Property.Name.GetBytes().Aggregate((c, b) => (byte)(c ^ b)
                    )
                    )
                    throw new SerializerException($"{type}.{pi.Property.Name} property name checksum mismatch");
                // Deserialize the property value
                obj = ReadAnyItemHeader(itemContext, i, pi.PropertyType);
                if (obj == null && itemContext.ObjectType == ObjectTypes.Null)
                {
                    if (!itemContext.Nullable)
                        throw new SerializerException($"Deserialized NULL for non-NULL property {type}.{pi.Property.Name}", new InvalidDataException());
                    pi.Setter!(res, null);
                }
                else if (obj == null)
                {
                    pi.Setter!(res, itemContext.ItemSerializer == SerializerTypes.Serializer
                        ? obj = ReadItem(itemContext)
                        : obj = ReadAnyInt(context, itemContext.ObjectType, itemContext.ItemType));
                    if (itemContext.ObjectType.RequiresObjectWriting()) itemContext.AddObject(obj);
                }
                else
                {
                    pi.Setter!(res, obj);
                }
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
        /// <param name="context">Context</param>
        /// <returns>Object</returns>
        public static object ReadAnyObject(this Stream stream, Type type, IDeserializationContext context)
        {
            using ContextRecursion cr = new(context);
            // Handle serializable type
            if (typeof(IStreamSerializer).IsAssignableFrom(type)) return ReadSerializedObject(stream, type, context);
            // Find the stream serializer attribute
            StreamSerializerAttribute? attr = type.GetCustomAttributeCached<StreamSerializerAttribute>();
            if (AnyObjectAttributeRequired && attr == null) throw new SerializerException($"Deserialization of {type} requires the {typeof(StreamSerializerAttribute)}");
            // Get properties to read
            PropertyInfoExt[] pis = StreamSerializerAttribute.GetReadProperties(type, ReadNumberNullable<int>(stream, context)).ToArray();
            int count = ReadNumber<int>(stream, context);
            if (count != pis.Length) throw new SerializerException($"The serialized type has {count} properties, while {type} has {pis.Length} properties");
            // Deserialize property values
            bool useChecksum = !(attr?.SkipPropertyNameChecksum ?? false);
            PropertyInfoExt pi;
            object res = Activator.CreateInstance(type) ?? throw new SerializerException($"Failed to instance {type}");
            object? obj;
            using ItemDeserializerContext itemContext = new(context);
            ISerializerOptions? options;
            for (int i = 0; i < count; i++)
            {
                pi = pis[i];
                attr = pi.Property.GetCustomAttributeCached<StreamSerializerAttribute>();
                options = attr?.GetSerializerOptions(pi, context);
                context.WithOptions(options);
                itemContext.Nullable = options?.IsNullable ?? pi.Property.IsNullable();
                // Validate the property name
                if (
                    useChecksum &&
                    !(
                        attr?.SkipPropertyNameChecksum ?? false) &&
                        ReadOneByte(stream, context) != pi.Property.Name.GetBytes().Aggregate((c, b) => (byte)(c ^ b)
                    )
                    )
                    throw new SerializerException($"{type}.{pi.Property.Name} property name checksum mismatch");
                // Deserialize the property value
                obj = ReadAnyItemHeader(itemContext, i, pi.PropertyType);
                if (obj == null && itemContext.ObjectType == ObjectTypes.Null)
                {
                    if (!itemContext.Nullable)
                        throw new SerializerException($"Deserialized NULL for non-NULL property {type}.{pi.Property.Name}", new InvalidDataException());
                    pi.Setter!(res, null);
                }
                else if (obj == null)
                {
                    pi.Setter!(res, itemContext.ItemSerializer == SerializerTypes.Serializer
                        ? obj = ReadItem(itemContext)
                        : obj = ReadAnyInt(context, itemContext.ObjectType, itemContext.ItemType));
                    if (itemContext.ObjectType.RequiresObjectWriting()) itemContext.AddObject(obj);
                }
                else
                {
                    pi.Setter!(res, obj);
                }
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
        /// <param name="context">Context</param>
        /// <returns>Object</returns>
        public static async Task<T> ReadAnyObjectAsync<T>(this Stream stream, IDeserializationContext context) where T : class, new()
        {
            using ContextRecursion cr = new(context);
            // Handle serializable type
            Type type = typeof(T);
            if (typeof(IStreamSerializer).IsAssignableFrom(type))
                return (T)await ReadSerializedObjectAsync(stream, type, context).DynamicContext();
            // Find the stream serializer attribute
            StreamSerializerAttribute? attr = type.GetCustomAttributeCached<StreamSerializerAttribute>();
            if (AnyObjectAttributeRequired && attr == null) throw new SerializerException($"Deserialization of {type} requires the {typeof(StreamSerializerAttribute)}");
            // Get properties to read
            PropertyInfoExt[] pis = StreamSerializerAttribute.GetReadProperties(
                type,
                await ReadNumberNullableAsync<int>(stream, context).DynamicContext()
                ).ToArray();
            int count = await ReadNumberAsync<int>(stream, context).DynamicContext();
            if (count != pis.Length) throw new SerializerException($"The serialized type has {count} properties, while {type} has {pis.Length} properties");
            // Deserialize property values
            bool useChecksum = !(attr?.SkipPropertyNameChecksum ?? false);
            PropertyInfoExt pi;
            T res = new();
            object? obj;
            using ItemDeserializerContext itemContext = new(context);
            ISerializerOptions? options;
            for (int i = 0; i < count; i++)
            {
                pi = pis[i];
                attr = pi.Property.GetCustomAttributeCached<StreamSerializerAttribute>();
                options = attr?.GetSerializerOptions(pi, context);
                context.WithOptions(options);
                itemContext.Nullable = options?.IsNullable ?? pi.Property.IsNullable();
                // Validate the property name
                if (
                    useChecksum &&
                    !(
                        attr?.SkipPropertyNameChecksum ?? false) &&
                        await ReadOneByteAsync(stream, context).DynamicContext() != pi.Property.Name.GetBytes().Aggregate((c, b) => (byte)(c ^ b)
                    )
                    )
                    throw new SerializerException($"{type}.{pi.Property.Name} property name checksum mismatch");
                // Deserialize the property value
                obj = await ReadAnyItemHeaderAsync(itemContext, i, pi.PropertyType).DynamicContext();
                if (obj == null && itemContext.ObjectType == ObjectTypes.Null)
                {
                    if (!itemContext.Nullable)
                        throw new SerializerException($"Deserialized NULL for non-NULL property {type}.{pi.Property.Name}", new InvalidDataException());
                    pi.Setter!(res, null);
                }
                else if (obj == null)
                {
                    pi.Setter!(res, obj = itemContext.ItemSerializer == SerializerTypes.Serializer
                        ? await ReadItemAsync(itemContext).DynamicContext()
                        : await ReadAnyIntAsync(context, itemContext.ObjectType, itemContext.ItemType).DynamicContext());
                    if (itemContext.ObjectType.RequiresObjectWriting()) itemContext.AddObject(obj);
                }
                else
                {
                    pi.Setter!(res, obj);
                }
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
        /// <param name="context">Context</param>
        /// <returns>Object</returns>
        public static async Task<object> ReadAnyObjectAsync(this Stream stream, Type type, IDeserializationContext context)
        {
            using ContextRecursion cr = new(context);
            // Handle serializable type
            if (typeof(IStreamSerializer).IsAssignableFrom(type)) return await ReadSerializedObjectAsync(stream, type, context).DynamicContext();
            // Find the stream serializer attribute
            StreamSerializerAttribute? attr = type.GetCustomAttributeCached<StreamSerializerAttribute>();
            if (AnyObjectAttributeRequired && attr == null) throw new SerializerException($"Deserialization of {type} requires the {typeof(StreamSerializerAttribute)}");
            // Get properties to read
            PropertyInfoExt[] pis = StreamSerializerAttribute.GetReadProperties(type, await ReadNumberNullableAsync<int>(stream, context).DynamicContext()).ToArray();
            int count = await ReadNumberAsync<int>(stream, context).DynamicContext();
            if (count != pis.Length) throw new SerializerException($"The serialized type has {count} properties, while {type} has {pis.Length} properties");
            // Deserialize property values
            bool useChecksum = !(attr?.SkipPropertyNameChecksum ?? false);
            PropertyInfoExt pi;
            object res = Activator.CreateInstance(type) ?? throw new SerializerException($"Failed to instance {type}");
            object? obj;
            using ItemDeserializerContext itemContext = new(context);
            ISerializerOptions? options;
            for (int i = 0; i < count; i++)
            {
                pi = pis[i];
                attr = pi.Property.GetCustomAttributeCached<StreamSerializerAttribute>();
                options = attr?.GetSerializerOptions(pi, context);
                context.WithOptions(options);
                itemContext.Nullable = options?.IsNullable ?? pi.Property.IsNullable();
                // Validate the property name
                if (
                    useChecksum &&
                    !(
                        attr?.SkipPropertyNameChecksum ?? false) &&
                        await ReadOneByteAsync(stream, context).DynamicContext() != pi.Property.Name.GetBytes().Aggregate((c, b) => (byte)(c ^ b)
                    )
                    )
                    throw new SerializerException($"{type}.{pi.Property.Name} property name checksum mismatch");
                // Deserialize the property value
                obj = await ReadAnyItemHeaderAsync(itemContext, i, pi.PropertyType).DynamicContext();
                if (obj == null && itemContext.ObjectType == ObjectTypes.Null)
                {
                    if (!itemContext.Nullable)
                        throw new SerializerException($"Deserialized NULL for non-NULL property {type}.{pi.Property.Name}", new InvalidDataException());
                    pi.Setter!(res, null);
                }
                else if (obj == null)
                {
                    pi.Setter!(res, itemContext.ItemSerializer == SerializerTypes.Serializer
                        ? obj = await ReadItemAsync(itemContext).DynamicContext()
                        : obj = await ReadAnyIntAsync(context, itemContext.ObjectType, itemContext.ItemType).DynamicContext());
                    if (itemContext.ObjectType.RequiresObjectWriting()) itemContext.AddObject(obj);
                }
                else
                {
                    pi.Setter!(res, obj);
                }
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
        /// <param name="context">Context</param>
        /// <returns>Object</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T? ReadAnyObjectNullable<T>(this Stream stream, IDeserializationContext context) where T : class, new()
            => ReadBool(stream, context) ? ReadAnyObject<T>(stream, context) : null;

        /// <summary>
        /// Read any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Object type</param>
        /// <param name="context">Context</param>
        /// <returns>Object</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object? ReadAnyObjectNullable(this Stream stream, Type type, IDeserializationContext context)
            => ReadBool(stream, context) ? ReadAnyObject(stream, type, context) : null;

        /// <summary>
        /// Read any object
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Object</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<T?> ReadAnyObjectNullableAsync<T>(this Stream stream, IDeserializationContext context)
            where T : class, new()
            => await ReadBoolAsync(stream, context).DynamicContext()
                ? await ReadAnyObjectAsync<T>(stream, context).DynamicContext()
                : null;

        /// <summary>
        /// Read any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Object type</param>
        /// <param name="context">Context</param>
        /// <returns>Object</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<object?> ReadAnyObjectNullableAsync(this Stream stream, Type type, IDeserializationContext context)
            => await ReadBoolAsync(stream, context).DynamicContext()
                ? await ReadAnyObjectAsync(stream, type, context).DynamicContext()
                : null;
    }
}
