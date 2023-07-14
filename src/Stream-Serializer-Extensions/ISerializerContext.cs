using System.Buffers;
using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    /// <summary>
    /// Interface for a serializer context
    /// </summary>
    public interface ISerializerContext : IDisposableObject
    {
        /// <summary>
        /// Stream
        /// </summary>
        Stream Stream { get; }
        /// <summary>
        /// Stream type
        /// </summary>
        Type StreamType { get; }
        /// <summary>
        /// Serializer version
        /// </summary>
        int Version { get; }
        /// <summary>
        /// Normalized serializer version (excluding customizable bits)
        /// </summary>
        int SerializerVersion { get; }
        /// <summary>
        /// Normalized custom version (right-shifted)
        /// </summary>
        int CustomVersion { get; }
        /// <summary>
        /// Current recursion level
        /// </summary>
        int RecursionLevel { get; set; }
        /// <summary>
        /// Cancellation
        /// </summary>
        CancellationToken Cancellation { get; set; }
        /// <summary>
        /// Cache size (zero to (temporary?) disable caching)
        /// </summary>
        int CacheSize { get; set; }
        /// <summary>
        /// Real cache size
        /// </summary>
        int RealCacheSize { get; }
        /// <summary>
        /// Cache index size in bytes
        /// </summary>
        int CacheIndexSize { get; }
        /// <summary>
        /// Determine if the cache is enabled
        /// </summary>
        bool IsCacheEnabled { get; }
        /// <summary>
        /// Current cache offset
        /// </summary>
        int CacheOffset { get; }
        /// <summary>
        /// Last object type
        /// </summary>
        ObjectTypes? LastObjectType { get; }
        /// <summary>
        /// Last number type
        /// </summary>
        NumberTypes? LastNumberType { get; }
        /// <summary>
        /// Buffer pool
        /// </summary>
        ArrayPool<byte> BufferPool { get; set; }
        /// <summary>
        /// Nullable?
        /// </summary>
        bool Nullable { get; set; }
        /// <summary>
        /// Enable the cache
        /// </summary>
        /// <returns>If the cache was disabled before, and is enabled now</returns>
        bool EnableCache();
        /// <summary>
        /// Disable the cache
        /// </summary>
        /// <returns>Was the cache enabled and is disabled now?</returns>
        bool DisableCache();
        /// <summary>
        /// Dispose including the stream
        /// </summary>
        void DisposeStream();
        /// <summary>
        /// Dispose including the stream
        /// </summary>
        ValueTask DisposeStreamAsync();
    }
}
