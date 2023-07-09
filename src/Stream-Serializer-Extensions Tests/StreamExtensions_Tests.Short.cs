using wan24.StreamSerializerExtensions;

namespace Stream_Serializer_Extensions_Tests
{
    public partial class StreamExtensions_Tests
    {
        [TestMethod]
        public void Short_Tests()
        {
            using MemoryStream ms = new();
            ms.Write((short)-1234);
            Assert.AreEqual(2L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((short)-1234, ms.ReadShort());
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable((short)-1234);
            Assert.AreEqual(3L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((short)-1234, ms.ReadShortNullable());
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable((short?)null);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.IsNull(ms.ReadShortNullable());
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable(short.MinValue);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(short.MinValue, ms.ReadShortNullable());
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable(short.MaxValue);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(short.MaxValue, ms.ReadShortNullable());
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable((short)0);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((short)0, ms.ReadShortNullable());
        }

        [TestMethod]
        public async Task ShortAsync_Tests()
        {
            using MemoryStream ms = new();
            await ms.WriteAsync((short)-1234);
            Assert.AreEqual(2L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((short)-1234, await ms.ReadShortAsync());
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync((short)-1234);
            Assert.AreEqual(3L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((short)-1234, await ms.ReadShortNullableAsync());
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync((short?)null);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.IsNull(await ms.ReadShortNullableAsync());
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync(short.MinValue);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(short.MinValue, await ms.ReadShortNullableAsync());
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync(short.MaxValue);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(short.MaxValue, await ms.ReadShortNullableAsync());
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync((short)0);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((short)0, await ms.ReadShortNullableAsync());
        }

        [TestMethod]
        public void UShort_Tests()
        {
            using MemoryStream ms = new();
            ms.Write((ushort)1234);
            Assert.AreEqual(2L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((ushort)1234, ms.ReadUShort());
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable((ushort)1234);
            Assert.AreEqual(3L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((ushort)1234, ms.ReadUShortNullable());
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable((ushort?)null);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.IsNull(ms.ReadUShortNullable());
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable(ushort.MinValue);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(ushort.MinValue, ms.ReadUShortNullable());
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable(ushort.MaxValue);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(ushort.MaxValue, ms.ReadUShortNullable());
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable((ushort)0);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((ushort)0, ms.ReadUShortNullable());
        }

        [TestMethod]
        public async Task UShortAsync_Tests()
        {
            using MemoryStream ms = new();
            await ms.WriteAsync((ushort)1234);
            Assert.AreEqual(2L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((ushort)1234, await ms.ReadUShortAsync());
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync((ushort)1234);
            Assert.AreEqual(3L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((ushort)1234, await ms.ReadUShortNullableAsync());
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync((ushort?)null);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.IsNull(await ms.ReadUShortNullableAsync());
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync(ushort.MinValue);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(ushort.MinValue, await ms.ReadUShortNullableAsync());
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync(ushort.MaxValue);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(ushort.MaxValue, await ms.ReadUShortNullableAsync());
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync((ushort)0);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((ushort)0, await ms.ReadUShortNullableAsync());
        }
    }
}
