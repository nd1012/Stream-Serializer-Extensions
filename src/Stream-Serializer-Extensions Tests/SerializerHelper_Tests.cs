using System.Reflection;
using System.Runtime.CompilerServices;
using wan24.Core;
using wan24.StreamSerializerExtensions;

namespace Stream_Serializer_Extensions_Tests
{
    [TestClass]
    public class SerializerHelper_Tests
    {
        [TestMethod]
        public void NumberType_RemoveFlags_Tests()
        {
            NumberTypes nt = NumberTypes.FLAGS;
            Assert.AreEqual(NumberTypes.None, nt.RemoveFlags());
            nt = NumberTypes.Int | NumberTypes.FLAGS;
            Assert.AreEqual(NumberTypes.Int, nt.RemoveFlags());
            nt = NumberTypes.Int | NumberTypes.MinValue | NumberTypes.FLAGS;
            Assert.AreEqual(NumberTypes.Int | NumberTypes.MinValue, nt.RemoveFlags());
        }

        [TestMethod]
        public void NumberType_RemoveValueFlags_Tests()
        {
            NumberTypes nt = NumberTypes.VALUE_FLAGS;
            Assert.AreEqual(NumberTypes.None, nt.RemoveValueFlags());
            nt = NumberTypes.Int | NumberTypes.VALUE_FLAGS;
            Assert.AreEqual(NumberTypes.Int, nt.RemoveValueFlags());
            nt = NumberTypes.Int | NumberTypes.MinValue | NumberTypes.FLAGS;
            Assert.AreEqual(NumberTypes.Int | NumberTypes.FLAGS, nt.RemoveValueFlags());
        }

        [TestMethod]
        public void NumberType_IsUnsigned_Tests()
        {
            NumberTypes nt = NumberTypes.Int;
            Assert.IsFalse(nt.IsUnsigned());
            nt |= NumberTypes.Unsigned;
            Assert.IsTrue(nt.IsUnsigned());
        }

        [TestMethod]
        public void NumberType_IsMinValue_Tests()
        {
            NumberTypes nt = NumberTypes.Int | NumberTypes.MinValue;
            Assert.IsTrue(nt.IsMinValue());
            nt &= ~NumberTypes.MinValue;
            Assert.IsFalse(nt.IsMinValue());
        }

        [TestMethod]
        public void NumberType_IsMaxValue_Tests()
        {
            NumberTypes nt = NumberTypes.Int | NumberTypes.MaxValue;
            Assert.IsTrue(nt.IsMaxValue());
            nt &= ~NumberTypes.MaxValue;
            Assert.IsFalse(nt.IsMaxValue());
        }

        [TestMethod]
        public void NumberType_IsZero_Tests()
        {
            NumberTypes nt = NumberTypes.Zero;
            Assert.IsTrue(nt.IsZero());
            nt |= NumberTypes.Int;
            Assert.IsFalse(nt.IsZero());
            nt &= ~NumberTypes.Zero;
            Assert.IsFalse(nt.IsZero());
        }

        [TestMethod]
        public void NumberType_HasValueFlags_Tests()
        {
            NumberTypes nt = NumberTypes.Int | NumberTypes.MinValue;
            Assert.IsTrue(nt.HasValueFlags());
            nt &= ~NumberTypes.MinValue;
            Assert.IsFalse(nt.HasValueFlags());
        }

        [TestMethod]
        public void ObjectType_RemoveFlags_Tests()
        {
            ObjectTypes ot = ObjectTypes.FLAGS;
            Assert.AreEqual(ObjectTypes.Null, ot.RemoveFlags());
            ot = ObjectTypes.Object | ObjectTypes.FLAGS;
            Assert.AreEqual(ObjectTypes.Object, ot.RemoveFlags());
        }

        [TestMethod]
        public void ObjectType_IsEmpty()
        {
            ObjectTypes ot = ObjectTypes.Array | ObjectTypes.Empty;
            Assert.IsTrue(ot.IsEmpty());
            ot &= ~ObjectTypes.Empty;
            Assert.IsFalse(ot.IsEmpty());
        }

