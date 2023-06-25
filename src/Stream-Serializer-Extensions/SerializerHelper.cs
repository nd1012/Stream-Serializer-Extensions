using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Reflection;
using System.Runtime;
using System.Runtime.CompilerServices;
using wan24.Core;
using wan24.ObjectValidation;

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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NumberTypes RemoveFlags(this NumberTypes type) => type & ~NumberTypes.FLAGS;

        /// <summary>
        /// Remove value flags
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Type without value flags</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NumberTypes RemoveValueFlags(this NumberTypes type) => type & ~NumberTypes.VALUE_FLAGS;

        /// <summary>
        /// Is unsigned?
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Unsigned?</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsUnsigned(this NumberTypes type) => type.ContainsAllFlags(NumberTypes.Unsigned);

        /// <summary>
        /// Is min. value?
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Min.value?</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsMinValue(this NumberTypes type) => type.ContainsAllFlags(NumberTypes.MinValue);

        /// <summary>
        /// Is max. value?
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Max. value?</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsMaxValue(this NumberTypes type) => type.ContainsAllFlags(NumberTypes.MaxValue);

        /// <summary>
        /// Is zero?
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Zero?</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsZero(this NumberTypes type) => type == NumberTypes.Zero;

        /// <summary>
        /// Determine if the number type has value flags
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Has value flags?</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasValueFlags(this NumberTypes type) => (type & NumberTypes.VALUE_FLAGS) != 0;

        /// <summary>
        /// Remove flags
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Type without flags</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ObjectTypes RemoveFlags(this ObjectTypes type) => type & ~ObjectTypes.FLAGS;

        /// <summary>
        /// Is empty?
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Empty?</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEmpty(this ObjectTypes type) => type.ContainsAllFlags(ObjectTypes.Empty);

        /// <summary>
        /// Is unsigned?
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Unsigned?</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsUnsigned(this ObjectTypes type) => type.ContainsAllFlags(ObjectTypes.Unsigned);

        /// <summary>
        /// Is a number?
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>A number?</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
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
        /// <typeparam name="T">Number type</typeparam>
        /// <param name="number">Number</param>
        /// <param name="useFlags">Use the flags?</param>
        /// <returns>Number type</returns>
        public static NumberTypes GetNumberType<T>(this T number, bool useFlags = true)
            => number switch
            {
                sbyte sb => useFlags
                    ? sb switch
                    {
                        0 => NumberTypes.Zero,
                        sbyte.MinValue => NumberTypes.Byte | NumberTypes.MinValue,
                        sbyte.MaxValue => NumberTypes.Byte | NumberTypes.MaxValue,
                        _ => NumberTypes.Byte
                    }
                    : NumberTypes.Byte,
                byte b => useFlags
                    ? b switch
                    {
                        0 => NumberTypes.Zero,
                        byte.MaxValue => NumberTypes.Byte | NumberTypes.MaxValue | NumberTypes.Unsigned,
                        _ => NumberTypes.Byte | NumberTypes.Unsigned
                    }
                    : NumberTypes.Byte | NumberTypes.Unsigned,
                short s => useFlags
                    ? s switch
                    {
                        0 => NumberTypes.Zero,
                        short.MinValue => NumberTypes.Short | NumberTypes.MinValue,
                        short.MaxValue => NumberTypes.Short | NumberTypes.MaxValue,
                        _ => NumberTypes.Short
                    }
                    : NumberTypes.Short,
                ushort us => useFlags
                    ? us switch
                    {
                        0 => NumberTypes.Zero,
                        ushort.MaxValue => NumberTypes.Short | NumberTypes.MaxValue | NumberTypes.Unsigned,
                        _ => NumberTypes.Short | NumberTypes.Unsigned
                    }
                    : NumberTypes.Short | NumberTypes.Unsigned,
                int i => useFlags
                    ? i switch
                    {
                        0 => NumberTypes.Zero,
                        int.MinValue => NumberTypes.Int | NumberTypes.MinValue,
                        int.MaxValue => NumberTypes.Int | NumberTypes.MaxValue,
                        _ => NumberTypes.Int
                    }
                    : NumberTypes.Int,
                uint ui => useFlags
                    ? ui switch
                    {
                        0 => NumberTypes.Zero,
                        uint.MaxValue => NumberTypes.Int | NumberTypes.MaxValue | NumberTypes.Unsigned,
                        _ => NumberTypes.Int | NumberTypes.Unsigned
                    }
                    : NumberTypes.Int | NumberTypes.Unsigned,
                long l => useFlags
                    ? l switch
                    {
                        0 => NumberTypes.Zero,
                        long.MinValue => NumberTypes.Long | NumberTypes.MinValue,
                        long.MaxValue => NumberTypes.Long | NumberTypes.MaxValue,
                        _ => NumberTypes.Long
                    }
                    : NumberTypes.Long,
                ulong ul => useFlags
                    ? ul switch
                    {
                        0 => NumberTypes.Zero,
                        ulong.MaxValue => NumberTypes.Long | NumberTypes.MaxValue | NumberTypes.Unsigned,
                        _ => NumberTypes.Long | NumberTypes.Unsigned
                    }
                    : NumberTypes.Long | NumberTypes.Unsigned,
                float f => useFlags
                    ? f switch
                    {
                        0 => NumberTypes.Zero,
                        float.MinValue => NumberTypes.Float | NumberTypes.MinValue,
                        float.MaxValue => NumberTypes.Float | NumberTypes.MaxValue,
                        _ => NumberTypes.Float
                    }
                    : NumberTypes.Float,
                double d => useFlags
                    ? d switch
                    {
                        0 => NumberTypes.Zero,
                        double.MinValue => NumberTypes.Double | NumberTypes.MinValue,
                        double.MaxValue => NumberTypes.Double | NumberTypes.MaxValue,
                        _ => NumberTypes.Double
                    }
                    : NumberTypes.Double,
                decimal m => useFlags
                    ? m switch
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
        /// <typeparam name="T">Number type</typeparam>
        /// <param name="number">Number</param>
        /// <returns>Number and type</returns>
        public static (object Number, NumberTypes Type) GetNumberAndType<T>(this T number)
        {
            ArgumentValidationHelper.EnsureValidArgument(nameof(number), number);
#if DEBUG
            Debug.Assert(number != null);
#endif
            NumberTypes origin = GetNumberType(number),
                type;
            ArgumentValidationHelper.EnsureValidArgument(nameof(number), origin != NumberTypes.None, () => "Not a supported numeric type");
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
                            type = NumberTypes.Short | NumberTypes.Unsigned;
                        }
                        else if (value > byte.MaxValue)
                        {
                            num = (short)value;
                            type = NumberTypes.Short;
                        }
                        else
                        {
                            num = (byte)value;
                            type = NumberTypes.Byte | NumberTypes.Unsigned;
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
                                type = NumberTypes.Short | NumberTypes.Unsigned;
                            }
                            else if (value > byte.MaxValue)
                            {
                                num = (short)value;
                                type = NumberTypes.Short;
                            }
                            else
                            {
                                num = (byte)value;
                                type = NumberTypes.Byte | NumberTypes.Unsigned;
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
            ArgumentValidationHelper.EnsureValidArgument(nameof(obj), obj);
#if DEBUG
            Debug.Assert(obj != null);
#endif
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
                else if (typeof(IList).IsAssignableFrom(type))
                {
                    objType = ObjectTypes.List;
                }
                else if (typeof(IDictionary).IsAssignableFrom(type))
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
        /// <typeparam name="T">Value type</typeparam>
        /// <param name="value">Value</param>
        /// <returns>Non-null value</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T EnsureNotNull<T>(T? value)
            => value ?? throw new SerializerException($"Argument {nameof(value)} is NULL", new ArgumentNullException(nameof(value)));

        /// <summary>
        /// Ensure a valid length
        /// </summary>
        /// <param name="len">Length</param>
        /// <param name="min">Minimum value</param>
        /// <param name="max">Maximum value</param>
        /// <returns>Length</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int EnsureValidLength(int len, int min = 0, int max = int.MaxValue)
        {
            if (len < min || len > max) throw new SerializerException($"Invalid length {len}", new InvalidDataException());
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long EnsureValidLength(long len, long min = 0, long max = long.MaxValue)
        {
            if (len < min || len > max) throw new SerializerException($"Invalid length {len}", new InvalidDataException());
            return len;
        }

        /// <summary>
        /// Validate an object and throw an exception, if the validation failed
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="value">Value</param>
        /// <returns>Value</returns>
        /// <exception cref="SerializerException">If the validation failed</exception>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T ValidateDeserializedObject<T>(this T value) where T : notnull
        {
            if (!value.TryValidateObject(out List<ValidationResult> results))
                throw new SerializerException(
                    $"The deserialized object contains {results.Count} errors: {results[0].ErrorMessage} ({string.Join(',', results[0].MemberNames)})",
                    new ObjectValidationException(results)
                    );
            return value;
        }

        /// <summary>
        /// Get bytes from a serializable object
        /// </summary>
        /// <param name="obj">Object</param>
        /// <param name="includeSerializerVersion">Include the serializer version number?</param>
        /// <returns>Bytes</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
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
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T ToObject<T>(this byte[] bytes, bool includesSerializerVersion = true) where T : class, IStreamSerializer, new()
        {
            using MemoryStream ms = new(bytes);
            return ms.ReadSerialized<T>(includesSerializerVersion ? ms.ReadSerializerVersion() : StreamSerializer.Version);
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ISerializerOptions? GetSerializerOptions(this PropertyInfoExt pi, Stream stream, int version, CancellationToken cancellationToken)
            => pi.GetCustomAttributeCached<StreamSerializerAttribute>()?.GetSerializerOptions(pi, stream, version, cancellationToken);

        /// <summary>
        /// Get the key serializer options
        /// </summary>
        /// <param name="pi">Property</param>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Serializer options</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ISerializerOptions? GetKeySerializerOptions(this PropertyInfoExt pi, Stream stream, int version, CancellationToken cancellationToken)
            => pi.GetCustomAttributeCached<StreamSerializerAttribute>()?.GetKeySerializerOptions(pi, stream, version, cancellationToken);

        /// <summary>
        /// Get the value serializer options
        /// </summary>
        /// <param name="pi">Property</param>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Serializer options</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ISerializerOptions? GetValueSerializerOptions(this PropertyInfoExt pi, Stream stream, int version, CancellationToken cancellationToken)
            => pi.GetCustomAttributeCached<StreamSerializerAttribute>()?.GetValueSerializerOptions(pi, stream, version, cancellationToken);

        /// <summary>
        /// Determine if the constructor is the serializer constructor
        /// </summary>
        /// <param name="ci">Constructor</param>
        /// <param name="requireAttribute">Require the <see cref="StreamSerializerAttribute"/>?</param>
        /// <returns>Is the serializer constructor?</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSerializerConstructor(this ConstructorInfo ci, bool requireAttribute = false)
        {
            ParameterInfo[] pis = ci.GetParametersCached();
            return pis.Select(p => p.ParameterType).ContainsAll(typeof(Stream), typeof(int)) &&
                pis[0].ParameterType == typeof(Stream) &&
                pis[1].ParameterType == typeof(int) &&
                (!requireAttribute || ci.GetCustomAttributeCached<StreamSerializerAttribute>() is not null);
        }
    }
}
