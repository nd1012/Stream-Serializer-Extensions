using wan24.StreamSerializerExtensions;

namespace Stream_Serializer_Extensions_Tests
{
    // Testing customized opt-out serialization (object version 2, Field2 should be included)
    [StreamSerializer(version: 2)]
    internal class TestObject3a : TestObject3
    {
        public TestObject3a() : base() { }
    }
}