        [TestMethod]
        public void ObjectType_IsUnsigned()
        {
            ObjectTypes ot = ObjectTypes.Int | ObjectTypes.Unsigned;
            Assert.IsTrue(ot.IsUnsigned());
            ot &= ~ObjectTypes.Unsigned;
            Assert.IsFalse(ot.IsUnsigned());
        }

        [TestMethod]
        public void ObjectType_IsNumber()
        {
            ObjectTypes ot = ObjectTypes.Int | ObjectTypes.Unsigned;
            Assert.IsTrue(ot.IsNumber());
            Assert.IsFalse(ObjectTypes.Object.IsNumber());
        }

        [TestMethod]
        public void GetNumberType_Tests()
        {
            Assert.AreEqual(NumberTypes.None, string.Empty.GetNumberType());
            Assert.AreEqual(NumberTypes.None, SerializerHelper.GetNumberType((string?)null));
            Assert.AreEqual(NumberTypes.None, SerializerHelper.GetNumberType((int?)null));

            Assert.AreEqual(NumberTypes.Byte, ((sbyte)123).GetNumberType());
            Assert.AreEqual(NumberTypes.Zero, ((sbyte)0).GetNumberType());
            Assert.AreEqual(NumberTypes.Byte | NumberTypes.MinValue, sbyte.MinValue.GetNumberType());
            Assert.AreEqual(NumberTypes.Byte | NumberTypes.MaxValue, sbyte.MaxValue.GetNumberType());
            Assert.AreEqual(NumberTypes.Byte, sbyte.MaxValue.GetNumberType(false));

            Assert.AreEqual(NumberTypes.Byte | NumberTypes.Unsigned, ((byte)123).GetNumberType());
            Assert.AreEqual(NumberTypes.Zero, ((byte)0).GetNumberType());
            Assert.AreEqual(NumberTypes.Byte | NumberTypes.MaxValue | NumberTypes.Unsigned, byte.MaxValue.GetNumberType());
            Assert.AreEqual(NumberTypes.Byte | NumberTypes.Unsigned, byte.MaxValue.GetNumberType(false));

            Assert.AreEqual(NumberTypes.Short, ((short)123).GetNumberType());
            Assert.AreEqual(NumberTypes.Zero, ((short)0).GetNumberType());
            Assert.AreEqual(NumberTypes.Short | NumberTypes.MinValue, short.MinValue.GetNumberType());
            Assert.AreEqual(NumberTypes.Short | NumberTypes.MaxValue, short.MaxValue.GetNumberType());
            Assert.AreEqual(NumberTypes.Short, short.MaxValue.GetNumberType(false));

            Assert.AreEqual(NumberTypes.Short | NumberTypes.Unsigned, ((ushort)123).GetNumberType());
            Assert.AreEqual(NumberTypes.Zero, ((ushort)0).GetNumberType());
            Assert.AreEqual(NumberTypes.Short | NumberTypes.MaxValue | NumberTypes.Unsigned, ushort.MaxValue.GetNumberType());
            Assert.AreEqual(NumberTypes.Short | NumberTypes.Unsigned, ushort.MaxValue.GetNumberType(false));

            Assert.AreEqual(NumberTypes.Int, ((int)123).GetNumberType());
            Assert.AreEqual(NumberTypes.Zero, ((int)0).GetNumberType());
            Assert.AreEqual(NumberTypes.Int | NumberTypes.MinValue, int.MinValue.GetNumberType());
            Assert.AreEqual(NumberTypes.Int | NumberTypes.MaxValue, int.MaxValue.GetNumberType());
            Assert.AreEqual(NumberTypes.Int, int.MaxValue.GetNumberType(false));

            Assert.AreEqual(NumberTypes.Int | NumberTypes.Unsigned, ((uint)123).GetNumberType());
            Assert.AreEqual(NumberTypes.Zero, ((uint)0).GetNumberType());
            Assert.AreEqual(NumberTypes.Int | NumberTypes.MaxValue | NumberTypes.Unsigned, uint.MaxValue.GetNumberType());
            Assert.AreEqual(NumberTypes.Int | NumberTypes.Unsigned, uint.MaxValue.GetNumberType(false));

            Assert.AreEqual(NumberTypes.Long, ((long)123).GetNumberType());
            Assert.AreEqual(NumberTypes.Zero, ((long)0).GetNumberType());
            Assert.AreEqual(NumberTypes.Long | NumberTypes.MinValue, long.MinValue.GetNumberType());
            Assert.AreEqual(NumberTypes.Long | NumberTypes.MaxValue, long.MaxValue.GetNumberType());
            Assert.AreEqual(NumberTypes.Long, long.MaxValue.GetNumberType(false));

            Assert.AreEqual(NumberTypes.Long | NumberTypes.Unsigned, ((ulong)123).GetNumberType());
            Assert.AreEqual(NumberTypes.Zero, ((ulong)0).GetNumberType());
            Assert.AreEqual(NumberTypes.Long | NumberTypes.MaxValue | NumberTypes.Unsigned, ulong.MaxValue.GetNumberType());
            Assert.AreEqual(NumberTypes.Long | NumberTypes.Unsigned, ulong.MaxValue.GetNumberType(false));

            Assert.AreEqual(NumberTypes.Float, ((float)123).GetNumberType());
            Assert.AreEqual(NumberTypes.Zero, ((float)0).GetNumberType());
            Assert.AreEqual(NumberTypes.Float | NumberTypes.MinValue, float.MinValue.GetNumberType());
            Assert.AreEqual(NumberTypes.Float | NumberTypes.MaxValue, float.MaxValue.GetNumberType());
            Assert.AreEqual(NumberTypes.Float, float.MaxValue.GetNumberType(false));

            Assert.AreEqual(NumberTypes.Double, ((double)123).GetNumberType());
            Assert.AreEqual(NumberTypes.Zero, ((double)0).GetNumberType());
            Assert.AreEqual(NumberTypes.Double | NumberTypes.MinValue, double.MinValue.GetNumberType());
            Assert.AreEqual(NumberTypes.Double | NumberTypes.MaxValue, double.MaxValue.GetNumberType());
            Assert.AreEqual(NumberTypes.Double, double.MaxValue.GetNumberType(false));

            Assert.AreEqual(NumberTypes.Decimal, ((decimal)123).GetNumberType());
            Assert.AreEqual(NumberTypes.Zero, ((decimal)0).GetNumberType());
            Assert.AreEqual(NumberTypes.Decimal | NumberTypes.MinValue, decimal.MinValue.GetNumberType());
            Assert.AreEqual(NumberTypes.Decimal | NumberTypes.MaxValue, decimal.MaxValue.GetNumberType());
            Assert.AreEqual(NumberTypes.Decimal, decimal.MaxValue.GetNumberType(false));
        }

