using System.Security.Cryptography;
using wan24.StreamSerializerExtensions;

namespace Stream_Serializer_Extensions_Tests
{
    [TestClass]
    public class Serializer_Tests
    {
        private static readonly Dictionary<string, Type> Types;

        static Serializer_Tests()
        {
            StreamSerializer.SyncSerializer[typeof(TestObject)] = (c, v) => c.Stream.Write(((TestObject)v!).Value, c);
            StreamSerializer.AsyncSerializer[typeof(TestObject)] = (c, v) => c.Stream.WriteAsync(((TestObject)v!).Value, c);
            StreamSerializer.SyncDeserializer[typeof(TestObject)] = (c, t) => new TestObject() { Value = c.Stream.ReadBool(c) };
            StreamSerializer.AsyncDeserializer[typeof(TestObject)] = DeserializeTestObject;
            Types = new()
            {
                {typeof(TestEnum).ToString(), typeof(TestEnum) },
                {typeof(TestObject).ToString(), typeof(TestObject) },
                {typeof(TestObject2).ToString(), typeof(TestObject2) }
            };
            StreamSerializer.OnLoadType += (e) =>
            {
                if (e.Type == null && Types.ContainsKey(e.Name)) e.Type = Types[e.Name];
            };
        }
        private static async Task<TestObject> DeserializeTestObject(IDeserializationContext context, Type type)
            => new TestObject() { Value = await context.Stream.ReadBoolAsync(context) };

