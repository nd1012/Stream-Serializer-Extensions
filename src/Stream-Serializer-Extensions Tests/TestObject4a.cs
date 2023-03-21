using wan24.StreamSerializerExtensions;

namespace Stream_Serializer_Extensions_Tests
{
    [StreamSerializer(StreamSerializerModes.OptIn, version: 2)]
    internal class TestObject4a : TestObject4
    {
        public TestObject4a() : base() { }
    }
}
