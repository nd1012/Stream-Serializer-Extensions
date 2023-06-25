using System.Buffers;
using System.Collections;
using System.Runtime;
using System.Runtime.CompilerServices;
using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    // Dictionary
    public static partial class StreamExtensions
    {
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
        /// <param name="keyOptions">Key serializer options</param>
        /// <param name="valueOptions">Value serializer options</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Dictionary<tKey, tValue> ReadDict<tKey, tValue>(
            this Stream stream,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            ISerializerOptions? keyOptions = null,
            ISerializerOptions? valueOptions = null
            )
            where tKey : notnull
        {
            pool ??= StreamSerializer.BufferPool;
            int len = ReadNumber<int>(stream, version, pool);
            SerializerHelper.EnsureValidLength(len, minLen, maxLen);
            Dictionary<tKey, tValue> res = new(len);
            for (int i = 0; i < len; i++) res[ReadObject<tKey>(stream, version, keyOptions)] = ReadObject<tValue>(stream, version, valueOptions);
            return res;
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Dictionary type</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <param name="keyOptions">Key serializer options</param>
        /// <param name="valueOptions">Value serializer options</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static IDictionary ReadDict(
            this Stream stream,
            Type type,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            ISerializerOptions? keyOptions = null,
            ISerializerOptions? valueOptions = null
            )
        {
            SerializerException.Wrap(() => ArgumentValidationHelper.EnsureValidArgument(
                nameof(type),
                type.IsGenericType || type.IsGenericTypeDefinition || !typeof(Dictionary<,>).IsAssignableFrom(type.GetGenericTypeDefinition()),
                () => "Not a dictionary type"
                ));
            pool ??= StreamSerializer.BufferPool;
            int len = ReadNumber<int>(stream, version, pool);
            SerializerHelper.EnsureValidLength(len, minLen, maxLen);
            Type[] types = type.GetGenericArguments();
            IDictionary res = (IDictionary)(Activator.CreateInstance(type, len) ?? throw new SerializerException($"Failed to instance {type}"));
            for (int i = 0; i < len; i++) res[ReadObject(stream, types[0], version, keyOptions)] = ReadObject(stream, types[1], version, valueOptions);
            return res;
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
        /// <param name="keyOptions">Key serializer options</param>
        /// <param name="valueOptions">Value serializer options</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<Dictionary<tKey, tValue>> ReadDictAsync<tKey, tValue>(
            this Stream stream,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            ISerializerOptions? keyOptions = null,
            ISerializerOptions? valueOptions = null,
            CancellationToken cancellationToken = default
            )
            where tKey : notnull
        {
            pool ??= StreamSerializer.BufferPool;
            int len = await ReadNumberAsync<int>(stream, version, pool, cancellationToken).DynamicContext();
            SerializerHelper.EnsureValidLength(len, minLen, maxLen);
            Dictionary<tKey, tValue> res = new(len);
            for (int i = 0; i < len; i++)
                res[await ReadObjectAsync<tKey>(stream, version, keyOptions, cancellationToken).DynamicContext()]
                    = await ReadObjectAsync<tValue>(stream, version, valueOptions, cancellationToken).DynamicContext();
            return res;
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Dictionary type</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <param name="keyOptions">Key serializer options</param>
        /// <param name="valueOptions">Value serializer options</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<IDictionary> ReadDictAsync(
            this Stream stream,
            Type type,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            ISerializerOptions? keyOptions = null,
            ISerializerOptions? valueOptions = null,
            CancellationToken cancellationToken = default
            )
        {
            SerializerException.Wrap(() => ArgumentValidationHelper.EnsureValidArgument(
                nameof(type),
                type.IsGenericType || type.IsGenericTypeDefinition || !typeof(Dictionary<,>).IsAssignableFrom(type.GetGenericTypeDefinition()),
                () => "Not a dictionary type"
                ));
            pool ??= StreamSerializer.BufferPool;
            int len = await ReadNumberAsync<int>(stream, version, pool, cancellationToken).DynamicContext();
            SerializerHelper.EnsureValidLength(len, minLen, maxLen);
            Type[] types = type.GetGenericArguments();
            IDictionary res = (IDictionary)(Activator.CreateInstance(type, len) ?? throw new SerializerException($"Failed to instance {type}"));
            for (int i = 0; i < len; i++)
                res[await ReadObjectAsync(stream, types[0], version, keyOptions, cancellationToken).DynamicContext()]
                    = await ReadObjectAsync(stream, types[1], version, valueOptions, cancellationToken).DynamicContext();
            return res;
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
        /// <param name="keyOptions">Key serializer options</param>
        /// <param name="valueOptions">Value serializer options</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Dictionary<tKey, tValue>? ReadDictNullable<tKey, tValue>(
            this Stream stream,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            ISerializerOptions? keyOptions = null,
            ISerializerOptions? valueOptions = null
            )
            where tKey : notnull
        {
            pool ??= StreamSerializer.BufferPool;
            switch ((version ??= StreamSerializer.Version) & byte.MaxValue)// Serializer version switch
            {
                case 1:
                case 2:
                    return ReadBool(stream, version, pool) ? ReadDict<tKey, tValue>(stream, version, pool, minLen, maxLen, keyOptions, valueOptions) : null;
                default:
                    {
                        if (ReadNumberNullable<int>(stream, version, pool) is not int len) return null;
                        SerializerHelper.EnsureValidLength(len, minLen, maxLen);
                        Dictionary<tKey, tValue> res = new(len);
                        for (int i = 0; i < len; i++) res[ReadObject<tKey>(stream, version, keyOptions)] = ReadObject<tValue>(stream, version, valueOptions);
                        return res;
                    }
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Dictionary type</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <param name="keyOptions">Key serializer options</param>
        /// <param name="valueOptions">Value serializer options</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static IDictionary? ReadDictNullable(
            this Stream stream,
            Type type,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            ISerializerOptions? keyOptions = null,
            ISerializerOptions? valueOptions = null
            )
        {
            SerializerException.Wrap(() => ArgumentValidationHelper.EnsureValidArgument(
                nameof(type),
                type.IsGenericType || type.IsGenericTypeDefinition || !typeof(Dictionary<,>).IsAssignableFrom(type.GetGenericTypeDefinition()),
                () => "Not a dictionary type"
                ));
            pool ??= StreamSerializer.BufferPool;
            switch ((version ??= StreamSerializer.Version) & byte.MaxValue)// Serializer version switch
            {
                case 1:
                case 2:
                    {
                        return ReadBool(stream, version, pool) ? ReadDict(stream, type, version, pool, minLen, maxLen, keyOptions, valueOptions) : null;
                    }
                default:
                    {
                        if (ReadNumberNullable<int>(stream, version, pool) is not int len) return null;
                        SerializerHelper.EnsureValidLength(len, minLen, maxLen);
                        Type[] types = type.GetGenericArguments();
                        IDictionary res = (IDictionary)(Activator.CreateInstance(type, len) ?? throw new SerializerException($"Failed to instance {type}"));
                        for (int i = 0; i < len; i++) res[ReadObject(stream, types[0], version, keyOptions)] = ReadObject(stream, types[1], version, valueOptions);
                        return res;
                    }
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
        /// <param name="keyOptions">Key serializer options</param>
        /// <param name="valueOptions">Value serializer options</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<Dictionary<tKey, tValue>?> ReadDictNullableAsync<tKey, tValue>(
            this Stream stream,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            ISerializerOptions? keyOptions = null,
            ISerializerOptions? valueOptions = null,
            CancellationToken cancellationToken = default
            )
            where tKey : notnull
        {
            pool ??= StreamSerializer.BufferPool;
            switch ((version ??= StreamSerializer.Version) & byte.MaxValue)// Serializer version switch
            {
                case 1:
                case 2:
                    {
                        return await ReadBoolAsync(stream, version, pool, cancellationToken).DynamicContext()
                            ? await ReadDictAsync<tKey, tValue>(stream, version, pool, minLen, maxLen, keyOptions, valueOptions, cancellationToken).DynamicContext()
                            : null;
                    }
                default:
                    {
                        if(await ReadNumberNullableAsync<int>(stream, version, pool, cancellationToken).DynamicContext() is not int len) return null;
                        SerializerHelper.EnsureValidLength(len, minLen, maxLen);
                        Dictionary<tKey, tValue> res = new(len);
                        for (int i = 0; i < len; i++)
                            res[await ReadObjectAsync<tKey>(stream, version, keyOptions, cancellationToken).DynamicContext()]
                                = await ReadObjectAsync<tValue>(stream, version, valueOptions, cancellationToken).DynamicContext();
                        return res;
                    }
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Dictionary type</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <param name="keyOptions">Key serializer options</param>
        /// <param name="valueOptions">Value serializer options</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<IDictionary?> ReadDictNullableAsync(
            this Stream stream,
            Type type,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            ISerializerOptions? keyOptions = null,
            ISerializerOptions? valueOptions = null,
            CancellationToken cancellationToken = default
            )
        {
            SerializerException.Wrap(() => ArgumentValidationHelper.EnsureValidArgument(
                nameof(type),
                type.IsGenericType || type.IsGenericTypeDefinition || !typeof(Dictionary<,>).IsAssignableFrom(type.GetGenericTypeDefinition()),
                () => "Not a dictionary type"
                ));
            pool ??= StreamSerializer.BufferPool;
            switch ((version ??= StreamSerializer.Version) & byte.MaxValue)// Serializer version switch
            {
                case 1:
                case 2:
                    {
                        return await ReadBoolAsync(stream, version, pool, cancellationToken).DynamicContext()
                            ? await ReadDictAsync(stream, type, version, pool, minLen, maxLen, keyOptions, valueOptions, cancellationToken).DynamicContext()
                            : null;
                    }
                default:
                    {
                        if (await ReadNumberNullableAsync<int>(stream, version, pool, cancellationToken).DynamicContext() is not int len) return null;
                        SerializerHelper.EnsureValidLength(len, minLen, maxLen);
                        Type[] types = type.GetGenericArguments();
                        IDictionary res = (IDictionary)(Activator.CreateInstance(type, len) ?? throw new SerializerException($"Failed to instance {type}"));
                        for (int i = 0; i < len; i++)
                            res[await ReadObjectAsync(stream, types[0], version, keyOptions, cancellationToken).DynamicContext()]
                                = await ReadObjectAsync(stream, types[1], version, valueOptions, cancellationToken).DynamicContext();
                        return res;
                    }
            }
        }
    }
}
