using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    // IStreamSerializer
    public sealed partial class SerializedTypeInfo : IStreamSerializerVersion
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
        /// Serialized object version
        /// </summary>
        private int? SerializedObjectVersion = null;
        /// <summary>
        /// Serializer version
        /// </summary>
        private int? SerializerVersion = null;

        /// <inheritdoc/>
        int? IStreamSerializerVersion.ObjectVersion => VERSION;

        /// <inheritdoc/>
        int? IStreamSerializerVersion.SerializedObjectVersion => SerializedObjectVersion;

        /// <inheritdoc/>
        int? IStreamSerializerVersion.SerializerVersion => SerializerVersion;

        /// <inheritdoc/>
        public void Serialize(Stream stream)
        {
            stream.WriteNumberNullable(VERSION)
                .Write((byte)(ObjectType.ContainsAllFlags(ObjectTypes.Serializable) && StreamSerializer.TypeCacheEnabled ? ObjectType | ObjectTypes.Cached : ObjectType));
            if (ObjectType.ContainsAllFlags(ObjectTypes.Serializable) && StreamSerializer.TypeCacheEnabled)
            {
                stream.Write(ToClrType().GetHashCode());
                return;
            }
            else if (Name != null)
            {
                stream.WriteString(Name);
            }
            if (ObjectType.IsGeneric())
            {
                int len = IsGenericTypeDefinition ? GenericParameterCount : GenericParameters.Count;
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
            await stream.WriteAsync(
                (byte)(ObjectType.ContainsAllFlags(ObjectTypes.Serializable) && StreamSerializer.TypeCacheEnabled ? ObjectType | ObjectTypes.Cached : ObjectType),
                cancellationToken
                )
                .DynamicContext();
            if (ObjectType.ContainsAllFlags(ObjectTypes.Serializable) && StreamSerializer.TypeCacheEnabled)
            {
                await stream.WriteAsync(ToClrType().GetHashCode(), cancellationToken).DynamicContext();
                return;
            }
            else if (Name != null)
            {
                await stream.WriteStringAsync(Name, cancellationToken).DynamicContext();
            }
            if (ObjectType.IsGeneric())
            {
                int len = IsGenericTypeDefinition ? GenericParameterCount : GenericParameters.Count;
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
            if (SerializerVersion != null || ClrType != null) throw new SerializerException("Not a fresh instance", new InvalidOperationException());
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
                case ObjectTypes.Serializable:
                    if (StreamSerializer.TypeCacheEnabled && ObjectType.ContainsAllFlags(ObjectTypes.Cached))
                    {
                        int thc = stream.ReadInt(version);
                        if (!TypeCache.Types.TryGetValue(thc, out Type? type)) throw new SerializerException($"Unknown type #{thc}", new InvalidDataException());
                        if (!typeof(IStreamSerializer).IsAssignableFrom(type))
                            throw new SerializerException($"Invalid type {type} (possibly manipulated byte sequence)", new InvalidDataException());
                        SetTypeInfo(type);
                        return;
                    }
                    else
                    {
                        if (ObjectType.ContainsAllFlags(ObjectTypes.Cached))
                            throw new SerializerException(
                                $"The type cache needs to be enabled in order to be able to deserialize this type information",
                                new InvalidOperationException()
                                );
                        Name = stream.ReadString(version, minLen: 1, maxLen: short.MaxValue);
                    }
                    break;
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
                    List<SerializedTypeInfo> list = new(len);
                    SerializedTypeInfo info;
                    for (int i = 0; i < len; i++)
                    {
                        info = new()
                        {
                            Recursion = Recursion + 1
                        };
                        info.Deserialize(stream, version);
                        list.Add(info);
                    }
                    GenericParameters = list.AsReadOnly();
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
            if (SerializerVersion != null || ClrType != null) throw new SerializerException("Not a fresh instance", new InvalidOperationException());
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
                case ObjectTypes.Serializable:
                    if (StreamSerializer.TypeCacheEnabled && ObjectType.ContainsAllFlags(ObjectTypes.Cached))
                    {
                        int thc = await stream.ReadIntAsync(version, cancellationToken: cancellationToken).DynamicContext();
                        if (!TypeCache.Types.TryGetValue(thc, out Type? type)) throw new SerializerException($"Unknown type #{thc}", new InvalidDataException());
                        if (!typeof(IStreamSerializer).IsAssignableFrom(type))
                            throw new SerializerException($"Invalid type {type} (possibly manipulated byte sequence)", new InvalidDataException());
                        SetTypeInfo(type);
                        return;
                    }
                    else
                    {
                        if (ObjectType.ContainsAllFlags(ObjectTypes.Cached))
                            throw new SerializerException(
                                $"The type cache needs to be enabled in order to be able to deserialize this type information",
                                new InvalidOperationException()
                                );
                        Name = await stream.ReadStringAsync(version, minLen: 1, maxLen: short.MaxValue, cancellationToken: cancellationToken).DynamicContext();
                    }
                    break;
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
                    List<SerializedTypeInfo> list = new(len);
                    SerializedTypeInfo info;
                    for (int i = 0; i < len; i++)
                    {
                        info = new()
                        {
                            Recursion = Recursion + 1
                        };
                        await info.DeserializeAsync(stream, version, cancellationToken).DynamicContext();
                        list.Add(info);
                    }
                    GenericParameters = list.AsReadOnly();
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

        /// <summary>
        /// Cast as serialized data
        /// </summary>
        /// <param name="info"><see cref="SerializedTypeInfo"/></param>
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
