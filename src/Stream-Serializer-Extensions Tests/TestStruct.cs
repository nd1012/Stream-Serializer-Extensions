namespace Stream_Serializer_Extensions_Tests
{
    public readonly record struct TestStruct : ITestObject
    {
        public readonly bool Value;

        public TestStruct() => Value = false;

        public TestStruct(bool value) => Value = value;

        public bool CompareWith(ITestObject other) => other is TestStruct obj && Value == obj.Value;
    }
}
