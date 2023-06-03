using System.ComponentModel.DataAnnotations;
using System.Reflection;
using wan24.Core;
using wan24.ObjectValidation;

namespace wan24.StreamSerializerExtensions
{
    // Serialized
    public static partial class StreamExtensions
    {
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
        /// Read
        /// </summary>
        /// <typeparam name="T">Structure type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <returns>Structure</returns>
        public static T ReadSerializedStruct<T>(this Stream stream, int? version = null) where T : struct, IStreamSerializer
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
        /// <typeparam name="T">Structure type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Structure</returns>
        public static async Task<T> ReadSerializedStructAsync<T>(this Stream stream, int? version = null, CancellationToken cancellationToken = default) where T : struct, IStreamSerializer
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
        /// <typeparam name="T">Structure type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <returns>Structure</returns>
        public static T? ReadSerializedStructNullable<T>(this Stream stream, int? version = null) where T : struct, IStreamSerializer
            => ReadBool(stream, version) ? ReadSerializedStruct<T>(stream, version) : default(T?);

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Structure type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Structure</returns>
        public static async Task<T?> ReadSerializedStructNullableAsync<T>(this Stream stream, int? version = null, CancellationToken cancellationToken = default)
            where T : struct, IStreamSerializer
            => await ReadBoolAsync(stream, version, cancellationToken: cancellationToken).DynamicContext()
                ? await ReadSerializedStructAsync<T>(stream, version, cancellationToken).DynamicContext()
                : default(T?);
    }
}
