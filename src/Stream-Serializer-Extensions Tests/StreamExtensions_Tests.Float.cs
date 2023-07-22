using wan24.StreamSerializerExtensions;

namespace Stream_Serializer_Extensions_Tests
{
    public partial class StreamExtensions_Tests
    {
        [TestMethod]
        public void Float_Tests()
        {
            using MemoryStream ms = new();
            using SerializerContext sc = new(ms);
            using DeserializerContext dc = new(ms);
            ms.Write((float)-123456, sc);
            Assert.AreEqual(4L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((float)-123456, ms.ReadFloat(dc));
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable((float)-123456, sc);
            Assert.AreEqual(5L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((float)-123456, ms.ReadFloatNullable(dc));
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable((float?)null, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.IsNull(ms.ReadFloatNullable(dc));
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable(float.MinValue, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(float.MinValue, ms.ReadFloatNullable(dc));
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable(float.MaxValue, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(float.MaxValue, ms.ReadFloatNullable(dc));
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable((float)0, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((float)0, ms.ReadFloatNullable(dc));
        }

        [TestMethod]
        public async Task FloatAsync_Tests()
        {
            using MemoryStream ms = new();
            using SerializerContext sc = new(ms);
            using DeserializerContext dc = new(ms);
            await ms.WriteAsync((float)-123456, sc);
            Assert.AreEqual(4L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((float)-123456, await ms.ReadFloatAsync(dc));
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync((float)-123456, sc);
            Assert.AreEqual(5L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((float)-123456, await ms.ReadFloatNullableAsync(dc));
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync((float?)null, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.IsNull(await ms.ReadFloatNullableAsync(dc));
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync(float.MinValue, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(float.MinValue, await ms.ReadFloatNullableAsync(dc));
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync(float.MaxValue, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(float.MaxValue, await ms.ReadFloatNullableAsync(dc));
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync((float)0, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual((float)0, await ms.ReadFloatNullableAsync(dc));
        }
    }
}
