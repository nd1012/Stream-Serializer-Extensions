using System.Runtime;
using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    // Dictionary
    public static partial class StreamExtensions
    {
        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="tStream">Stream type</typeparam>
        /// <typeparam name="tKey">Key type</typeparam>
        /// <typeparam name="tValue">Value type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static tStream WriteDict<tStream, tKey, tValue>(this tStream stream, Dictionary<tKey, tValue> value)
            where tStream : Stream
            where tKey : notnull
        {
            try
            {
                WriteNumber(stream, value.Count);
                if (value.Count == 0) return stream;
                foreach (KeyValuePair<tKey, tValue> kvp in value)
                {
                    WriteObject(stream, kvp.Key);
                    WriteObject(stream, kvp.Value);
                }
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
        /// <typeparam name="tKey">Key type</typeparam>
        /// <typeparam name="tValue">Value type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        [TargetedPatchingOptOut("Tiny method")]
        public static async Task WriteDictAsync<tKey, tValue>(this Stream stream, Dictionary<tKey, tValue> value, CancellationToken cancellationToken = default)
            where tKey : notnull
        {
            try
            {
                await WriteNumberAsync(stream, value.Count, cancellationToken).DynamicContext();
                if (value.Count == 0) return;
                foreach (KeyValuePair<tKey, tValue> kvp in value)
                {
                    await WriteObjectAsync(stream, kvp.Key, cancellationToken).DynamicContext();
                    await WriteObjectAsync(stream, kvp.Value, cancellationToken).DynamicContext();
                }
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
        /// <typeparam name="tKey">Key type</typeparam>
        /// <typeparam name="tValue">Value type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static tStream WriteDictNullable<tStream, tKey, tValue>(this tStream stream, Dictionary<tKey, tValue>? value)
            where tStream : Stream
            where tKey : notnull
        {
            Write(stream, value != null);
            if (value != null) WriteDict(stream, value);
            return stream;
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="tKey">Key type</typeparam>
        /// <typeparam name="tValue">Value type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        [TargetedPatchingOptOut("Tiny method")]
        public static async Task WriteDictNullableAsync<tKey, tValue>(this Stream stream, Dictionary<tKey, tValue>? value, CancellationToken cancellationToken = default)
            where tKey : notnull
        {
            await WriteAsync(stream, value != null, cancellationToken).DynamicContext();
            if (value != null) await WriteDictAsync(stream, value, cancellationToken).DynamicContext();
        }
    }
}
