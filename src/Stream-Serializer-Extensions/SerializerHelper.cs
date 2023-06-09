﻿using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Runtime;
using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    /// <summary>
    /// Helper
    /// </summary>
    public static class SerializerHelper
    {
        /// <summary>
        /// Remove flags
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Type without flags</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static NumberTypes RemoveFlags(this NumberTypes type) => type & ~NumberTypes.FLAGS;

        /// <summary>
        /// Remove value flags
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Type without value flags</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static NumberTypes RemoveValueFlags(this NumberTypes type) => type & ~NumberTypes.VALUE_FLAGS;

        /// <summary>
        /// Is unsigned?
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Unsigned?</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static bool IsUnsigned(this NumberTypes type) => type.ContainsAllFlags(NumberTypes.Unsigned);

        /// <summary>
        /// Is min. value?
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Min.value?</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static bool IsMinValue(this NumberTypes type) => type.ContainsAllFlags(NumberTypes.MinValue);

        /// <summary>
        /// Is max. value?
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Max. value?</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static bool IsMaxValue(this NumberTypes type) => type.ContainsAllFlags(NumberTypes.MaxValue);

        /// <summary>
        /// Is zero?
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Zero?</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static bool IsZero(this NumberTypes type) => type == NumberTypes.Zero;

        /// <summary>
        /// Determine if the number type has value flags
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Has value flags?</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static bool HasValueFlags(this NumberTypes type) => (type & NumberTypes.VALUE_FLAGS) != 0;

        /// <summary>
        /// Remove flags
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Type without flags</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static ObjectTypes RemoveFlags(this ObjectTypes type) => type & ~ObjectTypes.FLAGS;

        /// <summary>
        /// Is empty?
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Empty?</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static bool IsEmpty(this ObjectTypes type) => type.ContainsAllFlags(ObjectTypes.Empty);

        /// <summary>
        /// Is unsigned?
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Unsigned?</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static bool IsUnsigned(this ObjectTypes type) => type.ContainsAllFlags(ObjectTypes.Unsigned);

        /// <summary>
        /// Is a number?
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>A number?</returns>
        public static bool IsNumber(this ObjectTypes type) => type.RemoveFlags() switch
        {
            ObjectTypes.Short => true,
            ObjectTypes.Int => true,
            ObjectTypes.Long => true,
            ObjectTypes.Float => true,
            ObjectTypes.Double => true,
            ObjectTypes.Decimal => true,
            _ => false
        };

        /// <summary>
        /// Get the type of a number
        /// </summary>
        /// <param name="number">Number</param>
        /// <param name="useFlags">Use the flags?</param>
        /// <returns>Number type</returns>
        public static NumberTypes GetNumberType(this object number, bool useFlags = true)
            => number switch
            {
                sbyte => useFlags
                    ? (sbyte)number switch
                    {
                        0 => NumberTypes.Zero,
                        sbyte.MinValue => NumberTypes.Byte | NumberTypes.MinValue,
                        sbyte.MaxValue => NumberTypes.Byte | NumberTypes.MaxValue,
                        _ => NumberTypes.Byte
                    }
                    : NumberTypes.Byte,
                byte => useFlags
                    ? (byte)number switch
                    {
                        0 => NumberTypes.Zero,
                        byte.MaxValue => NumberTypes.Byte | NumberTypes.MaxValue | NumberTypes.Unsigned,
                        _ => NumberTypes.Byte | NumberTypes.Unsigned
                    }
                    : NumberTypes.Byte | NumberTypes.Unsigned,
                short => useFlags
                    ? (short)number switch
                    {
                        0 => NumberTypes.Zero,
                        short.MinValue => NumberTypes.Short | NumberTypes.MinValue,
                        short.MaxValue => NumberTypes.Short | NumberTypes.MaxValue,
                        _ => NumberTypes.Short
                    }
                    : NumberTypes.Short,
                ushort => useFlags
                    ? (ushort)number switch
                    {
                        0 => NumberTypes.Zero,
                        ushort.MaxValue => NumberTypes.Short | NumberTypes.MaxValue | NumberTypes.Unsigned,
                        _ => NumberTypes.Short | NumberTypes.Unsigned
                    }
                    : NumberTypes.Short | NumberTypes.Unsigned,
                int => useFlags
                    ? (int)number switch
                    {
                        0 => NumberTypes.Zero,
                        int.MinValue => NumberTypes.Int | NumberTypes.MinValue,
                        int.MaxValue => NumberTypes.Int | NumberTypes.MaxValue,
                        _ => NumberTypes.Int
                    }
                    : NumberTypes.Int,
                uint => useFlags
                    ? (uint)number switch
                    {
                        0 => NumberTypes.Zero,
                        uint.MaxValue => NumberTypes.Int | NumberTypes.MaxValue | NumberTypes.Unsigned,
                        _ => NumberTypes.Int | NumberTypes.Unsigned
                    }
                    : NumberTypes.Int | NumberTypes.Unsigned,
                long => useFlags
                    ? (long)number switch
                    {
                        0 => NumberTypes.Zero,
                        long.MinValue => NumberTypes.Long | NumberTypes.MinValue,
                        long.MaxValue => NumberTypes.Long | NumberTypes.MaxValue,
                        _ => NumberTypes.Long
                    }
                    : NumberTypes.Long,
                ulong => useFlags
                    ? (ulong)number switch
                    {
                        0 => NumberTypes.Zero,
                        ulong.MaxValue => NumberTypes.Long | NumberTypes.MaxValue | NumberTypes.Unsigned,
                        _ => NumberTypes.Long | NumberTypes.Unsigned
                    }
                    : NumberTypes.Long | NumberTypes.Unsigned,
                float => useFlags
                    ? (float)number switch
                    {
                        0 => NumberTypes.Zero,
                        float.MinValue => NumberTypes.Float | NumberTypes.MinValue,
                        float.MaxValue => NumberTypes.Float | NumberTypes.MaxValue,
                        _ => NumberTypes.Float
                    }
                    : NumberTypes.Float,
                double => useFlags
                    ? (double)number switch
                    {
                        0 => NumberTypes.Zero,
                        double.MinValue => NumberTypes.Double | NumberTypes.MinValue,
                        double.MaxValue => NumberTypes.Double | NumberTypes.MaxValue,
                        _ => NumberTypes.Double
                    }
                    : NumberTypes.Double,
                decimal => useFlags
                    ? (decimal)number switch
                    {
                        0 => NumberTypes.Zero,
                        decimal.MinValue => NumberTypes.Decimal | NumberTypes.MinValue,
                        decimal.MaxValue => NumberTypes.Decimal | NumberTypes.MaxValue,
                        _ => NumberTypes.Decimal
                    }
                    : NumberTypes.Decimal,
                _ => NumberTypes.None
            };

        /// <summary>
        /// Get the best matching number type
        /// </summary>
        /// <param name="number">Number</param>
        /// <returns>Number and type</returns>
        public static (object Number, NumberTypes Type) GetNumberAndType(this object number)
        {
            NumberTypes origin = GetNumberType(number),
                type;
            if (origin == NumberTypes.None) throw new ArgumentException("Not a supported numeric type", nameof(number));
            if (origin.IsZero() || origin.IsMinValue() || origin.IsMaxValue()) return (0, origin);
            object num;
            switch (origin.RemoveFlags())
            {
                case NumberTypes.Float:
                    num = number;
                    type = NumberTypes.Float;
                    break;
                case NumberTypes.Double:
                    {
                        double value = number.ConvertType<double>();
                        if (double.IsNaN(value) || double.IsInfinity(value) || double.IsSubnormal(value) || value < float.MinValue || value > float.MaxValue)
                        {
                            num = value;
                            type = NumberTypes.Double;
                        }
                        else
                        {
                            num = (float)value;
                            type = NumberTypes.Float;
                        }
                    }
                    break;
                case NumberTypes.Decimal:
                    num = number;
                    type = NumberTypes.Decimal;
                    break;
                default:
                    if (origin.IsUnsigned())
                    {
                        ulong value = number.ConvertType<ulong>();
                        if (value > long.MaxValue)
                        {
                            num = value;
                            type = NumberTypes.Long | NumberTypes.Unsigned;
                        }
                        else if (value > uint.MaxValue)
                        {
                            num = (long)value;
                            type = NumberTypes.Long;
                        }
                        else if (value > int.MaxValue)
                        {
                            num = (uint)value;
                            type = NumberTypes.Int | NumberTypes.Unsigned;
                        }
                        else if (value > ushort.MaxValue)
                        {
                            num = (int)value;
                            type = NumberTypes.Int;
                        }
                        else if ((int)value > short.MaxValue)
                        {
                            num = (ushort)value;
                            type = NumberTypes.Int | NumberTypes.Unsigned;
                        }
                        else if (value > byte.MaxValue)
                        {
                            num = (short)value;
                            type = NumberTypes.Int;
                        }
                        else if ((short)value > sbyte.MaxValue)
                        {
                            num = (byte)value;
                            type = NumberTypes.Int | NumberTypes.Unsigned;
                        }
                        else
                        {
                            num = (sbyte)value;
                            type = NumberTypes.Int;
                        }
                    }
                    else
                    {
                        long value = number.ConvertType<long>();
                        if (value < 0)
                        {
                            if (value < int.MinValue)
                            {
                                num = value;
                                type = NumberTypes.Long;
                            }
                            else if (value < short.MinValue)
                            {
                                num = (int)value;
                                type = NumberTypes.Int;
                            }
                            else if (value < sbyte.MinValue)
                            {
                                num = (short)value;
                                type = NumberTypes.Short;
                            }
                            else
                            {
                                num = (sbyte)value;
                                type = NumberTypes.Byte;
                            }
                        }
                        else
                        {
                            if (value > uint.MaxValue)
                            {
                                num = value;
                                type = NumberTypes.Long;
                            }
                            else if (value > int.MaxValue)
                            {
                                num = (uint)value;
                                type = NumberTypes.Int | NumberTypes.Unsigned;
                            }
                            else if (value > ushort.MaxValue)
                            {
                                num = (int)value;
                                type = NumberTypes.Int;
                            }
                            else if ((int)value > short.MaxValue)
                            {
                                num = (ushort)value;
                                type = NumberTypes.Int | NumberTypes.Unsigned;
                            }
                            else if (value > byte.MaxValue)
                            {
                                num = (short)value;
                                type = NumberTypes.Int;
                            }
                            else if ((short)value > sbyte.MaxValue)
                            {
                                num = (byte)value;
                                type = NumberTypes.Int | NumberTypes.Unsigned;
                            }
                            else
                            {
                                num = (sbyte)value;
                                type = NumberTypes.Int;
                            }
                        }
                    }
                    break;
            }
            return (num, type);
        }

        /// <summary>
        /// Get object serializer informations
        /// </summary>
        /// <param name="obj">Object</param>
        /// <returns>Informations</returns>
        public static (Type Type, ObjectTypes ObjectType, bool WriteType, bool WriteObject) GetObjectSerializerInfo(this object obj)
        {
            Type type = obj.GetType();
            ObjectTypes objType = obj switch
            {
                bool => ObjectTypes.Bool,
                sbyte => ObjectTypes.Byte,
                byte => ObjectTypes.Byte | ObjectTypes.Unsigned,
                short => ObjectTypes.Short,
                ushort => ObjectTypes.Short | ObjectTypes.Unsigned,
                int => ObjectTypes.Int,
                uint => ObjectTypes.Int | ObjectTypes.Unsigned,
                long => ObjectTypes.Long,
                ulong => ObjectTypes.Long | ObjectTypes.Unsigned,
                float => ObjectTypes.Float,
                double => ObjectTypes.Double,
                decimal => ObjectTypes.Decimal,
                string => ObjectTypes.String,
                byte[] => ObjectTypes.Bytes,
                Stream => ObjectTypes.Stream,
                IStreamSerializer => ObjectTypes.Serializable,
                _ => ObjectTypes.Null
            };
            if (objType == ObjectTypes.Null)
                if (type.IsArray)
                {
                    objType = ObjectTypes.Array;
                }
                else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
                {
                    objType = ObjectTypes.List;
                }
                else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                {
                    objType = ObjectTypes.Dict;
                }
                else if (type.IsValueType && !type.IsEnum)
                {
                    objType = ObjectTypes.Struct;
                }
                else
                {
                    objType = ObjectTypes.Object;
                }
            bool writeType = false,
                writeObject = true;
            switch (objType.RemoveFlags())
            {
                case ObjectTypes.Bool:
                    if (!(bool)obj) objType |= ObjectTypes.Empty;
                    writeObject = false;
                    break;
                case ObjectTypes.Byte:
                case ObjectTypes.Short:
                case ObjectTypes.Int:
                case ObjectTypes.Long:
                case ObjectTypes.Float:
                case ObjectTypes.Double:
                case ObjectTypes.Decimal:
                    if (Activator.CreateInstance(type)!.Equals(obj))
                    {
                        objType |= ObjectTypes.Empty;
                        writeObject = false;
                    }
                    break;
                case ObjectTypes.String:
                    if (((string)obj).Length == 0)
                    {
                        objType |= ObjectTypes.Empty;
                        writeObject = false;
                    }
                    break;
                case ObjectTypes.Array:
                    if (((Array)obj).Length == 0)
                    {
                        objType |= ObjectTypes.Empty;
                        writeObject = false;
                    }
                    else
                    {
                        writeType = true;
                    }
                    break;
                case ObjectTypes.List:
                    if (((IList)obj).Count == 0)
                    {
                        objType |= ObjectTypes.Empty;
                        writeObject = false;
                    }
                    else
                    {
                        writeType = true;
                    }
                    break;
                case ObjectTypes.Dict:
                    if (((IDictionary)obj).Count == 0)
                    {
                        objType |= ObjectTypes.Empty;
                        writeObject = false;
                    }
                    else
                    {
                        writeType = true;
                    }
                    break;
                case ObjectTypes.Object:
                case ObjectTypes.Struct:
                case ObjectTypes.Serializable:
                    writeType = true;
                    break;
                case ObjectTypes.Bytes:
                    if (((byte[])obj).Length == 0)
                    {
                        objType |= ObjectTypes.Empty;
                        writeObject = false;
                    }
                    break;
                case ObjectTypes.Stream:
                    Stream stream = (Stream)obj;
                    if (stream.CanSeek && stream.Length == 0)
                    {
                        objType |= ObjectTypes.Empty;
                        writeObject = false;
                    }
                    break;
                default:
                    throw new InvalidProgramException();
            }
            return (type, objType, writeType, writeObject);
        }

        /// <summary>
        /// Ensure a non-null value
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Non-null value</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static object EnsureNotNull(object? value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            return value;
        }

        /// <summary>
        /// Ensure a valid length
        /// </summary>
        /// <param name="len">Length</param>
        /// <param name="min">Minimum value</param>
        /// <param name="max">Maximum value</param>
        /// <returns>Length</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static int EnsureValidLength(int len, int min = 0, int max = int.MaxValue)
        {
            if (len < min || len > max) throw new InvalidDataException($"Invalid length {len}");
            return len;
        }

        /// <summary>
        /// Ensure a valid length
        /// </summary>
        /// <param name="len">Length</param>
        /// <param name="min">Minimum value</param>
        /// <param name="max">Maximum value</param>
        /// <returns>Length</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static long EnsureValidLength(long len, long min = 0, long max = long.MaxValue)
        {
            if (len < min || len > max) throw new InvalidDataException($"Invalid length {len}");
            return len;
        }

        /// <summary>
        /// Validate an object and throw an exception, if the validation failed
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="value">Value</param>
        /// <returns>Value</returns>
        public static T ValidateObject<T>(this T value) where T : notnull
        {
            List<ValidationResult> results = new();
            if (!Validator.TryValidateObject(value, new(value, serviceProvider: null, items: null), results, validateAllProperties: true) ||
                !Validator.TryValidateObject(value, new(value, serviceProvider: null, items: null), results, validateAllProperties: false))
                throw new SerializerException($"The deserialized object contains {results.Count} errors: {results[0].ErrorMessage} ({string.Join(',', results[0].MemberNames)})");
            return value;
        }

        /// <summary>
        /// Get bytes from a serializable object
        /// </summary>
        /// <param name="obj">Object</param>
        /// <param name="includeSerializerVersion">Include the serializer version number?</param>
        /// <returns>Bytes</returns>
        public static byte[] ToBytes(this IStreamSerializer obj, bool includeSerializerVersion = true)
        {
            using MemoryStream ms = new();
            if (includeSerializerVersion) ms.WriteSerializerVersion();
            ms.WriteSerialized(obj);
            return ms.ToArray();
        }

        /// <summary>
        /// Deserialize an object
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="bytes">Bytes</param>
        /// <param name="includesSerializerVersion">Serializer version number included?</param>
        /// <returns>Object</returns>
        public static T ToObject<T>(this byte[] bytes, bool includesSerializerVersion = true) where T : class, IStreamSerializer, new()
        {
            using MemoryStream ms = new(bytes);
            int serializerVersion = StreamSerializer.VERSION;
            if (includesSerializerVersion) serializerVersion = ms.ReadSerializerVersion();
            return ms.ReadSerialized<T>(serializerVersion);
        }

        /// <summary>
        /// Get the serializer options
        /// </summary>
        /// <param name="pi">Property</param>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Serializer options</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static ISerializerOptions? GetSerializerOptions(this PropertyInfo pi, Stream stream, int version, CancellationToken cancellationToken)
            => pi.GetCustomAttribute<StreamSerializerAttribute>()?.GetSerializerOptions(pi, stream, version, cancellationToken);

        /// <summary>
        /// Get the key serializer options
        /// </summary>
        /// <param name="pi">Property</param>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Serializer options</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static ISerializerOptions? GetKeySerializerOptions(this PropertyInfo pi, Stream stream, int version, CancellationToken cancellationToken)
            => pi.GetCustomAttribute<StreamSerializerAttribute>()?.GetKeySerializerOptions(pi, stream, version, cancellationToken);

        /// <summary>
        /// Get the value serializer options
        /// </summary>
        /// <param name="pi">Property</param>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Serializer options</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static ISerializerOptions? GetValueSerializerOptions(this PropertyInfo pi, Stream stream, int version, CancellationToken cancellationToken)
            => pi.GetCustomAttribute<StreamSerializerAttribute>()?.GetValueSerializerOptions(pi, stream, version, cancellationToken);
    }
}
