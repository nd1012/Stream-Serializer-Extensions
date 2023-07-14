using System.ComponentModel.DataAnnotations;
using System.Runtime;
using System.Runtime.CompilerServices;
using wan24.ObjectValidation;

//TODO Write/read enumerables

namespace wan24.StreamSerializerExtensions
{
    /// <summary>
    /// Helper
    /// </summary>
    public static partial class SerializerHelper
    {
        /// <summary>
        /// Ensure a non-null value
        /// </summary>
        /// <typeparam name="T">Value type</typeparam>
        /// <param name="value">Value</param>
        /// <param name="name">Argument value</param>
        /// <returns>Non-null value</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T EnsureNotNull<T>(T? value, string? name = null)
            => value ?? throw new SerializerException($"Argument {name ?? nameof(value)} is NULL", new ArgumentNullException(name ?? nameof(value)));

        /// <summary>
        /// Ensure a valid length
        /// </summary>
        /// <param name="len">Length</param>
        /// <param name="min">Minimum value</param>
        /// <param name="max">Maximum value</param>
        /// <returns>Length</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int EnsureValidLength(int len, int min = 0, int max = int.MaxValue)
        {
            if (len < min || len > max) throw new SerializerException($"Invalid length {len}", new InvalidDataException());
            return len;
        }

        /// <summary>
        /// Ensure a valid length
        /// </summary>
        /// <param name="len">Length</param>
        /// <param name="min">Minimum value</param>
        /// <param name="max">Maximum value</param>
        /// <returns>Length</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long EnsureValidLength(long len, long min = 0, long max = long.MaxValue)
        {
            if (len < min || len > max) throw new SerializerException($"Invalid length {len}", new InvalidDataException());
            return len;
        }

        /// <summary>
        /// Validate an object and throw an exception, if the validation failed
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="value">Value</param>
        /// <returns>Value</returns>
        /// <exception cref="SerializerException">If the validation failed</exception>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T ValidateDeserializedObject<T>(this T value) where T : notnull
        {
            if (!value.TryValidateObject(out List<ValidationResult> results))
                throw new SerializerException(
                    $"The deserialized object contains {results.Count} errors: {results[0].ErrorMessage} ({string.Join(',', results[0].MemberNames)})",
                    new ObjectValidationException(results)
                    );
            return value;
        }

        /// <summary>
        /// Get bytes from a serializable object
        /// </summary>
        /// <param name="obj">Object</param>
        /// <param name="includeSerializerVersion">Include the serializer version number?</param>
        /// <returns>Bytes</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[] ToBytes(this IStreamSerializer obj, bool includeSerializerVersion = true)
        {
            using MemoryStream ms = new();
            using SerializerContext<MemoryStream> context = new(ms);
            if (includeSerializerVersion) ms.WriteSerializerVersion(context);
            ms.WriteSerialized(obj, context);
            return ms.ToArray();
        }

        /// <summary>
        /// Deserialize an object
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="bytes">Bytes</param>
        /// <param name="includesSerializerVersion">Serializer version number included?</param>
        /// <returns>Object</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T ToObject<T>(this byte[] bytes, bool includesSerializerVersion = true) where T : class, IStreamSerializer, new()
        {
            using MemoryStream ms = new(bytes);
            using DeserializerContext<MemoryStream> context = new(ms);
            int version = includesSerializerVersion ? ms.ReadSerializerVersion(context) : StreamSerializer.Version;
            using DeserializerContext<MemoryStream> objContext = includesSerializerVersion && version != StreamSerializer.Version 
                ? context.WithSerializerVersion(version) 
                : context;
            return ms.ReadSerialized<T>(context);
        }
    }
}
