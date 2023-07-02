using System.ComponentModel.DataAnnotations;
using System.Text;
using wan24.Core;
using wan24.ObjectValidation;

namespace wan24.StreamSerializerExtensions
{
    /// <summary>
    /// Serialized type informations
    /// </summary>
    public sealed class SerializedTypeInfo : ValidatableObjectBase, IStreamSerializerVersion
    {
        /// <summary>
        /// Object version
        /// </summary>
        public const int VERSION = 1;

        /// <summary>
        /// Recursion level (used to avoid a forced endless recursion using a manipulated byte sequence)
        /// </summary>
        private int Recursion = 0;
        /// <summary>
        /// CLR type
        /// </summary>
        private Type? ClrType = null;
        /// <summary>
        /// As string
        /// </summary>
        private string? AsString = null;
        /// <summary>
        /// Serialized object version
        /// </summary>
        private int? SerializedObjectVersion = null;
        /// <summary>
        /// Serializer version
        /// </summary>
        private int? SerializerVersion = null;

        /// <summary>
        /// Constructor
        /// </summary>
        public SerializedTypeInfo() : base() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">Type</param>
        public SerializedTypeInfo(Type type) : base()
        {
            ClrType = type;
            ObjectType = type.GetObjectType();
            switch (ObjectType.RemoveFlags())
            {
                case ObjectTypes.Stream:
                case ObjectTypes.Struct:
                case ObjectTypes.Object:
                    Name = $"{type.Namespace}.{type.Name}";
                    break;
            }
            if (type.IsGenericType)
            {
                if (Recursion >= 32) throw new InvalidOperationException("Type information branches too deep and won't be deserializable");
                IsGenericTypeDefinition = type.IsGenericTypeDefinition;
                if (IsGenericTypeDefinition)
                {
                    GenericParameterCount = type.GetGenericArguments().Length;
                }
                else
                {
                    GenericParameters = type.GetGenericArgumentsCached()
                        .Select(t => new SerializedTypeInfo(t) { Recursion = Recursion + 1 })
                        .ToArray();
                }
            }
            else if (type.IsArray)
            {
                if (Recursion >= 32) throw new InvalidOperationException("Type information branches too deep and won't be deserializable");
                ElementType = new(type.GetElementType()!) { Recursion = Recursion + 1 };
                ArrayRank = type.GetArrayRank();
            }
        }

        /// <summary>
        /// Object type
        /// </summary>
        public ObjectTypes ObjectType { get; private set; }

        /// <summary>
        /// Name
        /// </summary>
        [StringLength(short.MaxValue, MinimumLength = 1)]
        public string? Name { get; private set; }

        /// <summary>
        /// Is a generic type definition?
        /// </summary>
        public bool IsGenericTypeDefinition { get; private set; }

        /// <summary>
        /// Generic parameters
        /// </summary>
        [CountLimit(sbyte.MaxValue)]
        public SerializedTypeInfo[] GenericParameters { get; private set; } = Array.Empty<SerializedTypeInfo>();

        /// <summary>
        /// Generic parameter count of a generic type definition
        /// </summary>
        [Range(0, sbyte.MaxValue)]
        public int GenericParameterCount { get; private set; }

        /// <summary>
        /// Array element type
        /// </summary>
        public SerializedTypeInfo? ElementType { get; private set; }

        /// <summary>
        /// Array rank
        /// </summary>
        [Range(0, byte.MaxValue)]
        public int ArrayRank { get; private set; }

        /// <summary>
        /// Determine if the type is serializable
        /// </summary>
        public bool IsSerializable => StreamSerializer.IsTypeAllowed(ToClrType());

        /// <inheritdoc/>
        int? IStreamSerializerVersion.ObjectVersion => VERSION;

        /// <inheritdoc/>
        int? IStreamSerializerVersion.SerializedObjectVersion => SerializedObjectVersion;

        /// <inheritdoc/>
        int? IStreamSerializerVersion.SerializerVersion => SerializerVersion;

        /// <summary>
        /// Get as CLR type
        /// </summary>
        /// <returns>CLR type</returns>
        public Type ToClrType() => ClrType ??= TypeHelper.Instance.GetType(ToString()) ?? throw new TypeLoadException($"{ToString()} is not available");

        /// <summary>
        /// Get as serializabe type (ensures, that the type can be (de)serialized)
        /// </summary>
        /// <returns>Serializable type</returns>
        public Type ToSerializableType() => ClrType == null
            ? ClrType = StreamSerializer.LoadType(ToString())
            : StreamSerializer.IsTypeAllowed(ClrType)
                ? ClrType
                : throw new SerializerException($"Failed to load type \"{ToString()}\"");

