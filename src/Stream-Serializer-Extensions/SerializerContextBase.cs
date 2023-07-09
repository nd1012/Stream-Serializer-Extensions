using System.Buffers;
using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    /// <summary>
    /// Base class for a stream serializer context
    /// </summary>
    public abstract class SerializerContextBase : DisposableBase, ISerializerContext
    {
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
        /// <param name="stream">Stream (won't be disposed)</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cacheSize">Cache size</param>
        /// <param name="cancellationToken">Cancellation token</param>
        protected SerializerContextBase(Stream stream, int? version = null, int? cacheSize = null, CancellationToken cancellationToken = default) : base()
        {
            if (cacheSize != null)
            {
                this.EnsureValidArgument(nameof(cacheSize), 0, ushort.MaxValue + 1, cacheSize.Value);
                _CacheSize = cacheSize.Value;
            }
            Stream = stream;
            Version = version ?? StreamSerializer.Version;
            Cancellation = cancellationToken;
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
        public Stream Stream { get; }

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
            CacheSize = RealCacheSize;
            return true;
        }

        /// <inheritdoc/>
        public bool DisableCache()
        {
            if (!IsCacheEnabled) return false;
            CacheSize = 0;
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
    }
}
