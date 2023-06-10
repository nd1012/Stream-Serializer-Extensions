namespace wan24.StreamSerializerExtensions.Enumerator
{
    /// <summary>
    /// Stream object enumerator
    /// </summary>
    /// <typeparam name="T">Object type</typeparam>
    public class StreamSerializerAsyncEnumerator<T> : StreamAsyncEnumeratorBase<T> where T : class, IStreamSerializer
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public StreamSerializerAsyncEnumerator(Stream stream, int? version = null, CancellationToken cancellationToken = default) : base(stream, version, cancellationToken) { }

        /// <summary>
        /// Read object method
        /// </summary>
        /// <returns>Object</returns>
        protected override Task<T> ReadObjectAsync() => Stream.ReadSerializedAsync<T>(SerializerVersion, Cancellation);
    }
}
