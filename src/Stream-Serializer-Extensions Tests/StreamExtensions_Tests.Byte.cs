using wan24.StreamSerializerExtensions;

namespace Stream_Serializer_Extensions_Tests
{
    public partial class StreamExtensions_Tests
    {
        [TestMethod]
        public void SByte_Tests()
        {
            using MemoryStream ms = new();
            using SerializerContext sc = new(ms);
            using DeserializerContext dc = new(ms);
            ms.Write((sbyte)-123, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((sbyte)-123, ms.ReadOneSByte(dc));
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable((sbyte)-123, sc);
            Assert.AreEqual(2L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((sbyte)-123, ms.ReadOneSByteNullable(dc));
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable((sbyte?)null, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.IsNull(ms.ReadOneByteNullable(dc));
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable(sbyte.MinValue, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(sbyte.MinValue, ms.ReadOneSByteNullable(dc));
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable(sbyte.MaxValue, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(sbyte.MaxValue, ms.ReadOneSByteNullable(dc));
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable((sbyte)0, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((sbyte)0, ms.ReadOneSByteNullable(dc));
        }

        [TestMethod]
        public async Task SByteAsync_Tests()
        {
            using MemoryStream ms = new();
            using SerializerContext sc = new(ms);
            using DeserializerContext dc = new(ms);
            await ms.WriteAsync((sbyte)-123, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((sbyte)-123, await ms.ReadOneSByteAsync(dc));
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync((sbyte)-123, sc);
            Assert.AreEqual(2L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((sbyte)-123, await ms.ReadOneSByteNullableAsync(dc));
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync((sbyte?)null, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.IsNull(await ms.ReadOneByteNullableAsync(dc));
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync(sbyte.MinValue, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(sbyte.MinValue, await ms.ReadOneSByteNullableAsync(dc));
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync(sbyte.MaxValue, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(sbyte.MaxValue, await ms.ReadOneSByteNullableAsync(dc));
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync((sbyte)0, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((sbyte)0, await ms.ReadOneSByteNullableAsync(dc));
        }

        [TestMethod]
        public void Byte_Tests()
        {
            using MemoryStream ms = new();
            using SerializerContext sc = new(ms);
            using DeserializerContext dc = new(ms);
            ms.Write((byte)123, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((byte)123, ms.ReadOneByte(dc));
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable((byte)123, sc);
            Assert.AreEqual(2L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((byte)123, ms.ReadOneByteNullable(dc));
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable((byte?)null, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.IsNull(ms.ReadOneByteNullable(dc));
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable(byte.MinValue, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(byte.MinValue, ms.ReadOneByteNullable(dc));
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable(byte.MaxValue, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(byte.MaxValue, ms.ReadOneByteNullable(dc));
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable((byte)0, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((byte)0, ms.ReadOneByteNullable(dc));
        }

        [TestMethod]
        public async Task ByteAsync_Tests()
        {
            using MemoryStream ms = new();
            using SerializerContext sc = new(ms);
            using DeserializerContext dc = new(ms);
            await ms.WriteAsync((byte)123, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((byte)123, await ms.ReadOneByteAsync(dc));
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync((byte)123, sc);
            Assert.AreEqual(2L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((byte)123, await ms.ReadOneByteNullableAsync(dc));
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync((byte?)null, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.IsNull(await ms.ReadOneByteNullableAsync(dc));
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync(byte.MinValue, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(byte.MinValue, await ms.ReadOneByteNullableAsync(dc));
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync(byte.MaxValue, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(byte.MaxValue, await ms.ReadOneByteNullableAsync(dc));
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync((byte)0, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((byte)0, await ms.ReadOneByteNullableAsync(dc));
        }
    }
}
