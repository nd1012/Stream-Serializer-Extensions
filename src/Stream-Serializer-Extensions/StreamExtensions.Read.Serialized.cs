using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Runtime;
using System.Runtime.CompilerServices;
using wan24.Core;
using wan24.ObjectValidation;

//TODO Check use of StreamSerializer.VERSION/Version

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
            => SerializerException.Wrap(() =>
            {
                Type type = typeof(T);
                T res = StreamSerializer.CreateInstance<T>(out ConstructorInfo? ci, stream, version);
                if (!(ci?.IsSerializerConstructor() ?? false)) res.Deserialize(stream, version ?? StreamSerializer.Version);
                if (!res.TryValidateObject(out List<ValidationResult> results))
                    throw new SerializerException(
                        $"The deserialized object contains {results.Count} errors: {results[0].ErrorMessage} ({string.Join(',', results[0].MemberNames)})",
                        new ObjectValidationException(results)
                        );
                return res;
            });

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Object type</param>
        /// <param name="version">Serializer version</param>
        /// <returns>Object</returns>
        public static IStreamSerializer ReadSerializedObject(this Stream stream, Type type, int? version = null)
            => SerializerException.Wrap(() =>
            {
                if (type.IsAbstract || type.IsInterface || type.IsGenericTypeDefinition)
                    throw new SerializerException($"Type {type} isn't a supported deserializer type");
                IStreamSerializer res = (IStreamSerializer)StreamSerializer.CreateInstance(out ConstructorInfo? ci, type, stream, version);
                if (!(ci?.IsSerializerConstructor() ?? false)) res.Deserialize(stream, version ?? StreamSerializer.Version);
                if (!res.TryValidateObject(out List<ValidationResult> results))
                    throw new SerializerException(
                        $"The deserialized object contains {results.Count} errors: {results[0].ErrorMessage} ({string.Join(',', results[0].MemberNames)})",
                        new ObjectValidationException(results)
                        );
                return res;
            });

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Object</returns>
        public static Task<T> ReadSerializedAsync<T>(this Stream stream, int? version = null, CancellationToken cancellationToken = default) where T : class, IStreamSerializer
            => SerializerException.WrapAsync(async () =>
            {
                Type type = typeof(T);
                if (type.IsAbstract || type.IsInterface || type.IsGenericTypeDefinition)
                    throw new SerializerException($"Type {type} isn't a supported deserializer type");
                T res = StreamSerializer.CreateInstance<T>(out ConstructorInfo? ci, stream, version);
                if ((ci?.GetParametersCached().Length ?? 0) == 0) await res.DeserializeAsync(stream, version ?? StreamSerializer.Version, cancellationToken).DynamicContext();
                if (!res.TryValidateObject(out List<ValidationResult> results))
                    throw new SerializerException(
                        $"The deserialized object contains {results.Count} errors: {results[0].ErrorMessage} ({string.Join(',', results[0].MemberNames)})",
                        new ObjectValidationException(results)
                        );
                return res;
            });

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Type</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Object</returns>
        public static Task<IStreamSerializer> ReadSerializedObjectAsync(this Stream stream, Type type, int? version = null, CancellationToken cancellationToken = default)
            => SerializerException.WrapAsync(async () =>
            {
                if (type.IsAbstract || type.IsInterface || type.IsGenericTypeDefinition)
                    throw new SerializerException($"Type {type} isn't a supported deserializer type");
                IStreamSerializer res = (IStreamSerializer)StreamSerializer.CreateInstance(out ConstructorInfo? ci, type, stream, version);
                if (!(ci?.IsSerializerConstructor() ?? false)) await res.DeserializeAsync(stream, version ?? StreamSerializer.Version, cancellationToken).DynamicContext();
                if (!res.TryValidateObject(out List<ValidationResult> results))
                    throw new SerializerException(
                        $"The deserialized object contains {results.Count} errors: {results[0].ErrorMessage} ({string.Join(',', results[0].MemberNames)})",
                        new ObjectValidationException(results)
                        );
                return res;
            });

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <returns>Object</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
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
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
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
            => SerializerException.Wrap(() =>
            {
                Type type = typeof(T);
                if (type.IsAbstract || type.IsInterface || type.IsGenericTypeDefinition)
                    throw new SerializerException($"Type {type} isn't a supported deserializer type");
                T res = StreamSerializer.CreateInstance<T>(out ConstructorInfo? ci, stream, version);
                if (!(ci?.IsSerializerConstructor() ?? false)) res.Deserialize(stream, version ?? StreamSerializer.Version);
                if (!res.TryValidateObject(out List<ValidationResult> results))
                    throw new SerializerException(
                        $"The deserialized object contains {results.Count} errors: {results[0].ErrorMessage} ({string.Join(',', results[0].MemberNames)})",
                        new ObjectValidationException(results)
                        );
                return res;
            });

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Structure type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Structure</returns>
        public static Task<T> ReadSerializedStructAsync<T>(this Stream stream, int? version = null, CancellationToken cancellationToken = default) where T : struct, IStreamSerializer
            => SerializerException.WrapAsync(async () =>
            {
                Type type = typeof(T);
                if (type.IsAbstract || type.IsInterface || type.IsGenericTypeDefinition)
                    throw new SerializerException($"Type {type} isn't a supported deserializer type");
                T res = StreamSerializer.CreateInstance<T>(out ConstructorInfo? ci, stream, version);
                if (!(ci?.IsSerializerConstructor() ?? false)) await res.DeserializeAsync(stream, version ?? StreamSerializer.Version, cancellationToken).DynamicContext();
                if (!res.TryValidateObject(out List<ValidationResult> results))
                    throw new SerializerException(
                        $"The deserialized object contains {results.Count} errors: {results[0].ErrorMessage} ({string.Join(',', results[0].MemberNames)})",
                        new ObjectValidationException(results)
                        );
                return res;
            });

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Structure type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <returns>Structure</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
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
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<T?> ReadSerializedStructNullableAsync<T>(this Stream stream, int? version = null, CancellationToken cancellationToken = default)
            where T : struct, IStreamSerializer
            => await ReadBoolAsync(stream, version, cancellationToken: cancellationToken).DynamicContext()
                ? await ReadSerializedStructAsync<T>(stream, version, cancellationToken).DynamicContext()
                : default(T?);
    }
}
