using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    /// <summary>
    /// Base class for a stream serializer context
    /// </summary>
    public abstract class SerializerContextBase : DisposableBase, ISerializerContext
    {
        /// <summary>
        /// Serialization context table
        /// </summary>
        public static readonly ConcurrentDictionary<int, ISerializationContext> SerializationContextTable = new();
        /// <summary>
        /// Deserialization context table
        /// </summary>
        public static readonly ConcurrentDictionary<int, IDeserializationContext> DeserializationContextTable = new();

        /// <summary>
        /// Cache size
        /// </summary>
        protected int _CacheSize = DefaultCacheSize;
        /// <summary>
        /// Current recursion level
        /// </summary>
        protected int _RecursionLevel = 0;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cacheSize">Cache size (1-<see cref="ushort.MaxValue"/>)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        protected SerializerContextBase(Stream stream, int? version = null, int? cacheSize = null, CancellationToken cancellationToken = default) : base()
        {
            if (version != null) this.EnsureValidArgument(nameof(version), 1, StreamSerializer.VERSION, version.Value & byte.MaxValue);
            if (cacheSize != null)
            {
                this.EnsureValidArgument(nameof(cacheSize), 0, ushort.MaxValue + 1, cacheSize.Value);
                _CacheSize = cacheSize.Value;
            }
            Stream = stream;
            Version = version ?? StreamSerializer.Version;
            Cancellation = cancellationToken;
            if(this is ISerializationContext sc)
            {
                SerializationContextTable.TryAdd(stream.GetHashCode(), sc);
            }
            else if(this is IDeserializationContext dc)
            {
                DeserializationContextTable.TryAdd(stream.GetHashCode(), dc);
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="baseContext">Base deserializer context</param>
        /// <param name="version">Serializer version</param>
        protected SerializerContextBase(SerializerContextBase baseContext, int version) : base()
        {
            this.EnsureValidArgument(nameof(baseContext), baseContext as IDeserializationContext, () => $"{nameof(baseContext)} is a {baseContext.GetType()}, but must be a {typeof(IDeserializationContext)}");
            this.EnsureValidArgument(nameof(version), 1, StreamSerializer.VERSION, version & byte.MaxValue);
            baseContext.EnsureUndisposed();
            BaseContext = baseContext;
            Stream = baseContext.Stream;
            Version = version;
            Cancellation = baseContext.Cancellation;
            _RecursionLevel = baseContext.RecursionLevel;
            BufferPool = baseContext.BufferPool;
            _CacheSize = baseContext.CacheSize;
            RealCacheSize = baseContext.RealCacheSize;
            CacheIndexSize = baseContext.CacheIndexSize;
            CacheOffset = baseContext.CacheOffset;
            Nullable = baseContext.Nullable;
        }

        /// <summary>
        /// Default context cache size (zero to disable caching at all)
        /// </summary>
        public static int DefaultCacheSize { get; set; } = byte.MaxValue;

        /// <summary>
        /// Max. recursion level
        /// </summary>
        public static int MaxRecursion { get; set; } = 32;

        /// <inheritdoc/>
        public SerializerContextBase? BaseContext { get; }

        /// <inheritdoc/>
        public Stream Stream { get; }

        /// <inheritdoc/>
        public abstract Type StreamType { get; }

        /// <inheritdoc/>
        public int Version { get; }

        /// <inheritdoc/>
        public int SerializerVersion => Version & byte.MaxValue;

        /// <inheritdoc/>
        public int CustomVersion => Version >> 8;

        /// <inheritdoc/>
        public int RecursionLevel
        {
            get => _RecursionLevel;
            set
            {
                SerializerException.Wrap(() => this.EnsureValidArgument(nameof(value), value >= 0, $"Invalid recursion level {value}"));
                if (value > MaxRecursion) throw new SerializerException($"Max. recursion of {MaxRecursion} exceeded ({value})", new StackOverflowException());
                _RecursionLevel = value;
            }
        }

        /// <inheritdoc/>
        public CancellationToken Cancellation { get; set; }

        /// <inheritdoc/>
        public abstract int CacheSize { get; set; }

        /// <inheritdoc/>
        public int RealCacheSize { get; protected set; } = DefaultCacheSize;

        /// <inheritdoc/>
        public int CacheIndexSize { get; protected set; }

        /// <inheritdoc/>
        public bool IsCacheEnabled => _CacheSize > 0;

        /// <inheritdoc/>
        public int CacheOffset { get; protected set; }

        /// <inheritdoc/>
        public ObjectTypes? LastObjectType { get; protected set; }

        /// <inheritdoc/>
        public NumberTypes? LastNumberType { get; protected set; }

        /// <inheritdoc/>
        public ArrayPool<byte> BufferPool { get; set; } = StreamSerializer.BufferPool;

        /// <inheritdoc/>
        public bool Nullable { get; set; }

        /// <inheritdoc/>
        public bool EnableCache()
        {
            if (IsCacheEnabled || RealCacheSize == 0) return false;
            _CacheSize = RealCacheSize;
            return true;
        }

        /// <inheritdoc/>
        public bool DisableCache()
        {
            if (!IsCacheEnabled) return false;
            _CacheSize = 0;
            return true;
        }

        /// <inheritdoc/>
        public virtual void DisposeStream()
        {
            Dispose();
            Stream.Dispose();
        }

        /// <inheritdoc/>
        public virtual async ValueTask DisposeStreamAsync()
        {
            await DisposeAsync().DynamicContext();
            await Stream.DisposeAsync().DynamicContext();
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (BaseContext == null)
            {
                int shc = Stream.GetHashCode();
                if (this is ISerializationContext && SerializationContextTable.TryGetValue(shc, out ISerializationContext? sc) && sc == this)
                {
                    SerializationContextTable.TryRemove(shc, out _);
                }
                else if (this is IDeserializationContext && DeserializationContextTable.TryGetValue(shc, out IDeserializationContext? dc) && dc == this)
                {
                    DeserializationContextTable.TryRemove(shc, out _);
                }
            }
        }
    }

    /// <summary>
    /// Base class for a stream serializer context
    /// </summary>
    /// <typeparam name="T">Stream type</typeparam>
    public abstract class SerializerContextBase<T> : SerializerContextBase where T : Stream
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="stream">Stream (won't be disposed)</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cacheSize">Cache size</param>
        /// <param name="cancellationToken">Cancellation token</param>
        protected SerializerContextBase(T stream, int? version = null, int? cacheSize = null, CancellationToken cancellationToken = default)
            : base(stream, version, cacheSize, cancellationToken)
            => GenericStream = stream;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="baseContext">Base deserializer context</param>
        /// <param name="version">Serializer version</param>
        protected SerializerContextBase(SerializerContextBase<T> baseContext, int version) : base(baseContext, version)
            => GenericStream = baseContext.GenericStream;

        /// <summary>
        /// Stream
        /// </summary>
        public T GenericStream { get; }

        /// <inheritdoc/>
        public override Type StreamType => typeof(T);
    }
}
