using wan24.Core;
using wan24.StreamSerializerExtensions;

namespace Stream_Serializer_Extensions_Tests
{
    // Testing StreamSerializerBase
    internal class TestObject2 : StreamSerializerBase, ITestObject
    {
        public TestObject2() : base(1) { }

        public TestObject2(IDeserializationContext context) : base(context, 1) { }

        public bool Value { get; set; }

        public virtual bool CompareWith(ITestObject other) => other is TestObject2 obj && Value == obj.Value;

        protected override void Serialize(ISerializationContext context) => context.Stream.Write(Value, context);

        protected override async Task SerializeAsync(ISerializationContext context)
            => await context.Stream.WriteAsync(Value, context).DynamicContext();

        protected override void Deserialize(IDeserializationContext context)
            => Value = context.Stream.ReadBool(context);

        protected override async Task DeserializeAsync(IDeserializationContext context)
            => Value = await context.Stream.ReadBoolAsync(context).DynamicContext();
    }
}
