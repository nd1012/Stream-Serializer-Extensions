using System.Buffers;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;
using wan24.Core;
using wan24.ObjectValidation;

namespace wan24.StreamSerializerExtensions
{
    // Read
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
        /// Read the serializer version
        /// </summary>
        /// <param name="stream">Steam</param>
        /// <returns>Serializer version</returns>
        public static int ReadSerializerVersion(this Stream stream)
        {
            int res = ReadNumber<int>(stream, version: 1);
            if (res < 1 || res > StreamSerializer.VERSION) throw new InvalidDataException($"Invalid or unsupported stream serializer version #{res}");
            return res;
        }

        /// <summary>
        /// Read the serializer version
        /// </summary>
        /// <param name="stream">Steam</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Serializer version</returns>
        public static async Task<int> ReadSerializerVersionAsync(this Stream stream, CancellationToken cancellationToken = default)
        {
            int res = await ReadNumberAsync<int>(stream, version: 1, cancellationToken: cancellationToken).DynamicContext();
            if (res < 1 || res > StreamSerializer.VERSION) throw new InvalidDataException($"Invalid or unsupported stream serializer version #{res}");
            return res;
        }

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
                return (T)await ReadSerializedObjectAsync(stream, typeof(T), version, cancellationToken).DynamicContext();
            if (StreamSerializer.FindAsyncDeserializer(typeof(T)) is not StreamSerializer.AsyncDeserialize_Delegate deserializer)
            {
                await Task.Yield();
                return ReadObject<T>(stream, version);
            }
            try
            {
                Task task = deserializer(stream, typeof(T), version ?? StreamSerializer.Version, cancellationToken);
                await task.DynamicContext();
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
#pragma warning disable IDE0034 // default expression can be simplified
            => ReadBool(stream, version) ? ReadObject<T>(stream, version) : default(T?);
#pragma warning restore IDE0034 // default expression can be simplified

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static async Task<T?> ReadObjectNullableAsync<T>(this Stream stream, int? version = null, CancellationToken cancellationToken = default)
            => await ReadBoolAsync(stream, version, cancellationToken: cancellationToken).DynamicContext()
                ? await ReadObjectAsync<T>(stream, version, cancellationToken).DynamicContext()
#pragma warning disable IDE0034 // default expression can be simplified
                : default(T?);
#pragma warning restore IDE0034 // default expression can be simplified

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
                byte[] data = await ReadSerializedDataAsync(stream, len: 1, cancellationToken: cancellationToken).DynamicContext();
                try
                {
                    objType = (ObjectTypes)data[0];
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(data);
                }
                return await ReadAnyIntAsync(stream, version, objType, cancellationToken).DynamicContext();
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
                    return isEmpty ? string.Empty : await ReadStringAsync(stream, version, cancellationToken: cancellationToken).DynamicContext();
                case ObjectTypes.Bytes:
                    return isEmpty ? Array.Empty<byte>() : (await ReadBytesAsync(stream, version, cancellationToken: cancellationToken).DynamicContext()).Value;
            }
            type ??= StreamSerializer.LoadType(await ReadStringAsync(stream, version, pool: null, minLen: 1, maxLen: short.MaxValue, cancellationToken).DynamicContext());
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
                    return await ReadSerializedObjectAsync(stream, type, version, cancellationToken)!.DynamicContext();
                default:
                    throw new InvalidProgramException();
            }
            await task.DynamicContext();
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
                byte[] data = await ReadSerializedDataAsync(stream, len: 1, cancellationToken: cancellationToken).DynamicContext();
                try
                {
                    objType = (ObjectTypes)data[0];
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(data);
                }
                return objType == ObjectTypes.Null ? null : await ReadAnyIntAsync(stream, version, objType, cancellationToken).DynamicContext();
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
#pragma warning disable IDE0060 // Remove unused parameter
        public static bool ReadBool(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
#pragma warning restore IDE0060
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
#pragma warning disable IDE0060 // Remove unused parameter
        public static async Task<bool> ReadBoolAsync(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            byte[] data = await ReadSerializedDataAsync(stream, len: 1, pool, cancellationToken: cancellationToken).DynamicContext();
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
            => await ReadBoolAsync(stream, version, pool, cancellationToken).DynamicContext()
                ? await ReadBoolAsync(stream, version, pool, cancellationToken).DynamicContext()
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
#pragma warning disable IDE0060 // Remove unused parameter
        public static async Task<sbyte> ReadOneSByteAsync(this Stream stream, int? version = null, CancellationToken cancellationToken = default)
#pragma warning restore IDE0060 // Remove unused parameter
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
            => await ReadBoolAsync(stream, version, cancellationToken: cancellationToken).DynamicContext()
                ? await ReadOneSByteAsync(stream, version, cancellationToken).DynamicContext()
                : default(sbyte?);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused parameter
        public static byte ReadOneByte(this Stream stream, int? version = null)
#pragma warning restore IDE0060 // Remove unused parameter
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
#pragma warning disable IDE0060 // Remove unused parameter
        public static async Task<byte> ReadOneByteAsync(this Stream stream, int? version = null, CancellationToken cancellationToken = default)
#pragma warning restore IDE0060 // Remove unused parameter
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
            => await ReadBoolAsync(stream, version, cancellationToken: cancellationToken).DynamicContext()
                ? await ReadOneByteAsync(stream, version, cancellationToken).DynamicContext()
                : default(ushort?);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused parameter
        public static short ReadShort(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            byte[] data = ReadSerializedData(stream, len: sizeof(short), pool);
            try
            {
                return data.AsSpan().ToShort();
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
#pragma warning disable IDE0060 // Remove unused parameter
        public static async Task<short> ReadShortAsync(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            byte[] data = await ReadSerializedDataAsync(stream, len: sizeof(short), pool, cancellationToken).DynamicContext();
            try
            {
                return data.AsSpan().ToShort();
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
            => await ReadBoolAsync(stream, version, pool, cancellationToken).DynamicContext()
                ? await ReadShortAsync(stream, version, pool, cancellationToken).DynamicContext()
                : default(short?);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused parameter
        public static ushort ReadUShort(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            byte[] data = ReadSerializedData(stream, len: sizeof(ushort), pool);
            try
            {
                return data.AsSpan().ToUShort();
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
#pragma warning disable IDE0060 // Remove unused parameter
        public static async Task<ushort> ReadUShortAsync(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            byte[] data = await ReadSerializedDataAsync(stream, len: sizeof(ushort), pool, cancellationToken).DynamicContext();
            try
            {
                return data.AsSpan().ToUShort();
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
            => await ReadBoolAsync(stream, version, pool, cancellationToken).DynamicContext()
                ? await ReadUShortAsync(stream, version, pool, cancellationToken).DynamicContext()
                : default(ushort?);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused parameter
        public static int ReadInt(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            byte[] data = ReadSerializedData(stream, len: sizeof(int), pool);
            try
            {
                return data.AsSpan().ToInt();
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
#pragma warning disable IDE0060 // Remove unused parameter
        public static async Task<int> ReadIntAsync(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            byte[] data = await ReadSerializedDataAsync(stream, len: sizeof(int), pool, cancellationToken).DynamicContext();
            try
            {
                return data.AsSpan().ToInt();
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
            => await ReadBoolAsync(stream, version, pool, cancellationToken).DynamicContext()
                ? await ReadIntAsync(stream, version, pool, cancellationToken).DynamicContext()
                : default(int?);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused parameter
        public static uint ReadUInt(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            byte[] data = ReadSerializedData(stream, len: sizeof(uint), pool);
            try
            {
                return data.AsSpan().ToUInt();
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
#pragma warning disable IDE0060 // Remove unused parameter
        public static async Task<uint> ReadUIntAsync(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            byte[] data = await ReadSerializedDataAsync(stream, len: sizeof(uint), pool, cancellationToken).DynamicContext();
            try
            {
                return data.AsSpan().ToUInt();
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
            => await ReadBoolAsync(stream, version, pool, cancellationToken).DynamicContext()
                ? await ReadUIntAsync(stream, version, pool, cancellationToken).DynamicContext()
                : default(uint?);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused parameter
        public static long ReadLong(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            byte[] data = ReadSerializedData(stream, len: sizeof(long), pool);
            try
            {
                return data.AsSpan().ToLong();
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
#pragma warning disable IDE0060 // Remove unused parameter
        public static async Task<long> ReadLongAsync(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            byte[] data = await ReadSerializedDataAsync(stream, len: sizeof(long), pool, cancellationToken).DynamicContext();
            try
            {
                return data.AsSpan().ToLong();
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
            => await ReadBoolAsync(stream, version, pool, cancellationToken).DynamicContext()
                ? await ReadLongAsync(stream, version, pool, cancellationToken).DynamicContext()
                : default(long?);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused parameter
        public static ulong ReadULong(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            byte[] data = ReadSerializedData(stream, len: sizeof(ulong), pool);
            try
            {
                return data.AsSpan().ToULong();
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
#pragma warning disable IDE0060 // Remove unused parameter
        public static async Task<ulong> ReadULongAsync(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            byte[] data = await ReadSerializedDataAsync(stream, len: sizeof(ulong), pool, cancellationToken).DynamicContext();
            try
            {
                return data.AsSpan().ToULong();
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
            => await ReadBoolAsync(stream, version, pool, cancellationToken).DynamicContext()
                ? await ReadULongAsync(stream, version, pool, cancellationToken).DynamicContext()
                : default(ulong?);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused parameter
        public static float ReadFloat(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            byte[] data = ReadSerializedData(stream, len: sizeof(float), pool);
            try
            {
                return data.AsSpan().ToFloat();
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
#pragma warning disable IDE0060 // Remove unused parameter
        public static async Task<float> ReadFloatAsync(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            byte[] data = await ReadSerializedDataAsync(stream, len: sizeof(float), pool, cancellationToken).DynamicContext();
            try
            {
                return data.AsSpan().ToFloat();
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
            => await ReadBoolAsync(stream, version, pool, cancellationToken).DynamicContext()
                ? await ReadFloatAsync(stream, version, pool, cancellationToken).DynamicContext()
                : default(float?);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused parameter
        public static double ReadDouble(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            byte[] data = ReadSerializedData(stream, len: sizeof(double), pool);
            try
            {
                return data.AsSpan().ToDouble();
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
#pragma warning disable IDE0060 // Remove unused parameter
        public static async Task<double> ReadDoubleAsync(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            byte[] data = await ReadSerializedDataAsync(stream, len: sizeof(double), pool, cancellationToken).DynamicContext();
            try
            {
                return data.AsSpan().ToDouble();
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
            => await ReadBoolAsync(stream, version, pool, cancellationToken).DynamicContext()
                ? await ReadDoubleAsync(stream, version, pool, cancellationToken).DynamicContext()
                : default(double?);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
#pragma warning disable IDE0060 // Remove unused parameter
        public static decimal ReadDecimal(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            byte[] data = ReadSerializedData(stream, len: sizeof(int) << 2, pool);
            try
            {
                return data.AsSpan().ToDecimal();
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
#pragma warning disable IDE0060 // Remove unused parameter
        public static async Task<decimal> ReadDecimalAsync(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            byte[] data = await ReadSerializedDataAsync(stream, len: sizeof(int) << 2, pool, cancellationToken).DynamicContext();
            try
            {
                return data.AsSpan().ToDecimal();
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
            => await ReadBoolAsync(stream, version, pool, cancellationToken).DynamicContext()
                ? await ReadDecimalAsync(stream, version, pool, cancellationToken).DynamicContext()
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
                        return data[0].ConvertType<T>();
                    case NumberTypes.Byte | NumberTypes.Unsigned:
                        switch (type)
                        {
                            case NumberTypes.Byte | NumberTypes.MaxValue | NumberTypes.Unsigned:
                                return (T)(object)byte.MaxValue;
                        }
                        if (stream.Read(data.AsSpan(0, 1)) != 1) throw new SerializerException("Failed to read serialized data (1 bytes)");
                        return data[0].ConvertType<T>();
                    case NumberTypes.Short:
                        return type switch
                        {
                            NumberTypes.Short | NumberTypes.MinValue => (T)(object)short.MinValue,
                            NumberTypes.Short | NumberTypes.MaxValue => (T)(object)short.MaxValue,
                            _ => ReadShort(stream, version, pool).ConvertType<T>()
                        };
                    case NumberTypes.Short | NumberTypes.Unsigned:
                        return type switch
                        {
                            NumberTypes.Short | NumberTypes.MaxValue | NumberTypes.Unsigned => (T)(object)ushort.MaxValue,
                            _ => ReadUShort(stream, version, pool).ConvertType<T>()
                        };
                    case NumberTypes.Int:
                        return type switch
                        {
                            NumberTypes.Int | NumberTypes.MinValue => (T)(object)int.MinValue,
                            NumberTypes.Int | NumberTypes.MaxValue => (T)(object)int.MaxValue,
                            _ => ReadInt(stream, version, pool).ConvertType<T>()
                        };
                    case NumberTypes.Int | NumberTypes.Unsigned:
                        return type switch
                        {
                            NumberTypes.Int | NumberTypes.MaxValue | NumberTypes.Unsigned => (T)(object)uint.MaxValue,
                            _ => ReadUInt(stream, version, pool).ConvertType<T>()
                        };
                    case NumberTypes.Long:
                        return type switch
                        {
                            NumberTypes.Long | NumberTypes.MinValue => (T)(object)long.MinValue,
                            NumberTypes.Long | NumberTypes.MaxValue => (T)(object)long.MaxValue,
                            _ => ReadLong(stream, version, pool).ConvertType<T>()
                        };
                    case NumberTypes.Long | NumberTypes.Unsigned:
                        return type switch
                        {
                            NumberTypes.Long | NumberTypes.MaxValue | NumberTypes.Unsigned => (T)(object)ulong.MaxValue,
                            _ => ReadULong(stream, version, pool).ConvertType<T>()
                        };
                    case NumberTypes.Float:
                        return type switch
                        {
                            NumberTypes.Float | NumberTypes.MinValue => (T)(object)float.MinValue,
                            NumberTypes.Float | NumberTypes.MaxValue => (T)(object)float.MaxValue,
                            _ => ReadFloat(stream, version, pool).ConvertType<T>()
                        };
                    case NumberTypes.Double:
                        return type switch
                        {
                            NumberTypes.Double | NumberTypes.MinValue => (T)(object)double.MinValue,
                            NumberTypes.Double | NumberTypes.MaxValue => (T)(object)double.MaxValue,
                            _ => ReadDouble(stream, version, pool).ConvertType<T>()
                        };
                    case NumberTypes.Decimal:
                        return type switch
                        {
                            NumberTypes.Decimal | NumberTypes.MinValue => (T)(object)decimal.MinValue,
                            NumberTypes.Decimal | NumberTypes.MaxValue => (T)(object)decimal.MaxValue,
                            _ => ReadDecimal(stream, version, pool).ConvertType<T>()
                        };
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
            byte[] data = await ReadSerializedDataAsync(stream, len: 1, pool, cancellationToken).DynamicContext();
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
                        if (await stream.ReadAsync(data.AsMemory(0, 1), cancellationToken).DynamicContext() != 1)
                            throw new SerializerException("Failed to read serialized data (1 bytes)");
                        return data[0].ConvertType<T>();
                    case NumberTypes.Byte | NumberTypes.Unsigned:
                        switch (type)
                        {
                            case NumberTypes.Byte | NumberTypes.MaxValue | NumberTypes.Unsigned:
                                return (T)(object)byte.MaxValue;
                        }
                        if (await stream.ReadAsync(data.AsMemory(0, 1), cancellationToken).DynamicContext() != 1)
                            throw new SerializerException("Failed to read serialized data (1 bytes)");
                        return data[0].ConvertType<T>();
                    case NumberTypes.Short:
                        return type switch
                        {
                            NumberTypes.Short | NumberTypes.MinValue => (T)(object)short.MinValue,
                            NumberTypes.Short | NumberTypes.MaxValue => (T)(object)short.MaxValue,
                            _ => (await ReadShortAsync(stream, version, pool, cancellationToken).DynamicContext()).ConvertType<T>()
                        };
                    case NumberTypes.Short | NumberTypes.Unsigned:
                        return type switch
                        {
                            NumberTypes.Short | NumberTypes.MaxValue | NumberTypes.Unsigned => (T)(object)ushort.MaxValue,
                            _ => (await ReadUShortAsync(stream, version, pool, cancellationToken).DynamicContext()).ConvertType<T>()
                        };
                    case NumberTypes.Int:
                        return type switch
                        {
                            NumberTypes.Int | NumberTypes.MinValue => (T)(object)int.MinValue,
                            NumberTypes.Int | NumberTypes.MaxValue => (T)(object)int.MaxValue,
                            _ => (await ReadIntAsync(stream, version, pool, cancellationToken).DynamicContext()).ConvertType<T>()
                        };
                    case NumberTypes.Int | NumberTypes.Unsigned:
                        return type switch
                        {
                            NumberTypes.Int | NumberTypes.MaxValue | NumberTypes.Unsigned => (T)(object)uint.MaxValue,
                            _ => (await ReadUIntAsync(stream, version, pool, cancellationToken).DynamicContext()).ConvertType<T>()
                        };
                    case NumberTypes.Long:
                        return type switch
                        {
                            NumberTypes.Long | NumberTypes.MinValue => (T)(object)long.MinValue,
                            NumberTypes.Long | NumberTypes.MaxValue => (T)(object)long.MaxValue,
                            _ => (await ReadLongAsync(stream, version, pool, cancellationToken).DynamicContext()).ConvertType<T>()
                        };
                    case NumberTypes.Long | NumberTypes.Unsigned:
                        return type switch
                        {
                            NumberTypes.Long | NumberTypes.MaxValue | NumberTypes.Unsigned => (T)(object)ulong.MaxValue,
                            _ => (await ReadULongAsync(stream, version, pool, cancellationToken).DynamicContext()).ConvertType<T>()
                        };
                    case NumberTypes.Float:
                        return type switch
                        {
                            NumberTypes.Float | NumberTypes.MinValue => (T)(object)float.MinValue,
                            NumberTypes.Float | NumberTypes.MaxValue => (T)(object)float.MaxValue,
                            _ => (await ReadFloatAsync(stream, version, pool, cancellationToken).DynamicContext()).ConvertType<T>()
                        };
                    case NumberTypes.Double:
                        return type switch
                        {
                            NumberTypes.Double | NumberTypes.MinValue => (T)(object)double.MinValue,
                            NumberTypes.Double | NumberTypes.MaxValue => (T)(object)double.MaxValue,
                            _ => (await ReadDoubleAsync(stream, version, pool, cancellationToken).DynamicContext()).ConvertType<T>()
                        };
                    case NumberTypes.Decimal:
                        return type switch
                        {
                            NumberTypes.Decimal | NumberTypes.MinValue => (T)(object)decimal.MinValue,
                            NumberTypes.Decimal | NumberTypes.MaxValue => (T)(object)decimal.MaxValue,
                            _ => (await ReadDecimalAsync(stream, version, pool, cancellationToken).DynamicContext()).ConvertType<T>()
                        };
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
            => await ReadBoolAsync(stream, version, pool, cancellationToken).DynamicContext()
                ? await ReadNumberAsync<T>(stream, version, pool, cancellationToken).DynamicContext()
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
                if (!res.IsValid()) throw new SerializerException($"Unknown enumeration value {res} for {typeof(T)}");
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
                await task.DynamicContext();
                T res = (T)Enum.ToObject(typeof(T), task.GetResult(type));
                if (!res.IsValid()) throw new SerializerException($"Unknown enumeration value {res} for {typeof(T)}");
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
            => await ReadBoolAsync(stream, version, pool, cancellationToken).DynamicContext()
                ? await ReadEnumAsync<T>(stream, version, pool, cancellationToken).DynamicContext()
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
                int len = await ReadNumberAsync<int>(stream, version, pool, cancellationToken).DynamicContext();
                SerializerHelper.EnsureValidLength(len, minLen, maxLen);
                if (len == 0 && buffer == null) buffer = Array.Empty<byte>();
                rented = buffer == null && pool != null;
                buffer ??= rented ? pool!.Rent(len) : new byte[len];
                if (len != 0 && await stream.ReadAsync(buffer.AsMemory(0, len), cancellationToken).DynamicContext() != len)
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
            => await ReadBoolAsync(stream, version, pool, cancellationToken).DynamicContext()
                ? await ReadBytesAsync(stream, version, buffer, pool, minLen, maxLen, cancellationToken).DynamicContext()
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
                return data.AsSpan(0, len).ToUtf8String();
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
            (byte[] data, int len) = await ReadBytesAsync(stream, version, buffer: null, pool, minLen, maxLen, cancellationToken).DynamicContext();
            try
            {
                return data.AsSpan(0, len).ToUtf8String();
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
#pragma warning disable IDE0034 // default expression can be simplified
            => ReadBool(stream, version, pool) ? ReadString(stream, version, pool, minLen, maxLen) : default(string?);
#pragma warning restore IDE0034 // default expression can be simplified

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
            => await ReadBoolAsync(stream, version, pool, cancellationToken).DynamicContext()
                ? await ReadStringAsync(stream, version, pool, minLen, maxLen, cancellationToken).DynamicContext()
#pragma warning disable IDE0034 // default expression can be simplified
                : default(string?);
#pragma warning restore IDE0034 // default expression can be simplified

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
                return (await ReadBytesAsync(stream, version, buffer: null, pool, minLen, maxLen, cancellationToken).DynamicContext() as T[])!;
            try
            {
                int len = await ReadNumberAsync<int>(stream, version, pool, cancellationToken).DynamicContext();
                SerializerHelper.EnsureValidLength(len, minLen, maxLen);
                T[] res = new T[len];
                for (int i = 0; i < len; res[i] = await ReadObjectAsync<T>(stream, version, cancellationToken).DynamicContext(), i++) ;
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
#pragma warning disable IDE0034 // default expression can be simplified
            => ReadBool(stream, version, pool) ? ReadArray<T>(stream, version, pool, minLen, maxLen) : default(T[]?);
#pragma warning restore IDE0034 // default expression can be simplified

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
            => await ReadBoolAsync(stream, version, pool, cancellationToken).DynamicContext()
                ? await ReadArrayAsync<T>(stream, version, pool, minLen, maxLen, cancellationToken).DynamicContext()
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
                return new List<T>((await ReadBytesAsync(stream, version, buffer: null, pool, minLen, maxLen, cancellationToken).DynamicContext() as T[])!);
            try
            {
                int len = await ReadNumberAsync<int>(stream, version, pool, cancellationToken).DynamicContext();
                SerializerHelper.EnsureValidLength(len, minLen, maxLen);
                List<T> res = new(len);
                for (int i = 0; i < len; res.Add(await ReadObjectAsync<T>(stream, version, cancellationToken).DynamicContext()), i++) ;
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
#pragma warning disable IDE0034 // default expression can be simplified
            => ReadBool(stream, version, pool) ? ReadList<T>(stream, version, pool, minLen, maxLen) : default(List<T>?);
#pragma warning restore IDE0034 // default expression can be simplified

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
            => await ReadBoolAsync(stream, version, pool, cancellationToken).DynamicContext()
                ? await ReadListAsync<T>(stream, version, pool, minLen, maxLen, cancellationToken).DynamicContext()
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
                int len = await ReadNumberAsync<int>(stream, version, pool, cancellationToken).DynamicContext();
                SerializerHelper.EnsureValidLength(len, minLen, maxLen);
                Dictionary<tKey, tValue> res = new(len);
                for (int i = 0; i < len; i++)
                    res[await ReadObjectAsync<tKey>(stream, version, cancellationToken).DynamicContext()]
                        = await ReadObjectAsync<tValue>(stream, version, cancellationToken).DynamicContext();
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
#pragma warning disable IDE0034 // default expression can be simplified
            => ReadBool(stream, version, pool) ? ReadDict<tKey, tValue>(stream, version, pool, minLen, maxLen) : default(Dictionary<tKey, tValue>?);
#pragma warning restore IDE0034 // default expression can be simplified

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
            => await ReadBoolAsync(stream, version, pool, cancellationToken).DynamicContext()
                ? await ReadDictAsync<tKey, tValue>(stream, version, pool, minLen, maxLen, cancellationToken).DynamicContext()
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
            List<ValidationResult> results = new();
            if (!res.TryValidateObject(results))
                throw new SerializerException($"The deserialized object contains {results.Count} errors: {results[0].ErrorMessage} ({string.Join(',', results[0].MemberNames)})");
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
            if (!serializerConstructor) await res.DeserializeAsync(stream, version ?? StreamSerializer.Version, cancellationToken).DynamicContext();
            List<ValidationResult> results = new();
            if (!res.TryValidateObject(results))
                throw new SerializerException($"The deserialized object contains {results.Count} errors: {results[0].ErrorMessage} ({string.Join(',', results[0].MemberNames)})");
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
            await task.DynamicContext();
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
#pragma warning disable IDE0034 // default expression can be simplified
            => ReadBool(stream, version) ? ReadSerialized<T>(stream, version) : default(T?);
#pragma warning restore IDE0034 // default expression can be simplified

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Object</returns>
        public static async Task<T?> ReadSerializedNullableAsync<T>(this Stream stream, int? version = null, CancellationToken cancellationToken = default) where T : class, IStreamSerializer
            => await ReadBoolAsync(stream, version, cancellationToken: cancellationToken).DynamicContext()
                ? await ReadSerializedAsync<T>(stream, version, cancellationToken).DynamicContext()
#pragma warning disable IDE0034 // default expression can be simplified
                : default(T?);
#pragma warning restore IDE0034 // default expression can be simplified

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
            List<ValidationResult> results = new();
            if (!res.TryValidateObject(results))
                throw new SerializerException($"The deserialized object contains {results.Count} errors: {results[0].ErrorMessage} ({string.Join(',', results[0].MemberNames)})");
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
                await task.DynamicContext();
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
                await task.DynamicContext();
                pis[done].SetValue(res, isNullable ? task.GetResultNullable<object>() : task.GetResult<object>());
            }
            List<ValidationResult> results = new();
            if (!res.TryValidateObject(results))
                throw new SerializerException($"The deserialized object contains {results.Count} errors: {results[0].ErrorMessage} ({string.Join(',', results[0].MemberNames)})");
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
            => await ReadBoolAsync(stream, version, cancellationToken: cancellationToken).DynamicContext()
                ? await ReadAnyObjectAsync<T>(stream, version, cancellationToken).DynamicContext()
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
                if (await stream.ReadAsync(res.AsMemory(0, len), cancellationToken).DynamicContext() != len)
                    throw new SerializerException($"Failed to read serialized data ({len} bytes)");
                return res;
            }
            catch
            {
                (pool ?? ArrayPool<byte>.Shared).Return(res);
                throw;
            }
        }

        /// <summary>
        /// Read a stream
        /// </summary>
        /// <typeparam name="T">Target stream type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="target">Target stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="maxBufferSize">Maximum buffer size in bytes</param>
        /// <returns>Target stream</returns>
        public static T ReadStream<T>(this Stream stream, T target, int? version = null, ArrayPool<byte>? pool = null, int? maxBufferSize = null) where T : Stream
        {
            //TODO Test
            if (maxBufferSize != null && maxBufferSize.Value < 1) throw new ArgumentOutOfRangeException(nameof(maxBufferSize));
            long len = stream.ReadNumber<long>(version, pool);
            if (len == 0) throw new SerializerException("Invalid stream/chunk length", new InvalidDataException());
            if (len < 0)
            {
                len = Math.Abs(len);
                if (len > int.MaxValue) throw new SerializerException("Invalid chunk length", new InvalidDataException());
                if (len > (maxBufferSize ?? Settings.BufferSize))
                    throw new SerializerException($"Chunk length of {len} bytes exceeds max. buffer size of {maxBufferSize ?? Settings.BufferSize}", new InvalidDataException());
                using RentedArray<byte> buffer = new((int)len, pool);
                for (int red = (int)len; red == len;)
                {
                    red = stream.ReadBytes(version, buffer.Array, maxLen: buffer.Length).Length;
                    if (red < 1) break;
                    target.Write(buffer.Span[..red]);
                }
            }
            else
            {
                using RentedArray<byte> buffer = new(maxBufferSize ?? Settings.BufferSize, pool);
                long total = 0;
                for (int red = buffer.Length; red == len && total < len; total += red)
                {
                    red = stream.ReadBytes(version, buffer.Array, maxLen: buffer.Length).Length;
                    if (red < 1) break;
                    target.Write(buffer.Span[..red]);
                }
                if (total != len) throw new SerializerException($"Invalid chunk length", new IOException());
            }
            return target;
        }

        /// <summary>
        /// Read a stream
        /// </summary>
        /// <typeparam name="T">Target stream type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="target">Target stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="maxBufferSize">Maximum buffer size in bytes</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Target stream</returns>
        public static async Task<T> ReadStreamAsync<T>(
            this Stream stream,
            T target,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int? maxBufferSize = null,
            CancellationToken cancellationToken = default
            )
            where T : Stream
        {
            //TODO Test
            if (maxBufferSize != null && maxBufferSize.Value < 1) throw new ArgumentOutOfRangeException(nameof(maxBufferSize));
            long len = await stream.ReadNumberAsync<long>(version, pool, cancellationToken).DynamicContext();
            if (len == 0) throw new SerializerException("Invalid stream/chunk length", new InvalidDataException());
            if (len < 0)
            {
                len = Math.Abs(len);
                if (len > int.MaxValue) throw new SerializerException("Invalid chunk length", new InvalidDataException());
                if (len > (maxBufferSize ?? Settings.BufferSize))
                    throw new SerializerException($"Chunk length of {len} bytes exceeds max. buffer size of {maxBufferSize ?? Settings.BufferSize}", new InvalidDataException());
                using RentedArray<byte> buffer = new((int)len, pool);
                for (int red = (int)len; red == len;)
                {
                    red = (await stream.ReadBytesAsync(version, buffer.Array, maxLen: buffer.Length, cancellationToken: cancellationToken).DynamicContext()).Length;
                    if (red < 1) break;
                    await target.WriteAsync(buffer.Memory[..red], cancellationToken).DynamicContext();
                }
            }
            else
            {
                using RentedArray<byte> buffer = new(maxBufferSize ?? Settings.BufferSize, pool);
                long total = 0;
                for (int red = buffer.Length; red == len && total < len; total += red)
                {
                    red = (await stream.ReadBytesAsync(version, buffer.Array, maxLen: buffer.Length, cancellationToken: cancellationToken).DynamicContext()).Length;
                    if (red < 1) break;
                    await target.WriteAsync(buffer.Memory[..red], cancellationToken).DynamicContext();
                }
                if (total != len) throw new SerializerException($"Invalid chunk length", new IOException());
            }
            return target;
        }
    }
}