        [TestMethod]
        public void Serializer_Test()
        {
            int[] fixedData = new int[] { 0, 1, 2 };
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
            using var ms = new MemoryStream();
            using SerializerContext sc = new(ms);
            using DeserializerContext dc = new(ms);
            ms.Write(true, sc)
                .Write((sbyte)0, sc)
                .Write((byte)0, sc)
                .Write((short)0, sc)
                .Write((ushort)0, sc)
                .Write(0, sc)
                .Write((uint)0, sc)
                .Write((long)0, sc)
                .Write((ulong)0, sc)
                .Write((float)0, sc)
                .Write((double)0, sc)
                .Write((decimal)0, sc)
                .WriteNumber((sbyte)0, sc)
                .WriteNumber(sbyte.MinValue, sc)
                .WriteNumber(sbyte.MaxValue, sc)
                .WriteNumber((byte)0, sc)
                .WriteNumber(byte.MaxValue, sc)
                .WriteNumber((short)0, sc)
                .WriteNumber(short.MinValue, sc)
                .WriteNumber(short.MaxValue, sc)
                .WriteNumber((ushort)0, sc)
                .WriteNumber(ushort.MaxValue, sc)
                .WriteNumber(0, sc)
                .WriteNumber(int.MinValue, sc)
                .WriteNumber(int.MaxValue, sc)
                .WriteNumber((uint)0, sc)
                .WriteNumber(uint.MaxValue, sc)
                .WriteNumber((long)0, sc)
                .WriteNumber(long.MinValue, sc)
                .WriteNumber(long.MaxValue, sc)
                .WriteNumber((ulong)0, sc)
                .WriteNumber(ulong.MaxValue, sc)
                .WriteNumber((float)0, sc)
                .WriteNumber(float.MinValue, sc)
                .WriteNumber(float.MaxValue, sc)
                .WriteNumber((double)0, sc)
                .WriteNumber(double.MinValue, sc)
                .WriteNumber(double.MaxValue, sc)
                .WriteNumber((decimal)0, sc)
                .WriteNumber(decimal.MinValue, sc)
                .WriteNumber(decimal.MaxValue, sc)
                .WriteEnum(TestEnum.Zero, sc)
                .WriteString(true.ToString(), sc)
                .WriteBytes(new byte[] { 0 }, sc)
                .WriteArray(new bool[] { true, false }, sc)
                .WriteList(new List<bool>(new bool[] { true, false }), sc)
                .WriteDict(new Dictionary<string, bool>()
                {
                    {true.ToString(),true },
                    {false.ToString(),false }
                }, sc)
                .WriteObject(new TestObject() { Value = true }, sc)
                .WriteSerialized(new TestObject2() { Value = true }, sc)
                .WriteNullable((bool?)null, sc)
                .WriteNullable(true, sc)
                .WriteNullable((sbyte?)null, sc)
                .WriteNullable((sbyte?)0, sc)
                .WriteNullable((byte?)null, sc)
                .WriteNullable((byte?)0, sc)
                .WriteNullable((short?)null, sc)
                .WriteNullable((short?)0, sc)
                .WriteNullable((ushort?)null, sc)
                .WriteNullable((ushort?)0, sc)
                .WriteNullable((int?)null, sc)
                .WriteNullable((int?)0, sc)
                .WriteNullable((uint?)null, sc)
                .WriteNullable((uint?)0, sc)
                .WriteNullable((long?)null, sc)
                .WriteNullable((long?)0, sc)
                .WriteNullable((ulong?)null, sc)
                .WriteNullable((ulong?)0, sc)
                .WriteNullable((float?)null, sc)
                .WriteNullable((float?)0, sc)
                .WriteNullable((double?)null, sc)
                .WriteNullable((double?)0, sc)
                .WriteNullable((decimal?)null, sc)
                .WriteNullable((decimal?)0, sc)
                .WriteEnumNullable((TestEnum?)null, sc)
                .WriteEnumNullable((TestEnum?)TestEnum.Zero, sc)
                .WriteStringNullable(null, sc)
                .WriteStringNullable((string?)true.ToString(), sc)
                .WriteBytesNullable(null, sc)
                .WriteBytesNullable((byte[]?)new byte[] { 0 }, sc)
                .WriteArrayNullable((bool[]?)null, sc)
                .WriteArrayNullable(new bool[] { true, false }, sc)
                .WriteListNullable((List<bool>?)null, sc)
                .WriteListNullable(new List<bool>(new bool[] { true, false }), sc)
                .WriteDictNullable((Dictionary<string, bool>?)null, sc)
                .WriteDictNullable(new Dictionary<string, bool>()
                {
                    {true.ToString(),true },
                    {false.ToString(),false }
                }, sc)
                .WriteObjectNullable((TestObject?)null, sc)
                .WriteObjectNullable(new TestObject() { Value = true }, sc)
                .WriteSerializedNullable((TestObject2?)null, sc)
                .WriteSerializedNullable(new TestObject2() { Value = true }, sc)
                .WriteAny(true, sc)
                .WriteAny((sbyte)0, sc)
                .WriteAny((byte)0, sc)
                .WriteAny((short)0, sc)
                .WriteAny((ushort)0, sc)
                .WriteAny(0, sc)
                .WriteAny((uint)0, sc)
                .WriteAny((long)0, sc)
                .WriteAny((ulong)0, sc)
                .WriteAny((float)0, sc)
                .WriteAny((double)0, sc)
                .WriteAny((decimal)0, sc)
                .WriteAny(TestEnum.Zero, sc)
                .WriteAny(true.ToString(), sc)
                .WriteAny(new byte[] { 0 }, sc)
                .WriteAny(new bool[] { true, false }, sc)
                .WriteAny(new List<bool>() { true, false }, sc)
                .WriteAny(new Dictionary<string, bool>()
                {
                    {true.ToString(),true },
                    {false.ToString(),false }
                }, sc)
                .WriteAny(new TestObject() { Value = true }, sc)
                .WriteAny(new TestObject2() { Value = true }, sc)
                .WriteAnyNullable(null, sc)
                .WriteAnyObject(new TestObject3() { Field1 = true, Field2 = true, Field3 = true }, sc)
                .WriteAnyObject(new TestObject3a() { Field1 = true, Field2 = true, Field3 = true }, sc)
                .WriteAnyObject(new TestObject3b() { Field1 = true, Field2 = true, Field3 = true }, sc)
                .WriteAnyObject(new TestObject4() { Field1 = true, Field2 = true, Field3 = true }, sc)
                .WriteAnyObject(new TestObject4a() { Field1 = true, Field2 = true, Field3 = true }, sc)
                .WriteAnyObject(new TestObject4b() { Field1 = true, Field2 = true, Field3 = true }, sc)
                .WriteAnyObjectNullable((TestObject3?)null, sc)
                .WriteSerialized(test5, sc)
                .WriteSerialized(test5a, sc)
                .WriteSerialized(test5b, sc)
                .WriteFixedArray(fixedData.AsSpan(), sc)
                .WriteStruct(new TestStruct(true), sc);
            ms.Position = 0;
            Assert.IsTrue(ms.ReadBool(dc));
            Assert.AreEqual((sbyte)0, ms.ReadOneSByte(dc));
            Assert.AreEqual((byte)0, ms.ReadOneByte(dc));
            Assert.AreEqual((short)0, ms.ReadShort(dc));
            Assert.AreEqual((ushort)0, ms.ReadUShort(dc));
            Assert.AreEqual(0, ms.ReadInt(dc));
            Assert.AreEqual((uint)0, ms.ReadUInt(dc));
            Assert.AreEqual(0, ms.ReadLong(dc));
            Assert.AreEqual((ulong)0, ms.ReadULong(dc));
            Assert.AreEqual(0, ms.ReadFloat(dc));
            Assert.AreEqual(0, ms.ReadDouble(dc));
            Assert.AreEqual(0, ms.ReadDecimal(dc));
            Assert.AreEqual((sbyte)0, ms.ReadNumber<sbyte>(dc));
            Assert.AreEqual(sbyte.MinValue, ms.ReadNumber<sbyte>(dc));
            Assert.AreEqual(sbyte.MaxValue, ms.ReadNumber<sbyte>(dc));
            Assert.AreEqual((byte)0, ms.ReadNumber<byte>(dc));
            Assert.AreEqual(byte.MaxValue, ms.ReadNumber<byte>(dc));
            Assert.AreEqual((short)0, ms.ReadNumber<short>(dc));
            Assert.AreEqual(short.MinValue, ms.ReadNumber<short>(dc));
            Assert.AreEqual(short.MaxValue, ms.ReadNumber<short>(dc));
            Assert.AreEqual((ushort)0, ms.ReadNumber<ushort>(dc));
            Assert.AreEqual(ushort.MaxValue, ms.ReadNumber<ushort>(dc));
            Assert.AreEqual(0, ms.ReadNumber<int>(dc));
            Assert.AreEqual(int.MinValue, ms.ReadNumber<int>(dc));
            Assert.AreEqual(int.MaxValue, ms.ReadNumber<int>(dc));
            Assert.AreEqual((uint)0, ms.ReadNumber<uint>(dc));
            Assert.AreEqual(uint.MaxValue, ms.ReadNumber<uint>(dc));
            Assert.AreEqual(0, ms.ReadNumber<long>(dc));
            Assert.AreEqual(long.MinValue, ms.ReadNumber<long>(dc));
            Assert.AreEqual(long.MaxValue, ms.ReadNumber<long>(dc));
            Assert.AreEqual((ulong)0, ms.ReadNumber<ulong>(dc));
            Assert.AreEqual(ulong.MaxValue, ms.ReadNumber<ulong>(dc));
            Assert.AreEqual(0, ms.ReadNumber<float>(dc));
            Assert.AreEqual(float.MinValue, ms.ReadNumber<float>(dc));
            Assert.AreEqual(float.MaxValue, ms.ReadNumber<float>(dc));
            Assert.AreEqual(0, ms.ReadNumber<double>(dc));
            Assert.AreEqual(double.MinValue, ms.ReadNumber<double>(dc));
            Assert.AreEqual(double.MaxValue, ms.ReadNumber<double>(dc));
            Assert.AreEqual(0, ms.ReadNumber<decimal>(dc));
            Assert.AreEqual(decimal.MinValue, ms.ReadNumber<decimal>(dc));
            Assert.AreEqual(decimal.MaxValue, ms.ReadNumber<decimal>(dc));
            Assert.AreEqual(TestEnum.Zero, ms.ReadEnum<TestEnum>(dc));
            Assert.AreEqual(true.ToString(), ms.ReadString(dc));
            Assert.IsTrue(ms.ReadBytes(dc).Value.SequenceEqual(new byte[] { 0 }));
            Assert.IsTrue(ms.ReadArray<bool>(dc).SequenceEqual(new bool[] { true, false }));
            Assert.IsTrue(ms.ReadList<bool>(dc).SequenceEqual(new bool[] { true, false }));
            {
                Dictionary<string, bool> temp = ms.ReadDict<string, bool>(dc);
                Assert.AreEqual(temp[true.ToString()], true);
                Assert.AreEqual(temp[false.ToString()], false);
            }
            Assert.IsTrue(ms.ReadObject<TestObject>(dc).Value);
            Assert.IsTrue(ms.ReadSerialized<TestObject2>(dc).Value);
            Assert.IsNull(ms.ReadBoolNullable(dc));
            Assert.IsTrue(ms.ReadBoolNullable(dc));
            Assert.IsNull(ms.ReadOneSByteNullable(dc));
            Assert.AreEqual((sbyte)0, ms.ReadOneSByteNullable(dc));
            Assert.IsNull(ms.ReadOneByteNullable(dc));
            Assert.AreEqual((byte)0, ms.ReadOneByteNullable(dc));
            Assert.IsNull(ms.ReadShortNullable(dc));
            Assert.AreEqual((short)0, ms.ReadShortNullable(dc));
            Assert.IsNull(ms.ReadUShortNullable(dc));
            Assert.AreEqual((ushort)0, ms.ReadUShortNullable(dc));
            Assert.IsNull(ms.ReadIntNullable(dc));
            Assert.AreEqual(0, ms.ReadIntNullable(dc));
            Assert.IsNull(ms.ReadUIntNullable(dc));
            Assert.AreEqual((uint)0, ms.ReadUIntNullable(dc));
            Assert.IsNull(ms.ReadLongNullable(dc));
            Assert.AreEqual(0, ms.ReadLongNullable(dc));
            Assert.IsNull(ms.ReadULongNullable(dc));
            Assert.AreEqual((ulong)0, ms.ReadULongNullable(dc));
            Assert.IsNull(ms.ReadFloatNullable(dc));
            Assert.AreEqual(0, ms.ReadFloatNullable(dc));
            Assert.IsNull(ms.ReadDoubleNullable(dc));
            Assert.AreEqual(0, ms.ReadDoubleNullable(dc));
            Assert.IsNull(ms.ReadDecimalNullable(dc));
            Assert.AreEqual(0, ms.ReadDecimalNullable(dc));
            Assert.IsNull(ms.ReadEnumNullable<TestEnum>(dc));
            Assert.AreEqual(TestEnum.Zero, ms.ReadEnumNullable<TestEnum>(dc));
            Assert.IsNull(ms.ReadStringNullable(dc));
            Assert.AreEqual(true.ToString(), ms.ReadStringNullable(dc));
            Assert.IsNull(ms.ReadBytesNullable(dc));
            Assert.IsTrue(ms.ReadBytesNullable(dc)?.Value.SequenceEqual(new byte[] { 0 }));
            Assert.IsNull(ms.ReadArrayNullable<bool>(dc));
            Assert.IsTrue(ms.ReadArrayNullable<bool>(dc)?.SequenceEqual(new bool[] { true, false }));
            Assert.IsNull(ms.ReadListNullable<bool>(dc));
            Assert.IsTrue(ms.ReadListNullable<bool>(dc)?.SequenceEqual(new bool[] { true, false }));
            Assert.IsNull(ms.ReadDictNullable<string, bool>(dc));
            Assert.IsNotNull(ms.ReadDictNullable<string, bool>(dc));
            Assert.IsNull(ms.ReadObjectNullable<TestObject>(dc));
            Assert.IsTrue(ms.ReadObjectNullable<TestObject>(dc)?.Value);
            Assert.IsNull(ms.ReadSerializedNullable<TestObject2>(dc));
            Assert.IsTrue(ms.ReadSerializedNullable<TestObject2>(dc)?.Value);
            Assert.AreEqual(true, ms.ReadAny(dc));
            Assert.AreEqual((sbyte)0, ms.ReadAny(dc));
            Assert.AreEqual((byte)0, ms.ReadAny(dc));
            Assert.AreEqual((short)0, ms.ReadAny(dc));
            Assert.AreEqual((ushort)0, ms.ReadAny(dc));
            Assert.AreEqual(0, ms.ReadAny(dc));
            Assert.AreEqual((uint)0, ms.ReadAny(dc));
            Assert.AreEqual((long)0, ms.ReadAny(dc));
            Assert.AreEqual((ulong)0, ms.ReadAny(dc));
            Assert.AreEqual((float)0, ms.ReadAny(dc));
            Assert.AreEqual((double)0, ms.ReadAny(dc));
            Assert.AreEqual((decimal)0, ms.ReadAny(dc));
            Assert.AreEqual(TestEnum.Zero, ms.ReadAny(dc));
            Assert.AreEqual(true.ToString(), ms.ReadAny(dc));
            Assert.IsTrue(((byte[])ms.ReadAny(dc)).SequenceEqual(new byte[] { 0 }));
            Assert.IsTrue(((bool[])ms.ReadAny(dc)).SequenceEqual(new bool[] { true, false }));
            Assert.IsTrue(((List<bool>)ms.ReadAny(dc)).ToArray().SequenceEqual(new bool[] { true, false }));
            {
                Dictionary<string, bool> temp = (Dictionary<string, bool>)ms.ReadAny(dc);
                Assert.AreEqual(temp[true.ToString()], true);
                Assert.AreEqual(temp[false.ToString()], false);
            }
            Assert.IsTrue(ms.ReadAny(dc) is TestObject test1 && test1.Value);
            Assert.IsTrue(ms.ReadAny(dc) is TestObject2 test2 && test2.Value);
            Assert.IsNull(ms.ReadAnyNullable(dc));
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
            Assert.IsNull(ms.ReadAnyObjectNullable<TestObject3>(dc));
            new TestObject2().ToBytes().ToObject<TestObject2>();
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
            {
                int[] fixedData2 = ms.ReadFixedArray(new int[fixedData.Length], dc);
                Assert.IsTrue(fixedData.SequenceEqual(fixedData2));
            }
            {
                TestStruct obj = ms.ReadStruct<TestStruct>(dc);
                Assert.IsTrue(obj.Value);
            }
            Assert.AreEqual(ms.Length, ms.Position);
        }

