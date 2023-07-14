using wan24.StreamSerializerExtensions;

namespace Stream_Serializer_Extensions_Tests
{
    public partial class StreamExtensions_Tests
    {
        [TestMethod]
        public void Enum_Tests()
        {
            using MemoryStream ms = new();
            using SerializerContext sc = new(ms);
            using DeserializerContext dc = new(ms);
            ms.WriteEnum(TestEnum.One, sc);
            Assert.AreEqual(2L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(TestEnum.One, ms.ReadEnum<TestEnum>(dc));
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteEnum(TestEnum.Zero, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(TestEnum.Zero, ms.ReadEnum<TestEnum>(dc));
        }

        [TestMethod]
        public async Task EnumAsync_Tests()
        {
            using MemoryStream ms = new();
            using SerializerContext sc = new(ms);
            using DeserializerContext dc = new(ms);
            await ms.WriteEnumAsync(TestEnum.One, sc);
            Assert.AreEqual(2L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(TestEnum.One, await ms.ReadEnumAsync<TestEnum>(dc));
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteEnumAsync(TestEnum.Zero, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(TestEnum.Zero, await ms.ReadEnumAsync<TestEnum>(dc));
        }
    }
}
