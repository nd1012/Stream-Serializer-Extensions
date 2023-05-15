using wan24.StreamSerializerExtensions;

namespace Stream_Serializer_Extensions_Tests
{
    // Test automatic serializer configuration (object version 2, ZValue is included)
    [StreamSerializer(StreamSerializerModes.OptIn, version: 2)]
    internal sealed class TestObject5a : TestObject5<TestObject5a>
    {
        public TestObject5a() : base() { }
    }
}
