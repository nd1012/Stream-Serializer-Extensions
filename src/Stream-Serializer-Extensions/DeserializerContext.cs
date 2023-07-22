using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    /// <summary>
    /// Stream deserializer context
    /// </summary>
    public class DeserializerContext : DeserializerContext<Stream>, IDeserializationContext
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="stream">Stream (won't be disposed)</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cacheSize">Cache size</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public DeserializerContext(Stream stream, int? version = null, int? cacheSize = null, CancellationToken cancellationToken = default)
            : base(stream, version, cacheSize, cancellationToken)
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="baseContext">Base deserializer context</param>
        /// <param name="version">Serializer version</param>
        public DeserializerContext(DeserializerContext baseContext, int version) : base(baseContext, version) { }

        /// <summary>
        /// Create a temporary instance which uses another serializer version
        /// </summary>
        /// <param name="version">New serializer version</param>
        /// <returns>Temporary instance (don't forget to dispose!)</returns>
        new public DeserializerContext WithSerializerVersion(int version) => new(this, version);

        /// <inheritdoc/>
        IDeserializationContext IDeserializationContext.WithSerializerVersion(int version) => WithSerializerVersion(version);
    }

    /// <summary>
    /// Stream deserializer context
    /// </summary>
    /// <typeparam name="T">Stream type</typeparam>
    public class DeserializerContext<T> : SerializerContextBase<T>, IDeserializationContext where T : Stream
    {
        /// <summary>
        /// Base context
        /// </summary>
        protected readonly DeserializerContext<T>? _BaseContext = null;
        /// <summary>
        /// Cache
        /// </summary>
        protected object[] _Cache;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="stream">Stream (won't be disposed)</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cacheSize">Cache size</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public DeserializerContext(T stream, int? version = null, int? cacheSize = null, CancellationToken cancellationToken = default)
            : base(stream, version, cacheSize, cancellationToken)
        {
            _Cache = _CacheSize > 0 ? StreamSerializer.ObjectCachePool.RentClean(_CacheSize) : Array.Empty<object>();
            CacheIndexSize = _CacheSize > byte.MaxValue ? 2 : 1;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="baseContext">Base deserializer context</param>
        /// <param name="version">Serializer version</param>
        public DeserializerContext(DeserializerContext<T> baseContext, int version) : base(baseContext, version)
        {
            _BaseContext = baseContext;
            _Cache = baseContext._Cache;
        }

        /// <summary>
        /// Cache
        /// </summary>
        public Memory<object> Cache
        {
            get
            {
                EnsureUndisposed();
                return _Cache.AsMemory(0, RealCacheSize);
            }
        }

        /// <inheritdoc/>
        public ISerializerOptions? Options { get; set; }

        /// <inheritdoc/>
        public override int CacheSize
        {
            get => _CacheSize;
            set
            {
                EnsureUndisposed();
                if (value == _CacheSize) return;
                if (_BaseContext != null)
                {
                    _BaseContext.CacheSize = value;
                    _CacheSize = _BaseContext._CacheSize;
                    RealCacheSize = _BaseContext.RealCacheSize;
                    CacheIndexSize = _BaseContext.CacheIndexSize;
                    _Cache = _BaseContext._Cache;
                    return;
                }
                if (value < 1)
                {
                    _CacheSize = value;
                    if (RealCacheSize != 0 && CacheOffset == 0)
                    {
                        StreamSerializer.ObjectCachePool.Return(_Cache);
                        _Cache = Array.Empty<object>();
                    }
                    return;
                }
                SerializerException.Wrap(() => this.EnsureValidArgument(nameof(value), CacheOffset, ushort.MaxValue + 1, value));
                if (CacheOffset != 0 && RealCacheSize > 0 && (value > byte.MaxValue ? 2 : 1) != CacheIndexSize)
                    throw new SerializerException("Can't change the cache index size", new InvalidOperationException());
                if (value > RealCacheSize)
                {
                    object[] newCache = StreamSerializer.ObjectCachePool.RentClean(value);
                    if (CacheOffset > 0) _Cache.AsSpan(0, CacheOffset - 1).CopyTo(newCache);
                    if (_Cache.Length != 0) StreamSerializer.ObjectCachePool.Return(_Cache);
                    _Cache = newCache;
                }
                _CacheSize = value;
                RealCacheSize = value;
                CacheIndexSize = _CacheSize > byte.MaxValue ? 2 : 1;
            }
        }

        /// <inheritdoc/>
        public tObject AddToCache<tObject>(tObject obj)
        {
            int index = CacheOffset;
            if (index >= _CacheSize) return obj;
            _Cache[index] = obj!;
            CacheOffset = index + 1;
            return obj;
        }

        /// <inheritdoc/>
        public bool TryReadCached<tObject>(out tObject? obj)
        {
            EnsureUndisposed();
            obj = default;
            if (!IsCacheEnabled) return false;
            SequenceTypes st = (SequenceTypes)Stream.ReadOneByte(this);
            switch (st & ~SequenceTypes.SmallIndex)
            {
                case SequenceTypes.Null: return true;
                case SequenceTypes.NotCached: return false;
                case SequenceTypes.Cached:
                    int index = CacheIndexSize == 1 || st.ContainsAllFlags(SequenceTypes.SmallIndex) ? Stream.ReadOneByte(this) : Stream.ReadUShort(this);
                    if (index < 0 || index >= CacheOffset) throw new SerializerException($"Invalid cache index #{index}", new InvalidDataException());
                    obj = SerializerException.Wrap(() => (tObject)_Cache[index]);
                    return true;
                default: throw new SerializerException($"Invalid sequence type {st}", new InvalidDataException());
            }
        }

        /// <inheritdoc/>
        public async Task<(bool Succeed, tObject? Object)> TryReadCachedAsync<tObject>()
        {
            EnsureUndisposed();
            if (!IsCacheEnabled) return (false, default);
            SequenceTypes st = (SequenceTypes)await Stream.ReadOneByteAsync(this).DynamicContext();
            switch (st & ~SequenceTypes.SmallIndex)
            {
                case SequenceTypes.Null: return (true, default);
                case SequenceTypes.NotCached: return (false, default);
                case SequenceTypes.Cached:
                    int index = CacheIndexSize == 1 || st.ContainsAllFlags(SequenceTypes.SmallIndex)
                        ? await Stream.ReadOneByteAsync(this).DynamicContext()
                        : await Stream.ReadUShortAsync(this).DynamicContext();
                    if (index < 0 || index >= CacheOffset) throw new SerializerException($"Invalid cache index #{index}", new InvalidDataException());
                    return (true, SerializerException.Wrap(() => (tObject)_Cache[index]));
                default: throw new SerializerException($"Invalid sequence type {st}", new InvalidDataException());
            }
        }

        /// <inheritdoc/>
        public bool TryReadCachedObject<tObject>(out tObject? obj, bool readType = false)
        {
            EnsureUndisposed();
            obj = default;
            if (!IsCacheEnabled) return false;
            SequenceTypes st = (SequenceTypes)Stream.ReadOneByte(this);
            ObjectTypes objType;
            if (readType)
            {
                objType = st switch
                {
                    SequenceTypes.Cached => ObjectTypes.Null,
                    SequenceTypes.NotCached => (ObjectTypes)Stream.ReadOneByte(this),
                    _ => (ObjectTypes)(st & ~SequenceTypes.ALL_FLAGS)
                };
                if (objType != ObjectTypes.Null)
                {
                    LastObjectType = (ObjectTypes)(st & ~SequenceTypes.ALL_VALUES);
                    st &= SequenceTypes.ALL_FLAGS;
                }
                else
                {
                    LastObjectType = null;
                }
            }
            else
            {
                objType = ObjectTypes.Null;
                LastObjectType = null;
            }
            switch (st & ~SequenceTypes.SmallIndex)
            {
                case SequenceTypes.Null: return true;
                case SequenceTypes.NotCached: return false;
                case SequenceTypes.Cached:
                    int index = CacheIndexSize == 1 || st.ContainsAllFlags(SequenceTypes.SmallIndex) ? Stream.ReadOneByte(this) : Stream.ReadUShort(this);
                    if (index < 0 || index >= CacheOffset) throw new SerializerException($"Invalid cache index #{index}", new InvalidDataException());
                    obj = SerializerException.Wrap(() => (tObject)_Cache[index]);
                    if (readType) LastObjectType = obj!.GetObjectSerializerInfo().ObjectType;
                    return true;
                default: throw new SerializerException($"Invalid sequence type {st}", new InvalidDataException());
            }
        }

        /// <inheritdoc/>
        public async Task<(bool Succeed, tObject? Object)> TryReadCachedObjectAsync<tObject>(bool readType = false)
        {
            EnsureUndisposed();
            if (!IsCacheEnabled) return (false, default);
            SequenceTypes st = (SequenceTypes)await Stream.ReadOneByteAsync(this).DynamicContext();
            ObjectTypes objType;
            if (readType)
            {
                objType = st switch
                {
                    SequenceTypes.Cached => ObjectTypes.Null,
                    SequenceTypes.NotCached => (ObjectTypes)await Stream.ReadOneByteAsync(this).DynamicContext(),
                    _ => (ObjectTypes)(st & ~SequenceTypes.ALL_FLAGS)
                };
                if (objType != ObjectTypes.Null)
                {
                    LastObjectType = (ObjectTypes)(st & ~SequenceTypes.ALL_VALUES);
                    st &= SequenceTypes.ALL_FLAGS;
                }
                else
                {
                    LastObjectType = null;
                }
            }
            else
            {
                objType = ObjectTypes.Null;
                LastObjectType = null;
            }
            switch (st & ~SequenceTypes.SmallIndex)
            {
                case SequenceTypes.Null:
                    LastObjectType = ObjectTypes.Null;
                    return (true, default);
                case SequenceTypes.NotCached: return (false, default);
                case SequenceTypes.Cached:
                    int index = CacheIndexSize == 1 || st.ContainsAllFlags(SequenceTypes.SmallIndex)
                        ? await Stream.ReadOneByteAsync(this).DynamicContext()
                        : await Stream.ReadUShortAsync(this).DynamicContext();
                    if (index < 0 || index >= CacheOffset) throw new SerializerException($"Invalid cache index #{index}", new InvalidDataException());
                    tObject obj = SerializerException.Wrap(() => (tObject)_Cache[index])!;
                    if (readType) LastObjectType = obj.GetObjectSerializerInfo().ObjectType;
                    return (true, obj);
                default: throw new SerializerException($"Invalid sequence type {st}", new InvalidDataException());
            }
        }

        /// <inheritdoc/>
        public bool TryReadCachedObjectCountable<tObject>(out tObject? obj, out long len, bool readType = false)
        {
            EnsureUndisposed();
            obj = default;
            len = 0;
            if (!IsCacheEnabled) return false;
            SequenceTypes st = (SequenceTypes)Stream.ReadOneByte(this);
            LastNumberType = (st & ~SequenceTypes.SmallIndex) switch
            {
                SequenceTypes.Cached => null,
                SequenceTypes.NotCached => (NumberTypes)Stream.ReadOneByte(this),
                _ => (NumberTypes)(st & ~SequenceTypes.ALL_VALUES)
            };
            st &= SequenceTypes.ALL_FLAGS;
            switch (st & ~SequenceTypes.SmallIndex)
            {
                case SequenceTypes.Null:
                    LastObjectType = ObjectTypes.Null;
                    return true;
                case SequenceTypes.NotCached:
                    len = (long)StreamExtensions.ReadNumberInt(this, typeof(long), LastNumberType);
                    LastObjectType = readType ? (ObjectTypes)Stream.ReadOneByte(this) : null;
                    return false;
                case SequenceTypes.Cached:
                    int index = CacheIndexSize == 1 || st.ContainsAllFlags(SequenceTypes.SmallIndex) ? Stream.ReadOneByte(this) : Stream.ReadUShort(this);
                    if (index < 0 || index >= CacheOffset) throw new SerializerException($"Invalid cache index #{index}", new InvalidDataException());
                    obj = SerializerException.Wrap(() => (tObject)_Cache[index]);
                    LastObjectType = obj!.GetObjectSerializerInfo().ObjectType;
                    return true;
                default: throw new SerializerException($"Invalid sequence type {st}", new InvalidDataException());
            }
        }

        /// <inheritdoc/>
        public async Task<(bool Succeed, tObject? Object, long Length)> TryReadCachedObjectCountableAsync<tObject>(bool readType = false)
        {
            EnsureUndisposed();
            if (!IsCacheEnabled) return (false, default, 0);
            SequenceTypes st = (SequenceTypes)await Stream.ReadOneByteAsync(this).DynamicContext();
            LastNumberType = (st & ~SequenceTypes.SmallIndex) switch
            {
                SequenceTypes.Cached => null,
                SequenceTypes.NotCached => (NumberTypes)await Stream.ReadOneByteAsync(this).DynamicContext(),
                _ => (NumberTypes)(st & ~SequenceTypes.ALL_VALUES)
            };
            st &= SequenceTypes.ALL_FLAGS;
            switch (st & ~SequenceTypes.SmallIndex)
            {
                case SequenceTypes.Null:
                    LastObjectType = ObjectTypes.Null;
                    return (true, default, 0);
                case SequenceTypes.NotCached:
                    long len = (long)await StreamExtensions.ReadNumberIntAsync(this, typeof(long), LastNumberType).DynamicContext();
                    LastObjectType = readType ? (ObjectTypes)await Stream.ReadOneByteAsync(this).DynamicContext() : null;
                    return (false, default, len);
                case SequenceTypes.Cached:
                    int index = CacheIndexSize == 1 || st.ContainsAllFlags(SequenceTypes.SmallIndex)
                        ? await Stream.ReadOneByteAsync(this).DynamicContext()
                        : await Stream.ReadUShortAsync(this).DynamicContext();
                    if (index < 0 || index >= CacheOffset) throw new SerializerException($"Invalid cache index #{index}", new InvalidDataException());
                    return (true, SerializerException.Wrap(() => (tObject)_Cache[index]), 0);
                default: throw new SerializerException($"Invalid sequence type {st}", new InvalidDataException());
            }
        }

        /// <inheritdoc/>
        public bool TryReadCachedNumber<tObject>(out tObject? obj, bool readType = false) where tObject : struct, IConvertible
        {
            EnsureUndisposed();
            obj = default;
            if (!IsCacheEnabled) return false;
            SequenceTypes st = (SequenceTypes)Stream.ReadOneByte(this);
            NumberTypes numberType;
            if (readType)
            {
                numberType = st switch
                {
                    SequenceTypes.Cached => NumberTypes.None,
                    SequenceTypes.NotCached => (NumberTypes)Stream.ReadOneByte(this),
                    _ => (NumberTypes)(st & ~SequenceTypes.ALL_FLAGS)
                };
                if (numberType != NumberTypes.None)
                {
                    LastNumberType = (NumberTypes)(st & ~SequenceTypes.ALL_VALUES);
                    st &= SequenceTypes.ALL_FLAGS;
                }
                else
                {
                    LastNumberType = null;
                }
            }
            else
            {
                numberType = NumberTypes.None;
                LastNumberType = null;
            }
            switch (st & ~SequenceTypes.SmallIndex)
            {
                case SequenceTypes.Null: return true;
                case SequenceTypes.NotCached: return false;
                case SequenceTypes.Cached:
                    int index = CacheIndexSize == 1 || st.ContainsAllFlags(SequenceTypes.SmallIndex) ? Stream.ReadOneByte(this) : Stream.ReadUShort(this);
                    if (index < 0 || index >= CacheOffset) throw new SerializerException($"Invalid cache index #{index}", new InvalidDataException());
                    obj = SerializerException.Wrap(() => (tObject)_Cache[index]);
                    LastNumberType = obj.GetNumberType();
                    return true;
                default: throw new SerializerException($"Invalid sequence type {st}", new InvalidDataException());
            }
        }

        /// <inheritdoc/>
        public async Task<(bool Succeed, tObject? Object)> TryReadCachedNumberAsync<tObject>(bool readType = false) where tObject : struct, IConvertible
        {
            EnsureUndisposed();
            if (!IsCacheEnabled) return (false, default);
            SequenceTypes st = (SequenceTypes)await Stream.ReadOneByteAsync(this).DynamicContext();
            NumberTypes numberType;
            if (readType)
            {
                numberType = st switch
                {
                    SequenceTypes.Cached => NumberTypes.None,
                    SequenceTypes.NotCached => (NumberTypes)Stream.ReadOneByte(this),
                    _ => (NumberTypes)(st & ~SequenceTypes.ALL_FLAGS)
                };
                if (numberType != NumberTypes.None)
                {
                    LastNumberType = (NumberTypes)(st & ~SequenceTypes.ALL_VALUES);
                    st &= SequenceTypes.ALL_FLAGS;
                }
                else
                {
                    LastNumberType = null;
                }
            }
            else
            {
                numberType = NumberTypes.None;
                LastNumberType = null;
            }
            switch (st & ~SequenceTypes.SmallIndex)
            {
                case SequenceTypes.Null: return (true, default);
                case SequenceTypes.NotCached: return (false, default);
                case SequenceTypes.Cached:
                    int index = CacheIndexSize == 1 || st.ContainsAllFlags(SequenceTypes.SmallIndex)
                        ? await Stream.ReadOneByteAsync(this).DynamicContext()
                        : await Stream.ReadUShortAsync(this).DynamicContext();
                    if (index < 0 || index >= CacheOffset) throw new SerializerException($"Invalid cache index #{index}", new InvalidDataException());
                    tObject num = SerializerException.Wrap(() => (tObject)_Cache[index]);
                    LastNumberType = num.GetNumberType();
                    return (true, num);
                default: throw new SerializerException($"Invalid sequence type {st}", new InvalidDataException());
            }
        }

        /// <summary>
        /// Create a temporary instance which uses another serializer version
        /// </summary>
        /// <param name="version">New serializer version</param>
        /// <returns>Temporary instance (don't forget to dispose!)</returns>
        public DeserializerContext<T> WithSerializerVersion(int version) => new(this, version);

        /// <inheritdoc/>
        IDeserializationContext IDeserializationContext.WithSerializerVersion(int version) => WithSerializerVersion(version);

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (_BaseContext == null && _Cache.Length != 0)
                StreamSerializer.ObjectCachePool.Return(_Cache);
        }
    }
}
