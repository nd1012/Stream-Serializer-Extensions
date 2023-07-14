using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using wan24.Core;
using wan24.ObjectValidation;

namespace wan24.StreamSerializerExtensions
{
    /// <summary>
    /// Serialized type informations
    /// </summary>
    public sealed partial class SerializedTypeInfo : ValidatableObjectBase
    {
        /// <summary>
        /// Cache
        /// </summary>
        private static readonly ConcurrentDictionary<int, SerializedTypeInfo> Cache = new();

        /// <summary>
        /// CLR type
        /// </summary>
        private Type? ClrType = null;
        /// <summary>
        /// As string
        /// </summary>
        private string? AsString = null;

        /// <summary>
        /// Constructor
        /// </summary>
        public SerializedTypeInfo() : base() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">Type</param>
        public SerializedTypeInfo(Type type) : base() => SetTypeInfo(type);

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">Object type</param>
        public SerializedTypeInfo(ObjectTypes type) : base() => ObjectType = type;

        /// <summary>
        /// Object type
        /// </summary>
        [AllowedValues(
            ObjectTypes.Bool,
            ObjectTypes.Byte,
            ObjectTypes.Byte | ObjectTypes.Unsigned,
            ObjectTypes.Short,
            ObjectTypes.Short | ObjectTypes.Unsigned,
            ObjectTypes.Int,
            ObjectTypes.Int | ObjectTypes.Unsigned,
            ObjectTypes.Long,
            ObjectTypes.Long | ObjectTypes.Unsigned,
            ObjectTypes.Float,
            ObjectTypes.Double,
            ObjectTypes.Decimal,
            ObjectTypes.Bytes,
            ObjectTypes.String,
            ObjectTypes.List | ObjectTypes.Generic,
            ObjectTypes.Dict | ObjectTypes.Generic,
            ObjectTypes.Object,
            ObjectTypes.Object | ObjectTypes.Generic,
            ObjectTypes.Struct,
            ObjectTypes.Struct | ObjectTypes.Generic,
            ObjectTypes.Serializable,
            ObjectTypes.Serializable | ObjectTypes.Generic,
            ObjectTypes.Stream,
            ObjectTypes.Array,
            ObjectTypes.Array | ObjectTypes.NoRank,
            ObjectTypes.ClrType
            )]
        public ObjectTypes ObjectType { get; private set; }

        /// <summary>
        /// Name
        /// </summary>
        [StringLength(short.MaxValue, MinimumLength = 1)]
        [RequiredIf(
            nameof(ObjectType),
            ObjectTypes.Object,
            ObjectTypes.Object | ObjectTypes.Generic,
            ObjectTypes.Struct,
            ObjectTypes.Struct | ObjectTypes.Generic,
            ObjectTypes.Stream,
            ObjectTypes.Stream | ObjectTypes.Generic,
            ObjectTypes.Serializable,
            ObjectTypes.Serializable | ObjectTypes.Generic
            )]
        public string? Name { get; private set; }

        /// <summary>
        /// Is a generic type definition?
        /// </summary>
        public bool IsGenericTypeDefinition { get; private set; }

        /// <summary>
        /// Generic parameters
        /// </summary>
        [CountLimit(sbyte.MaxValue)]
        [RequiredIf(
            nameof(ObjectType),
            ObjectTypes.Object | ObjectTypes.Generic,
            ObjectTypes.Struct | ObjectTypes.Generic,
            ObjectTypes.Stream | ObjectTypes.Generic,
            ObjectTypes.Serializable | ObjectTypes.Generic,
            ObjectTypes.List | ObjectTypes.Generic,
            ObjectTypes.Dict | ObjectTypes.Generic
            )]
        public ReadOnlyCollection<SerializedTypeInfo> GenericParameters { get; private set; } = new List<SerializedTypeInfo>().AsReadOnly();

