using System.Runtime.CompilerServices;
using wan24.Core;

namespace wan24.StreamSerializerExtensions.Enumerator
{
    /// <summary>
    /// Base class for a stream enumerator
    /// </summary>
    /// <typeparam name="T">Object type</typeparam>
    public abstract class StreamAsyncEnumeratorBase<T> : DisposableBase, IAsyncEnumerator<T>
    {
        /// <summary>
        /// Stream
        /// </summary>
        protected readonly Stream Stream;
        /// <summary>
        /// Serializer version
        /// </summary>
        protected readonly int SerializerVersion;
        /// <summary>
        /// Cancellation token
        /// </summary>
        protected readonly CancellationToken Cancellation;
        /// <summary>
        /// Current object
        /// </summary>
        protected T? _Current = default;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        protected StreamAsyncEnumeratorBase(Stream stream, int? version = null, CancellationToken cancellationToken = default) : base()
        {
            SerializerVersion = version ?? StreamSerializer.VERSION;
            Cancellation = cancellationToken;
            Stream = stream;
        }

        /// <inheritdoc/>
        public T Current => IfUndisposed(_Current!);

        /// <inheritdoc/>
        public async ValueTask<bool> MoveNextAsync()
        {
            EnsureUndisposed();
            if (Stream.CanSeek && Stream.Position == Stream.Length) return false;
            try
            {
                _Current = await ReadObjectAsync().DynamicContext();
            }
            catch (IOException)
            {
                if (!Stream.CanSeek) return false;
                throw;
            }
            catch (SerializerException ex)
            {
                if (!Stream.CanSeek && ex.InnerException is IOException) return false;
                throw;
            }
            return true;
        }

        /// <summary>
        /// Read object method
        /// </summary>
        /// <returns>Object</returns>
        protected abstract Task<T> ReadObjectAsync();

        /// <inheritdoc/>
        protected override void Dispose(bool disposing) { }

        /// <summary>
        /// Enumerate
        /// </summary>
        /// <typeparam name="tEnumerator">Final enumerator type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Enumerable</returns>
        public static async IAsyncEnumerable<T> EnumerateAsync<tEnumerator>(Stream stream, int? version = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
            where tEnumerator : StreamAsyncEnumeratorBase<T>
        {
            Type type = typeof(tEnumerator);
            if (type.IsAbstract) throw new ArgumentException("Non-abstract type required", nameof(tEnumerator));
            StreamAsyncEnumeratorBase<T> enumerator = Activator.CreateInstance(type, stream, version, cancellationToken) as StreamAsyncEnumeratorBase<T>
                ?? throw new InvalidProgramException($"Failed to instance {type}");
            await using (enumerator.DynamicContext())
                while (!cancellationToken.IsCancellationRequested && await enumerator.MoveNextAsync().DynamicContext())
                    yield return enumerator.Current;
        }
    }
}
