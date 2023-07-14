using System.Security.Cryptography;
using wan24.StreamSerializerExtensions;

namespace Stream_Serializer_Extensions_Tests
{
    [TestClass]
    public class AutoStreamSerializer_Tests
    {
        [TestMethod]
        public void AutoSerializer_Tests()
        {
            using MemoryStream ms = new();
            using SerializerContext sc = new(ms);
            using DeserializerContext dc = new(ms);
            using MemoryStream stream = new();
            stream.Write(RandomNumberGenerator.GetBytes(200000));
            stream.Position = 0;
            using TestObject5 test5 = new()
            {
                AValue = true,
                BValue = true,
                Stream = stream,
                ZValue = true
            };
            using MemoryStream stream_a = new();
            stream_a.Write(RandomNumberGenerator.GetBytes(200000));
            stream_a.Position = 0;
            using TestObject5a test5a = new()
            {
                AValue = true,
                BValue = true,
                Stream = stream_a,
                ZValue = true
            };
            using TestObject5b test5b = new()
            {
                AValue = true,
                BValue = true,
                ZValue = true
            };
            ms.WriteSerialized(test5, sc)
                .WriteSerialized(test5a, sc)
                .WriteSerialized(test5b, sc);
            ms.Position = 0;
            {
                Assert.AreEqual(stream.Length, stream.Position);
                using TestObject5 test5_2 = ms.ReadSerialized<TestObject5>(dc);
                using Stream? stream2 = test5_2.Stream;
                Assert.IsTrue(test5_2.AValue);
                Assert.IsFalse(test5_2.BValue);
                Assert.IsInstanceOfType<FileStream>(stream2);
                Assert.IsFalse(test5_2.ZValue);
                Assert.AreEqual(stream.Length, stream2.Length);
                byte[] buffer = new byte[stream.Length];
                stream2.Position = 0;
                Assert.AreEqual(buffer.Length, stream2.Read(buffer));
                Assert.IsTrue(stream.ToArray().SequenceEqual(buffer));
            }
            {
                Assert.AreEqual(stream_a.Length, stream_a.Position);
                using TestObject5a test5_2 = ms.ReadSerialized<TestObject5a>(dc);
                using Stream? stream2 = test5_2.Stream;
                Assert.IsTrue(test5_2.AValue);
                Assert.IsFalse(test5_2.BValue);
                Assert.IsInstanceOfType<FileStream>(stream2);
                Assert.IsTrue(test5_2.ZValue);
                Assert.AreEqual(stream.Length, stream2.Length);
                byte[] buffer = new byte[stream.Length];
                stream2.Position = 0;
                Assert.AreEqual(buffer.Length, stream2.Read(buffer));
                Assert.IsTrue(stream_a.ToArray().SequenceEqual(buffer));
            }
            {
                using TestObject5b test5_2 = ms.ReadSerialized<TestObject5b>(dc);
                using Stream? stream2 = test5_2.Stream;
                Assert.IsTrue(test5_2.AValue);
                Assert.IsFalse(test5_2.BValue);
                Assert.IsNull(stream2);
                Assert.IsTrue(test5_2.ZValue);
            }
        }

        [TestMethod]
        public async Task AutoSerializerAsync_Tests()
        {
            using MemoryStream ms = new();
            using SerializerContext sc = new(ms);
            using DeserializerContext dc = new(ms);
            using MemoryStream stream = new();
            stream.Write(RandomNumberGenerator.GetBytes(200000));
            stream.Position = 0;
            using TestObject5 test5 = new()
            {
                AValue = true,
                BValue = true,
                Stream = stream,
                ZValue = true
            };
            using MemoryStream stream_a = new();
            stream_a.Write(RandomNumberGenerator.GetBytes(200000));
            stream_a.Position = 0;
            using TestObject5a test5a = new()
            {
                AValue = true,
                BValue = true,
                Stream = stream_a,
                ZValue = true
            };
            using TestObject5b test5b = new()
            {
                AValue = true,
                BValue = true,
                ZValue = true
            };
            await ms.WriteSerializedAsync(test5, sc)
                .WriteSerializedAsync(test5a, sc)
                .WriteSerializedAsync(test5b, sc);
            ms.Position = 0;
            {
                Assert.AreEqual(stream.Length, stream.Position);
                using TestObject5 test5_2 = await ms.ReadSerializedAsync<TestObject5>(dc);
                using Stream? stream2 = test5_2.Stream;
                Assert.IsTrue(test5_2.AValue);
                Assert.IsFalse(test5_2.BValue);
                Assert.IsInstanceOfType<FileStream>(stream2);
                Assert.IsFalse(test5_2.ZValue);
                Assert.AreEqual(stream.Length, stream2.Length);
                byte[] buffer = new byte[stream.Length];
                stream2.Position = 0;
                Assert.AreEqual(buffer.Length, stream2.Read(buffer));
                Assert.IsTrue(stream.ToArray().SequenceEqual(buffer));
            }
            {
                Assert.AreEqual(stream_a.Length, stream_a.Position);
                using TestObject5a test5_2 = await ms.ReadSerializedAsync<TestObject5a>(dc);
                using Stream? stream2 = test5_2.Stream;
                Assert.IsTrue(test5_2.AValue);
                Assert.IsFalse(test5_2.BValue);
                Assert.IsInstanceOfType<FileStream>(stream2);
                Assert.IsTrue(test5_2.ZValue);
                Assert.AreEqual(stream.Length, stream2.Length);
                byte[] buffer = new byte[stream.Length];
                stream2.Position = 0;
                Assert.AreEqual(buffer.Length, stream2.Read(buffer));
                Assert.IsTrue(stream_a.ToArray().SequenceEqual(buffer));
            }
            {
                using TestObject5b test5_2 = await ms.ReadSerializedAsync<TestObject5b>(dc);
                using Stream? stream2 = test5_2.Stream;
                Assert.IsTrue(test5_2.AValue);
                Assert.IsFalse(test5_2.BValue);
                Assert.IsNull(stream2);
                Assert.IsTrue(test5_2.ZValue);
            }
        }

