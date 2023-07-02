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
            List<bool> list = new()
            {
                true,
                false
            };
            ms.WriteList(list);
            ms.Position = 0;
            CompareList(list, ms.ReadList<bool>());
            list.Clear();
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteList(list);
            Assert.AreEqual(1L, ms.Length);
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteListNullable(null);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.IsNull(ms.ReadListNullable<bool>());
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteListNullable(list);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.IsNotNull(ms.ReadListNullable<bool>());
        }

        [TestMethod]
        public async Task ListAsync_Tests()
        {
            using MemoryStream ms = new();
            List<bool> list = new()
            {
                true,
                false
            };
            await ms.WriteListAsync(list);
            ms.Position = 0;
            CompareList(list, await ms.ReadListAsync<bool>());
            list.Clear();
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteListAsync(list);
            Assert.AreEqual(1L, ms.Length);
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteListNullableAsync(null);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.IsNull(await ms.ReadListNullableAsync<bool>());
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteListNullableAsync(list);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.IsNotNull(await ms.ReadListNullableAsync<bool>());
        }

        private static void CompareList(IList a, IList b)
        {
            Assert.AreEqual(a.Count, b.Count);
            for (int i = 0; i < a.Count; i++) CompareObjects(a[i], b[i]);
        }
    }
}
