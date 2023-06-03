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
        public static tStream WriteEnum<tStream, tEnum>(this tStream stream, tEnum value)
            where tStream : Stream
            where tEnum : struct, Enum
        {
            try
            {
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
        public static async Task WriteEnumAsync<T>(this Stream stream, T value, CancellationToken cancellationToken = default)
            where T : struct, Enum
        {
            try
            {
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
        public static tStream WriteEnumNullable<tStream, tEnum>(this tStream stream, tEnum? value)
            where tStream : Stream
            where tEnum : struct, Enum
        {
            Write(stream, value != null);
            if (value != null) WriteEnum(stream, value.Value);
            return stream;
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Enumeration type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static async Task WriteEnumNullableAsync<T>(this Stream stream, T? value, CancellationToken cancellationToken = default)
            where T : struct, Enum
        {
            await WriteAsync(stream, value != null, cancellationToken).DynamicContext();
            if (value != null) await WriteEnumAsync(stream, value.Value, cancellationToken).DynamicContext();
        }
    }
}
