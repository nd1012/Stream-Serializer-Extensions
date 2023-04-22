using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    /// <summary>
    /// Stream serializer adapter methods for simple implementation of <see cref="IStreamSerializerVersion"/>
    /// </summary>
    public static class StreamSerializerAdapter
    {
        /// <summary>
        /// Read the serialized object version
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="serializerVersion">Serializer version</param>
        /// <param name="objectVersion">Object version</param>
        /// <returns>Serialized object version</returns>
        public static int ReadSerializedObjectVersion(Stream stream, int serializerVersion, int objectVersion)
        {
            int res = stream.ReadNumber<int>(serializerVersion);
            if (res > objectVersion) throw new SerializerException($"Unsupported object version {res} (max. supported version is {objectVersion})");
            return res;
        }

        /// <summary>
        /// Read the serialized object version
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="serializerVersion">Serializer version</param>
        /// <param name="objectVersion">Object version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Serialized object version</returns>
        public static async Task<int> ReadSerializedObjectVersionAsync(Stream stream, int serializerVersion, int objectVersion, CancellationToken cancellationToken = default)
        {
            int res = await stream.ReadNumberAsync<int>(serializerVersion, cancellationToken: cancellationToken).DynamicContext();
            if (res > objectVersion) throw new SerializerException($"Unsupported object version {res} (max. supported version is {objectVersion})");
            return res;
        }
    }
}
