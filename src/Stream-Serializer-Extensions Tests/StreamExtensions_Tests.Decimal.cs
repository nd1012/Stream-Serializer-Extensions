using wan24.StreamSerializerExtensions;

namespace Stream_Serializer_Extensions_Tests
{
    public partial class StreamExtensions_Tests
    {
        [TestMethod]
        public void Decimal_Tests()
        {
            using MemoryStream ms = new();
            ms.Write((decimal)-1234567890123456789);
            Assert.AreEqual(16L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((decimal)-1234567890123456789, ms.ReadDecimal());
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable((decimal)-1234567890123456789);
            Assert.AreEqual(17L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((decimal)-1234567890123456789, ms.ReadDecimalNullable());
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable((decimal?)null);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.IsNull(ms.ReadDecimalNullable());
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable(decimal.MinValue);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(decimal.MinValue, ms.ReadDecimalNullable());
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable(decimal.MaxValue);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(decimal.MaxValue, ms.ReadDecimalNullable());
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable((decimal)0);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((decimal)0, ms.ReadDecimalNullable());
        }

        [TestMethod]
        public async Task DecimalAsync_Tests()
        {
            using MemoryStream ms = new();
            await ms.WriteAsync((decimal)-1234567890123456789);
            Assert.AreEqual(16L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((decimal)-1234567890123456789, await ms.ReadDecimalAsync());
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync((decimal)-1234567890123456789);
            Assert.AreEqual(17L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((decimal)-1234567890123456789, await ms.ReadDecimalNullableAsync());
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync((decimal?)null);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.IsNull(await ms.ReadDecimalNullableAsync());
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync(decimal.MinValue);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(decimal.MinValue, await ms.ReadDecimalNullableAsync());
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync(decimal.MaxValue);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(decimal.MaxValue, await ms.ReadDecimalNullableAsync());
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync((decimal)0);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((decimal)0, await ms.ReadDecimalNullableAsync());
        }
    }
}
