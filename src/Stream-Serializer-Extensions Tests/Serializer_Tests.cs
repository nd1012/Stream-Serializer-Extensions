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
            StreamSerializer.SyncSerializer[typeof(TestObject)] = (s, v) => s.Write(((TestObject)v!).Value);
            StreamSerializer.AsyncSerializer[typeof(TestObject)] = (s, v, ct) => s.WriteAsync(((TestObject)v!).Value, ct);
            StreamSerializer.SyncDeserializer[typeof(TestObject)] = (s, t, v, o) => new TestObject() { Value = s.ReadBool() };
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
        private static async Task<TestObject> DeserializeTestObject(Stream stream, Type type, int version, ISerializerOptions? options, CancellationToken cancellationToken)
            => new TestObject() { Value = await stream.ReadBoolAsync(version, cancellationToken: cancellationToken) };

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
            ms.Write(true)
                .Write((sbyte)0)
                .Write((byte)0)
                .Write((short)0)
                .Write((ushort)0)
                .Write(0)
                .Write((uint)0)
                .Write((long)0)
                .Write((ulong)0)
                .Write((float)0)
                .Write((double)0)
                .Write((decimal)0)
                .WriteNumber((sbyte)0)
                .WriteNumber(sbyte.MinValue)
                .WriteNumber(sbyte.MaxValue)
                .WriteNumber((byte)0)
                .WriteNumber(byte.MaxValue)
                .WriteNumber((short)0)
                .WriteNumber(short.MinValue)
                .WriteNumber(short.MaxValue)
                .WriteNumber((ushort)0)
                .WriteNumber(ushort.MaxValue)
                .WriteNumber(0)
                .WriteNumber(int.MinValue)
                .WriteNumber(int.MaxValue)
                .WriteNumber((uint)0)
                .WriteNumber(uint.MaxValue)
                .WriteNumber((long)0)
                .WriteNumber(long.MinValue)
                .WriteNumber(long.MaxValue)
                .WriteNumber((ulong)0)
                .WriteNumber(ulong.MaxValue)
                .WriteNumber((float)0)
                .WriteNumber(float.MinValue)
                .WriteNumber(float.MaxValue)
                .WriteNumber((double)0)
                .WriteNumber(double.MinValue)
                .WriteNumber(double.MaxValue)
                .WriteNumber((decimal)0)
                .WriteNumber(decimal.MinValue)
                .WriteNumber(decimal.MaxValue)
                .WriteEnum(TestEnum.Zero)
                .WriteString(true.ToString())
                .WriteBytes(new byte[] { 0 })
                .WriteArray(new bool[] { true, false })
                .WriteList(new List<bool>(new bool[] { true, false }))
                .WriteDict(new Dictionary<string, bool>()
                {
                    {true.ToString(),true },
                    {false.ToString(),false }
                })
                .WriteObject(new TestObject() { Value = true })
                .WriteSerialized(new TestObject2() { Value = true })
                .WriteNullable((bool?)null)
                .WriteNullable(true)
                .WriteNullable((sbyte?)null)
                .WriteNullable((sbyte?)0)
                .WriteNullable((byte?)null)
                .WriteNullable((byte?)0)
                .WriteNullable((short?)null)
                .WriteNullable((short?)0)
                .WriteNullable((ushort?)null)
                .WriteNullable((ushort?)0)
                .WriteNullable((int?)null)
                .WriteNullable((int?)0)
                .WriteNullable((uint?)null)
                .WriteNullable((uint?)0)
                .WriteNullable((long?)null)
                .WriteNullable((long?)0)
                .WriteNullable((ulong?)null)
                .WriteNullable((ulong?)0)
                .WriteNullable((float?)null)
                .WriteNullable((float?)0)
                .WriteNullable((double?)null)
                .WriteNullable((double?)0)
                .WriteNullable((decimal?)null)
                .WriteNullable((decimal?)0)
                .WriteEnumNullable((TestEnum?)null)
                .WriteEnumNullable((TestEnum?)TestEnum.Zero)
                .WriteStringNullable(null)
                .WriteStringNullable((string?)true.ToString())
                .WriteBytesNullable(null)
                .WriteBytesNullable((byte[]?)new byte[] { 0 })
                .WriteArrayNullable((bool[]?)null)
                .WriteArrayNullable(new bool[] { true, false })
                .WriteListNullable((List<bool>?)null)
                .WriteListNullable(new List<bool>(new bool[] { true, false }))
                .WriteDictNullable((Dictionary<string, bool>?)null)
                .WriteDictNullable(new Dictionary<string, bool>()
                {
                    {true.ToString(),true },
                    {false.ToString(),false }
                })
                .WriteObjectNullable((TestObject?)null)
                .WriteObjectNullable(new TestObject() { Value = true })
                .WriteSerializedNullable((TestObject2?)null)
                .WriteSerializedNullable(new TestObject2() { Value = true })
                .WriteAny(true)
                .WriteAny((sbyte)0)
                .WriteAny((byte)0)
                .WriteAny((short)0)
                .WriteAny((ushort)0)
                .WriteAny(0)
                .WriteAny((uint)0)
                .WriteAny((long)0)
                .WriteAny((ulong)0)
                .WriteAny((float)0)
                .WriteAny((double)0)
                .WriteAny((decimal)0)
                .WriteAny(TestEnum.Zero)
                .WriteAny(true.ToString())
                .WriteAny(new byte[] { 0 })
                .WriteAny(new bool[] { true, false })
                .WriteAny(new List<bool>() { true, false })
                .WriteAny(new Dictionary<string, bool>()
                {
                    {true.ToString(),true },
                    {false.ToString(),false }
                })
                .WriteAny(new TestObject() { Value = true })
                .WriteAny(new TestObject2() { Value = true })
                .WriteAnyNullable(null)
                .WriteAnyObject(new TestObject3() { Field1 = true, Field2 = true, Field3 = true })
                .WriteAnyObject(new TestObject3a() { Field1 = true, Field2 = true, Field3 = true })
                .WriteAnyObject(new TestObject3b() { Field1 = true, Field2 = true, Field3 = true })
                .WriteAnyObject(new TestObject4() { Field1 = true, Field2 = true, Field3 = true })
                .WriteAnyObject(new TestObject4a() { Field1 = true, Field2 = true, Field3 = true })
                .WriteAnyObject(new TestObject4b() { Field1 = true, Field2 = true, Field3 = true })
                .WriteAnyObjectNullable((TestObject3?)null)
                .WriteSerialized(test5)
                .WriteSerialized(test5a)
                .WriteSerialized(test5b)
                .WriteFixedArray(fixedData.AsSpan())
                .WriteStruct(new TestStruct(true));
            ms.Position = 0;
            Assert.IsTrue(ms.ReadBool());
            Assert.AreEqual((sbyte)0, ms.ReadOneSByte());
            Assert.AreEqual((byte)0, ms.ReadOneByte());
            Assert.AreEqual((short)0, ms.ReadShort());
            Assert.AreEqual((ushort)0, ms.ReadUShort());
            Assert.AreEqual(0, ms.ReadInt());
            Assert.AreEqual((uint)0, ms.ReadUInt());
            Assert.AreEqual(0, ms.ReadLong());
            Assert.AreEqual((ulong)0, ms.ReadULong());
            Assert.AreEqual(0, ms.ReadFloat());
            Assert.AreEqual(0, ms.ReadDouble());
            Assert.AreEqual(0, ms.ReadDecimal());
            Assert.AreEqual((sbyte)0, ms.ReadNumber<sbyte>());
            Assert.AreEqual(sbyte.MinValue, ms.ReadNumber<sbyte>());
            Assert.AreEqual(sbyte.MaxValue, ms.ReadNumber<sbyte>());
            Assert.AreEqual((byte)0, ms.ReadNumber<byte>());
            Assert.AreEqual(byte.MaxValue, ms.ReadNumber<byte>());
            Assert.AreEqual((short)0, ms.ReadNumber<short>());
            Assert.AreEqual(short.MinValue, ms.ReadNumber<short>());
            Assert.AreEqual(short.MaxValue, ms.ReadNumber<short>());
            Assert.AreEqual((ushort)0, ms.ReadNumber<ushort>());
            Assert.AreEqual(ushort.MaxValue, ms.ReadNumber<ushort>());
            Assert.AreEqual(0, ms.ReadNumber<int>());
            Assert.AreEqual(int.MinValue, ms.ReadNumber<int>());
            Assert.AreEqual(int.MaxValue, ms.ReadNumber<int>());
            Assert.AreEqual((uint)0, ms.ReadNumber<uint>());
            Assert.AreEqual(uint.MaxValue, ms.ReadNumber<uint>());
            Assert.AreEqual(0, ms.ReadNumber<long>());
            Assert.AreEqual(long.MinValue, ms.ReadNumber<long>());
            Assert.AreEqual(long.MaxValue, ms.ReadNumber<long>());
            Assert.AreEqual((ulong)0, ms.ReadNumber<ulong>());
            Assert.AreEqual(ulong.MaxValue, ms.ReadNumber<ulong>());
            Assert.AreEqual(0, ms.ReadNumber<float>());
            Assert.AreEqual(float.MinValue, ms.ReadNumber<float>());
            Assert.AreEqual(float.MaxValue, ms.ReadNumber<float>());
            Assert.AreEqual(0, ms.ReadNumber<double>());
            Assert.AreEqual(double.MinValue, ms.ReadNumber<double>());
            Assert.AreEqual(double.MaxValue, ms.ReadNumber<double>());
            Assert.AreEqual(0, ms.ReadNumber<decimal>());
            Assert.AreEqual(decimal.MinValue, ms.ReadNumber<decimal>());
            Assert.AreEqual(decimal.MaxValue, ms.ReadNumber<decimal>());
            Assert.AreEqual(TestEnum.Zero, ms.ReadEnum<TestEnum>());
            Assert.AreEqual(true.ToString(), ms.ReadString());
            Assert.IsTrue(ms.ReadBytes().Value.SequenceEqual(new byte[] { 0 }));
            Assert.IsTrue(ms.ReadArray<bool>().SequenceEqual(new bool[] { true, false }));
            Assert.IsTrue(ms.ReadList<bool>().SequenceEqual(new bool[] { true, false }));
            {
                Dictionary<string, bool> temp = ms.ReadDict<string, bool>();
                Assert.AreEqual(temp[true.ToString()], true);
                Assert.AreEqual(temp[false.ToString()], false);
            }
            Assert.IsTrue(ms.ReadObject<TestObject>().Value);
            Assert.IsTrue(ms.ReadSerialized<TestObject2>().Value);
            Assert.IsNull(ms.ReadBoolNullable());
            Assert.IsTrue(ms.ReadBoolNullable());
            Assert.IsNull(ms.ReadOneSByteNullable());
            Assert.AreEqual((sbyte)0, ms.ReadOneSByteNullable());
            Assert.IsNull(ms.ReadOneByteNullable());
            Assert.AreEqual((byte)0, ms.ReadOneByteNullable());
            Assert.IsNull(ms.ReadShortNullable());
            Assert.AreEqual((short)0, ms.ReadShortNullable());
            Assert.IsNull(ms.ReadUShortNullable());
            Assert.AreEqual((ushort)0, ms.ReadUShortNullable());
            Assert.IsNull(ms.ReadIntNullable());
            Assert.AreEqual(0, ms.ReadIntNullable());
            Assert.IsNull(ms.ReadUIntNullable());
            Assert.AreEqual((uint)0, ms.ReadUIntNullable());
            Assert.IsNull(ms.ReadLongNullable());
            Assert.AreEqual(0, ms.ReadLongNullable());
            Assert.IsNull(ms.ReadULongNullable());
            Assert.AreEqual((ulong)0, ms.ReadULongNullable());
            Assert.IsNull(ms.ReadFloatNullable());
            Assert.AreEqual(0, ms.ReadFloatNullable());
            Assert.IsNull(ms.ReadDoubleNullable());
            Assert.AreEqual(0, ms.ReadDoubleNullable());
            Assert.IsNull(ms.ReadDecimalNullable());
            Assert.AreEqual(0, ms.ReadDecimalNullable());
            Assert.IsNull(ms.ReadEnumNullable<TestEnum>());
            Assert.AreEqual(TestEnum.Zero, ms.ReadEnumNullable<TestEnum>());
            Assert.IsNull(ms.ReadStringNullable());
            Assert.AreEqual(true.ToString(), ms.ReadStringNullable());
            Assert.IsNull(ms.ReadBytesNullable());
            Assert.IsTrue(ms.ReadBytesNullable()?.Value.SequenceEqual(new byte[] { 0 }));
            Assert.IsNull(ms.ReadArrayNullable<bool>());
            Assert.IsTrue(ms.ReadArrayNullable<bool>()?.SequenceEqual(new bool[] { true, false }));
            Assert.IsNull(ms.ReadListNullable<bool>());
            Assert.IsTrue(ms.ReadListNullable<bool>()?.SequenceEqual(new bool[] { true, false }));
            Assert.IsNull(ms.ReadDictNullable<string, bool>());
            Assert.IsNotNull(ms.ReadDictNullable<string, bool>());
            Assert.IsNull(ms.ReadObjectNullable<TestObject>());
            Assert.IsTrue(ms.ReadObjectNullable<TestObject>()?.Value);
            Assert.IsNull(ms.ReadSerializedNullable<TestObject2>());
            Assert.IsTrue(ms.ReadSerializedNullable<TestObject2>()?.Value);
            Assert.AreEqual(true, ms.ReadAny());
            Assert.AreEqual((sbyte)0, ms.ReadAny());
            Assert.AreEqual((byte)0, ms.ReadAny());
            Assert.AreEqual((short)0, ms.ReadAny());
            Assert.AreEqual((ushort)0, ms.ReadAny());
            Assert.AreEqual(0, ms.ReadAny());
            Assert.AreEqual((uint)0, ms.ReadAny());
            Assert.AreEqual((long)0, ms.ReadAny());
            Assert.AreEqual((ulong)0, ms.ReadAny());
            Assert.AreEqual((float)0, ms.ReadAny());
            Assert.AreEqual((double)0, ms.ReadAny());
            Assert.AreEqual((decimal)0, ms.ReadAny());
            Assert.AreEqual(TestEnum.Zero, ms.ReadAny());
            Assert.AreEqual(true.ToString(), ms.ReadAny());
            Assert.IsTrue(((byte[])ms.ReadAny()).SequenceEqual(new byte[] { 0 }));
            Assert.IsTrue(((bool[])ms.ReadAny()).SequenceEqual(new bool[] { true, false }));
            Assert.IsTrue(((List<bool>)ms.ReadAny()).ToArray().SequenceEqual(new bool[] { true, false }));
            {
                Dictionary<string, bool> temp = (Dictionary<string, bool>)ms.ReadAny();
                Assert.AreEqual(temp[true.ToString()], true);
                Assert.AreEqual(temp[false.ToString()], false);
            }
            Assert.IsTrue(ms.ReadAny() is TestObject test1 && test1.Value);
            Assert.IsTrue(ms.ReadAny() is TestObject2 test2 && test2.Value);
            Assert.IsNull(ms.ReadAnyNullable());
            {
                TestObject3 temp = ms.ReadAnyObject<TestObject3>();
                Assert.IsTrue(temp.Field1);
                Assert.IsFalse(temp.Field2);
                Assert.IsTrue(temp.Field3);
            }
            {
                TestObject3a temp = ms.ReadAnyObject<TestObject3a>();
                Assert.IsTrue(temp.Field1);
                Assert.IsTrue(temp.Field2);
                Assert.IsTrue(temp.Field3);
            }
            {
                TestObject3b temp = ms.ReadAnyObject<TestObject3b>();
                Assert.IsTrue(temp.Field1);
                Assert.IsTrue(temp.Field2);
                Assert.IsFalse(temp.Field3);
            }
            {
                TestObject4 temp = ms.ReadAnyObject<TestObject4>();
                Assert.IsTrue(temp.Field1);
                Assert.IsFalse(temp.Field2);
                Assert.IsTrue(temp.Field3);
            }
            {
                TestObject4a temp = ms.ReadAnyObject<TestObject4a>();
                Assert.IsTrue(temp.Field1);
                Assert.IsTrue(temp.Field2);
                Assert.IsTrue(temp.Field3);
            }
            {
                TestObject4b temp = ms.ReadAnyObject<TestObject4b>();
                Assert.IsTrue(temp.Field1);
                Assert.IsTrue(temp.Field2);
                Assert.IsFalse(temp.Field3);
            }
            Assert.IsNull(ms.ReadAnyObjectNullable<TestObject3>());
            new TestObject2().ToBytes().ToObject<TestObject2>();
            {
                Assert.AreEqual(stream.Length, stream.Position);
                using TestObject5 test5_2 = ms.ReadSerialized<TestObject5>();
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
                using TestObject5a test5_2 = ms.ReadSerialized<TestObject5a>();
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
                using TestObject5b test5_2 = ms.ReadSerialized<TestObject5b>();
                using Stream? stream2 = test5_2.Stream;
                Assert.IsTrue(test5_2.AValue);
                Assert.IsFalse(test5_2.BValue);
                Assert.IsNull(stream2);
                Assert.IsTrue(test5_2.ZValue);
            }
            {
                int[] fixedData2 = ms.ReadFixedArray(new int[fixedData.Length]);
                Assert.IsTrue(fixedData.SequenceEqual(fixedData2));
            }
            {
                TestStruct obj = ms.ReadStruct<TestStruct>();
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
            await ms.WriteAsync(true);
            await ms.WriteAsync((sbyte)0);
            await ms.WriteAsync((byte)0);
            await ms.WriteAsync((short)0);
            await ms.WriteAsync((ushort)0);
            await ms.WriteAsync(0);
            await ms.WriteAsync((uint)0);
            await ms.WriteAsync((long)0);
            await ms.WriteAsync((ulong)0);
            await ms.WriteAsync((float)0);
            await ms.WriteAsync((double)0);
            await ms.WriteAsync((decimal)0);
            await ms.WriteNumberAsync((sbyte)0);
            await ms.WriteNumberAsync(sbyte.MinValue);
            await ms.WriteNumberAsync(sbyte.MaxValue);
            await ms.WriteNumberAsync((byte)0);
            await ms.WriteNumberAsync(byte.MaxValue);
            await ms.WriteNumberAsync((short)0);
            await ms.WriteNumberAsync(short.MinValue);
            await ms.WriteNumberAsync(short.MaxValue);
            await ms.WriteNumberAsync((ushort)0);
            await ms.WriteNumberAsync(ushort.MaxValue);
            await ms.WriteNumberAsync(0);
            await ms.WriteNumberAsync(int.MinValue);
            await ms.WriteNumberAsync(int.MaxValue);
            await ms.WriteNumberAsync((uint)0);
            await ms.WriteNumberAsync(uint.MaxValue);
            await ms.WriteNumberAsync((long)0);
            await ms.WriteNumberAsync(long.MinValue);
            await ms.WriteNumberAsync(long.MaxValue);
            await ms.WriteNumberAsync((ulong)0);
            await ms.WriteNumberAsync(ulong.MaxValue);
            await ms.WriteNumberAsync((float)0);
            await ms.WriteNumberAsync(float.MinValue);
            await ms.WriteNumberAsync(float.MaxValue);
            await ms.WriteNumberAsync((double)0);
            await ms.WriteNumberAsync(double.MinValue);
            await ms.WriteNumberAsync(double.MaxValue);
            await ms.WriteNumberAsync((decimal)0);
            await ms.WriteNumberAsync(decimal.MinValue);
            await ms.WriteNumberAsync(decimal.MaxValue);
            await ms.WriteEnumAsync(TestEnum.Zero);
            await ms.WriteStringAsync(true.ToString());
            await ms.WriteBytesAsync(new byte[] { 0 });
            await ms.WriteArrayAsync(new bool[] { true, false });
            await ms.WriteListAsync(new List<bool>(new bool[] { true, false }));
            await ms.WriteDictAsync(new Dictionary<string, bool>()
                {
                    {true.ToString(),true },
                    {false.ToString(),false }
                });
            await ms.WriteObjectAsync(new TestObject() { Value = true });
            await ms.WriteSerializedAsync(new TestObject2() { Value = true });
            await ms.WriteNullableAsync((bool?)null);
            await ms.WriteNullableAsync(true);
            await ms.WriteNullableAsync((sbyte?)null);
            await ms.WriteNullableAsync((sbyte?)0);
            await ms.WriteNullableAsync((byte?)null);
            await ms.WriteNullableAsync((byte?)0);
            await ms.WriteNullableAsync((short?)null);
            await ms.WriteNullableAsync((short?)0);
            await ms.WriteNullableAsync((ushort?)null);
            await ms.WriteNullableAsync((ushort?)0);
            await ms.WriteNullableAsync((int?)null);
            await ms.WriteNullableAsync((int?)0);
            await ms.WriteNullableAsync((uint?)null);
            await ms.WriteNullableAsync((uint?)0);
            await ms.WriteNullableAsync((long?)null);
            await ms.WriteNullableAsync((long?)0);
            await ms.WriteNullableAsync((ulong?)null);
            await ms.WriteNullableAsync((ulong?)0);
            await ms.WriteNullableAsync((float?)null);
            await ms.WriteNullableAsync((float?)0);
            await ms.WriteNullableAsync((double?)null);
            await ms.WriteNullableAsync((double?)0);
            await ms.WriteNullableAsync((decimal?)null);
            await ms.WriteNullableAsync((decimal?)0);
            await ms.WriteEnumNullableAsync((TestEnum?)null);
            await ms.WriteEnumNullableAsync((TestEnum?)TestEnum.Zero);
            await ms.WriteStringNullableAsync(null);
            await ms.WriteStringNullableAsync((string?)true.ToString());
            await ms.WriteBytesNullableAsync(null);
            await ms.WriteBytesNullableAsync((byte[]?)new byte[] { 0 });
            await ms.WriteArrayNullableAsync((bool[]?)null);
            await ms.WriteArrayNullableAsync(new bool[] { true, false });
            await ms.WriteListNullableAsync((List<bool>?)null);
            await ms.WriteListNullableAsync(new List<bool>(new bool[] { true, false }));
            await ms.WriteDictNullableAsync((Dictionary<string, bool>?)null);
            await ms.WriteDictNullableAsync(new Dictionary<string, bool>()
            {
                {true.ToString(),true },
                {false.ToString(),false }
            });
            await ms.WriteObjectNullableAsync((TestObject?)null);
            await ms.WriteObjectNullableAsync(new TestObject() { Value = true });
            await ms.WriteSerializedNullableAsync((TestObject2?)null);
            await ms.WriteSerializedNullableAsync(new TestObject2() { Value = true });
            await ms.WriteAnyAsync(true);
            await ms.WriteAnyAsync((sbyte)0);
            await ms.WriteAnyAsync((byte)0);
            await ms.WriteAnyAsync((short)0);
            await ms.WriteAnyAsync((ushort)0);
            await ms.WriteAnyAsync(0);
            await ms.WriteAnyAsync((uint)0);
            await ms.WriteAnyAsync((long)0);
            await ms.WriteAnyAsync((ulong)0);
            await ms.WriteAnyAsync((float)0);
            await ms.WriteAnyAsync((double)0);
            await ms.WriteAnyAsync((decimal)0);
            await ms.WriteAnyAsync(TestEnum.Zero);
            await ms.WriteAnyAsync(true.ToString());
            await ms.WriteAnyAsync(new byte[] { 0 });
            await ms.WriteAnyAsync(new bool[] { true, false });
            await ms.WriteAnyAsync(new List<bool>() { true, false });
            await ms.WriteAnyAsync(new Dictionary<string, bool>()
            {
                {true.ToString(),true },
                {false.ToString(),false }
            });
            await ms.WriteAnyAsync(new TestObject() { Value = true });
            await ms.WriteAnyAsync(new TestObject2() { Value = true });
            await ms.WriteAnyNullableAsync(null);
            await ms.WriteAnyObjectAsync(new TestObject3() { Field1 = true, Field2 = true, Field3 = true });
            await ms.WriteAnyObjectAsync(new TestObject3a() { Field1 = true, Field2 = true, Field3 = true });
            await ms.WriteAnyObjectAsync(new TestObject3b() { Field1 = true, Field2 = true, Field3 = true });
            await ms.WriteAnyObjectAsync(new TestObject4() { Field1 = true, Field2 = true, Field3 = true });
            await ms.WriteAnyObjectAsync(new TestObject4a() { Field1 = true, Field2 = true, Field3 = true });
            await ms.WriteAnyObjectAsync(new TestObject4b() { Field1 = true, Field2 = true, Field3 = true });
            await ms.WriteAnyObjectNullableAsync((TestObject3?)null);
            await ms.WriteSerializedAsync(test5);
            await ms.WriteSerializedAsync(test5a);
            await ms.WriteSerializedAsync(test5b);
            await ms.WriteFixedArrayAsync(fixedData.AsMemory());
            await ms.WriteStructAsync(new TestStruct(true));
            ms.Position = 0;
            Assert.IsTrue(await ms.ReadBoolAsync());
            Assert.AreEqual((sbyte)0, await ms.ReadOneSByteAsync());
            Assert.AreEqual((byte)0, await ms.ReadOneByteAsync());
            Assert.AreEqual((short)0, await ms.ReadShortAsync());
            Assert.AreEqual((ushort)0, await ms.ReadUShortAsync());
            Assert.AreEqual(0, await ms.ReadIntAsync());
            Assert.AreEqual((uint)0, await ms.ReadUIntAsync());
            Assert.AreEqual(0, await ms.ReadLongAsync());
            Assert.AreEqual((ulong)0, await ms.ReadULongAsync());
            Assert.AreEqual(0, await ms.ReadFloatAsync());
            Assert.AreEqual(0, await ms.ReadDoubleAsync());
            Assert.AreEqual(0, await ms.ReadDecimalAsync());
            Assert.AreEqual((sbyte)0, await ms.ReadNumberAsync<sbyte>());
            Assert.AreEqual(sbyte.MinValue, await ms.ReadNumberAsync<sbyte>());
            Assert.AreEqual(sbyte.MaxValue, await ms.ReadNumberAsync<sbyte>());
            Assert.AreEqual((byte)0, await ms.ReadNumberAsync<byte>());
            Assert.AreEqual(byte.MaxValue, await ms.ReadNumberAsync<byte>());
            Assert.AreEqual((short)0, await ms.ReadNumberAsync<short>());
            Assert.AreEqual(short.MinValue, await ms.ReadNumberAsync<short>());
            Assert.AreEqual(short.MaxValue, await ms.ReadNumberAsync<short>());
            Assert.AreEqual((ushort)0, await ms.ReadNumberAsync<ushort>());
            Assert.AreEqual(ushort.MaxValue, await ms.ReadNumberAsync<ushort>());
            Assert.AreEqual(0, await ms.ReadNumberAsync<int>());
            Assert.AreEqual(int.MinValue, await ms.ReadNumberAsync<int>());
            Assert.AreEqual(int.MaxValue, await ms.ReadNumberAsync<int>());
            Assert.AreEqual((uint)0, await ms.ReadNumberAsync<uint>());
            Assert.AreEqual(uint.MaxValue, await ms.ReadNumberAsync<uint>());
            Assert.AreEqual(0, await ms.ReadNumberAsync<long>());
            Assert.AreEqual(long.MinValue, await ms.ReadNumberAsync<long>());
            Assert.AreEqual(long.MaxValue, await ms.ReadNumberAsync<long>());
            Assert.AreEqual((ulong)0, await ms.ReadNumberAsync<ulong>());
            Assert.AreEqual(ulong.MaxValue, await ms.ReadNumberAsync<ulong>());
            Assert.AreEqual(0, await ms.ReadNumberAsync<float>());
            Assert.AreEqual(float.MinValue, await ms.ReadNumberAsync<float>());
            Assert.AreEqual(float.MaxValue, await ms.ReadNumberAsync<float>());
            Assert.AreEqual(0, await ms.ReadNumberAsync<double>());
            Assert.AreEqual(double.MinValue, await ms.ReadNumberAsync<double>());
            Assert.AreEqual(double.MaxValue, await ms.ReadNumberAsync<double>());
            Assert.AreEqual(0, await ms.ReadNumberAsync<decimal>());
            Assert.AreEqual(decimal.MinValue, await ms.ReadNumberAsync<decimal>());
            Assert.AreEqual(decimal.MaxValue, await ms.ReadNumberAsync<decimal>());
            Assert.AreEqual(TestEnum.Zero, await ms.ReadEnumAsync<TestEnum>());
            Assert.AreEqual(true.ToString(), await ms.ReadStringAsync());
            Assert.IsTrue((await ms.ReadBytesAsync()).Value.SequenceEqual(new byte[] { 0 }));
            Assert.IsTrue((await ms.ReadArrayAsync<bool>()).SequenceEqual(new bool[] { true, false }));
            Assert.IsTrue((await ms.ReadListAsync<bool>()).SequenceEqual(new bool[] { true, false }));
            {
                Dictionary<string, bool> temp = await ms.ReadDictAsync<string, bool>();
                Assert.AreEqual(temp[true.ToString()], true);
                Assert.AreEqual(temp[false.ToString()], false);
            }
            Assert.IsTrue((await ms.ReadObjectAsync<TestObject>()).Value);
            Assert.IsTrue((await ms.ReadObjectAsync<TestObject2>()).Value);
            Assert.IsNull(await ms.ReadBoolNullableAsync());
            Assert.IsTrue(await ms.ReadBoolNullableAsync());
            Assert.IsNull(await ms.ReadOneSByteNullableAsync());
            Assert.AreEqual((sbyte)0, await ms.ReadOneSByteNullableAsync());
            Assert.IsNull(await ms.ReadOneByteNullableAsync());
            Assert.AreEqual((byte)0, await ms.ReadOneByteNullableAsync());
            Assert.IsNull(await ms.ReadShortNullableAsync());
            Assert.AreEqual((short)0, await ms.ReadShortNullableAsync());
            Assert.IsNull(await ms.ReadUShortNullableAsync());
            Assert.AreEqual((ushort)0, await ms.ReadUShortNullableAsync());
            Assert.IsNull(await ms.ReadIntNullableAsync());
            Assert.AreEqual(0, await ms.ReadIntNullableAsync());
            Assert.IsNull(await ms.ReadUIntNullableAsync());
            Assert.AreEqual((uint)0, await ms.ReadUIntNullableAsync());
            Assert.IsNull(await ms.ReadLongNullableAsync());
            Assert.AreEqual(0, await ms.ReadLongNullableAsync());
            Assert.IsNull(await ms.ReadULongNullableAsync());
            Assert.AreEqual((ulong)0, await ms.ReadULongNullableAsync());
            Assert.IsNull(await ms.ReadFloatNullableAsync());
            Assert.AreEqual(0, await ms.ReadFloatNullableAsync());
            Assert.IsNull(await ms.ReadDoubleNullableAsync());
            Assert.AreEqual(0, await ms.ReadDoubleNullableAsync());
            Assert.IsNull(await ms.ReadDecimalNullableAsync());
            Assert.AreEqual(0, await ms.ReadDecimalNullableAsync());
            Assert.IsNull(await ms.ReadEnumNullableAsync<TestEnum>());
            Assert.AreEqual(TestEnum.Zero, await ms.ReadEnumNullableAsync<TestEnum>());
            Assert.IsNull(await ms.ReadStringNullableAsync());
            Assert.AreEqual(true.ToString(), await ms.ReadStringNullableAsync());
            Assert.IsNull(await ms.ReadBytesNullableAsync());
            Assert.IsTrue((await ms.ReadBytesNullableAsync())?.Value.SequenceEqual(new byte[] { 0 }));
            Assert.IsNull(await ms.ReadArrayNullableAsync<bool>());
            Assert.IsTrue((await ms.ReadArrayNullableAsync<bool>())?.SequenceEqual(new bool[] { true, false }));
            Assert.IsNull(await ms.ReadListNullableAsync<bool>());
            Assert.IsTrue((await ms.ReadListNullableAsync<bool>())?.SequenceEqual(new bool[] { true, false }));
            Assert.IsNull(await ms.ReadDictNullableAsync<string, bool>());
            Assert.IsNotNull(await ms.ReadDictNullableAsync<string, bool>());
            Assert.IsNull(await ms.ReadObjectNullableAsync<TestObject>());
            Assert.IsTrue((await ms.ReadObjectNullableAsync<TestObject>())?.Value);
            Assert.IsNull(await ms.ReadSerializedNullableAsync<TestObject2>());
            Assert.IsTrue((await ms.ReadSerializedNullableAsync<TestObject2>())?.Value);
            Assert.AreEqual(true, await ms.ReadAnyAsync());
            Assert.AreEqual((sbyte)0, await ms.ReadAnyAsync());
            Assert.AreEqual((byte)0, await ms.ReadAnyAsync());
            Assert.AreEqual((short)0, await ms.ReadAnyAsync());
            Assert.AreEqual((ushort)0, await ms.ReadAnyAsync());
            Assert.AreEqual(0, await ms.ReadAnyAsync());
            Assert.AreEqual((uint)0, await ms.ReadAnyAsync());
            Assert.AreEqual((long)0, await ms.ReadAnyAsync());
            Assert.AreEqual((ulong)0, await ms.ReadAnyAsync());
            Assert.AreEqual((float)0, await ms.ReadAnyAsync());
            Assert.AreEqual((double)0, await ms.ReadAnyAsync());
            Assert.AreEqual((decimal)0, await ms.ReadAnyAsync());
            Assert.AreEqual(TestEnum.Zero, await ms.ReadAnyAsync());
            Assert.AreEqual(true.ToString(), await ms.ReadAnyAsync());
            Assert.IsTrue(((byte[])await ms.ReadAnyAsync()).SequenceEqual(new byte[] { 0 }));
            Assert.IsTrue(((bool[])await ms.ReadAnyAsync()).SequenceEqual(new bool[] { true, false }));
            Assert.IsTrue(((List<bool>)await ms.ReadAnyAsync()).ToArray().SequenceEqual(new bool[] { true, false }));
            {
                Dictionary<string, bool> temp = (Dictionary<string, bool>)await ms.ReadAnyAsync();
                Assert.AreEqual(temp[true.ToString()], true);
                Assert.AreEqual(temp[false.ToString()], false);
            }
            Assert.IsTrue(await ms.ReadAnyAsync() is TestObject test1 && test1.Value);
            Assert.IsTrue(await ms.ReadAnyAsync() is TestObject2 test2 && test2.Value);
            Assert.IsNull(await ms.ReadAnyNullableAsync());
            {
                TestObject3 temp = await ms.ReadAnyObjectAsync<TestObject3>();
                Assert.IsTrue(temp.Field1);
                Assert.IsFalse(temp.Field2);
                Assert.IsTrue(temp.Field3);
            }
            {
                TestObject3a temp = await ms.ReadAnyObjectAsync<TestObject3a>();
                Assert.IsTrue(temp.Field1);
                Assert.IsTrue(temp.Field2);
                Assert.IsTrue(temp.Field3);
            }
            {
                TestObject3b temp = await ms.ReadAnyObjectAsync<TestObject3b>();
                Assert.IsTrue(temp.Field1);
                Assert.IsTrue(temp.Field2);
                Assert.IsFalse(temp.Field3);
            }
            {
                TestObject4 temp = await ms.ReadAnyObjectAsync<TestObject4>();
                Assert.IsTrue(temp.Field1);
                Assert.IsFalse(temp.Field2);
                Assert.IsTrue(temp.Field3);
            }
            {
                TestObject4a temp = await ms.ReadAnyObjectAsync<TestObject4a>();
                Assert.IsTrue(temp.Field1);
                Assert.IsTrue(temp.Field2);
                Assert.IsTrue(temp.Field3);
            }
            {
                TestObject4b temp = await ms.ReadAnyObjectAsync<TestObject4b>();
                Assert.IsTrue(temp.Field1);
                Assert.IsTrue(temp.Field2);
                Assert.IsFalse(temp.Field3);
            }
            Assert.IsNull(ms.ReadAnyObjectNullable<TestObject3>());
            {
                Assert.AreEqual(stream.Length, stream.Position);
                using TestObject5 test5_2 = await ms.ReadSerializedAsync<TestObject5>();
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
                using TestObject5a test5_2 = await ms.ReadSerializedAsync<TestObject5a>();
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
                using TestObject5b test5_2 = await ms.ReadSerializedAsync<TestObject5b>();
                using Stream? stream2 = test5_2.Stream;
                Assert.IsTrue(test5_2.AValue);
                Assert.IsFalse(test5_2.BValue);
                Assert.IsNull(stream2);
                Assert.IsTrue(test5_2.ZValue);
            }
            {
                int[] fixedData2 = await ms.ReadFixedArrayAsync(new int[fixedData.Length]);
                Assert.IsTrue(fixedData.SequenceEqual(fixedData2));
            }
            {
                TestStruct obj = await ms.ReadStructAsync<TestStruct>();
                Assert.IsTrue(obj.Value);
            }
            Assert.AreEqual(ms.Length, ms.Position);
        }
    }
}
