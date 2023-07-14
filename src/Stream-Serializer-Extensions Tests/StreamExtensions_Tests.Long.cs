using wan24.StreamSerializerExtensions;

namespace Stream_Serializer_Extensions_Tests
{
    public partial class StreamExtensions_Tests
    {
        [TestMethod]
        public void Long_Tests()
        {
            using MemoryStream ms = new();
            using SerializerContext sc = new(ms);
            using DeserializerContext dc = new(ms);
            ms.Write((long)-12345678901, sc);
            Assert.AreEqual(8L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((long)-12345678901, ms.ReadLong(dc));
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable((long)-12345678901, sc);
            Assert.AreEqual(9L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((long)-12345678901, ms.ReadLongNullable(dc));
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable((long?)null, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.IsNull(ms.ReadLongNullable(dc));
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable(long.MinValue, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(long.MinValue, ms.ReadLongNullable(dc));
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable(long.MaxValue, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(long.MaxValue, ms.ReadLongNullable(dc));
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable((long)0, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((long)0, ms.ReadLongNullable(dc));
        }

        [TestMethod]
        public async Task LongAsync_Tests()
        {
            using MemoryStream ms = new();
            using SerializerContext sc = new(ms);
            using DeserializerContext dc = new(ms);
            await ms.WriteAsync((long)-12345678901, sc);
            Assert.AreEqual(8L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((long)-12345678901, await ms.ReadLongAsync(dc));
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync((long)-12345678901, sc);
            Assert.AreEqual(9L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((long)-12345678901, await ms.ReadLongNullableAsync(dc));
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync((long?)null, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.IsNull(await ms.ReadLongNullableAsync(dc));
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync(long.MinValue, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(long.MinValue, await ms.ReadLongNullableAsync(dc));
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync(long.MaxValue, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(long.MaxValue, await ms.ReadLongNullableAsync(dc));
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync((long)0, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((long)0, await ms.ReadLongNullableAsync(dc));
        }

        [TestMethod]
        public void ULong_Tests()
        {
            using MemoryStream ms = new();
            using SerializerContext sc = new(ms);
            using DeserializerContext dc = new(ms);
            ms.Write((ulong)12345678901, sc);
            Assert.AreEqual(8L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((ulong)12345678901, ms.ReadULong(dc));
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable((ulong)12345678901, sc);
            Assert.AreEqual(9L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((ulong)12345678901, ms.ReadULongNullable(dc));
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable((ulong?)null, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.IsNull(ms.ReadULongNullable(dc));
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable(ulong.MinValue, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(ulong.MinValue, ms.ReadULongNullable(dc));
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable(ulong.MaxValue, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(ulong.MaxValue, ms.ReadULongNullable(dc));
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable((ulong)0, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((ulong)0, ms.ReadULongNullable(dc));
        }

        [TestMethod]
        public async Task ULongAsync_Tests()
        {
            using MemoryStream ms = new();
            using SerializerContext sc = new(ms);
            using DeserializerContext dc = new(ms);
            await ms.WriteAsync((ulong)12345678901, sc);
            Assert.AreEqual(8L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((ulong)12345678901, await ms.ReadULongAsync(dc));
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync((ulong)12345678901, sc);
            Assert.AreEqual(9L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((ulong)12345678901, await ms.ReadULongNullableAsync(dc));
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync((ulong?)null, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.IsNull(await ms.ReadULongNullableAsync(dc));
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync(ulong.MinValue, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(ulong.MinValue, await ms.ReadULongNullableAsync(dc));
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync(ulong.MaxValue, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(ulong.MaxValue, await ms.ReadULongNullableAsync(dc));
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync((ulong)0, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((ulong)0, await ms.ReadULongNullableAsync(dc));
        }
    }
}
