using wan24.StreamSerializerExtensions;

namespace Stream_Serializer_Extensions_Tests
{
    // Test automatic serializer configuration (object version 3, Stream is excluded)
    [StreamSerializer(StreamSerializerModes.OptIn, version: 3)]
    internal sealed class TestObject5b : TestObject5<TestObject5b>
    {
        public TestObject5b() : base() { }
    }
}
