using wan24.StreamSerializerExtensions;

namespace Stream_Serializer_Extensions_Tests
{
    public partial class StreamExtensions_Tests
    {
        [TestMethod]
        public void String_Tests()
        {
            using MemoryStream ms = new();

            string str = "abcdef";
            ms.WriteString(str);
            Assert.AreEqual((long)str.Length + 2, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(str, ms.ReadString());
            Assert.AreEqual(ms.Length, ms.Position);

            ms.SetLength(0);
            ms.Position = 0;
            str = "abcdefäüöß";
            ms.WriteString(str);
            ms.Position = 0;
            Assert.AreEqual(str, ms.ReadString());
            Assert.AreEqual(ms.Length, ms.Position);

            ms.SetLength(0);
            ms.Position = 0;
            str = "abcdefäüöß";
            ms.WriteString16(str);
            Assert.AreEqual(((long)str.Length << 1) + 2, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(str, ms.ReadString16());
            Assert.AreEqual(ms.Length, ms.Position);

            ms.SetLength(0);
            ms.Position = 0;
            str = "abcdefäüößﻼ";//TODO Correct sample?
            ms.WriteString32(str);
            Assert.AreEqual(((long)str.Length << 2) + 2, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(str, ms.ReadString32());
            Assert.AreEqual(ms.Length, ms.Position);

            ms.SetLength(0);
            ms.Position = 0;
            str = "abcdef";
            ms.WriteStringNullable(str);
            Assert.AreEqual((long)str.Length + 2, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(str, ms.ReadStringNullable());
            Assert.AreEqual(ms.Length, ms.Position);

            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteStringNullable(null);
            Assert.AreEqual(1, ms.Length);
            ms.Position = 0;
            Assert.IsNull(ms.ReadStringNullable());
            Assert.AreEqual(ms.Length, ms.Position);
        }

        [TestMethod]
        public async Task StringAsync_Tests()
        {
            using MemoryStream ms = new();

            string str = "abcdef";
            await ms.WriteStringAsync(str);
            Assert.AreEqual((long)str.Length + 2, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(str, await ms.ReadStringAsync());

            ms.SetLength(0);
            ms.Position = 0;
            str = "abcdefäüöß";
            await ms.WriteStringAsync(str);
            ms.Position = 0;
            Assert.AreEqual(str, await ms.ReadStringAsync());

            ms.SetLength(0);
            ms.Position = 0;
            str = "abcdefäüöß";
            await ms.WriteString16Async(str);
            Assert.AreEqual(((long)str.Length << 1) + 2, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(str, await ms.ReadString16Async());

            ms.SetLength(0);
            ms.Position = 0;
            str = "abcdefäüößﻼ";//TODO Correct sample?
            await ms.WriteString32Async(str);
            Assert.AreEqual(((long)str.Length << 2) + 2, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(str, await ms.ReadString32Async());

            ms.SetLength(0);
            ms.Position = 0;
            str = "abcdef";
            await ms.WriteStringNullableAsync(str);
            Assert.AreEqual((long)str.Length + 2, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(str, await ms.ReadStringNullableAsync());
            Assert.AreEqual(ms.Length, ms.Position);

            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteStringNullableAsync(null);
            Assert.AreEqual(1, ms.Length);
            ms.Position = 0;
            Assert.IsNull(await ms.ReadStringNullableAsync());
            Assert.AreEqual(ms.Length, ms.Position);
        }
    }
}
