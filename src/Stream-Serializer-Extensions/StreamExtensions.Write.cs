using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime;
using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    /// <summary>
    /// Stream extensions
    /// </summary>
    public static partial class StreamExtensions
    {
        /// <summary>
        /// Constructor
        /// </summary>
        static StreamExtensions()
        {
            ArrayEmptyMethod = typeof(Array).GetMethodCached(nameof(Array.Empty), BindingFlags.Static | BindingFlags.Public)!;
            Type type = typeof(StreamExtensions);
            ReadStructMethod = type.GetMethodCached(nameof(ReadStruct), BindingFlags.Static | BindingFlags.Public)
                ?? throw new TypeLoadException($"Failed to reflect {nameof(ReadStruct)}");
            ReadStructAsyncMethod = type.GetMethodCached(nameof(ReadStructAsync), BindingFlags.Static | BindingFlags.Public)
                ?? throw new TypeLoadException($"Failed to reflect {nameof(ReadStructAsync)}");
        }

        /// <summary>
        /// Write the serializer version
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static Stream WriteSerializerVersion(this Stream stream) => WriteNumber(stream, StreamSerializer.VERSION);

        /// <summary>
        /// Write the serializer version
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="cancellationToken">Cancellation token</param>
        [TargetedPatchingOptOut("Tiny method")]
        public static Task WriteSerializerVersionAsync(this Stream stream, CancellationToken cancellationToken = default)
            => WriteNumberAsync(stream, StreamSerializer.VERSION, cancellationToken);

        /// <summary>
        /// Write a boolean flag if an object is not <see langword="null"/>
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <returns>Is not <see langword="null"/>?</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static bool WriteIfNull<T>(this Stream stream, [NotNullWhen(true)] T? obj)
        {
            bool isNotNull = obj != null;
            Write(stream, isNotNull);
            return isNotNull;
        }

        /// <summary>
        /// Write a boolean flag if an object is not <see langword="null"/>
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Is not <see langword="null"/>?</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static async Task<bool> WriteIfNullAsync<T>(this Stream stream, [NotNullWhen(true)] T? obj, CancellationToken cancellationToken = default)
        {
            //TODO NotNullWhen seems not to work with a task result
            bool isNotNull = obj != null;
            await WriteAsync(stream, isNotNull, cancellationToken).DynamicContext();
            return isNotNull;
        }

        /// <summary>
        /// Write a boolean flag if an object is not <see langword="null"/>
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="action">Write action to execute, if the object isn't <see langword="null"/></param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static Stream WriteIfNull<T>(this Stream stream, T? obj, Action action)
        {
            if (WriteIfNull(stream, obj)) action();
            return stream;
        }

        /// <summary>
        /// Write a boolean flag if an object is not <see langword="null"/>
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="action">Write action to execute, if the object isn't <see langword="null"/></param>
        /// <param name="cancellationToken">Cancellation token</param>
        [TargetedPatchingOptOut("Tiny method")]
        public static async Task WriteIfNullAsync<T>(this Stream stream, T? obj, Func<Task> action, CancellationToken cancellationToken = default)
        {
            if (await WriteIfNullAsync(stream, obj, cancellationToken).DynamicContext())
                await action().DynamicContext();
        }

        /// <summary>
        /// Write serialized Data
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="data">Serialized data</param>
        /// <param name="len">Data length in bytes</param>
        /// <param name="pool">Array pool (<c>data</c> will returned to that pool)</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static Stream WriteSerializedData(this Stream stream, byte[] data, int len, ArrayPool<byte>? pool = null)
            => SerializerException.Wrap(() =>
            {
                try
                {
                    stream.Write(data.AsSpan(0, len));
                }
                finally
                {
                    (pool ?? StreamSerializer.BufferPool).Return(data, clearArray: false);
                }
                return stream;
            });

        /// <summary>
        /// Write serialized Data
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="data">Serialized data</param>
        /// <param name="len">Data length in bytes</param>
        /// <param name="pool">Array pool (<c>data</c> will returned to that pool)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        [TargetedPatchingOptOut("Tiny method")]
        public static async Task WriteSerializedDataAsync(this Stream stream, byte[] data, int len, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
            => await SerializerException.WrapAsync(async () =>
            {
                try
                {
                    await stream.WriteAsync(data.AsMemory(0, len), cancellationToken).DynamicContext();
                }
                finally
                {
                    (pool ?? StreamSerializer.BufferPool).Return(data, clearArray: false);
                }
            });
    }
}
