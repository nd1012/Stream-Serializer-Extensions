using wan24.StreamSerializerExtensions;

namespace Stream_Serializer_Extensions_Tests
{
    // Testing customized opt-in serialization (object version 1, Field2 should be excluded)
    [StreamSerializer(StreamSerializerModes.OptIn, version: 1)]
    internal class TestObject4
    {
        public TestObject4() { }

        // Always included
        [StreamSerializer(0)]
        public bool Field1 { get; set; }

        // Included from object version 2+
        [StreamSerializer(2)]
        public bool Field2 { get; set; }

        // Included until object version 2 (excluded from object version 3+)
        [StreamSerializer(0, 2)]
        public bool Field3 { get; set; }
    }
}
