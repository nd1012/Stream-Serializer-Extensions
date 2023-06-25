using wan24.StreamSerializerExtensions;

namespace Stream_Serializer_Extensions_Tests
{
    public partial class StreamExtensions_Tests
    {
        [TestMethod]
        public void Long_Tests()
        {
            using MemoryStream ms = new();
            ms.Write((long)-12345678901);
            Assert.AreEqual(8L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((long)-12345678901, ms.ReadLong());
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable((long)-12345678901);
            Assert.AreEqual(9L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((long)-12345678901, ms.ReadLongNullable());
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable((long?)null);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.IsNull(ms.ReadLongNullable());
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable(long.MinValue);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(long.MinValue, ms.ReadLongNullable());
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable(long.MaxValue);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(long.MaxValue, ms.ReadLongNullable());
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable((long)0);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((long)0, ms.ReadLongNullable());
        }

        [TestMethod]
        public async Task LongAsync_Tests()
        {
            using MemoryStream ms = new();
            await ms.WriteAsync((long)-12345678901);
            Assert.AreEqual(8L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((long)-12345678901, await ms.ReadLongAsync());
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync((long)-12345678901);
            Assert.AreEqual(9L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((long)-12345678901, await ms.ReadLongNullableAsync());
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync((long?)null);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.IsNull(await ms.ReadLongNullableAsync());
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync(long.MinValue);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(long.MinValue, await ms.ReadLongNullableAsync());
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync(long.MaxValue);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(long.MaxValue, await ms.ReadLongNullableAsync());
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync((long)0);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((long)0, await ms.ReadLongNullableAsync());
        }

        [TestMethod]
        public void ULong_Tests()
        {
            using MemoryStream ms = new();
            ms.Write((ulong)12345678901);
            Assert.AreEqual(8L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((ulong)12345678901, ms.ReadULong());
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable((ulong)12345678901);
            Assert.AreEqual(9L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((ulong)12345678901, ms.ReadULongNullable());
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable((ulong?)null);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.IsNull(ms.ReadULongNullable());
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable(ulong.MinValue);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(ulong.MinValue, ms.ReadULongNullable());
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable(ulong.MaxValue);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(ulong.MaxValue, ms.ReadULongNullable());
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable((ulong)0);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((ulong)0, ms.ReadULongNullable());
        }

        [TestMethod]
        public async Task ULongAsync_Tests()
        {
            using MemoryStream ms = new();
            await ms.WriteAsync((ulong)12345678901);
            Assert.AreEqual(8L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((ulong)12345678901, await ms.ReadULongAsync());
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync((ulong)12345678901);
            Assert.AreEqual(9L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((ulong)12345678901, await ms.ReadULongNullableAsync());
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync((ulong?)null);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.IsNull(await ms.ReadULongNullableAsync());
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync(ulong.MinValue);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(ulong.MinValue, await ms.ReadULongNullableAsync());
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync(ulong.MaxValue);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(ulong.MaxValue, await ms.ReadULongNullableAsync());
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync((ulong)0);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((ulong)0, await ms.ReadULongNullableAsync());
        }
    }
}
