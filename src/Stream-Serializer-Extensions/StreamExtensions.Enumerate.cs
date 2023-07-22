using System.Runtime;
using System.Runtime.CompilerServices;
using wan24.Core;
using wan24.StreamSerializerExtensions.Enumerator;

namespace wan24.StreamSerializerExtensions
{
    // Enumerate
    public static partial class StreamExtensions
    {
        /// <summary>
        /// Enumerate serialized objects
        /// </summary>
        /// <typeparam name="tObject">Object type</typeparam>
        /// <typeparam name="tEnumerator">Enumeratortype</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Enumerable</returns>
        [TargetedPatchingOptOut("Just a method adapter")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#pragma warning disable IDE0060 // Remove unused argument
        public static IEnumerable<tObject> Enumerate<tObject, tEnumerator>(this Stream stream, IDeserializationContext context)
#pragma warning restore IDE0060 // Remove unused argument

            where tEnumerator : StreamEnumeratorBase<tObject>
            => StreamEnumeratorBase<tObject>.Enumerate<tEnumerator>(context);

        /// <summary>
        /// Enumerate serialized objects
        /// </summary>
        /// <typeparam name="tObject">Object type</typeparam>
        /// <typeparam name="tEnumerator">Enumeratortype</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Enumerable</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
#pragma warning disable IDE0060 // Remove unused argument
        public static async IAsyncEnumerable<tObject> EnumerateAsync<tObject, tEnumerator>(this Stream stream, IDeserializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            where tEnumerator : StreamAsyncEnumeratorBase<tObject>
        {
            await foreach (tObject obj in StreamAsyncEnumeratorBase<tObject>.EnumerateAsync<tEnumerator>(context)
                .WithCancellation(context.Cancellation)
                .ConfigureAwait(continueOnCapturedContext: false)
                )
                yield return obj;
        }

        /// <summary>
        /// Enumerate serialized objects
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Enumerable</returns>
        [TargetedPatchingOptOut("Just a method adapter")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#pragma warning disable IDE0060 // Remove unused argument
        public static IEnumerable<T> EnumerateSerialized<T>(this Stream stream, IDeserializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            where T : class, IStreamSerializer
            => StreamEnumeratorBase<T>.Enumerate<StreamSerializerEnumerator<T>>(context);

        /// <summary>
        /// Enumerate serialized objects
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Enumerable</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
#pragma warning disable IDE0060 // Remove unused argument
        public static async IAsyncEnumerable<T> EnumerateSerializedAsync<T>(this Stream stream, IDeserializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            where T : class, IStreamSerializer
        {
            await foreach (T obj in StreamAsyncEnumeratorBase<T>.EnumerateAsync<StreamSerializerAsyncEnumerator<T>>(context)
                .WithCancellation(context.Cancellation)
                .ConfigureAwait(continueOnCapturedContext: false)
                )
                yield return obj;
        }

        /// <summary>
        /// Enumerate numbers
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Enumerable</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
#pragma warning disable IDE0060 // Remove unused argument
        public static IEnumerable<T> EnumerateNumber<T>(this Stream stream, IDeserializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            where T : struct, IConvertible
        {
            using StreamNumberEnumerator<T> enumerator = new(context);
            while (enumerator.MoveNext()) yield return enumerator.Current;
        }

        /// <summary>
        /// Enumerate numbers
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Enumerable</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
#pragma warning disable IDE0060 // Remove unused argument
        public static async IAsyncEnumerable<T> EnumerateNumberAsync<T>(this Stream stream, IDeserializationContext context)
#pragma warning restore IDE0060 // Remove unused argument
            where T : struct, IConvertible
        {
            StreamNumberAsyncEnumerator<T> enumerator = new(context);
            await using (enumerator.DynamicContext())
                while (!context.Cancellation.IsCancellationRequested && await enumerator.MoveNextAsync().DynamicContext())
                    yield return enumerator.Current;
        }

        /// <summary>
        /// Enumerate strings
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <param name="minLen">Minimum UTF-8 string bytes length</param>
        /// <param name="maxLen">Maximum UTF-8 string bytes length</param>
        /// <returns>Enumerable</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
#pragma warning disable IDE0060 // Remove unused argument
        public static IEnumerable<string> EnumerateString(this Stream stream, IDeserializationContext context, int minLen = 0, int maxLen = int.MaxValue)
#pragma warning restore IDE0060 // Remove unused argument
        {
            using StreamStringEnumerator enumerator = new(context, minLen, maxLen);
            while (enumerator.MoveNext()) yield return enumerator.Current;
        }

        /// <summary>
        /// Enumerate strings
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <param name="minLen">Minimum UTF-8 string bytes length</param>
        /// <param name="maxLen">Maximum UTF-8 string bytes length</param>
        /// <returns>Enumerable</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
#pragma warning disable IDE0060 // Remove unused argument
        public static async IAsyncEnumerable<string> EnumerateStringAsync(this Stream stream, IDeserializationContext context, int minLen = 0, int maxLen = int.MaxValue)
#pragma warning restore IDE0060 // Remove unused argument
        {
            StreamStringAsyncEnumerator enumerator = new(context, minLen, maxLen);
            await using (enumerator.DynamicContext())
                while (!context.Cancellation.IsCancellationRequested && await enumerator.MoveNextAsync().DynamicContext())
                    yield return enumerator.Current;
        }
    }
}
