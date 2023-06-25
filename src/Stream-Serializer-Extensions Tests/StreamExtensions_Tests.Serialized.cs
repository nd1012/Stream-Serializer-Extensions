using wan24.StreamSerializerExtensions;

namespace Stream_Serializer_Extensions_Tests
{
    public partial class StreamExtensions_Tests
    {
        [TestMethod]
        public void Serialized_Tests()
        {
            using MemoryStream ms = new();

            TestObject2 obj = new()
            {
                Value = true
            };
            ms.WriteSerialized(obj);
            ms.Position = 0;
            Assert.IsTrue(ms.ReadSerialized<TestObject2>().Value);
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteSerializedNullable(obj);
            ms.Position = 0;
            Assert.IsTrue(ms.ReadSerializedNullable<TestObject2>()?.Value);
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteSerializedNullable((TestObject2?)null);
            ms.Position = 0;
            Assert.IsNull(ms.ReadSerializedNullable<TestObject2>());
        }

        [TestMethod]
        public async Task SerializedAsync_Tests()
        {
            using MemoryStream ms = new();

            TestObject2 obj = new()
            {
                Value = true
            };
            await ms.WriteSerializedAsync(obj);
            ms.Position = 0;
            Assert.IsTrue((await ms.ReadSerializedAsync<TestObject2>()).Value);
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteSerializedNullableAsync(obj);
            ms.Position = 0;
            Assert.IsTrue((await ms.ReadSerializedNullableAsync<TestObject2>())?.Value);
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteSerializedNullableAsync((TestObject2?)null);
            ms.Position = 0;
            Assert.IsNull(await ms.ReadSerializedNullableAsync<TestObject2>());
        }
    }
}
