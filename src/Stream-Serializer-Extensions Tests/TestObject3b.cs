using wan24.StreamSerializerExtensions;

namespace Stream_Serializer_Extensions_Tests
{
    [StreamSerializer(version: 3)]
    internal class TestObject3b : TestObject3a
    {
        public TestObject3b() : base() { }
    }
}
