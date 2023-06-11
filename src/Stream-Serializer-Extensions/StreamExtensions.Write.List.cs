using System.Runtime;
using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    // List
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
        [TargetedPatchingOptOut("Tiny method")]
        public static tStream WriteList<tStream, tElement>(this tStream stream, List<tElement> value) where tStream : Stream
        {
            if (typeof(tElement) == typeof(byte)) return WriteBytes(stream, (value as byte[])!);
            try
            {
                WriteNumber(stream, value.Count);
                if (value.Count == 0) return stream;
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
        [TargetedPatchingOptOut("Tiny method")]
        public static async Task WriteListAsync<T>(this Stream stream, List<T> value, CancellationToken cancellationToken = default)
        {
            if (typeof(T) == typeof(byte))
            {
                await WriteBytesAsync(stream, (value as byte[])!, cancellationToken).DynamicContext();
                return;
            }
            try
            {
                await WriteNumberAsync(stream, value.Count, cancellationToken).DynamicContext();
                if (value.Count == 0) return;
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
        [TargetedPatchingOptOut("Tiny method")]
        public static tStream WriteListNullable<tStream, tElement>(this tStream stream, List<tElement>? value) where tStream : Stream
        {
            Write(stream, value != null);
            if (value != null) WriteList(stream, value);
            return stream;
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        [TargetedPatchingOptOut("Tiny method")]
        public static async Task WriteListNullableAsync<T>(this Stream stream, List<T>? value, CancellationToken cancellationToken = default)
        {
            await WriteAsync(stream, value != null, cancellationToken).DynamicContext();
            if (value != null) await WriteListAsync(stream, value, cancellationToken).DynamicContext();
        }
    }
}
