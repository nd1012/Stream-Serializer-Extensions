using wan24.StreamSerializerExtensions;

namespace Stream_Serializer_Extensions_Tests
{
    // Testing customized opt-out serialization (object version 3, Field3 should be excluded)
    [StreamSerializer(version: 3)]
    internal class TestObject3b : TestObject3a
    {
        public TestObject3b() : base() { }
    }
}
