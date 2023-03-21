using System.Buffers;
using System.Reflection;
using System.Text;

namespace wan24.StreamSerializerExtensions
{
    public static partial class StreamExtensions
    {
        /// <summary>
        /// Read object method
        /// </summary>
        public static readonly MethodInfo ReadObjectMethod;
        /// <summary>
        /// Read object method
        /// </summary>
        public static readonly MethodInfo ReadObjectAsyncMethod;
        /// <summary>
        /// Read any method
        /// </summary>
        public static readonly MethodInfo ReadAnyMethod;
        /// <summary>
        /// Read any method
        /// </summary>
        public static readonly MethodInfo ReadAnyAsyncMethod;
        /// <summary>
        /// Read any method
        /// </summary>
        public static readonly MethodInfo ReadAnyNullableMethod;
        /// <summary>
        /// Read any method
        /// </summary>
        public static readonly MethodInfo ReadAnyNullableAsyncMethod;
        /// <summary>
        /// Read serialized method
        /// </summary>
        public static readonly MethodInfo ReadSerializedMethod;
        /// <summary>
        /// Read serialized method
        /// </summary>
        public static readonly MethodInfo ReadSerializedAsyncMethod;
        /// <summary>
        /// Read number method
        /// </summary>
        public static readonly MethodInfo ReadNumberMethod;
        /// <summary>
        /// Read number method
        /// </summary>
        public static readonly MethodInfo ReadNumberAsyncMethod;
        /// <summary>
        /// Read enumeration method
        /// </summary>
        public static readonly MethodInfo ReadEnumMethod;
        /// <summary>
        /// Read enumeration method
        /// </summary>
        public static readonly MethodInfo ReadEnumAsyncMethod;
        /// <summary>
        /// Read array method
        /// </summary>
        public static readonly MethodInfo ReadArrayMethod;
        /// <summary>
        /// Read array method
        /// </summary>
        public static readonly MethodInfo ReadArrayAsyncMethod;
        /// <summary>
        /// Read list method
        /// </summary>
        public static readonly MethodInfo ReadListMethod;
        /// <summary>
        /// Read list method
        /// </summary>
        public static readonly MethodInfo ReadListAsyncMethod;
        /// <summary>
        /// Read dictionary method
        /// </summary>
        public static readonly MethodInfo ReadDictMethod;
        /// <summary>
        /// Read dictionary method
        /// </summary>
        public static readonly MethodInfo ReadDictAsyncMethod;
        /// <summary>
        /// Array empty method
        /// </summary>
        public static readonly MethodInfo ArrayEmptyMethod;

