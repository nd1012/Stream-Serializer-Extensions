using wan24.StreamSerializerExtensions;

namespace Stream_Serializer_Extensions_Tests
{
    public partial class StreamExtensions_Tests
    {
        [TestMethod]
        public void Number_Tests()
        {
            using MemoryStream ms = new();
            using SerializerContext sc = new(ms);
            using DeserializerContext dc = new(ms);
            (object Number, long Length, Type Type, NumberTypes NumberType)[] data = new (object Number, long Length, Type Type, NumberTypes NumberType)[]
            {
                (0, 1, typeof(int), NumberTypes.Zero),
                (123, 2, typeof(int), NumberTypes.Byte|NumberTypes.Unsigned),
                (1234, 3, typeof(int), NumberTypes.Short),
                (123456, 5, typeof(int), NumberTypes.Int),
                (12345678901, 9, typeof(long), NumberTypes.Long),
                (1f, 5, typeof(float), NumberTypes.Float),
                (1d, 5, typeof(float), NumberTypes.Float),
                (1234567890123456789012345678901234567890d, 9, typeof(double), NumberTypes.Double),
                (1m, 17, typeof(decimal), NumberTypes.Decimal)
            };
            foreach (var info in data)
            {
                Assert.AreEqual(info.NumberType, SerializerHelper.GetNumberAndType(info.Number).Type, info.Number.ToString());
                ms.WriteNumber(info.Number, sc);
                Assert.AreEqual(info.Length, ms.Length, info.Number.ToString());
                ms.Position = 0;
                Assert.AreEqual(Convert.ChangeType(info.Number, info.Type), ms.ReadNumber(info.Type, dc), $"{info.Number.GetType()} {info.Number}");
                ms.SetLength(0);
                ms.Position = 0;
            }
            ms.WriteNumberNullable(123, sc);
            Assert.AreEqual(2L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(123, ms.ReadNumberNullable<int>(dc));
            ms.SetLength(0);
            ms.Position = 0;
            ms.WriteNumberNullable((int?)null, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.IsNull(ms.ReadNumberNullable<int>(dc));
        }

        [TestMethod]
        public async Task NumberAsync_Tests()
        {
            using MemoryStream ms = new();
            using SerializerContext sc = new(ms);
            using DeserializerContext dc = new(ms);
            (object Number, long Length, Type Type, NumberTypes NumberType)[] data = new (object Number, long Length, Type Type, NumberTypes NumberType)[]
            {
                (0, 1, typeof(int), NumberTypes.Zero),
                (123, 2, typeof(int), NumberTypes.Byte|NumberTypes.Unsigned),
                (1234, 3, typeof(int), NumberTypes.Short),
                (123456, 5, typeof(int), NumberTypes.Int),
                (12345678901, 9, typeof(long), NumberTypes.Long),
                (1f, 5, typeof(float), NumberTypes.Float),
                (1d, 5, typeof(float), NumberTypes.Float),
                (1234567890123456789012345678901234567890d, 9, typeof(double), NumberTypes.Double),
                (1m, 17, typeof(decimal), NumberTypes.Decimal)
            };
            foreach (var info in data)
            {
                Assert.AreEqual(info.NumberType, SerializerHelper.GetNumberAndType(info.Number).Type, info.Number.ToString());
                await ms.WriteNumberAsync(info.Number, sc);
                Assert.AreEqual(info.Length, ms.Length, info.Number.ToString());
                ms.Position = 0;
                Assert.AreEqual(Convert.ChangeType(info.Number, info.Type), await ms.ReadNumberAsync(info.Type, dc), $"{info.Number.GetType()} {info.Number}");
                ms.SetLength(0);
                ms.Position = 0;
            }
            await ms.WriteNumberNullableAsync(123, sc);
            Assert.AreEqual(2L, ms.Length);
            ms.Position = 0;
            Assert.AreEqual(123, await ms.ReadNumberNullableAsync<int>(dc));
            ms.SetLength(0);
            ms.Position = 0;
            await ms.WriteNumberNullableAsync((int?)null, sc);
            Assert.AreEqual(1L, ms.Length);
            ms.Position = 0;
            Assert.IsNull(await ms.ReadNumberNullableAsync<int>(dc));
        }
    }
}
