using System.Collections;
using wan24.Core;
using wan24.StreamSerializerExtensions;

namespace Stream_Serializer_Extensions_Tests
{
    public partial class StreamExtensions_Tests
    {
        [TestMethod]
        public void Any_Tests()
        {
            StreamExtensions.AnyObjectAttributeRequired = false;
            try
            {
                using MemoryStream ms = new();
                using MemoryStream test = new();
                test.Write(new byte[] { 1, 2, 3 });
                test.Position = 0;
                (object Object, Action<object, object> Comparer)[] data = new (object Object, Action<object, object> Comparer)[]
                {
                (true, (a,b)=>Assert.AreEqual(a,b)),
                ((sbyte)123, (a,b)=>Assert.AreEqual(a,b,a.GetType().ToString())),
                ((byte)123, (a,b)=>Assert.AreEqual(a,b,a.GetType().ToString())),
                ((short)1234, (a,b)=>Assert.AreEqual(a,b,a.GetType().ToString())),
                ((ushort)short.MaxValue+1, (a,b)=>Assert.AreEqual(a,b,a.GetType().ToString())),
                (123456, (a,b)=>Assert.AreEqual(a,b,a.GetType().ToString())),
                ((uint)int.MaxValue+1, (a,b)=>Assert.AreEqual(a,b,a.GetType().ToString())),
                (12345678901L, (a,b)=>Assert.AreEqual(a,b,a.GetType().ToString())),
                ((ulong)long.MaxValue+1, (a,b)=>Assert.AreEqual(a,b,a.GetType().ToString())),
                (123f, (a,b)=>Assert.AreEqual(a,b,a.GetType().ToString())),
                (123d, (a,b)=>Assert.AreEqual(a,b,a.GetType().ToString())),
                (123m, (a,b)=>Assert.AreEqual(a,b,a.GetType().ToString())),
                (Array.Empty<byte>(), (a,b)=>CompareArray((Array)a,(Array)b)),
                (string.Empty, (a,b)=>Assert.AreEqual(a,b,a.GetType().ToString())),
                (new bool[]{true,false}, (a,b)=>CompareArray((Array)a,(Array)b)),
                (new List<bool>(){true,false}, (a,b)=>CompareList((IList)a,(IList)b)),
                (new Dictionary<int,bool>(){{0,true},{1,false}}, (a,b)=>CompareDict((IDictionary)a,(IDictionary)b)),
                (TestEnum.Zero, (a,b)=>Assert.AreEqual(a,b,a.GetType().ToString())),
                (test, (a,b)=>
                {
                    Assert.IsTrue(b is Stream,a.GetType().ToString());
                    using Stream stream=(Stream)b;
                    stream.Position=0;
                    Assert.AreEqual(test.Length,stream.Length,a.GetType().ToString());
                    byte[] temp=new byte[3];
                    Assert.AreEqual(temp.Length,stream.Read(temp),a.GetType().ToString());
                    Assert.IsTrue(test.ToArray().SequenceEqual(temp),a.GetType().ToString());
                }),
                (new TestObject2(){Value=true}, (a,b)=>
                {
                    TestObject2 toa=(a as TestObject2)!;
                    Assert.IsTrue(b is TestObject2,a.GetType().ToString());
                    TestObject2 tob=(b as TestObject2)!;
                    Assert.AreEqual(toa.Value, tob.Value,a.GetType().ToString());
                }),
                (new TestObject(){Value=true}, (a,b)=>
                {
                    TestObject toa=(a as TestObject)!;
                    Assert.IsTrue(b is TestObject,a.GetType().ToString());
                    TestObject tob=(b as TestObject)!;
                    Assert.AreEqual(toa.Value, tob.Value,a.GetType().ToString());
                })
                };
                object b;
                for (int i = 0; i < data.Length; i++)
                {
                    var info = data[i];
                    Logging.WriteInfo(info.Object.GetType().ToString());
                    ms.WriteAny(info.Object);
                    ms.Position = 0;
                    b = ms.ReadAny();
                    info.Comparer(info.Object, b);
                    ms.SetLength(0);
                    ms.Position = 0;
                }
                ms.WriteAnyNullable(true);
                ms.Position = 0;
                Assert.AreEqual(true, ms.ReadAnyNullable());
                ms.SetLength(0);
                ms.Position = 0;
                ms.WriteAnyNullable(null);
                ms.Position = 0;
                Assert.IsNull(ms.ReadAnyNullable());
            }
            finally
            {
                StreamExtensions.AnyObjectAttributeRequired = true;
            }
        }