        /// <inheritdoc/>
        public void Serialize(Stream stream)
        {
            stream.WriteNumberNullable(VERSION)
                .Write((byte)ObjectType);
            if (Name != null) stream.WriteString(Name);
            if (ObjectType.IsGeneric())
            {
                int len = IsGenericTypeDefinition ? GenericParameterCount : GenericParameters.Length;
                stream.Write((sbyte)(IsGenericTypeDefinition ? -len : len));
                if (!IsGenericTypeDefinition) for (int i = 0; i < len; stream.WriteSerialized(GenericParameters[i]), i++) ;
            }
            else if (ObjectType.IsArray())
            {
                stream.WriteSerialized(ElementType!);
                if (!ObjectType.IsNotRanked()) stream.Write((byte)(ArrayRank - 1));
            }
        }

        /// <inheritdoc/>
        public async Task SerializeAsync(Stream stream, CancellationToken cancellationToken)
        {
            await stream.WriteNumberNullableAsync(VERSION, cancellationToken).DynamicContext();
            await stream.WriteAsync((byte)ObjectType, cancellationToken).DynamicContext();
            if (Name != null) await stream.WriteStringAsync(Name, cancellationToken).DynamicContext();
            if (ObjectType.IsGeneric())
            {
                int len = IsGenericTypeDefinition ? GenericParameterCount : GenericParameters.Length;
                await stream.WriteAsync((sbyte)(IsGenericTypeDefinition ? -len : len), cancellationToken).DynamicContext();
                if (!IsGenericTypeDefinition)
                    for (int i = 0; i < len; i++)
                        await stream.WriteSerializedAsync(GenericParameters[i], cancellationToken).DynamicContext();
            }
            else if (ObjectType.IsArray())
            {
                await stream.WriteSerializedAsync(ElementType!, cancellationToken).DynamicContext();
                if (!ObjectType.IsNotRanked()) await stream.WriteAsync((byte)(ArrayRank - 1), cancellationToken).DynamicContext();
            }
        }

        /// <inheritdoc/>
        public void Deserialize(Stream stream, int version)
        {
            SerializerVersion = version;
            if (ObjectType == ObjectTypes.Null)
            {
                SerializedObjectVersion = stream.ReadNumberNullable<int>(version) ?? throw new SerializerException($"Invalid object version", new InvalidDataException());
                if (SerializedObjectVersion > VERSION)
                    throw new SerializerException($"Unsupported object version {SerializedObjectVersion} (max. supported version is {VERSION})", new InvalidDataException());
                ObjectType = (ObjectTypes)stream.ReadOneByte(version);
            }
            switch (ObjectType.RemoveFlags())
            {
                case ObjectTypes.Stream:
                case ObjectTypes.Struct:
                case ObjectTypes.Object:
                    Name = stream.ReadString(version, minLen: 1, maxLen: short.MaxValue);
                    break;
            }
            if (ObjectType.IsGeneric())
            {
                if (Recursion >= 32) throw new SerializerException($"Avoided endless recursion (possibly manipulated byte sequence)", new StackOverflowException());
                int len = stream.ReadOneSByte(version);
                IsGenericTypeDefinition = len < 0;
                if (len < 0) len = -len;
                SerializerHelper.EnsureValidLength(len, 1);
                if (IsGenericTypeDefinition)
                {
                    GenericParameterCount = len;
                }
                else
                {
                    GenericParameters = new SerializedTypeInfo[len];
                    for (int i = 0; i < len; i++)
                    {
                        GenericParameters[i] = new()
                        {
                            Recursion = Recursion + 1
                        };
                        GenericParameters[i].Deserialize(stream, version);
                    }
                }
            }
            else if (ObjectType.IsArray())
            {
                if (Recursion >= 32) throw new SerializerException($"Avoided endless recursion (possibly manipulated byte sequence)", new StackOverflowException());
                ElementType = new()
                {
                    Recursion = Recursion + 1
                };
                ElementType.Deserialize(stream, version);
                if (!ObjectType.IsNotRanked())
                {
                    ArrayRank = stream.ReadOneByte() + 1;
                    if (ArrayRank == 0) throw new SerializerException("No array rank", new InvalidDataException());
                }
                else
                {
                    ArrayRank = 1;
                }
            }
        }

