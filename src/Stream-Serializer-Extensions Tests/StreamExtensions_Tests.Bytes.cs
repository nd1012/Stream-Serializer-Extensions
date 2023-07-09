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
            ms.WriteBytes(data);
            Assert.AreEqual(22L, ms.Length);
            ms.Position = 0;
            (byte[] temp, int len) = ms.ReadBytes();
            Assert.AreEqual((int)data.Length, len);
            Assert.IsTrue(data.SequenceEqual(temp));
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteBytesNullable(data);
            Assert.AreEqual(22L, ms.Length);
            ms.Position = 0;
            var info = ms.ReadBytesNullable();
            Assert.IsNotNull(info);
            Assert.AreEqual((int)data.Length, info.Value.Length);
            Assert.IsTrue(data.SequenceEqual(info.Value.Value));
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteBytesNullable(null);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.IsNull(ms.ReadBytesNullable());
        }

        [TestMethod]
        public async Task BytesAsync_Tests()
        {
            byte[] data = new byte[20];
            Random.Shared.NextBytes(data);
            using MemoryStream ms = new();
            await ms.WriteBytesAsync(data);
            Assert.AreEqual(22L, ms.Length);
            ms.Position = 0;
            (byte[] temp, int len) = await ms.ReadBytesAsync();
            Assert.AreEqual((int)data.Length, len);
            Assert.IsTrue(data.SequenceEqual(temp));
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteBytesNullableAsync(data);
            Assert.AreEqual(22L, ms.Length);
            ms.Position = 0;
            var info = await ms.ReadBytesNullableAsync();
            Assert.IsNotNull(info);
            Assert.AreEqual((int)data.Length, info.Value.Length);
            Assert.IsTrue(data.SequenceEqual(info.Value.Value));
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteBytesNullableAsync(null);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.IsNull(await ms.ReadBytesNullableAsync());
        }
    }
}
