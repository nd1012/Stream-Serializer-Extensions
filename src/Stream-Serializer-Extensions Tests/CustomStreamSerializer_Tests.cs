using wan24.StreamSerializerExtensions;

namespace Stream_Serializer_Extensions_Tests
{
    [TestClass]
    public class CustomStreamSerializer_Tests
    {
        private int AsyncDeserializer;

        [TestMethod]
        public void Custom_Tests()
        {
            int syncSerializer = 0,
                asyncSerializer = 0,
                syncDeserializer = 0;
            AsyncDeserializer = 0;
            StreamSerializer.SyncSerializer[typeof(TestObject)] = (c, v) =>
            {
                syncSerializer++;
                c.Stream.Write(((TestObject)v!).Value, c);
            };
            StreamSerializer.AsyncSerializer[typeof(TestObject)] = async (c, v) =>
            {
                asyncSerializer++;
                await c.Stream.WriteAsync(((TestObject)v!).Value, c);
            };
            StreamSerializer.SyncDeserializer[typeof(TestObject)] = (c, t) =>
            {
                syncDeserializer++;
                return new TestObject() { Value = c.Stream.ReadBool(c) };
            };
            StreamSerializer.AsyncDeserializer[typeof(TestObject)] = DeserializeTestObject;
            try
            {
                using MemoryStream ms = new();
                using SerializerContext sc = new(ms);
                using DeserializerContext dc = new(ms);
                ms.WriteObject(new TestObject() { Value = true }, sc);
                ms.Position = 0;
                Assert.IsTrue(ms.ReadObject<TestObject>(dc).Value);
                Assert.AreEqual(1, syncSerializer);
                Assert.AreEqual(0, asyncSerializer);
                Assert.AreEqual(1, syncDeserializer);
                Assert.AreEqual(0, AsyncDeserializer);
            }
            finally
            {
                StreamSerializer.SyncSerializer.Remove(typeof(TestObject), out _);
                StreamSerializer.AsyncSerializer.Remove(typeof(TestObject), out _);
                StreamSerializer.SyncDeserializer.Remove(typeof(TestObject), out _);
                StreamSerializer.AsyncDeserializer.Remove(typeof(TestObject), out _);
            }
        }

        [TestMethod]
        public async Task CustomAsync_Tests()
        {
            int syncSerializer = 0,
                asyncSerializer = 0,
                syncDeserializer = 0;
            AsyncDeserializer = 0;
            StreamSerializer.SyncSerializer[typeof(TestObject)] = (c, v) =>
            {
                syncSerializer++;
                c.Stream.Write(((TestObject)v!).Value, c);
            };
            StreamSerializer.AsyncSerializer[typeof(TestObject)] = async (c, v) =>
            {
                asyncSerializer++;
                await c.Stream.WriteAsync(((TestObject)v!).Value, c);
            };
            StreamSerializer.SyncDeserializer[typeof(TestObject)] = (c, t) =>
            {
                syncDeserializer++;
                return new TestObject() { Value = c.Stream.ReadBool(c) };
            };
            StreamSerializer.AsyncDeserializer[typeof(TestObject)] = DeserializeTestObject;
            try
            {
                using MemoryStream ms = new();
                using SerializerContext sc = new(ms);
                using DeserializerContext dc = new(ms);
                await ms.WriteObjectAsync(new TestObject() { Value = true },sc);
                ms.Position = 0;
                Assert.IsTrue((await ms.ReadObjectAsync<TestObject>(dc)).Value);
                Assert.AreEqual(0, syncSerializer);
                Assert.AreEqual(1, asyncSerializer);
                Assert.AreEqual(0, syncDeserializer);
                Assert.AreEqual(1, AsyncDeserializer);
            }
            finally
            {
                StreamSerializer.SyncSerializer.Remove(typeof(TestObject), out _);
                StreamSerializer.AsyncSerializer.Remove(typeof(TestObject), out _);
                StreamSerializer.SyncDeserializer.Remove(typeof(TestObject), out _);
                StreamSerializer.AsyncDeserializer.Remove(typeof(TestObject), out _);
            }
        }

        private async Task<TestObject> DeserializeTestObject(IDeserializationContext context, Type type)
        {
            AsyncDeserializer++;
            return new TestObject() { Value = await context.Stream.ReadBoolAsync(context) };
        }
    }
}