        [TestMethod]
        public void OptOut_Tests()
        {
            using MemoryStream ms = new();
            using SerializerContext sc = new(ms);
            using DeserializerContext dc = new(ms);
            ms.WriteAnyObject(new TestObject3() { Field1 = true, Field2 = true, Field3 = true }, sc)
                .WriteAnyObject(new TestObject3a() { Field1 = true, Field2 = true, Field3 = true }, sc)
                .WriteAnyObject(new TestObject3b() { Field1 = true, Field2 = true, Field3 = true }, sc);
            ms.Position = 0;
            {
                TestObject3 temp = ms.ReadAnyObject<TestObject3>(dc);
                Assert.IsTrue(temp.Field1);
                Assert.IsFalse(temp.Field2);
                Assert.IsTrue(temp.Field3);
            }
            {
                TestObject3a temp = ms.ReadAnyObject<TestObject3a>(dc);
                Assert.IsTrue(temp.Field1);
                Assert.IsTrue(temp.Field2);
                Assert.IsTrue(temp.Field3);
            }
            {
                TestObject3b temp = ms.ReadAnyObject<TestObject3b>(dc);
                Assert.IsTrue(temp.Field1);
                Assert.IsTrue(temp.Field2);
                Assert.IsFalse(temp.Field3);
            }
        }

        [TestMethod]
        public async Task OptOutAsync_Tests()
        {
            using MemoryStream ms = new();
            using SerializerContext sc = new(ms);
            using DeserializerContext dc = new(ms);
            await ms.WriteAnyObjectAsync(new TestObject3() { Field1 = true, Field2 = true, Field3 = true }, sc)
                .WriteAnyObjectAsync(new TestObject3a() { Field1 = true, Field2 = true, Field3 = true }, sc)
                .WriteAnyObjectAsync(new TestObject3b() { Field1 = true, Field2 = true, Field3 = true }, sc);
            ms.Position = 0;
            {
                TestObject3 temp = await ms.ReadAnyObjectAsync<TestObject3>(dc);
                Assert.IsTrue(temp.Field1);
                Assert.IsFalse(temp.Field2);
                Assert.IsTrue(temp.Field3);
            }
            {
                TestObject3a temp = await ms.ReadAnyObjectAsync<TestObject3a>(dc);
                Assert.IsTrue(temp.Field1);
                Assert.IsTrue(temp.Field2);
                Assert.IsTrue(temp.Field3);
            }
            {
                TestObject3b temp = await ms.ReadAnyObjectAsync<TestObject3b>(dc);
                Assert.IsTrue(temp.Field1);
                Assert.IsTrue(temp.Field2);
                Assert.IsFalse(temp.Field3);
            }
        }

        [TestMethod]
        public void OptIn_Tests()
        {
            using MemoryStream ms = new();
            using SerializerContext sc = new(ms);
            using DeserializerContext dc = new(ms);
            ms.WriteAnyObject(new TestObject4() { Field1 = true, Field2 = true, Field3 = true }, sc)
                .WriteAnyObject(new TestObject4a() { Field1 = true, Field2 = true, Field3 = true }, sc)
                .WriteAnyObject(new TestObject4b() { Field1 = true, Field2 = true, Field3 = true }, sc);
            ms.Position = 0;
            {
                TestObject4 temp = ms.ReadAnyObject<TestObject4>(dc);
                Assert.IsTrue(temp.Field1);
                Assert.IsFalse(temp.Field2);
                Assert.IsTrue(temp.Field3);
            }
            {
                TestObject4a temp = ms.ReadAnyObject<TestObject4a>(dc);
                Assert.IsTrue(temp.Field1);
                Assert.IsTrue(temp.Field2);
                Assert.IsTrue(temp.Field3);
            }
            {
                TestObject4b temp = ms.ReadAnyObject<TestObject4b>(dc);
                Assert.IsTrue(temp.Field1);
                Assert.IsTrue(temp.Field2);
                Assert.IsFalse(temp.Field3);
            }
        }

        [TestMethod]
        public async Task OptInAsync_Tests()
        {
            using MemoryStream ms = new();
            using SerializerContext sc = new(ms);
            using DeserializerContext dc = new(ms);
            await ms.WriteAnyObjectAsync(new TestObject4() { Field1 = true, Field2 = true, Field3 = true }, sc)
                .WriteAnyObjectAsync(new TestObject4a() { Field1 = true, Field2 = true, Field3 = true }, sc)
                .WriteAnyObjectAsync(new TestObject4b() { Field1 = true, Field2 = true, Field3 = true }, sc);
            ms.Position = 0;
            {
                TestObject4 temp = await ms.ReadAnyObjectAsync<TestObject4>(dc);
                Assert.IsTrue(temp.Field1);
                Assert.IsFalse(temp.Field2);
                Assert.IsTrue(temp.Field3);
            }
            {
                TestObject4a temp = await ms.ReadAnyObjectAsync<TestObject4a>(dc);
                Assert.IsTrue(temp.Field1);
                Assert.IsTrue(temp.Field2);
                Assert.IsTrue(temp.Field3);
            }
            {
                TestObject4b temp = await ms.ReadAnyObjectAsync<TestObject4b>(dc);
                Assert.IsTrue(temp.Field1);
                Assert.IsTrue(temp.Field2);
                Assert.IsFalse(temp.Field3);
            }
        }
    }
}
