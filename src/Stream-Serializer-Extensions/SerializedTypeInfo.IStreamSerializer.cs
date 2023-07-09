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
        public void Serialize(ISerializationContext context)
        {
            context.Stream.Write((byte)VERSION, context)
                .Write((byte)(ObjectType.ContainsAllFlags(ObjectTypes.Serializable) && StreamSerializer.TypeCacheEnabled ? ObjectType | ObjectTypes.Cached : ObjectType), context);
            if (ObjectType.ContainsAllFlags(ObjectTypes.Serializable) && StreamSerializer.TypeCacheEnabled)
            {
                context.Stream.Write(ToClrType().GetHashCode(), context);
                return;
            }
            else if (Name != null)
            {
                context.Stream.WriteString(Name, context);
            }
            if (ObjectType.IsGeneric())
            {
                int len = IsGenericTypeDefinition ? GenericParameterCount : GenericParameters.Count;
                context.Stream.Write((sbyte)(IsGenericTypeDefinition ? -len : len), context);
                if (!IsGenericTypeDefinition) for (int i = 0; i < len; context.Stream.WriteSerialized(GenericParameters[i], context), i++) ;
            }
            else if (ObjectType.IsArray())
            {
                context.Stream.WriteSerialized(ElementType!, context);
                if (!ObjectType.IsNotRanked()) context.Stream.Write((byte)(ArrayRank - 1), context);
            }
        }

        /// <inheritdoc/>
        public async Task SerializeAsync(ISerializationContext context)
        {
            await context.Stream.WriteAsync((byte)VERSION, context).DynamicContext();
            await context.Stream.WriteAsync(
                (byte)(ObjectType.ContainsAllFlags(ObjectTypes.Serializable) && StreamSerializer.TypeCacheEnabled ? ObjectType | ObjectTypes.Cached : ObjectType),
                context
                )
                .DynamicContext();
            if (ObjectType.ContainsAllFlags(ObjectTypes.Serializable) && StreamSerializer.TypeCacheEnabled)
            {
                await context.Stream.WriteAsync(ToClrType().GetHashCode(), context).DynamicContext();
                return;
            }
            else if (Name != null)
            {
                await context.Stream.WriteStringAsync(Name, context).DynamicContext();
            }
            if (ObjectType.IsGeneric())
            {
                int len = IsGenericTypeDefinition ? GenericParameterCount : GenericParameters.Count;
                await context.Stream.WriteAsync((sbyte)(IsGenericTypeDefinition ? -len : len), context).DynamicContext();
                if (!IsGenericTypeDefinition)
                    for (int i = 0; i < len; i++)
                        await context.Stream.WriteSerializedAsync(GenericParameters[i], context).DynamicContext();
            }
            else if (ObjectType.IsArray())
            {
                await context.Stream.WriteSerializedAsync(ElementType!, context).DynamicContext();
                if (!ObjectType.IsNotRanked()) await context.Stream.WriteAsync((byte)(ArrayRank - 1), context).DynamicContext();
            }
        }

        /// <inheritdoc/>
        public void Deserialize(IDeserializationContext context)
        {
            if (context.SerializerVersion < 3) throw new SerializerException($"CLR type reading isn't available in version {context.SerializerVersion}", new InvalidDataException());
            if (SerializerVersion != null || ClrType != null) throw new SerializerException("Not a fresh instance", new InvalidOperationException());
            SerializerVersion = context.Version;
            if (ObjectType == ObjectTypes.Null)
            {
                SerializedObjectVersion = context.Stream.ReadOneByte(context);
                if (SerializedObjectVersion == 0) throw new SerializerException($"Trying to deserialize NULL type information", new InvalidDataException());
                if (SerializedObjectVersion > VERSION)
                    throw new SerializerException($"Unsupported object version {SerializedObjectVersion} (max. supported version is {VERSION})", new InvalidDataException());
                ObjectType = (ObjectTypes)context.Stream.ReadOneByte(context);
            }
            switch (ObjectType.RemoveFlags())
            {
                case ObjectTypes.Serializable:
                    if (StreamSerializer.TypeCacheEnabled && ObjectType.ContainsAllFlags(ObjectTypes.Cached))
                    {
                        int thc = context.Stream.ReadInt(context);
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
                        Name = context.Stream.ReadString(context, minLen: 1, maxLen: short.MaxValue);
                    }
                    break;
                case ObjectTypes.Stream:
                case ObjectTypes.Struct:
                case ObjectTypes.Object:
                    Name = context.Stream.ReadString(context, minLen: 1, maxLen: short.MaxValue);
                    break;
            }
            if (ObjectType.IsGeneric())
            {
                if (Recursion >= SerializerContextBase.MaxRecursion)
                    throw new SerializerException($"Avoided possible endless recursion (possibly manipulated byte sequence)", new StackOverflowException());
                int len = context.Stream.ReadOneSByte(context);
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
                        info.Deserialize(context);
                        list.Add(info);
                    }
                    GenericParameters = list.AsReadOnly();
                }
            }
            else if (ObjectType.IsArray())
            {
                if (Recursion >= SerializerContextBase.MaxRecursion)
                    throw new SerializerException($"Avoided possible endless recursion (possibly manipulated byte sequence)", new StackOverflowException());
                ElementType = new()
                {
                    Recursion = Recursion + 1
                };
                ElementType.Deserialize(context);
                if (!ObjectType.IsNotRanked())
                {
                    ArrayRank = context.Stream.ReadOneByte(context) + 1;
                    if (ArrayRank == 0) throw new SerializerException("No array rank", new InvalidDataException());
                }
                else
                {
                    ArrayRank = 1;
                }
            }
        }

        /// <inheritdoc/>
        public async Task DeserializeAsync(IDeserializationContext context)
        {
            if (context.SerializerVersion < 3) throw new SerializerException($"CLR type reading isn't available in version {context.SerializerVersion}", new InvalidDataException());
            if (SerializerVersion != null || ClrType != null) throw new SerializerException("Not a fresh instance", new InvalidOperationException());
            SerializerVersion = context.Version;
            if (ObjectType == ObjectTypes.Null)
            {
                SerializedObjectVersion = await context.Stream.ReadOneByteAsync(context).DynamicContext();
                if (SerializedObjectVersion == 0) throw new SerializerException($"Trying to deserialize NULL type information", new InvalidDataException());
                if (SerializedObjectVersion > VERSION)
                    throw new SerializerException($"Unsupported object version {SerializedObjectVersion} (max. supported version is {VERSION})", new InvalidDataException());
                ObjectType = (ObjectTypes)await context.Stream.ReadOneByteAsync(context).DynamicContext();
            }
            switch (ObjectType.RemoveFlags())
            {
                case ObjectTypes.Serializable:
                    if (StreamSerializer.TypeCacheEnabled && ObjectType.ContainsAllFlags(ObjectTypes.Cached))
                    {
                        int thc = await context.Stream.ReadIntAsync(context).DynamicContext();
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
                        Name = await context.Stream.ReadStringAsync(context, minLen: 1, maxLen: short.MaxValue).DynamicContext();
                    }
                    break;
                case ObjectTypes.Stream:
                case ObjectTypes.Struct:
                case ObjectTypes.Object:
                    Name = await context.Stream.ReadStringAsync(context, minLen: 1, maxLen: short.MaxValue).DynamicContext();
                    break;
            }
            if (ObjectType.IsGeneric())
            {
                if (Recursion >= SerializerContextBase.MaxRecursion)
                    throw new SerializerException($"Avoided possible endless recursion (possibly manipulated byte sequence)", new StackOverflowException());
                int len = await context.Stream.ReadOneSByteAsync(context).DynamicContext();
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
                        await info.DeserializeAsync(context).DynamicContext();
                        list.Add(info);
                    }
                    GenericParameters = list.AsReadOnly();
                }
            }
            else if (ObjectType.IsArray())
            {
                if (Recursion >= SerializerContextBase.MaxRecursion)
                    throw new SerializerException($"Avoided possible endless recursion (possibly manipulated byte sequence)", new StackOverflowException());
                ElementType = new()
                {
                    Recursion = Recursion + 1
                };
                await ElementType.DeserializeAsync(context).DynamicContext();
                if (!ObjectType.IsNotRanked())
                {
                    ArrayRank = await context.Stream.ReadOneByteAsync(context).DynamicContext() + 1;
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
        /// <param name="context">Context</param>
        /// <param name="objVersion">Object version</param>
        /// <param name="objType">Object type</param>
        /// <returns>Instance</returns>
        public static SerializedTypeInfo From(IDeserializationContext context, int objVersion, ObjectTypes objType = ObjectTypes.Null)
        {
            SerializedTypeInfo res = new()
            {
                SerializedObjectVersion = objVersion,
                ObjectType = objType == ObjectTypes.Null ? (ObjectTypes)context.Stream.ReadOneByte(context) : objType
            };
            res.Deserialize(context);
            return res;
        }

        /// <summary>
        /// Deserialize with a pre-red object version (and type)
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="objVersion">Object version</param>
        /// <param name="objType">Object type</param>
        /// <returns>Instance</returns>
        public static async Task<SerializedTypeInfo> FromAsync(IDeserializationContext context, int objVersion, ObjectTypes objType = ObjectTypes.Null)
        {
            SerializedTypeInfo res = new()
            {
                SerializedObjectVersion = objVersion,
                ObjectType = objType == ObjectTypes.Null ? (ObjectTypes)await context.Stream.ReadOneByteAsync(context).DynamicContext() : objType
            };
            await res.DeserializeAsync(context).DynamicContext();
            return res;
        }
    }
}
