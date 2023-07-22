using wan24.StreamSerializerExtensions;

namespace Stream_Serializer_Extensions_Tests
{
    public partial class StreamExtensions_Tests
    {
        [TestMethod]
        public void Serialized_Tests()
        {
            using MemoryStream ms = new();
            using SerializerContext sc = new(ms);
            using DeserializerContext dc = new(ms);
            TestObject2 obj = new()
            {
                Value = true
            };
            ms.WriteSerialized(obj, sc);
            ms.Position = 0;
            Assert.IsTrue(ms.ReadSerialized<TestObject2>(dc).Value);
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteSerializedNullable(obj, sc);
            ms.Position = 0;
            Assert.IsTrue(ms.ReadSerializedNullable<TestObject2>(dc)?.Value);
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteSerializedNullable((TestObject2?)null, sc);
            ms.Position = 0;
            Assert.IsNull(ms.ReadSerializedNullable<TestObject2>(dc));
        }

        [TestMethod]
        public async Task SerializedAsync_Tests()
        {
            using MemoryStream ms = new();
            using SerializerContext sc = new(ms);
            using DeserializerContext dc = new(ms);
            TestObject2 obj = new()
            {
                Value = true
            };
            await ms.WriteSerializedAsync(obj, sc);
            ms.Position = 0;
            Assert.IsTrue((await ms.ReadSerializedAsync<TestObject2>(dc)).Value);
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteSerializedNullableAsync(obj, sc);
            ms.Position = 0;
            Assert.IsTrue((await ms.ReadSerializedNullableAsync<TestObject2>(dc))?.Value);
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteSerializedNullableAsync((TestObject2?)null, sc);
            ms.Position = 0;
            Assert.IsNull(await ms.ReadSerializedNullableAsync<TestObject2>(dc));
        }
    }
}
