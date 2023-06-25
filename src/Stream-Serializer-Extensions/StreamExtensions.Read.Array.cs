using System.Buffers;
using System.Runtime;
using System.Runtime.CompilerServices;
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
        /// <param name="valueOptions">Value deserializer options</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[] ReadArray<T>(
            this Stream stream,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            ISerializerOptions? valueOptions = null
            )
        {
            int len = ReadNumber<int>(stream, version, pool);
            SerializerHelper.EnsureValidLength(len, minLen, maxLen);
            if (len == 0) return Array.Empty<T>();
            T[] res = new T[len];
            for (int i = 0; i < len; res[i] = ReadObject<T>(stream, version, valueOptions), i++) ;
            return res;
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Array type</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <param name="valueOptions">Value deserializer options</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Array ReadArray(
            this Stream stream,
            Type type,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            ISerializerOptions? valueOptions = null
            )
        {
            SerializerException.Wrap(() => ArgumentValidationHelper.EnsureValidArgument(nameof(type), type.IsArray, "Not an array type"));
            int len = ReadNumber<int>(stream, version, pool);
            SerializerHelper.EnsureValidLength(len, minLen, maxLen);
            Type elementType = type.GetElementType()!;
            Array res = Array.CreateInstance(elementType, len);
            for (int i = 0; i < len; res.SetValue(ReadObject(stream, elementType, version, valueOptions), i), i++) ;
            return res;
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
        /// <param name="valueOptions">Value deserializer options</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<T[]> ReadArrayAsync<T>(
            this Stream stream,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            ISerializerOptions? valueOptions = null,
            CancellationToken cancellationToken = default
            )
        {
            int len = await ReadNumberAsync<int>(stream, version, pool, cancellationToken).DynamicContext();
            SerializerHelper.EnsureValidLength(len, minLen, maxLen);
            if (len == 0) return Array.Empty<T>();
            T[] res = new T[len];
            for (int i = 0; i < len; res[i] = await ReadObjectAsync<T>(stream, version, valueOptions, cancellationToken).DynamicContext(), i++) ;
            return res;
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Array type</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <param name="valueOptions">Value deserializer options</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<Array> ReadArrayAsync(
            this Stream stream,
            Type type,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            ISerializerOptions? valueOptions = null,
            CancellationToken cancellationToken = default
            )
        {
            SerializerException.Wrap(() => ArgumentValidationHelper.EnsureValidArgument(nameof(type), type.IsArray, "Not an array type"));
            int len = await ReadNumberAsync<int>(stream, version, pool, cancellationToken).DynamicContext();
            SerializerHelper.EnsureValidLength(len, minLen, maxLen);
            Type elementType = type.GetElementType()!;
            Array res = Array.CreateInstance(elementType, len);
            for (int i = 0; i < len; res.SetValue(await ReadObjectAsync(stream, elementType, version, valueOptions, cancellationToken).DynamicContext(), i), i++) ;
            return res;
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
        /// <param name="valueOptions">Value deserializer options</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[]? ReadArrayNullable<T>(
            this Stream stream,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            ISerializerOptions? valueOptions = null
            )
        {
            switch ((version ??= StreamSerializer.VERSION) & byte.MaxValue)// Serializer version switch
            {
                case 1:
                case 2:
                    {
                        return ReadBool(stream, version, pool) ? ReadArray<T>(stream, version, pool, minLen, maxLen, valueOptions) : null;
                    }
                default:
                    {
                        if (ReadNumberNullable<int>(stream, version, pool) is not int len) return null;
                        SerializerHelper.EnsureValidLength(len, minLen, maxLen);
                        if (len == 0) return Array.Empty<T>();
                        T[] res = new T[len];
                        ReadFixedArray(stream, res.AsSpan(), version, valueOptions);
                        return res;
                    }
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Array type</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <param name="valueOptions">Value deserializer options</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Array? ReadArrayNullable(
            this Stream stream,
            Type type,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            ISerializerOptions? valueOptions = null
            )
        {
            switch ((version ??= StreamSerializer.VERSION) & byte.MaxValue)// Serializer version switch
            {
                case 1:
                case 2:
                    {
                        return ReadBool(stream, version, pool) ? ReadArray(stream, type, version, pool, minLen, maxLen, valueOptions) : null;
                    }
                default:
                    {
                        if (ReadNumberNullable<int>(stream, version, pool) is not int len) return null;
                        return len < 1
                            ? Array.CreateInstance(type.GetElementType()!, length: 0)
                            : ReadFixedArray(
                                stream,
                                Array.CreateInstance(type.GetElementType()!, SerializerHelper.EnsureValidLength(len, minLen, maxLen)),
                                version,
                                valueOptions
                                );
                    }
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
        /// <param name="valueOptions">Value deserializer options</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<T[]?> ReadArrayNullableAsync<T>(
            this Stream stream,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            ISerializerOptions? valueOptions = null,
            CancellationToken cancellationToken = default
            )
        {
            switch ((version ??= StreamSerializer.VERSION) & byte.MaxValue)// Serializer version switch
            {
                case 1:
                case 2:
                    {
                        return await ReadBoolAsync(stream, version, pool, cancellationToken).DynamicContext()
                            ? await ReadArrayAsync<T>(stream, version, pool, minLen, maxLen, valueOptions, cancellationToken).DynamicContext()
                            : null;
                    }
                default:
                    {
                        if (await ReadNumberNullableAsync<int>(stream, version, pool, cancellationToken).DynamicContext() is not int len) return null;
                        SerializerHelper.EnsureValidLength(len, minLen, maxLen);
                        if (len == 0) return Array.Empty<T>();
                        T[] res = new T[len];
                        await ReadFixedArrayAsync(stream, res.AsMemory(), version, valueOptions, cancellationToken).DynamicContext();
                        return res;
                    }
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Array type</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <param name="valueOptions">Value deserializer options</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
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
        {
            switch ((version ??= StreamSerializer.VERSION) & byte.MaxValue)// Serializer version switch
            {
                case 1:
                case 2:
                    {
                        return await ReadBoolAsync(stream, version, pool, cancellationToken).DynamicContext()
                            ? await ReadArrayAsync(stream, type, version, pool, minLen, maxLen, valueOptions, cancellationToken).DynamicContext()
                            : null;
                    }
                default:
                    {
                        if (await ReadNumberNullableAsync<int>(stream, version, pool, cancellationToken).DynamicContext() is not int len) return null;
                        return len < 1
                            ? Array.CreateInstance(type.GetElementType()!, length: 0)
                            : await ReadFixedArrayAsync(
                                stream,
                                Array.CreateInstance(type.GetElementType()!, SerializerHelper.EnsureValidLength(len, minLen, maxLen)),
                                version,
                                valueOptions,
                                cancellationToken
                                ).DynamicContext();
                    }
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="arr">Array</param>
        /// <param name="version">Serializer version</param>
        /// <param name="valueOptions">Value deserializer options</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] ReadFixedArray<T>(this Stream stream, T[] arr, int? version = null, ISerializerOptions? valueOptions = null)
        {
            ReadFixedArray(stream, arr.AsSpan(), version, valueOptions);
            return arr;
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="arr">Array</param>
        /// <param name="version">Serializer version</param>
        /// <param name="valueOptions">Value deserializer options</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Span<T> ReadFixedArray<T>(this Stream stream, Span<T> arr, int? version = null, ISerializerOptions? valueOptions = null)
        {
            try
            {
                for (int i = 0, len = arr.Length; i < len; arr[i] = ReadObject<T>(stream, version, valueOptions), i++) ;
                return arr;
            }
            catch (SerializerException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw SerializerException.From(ex);
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="arr">Array</param>
        /// <param name="version">Serializer version</param>
        /// <param name="valueOptions">Value deserializer options</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Array ReadFixedArray(this Stream stream, Array arr, int? version = null, ISerializerOptions? valueOptions = null)
        {
            Type elementType = arr.GetType().GetElementType()!;
            for (int i = 0, len = arr.Length; i < len; arr.SetValue(ReadObject(stream, elementType, version, valueOptions), i), i++) ;
            return arr;
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="arr">Array</param>
        /// <param name="version">Serializer version</param>
        /// <param name="valueOptions">Value deserializer options</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<T[]> ReadFixedArrayAsync<T>(
            this Stream stream,
            T[] arr,
            int? version = null,
            ISerializerOptions? valueOptions = null,
            CancellationToken cancellationToken = default
            )
        {
            await ReadFixedArrayAsync(stream, arr.AsMemory(), version, valueOptions, cancellationToken).DynamicContext();
            return arr;
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="arr">Array</param>
        /// <param name="version">Serializer version</param>
        /// <param name="valueOptions">Value deserializer options</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<Memory<T>> ReadFixedArrayAsync<T>(
            this Stream stream,
            Memory<T> arr,
            int? version = null,
            ISerializerOptions? valueOptions = null,
            CancellationToken cancellationToken = default
            )
        {
            T item;
            for (int i = 0, len = arr.Length; i < len; i++)
            {
                item = await ReadObjectAsync<T>(stream, version, valueOptions, cancellationToken).DynamicContext();
                arr.Span[i] = item;
            }
            return arr;
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="arr">Array</param>
        /// <param name="version">Serializer version</param>
        /// <param name="valueOptions">Value deserializer options</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<Array> ReadFixedArrayAsync(
            this Stream stream,
            Array arr,
            int? version = null,
            ISerializerOptions? valueOptions = null,
            CancellationToken cancellationToken = default
            )
        {
            Type elementType = arr.GetType().GetElementType()!;
            for (
                int i = 0, len = arr.Length;
                i < len;
                arr.SetValue(await ReadObjectAsync(stream, elementType, version, valueOptions, cancellationToken).DynamicContext(), i), i++
                ) ;
            return arr;
        }
    }
}
