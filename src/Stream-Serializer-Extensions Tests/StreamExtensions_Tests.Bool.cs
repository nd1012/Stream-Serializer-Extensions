using wan24.StreamSerializerExtensions;

namespace Stream_Serializer_Extensions_Tests
{
    public partial class StreamExtensions_Tests
    {
        [TestMethod]
        public void Bool_Tests()
        {
            using MemoryStream ms = new();
            using SerializerContext sc = new(ms);
            using DeserializerContext dc = new(ms);
            ms.Write(true, sc);
            Assert.AreEqual(1L, ms.Position);
            ms.Position = 0;
            Assert.IsTrue(ms.ReadBool(dc));
            ms.SetLength(0);
            ms.Position = 0;
            ms.Write(false, sc);
            Assert.AreEqual(1L, ms.Position);
            ms.Position = 0;
            Assert.IsFalse(ms.ReadBool(dc));
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable((bool?)null, sc);
            Assert.AreEqual(1L, ms.Position);
            ms.Position = 0;
            Assert.IsNull(ms.ReadBoolNullable(dc));
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable(true, sc);
            Assert.AreEqual(1L, ms.Position);
            ms.Position = 0;
            Assert.IsTrue(ms.ReadBoolNullable(dc));
        }

        [TestMethod]
        public async Task BoolAsync_Tests()
        {
            using MemoryStream ms = new();
            using SerializerContext sc = new(ms);
            using DeserializerContext dc = new(ms);
            await ms.WriteAsync(true, sc);
            Assert.AreEqual(1L, ms.Position);
            ms.Position = 0;
            Assert.IsTrue(await ms.ReadBoolAsync(dc));
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteAsync(false, sc);
            Assert.AreEqual(1L, ms.Position);
            ms.Position = 0;
            Assert.IsFalse(await ms.ReadBoolAsync(dc));
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync((bool?)null, sc);
            Assert.AreEqual(1L, ms.Position);
            ms.Position = 0;
            Assert.IsNull(await ms.ReadBoolNullableAsync(dc));
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync(true, sc);
            Assert.AreEqual(1L, ms.Position);
            ms.Position = 0;
            Assert.IsTrue(await ms.ReadBoolNullableAsync(dc));
        }
    }
}
