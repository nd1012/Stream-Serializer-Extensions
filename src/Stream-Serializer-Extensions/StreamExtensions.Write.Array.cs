using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    // Array
    public static partial class StreamExtensions
    {
        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="tStream">Stream type</typeparam>
        /// <typeparam name="tElement">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        public static tStream WriteArray<tStream, tElement>(this tStream stream, tElement[] value) where tStream : Stream
        {
            if (typeof(tElement) == typeof(byte)) return WriteBytes(stream, (value as byte[])!);
            try
            {
                WriteNumber(stream, value.Length);
                if (value.Length == 0) return stream;
                foreach (tElement element in value) WriteObject(stream, element);
                return stream;
            }
            catch (SerializerException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static async Task WriteArrayAsync<T>(this Stream stream, T[] value, CancellationToken cancellationToken = default)
        {
            if (typeof(T) == typeof(byte))
            {
                await WriteBytesAsync(stream, (value as byte[])!, cancellationToken).DynamicContext();
                return;
            }
            try
            {
                await WriteNumberAsync(stream, value.Length, cancellationToken).DynamicContext();
                if (value.Length == 0) return;
                foreach (T element in value) await WriteObjectAsync(stream, element, cancellationToken).DynamicContext();
            }
            catch (SerializerException)
            {
                throw;
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
        /// <typeparam name="tElement">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        public static tStream WriteArrayNullable<tStream, tElement>(this tStream stream, tElement[]? value) where tStream : Stream
        {
            Write(stream, value != null);
            if (value != null) WriteArray(stream, value);
            return stream;
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static async Task WriteArrayNullableAsync<T>(this Stream stream, T[]? value, CancellationToken cancellationToken = default)
        {
            await WriteAsync(stream, value != null, cancellationToken).DynamicContext();
            if (value != null) await WriteArrayAsync(stream, value, cancellationToken).DynamicContext();
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="tStream">Stream type</typeparam>
        /// <typeparam name="tElement">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        public static tStream WriteFixedArray<tStream, tElement>(this tStream stream, Span<tElement> value) where tStream : Stream
        {
            try
            {
                foreach (tElement element in value)
                    WriteObject(stream, element);
                return stream;
            }
            catch (SerializerException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static async Task WriteFixedArrayAsync<T>(this Stream stream, Memory<T> value, CancellationToken cancellationToken = default)
        {
            try
            {
                for (int i = 0; i < value.Length; i++) await WriteObjectAsync(stream, value.Span[i], cancellationToken).DynamicContext();
            }
            catch (SerializerException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }
    }
}
