namespace wan24.StreamSerializerExtensions.Enumerator
{
    /// <summary>
    /// Stream object enumerator
    /// </summary>
    /// <typeparam name="T">Object type</typeparam>
    public class StreamSerializerEnumerator<T> : StreamEnumeratorBase<T> where T : class, IStreamSerializer
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        public StreamSerializerEnumerator(Stream stream, int? version = null) : base(stream, version) { }

        /// <summary>
        /// Read object method
        /// </summary>
        /// <returns>Object</returns>
        protected override T ReadObject() => Stream.ReadSerialized<T>(SerializerVersion);

        /// <inheritdoc/>
        protected override void Dispose(bool disposing) { }
    }
}
