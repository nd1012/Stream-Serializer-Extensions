using System.Diagnostics;
using System.Runtime;
using System.Runtime.CompilerServices;
using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    // NumberTypes
    public static partial class SerializerHelper
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
        /// Does the type require to write the serialized object?
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Is required?</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool RequiresObjectWriting(this NumberTypes type) => !type.IsZero() && !type.IsMinValue() && !type.IsMaxValue();

        /// <summary>
        /// Determine if the number type has value flags
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Has value flags?</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasValueFlags(this NumberTypes type) => (type & NumberTypes.VALUE_FLAGS) != 0;

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
        /// Get the CLR type of a number type
        /// </summary>
        /// <param name="type">Number type</param>
        /// <returns>CLR type</returns>
        public static Type GetClrType(this NumberTypes type) => type.RemoveValueFlags() switch
        {
            NumberTypes.Byte => typeof(sbyte),
            NumberTypes.Byte | NumberTypes.Unsigned => typeof(byte),
            NumberTypes.Short => typeof(short),
            NumberTypes.Short | NumberTypes.Unsigned => typeof(ushort),
            NumberTypes.Int => typeof(int),
            NumberTypes.Int | NumberTypes.Unsigned => typeof(uint),
            NumberTypes.Long => typeof(long),
            NumberTypes.Long | NumberTypes.Unsigned => typeof(ulong),
            NumberTypes.Float => typeof(float),
            NumberTypes.Double => typeof(double),
            NumberTypes.Decimal => typeof(decimal),
            _ => throw new ArgumentException($"Unknown number type {type}", nameof(type))
        };
    }
}
