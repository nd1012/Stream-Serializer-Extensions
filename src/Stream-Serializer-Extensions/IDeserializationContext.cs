namespace wan24.StreamSerializerExtensions
{
    /// <summary>
    /// Interface for a stream deserialization context
    /// </summary>
    public interface IDeserializationContext : ISerializerContext
    {
        /// <summary>
        /// Options
        /// </summary>
        ISerializerOptions? Options { get; set; }
        /// <summary>
        /// Add an object to the cache
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="obj">Object</param>
        /// <returns>Object</returns>
        T AddToCache<T>(T obj);
        /// <summary>
        /// Try to read an object cached
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="obj">Object</param>
        /// <returns>Succeed?</returns>
        bool TryReadCached<T>(out T? obj);
        /// <summary>
        /// Try to read an object cached
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <returns>If succeed and the object</returns>
        Task<(bool Succeed, T? Object)> TryReadCachedAsync<T>();
        /// <summary>
        /// Try to read an object cached
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="obj">Object</param>
        /// <param name="readType">Read the type flags?</param>
        /// <returns>Succeed?</returns>
        bool TryReadCachedObject<T>(out T? obj, bool readType = false);
        /// <summary>
        /// Try to read an object cached
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="readType">Read the type flags?</param>
        /// <returns>If succeed and the object</returns>
        Task<(bool Succeed, T? Object)> TryReadCachedObjectAsync<T>(bool readType = false);
        /// <summary>
        /// Try to read a countable object cached
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="obj">Object</param>
        /// <param name="len">Length</param>
        /// <param name="readType">Read the type flags?</param>
        /// <returns>Succeed?</returns>
        bool TryReadCachedObjectCountable<T>(out T? obj, out long len, bool readType = false);
        /// <summary>
        /// Try to read a countable object cached
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="readType">Read the type flags?</param>
        /// <returns>If succeed, the object (if cached) and the length (if not cached)</returns>
        Task<(bool Succeed, T? Object, long Length)> TryReadCachedObjectCountableAsync<T>(bool readType = false);
        /// <summary>
        /// Try to read a number cached
        /// </summary>
        /// <typeparam name="T">Number type</typeparam>
        /// <param name="obj">Number</param>
        /// <param name="readType">Read the type flags?</param>
        /// <returns>Succeed?</returns>
        bool TryReadCachedNumber<T>(out T? obj, bool readType = false) where T : struct, IConvertible;
        /// <summary>
        /// Try to read a number cached
        /// </summary>
        /// <typeparam name="T">Number type</typeparam>
        /// <param name="readType">Read the type flags?</param>
        /// <returns>If succeed and the number</returns>
        Task<(bool Succeed, T? Object)> TryReadCachedNumberAsync<T>(bool readType = false) where T : struct, IConvertible;
        /// <summary>
        /// Create a temporary instance which uses another serializer version
        /// </summary>
        /// <param name="version">New serializer version</param>
        /// <returns>Temporary instance (don't forget to dispose!)</returns>
        IDeserializationContext WithSerializerVersion(int version);
    }
}
