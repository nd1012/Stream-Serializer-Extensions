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
        public static object ReadAny(this Stream stream, int? version = null, ISerializerOptions? options = null)
            => SerializerException.Wrap(() =>
            {
                ObjectTypes objType;
                byte[] data = ReadSerializedData(stream, len: 1);
                try
                {
                    objType = (ObjectTypes)data[0];
                }
                finally
                {
                    StreamSerializer.BufferPool.Return(data);
                }
                return ReadAnyInt(stream, version, objType, options);
            });

        /// <summary>
        /// Read any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="objType">Object type</param>
        /// <param name="options">Options</param>
        /// <returns>Object</returns>
        private static object ReadAnyInt(Stream stream, int? version, ObjectTypes objType, ISerializerOptions? options)
            => SerializerException.Wrap(() =>
            {
                if (objType == ObjectTypes.Null) throw new SerializerException("NULL object type is not supported by this method");
                bool isEmpty = objType.HasFlag(ObjectTypes.Empty),
                    isUnsigned = objType.HasFlag(ObjectTypes.Unsigned),
                    readType = true;
                Type? type = null;
                switch (objType.RemoveFlags())
                {
                    case ObjectTypes.Bool:
                        return !isEmpty;
                    case ObjectTypes.Byte:
                    case ObjectTypes.Short:
                    case ObjectTypes.Int:
                    case ObjectTypes.Long:
                    case ObjectTypes.Float:
                    case ObjectTypes.Double:
                    case ObjectTypes.Decimal:
                        type = objType.RemoveFlags() switch
                        {
                            ObjectTypes.Byte => isUnsigned ? typeof(byte) : typeof(sbyte),
                            ObjectTypes.Short => isUnsigned ? typeof(ushort) : typeof(short),
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
                        if (isEmpty) return ArrayEmptyMethod.MakeGenericMethod(type!.GetElementType()!).InvokeAuto(obj: null)!;
                        return ReadArray(
                            stream,
                            type!,
                            version,
                            pool: null,
                            options?.GetMinLen(0) ?? 0,
                            options?.GetMaxLen(int.MaxValue) ?? int.MaxValue,
                            options?.Attribute.GetValueSerializerOptions(property: null, stream, version ?? StreamSerializer.VERSION, default)
                            );
                    case ObjectTypes.List:
                        if (isEmpty) return Activator.CreateInstance(type!)!;
                        return ReadList(
                            stream,
                            type!,
                            version,
                            pool: null,
                            options?.GetMinLen(0) ?? 0,
                            options?.GetMaxLen(int.MaxValue) ?? int.MaxValue,
                            options?.Attribute.GetValueSerializerOptions(property: null, stream, version ?? StreamSerializer.VERSION, default)
                            );
                    case ObjectTypes.Dict:
                        if (isEmpty) return Activator.CreateInstance(type!)!;
                        return ReadDict(
                            stream,
                            type!,
                            version,
                            pool: null,
                            options?.GetMinLen(0) ?? 0,
                            options?.GetMaxLen(int.MaxValue) ?? int.MaxValue,
                            options?.Attribute.GetKeySerializerOptions(property: null, stream, version ?? StreamSerializer.VERSION),
                            options?.Attribute.GetValueSerializerOptions(property: null, stream, version ?? StreamSerializer.VERSION)
                            );
                    case ObjectTypes.Object:
                        return ReadObject(stream, type!, version, options);
                    case ObjectTypes.Struct:
                        return ReadStructMethod.MakeGenericMethod(type!).InvokeAuto(obj: null, stream, version)!;
                    case ObjectTypes.Serializable:
                        return ReadSerializedObject(stream, type!, version);
                    case ObjectTypes.Stream:
                        Stream res = options?.Attribute.GetStream(obj: null, property: null, stream, version ?? StreamSerializer.VERSION, default) ?? new FileStream(
                            Path.Combine(Settings.TempFolder, Guid.NewGuid().ToString()),
                            FileMode.OpenOrCreate,
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
        public static Task<object> ReadAnyAsync(this Stream stream, int? version = null, ISerializerOptions? options = null, CancellationToken cancellationToken = default)
            => SerializerException.WrapAsync(async () =>
            {
                ObjectTypes objType;
                byte[] data = await ReadSerializedDataAsync(stream, len: 1, cancellationToken: cancellationToken).DynamicContext();
                try
                {
                    objType = (ObjectTypes)data[0];
                }
                finally
                {
                    StreamSerializer.BufferPool.Return(data);
                }
                return await ReadAnyIntAsync(stream, version, objType, options, cancellationToken).DynamicContext();
            });

        /// <summary>
        /// Read any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="objType">Object type</param>
        /// <param name="options">Options</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Object</returns>
        private static Task<object> ReadAnyIntAsync(Stream stream, int? version, ObjectTypes objType, ISerializerOptions? options, CancellationToken cancellationToken)
            => SerializerException.WrapAsync(async () =>
            {
                if (objType == ObjectTypes.Null) throw new SerializerException("NULL object type is not supported by this method");
                bool isEmpty = objType.HasFlag(ObjectTypes.Empty),
                    isUnsigned = objType.HasFlag(ObjectTypes.Unsigned),
                    readType = true;
                Type? type = null;
                Task task;
                switch (objType.RemoveFlags())
                {
                    case ObjectTypes.Bool:
                        return !isEmpty;
                    case ObjectTypes.Byte:
                    case ObjectTypes.Short:
                    case ObjectTypes.Int:
                    case ObjectTypes.Long:
                    case ObjectTypes.Float:
                    case ObjectTypes.Double:
                    case ObjectTypes.Decimal:
                        type = objType.RemoveFlags() switch
                        {
                            ObjectTypes.Byte => isUnsigned ? typeof(byte) : typeof(sbyte),
                            ObjectTypes.Short => isUnsigned ? typeof(ushort) : typeof(short),
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
                        if (isEmpty) return ArrayEmptyMethod.MakeGenericMethod(type!.GetElementType()!).InvokeAuto(obj: null)!;
                        return await ReadArrayAsync(
                            stream,
                            type!,
                            version,
                            pool: null,
                            options?.GetMinLen(0) ?? 0,
                            options?.GetMaxLen(int.MaxValue) ?? int.MaxValue,
                            options?.Attribute.GetValueSerializerOptions(property: null, stream, version ?? StreamSerializer.VERSION, default),
                            cancellationToken
                            ).DynamicContext();
                    case ObjectTypes.List:
                        if (isEmpty) return Activator.CreateInstance(type!)!;
                        return await ReadListAsync(
                            stream,
                            type!,
                            version,
                            pool: null,
                            options?.GetMinLen(0) ?? 0,
                            options?.GetMaxLen(int.MaxValue) ?? int.MaxValue,
                            options?.Attribute.GetValueSerializerOptions(property: null, stream, version ?? StreamSerializer.VERSION, default),
                            cancellationToken
                            ).DynamicContext();
                    case ObjectTypes.Dict:
                        if (isEmpty) return Activator.CreateInstance(type!)!;
                        return await ReadDictAsync(
                            stream,
                            type!,
                            version,
                            pool: null,
                            options?.GetMinLen(0) ?? 0,
                            options?.GetMaxLen(int.MaxValue) ?? int.MaxValue,
                            options?.Attribute.GetKeySerializerOptions(property: null, stream, version ?? StreamSerializer.VERSION, cancellationToken),
                            options?.Attribute.GetValueSerializerOptions(property: null, stream, version ?? StreamSerializer.VERSION, cancellationToken),
                            cancellationToken
                            ).DynamicContext();
                    case ObjectTypes.Object:
                        return await ReadObjectAsync(stream, type!, version, options, cancellationToken).DynamicContext();
                    case ObjectTypes.Struct:
                        task = (Task)ReadStructAsyncMethod.MakeGenericMethod(type!).InvokeAuto(obj: null, stream, version, cancellationToken)!;
                        break;
                    case ObjectTypes.Serializable:
                        return await ReadSerializedObjectAsync(stream, type!, version, cancellationToken)!.DynamicContext();
                    case ObjectTypes.Stream:
                        Stream res = options?.Attribute.GetStream(obj: null, property: null, stream, version ?? StreamSerializer.VERSION, default) ?? new FileStream(
                            Path.Combine(Settings.TempFolder, Guid.NewGuid().ToString()),
                            FileMode.OpenOrCreate,
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
        public static object? ReadAnyNullable(this Stream stream, int? version = null, ISerializerOptions? options = null)
            => SerializerException.Wrap(() =>
            {
                ObjectTypes objType;
                byte[] data = ReadSerializedData(stream, len: 1);
                try
                {
                    objType = (ObjectTypes)data[0];
                }
                finally
                {
                    StreamSerializer.BufferPool.Return(data);
                }
                return objType == ObjectTypes.Null ? null : ReadAnyInt(stream, version, objType, options);
            });

        /// <summary>
        /// Read any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="options">Options</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Object</returns>
        public static Task<object?> ReadAnyNullableAsync(
            this Stream stream, 
            int? version = null, 
            ISerializerOptions? options = null, 
            CancellationToken cancellationToken = default
            )
            => SerializerException.WrapAsync(async () =>
            {
                ObjectTypes objType;
                byte[] data = await ReadSerializedDataAsync(stream, len: 1, cancellationToken: cancellationToken).DynamicContext();
                try
                {
                    objType = (ObjectTypes)data[0];
                }
                finally
                {
                    StreamSerializer.BufferPool.Return(data);
                }
                return objType == ObjectTypes.Null ? null : await ReadAnyIntAsync(stream, version, objType, options, cancellationToken).DynamicContext();
            });
    }
}
