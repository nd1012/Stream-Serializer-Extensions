using wan24.Core;

[assembly: Bootstrapper(typeof(wan24.StreamSerializerExtensions.Bootstrap), nameof(wan24.StreamSerializerExtensions.Bootstrap.Boot))]

namespace wan24.StreamSerializerExtensions
{
    /// <summary>
    /// Bootstrapping
    /// </summary>
    public static class Bootstrap
    {
        /// <summary>
        /// Boot
        /// </summary>
        public static void Boot()
        {
            ObjectSerializer.NamedSerializers[StreamSerializer.BINARY_SERIALIZER_NAME] = (name, obj, stream) => stream.WriteAnyNullable(obj);
            ObjectSerializer.NamedAsyncSerializers[StreamSerializer.BINARY_SERIALIZER_NAME] = async (name, obj, stream, ct) => await stream.WriteAnyNullableAsync(obj, ct).DynamicContext();
            ObjectSerializer.NamedDeserializers[StreamSerializer.BINARY_SERIALIZER_NAME] = (name, type, stream) => 
            {
                object? res = stream.ReadAnyNullable();
                if (res is not null && !type.IsAssignableFrom(res.GetType())) throw new SerializerException($"Red {res.GetType()} instead of {type}", new InvalidDataException());
                return res;
            };
            ObjectSerializer.NamedAsyncDeserializers[StreamSerializer.BINARY_SERIALIZER_NAME] = async (name, type, stream, ct) =>
            {
                object? res = await stream.ReadAnyNullableAsync(cancellationToken: ct).DynamicContext();
                if (res is not null && !type.IsAssignableFrom(res.GetType())) throw new SerializerException($"Red {res.GetType()} instead of {type}", new InvalidDataException());
                return res;
            };
        }
    }
}
