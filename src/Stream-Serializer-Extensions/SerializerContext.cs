using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    /// <summary>
    /// Stream serializer writing context
    /// </summary>
    public class SerializerContext : SerializerContextBase, ISerializationContext
    {
        /// <summary>
        /// Cache
        /// </summary>
        protected int[] _Cache;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="stream">Stream (won't be disposed)</param>
        /// <param name="cacheSize">Cache size</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public SerializerContext(Stream stream, int? cacheSize = null, CancellationToken cancellationToken = default)
            : base(stream, StreamSerializer.Version, cacheSize, cancellationToken)
        {
            _Cache = _CacheSize > 0 ? StreamSerializer.HashCodeCachePool.RentClean(_CacheSize) : Array.Empty<int>();
            CacheIndexSize = _CacheSize > byte.MaxValue ? 2 : 1;
        }

        /// <summary>
        /// Cache
        /// </summary>
        public Memory<int> Cache
        {
            get
            {
                EnsureUndisposed();
                return _Cache.AsMemory(0, RealCacheSize);
            }
        }

        /// <inheritdoc/>
        public override int CacheSize
        {
            get => _CacheSize;
            set
            {
                EnsureUndisposed();
                if (value == _CacheSize) return;
                if (value < 1)
                {
                    _CacheSize = value;
                    if (RealCacheSize != 0 && CacheOffset == 0)
                    {
                        StreamSerializer.HashCodeCachePool.Return(_Cache);
                        _Cache = Array.Empty<int>();
                    }
                    return;
                }
                SerializerException.Wrap(() => this.EnsureValidArgument(nameof(value), CacheOffset, ushort.MaxValue + 1, value));
                if (CacheOffset != 0 && RealCacheSize > 0 && (value > byte.MaxValue ? 2 : 1) != CacheIndexSize)
                    throw new SerializerException("Can't change the cache index size", new InvalidOperationException());
                if (value > RealCacheSize)
                {
                    int[] newCache = StreamSerializer.HashCodeCachePool.RentClean(value);
                    if (CacheOffset > 0) _Cache.AsSpan(0, CacheOffset - 1).CopyTo(newCache);
                    if (_Cache.Length != 0) StreamSerializer.HashCodeCachePool.Return(_Cache);
                    _Cache = newCache;
                }
                _CacheSize = value;
                RealCacheSize = value;
                CacheIndexSize = _CacheSize > byte.MaxValue ? 2 : 1;
            }
        }

        /// <inheritdoc/>
        public bool TryWriteCached<T>(T? obj)
        {
            EnsureUndisposed();
            // Ensure using the cache
            if (!IsCacheEnabled) return false;
            // Handle a NULL value
            if (obj == null)
            {
                Stream.Write((byte)SequenceTypes.Null, this);
                return true;
            }
            // Handle caching
            int ohc = (obj.GetType(), obj).GetHashCode(),
                co = _Cache.IndexOf(ohc);
            if (co == -1)
            {
                // Update the cache
                Stream.Write((byte)SequenceTypes.NotCached, this);
                return WriteNotCached(ohc);
            }
            // Use the cached object
            return WriteCached(co);
        }

        /// <inheritdoc/>
        public async Task<bool> TryWriteCachedAsync<T>(T? obj)
        {
            EnsureUndisposed();
            // Ensure using the cache
            if (!IsCacheEnabled) return false;
            // Handle a NULL value
            if (obj == null)
            {
                await Stream.WriteAsync((byte)SequenceTypes.Null, this).DynamicContext();
                return true;
            }
            // Handle caching
            int ohc = (obj.GetType(), obj).GetHashCode(),
                co = _Cache.IndexOf(ohc);
            if (co == -1)
            {
                // Update the cache
                await Stream.WriteAsync((byte)SequenceTypes.NotCached, this).DynamicContext();
                return WriteNotCached(ohc);
            }
            // Use the cached object
            return await WriteCachedAsync(co).DynamicContext();
        }

        /// <inheritdoc/>
        public bool TryWriteCached<T>(T? obj, ObjectTypes? objType, bool writeType = false)
        {
            EnsureUndisposed();
            // Ensure using the cache
            if (!IsCacheEnabled)
            {
                LastObjectType = null;
                return false;
            }
            // Handle a NULL value
            if (obj == null)
            {
                Stream.Write((byte)SequenceTypes.Null, this);
                LastObjectType = null;
                return true;
            }
            // Ensure having an object type
            Type type;
            bool writeObject;
            if (objType == null)
            {
                (type, objType, _, writeObject) = obj.GetObjectSerializerInfo();
            }
            else
            {
                type = obj.GetType();
                writeObject = objType.Value.RequiresObjectWriting();
            }
            LastObjectType = objType;
            // Handle non-caching environment
            if (!writeObject)
            {
                if (writeType)
                {
                    if (objType.Value.ContainsAnyFlag(ObjectTypes.Cached, ObjectTypes.Unsigned, ObjectTypes.Empty))
                    {
                        Stream.Write((byte)SequenceTypes.NotCached, this);
                        Stream.Write((byte)objType, this);
                    }
                    else
                    {
                        Stream.Write((byte)(objType | ObjectTypes.Empty), this);
                    }
                }
                else
                {
                    Stream.Write((byte)SequenceTypes.NotCached, this);
                }
                return writeType;
            }
            // Handle caching
            int ohc = (type, obj).GetHashCode(),
                co = _Cache.IndexOf(ohc);
            if (co == -1)
            {
                // Update the cache
                if (writeType)
                {
                    if (objType.Value.ContainsAnyFlag(ObjectTypes.Cached, ObjectTypes.Unsigned, ObjectTypes.Empty))
                    {
                        Stream.Write((byte)SequenceTypes.NotCached, this);
                        Stream.Write((byte)objType, this);
                    }
                    else
                    {
                        Stream.Write((byte)(objType | ObjectTypes.Empty), this);
                    }
                }
                else
                {
                    Stream.Write((byte)SequenceTypes.NotCached, this);
                }
                return WriteNotCached(ohc);
            }
            // Use the cached object
            return WriteCached(co);
        }

        /// <inheritdoc/>
        public async Task<bool> TryWriteCachedAsync<T>(T? obj, ObjectTypes? objType, bool writeType = false)
        {
            EnsureUndisposed();
            // Ensure using the cache
            if (!IsCacheEnabled)
            {
                LastObjectType = null;
                return false;
            }
            // Handle a NULL value
            if (obj == null)
            {
                await Stream.WriteAsync((byte)SequenceTypes.Null, this).DynamicContext();
                LastObjectType = null;
                return true;
            }
            // Ensure having an object type
            Type type;
            bool writeObject;
            if (objType == null)
            {
                (type, objType, _, writeObject) = obj.GetObjectSerializerInfo();
            }
            else
            {
                type = obj.GetType();
                writeObject = objType.Value.RequiresObjectWriting();
            }
            LastObjectType = objType;
            // Handle non-caching environment
            if (!writeObject)
            {
                if (writeType)
                {
                    if (objType.Value.ContainsAnyFlag(ObjectTypes.Cached, ObjectTypes.Unsigned, ObjectTypes.Empty))
                    {
                        await Stream.WriteAsync((byte)SequenceTypes.NotCached, this).DynamicContext();
                        await Stream.WriteAsync((byte)objType, this).DynamicContext();
                    }
                    else
                    {
                        await Stream.WriteAsync((byte)(objType | ObjectTypes.Empty), this).DynamicContext();
                    }
                }
                else
                {
                    await Stream.WriteAsync((byte)SequenceTypes.NotCached, this).DynamicContext();
                }
                return writeType;
            }
            // Handle caching
            int ohc = (type, obj).GetHashCode(),
                co = _Cache.IndexOf(ohc);
            if (co == -1)
            {
                // Update the cache
                if (writeType)
                {
                    if (objType.Value.ContainsAnyFlag(ObjectTypes.Cached, ObjectTypes.Unsigned, ObjectTypes.Empty))
                    {
                        await Stream.WriteAsync((byte)SequenceTypes.NotCached, this).DynamicContext();
                        await Stream.WriteAsync((byte)objType, this).DynamicContext();
                    }
                    else
                    {
                        await Stream.WriteAsync((byte)(objType | ObjectTypes.Empty), this).DynamicContext();
                    }
                }
                else
                {
                    await Stream.WriteAsync((byte)SequenceTypes.NotCached, this).DynamicContext();
                }
                return WriteNotCached(ohc);
            }
            // Use the cached object
            return await WriteCachedAsync(co).DynamicContext();
        }

        /// <inheritdoc/>
        public bool TryWriteCachedCountable<T>(T? obj, long? len, bool writeType = false)
        {
            EnsureUndisposed();
            // Ensure using the cache
            if (!IsCacheEnabled)
            {
                LastObjectType = null;
                LastNumberType = null;
                return false;
            }
            // Handle a NULL value
            if (obj == null)
            {
                Stream.Write((byte)SequenceTypes.Null, this);
                LastObjectType = null;
                LastNumberType = null;
                return true;
            }
            // Ensure valid arguments
            SerializerException.Wrap(() =>
            {
                this.EnsureValidArgument(nameof(len), len);
                this.EnsureValidArgument(nameof(len), 0, long.MaxValue, len!.Value);
            });
            // Ensure having an object and a number type
            Type type,
                nType;
            ObjectTypes objType;
            bool writeObject;
            NumberTypes numberType;
            object n;
            (type, objType, _, writeObject) = obj.GetObjectSerializerInfo();
            LastObjectType = objType;
            (n, numberType) = len!.Value.GetNumberAndType();
            nType = n.GetType();
            LastNumberType = numberType;
            // Handle an empty object and non-caching environment
            if (!writeObject || numberType.IsZero())
            {
                if (numberType.ContainsAnyFlag(NumberTypes.MinValue, NumberTypes.MaxValue, NumberTypes.Unsigned))
                {
                    Stream.Write((byte)SequenceTypes.NotCached, this);
                    Stream.Write((byte)numberType, this);
                }
                else
                {
                    Stream.Write((byte)(numberType | NumberTypes.Unsigned), this);
                }
                if (writeType) Stream.Write((byte)objType, this);
                return true;
            }
            // Handle caching
            int ohc = (type, obj).GetHashCode(),
                co = _Cache.IndexOf(ohc);
            if (co == -1)
            {
                // Update the cache
                if (numberType.ContainsAnyFlag(NumberTypes.MinValue, NumberTypes.MaxValue, NumberTypes.Unsigned))
                {
                    Stream.Write((byte)SequenceTypes.NotCached, this);
                    Stream.Write((byte)numberType, this);
                }
                else
                {
                    Stream.Write((byte)(numberType | NumberTypes.Unsigned), this);
                }
                StreamExtensions.WriteNumberInt(this, n, numberType, writeType: false);
                if (writeType) Stream.Write((byte)objType, this);
                return WriteNotCached(ohc);
            }
            // Use the cached object
            return WriteCached(co);
        }

        /// <inheritdoc/>
        public async Task<bool> TryWriteCachedCountableAsync<T>(T? obj, long? len, bool writeType = false)
        {
            EnsureUndisposed();
            // Ensure using the cache
            if (!IsCacheEnabled)
            {
                LastObjectType = null;
                LastNumberType = null;
                return false;
            }
            // Handle a NULL value
            if (obj == null)
            {
                await Stream.WriteAsync((byte)SequenceTypes.Null, this).DynamicContext();
                LastObjectType = null;
                LastNumberType = null;
                return true;
            }
            // Ensure valid arguments
            SerializerException.Wrap(() =>
            {
                this.EnsureValidArgument(nameof(len), len);
                this.EnsureValidArgument(nameof(len), 0, long.MaxValue, len!.Value);
            });
            // Ensure having an object and a number type
            Type type,
                nType;
            ObjectTypes objType;
            bool writeObject;
            NumberTypes numberType;
            object n;
            (type, objType, _, writeObject) = obj.GetObjectSerializerInfo();
            LastObjectType = objType;
            (n, numberType) = len!.Value.GetNumberAndType();
            nType = n.GetType();
            LastNumberType = numberType;
            // Handle an empty object and non-caching environment
            if (!writeObject || numberType.IsZero())
            {
                if (numberType.ContainsAnyFlag(NumberTypes.MinValue, NumberTypes.MaxValue, NumberTypes.Unsigned))
                {
                    await Stream.WriteAsync((byte)SequenceTypes.NotCached, this).DynamicContext();
                    await Stream.WriteAsync((byte)numberType, this).DynamicContext();
                }
                else
                {
                    await Stream.WriteAsync((byte)(numberType | NumberTypes.Unsigned), this).DynamicContext();
                }
                if (writeType) await Stream.WriteAsync((byte)objType, this).DynamicContext();
                return true;
            }
            // Handle caching
            int ohc = (type, obj).GetHashCode(),
                co = _Cache.IndexOf(ohc);
            if (co == -1)
            {
                // Update the cache
                if (numberType.ContainsAnyFlag(NumberTypes.MinValue, NumberTypes.MaxValue, NumberTypes.Unsigned))
                {
                    await Stream.WriteAsync((byte)SequenceTypes.NotCached, this).DynamicContext();
                    await Stream.WriteAsync((byte)numberType, this).DynamicContext();
                }
                else
                {
                    await Stream.WriteAsync((byte)(numberType | NumberTypes.Unsigned), this).DynamicContext();
                }
                await StreamExtensions.WriteNumberIntAsync(this, n, numberType, writeType: false).DynamicContext();
                if (writeType) await Stream.WriteAsync((byte)objType, this).DynamicContext();
                return WriteNotCached(ohc);
            }
            // Use the cached object
            return await WriteCachedAsync(co).DynamicContext();
        }

        /// <inheritdoc/>
        public bool TryWriteCached<T>(T? num, NumberTypes? numberType, bool writeType = false) where T : struct, IConvertible
        {
            EnsureUndisposed();
            // Ensure using the cache
            if (!IsCacheEnabled)
            {
                LastNumberType = null;
                return false;
            }
            // Handle a NULL value
            if (num == null)
            {
                Stream.Write((byte)SequenceTypes.Null, this);
                LastNumberType = null;
                return true;
            }
            // Ensure having a number type
            Type type;
            object n;
            if (numberType == null)
            {
                (n, numberType) = num.GetNumberAndType();
                type = n.GetType();
            }
            else
            {
                n = num;
                type = num.GetType();
            }
            LastNumberType = numberType;
            // Handle non-caching environment
            if (!numberType.Value.RequiresObjectWriting())
            {
                if (writeType)
                {
                    if (numberType.Value.ContainsAnyFlag(NumberTypes.MinValue, NumberTypes.MaxValue, NumberTypes.Unsigned))
                    {
                        Stream.Write((byte)SequenceTypes.NotCached, this);
                        Stream.Write((byte)numberType, this);
                    }
                    else
                    {
                        Stream.Write((byte)(numberType | NumberTypes.Unsigned), this);
                    }
                }
                else
                {
                    Stream.Write((byte)SequenceTypes.NotCached, this);
                }
                return writeType;
            }
            // Handle caching
            int ohc = (type, n).GetHashCode(),
                co = _Cache.IndexOf(ohc);
            if (co == -1)
            {
                // Update the cache
                if (writeType)
                {
                    if (numberType.Value.ContainsAnyFlag(NumberTypes.MinValue, NumberTypes.MaxValue, NumberTypes.Unsigned))
                    {
                        Stream.Write((byte)SequenceTypes.NotCached, this);
                        Stream.Write((byte)numberType, this);
                    }
                    else
                    {
                        Stream.Write((byte)(numberType | NumberTypes.Unsigned), this);
                    }
                }
                else
                {
                    Stream.Write((byte)SequenceTypes.NotCached, this);
                }
                return WriteNotCached(ohc);
            }
            // Use the cached number
            return WriteCached(co);
        }

        /// <inheritdoc/>
        public async Task<bool> TryWriteCachedAsync<T>(T? num, NumberTypes? numberType, bool writeType = false) where T : struct, IConvertible
        {
            EnsureUndisposed();
            // Ensure using the cache
            if (!IsCacheEnabled)
            {
                LastNumberType = null;
                return false;
            }
            // Handle a NULL value
            if (num == null)
            {
                await Stream.WriteAsync((byte)SequenceTypes.Null, this).DynamicContext();
                LastNumberType = null;
                return true;
            }
            // Ensure having a number type
            Type type;
            object n;
            if (numberType == null)
            {
                (n, numberType) = num.GetNumberAndType();
                type = n.GetType();
            }
            else
            {
                n = num;
                type = num.GetType();
            }
            LastNumberType = numberType;
            // Handle non-caching environment
            if (!numberType.Value.RequiresObjectWriting())
            {
                if (writeType)
                {
                    if (numberType.Value.ContainsAnyFlag(NumberTypes.MinValue, NumberTypes.MaxValue, NumberTypes.Unsigned))
                    {
                        await Stream.WriteAsync((byte)SequenceTypes.NotCached, this).DynamicContext();
                        await Stream.WriteAsync((byte)numberType, this).DynamicContext();
                    }
                    else
                    {
                        await Stream.WriteAsync((byte)(numberType | NumberTypes.Unsigned), this).DynamicContext();
                    }
                }
                else
                {
                    await Stream.WriteAsync((byte)SequenceTypes.NotCached, this).DynamicContext();
                }
                return writeType;
            }
            // Handle caching
            int ohc = (type, n).GetHashCode(),
                co = _Cache.IndexOf(ohc);
            if (co == -1)
            {
                // Update the cache
                if (writeType)
                {
                    if (numberType.Value.ContainsAnyFlag(NumberTypes.MinValue, NumberTypes.MaxValue, NumberTypes.Unsigned))
                    {
                        await Stream.WriteAsync((byte)SequenceTypes.NotCached, this).DynamicContext();
                        await Stream.WriteAsync((byte)numberType, this).DynamicContext();
                    }
                    else
                    {
                        await Stream.WriteAsync((byte)(numberType | NumberTypes.Unsigned), this).DynamicContext();
                    }
                }
                else
                {
                    await Stream.WriteAsync((byte)SequenceTypes.NotCached, this).DynamicContext();
                }
                return WriteNotCached(ohc);
            }
            // Use the cached number
            return await WriteCachedAsync(co).DynamicContext();
        }

        /// <summary>
        /// Write cached (will write to the stream!)
        /// </summary>
        /// <param name="index">Cache index</param>
        /// <returns><see langword="true"/></returns>
        protected bool WriteCached(int index)
        {
            EnsureUndisposed();
            SequenceTypes st = SequenceTypes.Cached;
            if (CacheIndexSize != 1 && index <= byte.MaxValue) st |= SequenceTypes.SmallIndex;
            Stream.Write((byte)st, this);
            if (CacheIndexSize == 1 || index <= byte.MaxValue)
            {
                Stream.Write((byte)index, this);
            }
            else
            {
                Stream.Write((ushort)index, this);
            }
            return true;
        }

        /// <summary>
        /// Write cached (will write to the stream!)
        /// </summary>
        /// <param name="index">Cache index</param>
        /// <returns><see langword="true"/></returns>
        protected async Task<bool> WriteCachedAsync(int index)
        {
            EnsureUndisposed();
            SequenceTypes st = SequenceTypes.Cached;
            if (CacheIndexSize != 1 && index <= byte.MaxValue) st |= SequenceTypes.SmallIndex;
            await Stream.WriteAsync((byte)st, this).DynamicContext();
            if (CacheIndexSize == 1 || index <= byte.MaxValue)
            {
                await Stream.WriteAsync((byte)index, this).DynamicContext();
            }
            else
            {
                await Stream.WriteAsync((ushort)index, this).DynamicContext();
            }
            return true;
        }

        /// <summary>
        /// Write not cached (but may store the objects hash code in the cache; will NOT write to the stream)
        /// </summary>
        /// <param name="hashCode">Type and object hash code</param>
        /// <returns><see langword="false"/></returns>
        protected bool WriteNotCached(int hashCode)
        {
            EnsureUndisposed();
            if (CacheOffset < _CacheSize)
            {
                _Cache[CacheOffset] = hashCode;
                CacheOffset++;
            }
            return false;
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (_Cache.Length != 0) StreamSerializer.HashCodeCachePool.Return(_Cache);
        }
    }
}
