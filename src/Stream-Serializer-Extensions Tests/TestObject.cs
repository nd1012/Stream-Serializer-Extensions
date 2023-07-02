namespace Stream_Serializer_Extensions_Tests
{
    // Testing automatic serialization
    internal class TestObject : ITestObject
    {
        public bool Value { get; set; }

        public virtual bool CompareWith(ITestObject other) => other is TestObject obj && Value == obj.Value;
    }
}