        [TestMethod]
        public void GetNumberAndType_Tests()
        {
            Assert.ThrowsException<ArgumentException>(() => string.Empty.GetNumberAndType());
            object number;
            NumberTypes nt;

            (number, nt) = 0.GetNumberAndType();
            Assert.AreEqual(0, number);
            Assert.AreEqual(NumberTypes.Zero, nt);

            (number, nt) = float.MinValue.GetNumberAndType();
            Assert.AreEqual(0, number);
            Assert.AreEqual(NumberTypes.Float | NumberTypes.MinValue, nt);

            (number, nt) = float.MaxValue.GetNumberAndType();
            Assert.AreEqual(0, number);
            Assert.AreEqual(NumberTypes.Float | NumberTypes.MaxValue, nt);

            (number, nt) = 123f.GetNumberAndType();
            Assert.AreEqual(123f, number);
            Assert.AreEqual(NumberTypes.Float, nt);

            (number, nt) = 123d.GetNumberAndType();
            Assert.AreEqual(123f, number);
            Assert.AreEqual(NumberTypes.Float, nt);

            (number, nt) = 1234567890123456789012345678901234567890d.GetNumberAndType();
            Assert.AreEqual(1234567890123456789012345678901234567890d, number);
            Assert.AreEqual(NumberTypes.Double, nt);

            (number, nt) = 123m.GetNumberAndType();
            Assert.AreEqual(123m, number);
            Assert.AreEqual(NumberTypes.Decimal, nt);

            (object Number, object Expected, NumberTypes NumberType)[] numbers = new (object Number, object Expected, NumberTypes NumberType)[]
            {
                (-1,(sbyte)-1,NumberTypes.Byte),
                (1,(byte)1,NumberTypes.Byte|NumberTypes.Unsigned),
                (sbyte.MinValue-1,(short)(sbyte.MinValue-1),NumberTypes.Short),
                (byte.MaxValue+1,(short)(byte.MaxValue+1),NumberTypes.Short),
                (short.MaxValue+1,(ushort)(short.MaxValue+1),NumberTypes.Short|NumberTypes.Unsigned),
                (ushort.MaxValue+1,ushort.MaxValue+1,NumberTypes.Int),
                ((long)int.MaxValue+1,(uint)int.MaxValue+1,NumberTypes.Int|NumberTypes.Unsigned),
                ((long)uint.MaxValue+1,(long)uint.MaxValue+1,NumberTypes.Long),
                ((ulong)long.MaxValue+1,(ulong)long.MaxValue+1,NumberTypes.Long|NumberTypes.Unsigned),
                (sbyte.MinValue-1,(short)(sbyte.MinValue-1),NumberTypes.Short),
                (short.MinValue-1,short.MinValue-1,NumberTypes.Int),
                ((long)int.MinValue-1,(long)int.MinValue-1,NumberTypes.Long)
            };
            for (int i = 0; i < numbers.Length; i++)
            {
                var (Number, Expected, NumberType) = numbers[i];
                (number, nt) = Number.GetNumberAndType();
                Assert.AreEqual(Expected, number, $"Expected number and type mismatch for {Number} at test #{i}");
                Assert.AreEqual(NumberType, nt, $"Expected number type mismatch for {Number} at test #{i}");
            }
        }

