using wan24.StreamSerializerExtensions;

namespace Stream_Serializer_Extensions_Tests
{
    [StreamSerializer(version: 2)]
    internal class TestObject3a : TestObject3
    {
        public TestObject3a() : base() { }
    }
}
