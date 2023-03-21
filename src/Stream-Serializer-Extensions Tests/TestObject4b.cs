using wan24.StreamSerializerExtensions;

namespace Stream_Serializer_Extensions_Tests
{
    [StreamSerializer(StreamSerializerModes.OptIn, version: 3)]
    internal class TestObject4b : TestObject4a
    {
        public TestObject4b() : base() { }
    }
}
