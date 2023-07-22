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
            ReadStructNullableMethod = type.GetMethodCached(nameof(ReadStructNullable), BindingFlags.Static | BindingFlags.Public)
                ?? throw new TypeLoadException($"Failed to reflect {nameof(ReadStructNullable)}");
            ReadStructNullableAsyncMethod = type.GetMethodCached(nameof(ReadStructNullableAsync), BindingFlags.Static | BindingFlags.Public)
                ?? throw new TypeLoadException($"Failed to reflect {nameof(ReadStructNullableAsync)}");
        }

        /// <summary>
        /// Write the serializer version
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream WriteSerializerVersion(this Stream stream, ISerializationContext context)
        {
            // The serializer version number sequence is fixed to serializer version #2
            (object n, NumberTypes nt) = StreamSerializer.Version.GetNumberAndType();
            Write(stream, (byte)nt, context);
            switch (n)
            {
                case sbyte sb: Write(stream, sb, context); break;
                case byte b: Write(stream, b, context); break;
                case short s: Write(stream, s, context); break;
                case ushort us: Write(stream, us, context); break;
                case int i: Write(stream, i, context); break;
                default: throw new SerializerException($"Invalid numeric type {nt} for serializer version {StreamSerializer.Version}");
            }
            return stream;
        }

        /// <summary>
        /// Write the serializer version
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<Stream> WriteSerializerVersionAsync(this Stream stream, ISerializationContext context)
        {
            // The serializer version number sequence is fixed to serializer version #2
            (object n, NumberTypes nt) = StreamSerializer.Version.GetNumberAndType();
            await WriteAsync(stream, (byte)nt, context).DynamicContext();
            switch (n)
            {
                case sbyte sb: await WriteAsync(stream, sb, context).DynamicContext(); break;
                case byte b: await WriteAsync(stream, b, context).DynamicContext(); break;
                case short s: await WriteAsync(stream, s, context).DynamicContext(); break;
                case ushort us: await WriteAsync(stream, us, context).DynamicContext(); break;
                case int i: await WriteAsync(stream, i, context).DynamicContext(); break;
                default: throw new SerializerException($"Invalid numeric type {nt} for serializer version {StreamSerializer.Version}");
            }
            return stream;
        }

        /// <summary>
        /// Write the serializer version
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteSerializerVersionAsync(this Task<Stream> stream, ISerializationContext context)
            => AsyncHelper.FluentAsync(stream, context, WriteSerializerVersionAsync);

        /// <summary>
        /// Write a boolean flag if an object is not <see langword="null"/>
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="context">Context</param>
        /// <returns>Is not <see langword="null"/>?</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool WriteIfNotNull<T>(this Stream stream, [NotNullWhen(true)] T? obj, ISerializationContext context)
        {
            bool isNotNull = obj != null;
            Write(stream, isNotNull, context);
            return isNotNull;
        }

        /// <summary>
        /// Write a boolean flag if an object is not <see langword="null"/>
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="context">Context</param>
        /// <returns>Is not <see langword="null"/>?</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<bool> WriteIfNotNullAsync<T>(this Stream stream, [NotNullWhen(true)] T? obj, ISerializationContext context)
        {
            //TODO NotNullWhen seems not to work with a task result https://github.com/dotnet/roslyn/issues/45228
            bool isNotNull = obj != null;
            await WriteAsync(stream, isNotNull, context).DynamicContext();
            return isNotNull;
        }

        /// <summary>
        /// Write a nullable count
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="count">Count</param>
        /// <returns>Is not <see langword="null"/>?</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool WriteNullableCount(this ISerializationContext context, [NotNullWhen(true)] long? count)
        {
            WriteNumberNullable(context.Stream, count, context);
            return count != null;
        }

        /// <summary>
        /// Write a nullable count
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="count">Count</param>
        /// <returns>Is not <see langword="null"/>?</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<bool> WriteNullableCountAsync(this ISerializationContext context, [NotNullWhen(true)] long? count)
        {
            //TODO NotNullWhen seems not to work with a task result https://github.com/dotnet/roslyn/issues/45228
            await WriteNumberNullableAsync(context.Stream, count, context).DynamicContext();
            return count != null;
        }

        /// <summary>
        /// Write a boolean flag if an object is not <see langword="null"/>
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="action">Write action to execute, if the object isn't <see langword="null"/></param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream WriteIfNotNull<T>(this Stream stream, T? obj, NullableWriter_Delegate action, ISerializationContext context)
        {
            if (WriteIfNotNull(stream, obj, context)) action();
            return stream;
        }

        /// <summary>
        /// Write a boolean flag if an object is not <see langword="null"/>
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="action">Write action to execute, if the object isn't <see langword="null"/></param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<Stream> WriteIfNotNullAsync<T>(this Stream stream, T? obj, AsyncNullableWriter_Delegate action, ISerializationContext context)
        {
            if (await WriteIfNotNullAsync(stream, obj, context).DynamicContext())
                await action().DynamicContext();
            return stream;
        }

        /// <summary>
        /// Write a nullable count
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="count">Count</param>
        /// <param name="action">Write action to execute, if the count isn't <see langword="null"/></param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream WriteNullableCount(this ISerializationContext context, long? count, NullableWriter_Delegate action)
        {
            if (WriteNullableCount(context, count)) action();
            return context.Stream;
        }

        /// <summary>
        /// Write a nullable count
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="count">Count</param>
        /// <param name="action">Write action to execute, if the count isn't <see langword="null"/></param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<Stream> WriteNullableCountAsync(this ISerializationContext context, long? count, AsyncNullableWriter_Delegate action)
        {
            if (await WriteNullableCountAsync(context, count).DynamicContext())
                await action().DynamicContext();
            return context.Stream;
        }

        /// <summary>
        /// Write serialized Data
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="data">Serialized data (will be returned to <c>pool</c>)</param>
        /// <param name="len">Data length in bytes</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream WriteSerializedData(this ISerializationContext context, byte[] data, int len)
            => SerializerException.Wrap(() =>
            {
                try
                {
                    context.Stream.Write(data.AsSpan(0, len));
                    return context.Stream;
                }
                finally
                {
                    StreamSerializer.BufferPool.Return(data);
                }
            });

        /// <summary>
        /// Write serialized Data
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="data">Serialized data (will be returned to <c>pool</c>)</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream WriteSerializedData(this ISerializationContext context, Memory<byte> data)
            => SerializerException.Wrap(() =>
            {
                context.Stream.Write(data.Span);
                return context.Stream;
            });

        /// <summary>
        /// Write serialized Data
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="data">Serialized data (will be returned to <c>pool</c>)</param>
        /// <param name="len">Data length in bytes</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<Stream> WriteSerializedDataAsync(this ISerializationContext context, byte[] data, int len)
            => await SerializerException.WrapAsync(async () =>
            {
                try
                {
                    await context.Stream.WriteAsync(data.AsMemory(0, len), context.Cancellation).DynamicContext();
                    return context.Stream;
                }
                finally
                {
                    StreamSerializer.BufferPool.Return(data);
                }
            });

        /// <summary>
        /// Write serialized Data
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="data">Serialized data (will be returned to <c>pool</c>)</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<Stream> WriteSerializedDataAsync(this ISerializationContext context, Memory<byte> data)
            => await SerializerException.WrapAsync(async () =>
            {
                await context.Stream.WriteAsync(data, context.Cancellation).DynamicContext();
                return context.Stream;
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
        public static Stream WriteIf(this Stream stream, bool condition, StreamNullableWiter_Delegate action)
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
        public static async Task<Stream> WriteIfAsync(this Stream stream, bool condition, AsyncStreamNullableWiter_Delegate action)
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
        public static Task<Stream> WriteIfAsync(this Task<Stream> stream, bool condition, AsyncStreamNullableWiter_Delegate action)
            => AsyncHelper.FluentAsync(stream, async (s) => condition ? await action(s).DynamicContext() : s);

        /// <summary>
        /// Delegate for a nullable writer
        /// </summary>
        public delegate void NullableWriter_Delegate();

        /// <summary>
        /// Delegate for a nullable writer
        /// </summary>
        public delegate Task AsyncNullableWriter_Delegate();

        /// <summary>
        /// Delegate for a nullable writer, which uses a stream
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>Stream</returns>
        public delegate Stream StreamNullableWiter_Delegate(Stream stream);

        /// <summary>
        /// Delegate for a nullable writer, which uses a stream
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>Stream</returns>
        public delegate Task<Stream> AsyncStreamNullableWiter_Delegate(Stream stream);
    }
}
