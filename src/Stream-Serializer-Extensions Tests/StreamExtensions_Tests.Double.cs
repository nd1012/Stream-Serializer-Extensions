using wan24.StreamSerializerExtensions;

namespace Stream_Serializer_Extensions_Tests
{
    public partial class StreamExtensions_Tests
    {
        [TestMethod]
        public void Double_Tests()
        {
            using MemoryStream ms = new();
            ms.Write((double)-12345678901);
            Assert.AreEqual(8L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((double)-12345678901, ms.ReadDouble());
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable((double)-12345678901);
            Assert.AreEqual(9L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((double)-12345678901, ms.ReadDoubleNullable());
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable((double?)null);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.IsNull(ms.ReadDoubleNullable());
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable(double.MinValue);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(double.MinValue, ms.ReadDoubleNullable());
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable(double.MaxValue);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(double.MaxValue, ms.ReadDoubleNullable());
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable((double)0);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((double)0, ms.ReadDoubleNullable());
        }

        [TestMethod]
        public async Task DoubleAsync_Tests()
        {
            using MemoryStream ms = new();
            await ms.WriteAsync((double)-12345678901);
            Assert.AreEqual(8L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((double)-12345678901, await ms.ReadDoubleAsync());
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync((double)-12345678901);
            Assert.AreEqual(9L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((double)-12345678901, await ms.ReadDoubleNullableAsync());
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync((double?)null);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.IsNull(await ms.ReadDoubleNullableAsync());
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync(double.MinValue);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(double.MinValue, await ms.ReadDoubleNullableAsync());
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync(double.MaxValue);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(double.MaxValue, await ms.ReadDoubleNullableAsync());
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync((double)0);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((double)0, await ms.ReadDoubleNullableAsync());
        }
    }
}
