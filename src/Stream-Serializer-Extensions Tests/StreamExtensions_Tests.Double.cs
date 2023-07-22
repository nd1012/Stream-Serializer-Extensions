using wan24.StreamSerializerExtensions;

namespace Stream_Serializer_Extensions_Tests
{
    public partial class StreamExtensions_Tests
    {
        [TestMethod]
        public void Double_Tests()
        {
            using MemoryStream ms = new();
            using SerializerContext sc = new(ms);
            using DeserializerContext dc = new(ms);
            ms.Write((double)-12345678901, sc);
            Assert.AreEqual(8L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((double)-12345678901, ms.ReadDouble(dc));
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable((double)-12345678901, sc);
            Assert.AreEqual(9L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((double)-12345678901, ms.ReadDoubleNullable(dc));
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable((double?)null, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.IsNull(ms.ReadDoubleNullable(dc));
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable(double.MinValue, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(double.MinValue, ms.ReadDoubleNullable(dc));
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable(double.MaxValue, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(double.MaxValue, ms.ReadDoubleNullable(dc));
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable((double)0, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((double)0, ms.ReadDoubleNullable(dc));
        }

        [TestMethod]
        public async Task DoubleAsync_Tests()
        {
            using MemoryStream ms = new();
            using SerializerContext sc = new(ms);
            using DeserializerContext dc = new(ms);
            await ms.WriteAsync((double)-12345678901, sc);
            Assert.AreEqual(8L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((double)-12345678901, await ms.ReadDoubleAsync(dc));
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync((double)-12345678901, sc);
            Assert.AreEqual(9L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((double)-12345678901, await ms.ReadDoubleNullableAsync(dc));
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync((double?)null, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.IsNull(await ms.ReadDoubleNullableAsync(dc));
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync(double.MinValue, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(double.MinValue, await ms.ReadDoubleNullableAsync(dc));
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync(double.MaxValue, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(double.MaxValue, await ms.ReadDoubleNullableAsync(dc));
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync((double)0, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((double)0, await ms.ReadDoubleNullableAsync(dc));
        }
    }
}