        /// <summary>
        /// Generic parameter count of a generic type definition
        /// </summary>
        [Range(0, sbyte.MaxValue)]
        [RequiredIf(nameof(IsGenericTypeDefinition), true)]
        public int GenericParameterCount { get; private set; }

        /// <summary>
        /// Array element type
        /// </summary>
        [RequiredIf(nameof(ObjectType), ObjectTypes.Array, ObjectTypes.Array | ObjectTypes.NoRank)]
        public SerializedTypeInfo? ElementType { get; private set; }

        /// <summary>
        /// Array rank
        /// </summary>
        [Range(0, byte.MaxValue)]
        [RequiredIf(nameof(ElementType))]
        public int ArrayRank { get; private set; }

        /// <summary>
        /// Determine if the CLR type is known
        /// </summary>
        [NoValidation]
        public bool IsKnown
        {
            get
            {
                try
                {
                    if (ClrType != null) return true;
                    if ((ClrType = TypeHelper.Instance.GetType(ToString())) == null) return false;
                    Cache.TryAdd(ClrType.GetHashCode(), this);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Determine if the type is serializable
        /// </summary>
        [NoValidation]
        public bool IsSerializable => IsKnown && StreamSerializer.IsTypeAllowed(ClrType!);

        /// <summary>
        /// Determine if the type information if for a basic type (will serialize to only one byte)
        /// </summary>
        [NoValidation]
        public bool IsBasicType => Name == null && ElementType == null && !IsGenericTypeDefinition && GenericParameters.Count == 0;

        /// <summary>
        /// Get as CLR type
        /// </summary>
        /// <returns>CLR type</returns>
        public Type ToClrType()
        {
            ClrType ??= TypeHelper.Instance.GetType(ToString()) ?? throw new TypeLoadException($"{AsString} is not available");
            Cache.TryAdd(ClrType.GetHashCode(), this);
            return ClrType;
        }

        /// <summary>
        /// Get as serializabe type (ensures, that the type can be (de)serialized)
        /// </summary>
        /// <returns>Serializable type</returns>
        public Type ToSerializableType()
        {
            Type res = ClrType == null
                ? ClrType = StreamSerializer.LoadType(ToString())
                : StreamSerializer.IsTypeAllowed(ClrType)
                    ? ClrType
                    : throw new InvalidOperationException(
                        $"\"{ToString()}\" is not an allowed deserializable type (see {nameof(StreamSerializer)}.{nameof(StreamSerializer.AllowedTypes)})"
                        );
            Cache.TryAdd(res.GetHashCode(), this);
            return res;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            string ToStringInt()
            {
                switch (ObjectType.RemoveFlags())
                {
                    case ObjectTypes.Bool: return typeof(bool).ToString();
                    case ObjectTypes.Byte: return ObjectType.IsUnsigned() ? typeof(byte).ToString() : typeof(sbyte).ToString();
                    case ObjectTypes.Short: return ObjectType.IsUnsigned() ? typeof(ushort).ToString() : typeof(short).ToString();
                    case ObjectTypes.Int: return ObjectType.IsUnsigned() ? typeof(uint).ToString() : typeof(int).ToString();
                    case ObjectTypes.Long: return ObjectType.IsUnsigned() ? typeof(ulong).ToString() : typeof(long).ToString();
                    case ObjectTypes.Float: return typeof(float).ToString();
                    case ObjectTypes.Double: return typeof(double).ToString();
                    case ObjectTypes.Decimal: return typeof(decimal).ToString();
                    case ObjectTypes.Bytes: return typeof(byte[]).ToString();
                    case ObjectTypes.String: return typeof(string).ToString();
                    case ObjectTypes.Array:
                        if (ElementType == null) throw new InvalidDataException("Missing array element type");
                        return $"{ElementType}[{(ArrayRank <= 1 ? string.Empty : new string(Enumerable.Repeat(',', ArrayRank - 1).ToArray()))}]";
                    case ObjectTypes.Dict:
                        return IsGenericTypeDefinition
                            ? $"{typeof(Dictionary<,>).Namespace}.{typeof(Dictionary<,>).Name}"
                            : typeof(Dictionary<,>).MakeGenericType(GenericParameters.Select(i => i.ToClrType()).ToArray()).ToString();
                    case ObjectTypes.List:
                        return IsGenericTypeDefinition
                            ? $"{typeof(List<>).Namespace}.{typeof(List<>).Name}"
                            : typeof(List<>).MakeGenericType(GenericParameters.Select(i => i.ToClrType()).ToArray()).ToString();
                    case ObjectTypes.ClrType: return typeof(Type).ToString();
                }
                if (Name == null) throw new InvalidDataException($"Missing type name for {ObjectType}");
                if (!ObjectType.IsGeneric()) return Name;
                if (IsGenericTypeDefinition) return $"{Name}`{GenericParameters.Count}";
                StringBuilder sb = new($"{Name}`{GenericParameters.Count}[");
                foreach (SerializedTypeInfo i in GenericParameters)
                {
                    sb.Append(i.ToString());
                    sb.Append(',');
                }
                sb.Remove(sb.Length - 1, 1);
                sb.Append(']');
                return sb.ToString();
            }
            return AsString ??= ToStringInt();
        }

        /// <summary>
        /// Set the instance properties from a type
        /// </summary>
        /// <param name="type">Type</param>
        private void SetTypeInfo(Type type)
        {
            ArgumentValidationHelper.EnsureValidArgument(nameof(type), type);
            ClrType = type;
            ObjectType = type.GetObjectType() & ~ObjectTypes.CachedSerializable;
            switch (ObjectType.RemoveFlags())
            {
                case ObjectTypes.Serializable:
                case ObjectTypes.Stream:
                case ObjectTypes.Struct:
                case ObjectTypes.Object:
                    Name = $"{type.Namespace}.{type.Name}";
                    break;
            }
            if (type.IsGenericType)
            {
                if (Recursion >= SerializerContext.MaxRecursion) throw new InvalidOperationException("Type information branches too deep and won't be deserializable");
                IsGenericTypeDefinition = type.IsGenericTypeDefinition;
                if (IsGenericTypeDefinition)
                {
                    GenericParameterCount = type.GetGenericArguments().Length;
                }
                else
                {
                    GenericParameters = type.GetGenericArgumentsCached()
                        .Select(t => new SerializedTypeInfo(t) { Recursion = Recursion + 1 })
                        .ToList()
                        .AsReadOnly();
                }
            }
            else if (type.IsArray)
            {
                if (Recursion >= SerializerContext.MaxRecursion) throw new InvalidOperationException("Type information branches too deep and won't be deserializable");
                ElementType = From(type.GetElementType()!);
                ElementType.Recursion = Recursion + 1;
                ArrayRank = type.GetArrayRank();
            }
            Cache.TryAdd(type.GetHashCode(), this);
        }

        /// <summary>
        /// Cast as <see cref="Type"/>
        /// </summary>
        /// <param name="info"><see cref="SerializedTypeInfo"/></param>
        public static implicit operator Type(SerializedTypeInfo info) => info.ToClrType();

        /// <summary>
        /// Cast as serializable flag
        /// </summary>
        /// <param name="info"><see cref="SerializedTypeInfo"/></param>
        public static implicit operator bool(SerializedTypeInfo info) => info.IsSerializable;

        /// <summary>
        /// Cast from <see cref="Type"/>
        /// </summary>
        /// <param name="type"><see cref="Type"/></param>
        public static implicit operator SerializedTypeInfo(Type type) => From(type);

        /// <summary>
        /// Get an instance
        /// </summary>
        /// <param name="type"></param>
        /// <returns>Instance</returns>
        public static SerializedTypeInfo From(Type type) => Cache.TryGetValue(type.GetHashCode(), out SerializedTypeInfo? res) ? res : new(type);
    }
}
