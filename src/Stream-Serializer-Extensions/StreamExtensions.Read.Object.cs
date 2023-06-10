using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Runtime;
using System.Text;
using wan24.Core;
using wan24.ObjectValidation;

namespace wan24.StreamSerializerExtensions
{
    // Object
    public static partial class StreamExtensions
    {

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="options">Options</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static T ReadObject<T>(this Stream stream, int? version = null, ISerializerOptions? options = null)
        {
            if (typeof(IStreamSerializer).IsAssignableFrom(typeof(T))) return (T)ReadSerializedObject(stream, typeof(T), version);
            StreamSerializer.Deserialize_Delegate deserializer = StreamSerializer.FindDeserializer(typeof(T)) ?? throw new SerializerException("No deserializer found");
            try
            {
                return (T)(deserializer(stream, typeof(T), version ?? StreamSerializer.Version, options) ?? throw new SerializerException($"{typeof(T)} deserialized to NULL"));
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
        /// <param name="options">Options</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static async Task<T> ReadObjectAsync<T>(this Stream stream, int? version = null, ISerializerOptions? options = null, CancellationToken cancellationToken = default)
        {
            if (typeof(IStreamSerializer).IsAssignableFrom(typeof(T)))
                return (T)await ReadSerializedObjectAsync(stream, typeof(T), version, cancellationToken).DynamicContext();
            if (StreamSerializer.FindAsyncDeserializer(typeof(T)) is not StreamSerializer.AsyncDeserialize_Delegate deserializer)
            {
                await Task.Yield();
                return ReadObject<T>(stream, version, options);
            }
            try
            {
                Task task = deserializer(stream, typeof(T), version ?? StreamSerializer.Version, options, cancellationToken);
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
        /// <param name="options">Options</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static T? ReadObjectNullable<T>(this Stream stream, int? version = null, ISerializerOptions? options = null)
#pragma warning disable IDE0034 // default expression can be simplified
            => ReadBool(stream, version) ? ReadObject<T>(stream, version, options) : default(T?);
#pragma warning restore IDE0034 // default expression can be simplified

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="options">Options</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static async Task<T?> ReadObjectNullableAsync<T>(this Stream stream, int? version = null, ISerializerOptions? options = null, CancellationToken cancellationToken = default)
            => await ReadBoolAsync(stream, version, cancellationToken: cancellationToken).DynamicContext()
                ? await ReadObjectAsync<T>(stream, version, options, cancellationToken).DynamicContext()
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
        [TargetedPatchingOptOut("Tiny method")]
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
        [TargetedPatchingOptOut("Tiny method")]
        public static async Task<T?> ReadAnyObjectNullableAsync<T>(this Stream stream, int? version = null, CancellationToken cancellationToken = default)
            where T : class, new()
            => await ReadBoolAsync(stream, version, cancellationToken: cancellationToken).DynamicContext()
                ? await ReadAnyObjectAsync<T>(stream, version, cancellationToken).DynamicContext()
                : null;
    }
}
