using wan24.StreamSerializerExtensions;

namespace Stream_Serializer_Extensions_Tests
{
    [StreamSerializer(version: 1)]
    internal class TestObject3
    {
        public TestObject3() { }

        public bool Field1 { get; set; }

        [StreamSerializer(2, mode: StreamSerializerModes.OptIn)]
        public bool Field2 { get; set; }

        [StreamSerializer(0, 2, mode: StreamSerializerModes.OptOut)]
        public bool Field3 { get; set; }
    }
}
