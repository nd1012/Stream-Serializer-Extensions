using wan24.StreamSerializerExtensions;

namespace Stream_Serializer_Extensions_Tests
{
    public partial class StreamExtensions_Tests
    {
        [TestMethod]
        public void Short_Tests()
        {
            using MemoryStream ms = new();
            using SerializerContext sc = new(ms);
            using DeserializerContext dc = new(ms);
            ms.Write((short)-1234, sc);
            Assert.AreEqual(2L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((short)-1234, ms.ReadShort(dc));
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable((short)-1234, sc);
            Assert.AreEqual(3L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((short)-1234, ms.ReadShortNullable(dc));
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable((short?)null, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.IsNull(ms.ReadShortNullable(dc));
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable(short.MinValue, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(short.MinValue, ms.ReadShortNullable(dc));
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable(short.MaxValue, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(short.MaxValue, ms.ReadShortNullable(dc));
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable((short)0, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((short)0, ms.ReadShortNullable(dc));
        }

        [TestMethod]
        public async Task ShortAsync_Tests()
        {
            using MemoryStream ms = new();
            using SerializerContext sc = new(ms);
            using DeserializerContext dc = new(ms);
            await ms.WriteAsync((short)-1234, sc);
            Assert.AreEqual(2L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((short)-1234, await ms.ReadShortAsync(dc));
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync((short)-1234, sc);
            Assert.AreEqual(3L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((short)-1234, await ms.ReadShortNullableAsync(dc));
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync((short?)null, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.IsNull(await ms.ReadShortNullableAsync(dc));
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync(short.MinValue, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(short.MinValue, await ms.ReadShortNullableAsync(dc));
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync(short.MaxValue, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(short.MaxValue, await ms.ReadShortNullableAsync(dc));
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync((short)0, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((short)0, await ms.ReadShortNullableAsync(dc));
        }

        [TestMethod]
        public void UShort_Tests()
        {
            using MemoryStream ms = new();
            using SerializerContext sc = new(ms);
            using DeserializerContext dc = new(ms);
            ms.Write((ushort)1234, sc);
            Assert.AreEqual(2L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((ushort)1234, ms.ReadUShort(dc));
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable((ushort)1234, sc);
            Assert.AreEqual(3L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((ushort)1234, ms.ReadUShortNullable(dc));
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable((ushort?)null, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.IsNull(ms.ReadUShortNullable(dc));
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable(ushort.MinValue, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(ushort.MinValue, ms.ReadUShortNullable(dc));
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable(ushort.MaxValue, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(ushort.MaxValue, ms.ReadUShortNullable(dc));
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable((ushort)0, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((ushort)0, ms.ReadUShortNullable(dc));
        }

        [TestMethod]
        public async Task UShortAsync_Tests()
        {
            using MemoryStream ms = new();
            using SerializerContext sc = new(ms);
            using DeserializerContext dc = new(ms);
            await ms.WriteAsync((ushort)1234, sc);
            Assert.AreEqual(2L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((ushort)1234, await ms.ReadUShortAsync(dc));
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync((ushort)1234, sc);
            Assert.AreEqual(3L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((ushort)1234, await ms.ReadUShortNullableAsync(dc));
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync((ushort?)null, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.IsNull(await ms.ReadUShortNullableAsync(dc));
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync(ushort.MinValue, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(ushort.MinValue, await ms.ReadUShortNullableAsync(dc));
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync(ushort.MaxValue, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(ushort.MaxValue, await ms.ReadUShortNullableAsync(dc));
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync((ushort)0, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((ushort)0, await ms.ReadUShortNullableAsync(dc));
        }
    }
}
