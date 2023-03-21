using wan24.StreamSerializerExtensions;

namespace Stream_Serializer_Extensions_Tests
{
    [StreamSerializer(StreamSerializerModes.OptIn, version: 1)]
    internal class TestObject4
    {
        public TestObject4() { }

        [StreamSerializer(0)]
        public bool Field1 { get; set; }

        [StreamSerializer(2, mode: StreamSerializerModes.OptIn)]
        public bool Field2 { get; set; }

        [StreamSerializer(0, 2, mode: StreamSerializerModes.OptOut)]
        public bool Field3 { get; set; }
    }
}
