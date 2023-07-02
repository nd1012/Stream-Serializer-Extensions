using Microsoft.Extensions.Primitives;
using System.Collections;
using System.Security.Cryptography;
using wan24.Core;
using wan24.StreamSerializerExtensions;

namespace Stream_Serializer_Extensions_Tests
{
    public partial class StreamExtensions_Tests
    {
        [TestMethod]
        public void Array_Tests()
        {
            using MemoryStream ms = new();
            {
                bool[] arr = new bool[] { true, false };
                ms.WriteArray(arr);
                Assert.AreEqual(4L, ms.Length);
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
            foreach (bool nullable in new bool[] { false, true })
            {
                Logging.WriteInfo($"Nullable values: {nullable}");
                using TestObject5 test = new();
                using MemoryStream testMs = new();
                byte[] testData = RandomNumberGenerator.GetBytes(200000);
                testMs.Write(testData);
                testMs.Position = 0;
                test.Stream = testMs;
                object?[] arr = new object?[]
                {
                    nullable ? null : false,
                    true,
                    0,
                    (sbyte)123,
                    (byte)123,
                    (short)123,
                    (ushort)123,
                    123,
                    123,
                    123,
                    (uint)123,
                    (long)123,
                    (ulong)123,
                    (float)123,
                    (double)123,
                    (decimal)123,
                    string.Empty,
                    "test",
                    Array.Empty<byte>(),
                    new byte[]{1,2,3 },
                    Array.Empty<int>(),
                    new int[]{1,2,3 },
                    new List<int>(),
                    new List<int>(){1,2,3},
                    new Dictionary<string, int>(),
                    new Dictionary<string, int>(){ { true.ToString(), 1 },{ false.ToString(), 0 } },
                    new TestObject(),
                    new TestObject2(),
                    new TestObject3(),
                    new TestObject4(),
                    test,
                    new TestStruct()
                };
                try
                {
                    ms.SetLength(0);
                    ms.Position = 0;
                    ms.WriteArray(arr, valuesNullable: nullable);
                    ms.Position = 0;
                    CompareArray(arr, ms.ReadArray<object>(valuesNullable: nullable));
                }
                finally
                {
                    foreach (object? o in arr) (o as IDisposable)?.Dispose();
                }
            }
        }

        [TestMethod]
        public async Task ArrayAsync_Tests()
        {
            using MemoryStream ms = new();
            bool[] arr = new bool[] { true, false };
            await ms.WriteArrayAsync(arr);
            Assert.AreEqual(4L, ms.Length);
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
            for (int i = 0; i < a.Length; i++) CompareObjects(a.GetValue(i), b.GetValue(i));
        }

        private static void CompareObjects(object? ao, object? bo)
        {
            if (ao is ITestObject ato)
            {
                ITestObject? bto = bo as ITestObject;
                Assert.IsNotNull(bto);
                Assert.IsTrue(ato.CompareWith(bto));
            }
            else if (ao is Array aao)
            {
                Array? bao = bo as Array;
                Assert.IsNotNull(bao);
                CompareArray(aao, bao);
            }
            else if (ao is IDictionary ado)
            {
                IDictionary? bdo = bo as IDictionary;
                Assert.IsNotNull(bdo);
                CompareDict(ado, bdo);
            }
            else if (ao is IList alo)
            {
                IList? blo = bo as IList;
                Assert.IsNotNull(blo);
                CompareList(alo, blo);
            }
            else
            {
                Assert.AreEqual(ao, bo);
            }
        }
    }
}
