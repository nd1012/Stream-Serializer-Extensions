using wan24.StreamSerializerExtensions;

namespace Stream_Serializer_Extensions_Tests
{
    public partial class StreamExtensions_Tests
    {
        [TestMethod]
        public void Bytes_Tests()
        {
            byte[] data = new byte[20];
            Random.Shared.NextBytes(data);
            using MemoryStream ms = new();
            using SerializerContext sc = new(ms);
            using DeserializerContext dc = new(ms);
            ms.WriteBytes(data, sc);
            Assert.AreEqual(22L, ms.Length);
            ms.Position = 0;
            (byte[] temp, int len) = ms.ReadBytes(dc);
            Assert.AreEqual((int)data.Length, len);
            Assert.IsTrue(data.SequenceEqual(temp));
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteBytesNullable(data, sc);
            Assert.AreEqual(22L, ms.Length);
            ms.Position = 0;
            var info = ms.ReadBytesNullable(dc);
            Assert.IsNotNull(info);
            Assert.AreEqual((int)data.Length, info.Value.Length);
            Assert.IsTrue(data.SequenceEqual(info.Value.Value));
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteBytesNullable(null, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.IsNull(ms.ReadBytesNullable(dc));
        }

        [TestMethod]
        public async Task BytesAsync_Tests()
        {
            byte[] data = new byte[20];
            Random.Shared.NextBytes(data);
            using MemoryStream ms = new();
            using SerializerContext sc = new(ms);
            using DeserializerContext dc = new(ms);
            await ms.WriteBytesAsync(data, sc);
            Assert.AreEqual(22L, ms.Length);
            ms.Position = 0;
            (byte[] temp, int len) = await ms.ReadBytesAsync(dc);
            Assert.AreEqual((int)data.Length, len);
            Assert.IsTrue(data.SequenceEqual(temp));
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteBytesNullableAsync(data, sc);
            Assert.AreEqual(22L, ms.Length);
            ms.Position = 0;
            var info = await ms.ReadBytesNullableAsync(dc);
            Assert.IsNotNull(info);
            Assert.AreEqual((int)data.Length, info.Value.Length);
            Assert.IsTrue(data.SequenceEqual(info.Value.Value));
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteBytesNullableAsync(null, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.IsNull(await ms.ReadBytesNullableAsync(dc));
        }
    }
}
