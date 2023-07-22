using wan24.StreamSerializerExtensions;

namespace Stream_Serializer_Extensions_Tests
{
    public partial class StreamExtensions_Tests
    {
        [TestMethod]
        public void String_Tests()
        {
            using MemoryStream ms = new();
            using SerializerContext sc = new(ms);
            using DeserializerContext dc = new(ms);

            string str = "abcdef";
            ms.WriteString(str, sc);
            Assert.AreEqual((long)str.Length + 2, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(str, ms.ReadString(dc));
            Assert.AreEqual(ms.Length, ms.Position);

            ms.SetLength(0);
            ms.Position = 0;
            str = "abcdefäüöß";
            ms.WriteString(str, sc);
            ms.Position = 0;
            Assert.AreEqual(str, ms.ReadString(dc));
            Assert.AreEqual(ms.Length, ms.Position);

            ms.SetLength(0);
            ms.Position = 0;
            str = "abcdefäüöß";
            ms.WriteString16(str, sc);
            Assert.AreEqual(((long)str.Length << 1) + 2, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(str, ms.ReadString16(dc));
            Assert.AreEqual(ms.Length, ms.Position);

            ms.SetLength(0);
            ms.Position = 0;
            str = "abcdefäüöß\U0001F642";
            ms.WriteString32(str, sc);
            ms.Position = 0;
            Assert.AreEqual(str, ms.ReadString32(dc));
            Assert.AreEqual(ms.Length, ms.Position);

            ms.SetLength(0);
            ms.Position = 0;
            str = "abcdef";
            ms.WriteStringNullable(str, sc);
            Assert.AreEqual((long)str.Length + 2, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(str, ms.ReadStringNullable(dc));
            Assert.AreEqual(ms.Length, ms.Position);

            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteStringNullable(null, sc);
            Assert.AreEqual(1, ms.Length);
            ms.Position = 0;
            Assert.IsNull(ms.ReadStringNullable(dc));
            Assert.AreEqual(ms.Length, ms.Position);
        }

        [TestMethod]
        public async Task StringAsync_Tests()
        {
            using MemoryStream ms = new();
            using SerializerContext sc = new(ms);
            using DeserializerContext dc = new(ms);

            string str = "abcdef";
            await ms.WriteStringAsync(str, sc);
            Assert.AreEqual((long)str.Length + 2, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(str, await ms.ReadStringAsync(dc));

            ms.SetLength(0);
            ms.Position = 0;
            str = "abcdefäüöß";
            await ms.WriteStringAsync(str, sc);
            ms.Position = 0;
            Assert.AreEqual(str, await ms.ReadStringAsync(dc));

            ms.SetLength(0);
            ms.Position = 0;
            str = "abcdefäüöß";
            await ms.WriteString16Async(str, sc);
            Assert.AreEqual(((long)str.Length << 1) + 2, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(str, await ms.ReadString16Async(dc));

            ms.SetLength(0);
            ms.Position = 0;
            str = "abcdefäüöß\U0001F642";
            await ms.WriteString32Async(str, sc);
            ms.Position = 0;
            Assert.AreEqual(str, await ms.ReadString32Async(dc));

            ms.SetLength(0);
            ms.Position = 0;
            str = "abcdef";
            await ms.WriteStringNullableAsync(str, sc);
            Assert.AreEqual((long)str.Length + 2, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(str, await ms.ReadStringNullableAsync(dc));
            Assert.AreEqual(ms.Length, ms.Position);

            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteStringNullableAsync(null, sc);
            Assert.AreEqual(1, ms.Length);
            ms.Position = 0;
            Assert.IsNull(await ms.ReadStringNullableAsync(dc));
            Assert.AreEqual(ms.Length, ms.Position);
        }
    }
}