        /// <inheritdoc/>
        public async Task DeserializeAsync(Stream stream, int version, CancellationToken cancellationToken)
        {
            SerializerVersion = version;
            if (ObjectType == ObjectTypes.Null)
            {
                SerializedObjectVersion = await stream.ReadNumberNullableAsync<int>(version, cancellationToken: cancellationToken).DynamicContext()
                    ?? throw new SerializerException($"Invalid object version", new InvalidDataException());
                if (SerializedObjectVersion > VERSION)
                    throw new SerializerException($"Unsupported object version {SerializedObjectVersion} (max. supported version is {VERSION})", new InvalidDataException());
                ObjectType = (ObjectTypes)await stream.ReadOneByteAsync(version, cancellationToken).DynamicContext();
            }
            switch (ObjectType.RemoveFlags())
            {
                case ObjectTypes.Stream:
                case ObjectTypes.Struct:
                case ObjectTypes.Object:
                    Name = await stream.ReadStringAsync(version, minLen: 1, maxLen: short.MaxValue, cancellationToken: cancellationToken).DynamicContext();
                    break;
            }
            if (ObjectType.IsGeneric())
            {
                if (Recursion >= 32) throw new SerializerException($"Avoided endless recursion (possibly manipulated byte sequence)", new StackOverflowException());
                int len = await stream.ReadOneSByteAsync(version, cancellationToken).DynamicContext();
                IsGenericTypeDefinition = len < 0;
                if (len < 0) len = -len;
                SerializerHelper.EnsureValidLength(len, 1);
                if (IsGenericTypeDefinition)
                {
                    GenericParameterCount = len;
                }
                else
                {
                    GenericParameters = new SerializedTypeInfo[len];
                    for (int i = 0; i < len; i++)
                    {
                        GenericParameters[i] = new()
                        {
                            Recursion = Recursion + 1
                        };
                        await GenericParameters[i].DeserializeAsync(stream, version, cancellationToken).DynamicContext();
                    }
                }
            }
            else if (ObjectType.IsArray())
            {
                if (Recursion >= 32) throw new SerializerException($"Avoided endless recursion (possibly manipulated byte sequence)", new StackOverflowException());
                ElementType = new()
                {
                    Recursion = Recursion + 1
                };
                await ElementType.DeserializeAsync(stream, version, cancellationToken).DynamicContext();
                if (!ObjectType.IsNotRanked())
                {
                    ArrayRank = await stream.ReadOneByteAsync(version, cancellationToken).DynamicContext() + 1;
                    if (ArrayRank == 0) throw new SerializerException("No array rank", new InvalidDataException());
                }
                else
                {
                    ArrayRank = 1;
                }
            }
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
                    case ObjectTypes.Array: return $"{ElementType ?? throw new InvalidDataException("Missing array element type")}[{(ArrayRank <= 1 ? string.Empty : new string(Enumerable.Repeat(',', ArrayRank - 1).ToArray()))}]";
                    case ObjectTypes.Dict:
                        return IsGenericTypeDefinition
                            ? $"{typeof(Dictionary<,>).Namespace}.{typeof(Dictionary<,>).Name}"
                            : typeof(Dictionary<,>).MakeGenericType(GenericParameters.Select(i => i.ToClrType()).ToArray()).ToString();
                    case ObjectTypes.List:
                        return IsGenericTypeDefinition
                            ? $"{typeof(List<>).Namespace}.{typeof(List<>).Name}"
                            : typeof(List<>).MakeGenericType(GenericParameters.Select(i => i.ToClrType()).ToArray()).ToString();
                }
                if (Name == null) throw new InvalidDataException($"Missing type name for {ObjectType}");
                if (!ObjectType.IsGeneric()) return Name;
                if (IsGenericTypeDefinition) return $"{Name}`{GenericParameters.Length}";
                StringBuilder sb = new($"{Name}`{GenericParameters.Length}[");
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
        /// Cast as <see cref="Type"/>
        /// </summary>
        /// <param name="info">Info</param>
        public static implicit operator Type(SerializedTypeInfo info) => info.ToClrType();

        /// <summary>
        /// Cast as serializable flag
        /// </summary>
        /// <param name="info">Info</param>
        public static implicit operator bool(SerializedTypeInfo info) => info.IsSerializable;

        /// <summary>
        /// Cast as serialized data
        /// </summary>
        /// <param name="info">Info</param>
        public static implicit operator byte[](SerializedTypeInfo info) => info.ToBytes();

        /// <summary>
        /// Cast from serialized data
        /// </summary>
        /// <param name="data">Data</param>
        public static implicit operator SerializedTypeInfo(byte[] data) => data.ToObject<SerializedTypeInfo>();

        /// <summary>
        /// Deserialize with a pre-red object version (and type)
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="objVersion">Object version</param>
        /// <param name="objType">Object type</param>
        /// <returns>Instance</returns>
        public static SerializedTypeInfo From(Stream stream, int version, int objVersion, ObjectTypes objType = ObjectTypes.Null)
        {
            SerializedTypeInfo res = new()
            {
                SerializedObjectVersion = objVersion,
                ObjectType = objType == ObjectTypes.Null ? (ObjectTypes)stream.ReadOneByte(version) : objType
            };
            res.Deserialize(stream, version);
            return res;
        }

        /// <summary>
        /// Deserialize with a pre-red object version (and type)
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="objVersion">Object version</param>
        /// <param name="objType">Object type</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Instance</returns>
        public static async Task<SerializedTypeInfo> FromAsync(
            Stream stream,
            int version,
            int objVersion,
            ObjectTypes objType = ObjectTypes.Null,
            CancellationToken cancellationToken = default
            )
        {
            SerializedTypeInfo res = new()
            {
                SerializedObjectVersion = objVersion,
                ObjectType = objType == ObjectTypes.Null ? (ObjectTypes)await stream.ReadOneByteAsync(version, cancellationToken).DynamicContext() : objType
            };
            await res.DeserializeAsync(stream, version, cancellationToken).DynamicContext();
            return res;
        }
    }
}
