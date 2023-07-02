using System.Runtime;
using System.Runtime.CompilerServices;
using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    // Any
    public static partial class StreamExtensions
    {
        /// <summary>
        /// Read any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="options">Options</param>
        /// <returns>Object</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object ReadAny(this Stream stream, int? version = null, ISerializerOptions? options = null)
            => ReadAnyInt(stream, version, (ObjectTypes)ReadOneByte(stream, version), type: null, options);

        /// <summary>
        /// Read any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="objType">Object type</param>
        /// <param name="type">CLR type</param>
        /// <param name="options">Options</param>
        /// <returns>Object</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static object ReadAnyInt(Stream stream, int? version, ObjectTypes objType, Type? type, ISerializerOptions? options)
            => SerializerException.Wrap(() =>
            {
                if (objType == ObjectTypes.Null) throw new SerializerException("NULL object type is not supported by this method");
                version ??= StreamSerializer.Version;
                bool isEmpty = objType.IsEmpty(),
                    isUnsigned = objType.IsUnsigned(),
                    readType = objType.RequiresTypeName();
                switch (objType.RemoveFlags())
                {
                    case ObjectTypes.Bool:
                        return !isEmpty;
                    case ObjectTypes.Byte:
                        switch (version & byte.MaxValue)// Serializer version switch
                        {
                            case 1:
                            case 2:
                                type = isUnsigned ? typeof(byte) : typeof(sbyte);
                                break;
                            default:
                                return isEmpty
                                    ? Convert.ChangeType(0, isUnsigned ? typeof(byte) : typeof(sbyte))
                                    : isUnsigned
                                        ? ReadOneByte(stream, version)
                                        : ReadOneSByte(stream, version);
                        }
                        break;
                    case ObjectTypes.Short:
                        switch (version & byte.MaxValue)// Serializer version switch
                        {
                            case 1:
                            case 2:
                                type = isUnsigned ? typeof(ushort) : typeof(short);
                                break;
                            default:
                                return isEmpty
                                    ? Convert.ChangeType(0, isUnsigned ? typeof(ushort) : typeof(short))
                                    : isUnsigned
                                        ? ReadUShort(stream, version)
                                        : ReadShort(stream, version);
                        }
                        break;
                    case ObjectTypes.Int:
                    case ObjectTypes.Long:
                    case ObjectTypes.Float:
                    case ObjectTypes.Double:
                    case ObjectTypes.Decimal:
                        type = objType.RemoveFlags() switch
                        {
                            ObjectTypes.Int => isUnsigned ? typeof(uint) : typeof(int),
                            ObjectTypes.Long => isUnsigned ? typeof(ulong) : typeof(long),
                            ObjectTypes.Float => typeof(float),
                            ObjectTypes.Double => typeof(double),
                            ObjectTypes.Decimal => typeof(decimal),
                            _ => throw new InvalidProgramException()
                        };
                        break;
                    case ObjectTypes.String:
                        return isEmpty
                            ? string.Empty
                            : ReadString(
                                stream,
                                version,
                                minLen: options?.GetMinLen(0) ?? 0,
                                maxLen: options?.GetMaxLen(int.MaxValue) ?? int.MaxValue
                                );
                    case ObjectTypes.String16:
                        return isEmpty
                            ? string.Empty
                            : ReadString16(
                                stream,
                                version,
                                minLen: options?.GetMinLen(0) ?? 0,
                                maxLen: options?.GetMaxLen(int.MaxValue) ?? int.MaxValue
                                );
                    case ObjectTypes.String32:
                        return isEmpty
                            ? string.Empty
                            : ReadString32(
                                stream,
                                version,
                                minLen: options?.GetMinLen(0) ?? 0,
                                maxLen: options?.GetMaxLen(int.MaxValue) ?? int.MaxValue
                                );
                    case ObjectTypes.Bytes:
                        return isEmpty
                            ? Array.Empty<byte>()
                            : ReadBytes(
                                stream,
                                version,
                                minLen: options?.GetMinLen(0) ?? 0,
                                maxLen: options?.GetMaxLen(int.MaxValue) ?? int.MaxValue
                                ).Value;
                    case ObjectTypes.Stream:
                        readType = false;
                        break;
                    case ObjectTypes.ClrType:
                        if (isUnsigned)
                        {
                            if(!StreamSerializer.TypeCacheEnabled)
                                throw new SerializerException(
                                    $"The type cache needs to be enabled in order to be able to deserialize this type information",
                                    new InvalidOperationException()
                                    );
                            int thc = stream.ReadInt(version);
                            if (!TypeCache.Types.TryGetValue(thc, out Type? type)) throw new SerializerException($"Unknown type #{thc}", new InvalidDataException());
                            if (!typeof(IStreamSerializer).IsAssignableFrom(type))
                                throw new SerializerException($"Invalid type {type} (possibly manipulated byte sequence)", new InvalidDataException());
                            return type;
                        }
                        return ReadType(stream, version);
                }
                if (readType && type == null) type = StreamSerializer.LoadType(ReadString(stream, version, minLen: 1, maxLen: short.MaxValue));
                switch (objType.RemoveFlags())
                {
                    case ObjectTypes.Byte:
                    case ObjectTypes.Short:
                    case ObjectTypes.Int:
                    case ObjectTypes.Long:
                    case ObjectTypes.Float:
                    case ObjectTypes.Double:
                    case ObjectTypes.Decimal:
                        return isEmpty ? Activator.CreateInstance(type!)! : ReadNumber(stream, type!, version)!;
                    case ObjectTypes.Array:
                        return isEmpty ? ArrayEmptyMethod.MakeGenericMethod(type!.GetElementType()!).InvokeAuto(obj: null)! : ReadArray(
                            stream,
                            type!,
                            version,
                            pool: null,
                            options?.GetMinLen(0) ?? 0,
                            options?.GetMaxLen(int.MaxValue) ?? int.MaxValue,
                            options?.Attribute.GetValueSerializerOptions(property: null, stream, version ?? StreamSerializer.Version, default)
                            );
                    case ObjectTypes.List:
                        return isEmpty ? Activator.CreateInstance(type!)! : ReadList(
                            stream,
                            type!,
                            version,
                            pool: null,
                            options?.GetMinLen(0) ?? 0,
                            options?.GetMaxLen(int.MaxValue) ?? int.MaxValue,
                            options?.Attribute.GetValueSerializerOptions(property: null, stream, version ?? StreamSerializer.Version, default)
                            );
                    case ObjectTypes.Dict:
                        return isEmpty ? Activator.CreateInstance(type!)! : ReadDict(
                            stream,
                            type!,
                            version,
                            pool: null,
                            options?.GetMinLen(0) ?? 0,
                            options?.GetMaxLen(int.MaxValue) ?? int.MaxValue,
                            options?.Attribute.GetKeySerializerOptions(property: null, stream, version ?? StreamSerializer.Version),
                            options?.Attribute.GetValueSerializerOptions(property: null, stream, version ?? StreamSerializer.Version)
                            );
                    case ObjectTypes.Object:
                        return ReadObject(stream, type!, version, options);
                    case ObjectTypes.Struct:
                        return ReadStructMethod.MakeGenericMethod(type!).InvokeAuto(obj: null, stream, version)!;
                    case ObjectTypes.Serializable:
                        return ReadSerializedObject(stream, type!, version);
                    case ObjectTypes.Stream:
                        Stream res = options?.Attribute.GetStream(obj: null, property: null, stream, version ?? StreamSerializer.Version, default) ?? new FileStream(
                            Path.Combine(Settings.TempFolder, Guid.NewGuid().ToString()),
                            FileMode.CreateNew,
                            FileAccess.ReadWrite,
                            FileShare.None,
                            bufferSize: Settings.BufferSize,
                            FileOptions.RandomAccess | FileOptions.DeleteOnClose
                            );
                        if (objType.IsEmpty()) return res;
                        try
                        {
                            return ReadStream(stream, res, version, minLen: options?.GetMinLen(0L) ?? 0, maxLen: options?.GetMaxLen(long.MaxValue) ?? long.MaxValue);
                        }
                        catch
                        {
                            res.Dispose();
                            throw;
                        }
                    default:
                        throw new InvalidProgramException();
                }
            });

        /// <summary>
        /// Read any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="options">Options</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Object</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<object> ReadAnyAsync(this Stream stream, int? version = null, ISerializerOptions? options = null, CancellationToken cancellationToken = default)
            => await ReadAnyIntAsync(stream, version, (ObjectTypes)await ReadOneByteAsync(stream, version, cancellationToken).DynamicContext(), type: null, options, cancellationToken)
                .DynamicContext();

        /// <summary>
        /// Read any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="objType">Object type</param>
        /// <param name="type">CLR type</param>
        /// <param name="options">Options</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Object</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Task<object> ReadAnyIntAsync(Stream stream, int? version, ObjectTypes objType, Type? type, ISerializerOptions? options, CancellationToken cancellationToken)
            => SerializerException.WrapAsync(async () =>
            {
                if (objType == ObjectTypes.Null) throw new SerializerException("NULL object type is not supported by this method");
                bool isEmpty = objType.IsEmpty(),
                    isUnsigned = objType.IsUnsigned(),
                    readType = true;
                Task task;
                switch (objType.RemoveFlags())
                {
                    case ObjectTypes.Bool:
                        return !isEmpty;
                    case ObjectTypes.Byte:
                        switch (version & byte.MaxValue)// Serializer version switch
                        {
                            case 1:
                            case 2:
                                type = isUnsigned ? typeof(byte) : typeof(sbyte);
                                break;
                            default:
                                return isEmpty
                                    ? Convert.ChangeType(0, isUnsigned ? typeof(byte) : typeof(sbyte))
                                    : isUnsigned
                                        ? await ReadOneByteAsync(stream, version, cancellationToken).DynamicContext()
                                        : await ReadOneSByteAsync(stream, version, cancellationToken).DynamicContext();
                        }
                        break;
                    case ObjectTypes.Short:
                        switch (version & byte.MaxValue)// Serializer version switch
                        {
                            case 1:
                            case 2:
                                type = isUnsigned ? typeof(ushort) : typeof(short);
                                break;
                            default:
                                return isEmpty
                                    ? Convert.ChangeType(0, isUnsigned ? typeof(ushort) : typeof(short))
                                    : isUnsigned
                                        ? await ReadUShortAsync(stream, version, cancellationToken: cancellationToken).DynamicContext()
                                        : await ReadShortAsync(stream, version, cancellationToken: cancellationToken).DynamicContext();
                        }
                        break;
                    case ObjectTypes.Int:
                    case ObjectTypes.Long:
                    case ObjectTypes.Float:
                    case ObjectTypes.Double:
                    case ObjectTypes.Decimal:
                        type = objType.RemoveFlags() switch
                        {
                            ObjectTypes.Int => isUnsigned ? typeof(uint) : typeof(int),
                            ObjectTypes.Long => isUnsigned ? typeof(ulong) : typeof(long),
                            ObjectTypes.Float => typeof(float),
                            ObjectTypes.Double => typeof(double),
                            ObjectTypes.Decimal => typeof(decimal),
                            _ => throw new InvalidProgramException()
                        };
                        break;
                    case ObjectTypes.String:
                        return isEmpty
                            ? string.Empty
                            : await ReadStringAsync(
                                stream,
                                version,
                                minLen: options?.GetMinLen(0) ?? 0,
                                maxLen: options?.GetMaxLen(int.MaxValue) ?? int.MaxValue,
                                cancellationToken: cancellationToken
                                ).DynamicContext();
                    case ObjectTypes.String16:
                        return isEmpty
                            ? string.Empty
                            : await ReadString16Async(
                                stream,
                                version,
                                minLen: options?.GetMinLen(0) ?? 0,
                                maxLen: options?.GetMaxLen(int.MaxValue) ?? int.MaxValue,
                                cancellationToken: cancellationToken
                                ).DynamicContext();
                    case ObjectTypes.String32:
                        return isEmpty
                            ? string.Empty
                            : await ReadString32Async(
                                stream,
                                version,
                                minLen: options?.GetMinLen(0) ?? 0,
                                maxLen: options?.GetMaxLen(int.MaxValue) ?? int.MaxValue,
                                cancellationToken: cancellationToken
                                ).DynamicContext();
                    case ObjectTypes.Bytes:
                        return isEmpty
                            ? Array.Empty<byte>()
                            : (await ReadBytesAsync(
                                stream,
                                version,
                                minLen: options?.GetMinLen(0) ?? 0,
                                maxLen: options?.GetMaxLen(int.MaxValue) ?? int.MaxValue,
                                cancellationToken: cancellationToken
                                ).DynamicContext()).Value;
                    case ObjectTypes.Stream:
                        readType = false;
                        break;
                    case ObjectTypes.ClrType:
                        if (isUnsigned)
                        {
                            if (!StreamSerializer.TypeCacheEnabled)
                                throw new SerializerException(
                                    $"The type cache needs to be enabled in order to be able to deserialize this type information",
                                    new InvalidOperationException()
                                    );
                            int thc = await stream.ReadIntAsync(version, cancellationToken: cancellationToken).DynamicContext();
                            if (!TypeCache.Types.TryGetValue(thc, out Type? type)) throw new SerializerException($"Unknown type #{thc}", new InvalidDataException());
                            if (!typeof(IStreamSerializer).IsAssignableFrom(type))
                                throw new SerializerException($"Invalid type {type} (possibly manipulated byte sequence)", new InvalidDataException());
                            return type;
                        }
                        return await ReadTypeAsync(stream, version, cancellationToken).DynamicContext();
                }
                if (readType && type == null)
                    type = StreamSerializer.LoadType(await ReadStringAsync(stream, version, pool: null, minLen: 1, maxLen: short.MaxValue, cancellationToken).DynamicContext());
                switch (objType.RemoveFlags())
                {
                    case ObjectTypes.Byte:
                    case ObjectTypes.Short:
                    case ObjectTypes.Int:
                    case ObjectTypes.Long:
                    case ObjectTypes.Float:
                    case ObjectTypes.Double:
                    case ObjectTypes.Decimal:
                        if (isEmpty) return Activator.CreateInstance(type!)!;
                        return await ReadNumberAsync(stream, type!, version, cancellationToken: cancellationToken).DynamicContext();
                    case ObjectTypes.Array:
                        return isEmpty ? ArrayEmptyMethod.MakeGenericMethod(type!.GetElementType()!).InvokeAuto(obj: null)! : await ReadArrayAsync(
                            stream,
                            type!,
                            version,
                            pool: null,
                            options?.GetMinLen(0) ?? 0,
                            options?.GetMaxLen(int.MaxValue) ?? int.MaxValue,
                            options?.Attribute.GetValueSerializerOptions(property: null, stream, version ?? StreamSerializer.Version, default),
                            cancellationToken: cancellationToken
                            ).DynamicContext();
                    case ObjectTypes.List:
                        return isEmpty ? Activator.CreateInstance(type!)! : await ReadListAsync(
                            stream,
                            type!,
                            version,
                            pool: null,
                            options?.GetMinLen(0) ?? 0,
                            options?.GetMaxLen(int.MaxValue) ?? int.MaxValue,
                            options?.Attribute.GetValueSerializerOptions(property: null, stream, version ?? StreamSerializer.Version, default),
                            cancellationToken: cancellationToken
                            ).DynamicContext();
                    case ObjectTypes.Dict:
                        return isEmpty ? Activator.CreateInstance(type!)! : await ReadDictAsync(
                            stream,
                            type!,
                            version,
                            pool: null,
                            options?.GetMinLen(0) ?? 0,
                            options?.GetMaxLen(int.MaxValue) ?? int.MaxValue,
                            options?.Attribute.GetKeySerializerOptions(property: null, stream, version ?? StreamSerializer.Version, cancellationToken),
                            options?.Attribute.GetValueSerializerOptions(property: null, stream, version ?? StreamSerializer.Version, cancellationToken),
                            cancellationToken: cancellationToken
                            ).DynamicContext();
                    case ObjectTypes.Object:
                        return await ReadObjectAsync(stream, type!, version, options, cancellationToken).DynamicContext();
                    case ObjectTypes.Struct:
                        task = (Task)ReadStructAsyncMethod.MakeGenericMethod(type!).InvokeAuto(obj: null, stream, version, cancellationToken)!;
                        break;
                    case ObjectTypes.Serializable:
                        return await ReadSerializedObjectAsync(stream, type!, version, cancellationToken)!.DynamicContext();
                    case ObjectTypes.Stream:
                        Stream res = options?.Attribute.GetStream(obj: null, property: null, stream, version ?? StreamSerializer.Version, default) ?? new FileStream(
                            Path.Combine(Settings.TempFolder, Guid.NewGuid().ToString()),
                            FileMode.CreateNew,
                            FileAccess.ReadWrite,
                            FileShare.None,
                            bufferSize: Settings.BufferSize,
                            FileOptions.RandomAccess | FileOptions.DeleteOnClose
                            );
                        if (objType.IsEmpty()) return res;
                        try
                        {
                            return await ReadStreamAsync(
                                stream,
                                res,
                                version,
                                minLen: options?.GetMinLen(0L) ?? 0,
                                maxLen: options?.GetMaxLen(long.MaxValue) ?? long.MaxValue,
                                cancellationToken: cancellationToken
                                ).DynamicContext();
                        }
                        catch
                        {
                            res.Dispose();
                            throw;
                        }
                    default:
                        throw new InvalidProgramException();
                }
                await task.DynamicContext();
                return task.GetResult(type!);
            });

        /// <summary>
        /// Read any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="options">Options</param>
        /// <returns>Object</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object? ReadAnyNullable(this Stream stream, int? version = null, ISerializerOptions? options = null)
        {
            ObjectTypes objType = (ObjectTypes)ReadOneByte(stream, version);
            return objType == ObjectTypes.Null ? null : ReadAnyInt(stream, version, objType, type: null, options);
        }

        /// <summary>
        /// Read any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="options">Options</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Object</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<object?> ReadAnyNullableAsync(
            this Stream stream,
            int? version = null,
            ISerializerOptions? options = null,
            CancellationToken cancellationToken = default
            )
        {
            ObjectTypes objType = (ObjectTypes)await ReadOneByteAsync(stream, version, cancellationToken).DynamicContext();
            return objType == ObjectTypes.Null ? null : await ReadAnyIntAsync(stream, version, objType, type: null, options, cancellationToken).DynamicContext();
        }
    }
}
