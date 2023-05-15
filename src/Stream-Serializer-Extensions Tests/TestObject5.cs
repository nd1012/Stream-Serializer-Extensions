using wan24.ObjectValidation;
using wan24.StreamSerializerExtensions;

namespace Stream_Serializer_Extensions_Tests
{
    // Test automatic serializer configuration (object version 1, ZValue is excluded)
    [StreamSerializer(StreamSerializerModes.OptIn, version: 1)]
    internal sealed class TestObject5 : TestObject5<TestObject5>
    {
        public TestObject5() : base() { }
    }

    internal abstract class TestObject5<T> : DisposableAutoStreamSerializerBase<T> where T : TestObject5<T>
    {
        protected TestObject5() : base() { }

        // Included in object version 2+
        [StreamSerializer(2)]
        public bool ZValue { get; set; }

        // Always excluded
        public bool BValue { get; set; }

        // Excluded in object version 3+ (but included in object version 1 and 2)
        [StreamSerializer(0, 2), NoValidation]//TODO Remove NoValidation for newer ObjectValidation version which denies Stream validation
        public Stream? Stream { get; set; }

        // Always included
        [StreamSerializer(0)]
        public bool AValue { get; set; }

        protected override void Dispose(bool disposing) => Stream?.Dispose();
    }
}
