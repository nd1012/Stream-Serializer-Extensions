using System.Buffers;
using System.Runtime;
using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    // Array
    public static partial class StreamExtensions
    {
        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <param name="valueOptions">Value serializer options</param>
        /// <returns>Value</returns>
        public static T[] ReadArray<T>(
            this Stream stream,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            ISerializerOptions? valueOptions = null
            )
            => SerializerException.Wrap(() =>
            {
                if (typeof(T) == typeof(byte)) return (ReadBytes(stream, version, buffer: null, pool, minLen, maxLen).Value as T[])!;
                int len = ReadNumber<int>(stream, version, pool);
                SerializerHelper.EnsureValidLength(len, minLen, maxLen);
                T[] res = new T[len];
                for (int i = 0; i < len; res[i] = ReadObject<T>(stream, version, valueOptions), i++) ;
                return res;
            });

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Array type</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <param name="valueOptions">Value serializer options</param>
        /// <returns>Value</returns>
        public static Array ReadArray(
            this Stream stream,
            Type type,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            ISerializerOptions? valueOptions = null
            )
            => SerializerException.Wrap(() =>
            {
                if (type.GetElementType() == typeof(byte)) return ReadBytes(stream, version, buffer: null, pool, minLen, maxLen).Value;
                ArgumentValidationHelper.EnsureValidArgument(nameof(type), type.IsArray, "Not an array type");
                int len = ReadNumber<int>(stream, version, pool);
                SerializerHelper.EnsureValidLength(len, minLen, maxLen);
                Type elementType = type.GetElementType()!;
                Array res = Array.CreateInstance(elementType, len);
                for (int i = 0; i < len; res.SetValue(ReadObject(stream, elementType, version, valueOptions), i), i++) ;
                return res;
            });

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <param name="valueOptions">Value serializer options</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static Task<T[]> ReadArrayAsync<T>(
            this Stream stream,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            ISerializerOptions? valueOptions = null,
            CancellationToken cancellationToken = default
            )
            => SerializerException.WrapAsync(async () =>
            {
                if (typeof(T) == typeof(byte))
                    return (await ReadBytesAsync(stream, version, buffer: null, pool, minLen, maxLen, cancellationToken).DynamicContext() as T[])!;
                int len = await ReadNumberAsync<int>(stream, version, pool, cancellationToken).DynamicContext();
                SerializerHelper.EnsureValidLength(len, minLen, maxLen);
                T[] res = new T[len];
                for (int i = 0; i < len; res[i] = await ReadObjectAsync<T>(stream, version, valueOptions, cancellationToken).DynamicContext(), i++) ;
                return res;
            });

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Array type</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <param name="valueOptions">Value serializer options</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static Task<Array> ReadArrayAsync(
            this Stream stream,
            Type type,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            ISerializerOptions? valueOptions = null,
            CancellationToken cancellationToken = default
            )
            => SerializerException.WrapAsync(async () =>
            {
                if (type.GetElementType() == typeof(byte))
                    return (await ReadBytesAsync(stream, version, buffer: null, pool, minLen, maxLen, cancellationToken).DynamicContext()).Value;
                ArgumentValidationHelper.EnsureValidArgument(nameof(type), type.IsArray, "Not an array type");
                int len = await ReadNumberAsync<int>(stream, version, pool, cancellationToken).DynamicContext();
                SerializerHelper.EnsureValidLength(len, minLen, maxLen);
                Type elementType = type.GetElementType()!;
                Array res = Array.CreateInstance(elementType, len);
                for (int i = 0; i < len; res.SetValue(await ReadObjectAsync(stream, elementType, version, valueOptions, cancellationToken).DynamicContext(), i), i++) ;
                return res;
            });

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <param name="valueOptions">Value serializer options</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static T[]? ReadArrayNullable<T>(
            this Stream stream,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            ISerializerOptions? valueOptions = null
            )
            => ReadBool(stream, version, pool) ? ReadArray<T>(stream, version, pool, minLen, maxLen, valueOptions) : null;

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Array type</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <param name="valueOptions">Value serializer options</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static Array? ReadArrayNullable(
            this Stream stream,
            Type type,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            ISerializerOptions? valueOptions = null
            )
            => ReadBool(stream, version, pool) ? ReadArray(stream, type, version, pool, minLen, maxLen, valueOptions) : null;

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <param name="valueOptions">Value serializer options</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static async Task<T[]?> ReadArrayNullableAsync<T>(
            this Stream stream,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            ISerializerOptions? valueOptions = null,
            CancellationToken cancellationToken = default
            )
            => await ReadBoolAsync(stream, version, pool, cancellationToken).DynamicContext()
                ? await ReadArrayAsync<T>(stream, version, pool, minLen, maxLen, valueOptions, cancellationToken).DynamicContext()
                : null;

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Array type</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <param name="valueOptions">Value serializer options</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static async Task<Array?> ReadArrayNullableAsync(
            this Stream stream,
            Type type,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            ISerializerOptions? valueOptions = null,
            CancellationToken cancellationToken = default
            )
            => await ReadBoolAsync(stream, version, pool, cancellationToken).DynamicContext()
                ? await ReadArrayAsync(stream, type, version, pool, minLen, maxLen, valueOptions, cancellationToken).DynamicContext()
                : null;

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="arr">Array</param>
        /// <param name="version">Serializer version</param>
        /// <param name="valueOptions">Value serializer options</param>
        /// <returns>Value</returns>
        public static T[] ReadFixedArray<T>(this Stream stream, T[] arr, int? version = null, ISerializerOptions? valueOptions = null)
            => SerializerException.Wrap(() =>
            {
                for (int i = 0; i < arr.Length; arr[i] = ReadObject<T>(stream, version, valueOptions), i++) ;
                return arr;
            });

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="arr">Array</param>
        /// <param name="version">Serializer version</param>
        /// <param name="valueOptions">Value serializer options</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static Task<T[]> ReadFixedArrayAsync<T>(
            this Stream stream,
            T[] arr,
            int? version = null,
            ISerializerOptions? valueOptions = null,
            CancellationToken cancellationToken = default
            )
            => SerializerException.WrapAsync(async () =>
            {
                for (int i = 0; i < arr.Length; arr[i] = await ReadObjectAsync<T>(stream, version, valueOptions, cancellationToken).DynamicContext(), i++) ;
                return arr;
            });
    }
}
