using System.Collections;
using wan24.Core;

namespace wan24.StreamSerializerExtensions.Enumerator
{
    /// <summary>
    /// Base class for a stream enumerator
    /// </summary>
    /// <typeparam name="T">Object type</typeparam>
    public abstract class StreamEnumeratorBase<T> : DisposableBase, IEnumerator<T>
    {
        /// <summary>
        /// Stream
        /// </summary>
        protected readonly IDeserializationContext Context;
        /// <summary>
        /// Stream start position
        /// </summary>
        protected long StartPosition;
        /// <summary>
        /// Current object
        /// </summary>
        protected T? _Current = default;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">Context</param>
        protected StreamEnumeratorBase(IDeserializationContext context) : base()
        {
            Context = context;
            StartPosition = context.Stream.CanSeek ? context.Stream.Position : 0;
        }

        /// <inheritdoc/>
        public virtual T Current => IfUndisposed(_Current!);

        /// <inheritdoc/>
        object IEnumerator.Current => Current!;

        /// <inheritdoc/>
        public virtual bool MoveNext()
        {
            EnsureUndisposed();
            if (Context.Stream.CanSeek && Context.Stream.Position == Context.Stream.Length) return false;
            try
            {
                _Current = ReadObject();
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

        /// <inheritdoc/>
        public virtual void Reset()
        {
            EnsureUndisposed();
            if (!Context.Stream.CanSeek) throw new NotSupportedException();
            Context.Stream.Position = StartPosition;
            _Current = default;
        }

        /// <summary>
        /// Read object method
        /// </summary>
        /// <returns>Object</returns>
        protected abstract T ReadObject();

        /// <inheritdoc/>
        protected override void Dispose(bool disposing) { }

        /// <summary>
        /// Enumerate
        /// </summary>
        /// <typeparam name="tEnumerator">Final enumerator type</typeparam>
        /// <param name="context">Context</param>
        /// <returns>Enumerable</returns>
        public static IEnumerable<T> Enumerate<tEnumerator>(IDeserializationContext context) where tEnumerator : StreamEnumeratorBase<T>
        {
            Type type = typeof(tEnumerator);
            ArgumentValidationHelper.EnsureValidArgument(nameof(tEnumerator), !type.IsAbstract, () => "Non-abstract type required");
            using StreamEnumeratorBase<T> enumerator = Activator.CreateInstance(type, context) as StreamEnumeratorBase<T>
                ?? throw new InvalidProgramException($"Failed to instance {type}");
            while (enumerator.MoveNext()) yield return enumerator.Current;
        }
    }
}
