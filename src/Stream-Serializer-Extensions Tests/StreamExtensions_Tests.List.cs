using System.Collections;
using wan24.StreamSerializerExtensions;

namespace Stream_Serializer_Extensions_Tests
{
    public partial class StreamExtensions_Tests
    {
        [TestMethod]
        public void List_Tests()
        {
            using MemoryStream ms = new();
            using SerializerContext sc = new(ms);
            using DeserializerContext dc = new(ms);
            List<bool> list = new()
            {
                true,
                false
            };
            ms.WriteList(list, sc);
            ms.Position = 0;
            CompareList(list, ms.ReadList<bool>(dc));
            list.Clear();
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteList(list, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteListNullable(null, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.IsNull(ms.ReadListNullable<bool>(dc));
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteListNullable(list, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.IsNotNull(ms.ReadListNullable<bool>(dc));
        }

        [TestMethod]
        public async Task ListAsync_Tests()
        {
            using MemoryStream ms = new();
            using SerializerContext sc = new(ms);
            using DeserializerContext dc = new(ms);
            List<bool> list = new()
            {
                true,
                false
            };
            await ms.WriteListAsync(list, sc);
            ms.Position = 0;
            CompareList(list, await ms.ReadListAsync<bool>(dc));
            list.Clear();
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteListAsync(list, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteListNullableAsync(null, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.IsNull(await ms.ReadListNullableAsync<bool>(dc));
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteListNullableAsync(list, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.IsNotNull(await ms.ReadListNullableAsync<bool>(dc));
        }

        private static void CompareList(IList a, IList b)
        {
            Assert.AreEqual(a.Count, b.Count);
            for (int i = 0; i < a.Count; i++) CompareObjects(a[i], b[i]);
        }
    }
}