        [TestMethod]
        public void EnsureNotNull_Tests()
        {
            Assert.ThrowsException<SerializerException>(() => SerializerHelper.EnsureNotNull((string?)null));
            Assert.AreEqual(string.Empty, SerializerHelper.EnsureNotNull(string.Empty));
        }

        [TestMethod]
        public void EnsureValidLength_Tests()
        {
            Assert.ThrowsException<SerializerException>(() => SerializerHelper.EnsureValidLength(2, 0, 1));
            Assert.AreEqual(1, SerializerHelper.EnsureValidLength(1, 0, 1));
            Assert.ThrowsException<SerializerException>(() => SerializerHelper.EnsureValidLength(2L, 0L, 1L));
            Assert.AreEqual(1L, SerializerHelper.EnsureValidLength(1L, 0L, 1L));
        }

        [TestMethod]
        public void IsSerializerCOnstructor_Tests()
        {
            ConstructorInfo[] cis = typeof(TestObject2).GetConstructorsCached().OrderBy(ci => ci.GetParametersCached().Length).ToArray();
            Assert.AreEqual(2, cis.Length);
            Assert.AreEqual(0, cis[0].GetParametersCached().Length);
            Assert.AreEqual(2, cis[1].GetParametersCached().Length);
            Assert.IsFalse(cis[0].IsSerializerConstructor());
            Assert.IsTrue(cis[1].IsSerializerConstructor());
            Assert.IsFalse(cis[1].IsSerializerConstructor(true));
        }
    }
}
