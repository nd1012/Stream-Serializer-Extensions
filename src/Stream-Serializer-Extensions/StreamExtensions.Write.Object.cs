using System.Buffers;
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
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream WriteObject(this Stream stream, object obj, ISerializationContext context)
        {
            SerializerException.Wrap(() => ArgumentValidationHelper.EnsureValidArgument(nameof(obj), obj));
            if (obj is IStreamSerializer streamSerializer)
                return WriteSerialized(stream, streamSerializer, context);
            else if (StreamSerializer.FindSerializer(obj.GetType()) is not StreamSerializer.Serializer_Delegate serializer)
                return WriteAnyObject(stream, obj, context);
            else
                SerializerException.Wrap(() => serializer(context, obj));
            return stream;
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<Stream> WriteObjectAsync(this Stream stream, object obj, ISerializationContext context)
        {
            SerializerException.Wrap(() => ArgumentValidationHelper.EnsureValidArgument(nameof(obj), obj));
            if (obj is IStreamSerializer streamSerializer)
                await WriteSerializedAsync(stream, streamSerializer, context).DynamicContext();
            else if (StreamSerializer.FindAsyncSerializer(obj.GetType()) is not StreamSerializer.AsyncSerializer_Delegate serializer)
                await WriteAnyObjectAsync(stream, obj, context).DynamicContext();
            else
                await SerializerException.WrapAsync(async () => await serializer(context, obj).DynamicContext()).DynamicContext();
            return stream;
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteObjectAsync(this Task<Stream> stream, object obj, ISerializationContext context)
            => AsyncHelper.FluentAsync(stream, obj, context, WriteObjectAsync);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream WriteObjectNullable(this Stream stream, object? obj, ISerializationContext context)
            => WriteIfNotNull(stream, obj, () => WriteObject(stream, obj!, context), context);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<Stream> WriteObjectNullableAsync(this Stream stream, object? obj, ISerializationContext context)
            => WriteIfNotNullAsync(stream, obj, () => WriteObjectAsync(stream, obj!, context), context);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteObjectNullableAsync(this Task<Stream> stream, object? obj, ISerializationContext context)
            => AsyncHelper.FluentAsync(stream, obj, context, WriteObjectNullableAsync);

        /// <summary>
        /// Write any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        public static Stream WriteAnyObject(this Stream stream, object obj, ISerializationContext context)
        {
            if (obj is IStreamSerializer serializable) return WriteSerialized(stream, serializable, context);
            using ContextRecursion cr = new(context);
            Type type = obj.GetType();
            PropertyInfoExt[] pis = StreamSerializerAttribute.GetWriteProperties(type).ToArray();
            StreamSerializerAttribute? attr = type.GetCustomAttributeCached<StreamSerializerAttribute>();
            bool useChecksum = !(attr?.SkipPropertyNameChecksum ?? false);
            WriteNumberNullable(stream, attr?.Version, context);
            WriteNumber(stream, pis.Length, context);
            using ItemSerializerContext itemContext = new(context);
            object? item;
            foreach (PropertyInfoExt pi in pis)
            {
                if (useChecksum && !(pi.Property.GetCustomAttributeCached<StreamSerializerAttribute>()?.SkipPropertyNameChecksum ?? false))
                    Write(stream, pi.Property.Name.GetBytes().Aggregate((c, b) => (byte)(c ^ b)), context);
                item = pi.Getter!(obj);
                if (item == null)
                {
                    Write(stream, (byte)ObjectTypes.Null, context);
                    continue;
                }
                if (WriteAnyItemHeader(itemContext, item!, pi.Property.PropertyType) || !itemContext.WriteObject) continue;
                if (itemContext.ItemSerializer == SerializerTypes.Serializer)
                {
                    WriteItem(itemContext, item!);
                }
                else
                {
                    WriteAny(context.Stream, item!, itemContext.ObjectType, itemContext.WriteObject, context);
                }
            }
            return stream;
        }

        /// <summary>
        /// Write any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        public static async Task<Stream> WriteAnyObjectAsync(this Stream stream, object obj, ISerializationContext context)
        {
            if (obj is IStreamSerializer serializable) return await WriteSerializedAsync(stream, serializable, context).DynamicContext();
            using ContextRecursion cr = new(context);
            Type type = obj.GetType();
            PropertyInfoExt[] pis = StreamSerializerAttribute.GetWriteProperties(type).ToArray();
            StreamSerializerAttribute? attr = type.GetCustomAttributeCached<StreamSerializerAttribute>();
            bool useChecksum = !(attr?.SkipPropertyNameChecksum ?? false);
            await WriteNumberNullableAsync(stream, attr?.Version, context).DynamicContext();
            await WriteNumberAsync(stream, pis.Length, context).DynamicContext();
            using ItemSerializerContext itemContext = new(context);
            object? item;
            foreach (PropertyInfoExt pi in pis)
            {
                if (useChecksum && !(pi.Property.GetCustomAttributeCached<StreamSerializerAttribute>()?.SkipPropertyNameChecksum ?? false))
                    await WriteAsync(stream, pi.Property.Name.GetBytes().Aggregate((c, b) => (byte)(c ^ b)), context).DynamicContext();
                item = pi.Getter!(obj);
                if (item == null)
                {
                    await WriteAsync(stream, (byte)ObjectTypes.Null, context).DynamicContext();
                    continue;
                }
                if (await WriteAnyItemHeaderAsync(itemContext, item!, pi.Property.PropertyType).DynamicContext() || !itemContext.WriteObject) continue;
                if (itemContext.ItemSerializer == SerializerTypes.Serializer)
                {
                    await WriteItemAsync(itemContext, item!).DynamicContext();
                }
                else
                {
                    await WriteAnyAsync(context.Stream, item!, itemContext.ObjectType, itemContext.WriteObject, context).DynamicContext();
                }
            }
            return stream;
        }

        /// <summary>
        /// Write any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteAnyObjectAsync(this Task<Stream> stream, object obj, ISerializationContext context)
            => AsyncHelper.FluentAsync(stream, obj, context, WriteAnyObjectAsync);

        /// <summary>
        /// Write any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream WriteAnyObjectNullable(this Stream stream, object? obj, ISerializationContext context)
            => WriteIfNotNull(stream, obj, () => WriteAnyObject(stream, obj!, context), context);

        /// <summary>
        /// Write any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<Stream> WriteAnyObjectNullableAsync(this Stream stream, object? obj, ISerializationContext context)
            => WriteIfNotNullAsync(stream, obj, () => WriteObjectAsync(stream, obj!, context), context);

        /// <summary>
        /// Write any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteAnyObjectNullableAsync(this Task<Stream> stream, object? obj, ISerializationContext context)
            => AsyncHelper.FluentAsync(stream, obj, context, WriteAnyObjectNullableAsync);
    }
}
