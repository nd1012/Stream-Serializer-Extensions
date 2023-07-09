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
        protected readonly IDeserializationContext Context;
        /// <summary>
        /// Current object
        /// </summary>
        protected T? _Current = default;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">Context</param>
        protected StreamAsyncEnumeratorBase(IDeserializationContext context) : base() => Context = context;

        /// <inheritdoc/>
        public T Current => IfUndisposed(_Current!);

        /// <inheritdoc/>
        public async ValueTask<bool> MoveNextAsync()
        {
            EnsureUndisposed();
            if (Context.Stream.CanSeek && Context.Stream.Position == Context.Stream.Length) return false;
            try
            {
                _Current = await ReadObjectAsync().DynamicContext();
            }
            catch (IOException)
            {
                if (!Context.Stream.CanSeek) return false;
                throw;
            }
            catch (SerializerException ex)
            {
                if (!Context.Stream.CanSeek && ex.InnerException is IOException) return false;
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
        /// <param name="context">Context</param>
        /// <returns>Enumerable</returns>
        public static async IAsyncEnumerable<T> EnumerateAsync<tEnumerator>(IDeserializationContext context)
            where tEnumerator : StreamAsyncEnumeratorBase<T>
        {
            Type type = typeof(tEnumerator);
            ArgumentValidationHelper.EnsureValidArgument(nameof(type), !type.IsAbstract, () => "Non-abstract type required");
            StreamAsyncEnumeratorBase<T> enumerator = Activator.CreateInstance(type, context) as StreamAsyncEnumeratorBase<T>
                ?? throw new InvalidProgramException($"Failed to instance {type}");
            await using (enumerator.DynamicContext())
                while (!context.Cancellation.IsCancellationRequested && await enumerator.MoveNextAsync().DynamicContext())
                    yield return enumerator.Current;
        }
    }
}
