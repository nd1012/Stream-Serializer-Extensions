namespace wan24.StreamSerializerExtensions
{
    /// <summary>
    /// Interface for a stream serializer writing context
    /// </summary>
    public interface ISerializationContext : ISerializerContext
    {
        /// <summary>
        /// Try to write the object cached
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="obj">Object</param>
        /// <returns>Cached?</returns>
        bool TryWriteCached<T>(T? obj);
        /// <summary>
        /// Try to write the object cached
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="obj">Object</param>
        /// <returns>Cached?</returns>
        Task<bool> TryWriteCachedAsync<T>(T? obj);
        /// <summary>
        /// Try to write the object cached
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="obj">Object</param>
        /// <param name="objType">Object type</param>
        /// <param name="writeType">Write the type flags?</param>
        /// <returns>Cached?</returns>
        bool TryWriteCached<T>(T? obj, ObjectTypes? objType, bool writeType = false);
        /// <summary>
        /// Try to write the object cached
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="obj">Object</param>
        /// <param name="objType">Object type</param>
        /// <param name="writeType">Write the type flags?</param>
        /// <returns>Cached?</returns>
        Task<bool> TryWriteCachedAsync<T>(T? obj, ObjectTypes? objType, bool writeType = false);
        /// <summary>
        /// Try to write a countable object cached
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="obj">Object</param>
        /// <param name="len">Length</param>
        /// <param name="writeType">Write the type flags?</param>
        /// <returns>Cached?</returns>
        bool TryWriteCachedCountable<T>(T? obj, long? len, bool writeType = false);
        /// <summary>
        /// Try to write a countable object cached
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="obj">Object</param>
        /// <param name="len">Length</param>
        /// <param name="writeType">Write the type flags?</param>
        /// <returns>Cached?</returns>
        Task<bool> TryWriteCachedCountableAsync<T>(T? obj, long? len, bool writeType = false);
        /// <summary>
        /// Try to write the number cached
        /// </summary>
        /// <typeparam name="T">Number type</typeparam>
        /// <param name="num">Number</param>
        /// <param name="numberType">Number type</param>
        /// <param name="writeType">Write the type flags?</param>
        /// <returns>Cached?</returns>
        bool TryWriteCached<T>(T? num, NumberTypes? numberType, bool writeType = false) where T : struct, IConvertible;
        /// <summary>
        /// Try to write the number cached
        /// </summary>
        /// <typeparam name="T">Number type</typeparam>
        /// <param name="num">Number</param>
        /// <param name="numberType">Number type</param>
        /// <param name="writeType">Write the type flags?</param>
        /// <returns>Cached?</returns>
        Task<bool> TryWriteCachedAsync<T>(T? num, NumberTypes? numberType, bool writeType = false) where T : struct, IConvertible;
    }
}
