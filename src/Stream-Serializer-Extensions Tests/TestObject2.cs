using wan24.StreamSerializerExtensions;

namespace Stream_Serializer_Extensions_Tests
{
    internal class TestObject2 : StreamSerializerBase
    {
        public TestObject2() : base(1) { }

        public TestObject2(Stream stream, int version) : base(stream, version, 1) { }

        public bool Value { get; set; }

        protected override void Serialize(Stream stream) => stream.Write(Value);

        protected override async Task SerializeAsync(Stream stream, CancellationToken cancellationToken)
            => await stream.WriteAsync(Value, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);

        protected override void Deserialize(Stream stream, int version)
        {
            if (((IStreamSerializer)this).SerializedObjectVersion != 1) throw new SerializerException("Invalid serialized object version");
            Value = stream.ReadBool(version);
        }

        protected override async Task DeserializeAsync(Stream stream, int version, CancellationToken cancellationToken)
        {
            if (((IStreamSerializer)this).SerializedObjectVersion != 1) throw new SerializerException("Invalid serialized object version");
            Value = await stream.ReadBoolAsync(version, cancellationToken: cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
        }
    }
}
