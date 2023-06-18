using System.Reflection;
using System.Runtime;
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
        public static Stream WriteObject(this Stream stream, object obj)
            => SerializerException.Wrap(() =>
            {
                ArgumentValidationHelper.EnsureValidArgument(nameof(obj), obj);
                if (obj is IStreamSerializer streamSerializer)
                    return WriteSerialized(stream, streamSerializer);
                else if (StreamSerializer.FindSerializer(obj.GetType()) is not StreamSerializer.Serialize_Delegate serializer)
                    return WriteAnyObject(stream, obj);
                else
                    SerializerException.Wrap(() => serializer(stream, obj));
                return stream;
            });

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        [TargetedPatchingOptOut("Tiny method")]
        public static Task WriteObjectAsync(this Stream stream, object obj, CancellationToken cancellationToken = default)
            => SerializerException.WrapAsync(async () =>
            {
                ArgumentValidationHelper.EnsureValidArgument(nameof(obj), obj);
                if (obj is IStreamSerializer streamSerializer)
                    await WriteSerializedAsync(stream, streamSerializer, cancellationToken).DynamicContext();
                else if (StreamSerializer.FindAsyncSerializer(obj.GetType()) is not StreamSerializer.AsyncSerialize_Delegate serializer)
                    await WriteAnyObjectAsync(stream, obj, cancellationToken).DynamicContext();
                else
                    await SerializerException.WrapAsync(async () => await serializer(stream, obj, cancellationToken).DynamicContext()).DynamicContext();
            });

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object to write</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static Stream WriteObjectNullable(this Stream stream, object? obj)
            => WriteIfNull(stream, obj, () => WriteObject(stream, obj!));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        [TargetedPatchingOptOut("Tiny method")]
        public static Task WriteObjectNullableAsync(this Stream stream, object? obj, CancellationToken cancellationToken = default)
            => WriteIfNullAsync(stream, obj, () => WriteObjectAsync(stream, obj!, cancellationToken), cancellationToken);

        /// <summary>
        /// Write any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <returns>Stream</returns>
        public static Stream WriteAnyObject(this Stream stream, object obj)
            => SerializerException.Wrap(() =>
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
            });

        /// <summary>
        /// Write any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static Task WriteAnyObjectAsync(this Stream stream, object obj, CancellationToken cancellationToken = default)
            => SerializerException.WrapAsync(async () =>
            {
                if (obj is IStreamSerializer serializable)
                {
                    await WriteSerializedAsync(stream, serializable, cancellationToken).DynamicContext();
                    return;
                }
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
            });

        /// <summary>
        /// Write any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static Stream WriteAnyObjectNullable(this Stream stream, object? obj)
            => WriteIfNull(stream, obj, () => WriteAnyObject(stream, obj!));

        /// <summary>
        /// Write any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="cancellationToken">Cancellation token</param>
        [TargetedPatchingOptOut("Tiny method")]
        public static Task WriteAnyObjectNullableAsync(this Stream stream, object? obj, CancellationToken cancellationToken = default)
            => WriteIfNullAsync(stream, obj, () => WriteObjectAsync(stream, obj!, cancellationToken), cancellationToken);
    }
}
