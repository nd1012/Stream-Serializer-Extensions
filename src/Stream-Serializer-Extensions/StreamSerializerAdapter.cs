using System.Runtime;
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
        /// <param name="context">Context</param>
        /// <param name="version">Object version</param>
        /// <returns>Serialized object version</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static int ReadSerializedObjectVersion(IDeserializationContext context, int version)
        {
            int res = context.Stream.ReadNumber<int>(context);
            if (res > version)
                throw new SerializerException($"Unsupported object version {res} (max. supported version is {version})", new InvalidDataException());
            return res;
        }

        /// <summary>
        /// Read the serialized object version
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="version">Object version</param>
        /// <returns>Serialized object version</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static async Task<int> ReadSerializedObjectVersionAsync(IDeserializationContext context, int version)
        {
            int res = await context.Stream.ReadNumberAsync<int>(context).DynamicContext();
            if (res > version)
                throw new SerializerException($"Unsupported object version {res} (max. supported version is {version})", new InvalidDataException());
            return res;
        }
    }
}
