using wan24.StreamSerializerExtensions;

namespace Stream_Serializer_Extensions_Tests
{
    public partial class StreamExtensions_Tests
    {
        [TestMethod]
        public void Decimal_Tests()
        {
            using MemoryStream ms = new();
            using SerializerContext sc = new(ms);
            using DeserializerContext dc = new(ms);
            ms.Write((decimal)-1234567890123456789, sc);
            Assert.AreEqual(16L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((decimal)-1234567890123456789, ms.ReadDecimal(dc));
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable((decimal)-1234567890123456789, sc);
            Assert.AreEqual(17L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((decimal)-1234567890123456789, ms.ReadDecimalNullable(dc));
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable((decimal?)null, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.IsNull(ms.ReadDecimalNullable(dc));
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable(decimal.MinValue, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(decimal.MinValue, ms.ReadDecimalNullable(dc));
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable(decimal.MaxValue, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(decimal.MaxValue, ms.ReadDecimalNullable(dc));
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable((decimal)0, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((decimal)0, ms.ReadDecimalNullable(dc));
        }

        [TestMethod]
        public async Task DecimalAsync_Tests()
        {
            using MemoryStream ms = new();
            using SerializerContext sc = new(ms);
            using DeserializerContext dc = new(ms);
            await ms.WriteAsync((decimal)-1234567890123456789, sc);
            Assert.AreEqual(16L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((decimal)-1234567890123456789, await ms.ReadDecimalAsync(dc));
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync((decimal)-1234567890123456789, sc);
            Assert.AreEqual(17L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((decimal)-1234567890123456789, await ms.ReadDecimalNullableAsync(dc));
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync((decimal?)null, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.IsNull(await ms.ReadDecimalNullableAsync(dc));
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync(decimal.MinValue, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(decimal.MinValue, await ms.ReadDecimalNullableAsync(dc));
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync(decimal.MaxValue, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(decimal.MaxValue, await ms.ReadDecimalNullableAsync(dc));
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync((decimal)0, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((decimal)0, await ms.ReadDecimalNullableAsync(dc));
        }
    }
}
