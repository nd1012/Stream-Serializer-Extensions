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
            StreamSerializer.SyncSerializer[typeof(TestObject)] = (s, v) =>
            {
                syncSerializer++;
                s.Write(((TestObject)v!).Value);
            };
            StreamSerializer.AsyncSerializer[typeof(TestObject)] = async (s, v, ct) =>
            {
                asyncSerializer++;
                await s.WriteAsync(((TestObject)v!).Value, ct);
            };
            StreamSerializer.SyncDeserializer[typeof(TestObject)] = (s, t, v, o) =>
            {
                syncDeserializer++;
                return new TestObject() { Value = s.ReadBool() };
            };
            StreamSerializer.AsyncDeserializer[typeof(TestObject)] = DeserializeTestObject;
            try
            {
                using MemoryStream ms = new();
                ms.WriteObject(new TestObject() { Value = true });
                ms.Position = 0;
                Assert.IsTrue(ms.ReadObject<TestObject>().Value);
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
            StreamSerializer.SyncSerializer[typeof(TestObject)] = (s, v) =>
            {
                syncSerializer++;
                s.Write(((TestObject)v!).Value);
            };
            StreamSerializer.AsyncSerializer[typeof(TestObject)] = async (s, v, ct) =>
            {
                asyncSerializer++;
                await s.WriteAsync(((TestObject)v!).Value, ct);
            };
            StreamSerializer.SyncDeserializer[typeof(TestObject)] = (s, t, v, o) =>
            {
                syncDeserializer++;
                return new TestObject() { Value = s.ReadBool() };
            };
            StreamSerializer.AsyncDeserializer[typeof(TestObject)] = DeserializeTestObject;
            try
            {
                using MemoryStream ms = new();
                await ms.WriteObjectAsync(new TestObject() { Value = true });
                ms.Position = 0;
                Assert.IsTrue((await ms.ReadObjectAsync<TestObject>()).Value);
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

        private async Task<TestObject> DeserializeTestObject(Stream stream, Type type, int version, ISerializerOptions? options, CancellationToken cancellationToken)
        {
            AsyncDeserializer++;
            return new TestObject() { Value = await stream.ReadBoolAsync(version, cancellationToken: cancellationToken) };
        }
    }
}
