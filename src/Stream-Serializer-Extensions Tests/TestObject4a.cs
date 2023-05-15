using wan24.StreamSerializerExtensions;

namespace Stream_Serializer_Extensions_Tests
{
    // Testing customized opt-in serialization (object version 2, Field2 should be included)
    [StreamSerializer(StreamSerializerModes.OptIn, version: 2)]
    internal class TestObject4a : TestObject4
    {
        public TestObject4a() : base() { }
    }
}
