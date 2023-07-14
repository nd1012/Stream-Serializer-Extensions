using wan24.StreamSerializerExtensions;

namespace Stream_Serializer_Extensions_Tests
{
    public partial class StreamExtensions_Tests
    {
        [TestMethod]
        public void Struct_Tests()
        {
            using MemoryStream ms = new();
            using SerializerContext sc = new(ms);
            using DeserializerContext dc = new(ms);

            TestStruct ts = new(true);
            ms.WriteStruct(ts, sc);
            ms.Position = 0;
            Assert.IsTrue(ms.ReadStruct<TestStruct>(dc).Value);
            ms.SetLength(0);
            ms.Position = 0;

            ms.WriteStructNullable(ts, sc);
            ms.Position = 0;
            Assert.IsTrue(ms.ReadStructNullable<TestStruct>(dc)?.Value);
            ms.SetLength(0);
            ms.Position = 0;

            ms.WriteStructNullable((TestStruct?)null, sc);
            ms.Position = 0;
            Assert.AreEqual(1L, ms.Length);
            Assert.IsNull(ms.ReadStructNullable<TestStruct>(dc));
        }

        [TestMethod]
        public async Task StructAsync_Tests()
        {
            using MemoryStream ms = new();
            using SerializerContext sc = new(ms);
            using DeserializerContext dc = new(ms);

            TestStruct ts = new(true);
            await ms.WriteStructAsync(ts, sc);
            ms.Position = 0;
            Assert.IsTrue((await ms.ReadStructAsync<TestStruct>(dc)).Value);
            ms.SetLength(0);
            ms.Position = 0;

            await ms.WriteStructNullableAsync(ts, sc);
            ms.Position = 0;
            Assert.IsTrue((await ms.ReadStructNullableAsync<TestStruct>(dc))?.Value);
            ms.SetLength(0);
            ms.Position = 0;

            await ms.WriteStructNullableAsync((TestStruct?)null, sc);
            ms.Position = 0;
            Assert.AreEqual(1L, ms.Length);
            Assert.IsNull(await ms.ReadStructNullableAsync<TestStruct>(dc));
        }
    }
}