        [TestMethod]
        public async Task Serializer_TestAsync()
        {
            int[] fixedData = new int[] { 0, 1, 2 };
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
            using var ms = new MemoryStream();
            using SerializerContext sc = new(ms);
            using DeserializerContext dc = new(ms);
            await ms.WriteAsync(true, sc);
            await ms.WriteAsync((sbyte)0, sc);
            await ms.WriteAsync((byte)0, sc);
            await ms.WriteAsync((short)0, sc);
            await ms.WriteAsync((ushort)0, sc);
            await ms.WriteAsync(0, sc);
            await ms.WriteAsync((uint)0, sc);
            await ms.WriteAsync((long)0, sc);
            await ms.WriteAsync((ulong)0, sc);
            await ms.WriteAsync((float)0, sc);
            await ms.WriteAsync((double)0, sc);
            await ms.WriteAsync((decimal)0, sc);
            await ms.WriteNumberAsync((sbyte)0, sc);
            await ms.WriteNumberAsync(sbyte.MinValue, sc);
            await ms.WriteNumberAsync(sbyte.MaxValue, sc);
            await ms.WriteNumberAsync((byte)0, sc);
            await ms.WriteNumberAsync(byte.MaxValue, sc);
            await ms.WriteNumberAsync((short)0, sc);
            await ms.WriteNumberAsync(short.MinValue, sc);
            await ms.WriteNumberAsync(short.MaxValue, sc);
            await ms.WriteNumberAsync((ushort)0, sc);
            await ms.WriteNumberAsync(ushort.MaxValue, sc);
            await ms.WriteNumberAsync(0, sc);
            await ms.WriteNumberAsync(int.MinValue, sc);
            await ms.WriteNumberAsync(int.MaxValue, sc);
            await ms.WriteNumberAsync((uint)0, sc);
            await ms.WriteNumberAsync(uint.MaxValue, sc);
            await ms.WriteNumberAsync((long)0, sc);
            await ms.WriteNumberAsync(long.MinValue, sc);
            await ms.WriteNumberAsync(long.MaxValue, sc);
            await ms.WriteNumberAsync((ulong)0, sc);
            await ms.WriteNumberAsync(ulong.MaxValue, sc);
            await ms.WriteNumberAsync((float)0, sc);
            await ms.WriteNumberAsync(float.MinValue, sc);
            await ms.WriteNumberAsync(float.MaxValue, sc);
            await ms.WriteNumberAsync((double)0, sc);
            await ms.WriteNumberAsync(double.MinValue, sc);
            await ms.WriteNumberAsync(double.MaxValue, sc);
            await ms.WriteNumberAsync((decimal)0, sc);
            await ms.WriteNumberAsync(decimal.MinValue, sc);
            await ms.WriteNumberAsync(decimal.MaxValue, sc);
            await ms.WriteEnumAsync(TestEnum.Zero, sc);
            await ms.WriteStringAsync(true.ToString(), sc);
            await ms.WriteBytesAsync(new byte[] { 0 }, sc);
            await ms.WriteArrayAsync(new bool[] { true, false }, sc);
            await ms.WriteListAsync(new List<bool>(new bool[] { true, false }), sc);
            await ms.WriteDictAsync(new Dictionary<string, bool>()
                {
                    {true.ToString(),true },
                    {false.ToString(),false }
                }, sc);
            await ms.WriteObjectAsync(new TestObject() { Value = true }, sc);
            await ms.WriteSerializedAsync(new TestObject2() { Value = true }, sc);
            await ms.WriteNullableAsync((bool?)null, sc);
            await ms.WriteNullableAsync(true, sc);
            await ms.WriteNullableAsync((sbyte?)null, sc);
            await ms.WriteNullableAsync((sbyte?)0, sc);
            await ms.WriteNullableAsync((byte?)null, sc);
            await ms.WriteNullableAsync((byte?)0, sc);
            await ms.WriteNullableAsync((short?)null, sc);
            await ms.WriteNullableAsync((short?)0, sc);
            await ms.WriteNullableAsync((ushort?)null, sc);
            await ms.WriteNullableAsync((ushort?)0, sc);
            await ms.WriteNullableAsync((int?)null, sc);
            await ms.WriteNullableAsync((int?)0, sc);
            await ms.WriteNullableAsync((uint?)null, sc);
            await ms.WriteNullableAsync((uint?)0, sc);
            await ms.WriteNullableAsync((long?)null, sc);
            await ms.WriteNullableAsync((long?)0, sc);
            await ms.WriteNullableAsync((ulong?)null, sc);
            await ms.WriteNullableAsync((ulong?)0, sc);
            await ms.WriteNullableAsync((float?)null, sc);
            await ms.WriteNullableAsync((float?)0, sc);
            await ms.WriteNullableAsync((double?)null, sc);
            await ms.WriteNullableAsync((double?)0, sc);
            await ms.WriteNullableAsync((decimal?)null, sc);
            await ms.WriteNullableAsync((decimal?)0, sc);
            await ms.WriteEnumNullableAsync((TestEnum?)null, sc);
            await ms.WriteEnumNullableAsync((TestEnum?)TestEnum.Zero, sc);
            await ms.WriteStringNullableAsync(null, sc);
            await ms.WriteStringNullableAsync((string?)true.ToString(), sc);
            await ms.WriteBytesNullableAsync(null, sc);
            await ms.WriteBytesNullableAsync((byte[]?)new byte[] { 0 }, sc);
            await ms.WriteArrayNullableAsync((bool[]?)null, sc);
            await ms.WriteArrayNullableAsync(new bool[] { true, false }, sc);
            await ms.WriteListNullableAsync((List<bool>?)null, sc);
            await ms.WriteListNullableAsync(new List<bool>(new bool[] { true, false }), sc);
            await ms.WriteDictNullableAsync((Dictionary<string, bool>?)null, sc);
            await ms.WriteDictNullableAsync(new Dictionary<string, bool>()
            {
                {true.ToString(),true },
                {false.ToString(),false }
            }, sc);
            await ms.WriteObjectNullableAsync((TestObject?)null, sc);
            await ms.WriteObjectNullableAsync(new TestObject() { Value = true }, sc);
            await ms.WriteSerializedNullableAsync((TestObject2?)null, sc);
            await ms.WriteSerializedNullableAsync(new TestObject2() { Value = true }, sc);
            await ms.WriteAnyAsync(true, sc);
            await ms.WriteAnyAsync((sbyte)0, sc);
            await ms.WriteAnyAsync((byte)0, sc);
            await ms.WriteAnyAsync((short)0, sc);
            await ms.WriteAnyAsync((ushort)0, sc);
            await ms.WriteAnyAsync(0, sc);
            await ms.WriteAnyAsync((uint)0, sc);
            await ms.WriteAnyAsync((long)0, sc);
            await ms.WriteAnyAsync((ulong)0, sc);
            await ms.WriteAnyAsync((float)0, sc);
            await ms.WriteAnyAsync((double)0, sc);
            await ms.WriteAnyAsync((decimal)0, sc);
            await ms.WriteAnyAsync(TestEnum.Zero, sc);
            await ms.WriteAnyAsync(true.ToString(), sc);
            await ms.WriteAnyAsync(new byte[] { 0 }, sc);
            await ms.WriteAnyAsync(new bool[] { true, false }, sc);
            await ms.WriteAnyAsync(new List<bool>() { true, false }, sc);
            await ms.WriteAnyAsync(new Dictionary<string, bool>()
            {
                {true.ToString(),true },
                {false.ToString(),false }
            }, sc);
            await ms.WriteAnyAsync(new TestObject() { Value = true }, sc);
            await ms.WriteAnyAsync(new TestObject2() { Value = true }, sc);
            await ms.WriteAnyNullableAsync(null, sc);
            await ms.WriteAnyObjectAsync(new TestObject3() { Field1 = true, Field2 = true, Field3 = true }, sc);
            await ms.WriteAnyObjectAsync(new TestObject3a() { Field1 = true, Field2 = true, Field3 = true }, sc);
            await ms.WriteAnyObjectAsync(new TestObject3b() { Field1 = true, Field2 = true, Field3 = true }, sc);
            await ms.WriteAnyObjectAsync(new TestObject4() { Field1 = true, Field2 = true, Field3 = true }, sc);
            await ms.WriteAnyObjectAsync(new TestObject4a() { Field1 = true, Field2 = true, Field3 = true }, sc);
            await ms.WriteAnyObjectAsync(new TestObject4b() { Field1 = true, Field2 = true, Field3 = true }, sc);
            await ms.WriteAnyObjectNullableAsync((TestObject3?)null, sc);
            await ms.WriteSerializedAsync(test5, sc);
            await ms.WriteSerializedAsync(test5a, sc);
            await ms.WriteSerializedAsync(test5b, sc);
            await ms.WriteFixedArrayAsync(fixedData.AsMemory(), sc);
            await ms.WriteStructAsync(new TestStruct(true), sc);
            ms.Position = 0;
            Assert.IsTrue(await ms.ReadBoolAsync(dc));
            Assert.AreEqual((sbyte)0, await ms.ReadOneSByteAsync(dc));
            Assert.AreEqual((byte)0, await ms.ReadOneByteAsync(dc));
            Assert.AreEqual((short)0, await ms.ReadShortAsync(dc));
            Assert.AreEqual((ushort)0, await ms.ReadUShortAsync(dc));
            Assert.AreEqual(0, await ms.ReadIntAsync(dc));
            Assert.AreEqual((uint)0, await ms.ReadUIntAsync(dc));
            Assert.AreEqual(0, await ms.ReadLongAsync(dc));
            Assert.AreEqual((ulong)0, await ms.ReadULongAsync(dc));
            Assert.AreEqual(0, await ms.ReadFloatAsync(dc));
            Assert.AreEqual(0, await ms.ReadDoubleAsync(dc));
            Assert.AreEqual(0, await ms.ReadDecimalAsync(dc));
            Assert.AreEqual((sbyte)0, await ms.ReadNumberAsync<sbyte>(dc));
            Assert.AreEqual(sbyte.MinValue, await ms.ReadNumberAsync<sbyte>(dc));
            Assert.AreEqual(sbyte.MaxValue, await ms.ReadNumberAsync<sbyte>(dc));
            Assert.AreEqual((byte)0, await ms.ReadNumberAsync<byte>(dc));
            Assert.AreEqual(byte.MaxValue, await ms.ReadNumberAsync<byte>(dc));
            Assert.AreEqual((short)0, await ms.ReadNumberAsync<short>(dc));
            Assert.AreEqual(short.MinValue, await ms.ReadNumberAsync<short>(dc));
            Assert.AreEqual(short.MaxValue, await ms.ReadNumberAsync<short>(dc));
            Assert.AreEqual((ushort)0, await ms.ReadNumberAsync<ushort>(dc));
            Assert.AreEqual(ushort.MaxValue, await ms.ReadNumberAsync<ushort>(dc));
            Assert.AreEqual(0, await ms.ReadNumberAsync<int>(dc));
            Assert.AreEqual(int.MinValue, await ms.ReadNumberAsync<int>(dc));
            Assert.AreEqual(int.MaxValue, await ms.ReadNumberAsync<int>(dc));
            Assert.AreEqual((uint)0, await ms.ReadNumberAsync<uint>(dc));
            Assert.AreEqual(uint.MaxValue, await ms.ReadNumberAsync<uint>(dc));
            Assert.AreEqual(0, await ms.ReadNumberAsync<long>(dc));
            Assert.AreEqual(long.MinValue, await ms.ReadNumberAsync<long>(dc));
            Assert.AreEqual(long.MaxValue, await ms.ReadNumberAsync<long>(dc));
            Assert.AreEqual((ulong)0, await ms.ReadNumberAsync<ulong>(dc));
            Assert.AreEqual(ulong.MaxValue, await ms.ReadNumberAsync<ulong>(dc));
            Assert.AreEqual(0, await ms.ReadNumberAsync<float>(dc));
            Assert.AreEqual(float.MinValue, await ms.ReadNumberAsync<float>(dc));
            Assert.AreEqual(float.MaxValue, await ms.ReadNumberAsync<float>(dc));
            Assert.AreEqual(0, await ms.ReadNumberAsync<double>(dc));
            Assert.AreEqual(double.MinValue, await ms.ReadNumberAsync<double>(dc));
            Assert.AreEqual(double.MaxValue, await ms.ReadNumberAsync<double>(dc));
            Assert.AreEqual(0, await ms.ReadNumberAsync<decimal>(dc));
            Assert.AreEqual(decimal.MinValue, await ms.ReadNumberAsync<decimal>(dc));
            Assert.AreEqual(decimal.MaxValue, await ms.ReadNumberAsync<decimal>(dc));
            Assert.AreEqual(TestEnum.Zero, await ms.ReadEnumAsync<TestEnum>(dc));
            Assert.AreEqual(true.ToString(), await ms.ReadStringAsync(dc));
            Assert.IsTrue((await ms.ReadBytesAsync(dc)).Value.SequenceEqual(new byte[] { 0 }));
            Assert.IsTrue((await ms.ReadArrayAsync<bool>(dc)).SequenceEqual(new bool[] { true, false }));
            Assert.IsTrue((await ms.ReadListAsync<bool>(dc)).SequenceEqual(new bool[] { true, false }));
            {
                Dictionary<string, bool> temp = await ms.ReadDictAsync<string, bool>(dc);
                Assert.AreEqual(temp[true.ToString()], true);
                Assert.AreEqual(temp[false.ToString()], false);
            }
            Assert.IsTrue((await ms.ReadObjectAsync<TestObject>(dc)).Value);
            Assert.IsTrue((await ms.ReadObjectAsync<TestObject2>(dc)).Value);
            Assert.IsNull(await ms.ReadBoolNullableAsync(dc));
            Assert.IsTrue(await ms.ReadBoolNullableAsync(dc));
            Assert.IsNull(await ms.ReadOneSByteNullableAsync(dc));
            Assert.AreEqual((sbyte)0, await ms.ReadOneSByteNullableAsync(dc));
            Assert.IsNull(await ms.ReadOneByteNullableAsync(dc));
            Assert.AreEqual((byte)0, await ms.ReadOneByteNullableAsync(dc));
            Assert.IsNull(await ms.ReadShortNullableAsync(dc));
            Assert.AreEqual((short)0, await ms.ReadShortNullableAsync(dc));
            Assert.IsNull(await ms.ReadUShortNullableAsync(dc));
            Assert.AreEqual((ushort)0, await ms.ReadUShortNullableAsync(dc));
            Assert.IsNull(await ms.ReadIntNullableAsync(dc));
            Assert.AreEqual(0, await ms.ReadIntNullableAsync(dc));
            Assert.IsNull(await ms.ReadUIntNullableAsync(dc));
            Assert.AreEqual((uint)0, await ms.ReadUIntNullableAsync(dc));
            Assert.IsNull(await ms.ReadLongNullableAsync(dc));
            Assert.AreEqual(0, await ms.ReadLongNullableAsync(dc));
            Assert.IsNull(await ms.ReadULongNullableAsync(dc));
            Assert.AreEqual((ulong)0, await ms.ReadULongNullableAsync(dc));
            Assert.IsNull(await ms.ReadFloatNullableAsync(dc));
            Assert.AreEqual(0, await ms.ReadFloatNullableAsync(dc));
            Assert.IsNull(await ms.ReadDoubleNullableAsync(dc));
            Assert.AreEqual(0, await ms.ReadDoubleNullableAsync(dc));
            Assert.IsNull(await ms.ReadDecimalNullableAsync(dc));
            Assert.AreEqual(0, await ms.ReadDecimalNullableAsync(dc));
            Assert.IsNull(await ms.ReadEnumNullableAsync<TestEnum>(dc));
            Assert.AreEqual(TestEnum.Zero, await ms.ReadEnumNullableAsync<TestEnum>(dc));
            Assert.IsNull(await ms.ReadStringNullableAsync(dc));
            Assert.AreEqual(true.ToString(), await ms.ReadStringNullableAsync(dc));
            Assert.IsNull(await ms.ReadBytesNullableAsync(dc));
            Assert.IsTrue((await ms.ReadBytesNullableAsync(dc))?.Value.SequenceEqual(new byte[] { 0 }));
            Assert.IsNull(await ms.ReadArrayNullableAsync<bool>(dc));
            Assert.IsTrue((await ms.ReadArrayNullableAsync<bool>(dc))?.SequenceEqual(new bool[] { true, false }));
            Assert.IsNull(await ms.ReadListNullableAsync<bool>(dc));
            Assert.IsTrue((await ms.ReadListNullableAsync<bool>(dc))?.SequenceEqual(new bool[] { true, false }));
            Assert.IsNull(await ms.ReadDictNullableAsync<string, bool>(dc));
            Assert.IsNotNull(await ms.ReadDictNullableAsync<string, bool>(dc));
            Assert.IsNull(await ms.ReadObjectNullableAsync<TestObject>(dc));
            Assert.IsTrue((await ms.ReadObjectNullableAsync<TestObject>(dc))?.Value);
            Assert.IsNull(await ms.ReadSerializedNullableAsync<TestObject2>(dc));
            Assert.IsTrue((await ms.ReadSerializedNullableAsync<TestObject2>(dc))?.Value);
            Assert.AreEqual(true, await ms.ReadAnyAsync(dc));
            Assert.AreEqual((sbyte)0, await ms.ReadAnyAsync(dc));
            Assert.AreEqual((byte)0, await ms.ReadAnyAsync(dc));
            Assert.AreEqual((short)0, await ms.ReadAnyAsync(dc));
            Assert.AreEqual((ushort)0, await ms.ReadAnyAsync(dc));
            Assert.AreEqual(0, await ms.ReadAnyAsync(dc));
            Assert.AreEqual((uint)0, await ms.ReadAnyAsync(dc));
            Assert.AreEqual((long)0, await ms.ReadAnyAsync(dc));
            Assert.AreEqual((ulong)0, await ms.ReadAnyAsync(dc));
            Assert.AreEqual((float)0, await ms.ReadAnyAsync(dc));
            Assert.AreEqual((double)0, await ms.ReadAnyAsync(dc));
            Assert.AreEqual((decimal)0, await ms.ReadAnyAsync(dc));
            Assert.AreEqual(TestEnum.Zero, await ms.ReadAnyAsync(dc));
            Assert.AreEqual(true.ToString(), await ms.ReadAnyAsync(dc));
            Assert.IsTrue(((byte[])await ms.ReadAnyAsync(dc)).SequenceEqual(new byte[] { 0 }));
            Assert.IsTrue(((bool[])await ms.ReadAnyAsync(dc)).SequenceEqual(new bool[] { true, false }));
            Assert.IsTrue(((List<bool>)await ms.ReadAnyAsync(dc)).ToArray().SequenceEqual(new bool[] { true, false }));
            {
                Dictionary<string, bool> temp = (Dictionary<string, bool>)await ms.ReadAnyAsync(dc);
                Assert.AreEqual(temp[true.ToString()], true);
                Assert.AreEqual(temp[false.ToString()], false);
            }
            Assert.IsTrue(await ms.ReadAnyAsync(dc) is TestObject test1 && test1.Value);
            Assert.IsTrue(await ms.ReadAnyAsync(dc) is TestObject2 test2 && test2.Value);
            Assert.IsNull(await ms.ReadAnyNullableAsync(dc));
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
            Assert.IsNull(ms.ReadAnyObjectNullable<TestObject3>(dc));
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
                Assert.AreEqual(buffer.Length, await stream2.ReadAsync(buffer));
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
                Assert.AreEqual(buffer.Length, await stream2.ReadAsync(buffer));
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
            {
                int[] fixedData2 = await ms.ReadFixedArrayAsync(new int[fixedData.Length], dc);
                Assert.IsTrue(fixedData.SequenceEqual(fixedData2));
            }
            {
                TestStruct obj = await ms.ReadStructAsync<TestStruct>(dc);
                Assert.IsTrue(obj.Value);
            }
            Assert.AreEqual(ms.Length, ms.Position);
        }
    }
}
