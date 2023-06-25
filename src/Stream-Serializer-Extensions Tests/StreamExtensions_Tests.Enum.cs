using wan24.StreamSerializerExtensions;

namespace Stream_Serializer_Extensions_Tests
{
    public partial class StreamExtensions_Tests
    {
        [TestMethod]
        public void Enum_Tests()
        {
            using MemoryStream ms = new();
            ms.WriteEnum(TestEnum.One);
            Assert.AreEqual(2L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(TestEnum.One, ms.ReadEnum<TestEnum>());
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteEnum(TestEnum.Zero);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(TestEnum.Zero, ms.ReadEnum<TestEnum>());
        }

        [TestMethod]
        public async Task EnumAsync_Tests()
        {
            using MemoryStream ms = new();
            await ms.WriteEnumAsync(TestEnum.One);
            Assert.AreEqual(2L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(TestEnum.One, await ms.ReadEnumAsync<TestEnum>());
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteEnumAsync(TestEnum.Zero);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(TestEnum.Zero, await ms.ReadEnumAsync<TestEnum>());
        }
    }
}
