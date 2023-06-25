using wan24.StreamSerializerExtensions;

namespace Stream_Serializer_Extensions_Tests
{
    public partial class StreamExtensions_Tests
    {
        [TestMethod]
        public void Struct_Tests()
        {
            using MemoryStream ms = new();

            TestStruct ts = new(true);
            ms.WriteStruct(ts);
            ms.Position = 0;
            Assert.IsTrue(ms.ReadStruct<TestStruct>().Value);
            ms.SetLength(0);
            ms.Position = 0;

            ms.WriteStructNullable(ts);
            ms.Position = 0;
            Assert.IsTrue(ms.ReadStructNullable<TestStruct>()?.Value);
            ms.SetLength(0);
            ms.Position = 0;

            ms.WriteStructNullable((TestStruct?)null);
            ms.Position = 0;
            Assert.AreEqual(1L, ms.Length);
            Assert.IsNull(ms.ReadStructNullable<TestStruct>());
        }

        [TestMethod]
        public async Task StructAsync_Tests()
        {
            using MemoryStream ms = new();

            TestStruct ts = new(true);
            await ms.WriteStructAsync(ts);
            ms.Position = 0;
            Assert.IsTrue((await ms.ReadStructAsync<TestStruct>()).Value);
            ms.SetLength(0);
            ms.Position = 0;

            await ms.WriteStructNullableAsync(ts);
            ms.Position = 0;
            Assert.IsTrue((await ms.ReadStructNullableAsync<TestStruct>())?.Value);
            ms.SetLength(0);
            ms.Position = 0;

            await ms.WriteStructNullableAsync((TestStruct?)null);
            ms.Position = 0;
            Assert.AreEqual(1L, ms.Length);
            Assert.IsNull(await ms.ReadStructNullableAsync<TestStruct>());
        }
    }
}
