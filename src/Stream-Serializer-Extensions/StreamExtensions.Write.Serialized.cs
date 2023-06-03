using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    // Serialized
    public static partial class StreamExtensions
    {
        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Stream type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <returns>Stream</returns>
        public static T WriteSerialized<T>(this T stream, IStreamSerializer obj) where T : Stream
        {
            obj.Serialize(stream);
            return stream;
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static async Task WriteSerializedAsync(this Stream stream, IStreamSerializer obj, CancellationToken cancellationToken = default)
            => await obj.SerializeAsync(stream, cancellationToken).DynamicContext();

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Stream type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <returns>Stream</returns>
        public static T WriteSerializedNullable<T>(this T stream, IStreamSerializer? obj) where T : Stream
        {
            Write(stream, obj != null);
            obj?.Serialize(stream);
            return stream;
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static async Task WriteSerializedNullableAsync(this Stream stream, IStreamSerializer? obj, CancellationToken cancellationToken = default)
        {
            await WriteAsync(stream, obj != null, cancellationToken).DynamicContext();
            if (obj != null) await obj.SerializeAsync(stream, cancellationToken).DynamicContext();
        }
    }
}
