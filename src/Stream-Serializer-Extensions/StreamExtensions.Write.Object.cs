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
            => FluentAsync(stream, (s) => WriteObjectAsync(s, obj, cancellationToken));

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
            => FluentAsync(stream, (s) => WriteObjectNullableAsync(s, obj, cancellationToken));

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
            foreach (PropertyInfoExt pi in pis)
            {
                if (useChecksum && !(pi.Property.GetCustomAttributeCached<StreamSerializerAttribute>()?.SkipPropertyNameChecksum ?? false))
                    Write(stream, pi.Property.Name.GetBytes().Aggregate((c, b) => (byte)(c ^ b)));
                if (pi.Property.PropertyType.IsNullable())
                {
                    WriteAnyNullable(stream, pi.Getter!(obj)!);
                }
                else
                {
                    WriteAny(stream, pi.Getter!(obj)!);
                }
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
            foreach (PropertyInfoExt pi in pis)
            {
                if (useChecksum && !(pi.Property.GetCustomAttribute<StreamSerializerAttribute>()?.SkipPropertyNameChecksum ?? false))
                    await WriteAsync(stream, pi.Property.Name.GetBytes().Aggregate((c, b) => (byte)(c ^ b)), cancellationToken).DynamicContext();
                if (pi.Property.PropertyType.IsNullable())
                {
                    await WriteAnyNullableAsync(stream, pi.Getter!(obj)!, cancellationToken).DynamicContext();
                }
                else
                {
                    await WriteAnyAsync(stream, pi.Getter!(obj)!, cancellationToken).DynamicContext();
                }
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
            => FluentAsync(stream, (s) => WriteAnyObjectAsync(s, obj, cancellationToken));

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
            => FluentAsync(stream, (s) => WriteAnyObjectNullableAsync(s, obj, cancellationToken));
    }
}
