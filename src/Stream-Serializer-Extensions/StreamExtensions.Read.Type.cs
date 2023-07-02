using System.Runtime.CompilerServices;
using System.Runtime;
using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    // Type
    public static partial class StreamExtensions
    {
        /// <summary>
        /// Read a type
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <returns>Type</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type ReadType(this Stream stream, int? version = null) => ReadSerialized<SerializedTypeInfo>(stream, version).ToClrType();

        /// <summary>
        /// Read a type
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Type</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<Type> ReadTypeAsync(this Stream stream, int? version = null, CancellationToken cancellationToken = default)
            => (await ReadSerializedAsync<SerializedTypeInfo>(stream, version, cancellationToken).DynamicContext()).ToClrType();

        /// <summary>
        /// Read a serializable type (
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <returns>Type</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type ReadSerializableType(this Stream stream, int? version = null) => ReadSerialized<SerializedTypeInfo>(stream, version).ToSerializableType();

        /// <summary>
        /// Read a type
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Type</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<Type> ReadSerializableTypeAsync(this Stream stream, int? version = null, CancellationToken cancellationToken = default)
            => (await ReadSerializedAsync<SerializedTypeInfo>(stream, version, cancellationToken).DynamicContext()).ToSerializableType();

        /// <summary>
        /// Read a type
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="objVersion">Pre-red object version</param>
        /// <param name="objType">Pre-red object type</param>
        /// <param name="version">Serializer version</param>
        /// <returns>Type</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type ReadType(this Stream stream, int objVersion, ObjectTypes objType, int? version = null)
            => SerializedTypeInfo.From(stream, version ?? StreamSerializer.Version, objVersion, objType).ToClrType();

        /// <summary>
        /// Read a type
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="objVersion">Pre-red object version</param>
        /// <param name="objType">Pre-red object type</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Type</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<Type> ReadTypeAsync(this Stream stream, int objVersion, ObjectTypes objType, int? version = null, CancellationToken cancellationToken = default)
            => (await SerializedTypeInfo.FromAsync(stream, version ?? StreamSerializer.Version, objVersion, objType, cancellationToken).DynamicContext()).ToClrType();

        /// <summary>
        /// Read a serializable type (
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="objVersion">Pre-red object version</param>
        /// <param name="objType">Pre-red object type</param>
        /// <param name="version">Serializer version</param>
        /// <returns>Type</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type ReadSerializableType(this Stream stream, int objVersion, ObjectTypes objType, int? version = null)
            => SerializedTypeInfo.From(stream, version ?? StreamSerializer.Version, objVersion, objType).ToSerializableType();

        /// <summary>
        /// Read a type
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="objVersion">Pre-red object version</param>
        /// <param name="objType">Pre-red object type</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Type</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<Type> ReadSerializableTypeAsync(
            this Stream stream,
            int objVersion,
            ObjectTypes objType,
            int? version = null,
            CancellationToken cancellationToken = default
            )
            => (await SerializedTypeInfo.FromAsync(stream, version ?? StreamSerializer.Version, objVersion, objType, cancellationToken).DynamicContext()).ToSerializableType();

        /// <summary>
        /// Read a type
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <returns>Type</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type? ReadTypeNullable(this Stream stream, int? version = null)
            => ReadNumberNullable<int>(stream, version) is int objVersion ? SerializedTypeInfo.From(stream, version ?? StreamSerializer.Version, objVersion).ToClrType() : null;

        /// <summary>
        /// Read a type
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Type</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<Type?> ReadTypeNullableAsync(this Stream stream, int? version = null, CancellationToken cancellationToken = default)
            => await ReadNumberNullableAsync<int>(stream, version, cancellationToken: cancellationToken).DynamicContext() is int objVersion
                ? (await SerializedTypeInfo.FromAsync(stream, version ?? StreamSerializer.Version, objVersion, cancellationToken: cancellationToken).DynamicContext()).ToClrType()
                : null;

        /// <summary>
        /// Read a serializable type (
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <returns>Type</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type? ReadSerializableTypeNullable(this Stream stream, int? version = null)
            => ReadNumberNullable<int>(stream, version) is int objVersion 
                ? SerializedTypeInfo.From(stream, version ?? StreamSerializer.Version, objVersion).ToSerializableType() 
                : null;

        /// <summary>
        /// Read a type
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Type</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<Type?> ReadSerializableTypeNullableAsync(this Stream stream, int? version = null, CancellationToken cancellationToken = default)
            => await ReadNumberNullableAsync<int>(stream, version, cancellationToken: cancellationToken).DynamicContext() is int objVersion
                ? (await SerializedTypeInfo.FromAsync(stream, version ?? StreamSerializer.Version, objVersion, cancellationToken: cancellationToken).DynamicContext())
                    .ToSerializableType()
                : null;
    }
}
