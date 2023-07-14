using wan24.Core;
using wan24.StreamSerializerExtensions;

namespace Stream_Serializer_Extensions_Tests
{
    public partial class StreamExtensions_Tests
    {
        [TestMethod]
        public void Type_Tests()
        {
            using MemoryStream ms = new();
            using SerializerContext sc = new(ms);
            using DeserializerContext dc = new(ms);
            foreach (Type type in new Type[]
            {
                typeof(bool),
                typeof(bool[]),
                typeof(bool[,]),
                typeof(List<bool>),
                typeof(Dictionary<bool,bool>),
                typeof(List<>),
                typeof(Dictionary<,>),
                typeof(IDisposable),
                typeof(Stream),
                typeof(StreamExtensions_Tests),
                typeof(Type)
            })
            {
                Logging.WriteInfo($"Type {type} ({SerializedTypeInfo.From(type)})");
                ms.SetLength(0);
                ms.Position = 0;
                ms.Write(type, sc);
                Logging.WriteInfo($"\tSerialized to {ms.Length} bytes");
                ms.Position = 0;
                Assert.AreEqual(type, ms.ReadType(dc));
            }
            foreach (Type? type in new Type?[]
            {
                typeof(bool),
                null
            })
            {
                Logging.WriteInfo($"Type {type?.ToString() ?? "NULL"}");
                ms.SetLength(0);
                ms.Position = 0;
                ms.WriteNullable(type, sc);
                Logging.WriteInfo($"\tSerialized to {ms.Length} bytes");
                ms.Position = 0;
                Assert.AreEqual(type, ms.ReadTypeNullable(dc));
            }
        }

        [TestMethod]
        public async Task TypeAsync_Tests()
        {
            using MemoryStream ms = new();
            using SerializerContext sc = new(ms);
            using DeserializerContext dc = new(ms);
            foreach (Type type in new Type[]
            {
                typeof(bool),
                typeof(bool[]),
                typeof(bool[,]),
                typeof(List<bool>),
                typeof(Dictionary<bool,bool>),
                typeof(List<>),
                typeof(Dictionary<,>),
                typeof(IDisposable),
                typeof(Stream),
                typeof(StreamExtensions_Tests),
                typeof(Type)
            })
            {
                Logging.WriteInfo($"Type {type} ({SerializedTypeInfo.From(type)})");
                ms.SetLength(0);
                ms.Position = 0;
                await ms.WriteAsync(type, sc);
                Logging.WriteInfo($"\tSerialized to {ms.Length} bytes");
                ms.Position = 0;
                Assert.AreEqual(type, await ms.ReadTypeAsync(dc));
            }
            foreach (Type? type in new Type?[]
            {
                typeof(bool),
                null
            })
            {
                Logging.WriteInfo($"Type {type?.ToString() ?? "NULL"}");
                ms.SetLength(0);
                ms.Position = 0;
                await ms.WriteNullableAsync(type, sc);
                Logging.WriteInfo($"\tSerialized to {ms.Length} bytes");
                ms.Position = 0;
                Assert.AreEqual(type, await ms.ReadTypeNullableAsync(dc));
            }
        }
    }
}
