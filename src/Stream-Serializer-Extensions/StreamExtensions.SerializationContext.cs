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
        /// <param name="stream">Stream (won't be disposed)</param>
        /// <param name="cacheSize">Cache size</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Context (don't forget to dispose!)</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ISerializationContext CreateSerializationContext(this Stream stream, int? cacheSize = null, CancellationToken cancellationToken = default)
            => new SerializerContext(stream, cacheSize, cancellationToken);

        /// <summary>
        /// Create a deserialization context for reading from the stream
        /// </summary>
        /// <param name="stream">Stream (won't be disposed)</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cacheSize">Cache size</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Context (don't forget to dispose!)</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IDeserializationContext CreateDeserializationContext(
            this Stream stream, 
            int? version = null, 
            int? cacheSize = null, 
            CancellationToken cancellationToken = default
            )
            => new DeserializerContext(stream, version, cacheSize, cancellationToken);
    }
}
