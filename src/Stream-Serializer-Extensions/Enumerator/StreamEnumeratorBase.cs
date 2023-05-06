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
        protected readonly Stream Stream;
        /// <summary>
        /// Serializer version
        /// </summary>
        protected readonly int SerializerVersion;
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
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        protected StreamEnumeratorBase(Stream stream, int? version = null) : base()
        {
            SerializerVersion = version ?? StreamSerializer.VERSION;
            StartPosition = stream.CanSeek ? stream.Position : 0;
            Stream = stream;
        }

        /// <inheritdoc/>
        public virtual T Current => IfUndisposed(_Current!);

        /// <inheritdoc/>
        object IEnumerator.Current => Current!;

        /// <inheritdoc/>
        public virtual bool MoveNext()
        {
            EnsureUndisposed();
            if (Stream.CanSeek && Stream.Position == Stream.Length) return false;
            try
            {
                _Current = ReadObject();
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

        /// <inheritdoc/>
        public virtual void Reset()
        {
            EnsureUndisposed();
            if (!Stream.CanSeek) throw new NotSupportedException();
            Stream.Position = StartPosition;
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
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <returns>Enumerable</returns>
        public static IEnumerable<T> Enumerate<tEnumerator>(Stream stream, int? version = null) where tEnumerator : StreamEnumeratorBase<T>
        {
            Type type = typeof(tEnumerator);
            if (type.IsAbstract) throw new ArgumentException("Non-abstract type required", nameof(tEnumerator));
            using StreamEnumeratorBase<T> enumerator = Activator.CreateInstance(type, stream, version) as StreamEnumeratorBase<T>
                ?? throw new InvalidProgramException($"Failed to instance {type}");
            while (enumerator.MoveNext()) yield return enumerator.Current;
        }
    }
}
