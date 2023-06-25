using wan24.StreamSerializerExtensions;

namespace Stream_Serializer_Extensions_Tests
{
    public partial class StreamExtensions_Tests
    {
        [TestMethod]
        public void Array_Tests()
        {
            using MemoryStream ms = new();
            bool[] arr = new bool[] { true, false };
            ms.WriteArray(arr);
            ms.Position = 0;
            CompareArray(arr, ms.ReadArray<bool>());
            arr = Array.Empty<bool>();
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteArray(arr);
            Assert.AreEqual(1L, ms.Length);
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteArrayNullable(null);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.IsNull(ms.ReadArrayNullable<bool>());
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteArrayNullable(arr);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.IsNotNull(ms.ReadArrayNullable<bool>());
        }

        [TestMethod]
        public async Task ArrayAsync_Tests()
        {
            using MemoryStream ms = new();
            bool[] arr = new bool[] { true, false };
            await ms.WriteArrayAsync(arr);
            ms.Position = 0;
            CompareArray(arr, await ms.ReadArrayAsync<bool>());
            arr = Array.Empty<bool>();
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteArrayAsync(arr);
            Assert.AreEqual(1L, ms.Length);
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteArrayNullableAsync(null);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.IsNull(await ms.ReadArrayNullableAsync<bool>());
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteArrayNullableAsync(arr);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.IsNotNull(await ms.ReadArrayNullableAsync<bool>());
        }

        private static void CompareArray(Array a, Array b)
        {
            Assert.AreEqual(a.Length, b.Length);
            for (int i = 0; i < a.Length; i++) Assert.AreEqual(a.GetValue(i), b.GetValue(i));
        }
    }
}
