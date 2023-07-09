using System.Runtime;
using System.Runtime.CompilerServices;
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
        /// <param name="context">Context</param>
        /// <returns>Type</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type ReadType(this Stream stream, IDeserializationContext context) => ReadSerialized<SerializedTypeInfo>(stream, context).ToClrType();

        /// <summary>
        /// Read a type
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Type</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<Type> ReadTypeAsync(this Stream stream, IDeserializationContext context)
            => (await ReadSerializedAsync<SerializedTypeInfo>(stream, context).DynamicContext()).ToClrType();

        /// <summary>
        /// Read a serializable type (
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Type</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type ReadSerializableType(this Stream stream, IDeserializationContext context)
            => ReadSerialized<SerializedTypeInfo>(stream, context).ToSerializableType();

        /// <summary>
        /// Read a type
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Type</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<Type> ReadSerializableTypeAsync(this Stream stream, IDeserializationContext context)
            => (await ReadSerializedAsync<SerializedTypeInfo>(stream, context).DynamicContext()).ToSerializableType();

        /// <summary>
        /// Read a type
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="objVersion">Pre-red object version</param>
        /// <param name="objType">Pre-red object type</param>
        /// <param name="context">Context</param>
        /// <returns>Type</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#pragma warning disable IDE0060 // Remove unused argument
        public static Type ReadType(this Stream stream, int objVersion, ObjectTypes objType, IDeserializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            => SerializedTypeInfo.From(context, objVersion, objType).ToClrType();

        /// <summary>
        /// Read a type
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="objVersion">Pre-red object version</param>
        /// <param name="objType">Pre-red object type</param>
        /// <param name="context">Context</param>
        /// <returns>Type</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#pragma warning disable IDE0060 // Remove unused argument
        public static async Task<Type> ReadTypeAsync(this Stream stream, int objVersion, ObjectTypes objType, IDeserializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            => (await SerializedTypeInfo.FromAsync(context, objVersion, objType).DynamicContext()).ToClrType();

        /// <summary>
        /// Read a serializable type (
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="objVersion">Pre-red object version</param>
        /// <param name="objType">Pre-red object type</param>
        /// <param name="context">Context</param>
        /// <returns>Type</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#pragma warning disable IDE0060 // Remove unused argument
        public static Type ReadSerializableType(this Stream stream, int objVersion, ObjectTypes objType, IDeserializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            => SerializedTypeInfo.From(context, objVersion, objType).ToSerializableType();

        /// <summary>
        /// Read a type
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="objVersion">Pre-red object version</param>
        /// <param name="objType">Pre-red object type</param>
        /// <param name="context">Context</param>
        /// <returns>Type</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#pragma warning disable IDE0060 // Remove unused argument
        public static async Task<Type> ReadSerializableTypeAsync(this Stream stream, int objVersion, ObjectTypes objType, IDeserializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            => (await SerializedTypeInfo.FromAsync(context, objVersion, objType).DynamicContext()).ToSerializableType();

        /// <summary>
        /// Read a type
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Type</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type? ReadTypeNullable(this Stream stream, IDeserializationContext context)
        {
            int objVersion = ReadOneByte(stream, context);
            return objVersion != 0 ? SerializedTypeInfo.From(context, objVersion).ToClrType() : null;
        }

        /// <summary>
        /// Read a type
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Type</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<Type?> ReadTypeNullableAsync(this Stream stream, IDeserializationContext context)
        {
            int objVersion = await ReadOneByteAsync(stream, context).DynamicContext();
            return objVersion != 0 ? (await SerializedTypeInfo.FromAsync(context, objVersion).DynamicContext()).ToClrType() : null;
        }

        /// <summary>
        /// Read a serializable type (
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Type</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type? ReadSerializableTypeNullable(this Stream stream, IDeserializationContext context)
        {
            int objVersion = ReadOneByte(stream, context);
            return objVersion != 0 ? SerializedTypeInfo.From(context, objVersion).ToSerializableType() : null;
        }

        /// <summary>
        /// Read a type
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Type</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<Type?> ReadSerializableTypeNullableAsync(this Stream stream, IDeserializationContext context)
        {
            int objVersion = await ReadOneByteAsync(stream, context).DynamicContext();
            return objVersion != 0 ? (await SerializedTypeInfo.FromAsync(context, objVersion).DynamicContext()).ToSerializableType() : null;
        }
    }
}
