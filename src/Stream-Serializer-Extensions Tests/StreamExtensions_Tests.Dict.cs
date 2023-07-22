using System.Collections;
using wan24.StreamSerializerExtensions;

namespace Stream_Serializer_Extensions_Tests
{
    public partial class StreamExtensions_Tests
    {
        [TestMethod]
        public void Dict_Tests()
        {
            using MemoryStream ms = new();
            using SerializerContext sc = new(ms);
            using DeserializerContext dc = new(ms);
            Dictionary<int, bool> dict = new()
            {
                {0, true },
                {1, false }
            };
            ms.WriteDict(dict, sc);
            ms.Position = 0;
            CompareDict(dict, ms.ReadDict<int, bool>(dc));
            dict.Clear();
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteDict(dict, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteDictNullable(null, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.IsNull(ms.ReadDictNullable<int, bool>(dc));
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteDictNullable(dict, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.IsNotNull(ms.ReadDictNullable<int, bool>(dc));
        }

        [TestMethod]
        public async Task DictAsync_Tests()
        {
            using MemoryStream ms = new();
            using SerializerContext sc = new(ms);
            using DeserializerContext dc = new(ms);
            Dictionary<int, bool> dict = new()
            {
                {0, true },
                {1, false }
            };
            await ms.WriteDictAsync(dict, sc);
            ms.Position = 0;
            CompareDict(dict, await ms.ReadDictAsync<int, bool>(dc));
            dict.Clear();
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteDictAsync(dict, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteDictNullableAsync(null, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.IsNull(await ms.ReadDictNullableAsync<int, bool>(dc));
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteDictNullableAsync(dict, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.IsNotNull(await ms.ReadDictNullableAsync<int, bool>(dc));
        }

        private static void CompareDict(IDictionary a, IDictionary b)
        {
            Assert.AreEqual(a.Count, b.Count);
            foreach (object key in a.Keys)
            {
                Assert.IsTrue(b.Contains(key));
                CompareObjects(a[key], b[key]);
            }
        }
    }
}
