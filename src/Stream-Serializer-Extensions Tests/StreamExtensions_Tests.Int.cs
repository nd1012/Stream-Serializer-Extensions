using wan24.StreamSerializerExtensions;

namespace Stream_Serializer_Extensions_Tests
{
    public partial class StreamExtensions_Tests
    {
        [TestMethod]
        public void Int_Tests()
        {
            using MemoryStream ms = new();
            using SerializerContext sc = new(ms);
            using DeserializerContext dc = new(ms);
            ms.Write((int)-123456, sc);
            Assert.AreEqual(4L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((int)-123456, ms.ReadInt(dc));
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable((int)-123456, sc);
            Assert.AreEqual(5L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((int)-123456, ms.ReadIntNullable(dc));
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable((int?)null, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.IsNull(ms.ReadIntNullable(dc));
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable(int.MinValue, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(int.MinValue, ms.ReadIntNullable(dc));
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable(int.MaxValue, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(int.MaxValue, ms.ReadIntNullable(dc));
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable((int)0, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((int)0, ms.ReadIntNullable(dc));
        }

        [TestMethod]
        public async Task IntAsync_Tests()
        {
            using MemoryStream ms = new();
            using SerializerContext sc = new(ms);
            using DeserializerContext dc = new(ms);
            await ms.WriteAsync((int)-123456, sc);
            Assert.AreEqual(4L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((int)-123456, await ms.ReadIntAsync(dc));
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync((int)-123456, sc);
            Assert.AreEqual(5L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((int)-123456, await ms.ReadIntNullableAsync(dc));
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync((int?)null, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.IsNull(await ms.ReadIntNullableAsync(dc));
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync(int.MinValue, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(int.MinValue, await ms.ReadIntNullableAsync(dc));
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync(int.MaxValue, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(int.MaxValue, await ms.ReadIntNullableAsync(dc));
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync((int)0, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((int)0, await ms.ReadIntNullableAsync(dc));
        }

        [TestMethod]
        public void UInt_Tests()
        {
            using MemoryStream ms = new();
            using SerializerContext sc = new(ms);
            using DeserializerContext dc = new(ms);
            ms.Write((uint)123456, sc);
            Assert.AreEqual(4L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((uint)123456, ms.ReadUInt(dc));
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable((uint)123456, sc);
            Assert.AreEqual(5L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((uint)123456, ms.ReadUIntNullable(dc));
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable((uint?)null, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.IsNull(ms.ReadUIntNullable(dc));
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable(uint.MinValue, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(uint.MinValue, ms.ReadUIntNullable(dc));
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable(uint.MaxValue, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(uint.MaxValue, ms.ReadUIntNullable(dc));
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable((uint)0, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((uint)0, ms.ReadUIntNullable(dc));
        }

        [TestMethod]
        public async Task UIntAsync_Tests()
        {
            using MemoryStream ms = new();
            using SerializerContext sc = new(ms);
            using DeserializerContext dc = new(ms);
            await ms.WriteAsync((uint)123456, sc);
            Assert.AreEqual(4L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((uint)123456, await ms.ReadUIntAsync(dc));
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync((uint)123456, sc);
            Assert.AreEqual(5L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((uint)123456, await ms.ReadUIntNullableAsync(dc));
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync((uint?)null, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.IsNull(await ms.ReadUIntNullableAsync(dc));
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync(uint.MinValue, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(uint.MinValue, await ms.ReadUIntNullableAsync(dc));
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync(uint.MaxValue, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(uint.MaxValue, await ms.ReadUIntNullableAsync(dc));
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync((uint)0, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((uint)0, await ms.ReadUIntNullableAsync(dc));
        }
    }
}
