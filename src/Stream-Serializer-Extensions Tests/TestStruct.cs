namespace Stream_Serializer_Extensions_Tests
{
    public readonly record struct TestStruct
    {
        public readonly bool Value;

        public TestStruct() => Value = false;

        public TestStruct(bool value) => Value = value;
    }
}
