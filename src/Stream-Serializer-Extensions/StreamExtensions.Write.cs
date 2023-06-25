using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime;
using System.Runtime.CompilerServices;
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
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream WriteSerializerVersion(this Stream stream)
        {
            (object n, NumberTypes nt) = StreamSerializer.Version.GetNumberAndType();
            Write(stream, (byte)nt);
            switch (n)
            {
                case sbyte sb: Write(stream, sb); break;
                case byte b: Write(stream, b); break;
                case short s: Write(stream, s); break;
                case ushort us: Write(stream, us); break;
                case int i: Write(stream, i); break;
                default: throw new SerializerException($"Invalid numeric type {nt} for serializer version {StreamSerializer.Version}");
            }
            return stream;
        }

        /// <summary>
        /// Write the serializer version
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<Stream> WriteSerializerVersionAsync(this Stream stream, CancellationToken cancellationToken = default)
        {
            (object n, NumberTypes nt) = StreamSerializer.Version.GetNumberAndType();
            await WriteAsync(stream, (byte)nt, cancellationToken).DynamicContext();
            switch (n)
            {
                case sbyte sb: await WriteAsync(stream, sb, cancellationToken).DynamicContext(); break;
                case byte b: await WriteAsync(stream, b, cancellationToken).DynamicContext(); break;
                case short s: await WriteAsync(stream, s, cancellationToken).DynamicContext(); break;
                case ushort us: await WriteAsync(stream, us, cancellationToken).DynamicContext(); break;
                case int i: await WriteAsync(stream, i, cancellationToken).DynamicContext(); break;
                default: throw new SerializerException($"Invalid numeric type {nt} for serializer version {StreamSerializer.Version}");
            }
            return stream;
        }

        /// <summary>
        /// Write the serializer version
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteSerializerVersionAsync(this Task<Stream> stream, CancellationToken cancellationToken = default)
            => AsyncHelper.FluentAsync(stream, cancellationToken, WriteSerializerVersionAsync);

        /// <summary>
        /// Write a boolean flag if an object is not <see langword="null"/>
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <returns>Is not <see langword="null"/>?</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool WriteIfNotNull<T>(this Stream stream, [NotNullWhen(true)] T? obj)
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
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<bool> WriteIfNotNullAsync<T>(this Stream stream, [NotNullWhen(true)] T? obj, CancellationToken cancellationToken = default)
        {
            //TODO NotNullWhen seems not to work with a task result https://github.com/dotnet/roslyn/issues/45228
            bool isNotNull = obj != null;
            await WriteAsync(stream, isNotNull, cancellationToken).DynamicContext();
            return isNotNull;
        }

        /// <summary>
        /// Write a nullable count
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="count">Count</param>
        /// <returns>Is not <see langword="null"/>?</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool WriteNullableCount(this Stream stream, [NotNullWhen(true)] long? count)
        {
            WriteNumberNullable(stream, count);
            return count != null;
        }

        /// <summary>
        /// Write a nullable count
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="count">Count</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Is not <see langword="null"/>?</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<bool> WriteNullableCountAsync(this Stream stream, [NotNullWhen(true)] long? count, CancellationToken cancellationToken = default)
        {
            //TODO NotNullWhen seems not to work with a task result https://github.com/dotnet/roslyn/issues/45228
            await WriteNumberNullableAsync(stream, count, cancellationToken).DynamicContext();
            return count != null;
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
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream WriteIfNotNull<T>(this Stream stream, T? obj, Action action)
        {
            if (WriteIfNotNull(stream, obj)) action();
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
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<Stream> WriteIfNotNullAsync<T>(this Stream stream, T? obj, Func<Task> action, CancellationToken cancellationToken = default)
        {
            if (await WriteIfNotNullAsync(stream, obj, cancellationToken).DynamicContext())
                await action().DynamicContext();
            return stream;
        }

        /// <summary>
        /// Write a nullable count
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="count">Count</param>
        /// <param name="action">Write action to execute, if the count isn't <see langword="null"/></param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream WriteNullableCount(this Stream stream, long? count, Action action)
        {
            if (WriteNullableCount(stream, count)) action();
            return stream;
        }

        /// <summary>
        /// Write a nullable count
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="count">Count</param>
        /// <param name="action">Write action to execute, if the count isn't <see langword="null"/></param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<Stream> WriteNullableCountAsync(this Stream stream, long? count, Func<Task> action, CancellationToken cancellationToken = default)
        {
            if (await WriteNullableCountAsync(stream, count, cancellationToken).DynamicContext())
                await action().DynamicContext();
            return stream;
        }

        /// <summary>
        /// Write serialized Data
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="data">Serialized data (will be returned to <c>pool</c>)</param>
        /// <param name="len">Data length in bytes</param>
        /// <param name="pool">Array pool (<c>data</c> will returned to that pool)</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
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
        /// <param name="data">Serialized data (will be returned to <c>pool</c>)</param>
        /// <param name="len">Data length in bytes</param>
        /// <param name="pool">Array pool (<c>data</c> will returned to that pool)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<Stream> WriteSerializedDataAsync(
            this Stream stream, 
            byte[] data, 
            int len, 
            ArrayPool<byte>? pool = null, 
            CancellationToken cancellationToken = default
            )
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
                return stream;
            });

        /// <summary>
        /// Write if a condition is <see langword="true"/>
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="condition">Condition</param>
        /// <param name="action">Write action to execute, if the condition is <see langword="true"/></param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream WriteIf(this Stream stream, bool condition, Func<Stream, Stream> action)
            => condition ? action(stream) : stream;

        /// <summary>
        /// Write if a condition is <see langword="true"/>
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="condition">Condition</param>
        /// <param name="action">Write action to execute, if the condition is <see langword="true"/></param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<Stream> WriteIfAsync(this Stream stream, bool condition, Func<Stream, Task<Stream>> action)
            => condition ? await action(stream).DynamicContext() : stream;

        /// <summary>
        /// Write if a condition is <see langword="true"/>
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="condition">Condition</param>
        /// <param name="action">Write action to execute, if the condition is <see langword="true"/></param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteIfAsync(this Task<Stream> stream, bool condition, Func<Stream, Task<Stream>> action)
            => AsyncHelper.FluentAsync(stream, async (s) => condition ? await action(s).DynamicContext() : s);
    }
}
