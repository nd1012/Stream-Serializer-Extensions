using System.Runtime;
using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    // Enumeration
    public static partial class StreamExtensions
    {
        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="tStream">Stream type</typeparam>
        /// <typeparam name="tEnum">Enumeration type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static tStream WriteEnum<tStream, tEnum>(this tStream stream, tEnum value)
            where tStream : Stream
            where tEnum : struct, Enum
        {
            try
            {
                if (ObjectHelper.AreEqual(value, default(tEnum))) return Write(stream, (byte)NumberTypes.Default);
                Type type = typeof(tEnum).GetEnumUnderlyingType();
                WriteNumberMethod.MakeGenericMethod(typeof(tStream), type).InvokeAuto(obj: null, stream, Convert.ChangeType(value, type));
                return stream;
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Enumeration type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        [TargetedPatchingOptOut("Tiny method")]
        public static async Task WriteEnumAsync<T>(this Stream stream, T value, CancellationToken cancellationToken = default)
            where T : struct, Enum
        {
            try
            {
                if (ObjectHelper.AreEqual(value, default(T)))
                {
                    await WriteAsync(stream, (byte)NumberTypes.Default, cancellationToken).DynamicContext();
                    return;
                }
                Type type = typeof(T).GetEnumUnderlyingType();
                Task task = (Task)WriteNumberAsyncMethod.MakeGenericMethod(type).InvokeAuto(obj: null, stream, Convert.ChangeType(value, type), cancellationToken)!;
                await task.DynamicContext();
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="tStream">Stream type</typeparam>
        /// <typeparam name="tEnum">Enumeration type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static tStream WriteEnumNullable<tStream, tEnum>(this tStream stream, tEnum? value)
            where tStream : Stream
            where tEnum : struct, Enum
            => value == null ? Write(stream, (byte)NumberTypes.Null) : WriteEnum(stream, value.Value);

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Enumeration type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        [TargetedPatchingOptOut("Tiny method")]
        public static async Task WriteEnumNullableAsync<T>(this Stream stream, T? value, CancellationToken cancellationToken = default)
            where T : struct, Enum
        {
            if (value == null)
            {
                await WriteAsync(stream, (byte)NumberTypes.Null, cancellationToken).DynamicContext();
            }
            else
            {
                await WriteEnumAsync(stream, value.Value, cancellationToken).DynamicContext();
            }
        }
    }
}
