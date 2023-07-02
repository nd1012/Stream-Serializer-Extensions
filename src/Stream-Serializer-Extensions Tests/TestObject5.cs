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
        [StreamSerializer(0, 2)]
        public Stream? Stream { get; set; }

        // Always included
        [StreamSerializer(0)]
        public bool AValue { get; set; }

        public virtual bool CompareWith(ITestObject other)
        {
            if(other is TestObject5<T> obj && ZValue == obj.ZValue && BValue == obj.BValue && AValue == obj.AValue)
            {
                if (Stream != null)
                {
                    Assert.IsNotNull(obj.Stream);
                    Assert.AreEqual(Stream.Length, obj.Stream.Length);
                    Stream.Position = 0;
                    byte[] temp1 = new byte[Stream.Length],
                        temp2 = new byte[temp1.Length];
                    Assert.AreEqual(temp1.Length, Stream.Read(temp1));
                    obj.Stream.Position = 0;
                    Assert.AreEqual(temp2.Length, obj.Stream.Read(temp2));
                    Assert.IsTrue(temp1.SequenceEqual(temp2));
                }
                else
                {
                    Assert.IsNull(obj.Stream);
                }
                return true;
            }
            return false;
        }

        protected override void Dispose(bool disposing) => Stream?.Dispose();
    }
}
