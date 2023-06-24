using System.Buffers;
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
        /// <param name="version">Serializer version</param>
        /// <returns>Enumerable</returns>
        [TargetedPatchingOptOut("Just a method adapter")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<tObject> Enumerate<tObject, tEnumerator>(this Stream stream, int? version = null)
            where tEnumerator : StreamEnumeratorBase<tObject>
            => StreamEnumeratorBase<tObject>.Enumerate<tEnumerator>(stream, version);

        /// <summary>
        /// Enumerate serialized objects
        /// </summary>
        /// <typeparam name="tObject">Object type</typeparam>
        /// <typeparam name="tEnumerator">Enumeratortype</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Enumerable</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async IAsyncEnumerable<tObject> EnumerateAsync<tObject, tEnumerator>(
            this Stream stream,
            int? version = null,
            [EnumeratorCancellation] CancellationToken cancellationToken = default
            )
            where tEnumerator : StreamAsyncEnumeratorBase<tObject>
        {
            await foreach (tObject obj in StreamAsyncEnumeratorBase<tObject>.EnumerateAsync<tEnumerator>(stream, version, cancellationToken)
                .WithCancellation(cancellationToken)
                .ConfigureAwait(continueOnCapturedContext: false)
                )
                yield return obj;
        }

        /// <summary>
        /// Enumerate serialized objects
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Version</param>
        /// <returns>Enumerable</returns>
        [TargetedPatchingOptOut("Just a method adapter")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> EnumerateSerialized<T>(this Stream stream, int? version = null)
            where T : class, IStreamSerializer
            => StreamEnumeratorBase<T>.Enumerate<StreamSerializerEnumerator<T>>(stream, version);

        /// <summary>
        /// Enumerate serialized objects
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Enumerable</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async IAsyncEnumerable<T> EnumerateSerializedAsync<T>(this Stream stream, int? version = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
            where T : class, IStreamSerializer
        {
            await foreach (T obj in StreamAsyncEnumeratorBase<T>.EnumerateAsync<StreamSerializerAsyncEnumerator<T>>(stream, version, cancellationToken)
                .WithCancellation(cancellationToken)
                .ConfigureAwait(continueOnCapturedContext: false)
                )
                yield return obj;
        }

        /// <summary>
        /// Enumerate numbers
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Version</param>
        /// <param name="pool">Array pool</param>
        /// <returns>Enumerable</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static IEnumerable<T> EnumerateNumber<T>(this Stream stream, int? version = null, ArrayPool<byte>? pool = null)
            where T : struct, IConvertible
        {
            using StreamNumberEnumerator<T> enumerator = new(stream, version, pool);
            while (enumerator.MoveNext()) yield return enumerator.Current;
        }

        /// <summary>
        /// Enumerate numbers
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Enumerable</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async IAsyncEnumerable<T> EnumerateNumberAsync<T>(
            this Stream stream,
            int? version = null,
            ArrayPool<byte>? pool = null,
            [EnumeratorCancellation] CancellationToken cancellationToken = default
            )
            where T : struct, IConvertible
        {
            StreamNumberAsyncEnumerator<T> enumerator = new(stream, version, pool, cancellationToken);
            await using (enumerator.DynamicContext())
                while (!cancellationToken.IsCancellationRequested && await enumerator.MoveNextAsync().DynamicContext())
                    yield return enumerator.Current;
        }

        /// <summary>
        /// Enumerate strings
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum UTF-8 string bytes length</param>
        /// <param name="maxLen">Maximum UTF-8 string bytes length</param>
        /// <returns>Enumerable</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static IEnumerable<string> EnumerateString(this Stream stream, int? version = null, ArrayPool<byte>? pool = null, int minLen = 0, int maxLen = int.MaxValue)
        {
            using StreamStringEnumerator enumerator = new(stream, version, pool, minLen, maxLen);
            while (enumerator.MoveNext()) yield return enumerator.Current;
        }

        /// <summary>
        /// Enumerate strings
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Version</param>
        /// <param name="pool">Array pool</param>
        /// <param name="minLen">Minimum UTF-8 string bytes length</param>
        /// <param name="maxLen">Maximum UTF-8 string bytes length</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Enumerable</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async IAsyncEnumerable<string> EnumerateStringAsync(
            this Stream stream,
            int? version = null,
            ArrayPool<byte>? pool = null,
            int minLen = 0,
            int maxLen = int.MaxValue,
            [EnumeratorCancellation] CancellationToken cancellationToken = default
            )
        {
            StreamStringAsyncEnumerator enumerator = new(stream, version, pool, minLen, maxLen, cancellationToken);
            await using (enumerator.DynamicContext())
                while (!cancellationToken.IsCancellationRequested && await enumerator.MoveNextAsync().DynamicContext())
                    yield return enumerator.Current;
        }
    }
}
