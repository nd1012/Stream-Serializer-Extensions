using wan24.StreamSerializerExtensions;

namespace Stream_Serializer_Extensions_Tests
{
    public partial class StreamExtensions_Tests
    {
        [TestMethod]
        public void SByte_Tests()
        {
            using MemoryStream ms = new();
            ms.Write((sbyte)-123);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((sbyte)-123, ms.ReadOneSByte());
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable((sbyte)-123);
            Assert.AreEqual(2L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((sbyte)-123, ms.ReadOneSByteNullable());
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable((sbyte?)null);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.IsNull(ms.ReadOneByteNullable());
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable(sbyte.MinValue);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(sbyte.MinValue, ms.ReadOneSByteNullable());
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable(sbyte.MaxValue);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(sbyte.MaxValue, ms.ReadOneSByteNullable());
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable((sbyte)0);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((sbyte)0, ms.ReadOneSByteNullable());
        }

        [TestMethod]
        public async Task SByteAsync_Tests()
        {
            using MemoryStream ms = new();
            await ms.WriteAsync((sbyte)-123);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((sbyte)-123, await ms.ReadOneSByteAsync());
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync((sbyte)-123);
            Assert.AreEqual(2L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((sbyte)-123, await ms.ReadOneSByteNullableAsync());
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync((sbyte?)null);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.IsNull(await ms.ReadOneByteNullableAsync());
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync(sbyte.MinValue);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(sbyte.MinValue, await ms.ReadOneSByteNullableAsync());
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync(sbyte.MaxValue);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(sbyte.MaxValue, await ms.ReadOneSByteNullableAsync());
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync((sbyte)0);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((sbyte)0, await ms.ReadOneSByteNullableAsync());
        }

        [TestMethod]
        public void Byte_Tests()
        {
            using MemoryStream ms = new();
            ms.Write((byte)123);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((byte)123, ms.ReadOneByte());
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable((byte)123);
            Assert.AreEqual(2L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((byte)123, ms.ReadOneByteNullable());
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable((byte?)null);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.IsNull(ms.ReadOneByteNullable());
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable(byte.MinValue);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(byte.MinValue, ms.ReadOneByteNullable());
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable(byte.MaxValue);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(byte.MaxValue, ms.ReadOneByteNullable());
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable((byte)0);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((byte)0, ms.ReadOneByteNullable());
        }

        [TestMethod]
        public async Task ByteAsync_Tests()
        {
            using MemoryStream ms = new();
            await ms.WriteAsync((byte)123);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((byte)123, await ms.ReadOneByteAsync());
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync((byte)123);
            Assert.AreEqual(2L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((byte)123, await ms.ReadOneByteNullableAsync());
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync((byte?)null);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.IsNull(await ms.ReadOneByteNullableAsync());
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync(byte.MinValue);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(byte.MinValue, await ms.ReadOneByteNullableAsync());
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync(byte.MaxValue);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(byte.MaxValue, await ms.ReadOneByteNullableAsync());
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync((byte)0);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((byte)0, await ms.ReadOneByteNullableAsync());
        }
    }
}
