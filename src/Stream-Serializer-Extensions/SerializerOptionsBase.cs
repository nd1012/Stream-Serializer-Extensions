using System.ComponentModel.DataAnnotations;
using System.Reflection;
using wan24.ObjectValidation;

namespace wan24.StreamSerializerExtensions
{
    /// <summary>
    /// Base class for serializer options
    /// </summary>
    public abstract class SerializerOptionsBase : ISerializerOptions
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="property">Target property</param>
        /// <param name="attr">Stream serializer attribute (required, if <c>property</c> is <see langword="null"/>)</param>
        protected SerializerOptionsBase(PropertyInfo? property, StreamSerializerAttribute? attr = null) : base()
        {
            Property = property;
            Attribute = attr ??
                property?.GetCustomAttribute<StreamSerializerAttribute>() ??
                throw (property == null ? new ArgumentNullException(nameof(attr)) : new ArgumentException($"{typeof(StreamSerializerAttribute)} attribute required", nameof(property)));
        }

        /// <inheritdoc/>
        public PropertyInfo? Property { get; }

        /// <inheritdoc/>
        public StreamSerializerAttribute Attribute { get; }

        /// <inheritdoc/>
        public virtual int GetMinLen(int defaultValue)
        {
            int? res = (int?)Attribute.MinLen;
            if (res == null && Property != null)
                if (Property.GetCustomAttribute<RangeAttribute>() is RangeAttribute range)
                {
                    res = (int)range.Minimum;
                }
                else if (Property.GetCustomAttribute<CountLimitAttribute>() is CountLimitAttribute countLimit)
                {
                    res = (int?)countLimit.Min;
                }
                else if (Property.GetCustomAttribute<MinLengthAttribute>() is MinLengthAttribute minLength)
                {
                    res = minLength.Length;
                }
                else if (Property.PropertyType == typeof(string))
                {
                    if (Property.GetCustomAttribute<StringLengthAttribute>() is StringLengthAttribute stringLength && stringLength.MinimumLength > 0)
                    {
                        res = stringLength.MinimumLength;
                    }
                    else if (Property.GetCustomAttribute<RequiredAttribute>() != null)
                    {
                        res = 1;
                    }
                    else if (Property.GetCustomAttribute<StringLengthAttribute>() is StringLengthAttribute stringLength2)
                    {
                        res = stringLength2.MinimumLength;
                    }
                }
            return res ?? defaultValue;
        }

        /// <inheritdoc/>
        public virtual int GetMaxLen(int defaultValue)
        {
            int? res = (int?)Attribute.MaxLen;
            if (res == null && Property != null)
                if (Property.GetCustomAttribute<RangeAttribute>() is RangeAttribute range)
                {
                    res = (int)range.Maximum;
                }
                else if (Property.GetCustomAttribute<CountLimitAttribute>() is CountLimitAttribute countLimit)
                {
                    res = (int)countLimit.Max;
                }
                else if (Property.GetCustomAttribute<MaxLengthAttribute>() is MaxLengthAttribute maxLength)
                {
                    res = maxLength.Length;
                }
                else if (Property.PropertyType == typeof(string) && Property.GetCustomAttribute<StringLengthAttribute>() is StringLengthAttribute stringLength)
                {
                    res = stringLength.MaximumLength;
                }
            return res ?? defaultValue;
        }

        /// <inheritdoc/>
        public virtual long GetMinLen(long defaultValue)
        {
            long? res = Attribute.MinLen;
            if (res == null && Property != null)
                if (Property.GetCustomAttribute<RangeAttribute>() is RangeAttribute range)
                {
                    res = (long)range.Minimum;
                }
                else if (Property.GetCustomAttribute<CountLimitAttribute>() is CountLimitAttribute countLimit)
                {
                    res = countLimit.Min;
                }
                else if (Property.GetCustomAttribute<MinLengthAttribute>() is MinLengthAttribute minLength)
                {
                    res = minLength.Length;
                }
                else if (Property.PropertyType == typeof(string))
                {
                    if (Property.GetCustomAttribute<StringLengthAttribute>() is StringLengthAttribute stringLength && stringLength.MinimumLength > 0)
                    {
                        res = stringLength.MinimumLength;
                    }
                    else if (Property.GetCustomAttribute<RequiredAttribute>() != null)
                    {
                        res = 1;
                    }
                    else if (Property.GetCustomAttribute<StringLengthAttribute>() is StringLengthAttribute stringLength2)
                    {
                        res = stringLength2.MinimumLength;
                    }
                }
            return res ?? defaultValue;
        }

        /// <inheritdoc/>
        public virtual long GetMaxLen(long defaultValue)
        {
            long? res = Attribute.MaxLen;
            if (res == null && Property != null)
                if (Property.GetCustomAttribute<RangeAttribute>() is RangeAttribute range)
                {
                    res = (long)range.Maximum;
                }
                else if (Property.GetCustomAttribute<CountLimitAttribute>() is CountLimitAttribute countLimit)
                {
                    res = countLimit.Max;
                }
                else if (Property.GetCustomAttribute<MaxLengthAttribute>() is MaxLengthAttribute maxLength)
                {
                    res = maxLength.Length;
                }
                else if (Property.PropertyType == typeof(string) && Property.GetCustomAttribute<StringLengthAttribute>() is StringLengthAttribute stringLength)
                {
                    res = stringLength.MaximumLength;
                }
            return res ?? defaultValue;
        }
    }
}
