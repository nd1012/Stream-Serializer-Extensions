﻿using System.Buffers;
using System.Runtime;
using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    // Enumeration
    public static partial class StreamExtensions
    {
        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Enumeration type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        public static T ReadEnum<T>(this Stream stream, int? version = null, ArrayPool<byte>? pool = null) where T : struct, Enum
            => (T)ReadEnumInt(stream, typeof(T), version, numberType: null, pool);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Enumeration type</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        public static Enum ReadEnum(this Stream stream, Type type, int? version = null, ArrayPool<byte>? pool = null)
            => ReadEnumInt(stream, type, version, numberType: null, pool);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Enumeration type</param>
        /// <param name="version">Serializer version</param>
        /// <param name="numberType">Number type</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        private static Enum ReadEnumInt(Stream stream, Type type, int? version, NumberTypes? numberType, ArrayPool<byte>? pool)
            => SerializerException.Wrap(() =>
            {
                IEnumInfo info = type.GetEnumInfo();
                if ((numberType ?? NumberTypes.None) == NumberTypes.Default)
                    return (Enum)(Activator.CreateInstance(type) ?? throw new SerializerException($"Failed to instance {type}"));
                numberType ??= (NumberTypes)ReadOneByte(stream, version);
                if ((version ?? StreamSerializer.VERSION) > 1 && numberType == NumberTypes.Default)
                    return (Enum)(Activator.CreateInstance(type) ?? throw new SerializerException($"Failed to instance {type}"));
                Enum res = (Enum)Enum.ToObject(
                    type,
                    ReadNumber(stream, type.GetEnumUnderlyingType()!, version, pool)
                    );
                if (!info.IsValidEnumerationValue()) throw new SerializerException($"Unknown enumeration value {res} for {type}");
                return res;
            });

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Enumeration type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static async Task<T> ReadEnumAsync<T>(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
            where T : struct, Enum
            => (T)await ReadEnumIntAsync(stream, typeof(T), version, numberType: null, pool, cancellationToken).DynamicContext();

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Enumeration type</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        public static Task<Enum> ReadEnumAsync(this Stream stream, Type type, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
            => ReadEnumIntAsync(stream, type, version, numberType: null, pool, cancellationToken);

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Enumeration type</param>
        /// <param name="version">Serializer version</param>
        /// <param name="numberType">Number type</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        private static Task<Enum> ReadEnumIntAsync(Stream stream, Type type, int? version, NumberTypes? numberType, ArrayPool<byte>? pool, CancellationToken cancellationToken)
            => SerializerException.WrapAsync(async () =>
            {
                if ((numberType ?? NumberTypes.None) == NumberTypes.Default)
                    return (Enum)(Activator.CreateInstance(type) ?? throw new SerializerException($"Failed to instance {type}"));
                numberType ??= (NumberTypes)await ReadOneByteAsync(stream, version, cancellationToken).DynamicContext();
                if ((version ?? StreamSerializer.VERSION) > 1 && numberType == NumberTypes.Default)
                    return (Enum)(Activator.CreateInstance(type) ?? throw new SerializerException($"Failed to instance {type}"));
                Enum res = (Enum)Enum.ToObject(type, await ReadNumberIntAsync(stream, type.GetEnumUnderlyingType(), version, numberType, pool, cancellationToken).DynamicContext());
                if (!res.IsValidEnumerationValue()) throw new SerializerException($"Unknown enumeration value {res} for {type}");
                return res;
            });

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Enumeration type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static T? ReadEnumNullable<T>(this Stream stream, int? version = null, ArrayPool<byte>? pool = null) where T : struct, Enum
            => SerializerException.Wrap(() =>
            {
                switch ((version ?? StreamSerializer.VERSION) & byte.MaxValue)
                {
                    case 1:
                        return ReadBool(stream, version, pool) ? ReadEnum<T>(stream, version, pool) : null;
                    default:
                        NumberTypes numberType = (NumberTypes)ReadOneByte(stream, version);
                        return numberType == NumberTypes.Null ? null : (T?)ReadEnumInt(stream, typeof(T), version, numberType, pool);
                }
            });

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Enumeration type</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static Enum? ReadEnumNullable(this Stream stream, Type type, int? version = null, ArrayPool<byte>? pool = null)
            => SerializerException.Wrap(() =>
            {
                switch ((version ?? StreamSerializer.VERSION) & byte.MaxValue)
                {
                    case 1:
                        return ReadBool(stream, version, pool)
                            ? ReadEnum(stream, type, version, pool)
                            : (Enum)(Activator.CreateInstance(type) ?? throw new SerializerException($"Failed to instance {type}"));
                    default:
                        NumberTypes numberType = (NumberTypes)ReadOneByte(stream, version);
                        return numberType == NumberTypes.Null
                            ? (Enum)(Activator.CreateInstance(type) ?? throw new SerializerException($"Failed to instance {type}"))
                            : ReadEnumInt(stream, type, version, numberType, pool);
                }
            });

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Enumeration type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static Task<T?> ReadEnumNullableAsync<T>(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
            where T : struct, Enum
            => SerializerException.WrapAsync(async () =>
            {
                switch ((version ?? StreamSerializer.VERSION) & byte.MaxValue)
                {
                    case 1:
                        return await ReadBoolAsync(stream, version, pool, cancellationToken).DynamicContext()
                            ? await ReadEnumAsync<T>(stream, version, pool, cancellationToken).DynamicContext()
                            : default(T?);
                    default:
                        NumberTypes numberType = (NumberTypes)await ReadOneByteAsync(stream, version, cancellationToken).DynamicContext();
                        return numberType == NumberTypes.Null
                            ? default(T?)
                            : (T)await ReadEnumIntAsync(stream, typeof(T), version, numberType, pool, cancellationToken).DynamicContext();
                }
            });

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Enumeration type</param>
        /// <param name="version">Serializer version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static Task<Enum?> ReadEnumNullableAsync(
            this Stream stream, 
            Type type,
            int? version = null, 
            ArrayPool<byte>? pool = null, 
            CancellationToken cancellationToken = default
            )
            => SerializerException.WrapAsync(async () =>
            {
                switch ((version ?? StreamSerializer.VERSION) & byte.MaxValue)
                {
                    case 1:
                        return await ReadBoolAsync(stream, version, pool, cancellationToken).DynamicContext()
                            ? await ReadEnumAsync(stream, type, version, pool, cancellationToken).DynamicContext()
                            : (Enum?)(Activator.CreateInstance(type) ?? throw new SerializerException($"Failed to instance {type}"));
                    default:
                        NumberTypes numberType = (NumberTypes)await ReadOneByteAsync(stream, version, cancellationToken).DynamicContext();
                        return numberType == NumberTypes.Null
                            ? (Enum?)(Activator.CreateInstance(type) ?? throw new SerializerException($"Failed to instance {type}"))
                            : await ReadEnumIntAsync(stream, type, version, numberType, pool, cancellationToken).DynamicContext();
                }
            });
    }
}
