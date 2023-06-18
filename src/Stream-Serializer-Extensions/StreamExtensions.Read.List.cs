﻿using System.Buffers;
using System.Collections;
using System.Runtime;
using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    // List
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
        public static List<T> ReadList<T>(
            this Stream stream,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            ISerializerOptions? valueOptions = null
            )
            => SerializerException.Wrap(() =>
            {
                if (typeof(T) == typeof(byte)) return new List<T>((ReadBytes(stream, version, buffer: null, pool, minLen, maxLen).Value as T[])!);
                int len = ReadNumber<int>(stream, version, pool);
                SerializerHelper.EnsureValidLength(len, minLen, maxLen);
                List<T> res = new(len);
                for (int i = 0; i < len; res.Add(ReadObject<T>(stream, version, valueOptions)), i++) ;
                return res;
            });

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">List type</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <param name="valueOptions">Value serializer options</param>
        /// <returns>Value</returns>
        public static IList ReadList(
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
                if (type.GetElementType() == typeof(byte)) return new List<byte>(ReadBytes(stream, version, buffer: null, pool, minLen, maxLen).Value);
                ArgumentValidationHelper.EnsureValidArgument(
                    nameof(type),
                    type.IsGenericType || type.IsGenericTypeDefinition || !typeof(List<>).IsAssignableFrom(type.GetGenericTypeDefinition()),
                    "Not a list type"
                    );
                int len = ReadNumber<int>(stream, version, pool);
                SerializerHelper.EnsureValidLength(len, minLen, maxLen);
                Type itemType = type.GetGenericArgumentsCached()[0];
                IList res = (IList)(Activator.CreateInstance(type, len) ?? throw new SerializerException($"Failed to instance {type}"));
                for (int i = 0; i < len; res.Add(ReadObject(stream, itemType, version, valueOptions)), i++) ;
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
        public static Task<List<T>> ReadListAsync<T>(
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
                    return new List<T>((await ReadBytesAsync(stream, version, buffer: null, pool, minLen, maxLen, cancellationToken).DynamicContext() as T[])!);
                int len = await ReadNumberAsync<int>(stream, version, pool, cancellationToken).DynamicContext();
                SerializerHelper.EnsureValidLength(len, minLen, maxLen);
                List<T> res = new(len);
                for (int i = 0; i < len; res.Add(await ReadObjectAsync<T>(stream, version, valueOptions, cancellationToken).DynamicContext()), i++) ;
                return res;
            });

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">List type</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <param name="valueOptions">Value serializer options</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static Task<IList> ReadListAsync(
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
                    return new List<byte>((await ReadBytesAsync(stream, version, buffer: null, pool, minLen, maxLen, cancellationToken).DynamicContext()).Value);
                ArgumentValidationHelper.EnsureValidArgument(
                    nameof(type),
                    type.IsGenericType || type.IsGenericTypeDefinition || !typeof(List<>).IsAssignableFrom(type.GetGenericTypeDefinition()),
                    "Not a list type"
                    );
                int len = await ReadNumberAsync<int>(stream, version, pool, cancellationToken).DynamicContext();
                SerializerHelper.EnsureValidLength(len, minLen, maxLen);
                Type itemType = type.GetGenericArgumentsCached()[0];
                IList res = (IList)(Activator.CreateInstance(type, len) ?? throw new SerializerException($"Failed to instance {type}"));
                for (int i = 0; i < len; res.Add(await ReadObjectAsync(stream, itemType, version, valueOptions, cancellationToken).DynamicContext()), i++) ;
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
        public static List<T>? ReadListNullable<T>(
            this Stream stream,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            ISerializerOptions? valueOptions = null
            )
            => ReadBool(stream, version, pool) ? ReadList<T>(stream, version, pool, minLen, maxLen, valueOptions) : null;

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">List type</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <param name="valueOptions">Value serializer options</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static IList? ReadListNullable(
            this Stream stream,
            Type type,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            ISerializerOptions? valueOptions = null
            )
            => ReadBool(stream, version, pool) ? ReadList(stream, type, version, pool, minLen, maxLen, valueOptions) : null;

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
        public static async Task<List<T>?> ReadListNullableAsync<T>(
            this Stream stream,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            ISerializerOptions? valueOptions = null,
            CancellationToken cancellationToken = default
            )
            => await ReadBoolAsync(stream, version, pool, cancellationToken).DynamicContext()
                ? await ReadListAsync<T>(stream, version, pool, minLen, maxLen, valueOptions, cancellationToken).DynamicContext()
                : null;

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">List type</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <param name="valueOptions">Value serializer options</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static async Task<IList?> ReadListNullableAsync(
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
                ? await ReadListAsync(stream, type, version, pool, minLen, maxLen, valueOptions, cancellationToken).DynamicContext()
                : null;
    }
}
