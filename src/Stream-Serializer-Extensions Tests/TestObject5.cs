using wan24.ObjectValidation;
using wan24.StreamSerializerExtensions;

namespace Stream_Serializer_Extensions_Tests
{
    [StreamSerializer(StreamSerializerModes.OptIn, version: 1)]
    internal sealed class TestObject5 : DisposableAutoStreamSerializerBase<TestObject5>
    {
        public TestObject5() : base() { }

        [StreamSerializer(0)]
        public bool ZValue { get; set; }

        [StreamSerializer(0), NoValidation]//TODO Remove NoValidation for newer ObjectValidation version
        public Stream? Stream { get; set; }

        [StreamSerializer(0)]
        public bool AValue { get; set; }

        protected override void Dispose(bool disposing) => Stream?.Dispose();
    }
}
