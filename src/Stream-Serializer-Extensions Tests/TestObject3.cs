using wan24.StreamSerializerExtensions;

namespace Stream_Serializer_Extensions_Tests
{
    // Testing customized opt-out serialization (object version 1, Field2 should be excluded)
    [StreamSerializer(version: 1)]
    internal class TestObject3 : ITestObject
    {
        public TestObject3() { }

        // Always included
        public bool Field1 { get; set; }

        // Included from object version 2+
        [StreamSerializer(2)]
        public bool Field2 { get; set; }

        // Excluded from object version 3+ (included until object version 2)
        [StreamSerializer(0, 2)]
        public bool Field3 { get; set; }

        public virtual bool CompareWith(ITestObject other) => other is TestObject3 obj && Field1 == obj.Field1 && Field2 == obj.Field2 && Field3 == obj.Field3;
    }
}
