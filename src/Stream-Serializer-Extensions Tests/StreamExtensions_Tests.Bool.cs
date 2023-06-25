using wan24.StreamSerializerExtensions;

namespace Stream_Serializer_Extensions_Tests
{
    public partial class StreamExtensions_Tests
    {
        [TestMethod]
        public void Bool_Tests()
        {
            using MemoryStream ms = new();
            ms.Write(true);
            Assert.AreEqual(1L, ms.Position);
            ms.Position = 0;
            Assert.IsTrue(ms.ReadBool());
            ms.SetLength(0);
            ms.Position = 0;
            ms.Write(false);
            Assert.AreEqual(1L, ms.Position);
            ms.Position = 0;
            Assert.IsFalse(ms.ReadBool());
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable((bool?)null);
            Assert.AreEqual(1L, ms.Position);
            ms.Position = 0;
            Assert.IsNull(ms.ReadBoolNullable());
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNullable(true);
            Assert.AreEqual(1L, ms.Position);
            ms.Position = 0;
            Assert.IsTrue(ms.ReadBoolNullable());
        }

        [TestMethod]
        public async Task BoolAsync_Tests()
        {
            using MemoryStream ms = new();
            await ms.WriteAsync(true);
            Assert.AreEqual(1L, ms.Position);
            ms.Position = 0;
            Assert.IsTrue(await ms.ReadBoolAsync());
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteAsync(false);
            Assert.AreEqual(1L, ms.Position);
            ms.Position = 0;
            Assert.IsFalse(await ms.ReadBoolAsync());
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync((bool?)null);
            Assert.AreEqual(1L, ms.Position);
            ms.Position = 0;
            Assert.IsNull(await ms.ReadBoolNullableAsync());
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNullableAsync(true);
            Assert.AreEqual(1L, ms.Position);
            ms.Position = 0;
            Assert.IsTrue(await ms.ReadBoolNullableAsync());
        }
    }
}
