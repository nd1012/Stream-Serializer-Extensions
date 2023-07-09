using wan24.Core;
using wan24.ObjectValidation;

namespace wan24.StreamSerializerExtensions
{
    /// <summary>
    /// Item serializer context
    /// </summary>
    public sealed class ItemSerializerContext : DisposableBase
    {
        /// <summary>
        /// Cache
        /// </summary>
        private readonly int[] Cache;
        /// <summary>
        /// Using object cache?
        /// </summary>
        private readonly bool UseObjectCache;
        /// <summary>
        /// Type cache offset
        /// </summary>
        private int TypeCacheOffset = 0;
        /// <summary>
        /// Object cache offset
        /// </summary>
        private int ObjectCacheOffset = 0;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">Serialization context</param>
        /// <param name="objectCache">Create an object cache, too?</param>
        public ItemSerializerContext(ISerializationContext context, bool objectCache = true) : base()
        {
            Context = context;
            Cache = objectCache
                ? StreamSerializer.HashCodeCachePool.RentClean(byte.MaxValue << 1)
                : StreamSerializer.HashCodeCachePool.RentClean(byte.MaxValue);
            UseObjectCache = objectCache;
        }

        /// <summary>
        /// Serialization context
        /// </summary>
        public ISerializationContext Context { get; }

        /// <summary>
        /// Last item type
        /// </summary>
        public Type? LastItemType { get; set; }

        /// <summary>
        /// Item serializer
        /// </summary>
        public SerializerTypes ItemSerializer { get; set; }

        /// <summary>
        /// Synchronous item serializer
        /// </summary>
        public StreamSerializer.Serializer_Delegate? ItemSyncSerializer { get; set; }

        /// <summary>
        /// Asynchronous item serializer
        /// </summary>
        public StreamSerializer.AsyncSerializer_Delegate? ItemAsyncSerializer { get; set; }

        /// <summary>
        /// Object type
        /// </summary>
        public ObjectTypes ObjectType { get; set; }

        /// <summary>
        /// Write the object?
        /// </summary>
        public bool WriteObject { get; set; }

        /// <summary>
        /// Nullable?
        /// </summary>
        public bool Nullable { get; set; }

        /// <summary>
        /// Get the cache index of a type
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Cache index or <c>-1</c>, if not cached</returns>
        public int GetTypeCacheIndex(Type type) => Cache.IndexOf(type.GetHashCode());

        /// <summary>
        /// Get the cache index of an object
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="obj">Object</param>
        /// <returns>Cache index or <c>-1</c>, if not cached</returns>
        public int GetObjectCacheIndex<T>(T obj) => Cache.AsSpan(byte.MaxValue).IndexOf((obj, obj!.GetType()).GetHashCode());

        /// <summary>
        /// Add a type to the cache
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Cache index or <c>-1</c>, if not cached</returns>
        public int AddType(Type type)
        {
            if (TypeCacheOffset > byte.MaxValue) return -1;
            int res = TypeCacheOffset;
            Cache[res] = type.GetHashCode();
            TypeCacheOffset++;
            return res;
        }

        /// <summary>
        /// Add an object to the cache
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="obj">Object</param>
        /// <returns>Cache index or <c>-1</c>, if not cached</returns>
        public int AddObject<T>(T obj)
        {
            if (!UseObjectCache || ObjectCacheOffset > byte.MaxValue) return -1;
            int res = ObjectCacheOffset + byte.MaxValue;
            Cache[res] = (obj!.GetType(), obj).GetHashCode();
            ObjectCacheOffset++;
            return res;
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing) => StreamSerializer.HashCodeCachePool.Return(Cache);

        /// <summary>
        /// Cast as last item type
        /// </summary>
        /// <param name="context"><see cref="ItemSerializerContext"/></param>
        public static implicit operator Type?(ItemSerializerContext context) => context.LastItemType;

        /// <summary>
        /// Cast as last item serializer type
        /// </summary>
        /// <param name="context"><see cref="ItemSerializerContext"/></param>
        public static implicit operator SerializerTypes(ItemSerializerContext context) => context.ItemSerializer;

        /// <summary>
        /// Cast as object type
        /// </summary>
        /// <param name="context"><see cref="ItemSerializerContext"/></param>
        public static implicit operator ObjectTypes(ItemSerializerContext context) => context.ObjectType;

        /// <summary>
        /// Cast as write object flag
        /// </summary>
        /// <param name="context"><see cref="ItemSerializerContext"/></param>
        public static implicit operator bool(ItemSerializerContext context) => context.WriteObject;

        /// <summary>
        /// Cast as stream
        /// </summary>
        /// <param name="context"><see cref="ItemSerializerContext"/></param>
        public static implicit operator Stream(ItemSerializerContext context) => context.Context.Stream;

        /// <summary>
        /// Cast as cancellation token
        /// </summary>
        /// <param name="context"><see cref="ItemSerializerContext"/></param>
        public static implicit operator CancellationToken(ItemSerializerContext context) => context.Context.Cancellation;
    }
}
