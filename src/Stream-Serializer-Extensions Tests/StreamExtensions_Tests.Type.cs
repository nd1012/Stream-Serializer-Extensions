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
                typeof(StreamExtensions_Tests)
            })
            {
                Logging.WriteInfo($"Type {type} ({new SerializedTypeInfo(type)})");
                ms.SetLength(0);
                ms.Position = 0;
                ms.Write(type);
                ms.Position = 0;
                Assert.AreEqual(type, ms.ReadType());
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
                ms.WriteNullable(type);
                ms.Position = 0;
                Assert.AreEqual(type, ms.ReadTypeNullable());
            }
        }

        [TestMethod]
        public async Task TypeAsync_Tests()
        {
            using MemoryStream ms = new();
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
                typeof(StreamExtensions_Tests)
            })
            {
                Logging.WriteInfo($"Type {type} ({new SerializedTypeInfo(type)})");
                ms.SetLength(0);
                ms.Position = 0;
                await ms.WriteAsync(type);
                ms.Position = 0;
                Assert.AreEqual(type, await ms.ReadTypeAsync());
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
                await ms.WriteNullableAsync(type);
                ms.Position = 0;
                Assert.AreEqual(type, await ms.ReadTypeNullableAsync());
            }
        }
    }
}
