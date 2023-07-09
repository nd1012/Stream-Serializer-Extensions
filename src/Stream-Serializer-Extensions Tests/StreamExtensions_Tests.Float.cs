using wan24.StreamSerializerExtensions;

namespace Stream_Serializer_Extensions_Tests
{
    public partial class StreamExtensions_Tests
    {
        [TestMethod]
        public void Float_Tests()
        {
            using MemoryStream ms = new();
            ms.Write((float)-123456);
            Assert.AreEqual(4L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((float)-123456, ms.ReadFloat());
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable((float)-123456);
            Assert.AreEqual(5L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((float)-123456, ms.ReadFloatNullable());
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable((float?)null);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.IsNull(ms.ReadFloatNullable());
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable(float.MinValue);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(float.MinValue, ms.ReadFloatNullable());
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable(float.MaxValue);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(float.MaxValue, ms.ReadFloatNullable());
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable((float)0);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((float)0, ms.ReadFloatNullable());
        }

        [TestMethod]
        public async Task FloatAsync_Tests()
        {
            using MemoryStream ms = new();
            await ms.WriteAsync((float)-123456);
            Assert.AreEqual(4L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((float)-123456, await ms.ReadFloatAsync());
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync((float)-123456);
            Assert.AreEqual(5L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((float)-123456, await ms.ReadFloatNullableAsync());
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync((float?)null);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.IsNull(await ms.ReadFloatNullableAsync());
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync(float.MinValue);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(float.MinValue, await ms.ReadFloatNullableAsync());
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync(float.MaxValue);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(float.MaxValue, await ms.ReadFloatNullableAsync());
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync((float)0);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((float)0, await ms.ReadFloatNullableAsync());
        }
    }
}
