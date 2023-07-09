using wan24.StreamSerializerExtensions;

namespace Stream_Serializer_Extensions_Tests
{
    public partial class StreamExtensions_Tests
    {
        [TestMethod]
        public void Int_Tests()
        {
            using MemoryStream ms = new();
            ms.Write((int)-123456);
            Assert.AreEqual(4L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((int)-123456, ms.ReadInt());
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable((int)-123456);
            Assert.AreEqual(5L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((int)-123456, ms.ReadIntNullable());
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable((int?)null);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.IsNull(ms.ReadIntNullable());
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable(int.MinValue);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(int.MinValue, ms.ReadIntNullable());
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable(int.MaxValue);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(int.MaxValue, ms.ReadIntNullable());
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable((int)0);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((int)0, ms.ReadIntNullable());
        }

        [TestMethod]
        public async Task IntAsync_Tests()
        {
            using MemoryStream ms = new();
            await ms.WriteAsync((int)-123456);
            Assert.AreEqual(4L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((int)-123456, await ms.ReadIntAsync());
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync((int)-123456);
            Assert.AreEqual(5L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((int)-123456, await ms.ReadIntNullableAsync());
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync((int?)null);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.IsNull(await ms.ReadIntNullableAsync());
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync(int.MinValue);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(int.MinValue, await ms.ReadIntNullableAsync());
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync(int.MaxValue);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(int.MaxValue, await ms.ReadIntNullableAsync());
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync((int)0);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((int)0, await ms.ReadIntNullableAsync());
        }

        [TestMethod]
        public void UInt_Tests()
        {
            using MemoryStream ms = new();
            ms.Write((uint)123456);
            Assert.AreEqual(4L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((uint)123456, ms.ReadUInt());
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable((uint)123456);
            Assert.AreEqual(5L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((uint)123456, ms.ReadUIntNullable());
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable((uint?)null);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.IsNull(ms.ReadUIntNullable());
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable(uint.MinValue);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(uint.MinValue, ms.ReadUIntNullable());
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable(uint.MaxValue);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(uint.MaxValue, ms.ReadUIntNullable());
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable((uint)0);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((uint)0, ms.ReadUIntNullable());
        }

        [TestMethod]
        public async Task UIntAsync_Tests()
        {
            using MemoryStream ms = new();
            await ms.WriteAsync((uint)123456);
            Assert.AreEqual(4L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((uint)123456, await ms.ReadUIntAsync());
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync((uint)123456);
            Assert.AreEqual(5L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((uint)123456, await ms.ReadUIntNullableAsync());
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync((uint?)null);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.IsNull(await ms.ReadUIntNullableAsync());
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync(uint.MinValue);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(uint.MinValue, await ms.ReadUIntNullableAsync());
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync(uint.MaxValue);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(uint.MaxValue, await ms.ReadUIntNullableAsync());
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync((uint)0);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((uint)0, await ms.ReadUIntNullableAsync());
        }
    }
}
