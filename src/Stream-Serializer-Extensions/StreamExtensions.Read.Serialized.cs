using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Runtime;
using System.Runtime.CompilerServices;
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
        /// <param name="context">Context</param>
        /// <returns>Object</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static T ReadSerialized<T>(this Stream stream, IDeserializationContext context) where T : class, IStreamSerializer
#pragma warning restore IDE0060 // Remove unused argument
            => SerializerException.Wrap(() =>
            {
                using ContextRecursion cr = new(context);
                Type type = typeof(T);
                T res = StreamSerializer.CreateInstance<T>(out ConstructorInfo? ci, context);
                if (!(ci?.IsSerializerConstructor() ?? false)) res.Deserialize(context);
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
        /// <param name="context">Context</param>
        /// <returns>Object</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static IStreamSerializer ReadSerializedObject(this Stream stream, Type type, IDeserializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            => SerializerException.Wrap(() =>
            {
                using ContextRecursion cr = new(context);
                if (type.IsAbstract || type.IsInterface || type.IsGenericTypeDefinition)
                    throw new SerializerException($"Type {type} isn't a supported deserializer type");
                IStreamSerializer res = (IStreamSerializer)StreamSerializer.CreateInstance(out ConstructorInfo? ci, type, context);
                if (!(ci?.IsSerializerConstructor() ?? false)) res.Deserialize(context);
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
        /// <param name="context">Context</param>
        /// <returns>Object</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static Task<T> ReadSerializedAsync<T>(this Stream stream, IDeserializationContext context) where T : class, IStreamSerializer
#pragma warning restore IDE0060 // Remove unused argument
            => SerializerException.WrapAsync(async () =>
            {
                using ContextRecursion cr = new(context);
                Type type = typeof(T);
                if (type.IsAbstract || type.IsInterface || type.IsGenericTypeDefinition)
                    throw new SerializerException($"Type {type} isn't a supported deserializer type");
                T res = StreamSerializer.CreateInstance<T>(out ConstructorInfo? ci, context);
                if ((ci?.GetParametersCached().Length ?? 0) == 0) await res.DeserializeAsync(context).DynamicContext();
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
        /// <param name="context">Context</param>
        /// <returns>Object</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static Task<IStreamSerializer> ReadSerializedObjectAsync(this Stream stream, Type type, IDeserializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            => SerializerException.WrapAsync(async () =>
            {
                using ContextRecursion cr = new(context);
                if (type.IsAbstract || type.IsInterface || type.IsGenericTypeDefinition)
                    throw new SerializerException($"Type {type} isn't a supported deserializer type");
                IStreamSerializer res = (IStreamSerializer)StreamSerializer.CreateInstance(out ConstructorInfo? ci, type, context);
                if (!(ci?.IsSerializerConstructor() ?? false)) await res.DeserializeAsync(context).DynamicContext();
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
        /// <param name="context">Context</param>
        /// <returns>Object</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T? ReadSerializedNullable<T>(this Stream stream, IDeserializationContext context) where T : class, IStreamSerializer
#pragma warning disable IDE0034 // default expression can be simplified
            => ReadBool(stream, context) ? ReadSerialized<T>(stream, context) : default(T?);
#pragma warning restore IDE0034 // default expression can be simplified

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Object</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<T?> ReadSerializedNullableAsync<T>(this Stream stream, IDeserializationContext context) where T : class, IStreamSerializer
            => await ReadBoolAsync(stream, context).DynamicContext()
                ? await ReadSerializedAsync<T>(stream, context).DynamicContext()
#pragma warning disable IDE0034 // default expression can be simplified
                : default(T?);
#pragma warning restore IDE0034 // default expression can be simplified


        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Type</param>
        /// <param name="context">Context</param>
        /// <returns>Object</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object? ReadSerializedObjectNullable(this Stream stream, Type type, IDeserializationContext context)
            => ReadBool(stream, context) ? ReadSerializedObject(stream, type, context) : null;

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Type</param>
        /// <param name="context">Context</param>
        /// <returns>Object</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<object?> ReadSerializedObjectNullableAsync(this Stream stream, Type type, IDeserializationContext context)
            => await ReadBoolAsync(stream, context).DynamicContext()
                ? await ReadSerializedObjectAsync(stream, type, context).DynamicContext()
                : null;

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Structure type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Structure</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static T ReadSerializedStruct<T>(this Stream stream, IDeserializationContext context) where T : struct, IStreamSerializer
#pragma warning restore IDE0060 // Remove unused argument
            => SerializerException.Wrap(() =>
            {
                using ContextRecursion cr = new(context);
                Type type = typeof(T);
                if (type.IsAbstract || type.IsInterface || type.IsGenericTypeDefinition)
                    throw new SerializerException($"Type {type} isn't a supported deserializer type");
                T res = StreamSerializer.CreateInstance<T>(out ConstructorInfo? ci, context);
                if (!(ci?.IsSerializerConstructor() ?? false)) res.Deserialize(context);
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
        /// <param name="context">Context</param>
        /// <returns>Structure</returns>
#pragma warning disable IDE0060 // Remove unused argument
        public static Task<T> ReadSerializedStructAsync<T>(this Stream stream, IDeserializationContext context) where T : struct, IStreamSerializer
#pragma warning restore IDE0060 // Remove unused argument
            => SerializerException.WrapAsync(async () =>
            {
                using ContextRecursion cr = new(context);
                Type type = typeof(T);
                if (type.IsAbstract || type.IsInterface || type.IsGenericTypeDefinition)
                    throw new SerializerException($"Type {type} isn't a supported deserializer type");
                T res = StreamSerializer.CreateInstance<T>(out ConstructorInfo? ci, context);
                if (!(ci?.IsSerializerConstructor() ?? false)) await res.DeserializeAsync(context).DynamicContext();
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
        /// <param name="context">Context</param>
        /// <returns>Structure</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T? ReadSerializedStructNullable<T>(this Stream stream, IDeserializationContext context) where T : struct, IStreamSerializer
            => ReadBool(stream, context) ? ReadSerializedStruct<T>(stream, context) : default(T?);

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Structure type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Structure</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<T?> ReadSerializedStructNullableAsync<T>(this Stream stream, IDeserializationContext context)
            where T : struct, IStreamSerializer
            => await ReadBoolAsync(stream, context).DynamicContext()
                ? await ReadSerializedStructAsync<T>(stream, context).DynamicContext()
                : default(T?);
    }
}