        [TestMethod]
        public async Task AnyAsync_Tests()
        {
            StreamExtensions.AnyObjectAttributeRequired = false;
            try
            {
                using MemoryStream ms = new();
                using MemoryStream test = new();
                test.Write(new byte[] { 1, 2, 3 });
                test.Position = 0;
                (object Object, Action<object, object> Comparer)[] data = new (object Object, Action<object, object> Comparer)[]
                {
                (true, (a,b)=>Assert.AreEqual(a,b)),
                ((sbyte)123, (a,b)=>Assert.AreEqual(a,b,a.GetType().ToString())),
                ((byte)123, (a,b)=>Assert.AreEqual(a,b,a.GetType().ToString())),
                ((short)1234, (a,b)=>Assert.AreEqual(a,b,a.GetType().ToString())),
                ((ushort)short.MaxValue+1, (a,b)=>Assert.AreEqual(a,b,a.GetType().ToString())),
                (123456, (a,b)=>Assert.AreEqual(a,b,a.GetType().ToString())),
                ((uint)int.MaxValue+1, (a,b)=>Assert.AreEqual(a,b,a.GetType().ToString())),
                (12345678901L, (a,b)=>Assert.AreEqual(a,b,a.GetType().ToString())),
                ((ulong)long.MaxValue+1, (a,b)=>Assert.AreEqual(a,b,a.GetType().ToString())),
                (123f, (a,b)=>Assert.AreEqual(a,b,a.GetType().ToString())),
                (123d, (a,b)=>Assert.AreEqual(a,b,a.GetType().ToString())),
                (123m, (a,b)=>Assert.AreEqual(a,b,a.GetType().ToString())),
                (Array.Empty<byte>(), (a,b)=>CompareArray((Array)a,(Array)b)),
                (string.Empty, (a,b)=>Assert.AreEqual(a,b,a.GetType().ToString())),
                (new bool[]{true,false}, (a,b)=>CompareArray((Array)a,(Array)b)),
                (new List<bool>(){true,false}, (a,b)=>CompareList((IList)a,(IList)b)),
                (new Dictionary<int,bool>(){{0,true},{1,false}}, (a,b)=>CompareDict((IDictionary)a,(IDictionary)b)),
                (TestEnum.Zero, (a,b)=>Assert.AreEqual(a,b,a.GetType().ToString())),
                (test, (a,b)=>
                {
                    Assert.IsTrue(b is Stream,a.GetType().ToString());
                    using Stream stream=(Stream)b;
                    stream.Position=0;
                    Assert.AreEqual(test.Length,stream.Length,a.GetType().ToString());
                    byte[] temp=new byte[3];
                    Assert.AreEqual(temp.Length,stream.Read(temp),a.GetType().ToString());
                    Assert.IsTrue(test.ToArray().SequenceEqual(temp),a.GetType().ToString());
                }),
                (new TestObject2(){Value=true}, (a,b)=>
                {
                    TestObject2 toa=(a as TestObject2)!;
                    Assert.IsTrue(b is TestObject2,a.GetType().ToString());
                    TestObject2 tob=(b as TestObject2)!;
                    Assert.AreEqual(toa.Value, tob.Value,a.GetType().ToString());
                }),
                (new TestObject(){Value=true}, (a,b)=>
                {
                    TestObject toa=(a as TestObject)!;
                    Assert.IsTrue(b is TestObject,a.GetType().ToString());
                    TestObject tob=(b as TestObject)!;
                    Assert.AreEqual(toa.Value, tob.Value,a.GetType().ToString());
                })
                };
                object b;
                for (int i = 0; i < data.Length; i++)
                {
                    var info = data[i];
                    Logging.WriteInfo(info.Object.GetType().ToString());
                    await ms.WriteAnyAsync(info.Object);
                    ms.Position = 0;
                    b = await ms.ReadAnyAsync();
                    info.Comparer(info.Object, b);
                    ms.SetLength(0);
                    ms.Position = 0;
                }
                await ms.WriteAnyNullableAsync(true);
                ms.Position = 0;
                Assert.AreEqual(true, await ms.ReadAnyNullableAsync());
                ms.SetLength(0);
                ms.Position = 0;
                await ms.WriteAnyNullableAsync(null);
                ms.Position = 0;
                Assert.IsNull(await ms.ReadAnyNullableAsync());
            }
            finally
            {
                StreamExtensions.AnyObjectAttributeRequired = true;
            }
        }
    }
}
