using System.Runtime;
using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    // Any
    public static partial class StreamExtensions
    {
        /// <summary>
        /// Write any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <returns>Stream</returns>
        public static Stream WriteAny(this Stream stream, object obj)
            => SerializerException.Wrap(() =>
            {
                (Type type, ObjectTypes objType, bool writeType, bool writeObject) = obj.GetObjectSerializerInfo();
                using (RentedArray<byte> poolData = new(1))
                {
                    poolData[0] = (byte)objType;
                    stream.Write(poolData.Span);
                }
                if (writeType) WriteString(stream, type.ToString());
                if (writeObject)
                    if (objType.IsNumber())
                    {
                        WriteNumber(stream, obj);
                    }
                    else
                    {
                        WriteObject(stream, obj);
                    }
                return stream;
            });

        /// <summary>
        /// Write any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static Task WriteAnyAsync(this Stream stream, object obj, CancellationToken cancellationToken = default)
            => SerializerException.WrapAsync(async () =>
            {
                (Type type, ObjectTypes objType, bool writeType, bool writeObject) = obj.GetObjectSerializerInfo();
                using (RentedArray<byte> poolData = new(1))
                {
                    poolData[0] = (byte)objType;
                    await stream.WriteAsync(poolData.Memory, cancellationToken).DynamicContext();
                }
                if (writeType) await WriteStringAsync(stream, type.ToString(), cancellationToken).DynamicContext();
                if (writeObject)
                    if (objType.IsNumber())
                    {
                        await WriteNumberAsync(stream, obj, cancellationToken).DynamicContext();
                    }
                    else
                    {
                        await WriteObjectAsync(stream, obj, cancellationToken).DynamicContext();
                    }
            });

        /// <summary>
        /// Write any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static Stream WriteAnyNullable(this Stream stream, object? obj)
            => SerializerException.Wrap(() =>
            {
                if (obj == null)
                {
                    using RentedArray<byte> poolData = new(1, clean: false);
                    poolData[0] = (byte)ObjectTypes.Null;
                    stream.Write(poolData.Span);
                }
                else
                {
                    WriteAny(stream, obj);
                }
                return stream;
            });

        /// <summary>
        /// Write any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="cancellationToken">Cancellation token</param>
        [TargetedPatchingOptOut("Tiny method")]
        public static Task WriteAnyNullableAsync(this Stream stream, object? obj, CancellationToken cancellationToken = default)
            => SerializerException.WrapAsync(async () =>
            {
                if (obj == null)
                {
                    using RentedArray<byte> poolData = new(1, clean: false);
                    poolData[0] = (byte)ObjectTypes.Null;
                    await stream.WriteAsync(poolData.Memory, cancellationToken).DynamicContext();
                }
                else
                {
                    await WriteAnyAsync(stream, obj, cancellationToken).DynamicContext();
                }
            });
    }
}
