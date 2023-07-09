using wan24.StreamSerializerExtensions;

namespace Stream_Serializer_Extensions_Tests
{
    [TestClass]
    public class StreamSerializer_Tests
    {
        [TestMethod]
        public void CustomVersion_Tests()
        {
            StreamSerializer.Version = StreamSerializer.VERSION | (1 << 8);
            try
            {
                using MemoryStream ms = new();
                ms.WriteSerializerVersion();
                ms.Position = 0;
                int temp = ms.ReadSerializerVersion();
                Assert.AreEqual(StreamSerializer.Version, temp);
                Assert.AreEqual(1, temp >> 8);
            }
            finally
            {
                StreamSerializer.Version = StreamSerializer.VERSION;
            }
        }

        [TestMethod]
        public async Task CustomVersionAsync_Tests()
        {
            StreamSerializer.Version = StreamSerializer.VERSION | (1 << 8);
            try
            {
                using MemoryStream ms = new();
                await ms.WriteSerializerVersionAsync();
                ms.Position = 0;
                int temp = await ms.ReadSerializerVersionAsync();
                Assert.AreEqual(StreamSerializer.Version, temp);
                Assert.AreEqual(1, temp >> 8);
            }
            finally
            {
                StreamSerializer.Version = StreamSerializer.VERSION;
            }
        }

        [TestMethod]
        public void IsTypeAllowed_Tests()
        {
            Assert.IsFalse(StreamSerializer.IsTypeAllowed(typeof(StreamSerializer_Tests)));
            StreamSerializer.AllowedTypes.Add(typeof(StreamSerializer_Tests));
            try
            {
                Assert.IsTrue(StreamSerializer.IsTypeAllowed(typeof(StreamSerializer_Tests)));
            }
            finally
            {
                StreamSerializer.AllowedTypes.TryTake(out _);
            }
            Assert.IsTrue(StreamSerializer.IsTypeAllowed(typeof(TestObject2)));
            Assert.IsTrue(StreamSerializer.IsTypeAllowed(typeof(TestObject2[])));
            Assert.IsFalse(StreamSerializer.IsTypeAllowed(typeof(Array)));
            Assert.IsTrue(StreamSerializer.IsTypeAllowed(typeof(List<bool>)));
            Assert.IsFalse(StreamSerializer.IsTypeAllowed(typeof(List<char>)));
            Assert.IsTrue(StreamSerializer.IsTypeAllowed(typeof(FileStream)));
        }
    }
}
