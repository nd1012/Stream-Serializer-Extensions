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
                using SerializerContext sc = new(ms);
                using DeserializerContext dc = new(ms);
                ms.WriteSerializerVersion(sc);
                ms.Position = 0;
                int temp = ms.ReadSerializerVersion(dc);
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
                using SerializerContext sc = new(ms);
                using DeserializerContext dc = new(ms);
                await ms.WriteSerializerVersionAsync(sc);
                ms.Position = 0;
                int temp = await ms.ReadSerializerVersionAsync(dc);
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
