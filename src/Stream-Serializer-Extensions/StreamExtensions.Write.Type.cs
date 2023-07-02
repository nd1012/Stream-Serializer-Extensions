using System.Runtime;
using System.Runtime.CompilerServices;
using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    // Type
    public static partial class StreamExtensions
    {
        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Type</param>
        /// <returns>Stream</returns>
        public static Stream Write(this Stream stream, Type type) => WriteSerialized(stream, new SerializedTypeInfo(type));

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Type</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        public static Task<Stream> WriteAsync(this Stream stream, Type type, CancellationToken cancellationToken = default)
            => WriteSerializedAsync(stream, new SerializedTypeInfo(type), cancellationToken);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Type</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteAsync(this Task<Stream> stream, Type type, CancellationToken cancellationToken = default)
            => AsyncHelper.FluentAsync(stream, type, cancellationToken, WriteAsync);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Type</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Stream WriteNullable(this Stream stream, Type? type)
            => type == null ? WriteNumberNullable(stream, (int?)null) : Write(stream, type);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Type</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteNullableAsync(this Stream stream, Type? type, CancellationToken cancellationToken = default)
            => type == null
                ? WriteNumberNullableAsync(stream, (int?)null, cancellationToken)
                : WriteAsync(stream, type, cancellationToken);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Type</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteNullableAsync(this Task<Stream> stream, Type? type, CancellationToken cancellationToken = default)
            => AsyncHelper.FluentAsync(stream, type, cancellationToken, WriteNullableAsync);
    }
}
