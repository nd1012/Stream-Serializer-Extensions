using wan24.StreamSerializerExtensions;

namespace Stream_Serializer_Extensions_Tests
{
    // Testing customized opt-in serialization (object version 3, Field3 should be excluded)
    [StreamSerializer(StreamSerializerModes.OptIn, version: 3)]
    internal class TestObject4b : TestObject4a
    {
        public TestObject4b() : base() { }
    }
}
