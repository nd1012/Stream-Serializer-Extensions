using System.Runtime;
using System.Runtime.CompilerServices;

namespace wan24.StreamSerializerExtensions
{
    // Serialization context
    public static partial class StreamExtensions
    {
        /// <summary>
        /// Create a serialization context for writing to the stream
        /// </summary>
        /// <typeparam name="T">Stream type</typeparam>
        /// <param name="stream">Stream (won't be disposed)</param>
        /// <param name="cacheSize">Cache size</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Context (don't forget to dispose!)</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SerializerContext<T> CreateSerializationContext<T>(this T stream, int? cacheSize = null, CancellationToken cancellationToken = default)
            where T : Stream
            => new(stream, cacheSize, cancellationToken);

        /// <summary>
        /// Create a deserialization context for reading from the stream
        /// </summary>
        /// <typeparam name="T">Stream type</typeparam>
        /// <param name="stream">Stream (won't be disposed)</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cacheSize">Cache size</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Context (don't forget to dispose!)</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DeserializerContext<T> CreateDeserializationContext<T>(
            this T stream,
            int? version = null,
            int? cacheSize = null,
            CancellationToken cancellationToken = default
            )
            where T : Stream
            => new(stream, version, cacheSize, cancellationToken);
    }
}