        /// <summary>
        /// Require the <see cref="StreamSerializerAttribute"/> attribute when using <see cref="ReadAnyObject{T}(Stream, int?)"/> etc.?
        /// </summary>
        public static bool AnyObjectAttributeRequired { get; set; } = true;

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <returns>Value</returns>
        public static T ReadObject<T>(this Stream stream, int? version = null)
        {
            if (typeof(IStreamSerializer).IsAssignableFrom(typeof(T))) return (T)ReadSerializedObject(stream, typeof(T), version);
            StreamSerializer.Deserialize_Delegate deserializer = StreamSerializer.FindDeserializer(typeof(T)) ?? throw new SerializerException("No deserializer found");
            try
            {
                return (T)(deserializer(stream, typeof(T), version ?? StreamSerializer.Version) ?? throw new SerializerException($"{typeof(T)} deserialized to NULL"));
            }
            catch (SerializerException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static async Task<T> ReadObjectAsync<T>(this Stream stream, int? version = null, CancellationToken cancellationToken = default)
        {
            if (typeof(IStreamSerializer).IsAssignableFrom(typeof(T)))
                return (T)await ReadSerializedObjectAsync(stream, typeof(T), version, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
            if (StreamSerializer.FindAsyncDeserializer(typeof(T)) is not StreamSerializer.AsyncDeserialize_Delegate deserializer)
            {
                await Task.Yield();
                return ReadObject<T>(stream, version);
            }
            try
            {
                Task task = deserializer(stream, typeof(T), version ?? StreamSerializer.Version, cancellationToken);
                await task.ConfigureAwait(continueOnCapturedContext: false);
                return task.GetResultNullable<T>() ?? throw new SerializerException($"{typeof(T)} deserialized to NULL");
            }
            catch (SerializerException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <returns>Value</returns>
        public static T? ReadObjectNullable<T>(this Stream stream, int? version = null)
            => ReadBool(stream, version) ? ReadObject<T>(stream, version) : default(T?);

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static async Task<T?> ReadObjectNullableAsync<T>(this Stream stream, int? version = null, CancellationToken cancellationToken = default)
            => await ReadBoolAsync(stream, version, cancellationToken: cancellationToken).ConfigureAwait(continueOnCapturedContext: false)
                ? await ReadObjectAsync<T>(stream, version, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)
                : default(T?);

        /// <summary>
        /// Read any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <returns>Object</returns>
        public static object ReadAny(this Stream stream, int? version = null)
        {
            try
            {
                ObjectTypes objType;
                byte[] data = ReadSerializedData(stream, len: 1);
                try
                {
                    objType = (ObjectTypes)data[0];
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(data);
                }
                return ReadAnyInt(stream, version, objType);
            }
            catch (SerializerException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Read any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="objType">Object type</param>
        /// <returns>Object</returns>
        private static object ReadAnyInt(Stream stream, int? version, ObjectTypes objType)
        {
            if (objType == ObjectTypes.Null) throw new SerializerException("NULL object type is not supported by this method");
            bool isEmpty = objType.HasFlag(ObjectTypes.Empty),
                isUnsigned = objType.HasFlag(ObjectTypes.Unsigned);
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
                    return isEmpty ? string.Empty : ReadString(stream, version);
                case ObjectTypes.Bytes:
                    return isEmpty ? Array.Empty<byte>() : ReadBytes(stream, version).Value;
            }
            type ??= StreamSerializer.LoadType(ReadString(stream, version, minLen: 1, maxLen: short.MaxValue));
            switch (objType.RemoveFlags())
            {
                case ObjectTypes.Byte:
                case ObjectTypes.Short:
                case ObjectTypes.Int:
                case ObjectTypes.Long:
                case ObjectTypes.Float:
                case ObjectTypes.Double:
                case ObjectTypes.Decimal:
                    return isEmpty ? Activator.CreateInstance(type)! : ReadNumberMethod.MakeGenericMethod(type).InvokeAuto(obj: null, stream, version)!;
                case ObjectTypes.Array:
                    if (isEmpty) return ArrayEmptyMethod.MakeGenericMethod(type.GetElementType()!).InvokeAuto(obj: null)!;
                    return ReadArrayMethod.MakeGenericMethod(type.GetElementType()!).InvokeAuto(obj: null, stream, version)!;
                case ObjectTypes.List:
                    if (isEmpty) return Activator.CreateInstance(type)!;
                    return ReadListMethod.MakeGenericMethod(type.GetGenericArguments()[0]).InvokeAuto(obj: null, stream, version)!;
                case ObjectTypes.Dict:
                    if (isEmpty) return Activator.CreateInstance(type)!;
                    Type[] genericArgs = type.GetGenericArguments();
                    return ReadDictMethod.MakeGenericMethod(genericArgs[0], genericArgs[1]).InvokeAuto(obj: null, stream, version)!;
                case ObjectTypes.Object:
                    return ReadObjectMethod.MakeGenericMethod(type).InvokeAuto(obj: null, stream, version)!;
                case ObjectTypes.Serializable:
                    return ReadSerializedObject(stream, type, version);
                default:
                    throw new InvalidProgramException();
            }
        }

        /// <summary>
        /// Read any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Object</returns>
        public static async Task<object> ReadAnyAsync(this Stream stream, int? version = null, CancellationToken cancellationToken = default)
        {
            try
            {
                ObjectTypes objType;
                byte[] data = await ReadSerializedDataAsync(stream, len: 1, cancellationToken: cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
                try
                {
                    objType = (ObjectTypes)data[0];
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(data);
                }
                return await ReadAnyIntAsync(stream, version, objType, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
            }
            catch (SerializerException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Read any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="objType">Object type</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Object</returns>
        private static async Task<object> ReadAnyIntAsync(Stream stream, int? version, ObjectTypes objType, CancellationToken cancellationToken)
        {
            if (objType == ObjectTypes.Null) throw new SerializerException("NULL object type is not supported by this method");
            bool isEmpty = objType.HasFlag(ObjectTypes.Empty),
                isUnsigned = objType.HasFlag(ObjectTypes.Unsigned);
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
                    return isEmpty ? string.Empty : await ReadStringAsync(stream, version, cancellationToken: cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
                case ObjectTypes.Bytes:
                    return isEmpty ? Array.Empty<byte>() : (await ReadBytesAsync(stream, version, cancellationToken: cancellationToken).ConfigureAwait(continueOnCapturedContext: false)).Value;
            }
            type ??= StreamSerializer.LoadType(await ReadStringAsync(stream, version, pool: null, minLen: 1, maxLen: short.MaxValue, cancellationToken).ConfigureAwait(continueOnCapturedContext: false));
            switch (objType.RemoveFlags())
            {
                case ObjectTypes.Byte:
                case ObjectTypes.Short:
                case ObjectTypes.Int:
                case ObjectTypes.Long:
                case ObjectTypes.Float:
                case ObjectTypes.Double:
                case ObjectTypes.Decimal:
                    if (isEmpty) return Activator.CreateInstance(type)!;
                    task = (Task)ReadNumberAsyncMethod.MakeGenericMethod(type).InvokeAuto(obj: null, stream, version, null, cancellationToken)!;
                    break;
                case ObjectTypes.Array:
                    if (isEmpty) return ArrayEmptyMethod.MakeGenericMethod(type.GetElementType()!).InvokeAuto(obj: null)!;
                    task = (Task)ReadArrayAsyncMethod.MakeGenericMethod(type.GetElementType()!).InvokeAuto(obj: null, stream, version, null, 0, int.MaxValue, cancellationToken)!;
                    break;
                case ObjectTypes.List:
                    if (isEmpty) return Activator.CreateInstance(type)!;
                    task = (Task)ReadListAsyncMethod.MakeGenericMethod(type.GetGenericArguments()[0]).InvokeAuto(obj: null, stream, version, null, 0, int.MaxValue, cancellationToken)!;
                    break;
                case ObjectTypes.Dict:
                    if (isEmpty) return Activator.CreateInstance(type)!;
                    Type[] genericArgs = type.GetGenericArguments();
                    task = (Task)ReadDictAsyncMethod.MakeGenericMethod(genericArgs[0], genericArgs[1]).InvokeAuto(obj: null, stream, version, null, 0, int.MaxValue, cancellationToken)!;
                    break;
                case ObjectTypes.Object:
                    task = (Task)ReadObjectAsyncMethod.MakeGenericMethod(type).InvokeAuto(obj: null, stream, version, cancellationToken)!;
                    break;
                case ObjectTypes.Serializable:
                    return await ReadSerializedObjectAsync(stream, type, version, cancellationToken)!.ConfigureAwait(continueOnCapturedContext: false);
                default:
                    throw new InvalidProgramException();
            }
            await task.ConfigureAwait(continueOnCapturedContext: false);
            return task.GetResult(type);
        }

        /// <summary>
        /// Read any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <returns>Object</returns>
        public static object? ReadAnyNullable(this Stream stream, int? version = null)
        {
            try
            {
                ObjectTypes objType;
                byte[] data = ReadSerializedData(stream, len: 1);
                try
                {
                    objType = (ObjectTypes)data[0];
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(data);
                }
                return objType == ObjectTypes.Null ? null : ReadAnyInt(stream, version, objType);
            }
            catch (SerializerException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Read any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Object</returns>
        public static async Task<object?> ReadAnyNullableAsync(this Stream stream, int? version = null, CancellationToken cancellationToken = default)
        {
            try
            {
                ObjectTypes objType;
                byte[] data = await ReadSerializedDataAsync(stream, len: 1, cancellationToken: cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
                try
                {
                    objType = (ObjectTypes)data[0];
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(data);
                }
                return objType == ObjectTypes.Null ? null : await ReadAnyIntAsync(stream, version, objType, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
            }
            catch (SerializerException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        public static bool ReadBool(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
        {
            byte[] data = ReadSerializedData(stream, len: 1, pool);
            try
            {
                return data[0] == 1;
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
            finally
            {
                (pool ?? ArrayPool<byte>.Shared).Return(data);
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static async Task<bool> ReadBoolAsync(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
        {
            byte[] data = await ReadSerializedDataAsync(stream, len: 1, pool, cancellationToken: cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
            try
            {
                return data[0] == 1;
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
            finally
            {
                (pool ?? ArrayPool<byte>.Shared).Return(data);
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        public static bool? ReadBoolNullable(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
            => ReadBool(stream, version, pool) ? ReadBool(stream, version, pool) : default(bool?);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static async Task<bool?> ReadBoolNullableAsync(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
            => await ReadBoolAsync(stream, version, pool, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)
                ? await ReadBoolAsync(stream, version, pool, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)
                : default(bool?);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <returns>Value</returns>
        public static sbyte ReadOneSByte(this Stream stream, int? version = null)
        {
            try
            {
                return (sbyte)ReadOneByte(stream, version);
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static async Task<sbyte> ReadOneSByteAsync(this Stream stream, int? version = null, CancellationToken cancellationToken = default)
        {
            await Task.Yield();
            return ReadOneSByte(stream, version);
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <returns>Value</returns>
        public static sbyte? ReadOneSByteNullable(this Stream stream, int? version = null)
            => ReadBool(stream, version) ? ReadOneSByte(stream, version) : default(sbyte?);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static async Task<sbyte?> ReadOneSByteNullableAsync(this Stream stream, int? version = null, CancellationToken cancellationToken = default)
            => await ReadBoolAsync(stream, version, cancellationToken: cancellationToken).ConfigureAwait(continueOnCapturedContext: false)
                ? await ReadOneSByteAsync(stream, version, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)
                : default(sbyte?);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <returns>Value</returns>
        public static byte ReadOneByte(this Stream stream, int? version = null)
        {
            try
            {
                int res = stream.ReadByte();
                if (res < 0) throw new SerializerException("Failed to read one byte from stream");
                return (byte)res;
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static async Task<byte> ReadOneByteAsync(this Stream stream, int? version = null, CancellationToken cancellationToken = default)
        {
            await Task.Yield();
            return ReadOneByte(stream, version);
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <returns>Value</returns>
        public static ushort? ReadOneByteNullable(this Stream stream, int? version = null)
            => ReadBool(stream, version) ? ReadOneByte(stream, version) : default(ushort?);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static async Task<ushort?> ReadOneByteNullableAsync(this Stream stream, int? version = null, CancellationToken cancellationToken = default)
            => await ReadBoolAsync(stream, version, cancellationToken: cancellationToken).ConfigureAwait(continueOnCapturedContext: false)
                ? await ReadOneByteAsync(stream, version, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)
                : default(ushort?);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        public static short ReadShort(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
        {
            byte[] data = ReadSerializedData(stream, len: sizeof(short), pool);
            try
            {
                return BitConverter.ToInt16(data.AsSpan(0, sizeof(short)).ConvertEndian());
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
            finally
            {
                (pool ?? ArrayPool<byte>.Shared).Return(data);
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static async Task<short> ReadShortAsync(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
        {
            byte[] data = await ReadSerializedDataAsync(stream, len: sizeof(short), pool, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
            try
            {
                return BitConverter.ToInt16(data.AsSpan(0, sizeof(short)).ConvertEndian());
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
            finally
            {
                (pool ?? ArrayPool<byte>.Shared).Return(data);
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        public static short? ReadShortNullable(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
            => ReadBool(stream, version, pool) ? ReadShort(stream, version, pool) : default(short?);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static async Task<short?> ReadShortNullableAsync(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
            => await ReadBoolAsync(stream, version, pool, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)
                ? await ReadShortAsync(stream, version, pool, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)
                : default(short?);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        public static ushort ReadUShort(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
        {
            byte[] data = ReadSerializedData(stream, len: sizeof(ushort), pool);
            try
            {
                return BitConverter.ToUInt16(data.AsSpan(0, sizeof(ushort)).ConvertEndian());
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
            finally
            {
                (pool ?? ArrayPool<byte>.Shared).Return(data);
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static async Task<ushort> ReadUShortAsync(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
        {
            byte[] data = await ReadSerializedDataAsync(stream, len: sizeof(ushort), pool, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
            try
            {
                return BitConverter.ToUInt16(data.AsSpan(0, sizeof(ushort)).ConvertEndian());
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
            finally
            {
                (pool ?? ArrayPool<byte>.Shared).Return(data);
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        public static ushort? ReadUShortNullable(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
            => ReadBool(stream, version, pool) ? ReadUShort(stream, version, pool) : default(ushort?);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static async Task<ushort?> ReadUShortNullableAsync(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
            => await ReadBoolAsync(stream, version, pool, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)
                ? await ReadUShortAsync(stream, version, pool, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)
                : default(ushort?);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        public static int ReadInt(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
        {
            byte[] data = ReadSerializedData(stream, len: sizeof(int), pool);
            try
            {
                return BitConverter.ToInt32(data.AsSpan(0, sizeof(int)).ConvertEndian());
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
            finally
            {
                (pool ?? ArrayPool<byte>.Shared).Return(data);
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static async Task<int> ReadIntAsync(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
        {
            byte[] data = await ReadSerializedDataAsync(stream, len: sizeof(int), pool, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
            try
            {
                return BitConverter.ToInt32(data.AsSpan(0, sizeof(int)).ConvertEndian());
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
            finally
            {
                (pool ?? ArrayPool<byte>.Shared).Return(data);
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        public static int? ReadIntNullable(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
            => ReadBool(stream, version, pool) ? ReadInt(stream, version, pool) : default(int?);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static async Task<int?> ReadIntNullableAsync(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
            => await ReadBoolAsync(stream, version, pool, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)
                ? await ReadIntAsync(stream, version, pool, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)
                : default(int?);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        public static uint ReadUInt(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
        {
            byte[] data = ReadSerializedData(stream, len: sizeof(uint), pool);
            try
            {
                return BitConverter.ToUInt32(data.AsSpan(0, sizeof(uint)).ConvertEndian());
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
            finally
            {
                (pool ?? ArrayPool<byte>.Shared).Return(data);
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static async Task<uint> ReadUIntAsync(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
        {
            byte[] data = await ReadSerializedDataAsync(stream, len: sizeof(uint), pool, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
            try
            {
                return BitConverter.ToUInt32(data.AsSpan(0, sizeof(uint)).ConvertEndian());
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
            finally
            {
                (pool ?? ArrayPool<byte>.Shared).Return(data);
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        public static uint? ReadUIntNullable(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
            => ReadBool(stream, version, pool) ? ReadUInt(stream, version, pool) : default(uint?);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static async Task<uint?> ReadUIntNullableAsync(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
            => await ReadBoolAsync(stream, version, pool, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)
                ? await ReadUIntAsync(stream, version, pool, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)
                : default(uint?);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        public static long ReadLong(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
        {
            byte[] data = ReadSerializedData(stream, len: sizeof(long), pool);
            try
            {
                return BitConverter.ToInt64(data.AsSpan(0, sizeof(long)).ConvertEndian());
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
            finally
            {
                (pool ?? ArrayPool<byte>.Shared).Return(data);
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static async Task<long> ReadLongAsync(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
        {
            byte[] data = await ReadSerializedDataAsync(stream, len: sizeof(long), pool, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
            try
            {
                return BitConverter.ToInt64(data.AsSpan(0, sizeof(long)).ConvertEndian());
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
            finally
            {
                (pool ?? ArrayPool<byte>.Shared).Return(data);
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        public static long? ReadLongNullable(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
            => ReadBool(stream, version, pool) ? ReadLong(stream, version, pool) : default(long?);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static async Task<long?> ReadLongNullableAsync(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
            => await ReadBoolAsync(stream, version, pool, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)
                ? await ReadLongAsync(stream, version, pool, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)
                : default(long?);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        public static ulong ReadULong(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
        {
            byte[] data = ReadSerializedData(stream, len: sizeof(ulong), pool);
            try
            {
                return BitConverter.ToUInt64(data.AsSpan(0, sizeof(ulong)).ConvertEndian());
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
            finally
            {
                (pool ?? ArrayPool<byte>.Shared).Return(data);
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static async Task<ulong> ReadULongAsync(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
        {
            byte[] data = await ReadSerializedDataAsync(stream, len: sizeof(ulong), pool, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
            try
            {
                return BitConverter.ToUInt64(data.AsSpan(0, sizeof(ulong)).ConvertEndian());
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
            finally
            {
                (pool ?? ArrayPool<byte>.Shared).Return(data);
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        public static ulong? ReadULongNullable(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
            => ReadBool(stream, version, pool) ? ReadULong(stream, version, pool) : default(ulong?);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static async Task<ulong?> ReadULongNullableAsync(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
            => await ReadBoolAsync(stream, version, pool, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)
                ? await ReadULongAsync(stream, version, pool, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)
                : default(ulong?);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        public static float ReadFloat(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
        {
            byte[] data = ReadSerializedData(stream, len: sizeof(float), pool);
            try
            {
                return BitConverter.ToSingle(data.AsSpan(0, sizeof(float)).ConvertEndian());
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
            finally
            {
                (pool ?? ArrayPool<byte>.Shared).Return(data);
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static async Task<float> ReadFloatAsync(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
        {
            byte[] data = await ReadSerializedDataAsync(stream, len: sizeof(float), pool, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
            try
            {
                return BitConverter.ToSingle(data.AsSpan(0, sizeof(float)).ConvertEndian());
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
            finally
            {
                (pool ?? ArrayPool<byte>.Shared).Return(data);
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        public static float? ReadFloatNullable(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
            => ReadBool(stream, version, pool) ? ReadFloat(stream, version, pool) : default(float?);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static async Task<float?> ReadFloatNullableAsync(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
            => await ReadBoolAsync(stream, version, pool, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)
                ? await ReadFloatAsync(stream, version, pool, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)
                : default(float?);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        public static double ReadDouble(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
        {
            byte[] data = ReadSerializedData(stream, len: sizeof(double), pool);
            try
            {
                return BitConverter.ToDouble(data.AsSpan(0, sizeof(double)).ConvertEndian());
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
            finally
            {
                (pool ?? ArrayPool<byte>.Shared).Return(data);
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static async Task<double> ReadDoubleAsync(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
        {
            byte[] data = await ReadSerializedDataAsync(stream, len: sizeof(double), pool, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
            try
            {
                return BitConverter.ToDouble(data.AsSpan(0, sizeof(double)).ConvertEndian());
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
            finally
            {
                (pool ?? ArrayPool<byte>.Shared).Return(data);
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        public static double? ReadDoubleNullable(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
            => ReadBool(stream, version, pool) ? ReadDouble(stream, version, pool) : default(double?);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static async Task<double?> ReadDoubleNullableAsync(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
            => await ReadBoolAsync(stream, version, pool, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)
                ? await ReadDoubleAsync(stream, version, pool, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)
                : default(double?);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        public static decimal ReadDecimal(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
        {
            int[] bits = new int[4];
            for (int i = 0; i < bits.Length; bits[i] = ReadInt(stream, version, pool), i++) ;
            try
            {
                return new decimal(bits);
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static async Task<decimal> ReadDecimalAsync(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
        {
            int[] bits = new int[4];
            for (int i = 0; i < bits.Length; bits[i] = await ReadIntAsync(stream, version, pool, cancellationToken).ConfigureAwait(continueOnCapturedContext: false), i++) ;
            try
            {
                return new decimal(bits);
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        public static decimal? ReadDecimalNullable(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
            => ReadBool(stream, version, pool) ? ReadDecimal(stream, version, pool) : default(decimal?);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static async Task<decimal?> ReadDecimalNullableAsync(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
            => await ReadBoolAsync(stream, version, pool, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)
                ? await ReadDecimalAsync(stream, version, pool, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)
                : default(decimal?);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        public static T ReadNumber<T>(this Stream stream, int? version = null, ArrayPool<byte>? pool = null) where T : struct, IConvertible
        {
            byte[] data = ReadSerializedData(stream, len: 1, pool);
            try
            {
                NumberTypes type = (NumberTypes)data[0];
                if (type.IsZero()) return (T)Activator.CreateInstance(typeof(T))!;
                switch (type.RemoveValueFlags())
                {
                    case NumberTypes.Byte:
                        switch (type)
                        {
                            case NumberTypes.Byte | NumberTypes.MinValue:
                                return (T)(object)sbyte.MinValue;
                            case NumberTypes.Byte | NumberTypes.MaxValue:
                                return (T)(object)sbyte.MaxValue;
                        }
                        if (stream.Read(data.AsSpan(0, 1)) != 1) throw new SerializerException("Failed to read serialized data (1 bytes)");
                        return (T)Convert.ChangeType((sbyte)data[0], typeof(T));
                    case NumberTypes.Byte | NumberTypes.Unsigned:
                        switch (type)
                        {
                            case NumberTypes.Byte | NumberTypes.MaxValue | NumberTypes.Unsigned:
                                return (T)(object)byte.MaxValue;
                        }
                        if (stream.Read(data.AsSpan(0, 1)) != 1) throw new SerializerException("Failed to read serialized data (1 bytes)");
                        return (T)Convert.ChangeType(data[0], typeof(T));
                    case NumberTypes.Short:
                        switch (type)
                        {
                            case NumberTypes.Short | NumberTypes.MinValue:
                                return (T)(object)short.MinValue;
                            case NumberTypes.Short | NumberTypes.MaxValue:
                                return (T)(object)short.MaxValue;
                        }
                        return (T)Convert.ChangeType(ReadShort(stream, version, pool), typeof(T));
                    case NumberTypes.Short | NumberTypes.Unsigned:
                        switch (type)
                        {
                            case NumberTypes.Short | NumberTypes.MaxValue | NumberTypes.Unsigned:
                                return (T)(object)ushort.MaxValue;
                        }
                        return (T)Convert.ChangeType(ReadUShort(stream, version, pool), typeof(T));
                    case NumberTypes.Int:
                        switch (type)
                        {
                            case NumberTypes.Int | NumberTypes.MinValue:
                                return (T)(object)int.MinValue;
                            case NumberTypes.Int | NumberTypes.MaxValue:
                                return (T)(object)int.MaxValue;
                        }
                        return (T)Convert.ChangeType(ReadInt(stream, version, pool), typeof(T));
                    case NumberTypes.Int | NumberTypes.Unsigned:
                        switch (type)
                        {
                            case NumberTypes.Int | NumberTypes.MaxValue | NumberTypes.Unsigned:
                                return (T)(object)uint.MaxValue;
                        }
                        return (T)Convert.ChangeType(ReadUInt(stream, version, pool), typeof(T));
                    case NumberTypes.Long:
                        switch (type)
                        {
                            case NumberTypes.Long | NumberTypes.MinValue:
                                return (T)(object)long.MinValue;
                            case NumberTypes.Long | NumberTypes.MaxValue:
                                return (T)(object)long.MaxValue;
                        }
                        return (T)Convert.ChangeType(ReadLong(stream, version, pool), typeof(T));
                    case NumberTypes.Long | NumberTypes.Unsigned:
                        switch (type)
                        {
                            case NumberTypes.Long | NumberTypes.MaxValue | NumberTypes.Unsigned:
                                return (T)(object)ulong.MaxValue;
                        }
                        return (T)Convert.ChangeType(ReadULong(stream, version, pool), typeof(T));
                    case NumberTypes.Float:
                        switch (type)
                        {
                            case NumberTypes.Float | NumberTypes.MinValue:
                                return (T)(object)float.MinValue;
                            case NumberTypes.Float | NumberTypes.MaxValue:
                                return (T)(object)float.MaxValue;
                        }
                        return (T)Convert.ChangeType(ReadFloat(stream, version, pool), typeof(T));
                    case NumberTypes.Double:
                        switch (type)
                        {
                            case NumberTypes.Double | NumberTypes.MinValue:
                                return (T)(object)double.MinValue;
                            case NumberTypes.Double | NumberTypes.MaxValue:
                                return (T)(object)double.MaxValue;
                        }
                        return (T)Convert.ChangeType(ReadDouble(stream, version, pool), typeof(T));
                    case NumberTypes.Decimal:
                        switch (type)
                        {
                            case NumberTypes.Decimal | NumberTypes.MinValue:
                                return (T)(object)decimal.MinValue;
                            case NumberTypes.Decimal | NumberTypes.MaxValue:
                                return (T)(object)decimal.MaxValue;
                        }
                        return (T)Convert.ChangeType(ReadDecimal(stream, version, pool), typeof(T));
                    default:
                        throw new SerializerException($"Unknown numeric type {type}");
                }
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
            finally
            {
                (pool ?? ArrayPool<byte>.Shared).Return(data);
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static async Task<T> ReadNumberAsync<T>(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
            where T : struct, IConvertible
        {
            byte[] data = await ReadSerializedDataAsync(stream, len: 1, pool, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
            try
            {
                NumberTypes type = (NumberTypes)data[0];
                if (type.IsZero()) return (T)Activator.CreateInstance(typeof(T))!;
                switch (type.RemoveValueFlags())
                {
                    case NumberTypes.Byte:
                        switch (type)
                        {
                            case NumberTypes.Byte | NumberTypes.MinValue:
                                return (T)(object)sbyte.MinValue;
                            case NumberTypes.Byte | NumberTypes.MaxValue:
                                return (T)(object)sbyte.MaxValue;
                        }
                        if (await stream.ReadAsync(data.AsMemory(0, 1), cancellationToken).ConfigureAwait(continueOnCapturedContext: false) != 1)
                            throw new SerializerException("Failed to read serialized data (1 bytes)");
                        return (T)Convert.ChangeType((sbyte)data[0], typeof(T));
                    case NumberTypes.Byte | NumberTypes.Unsigned:
                        switch (type)
                        {
                            case NumberTypes.Byte | NumberTypes.MaxValue | NumberTypes.Unsigned:
                                return (T)(object)byte.MaxValue;
                        }
                        if (await stream.ReadAsync(data.AsMemory(0, 1), cancellationToken).ConfigureAwait(continueOnCapturedContext: false) != 1)
                            throw new SerializerException("Failed to read serialized data (1 bytes)");
                        return (T)Convert.ChangeType(data[0], typeof(T));
                    case NumberTypes.Short:
                        switch (type)
                        {
                            case NumberTypes.Short | NumberTypes.MinValue:
                                return (T)(object)short.MinValue;
                            case NumberTypes.Short | NumberTypes.MaxValue:
                                return (T)(object)short.MaxValue;
                        }
                        return (T)Convert.ChangeType(await ReadShortAsync(stream, version, pool, cancellationToken).ConfigureAwait(continueOnCapturedContext: false), typeof(T));
                    case NumberTypes.Short | NumberTypes.Unsigned:
                        switch (type)
                        {
                            case NumberTypes.Short | NumberTypes.MaxValue | NumberTypes.Unsigned:
                                return (T)(object)ushort.MaxValue;
                        }
                        return (T)Convert.ChangeType(await ReadUShortAsync(stream, version, pool, cancellationToken).ConfigureAwait(continueOnCapturedContext: false), typeof(T));
                    case NumberTypes.Int:
                        switch (type)
                        {
                            case NumberTypes.Int | NumberTypes.MinValue:
                                return (T)(object)int.MinValue;
                            case NumberTypes.Int | NumberTypes.MaxValue:
                                return (T)(object)int.MaxValue;
                        }
                        return (T)Convert.ChangeType(await ReadIntAsync(stream, version, pool, cancellationToken).ConfigureAwait(continueOnCapturedContext: false), typeof(T));
                    case NumberTypes.Int | NumberTypes.Unsigned:
                        switch (type)
                        {
                            case NumberTypes.Int | NumberTypes.MaxValue | NumberTypes.Unsigned:
                                return (T)(object)uint.MaxValue;
                        }
                        return (T)Convert.ChangeType(await ReadUIntAsync(stream, version, pool, cancellationToken).ConfigureAwait(continueOnCapturedContext: false), typeof(T));
                    case NumberTypes.Long:
                        switch (type)
                        {
                            case NumberTypes.Long | NumberTypes.MinValue:
                                return (T)(object)long.MinValue;
                            case NumberTypes.Long | NumberTypes.MaxValue:
                                return (T)(object)long.MaxValue;
                        }
                        return (T)Convert.ChangeType(await ReadLongAsync(stream, version, pool, cancellationToken).ConfigureAwait(continueOnCapturedContext: false), typeof(T));
                    case NumberTypes.Long | NumberTypes.Unsigned:
                        switch (type)
                        {
                            case NumberTypes.Long | NumberTypes.MaxValue | NumberTypes.Unsigned:
                                return (T)(object)ulong.MaxValue;
                        }
                        return (T)Convert.ChangeType(await ReadULongAsync(stream, version, pool, cancellationToken).ConfigureAwait(continueOnCapturedContext: false), typeof(T));
                    case NumberTypes.Float:
                        switch (type)
                        {
                            case NumberTypes.Float | NumberTypes.MinValue:
                                return (T)(object)float.MinValue;
                            case NumberTypes.Float | NumberTypes.MaxValue:
                                return (T)(object)float.MaxValue;
                        }
                        return (T)Convert.ChangeType(await ReadFloatAsync(stream, version, pool, cancellationToken).ConfigureAwait(continueOnCapturedContext: false), typeof(T));
                    case NumberTypes.Double:
                        switch (type)
                        {
                            case NumberTypes.Double | NumberTypes.MinValue:
                                return (T)(object)double.MinValue;
                            case NumberTypes.Double | NumberTypes.MaxValue:
                                return (T)(object)double.MaxValue;
                        }
                        return (T)Convert.ChangeType(await ReadDoubleAsync(stream, version, pool, cancellationToken).ConfigureAwait(continueOnCapturedContext: false), typeof(T));
                    case NumberTypes.Decimal:
                        switch (type)
                        {
                            case NumberTypes.Decimal | NumberTypes.MinValue:
                                return (T)(object)decimal.MinValue;
                            case NumberTypes.Decimal | NumberTypes.MaxValue:
                                return (T)(object)decimal.MaxValue;
                        }
                        return (T)Convert.ChangeType(await ReadDecimalAsync(stream, version, pool, cancellationToken).ConfigureAwait(continueOnCapturedContext: false), typeof(T));
                    default:
                        throw new SerializerException($"Unknown numeric type {type}");
                }
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
            finally
            {
                (pool ?? ArrayPool<byte>.Shared).Return(data);
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        public static T? ReadNumberNullable<T>(this Stream stream, int? version = null, ArrayPool<byte>? pool = null) where T : struct, IConvertible
            => ReadBool(stream, version, pool) ? ReadNumber<T>(stream, version, pool) : default(T?);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static async Task<T?> ReadNumberNullableAsync<T>(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
            where T : struct, IConvertible
            => await ReadBoolAsync(stream, version, pool, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)
                ? await ReadNumberAsync<T>(stream, version, pool, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)
                : default(T?);

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Enumeration type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        public static T ReadEnum<T>(this Stream stream, int? version = null, ArrayPool<byte>? pool = null) where T : struct, Enum
        {
            try
            {
                T res = (T)Enum.ToObject(typeof(T), ReadNumberMethod.MakeGenericMethod(typeof(T).GetEnumUnderlyingType()).InvokeAuto(obj: null, stream, version, pool)!);
                if (!Enum.IsDefined(res)) throw new SerializerException($"Unknown enumeration value {res} for {typeof(T)}");
                return res;
            }
            catch (SerializerException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Enumeration type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static async Task<T> ReadEnumAsync<T>(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default) where T : struct, Enum
        {
            try
            {
                Type type = typeof(T).GetEnumUnderlyingType();
                Task task = (Task)ReadNumberAsyncMethod.MakeGenericMethod(type).InvokeAuto(obj: null, stream, version, pool, cancellationToken)!;
                await task.ConfigureAwait(continueOnCapturedContext: false);
                T res = (T)Enum.ToObject(typeof(T), task.GetResult(type));
                if (!Enum.IsDefined(res)) throw new SerializerException($"Unknown enumeration value {res} for {typeof(T)}");
                return res;
            }
            catch (SerializerException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Enumeration type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        public static T? ReadEnumNullable<T>(this Stream stream, int? version = null, ArrayPool<byte>? pool = null) where T : struct, Enum
            => ReadBool(stream, version, pool) ? ReadEnum<T>(stream, version, pool) : default(T?);

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Enumeration type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static async Task<T?> ReadEnumNullableAsync<T>(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
            where T : struct, Enum
            => await ReadBoolAsync(stream, version, pool, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)
                ? await ReadEnumAsync<T>(stream, version, pool, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)
                : default(T?);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="buffer">Result buffer to use</param>
        /// <param name="pool">Array pool (if given, and <c>buffer</c> is <see langword="null"/>, the returned value is a pool array which needs to be returned to the pool after use!)</param>
        /// <param name="minLen">Minimum length in bytes</param>
        /// <param name="maxLen">Maximum length in bytes</param>
        /// <returns>Value and length</returns>
        public static (byte[] Value, int Length) ReadBytes(this Stream stream, int? version = null, byte[]? buffer = null, ArrayPool<byte>? pool = null, int minLen = 0, int maxLen = int.MaxValue)
        {
            bool rented = false;
            try
            {
                int len = ReadNumber<int>(stream, version, pool);
                SerializerHelper.EnsureValidLength(len, minLen, maxLen);
                if (len == 0 && buffer == null) buffer = Array.Empty<byte>();
                rented = buffer == null && pool != null;
                buffer ??= rented ? pool!.Rent(len) : new byte[len];
                if (len != 0 && stream.Read(buffer.AsSpan(0, len)) != len) throw new SerializerException($"Failed to read serialized data ({len} bytes)");
                return (buffer, len);
            }
            catch (SerializerException)
            {
                if (rented) pool!.Return(buffer!);
                throw;
            }
            catch (Exception ex)
            {
                if (rented) pool!.Return(buffer!);
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="buffer">Result buffer to use</param>
        /// <param name="pool">Array pool (if given, and <c>buffer</c> is <see langword="null"/>, the returned value is a pool array which needs to be returned to the pool after use!)</param>
        /// <param name="minLen">Minimum length in bytes</param>
        /// <param name="maxLen">Maximum length in bytes</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value and length</returns>
        public static async Task<(byte[] Value, int Length)> ReadBytesAsync(
            this Stream stream,
            int? version = null,
            byte[]? buffer = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            CancellationToken cancellationToken = default
            )
        {
            bool rented = false;
            try
            {
                int len = await ReadNumberAsync<int>(stream, version, pool, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
                SerializerHelper.EnsureValidLength(len, minLen, maxLen);
                if (len == 0 && buffer == null) buffer = Array.Empty<byte>();
                rented = buffer == null && pool != null;
                buffer ??= rented ? pool!.Rent(len) : new byte[len];
                if (len != 0 && await stream.ReadAsync(buffer.AsMemory(0, len), cancellationToken).ConfigureAwait(continueOnCapturedContext: false) != len)
                    throw new SerializerException($"Failed to read serialized data ({len} bytes)");
                return (buffer, len);
            }
            catch (SerializerException)
            {
                if (rented) pool!.Return(buffer!);
                throw;
            }
            catch (Exception ex)
            {
                if (rented) pool!.Return(buffer!);
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="buffer">Result buffer to use</param>
        /// <param name="pool">Array pool (if given, and <c>buffer</c> is <see langword="null"/>, the returned value is a pool array which needs to be returned to the pool after use!)</param>
        /// <param name="minLen">Minimum length in bytes</param>
        /// <param name="maxLen">Maximum length in bytes</param>
        /// <returns>Value and length</returns>
        public static (byte[] Value, int Length)? ReadBytesNullable(
            this Stream stream,
            int? version = null,
            byte[]? buffer = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue
            )
            => ReadBool(stream, version, pool) ? ReadBytes(stream, version, buffer, pool, minLen, maxLen) : default((byte[] Value, int Length)?);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="buffer">Result buffer to use</param>
        /// <param name="pool">Array pool (if given, and <c>buffer</c> is <see langword="null"/>, the returned value is a pool array which needs to be returned to the pool after use!)</param>
        /// <param name="minLen">Minimum length in bytes</param>
        /// <param name="maxLen">Maximum length in bytes</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value and length</returns>
        public static async Task<(byte[] Value, int Length)?> ReadBytesNullableAsync(
            this Stream stream,
            int? version = null,
            byte[]? buffer = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            CancellationToken cancellationToken = default
            )
            => await ReadBoolAsync(stream, version, pool, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)
                ? await ReadBytesAsync(stream, version, buffer, pool, minLen, maxLen, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)
                : default((byte[] Value, int Length)?);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length in bytes</param>
        /// <param name="maxLen">Maximum length in bytes</param>
        /// <returns>Value</returns>
        public static string ReadString(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, int minLen = 0, int maxLen = int.MaxValue)
        {
            (byte[] data, int len) = ReadBytes(stream, version, buffer: null, pool, minLen, maxLen);
            try
            {
                char[] res = new char[len];
                new UTF8Encoding(encoderShouldEmitUTF8Identifier: true).GetDecoder().Convert(data.AsSpan(0, len), res, flush: true, out int used, out int characters, out bool completed);
                if (!completed || used != len) throw new SerializerException("Invalid UTF-8 string data");
                return new string(res, 0, characters);
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
            finally
            {
                pool?.Return(data);
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length in bytes</param>
        /// <param name="maxLen">Maximum length in bytes</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static async Task<string> ReadStringAsync(
            this Stream stream,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            CancellationToken cancellationToken = default
            )
        {
            (byte[] data, int len) = await ReadBytesAsync(stream, version, buffer: null, pool, minLen, maxLen, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
            try
            {
                char[] res = new char[len];
                new UTF8Encoding(encoderShouldEmitUTF8Identifier: true).GetDecoder().Convert(data.AsSpan(0, len), res, flush: true, out int used, out int characters, out bool completed);
                if (!completed || used != len) throw new SerializerException("Invalid UTF-8 string data");
                return new string(res, 0, characters);
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
            finally
            {
                pool?.Return(data);
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length in bytes</param>
        /// <param name="maxLen">Maximum length in bytes</param>
        /// <returns>Value</returns>
        public static string? ReadStringNullable(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, int minLen = 0, int maxLen = int.MaxValue)
            => ReadBool(stream, version, pool) ? ReadString(stream, version, pool, minLen, maxLen) : default(string?);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length in bytes</param>
        /// <param name="maxLen">Maximum length in bytes</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static async Task<string?> ReadStringNullableAsync(
            this Stream stream,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            CancellationToken cancellationToken = default
            )
            => await ReadBoolAsync(stream, version, pool, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)
                ? await ReadStringAsync(stream, version, pool, minLen, maxLen, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)
                : default(string?);

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <returns>Value</returns>
        public static T[] ReadArray<T>(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, int minLen = 0, int maxLen = int.MaxValue)
        {
            if (typeof(T) == typeof(byte)) return (ReadBytes(stream, version, buffer: null, pool, minLen, maxLen) as T[])!;
            try
            {
                int len = ReadNumber<int>(stream, version, pool);
                SerializerHelper.EnsureValidLength(len, minLen, maxLen);
                T[] res = new T[len];
                for (int i = 0; i < len; res[i] = ReadObject<T>(stream, version), i++) ;
                return res;
            }
            catch (SerializerException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static async Task<T[]> ReadArrayAsync<T>(
            this Stream stream,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            CancellationToken cancellationToken = default
            )
        {
            if (typeof(T) == typeof(byte))
                return (await ReadBytesAsync(stream, version, buffer: null, pool, minLen, maxLen, cancellationToken).ConfigureAwait(continueOnCapturedContext: false) as T[])!;
            try
            {
                int len = await ReadNumberAsync<int>(stream, version, pool, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
                SerializerHelper.EnsureValidLength(len, minLen, maxLen);
                T[] res = new T[len];
                for (int i = 0; i < len; res[i] = await ReadObjectAsync<T>(stream, version, cancellationToken).ConfigureAwait(continueOnCapturedContext: false), i++) ;
                return res;
            }
            catch (SerializerException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <returns>Value</returns>
        public static T[]? ReadArrayNullable<T>(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, int minLen = 0, int maxLen = int.MaxValue)
            => ReadBool(stream, version, pool) ? ReadArray<T>(stream, version, pool, minLen, maxLen) : default(T[]?);

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static async Task<T[]?> ReadArrayNullableAsync<T>(
            this Stream stream,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            CancellationToken cancellationToken = default
            )
            => await ReadBoolAsync(stream, version, pool, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)
                ? await ReadArrayAsync<T>(stream, version, pool, minLen, maxLen, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)
                : default(T[]?);

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <returns>Value</returns>
        public static List<T> ReadList<T>(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, int minLen = 0, int maxLen = int.MaxValue)
        {
            if (typeof(T) == typeof(byte)) return new List<T>((ReadBytes(stream, version, buffer: null, pool, minLen, maxLen) as T[])!);
            try
            {
                int len = ReadNumber<int>(stream, version, pool);
                SerializerHelper.EnsureValidLength(len, minLen, maxLen);
                List<T> res = new(len);
                for (int i = 0; i < len; res.Add(ReadObject<T>(stream, version)), i++) ;
                return res;
            }
            catch (SerializerException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static async Task<List<T>> ReadListAsync<T>(
            this Stream stream,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            CancellationToken cancellationToken = default
            )
        {
            if (typeof(T) == typeof(byte))
                return new List<T>((await ReadBytesAsync(stream, version, buffer: null, pool, minLen, maxLen, cancellationToken).ConfigureAwait(continueOnCapturedContext: false) as T[])!);
            try
            {
                int len = await ReadNumberAsync<int>(stream, version, pool, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
                SerializerHelper.EnsureValidLength(len, minLen, maxLen);
                List<T> res = new(len);
                for (int i = 0; i < len; res.Add(await ReadObjectAsync<T>(stream, version, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)), i++) ;
                return res;
            }
            catch (SerializerException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <returns>Value</returns>
        public static List<T>? ReadListNullable<T>(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, int minLen = 0, int maxLen = int.MaxValue)
            => ReadBool(stream, version, pool) ? ReadList<T>(stream, version, pool, minLen, maxLen) : default(List<T>?);

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static async Task<List<T>?> ReadListNullableAsync<T>(
            this Stream stream,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            CancellationToken cancellationToken = default
            )
            => await ReadBoolAsync(stream, version, pool, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)
                ? await ReadListAsync<T>(stream, version, pool, minLen, maxLen, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)
                : default(List<T>?);

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="tKey">Key type</typeparam>
        /// <typeparam name="tValue">Value type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <returns>Value</returns>
        public static Dictionary<tKey, tValue> ReadDict<tKey, tValue>(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, int minLen = 0, int maxLen = int.MaxValue)
            where tKey : notnull
        {
            try
            {
                int len = ReadNumber<int>(stream, version, pool);
                SerializerHelper.EnsureValidLength(len, minLen, maxLen);
                Dictionary<tKey, tValue> res = new(len);
                for (int i = 0; i < len; i++)
                    res[ReadObject<tKey>(stream, version)]
                        = ReadObject<tValue>(stream, version);
                return res;
            }
            catch (SerializerException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="tKey">Key type</typeparam>
        /// <typeparam name="tValue">Value type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static async Task<Dictionary<tKey, tValue>> ReadDictAsync<tKey, tValue>(
            this Stream stream,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            CancellationToken cancellationToken = default
            )
            where tKey : notnull
        {
            try
            {
                int len = await ReadNumberAsync<int>(stream, version, pool, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
                SerializerHelper.EnsureValidLength(len, minLen, maxLen);
                Dictionary<tKey, tValue> res = new(len);
                for (int i = 0; i < len; i++)
                    res[await ReadObjectAsync<tKey>(stream, version, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)]
                        = await ReadObjectAsync<tValue>(stream, version, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
                return res;
            }
            catch (SerializerException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="tKey">Key type</typeparam>
        /// <typeparam name="tValue">Value type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <returns>Value</returns>
        public static Dictionary<tKey, tValue>? ReadDictNullable<tKey, tValue>(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, int minLen = 0, int maxLen = int.MaxValue)
            where tKey : notnull
            => ReadBool(stream, version, pool) ? ReadDict<tKey, tValue>(stream, version, pool, minLen, maxLen) : default(Dictionary<tKey, tValue>?);

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="tKey">Key type</typeparam>
        /// <typeparam name="tValue">Value type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static async Task<Dictionary<tKey, tValue>?> ReadDictNullableAsync<tKey, tValue>(
            this Stream stream,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            CancellationToken cancellationToken = default
            )
            where tKey : notnull
            => await ReadBoolAsync(stream, version, pool, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)
                ? await ReadDictAsync<tKey, tValue>(stream, version, pool, minLen, maxLen, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)
                : default(Dictionary<tKey, tValue>?);

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <returns>Object</returns>
        public static T ReadSerialized<T>(this Stream stream, int? version = null) where T : class, IStreamSerializer
        {
            Type type = typeof(T);
            if (type.IsAbstract || type.IsInterface || type.IsGenericTypeDefinition) throw new SerializerException($"Type {type} isn't a supported deserializer type");
            ConstructorInfo ci = (from c in type.GetConstructors()
                                  where c.IsPublic &&
                                    (
                                        c.GetParameters().Length == 0 ||
                                        (c.GetParameters().Length == 2 && c.GetParameters()[0].ParameterType == typeof(Stream) && c.GetParameters()[1].ParameterType == typeof(int))
                                    )
                                  select c)
                                  .OrderBy(c => c.GetParameters().Length)
                                  .FirstOrDefault()
                                  ?? throw new SerializerException($"Failed to find the serializer constructor of type {type}");
            bool serializerConstructor = ci.GetParameters().Length > 0;
            T res = (T)(serializerConstructor ? ci.Invoke(new object?[] { stream, version ?? StreamSerializer.Version }) : ci.Invoke(Array.Empty<object?>()));
            if (!serializerConstructor) res.Deserialize(stream, version ?? StreamSerializer.Version);
            return res;
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Object type</param>
        /// <param name="version">Serializer version</param>
        /// <returns>Object</returns>
        public static object ReadSerializedObject(this Stream stream, Type type, int? version = null)
            => ReadSerializedMethod.MakeGenericMethod(type).InvokeAuto(obj: null, stream, version)!;

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Object</returns>
        public static async Task<T> ReadSerializedAsync<T>(this Stream stream, int? version = null, CancellationToken cancellationToken = default) where T : class, IStreamSerializer
        {
            Type type = typeof(T);
            if (type.IsAbstract || type.IsInterface || type.IsGenericTypeDefinition) throw new SerializerException($"Type {type} isn't a supported deserializer type");
            ConstructorInfo ci = (from c in type.GetConstructors()
                                  where c.IsPublic &&
                                    (
                                        c.GetParameters().Length == 0 ||
                                        (c.GetParameters().Length == 2 && c.GetParameters()[0].ParameterType == typeof(Stream) && c.GetParameters()[1].ParameterType == typeof(int))
                                    )
                                  select c)
                                  .OrderBy(c => c.GetParameters().Length)
                                  .FirstOrDefault()
                                  ?? throw new SerializerException($"Failed to find the serializer constructor of type {type}");
            bool serializerConstructor = ci.GetParameters().Length > 0;
            T res = (T)(serializerConstructor ? ci.Invoke(new object?[] { stream, version ?? StreamSerializer.Version }) : ci.Invoke(Array.Empty<object?>()));
            if (!serializerConstructor) await res.DeserializeAsync(stream, version ?? StreamSerializer.Version, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
            return res;
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Type</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Object</returns>
        public static async Task<object> ReadSerializedObjectAsync(this Stream stream, Type type, int? version = null, CancellationToken cancellationToken = default)
        {
            Task task = (Task)ReadSerializedAsyncMethod.MakeGenericMethod(type).InvokeAuto(obj: null, stream, version, cancellationToken)!;
            await task.ConfigureAwait(continueOnCapturedContext: false);
            return task.GetResult(type);
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <returns>Object</returns>
        public static T? ReadSerializedNullable<T>(this Stream stream, int? version = null) where T : class, IStreamSerializer
            => ReadBool(stream, version) ? ReadSerialized<T>(stream, version) : default(T?);

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Object</returns>
        public static async Task<T?> ReadSerializedNullableAsync<T>(this Stream stream, int? version = null, CancellationToken cancellationToken = default) where T : class, IStreamSerializer
            => await ReadBoolAsync(stream, version, cancellationToken: cancellationToken).ConfigureAwait(continueOnCapturedContext: false)
                ? await ReadSerializedAsync<T>(stream, version, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)
                : default(T?);

        /// <summary>
        /// Read any object
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <returns>Object</returns>
        public static T ReadAnyObject<T>(this Stream stream, int? version = null) where T : class, new()
        {
            Type type = typeof(T);
            if (typeof(IStreamSerializer).IsAssignableFrom(type)) return (T)ReadSerializedMethod.MakeGenericMethod(type).InvokeAuto(obj: null, stream, version)!;
            StreamSerializerAttribute? attr = type.GetCustomAttribute<StreamSerializerAttribute>(),
                objAttr;
            if (AnyObjectAttributeRequired && attr == null) throw new SerializerException($"Deserialization of {typeof(T)} requires the {typeof(StreamSerializerAttribute)}");
            PropertyInfo[] pis = StreamSerializerAttribute.GetReadProperties(type, ReadNumberNullable<int>(stream, version)).ToArray();
            int count = ReadNumber<int>(stream, version),
                done = 0;
            if (count != pis.Length) throw new SerializerException($"The serialized type has only {count} properties, while {type} has {pis.Length} properties");
            bool useChecksum = !(attr?.SkipPropertyNameChecksum ?? false);
            T res = new();
            for (; done < count; done++)
            {
                objAttr = pis[done].GetCustomAttribute<StreamSerializerAttribute>();
                if (useChecksum && !(objAttr?.SkipPropertyNameChecksum ?? false) && ReadOneByte(stream, version) != Encoding.UTF8.GetBytes(pis[done].Name).Aggregate((c, b) => (byte)(c ^ b)))
                    throw new SerializerException($"{type}.{pis[done].Name} property name checksum mismatch");
                pis[done].SetValue(
                    res,
                    Nullable.GetUnderlyingType(pis[done].PropertyType) == null
                        ? ReadAnyMethod.InvokeAuto(obj: null, stream, version)
                        : ReadAnyNullableMethod.InvokeAuto(obj: null, stream, version)
                    );
            }
            return res;
        }

        /// <summary>
        /// Read any object
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Object</returns>
        public static async Task<T> ReadAnyObjectAsync<T>(this Stream stream, int? version = null, CancellationToken cancellationToken = default) where T : class, new()
        {
            Type type = typeof(T);
            Task task;
            if (typeof(IStreamSerializer).IsAssignableFrom(type))
            {
                task = (Task)ReadSerializedAsyncMethod.MakeGenericMethod(type).InvokeAuto(obj: null, stream, version, cancellationToken)!;
                await task.ConfigureAwait(continueOnCapturedContext: false);
                return task.GetResult<T>();
            }
            StreamSerializerAttribute? attr = type.GetCustomAttribute<StreamSerializerAttribute>(),
                objAttr;
            if (AnyObjectAttributeRequired && attr == null) throw new SerializerException($"Deserialization of {typeof(T)} requires the {typeof(StreamSerializerAttribute)}");
            PropertyInfo[] pis = StreamSerializerAttribute.GetReadProperties(type, ReadNumberNullable<int>(stream, version)).ToArray();
            int count = ReadNumber<int>(stream, version),
                done = 0;
            if (count != pis.Length) throw new SerializerException($"The serialized type has only {count} properties, while {type} has {pis.Length} properties");
            bool useChecksum = !(attr?.SkipPropertyNameChecksum ?? false),
                isNullable;
            T res = new();
            for (; done < count; done++)
            {
                objAttr = pis[done].GetCustomAttribute<StreamSerializerAttribute>();
                if (useChecksum && !(objAttr?.SkipPropertyNameChecksum ?? false) && ReadOneByte(stream, version) != Encoding.UTF8.GetBytes(pis[done].Name).Aggregate((c, b) => (byte)(c ^ b)))
                    throw new SerializerException($"{type}.{pis[done].Name} property name checksum mismatch");
                isNullable = Nullable.GetUnderlyingType(pis[done].PropertyType) == null;
                task = (Task)(isNullable
                        ? ReadAnyAsyncMethod.InvokeAuto(obj: null, stream, version)
                        : ReadAnyNullableAsyncMethod.InvokeAuto(obj: null, stream, version))!;
                await task.ConfigureAwait(continueOnCapturedContext: false);
                pis[done].SetValue(res, isNullable ? task.GetResultNullable<object>() : task.GetResult<object>());
            }
            return res;
        }

        /// <summary>
        /// Read any object
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <returns>Object</returns>
        public static T? ReadAnyObjectNullable<T>(this Stream stream, int? version = null) where T : class, new()
            => ReadBool(stream, version) ? ReadAnyObject<T>(stream, version) : null;

        /// <summary>
        /// Read any object
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Object</returns>
        public static async Task<T?> ReadAnyObjectNullableAsync<T>(this Stream stream, int? version = null, CancellationToken cancellationToken = default)
            where T : class, new()
            => await ReadBoolAsync(stream, version, cancellationToken: cancellationToken).ConfigureAwait(continueOnCapturedContext: false)
                ? await ReadAnyObjectAsync<T>(stream, version, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)
                : null;

        /// <summary>
        /// Read serialized data
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="len">Length in bytes</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Serialized data (a pool array which needs to be returned to the pool after use and might be larger than the given length!)</returns>
        public static byte[] ReadSerializedData(this Stream stream, int len, ArrayPool<byte>? pool = null)
        {
            byte[] res = (pool ?? ArrayPool<byte>.Shared).Rent(len);
            try
            {
                if (stream.Read(res.AsSpan(0, len)) != len) throw new SerializerException($"Failed to read serialized data ({len} bytes)");
                return res;
            }
            catch
            {
                (pool ?? ArrayPool<byte>.Shared).Return(res);
                throw;
            }
        }

        /// <summary>
        /// Read serialized data
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="len">Length in bytes</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Serialized data (a pool array which needs to be returned to the pool after use and might be larger than the given length!)</returns>
        public static async Task<byte[]> ReadSerializedDataAsync(this Stream stream, int len, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
        {
            byte[] res = (pool ?? ArrayPool<byte>.Shared).Rent(len);
            try
            {
                if (await stream.ReadAsync(res.AsMemory(0, len), cancellationToken).ConfigureAwait(continueOnCapturedContext: false) != len)
                    throw new SerializerException($"Failed to read serialized data ({len} bytes)");
                return res;
            }
            catch
            {
                (pool ?? ArrayPool<byte>.Shared).Return(res);
                throw;
            }
        }
    }
}
