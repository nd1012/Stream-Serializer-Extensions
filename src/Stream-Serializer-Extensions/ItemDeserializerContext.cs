using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    /// <summary>
    /// Item deserializer context
    /// </summary>
    public sealed class ItemDeserializerContext : DisposableBase
    {
        /// <summary>
        /// Type cache
        /// </summary>
        private readonly Type[] TypeCache;
        /// <summary>
        /// Object cache
        /// </summary>
        private readonly object[] ObjectCache;
        /// <summary>
        /// Use the object cache?
        /// </summary>
        private readonly bool UseObjectCache;
        /// <summary>
        /// Type cache offset?
        /// </summary>
        private int TypeCacheOffset = 0;
        /// <summary>
        /// Object cache offset?
        /// </summary>
        private int ObjectCacheOffset = 0;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">Deserializer context</param>
        /// <param name="objectCache">Create an object cache?</param>
        public ItemDeserializerContext(IDeserializationContext context, bool objectCache = true) : base()
        {
            Context = context;
            TypeCache = StreamSerializer.TypeCachePool.RentClean(byte.MaxValue);
            ObjectCache = objectCache ? StreamSerializer.ObjectCachePool.RentClean(byte.MaxValue) : Array.Empty<object>();
            UseObjectCache = objectCache;
        }

        /// <summary>
        /// Deserializer context
        /// </summary>
        public IDeserializationContext Context { get; }

        /// <summary>
        /// Object type
        /// </summary>
        public ObjectTypes ObjectType { get; set; }

        /// <summary>
        /// Last object type
        /// </summary>
        public ObjectTypes LastObjectType { get; set; }

        /// <summary>
        /// Item type
        /// </summary>
        public Type? ItemType { get; set; }

        /// <summary>
        /// Item serializer
        /// </summary>
        public SerializerTypes ItemSerializer { get; set; }

        /// <summary>
        /// Synchronous item deserializer
        /// </summary>
        public StreamSerializer.Deserializer_Delegate? ItemSyncDeserializer { get; set; }

        /// <summary>
        /// Asynchronous item deserializer
        /// </summary>
        public StreamSerializer.AsyncDeserializer_Delegate? ItemAsyncDeserializer { get; set; }

        /// <summary>
        /// Nullable?
        /// </summary>
        public bool Nullable { get; set; }

        /// <summary>
        /// Add a type
        /// </summary>
        /// <param name="type">Type</param>
        public void AddType(Type type)
        {
            if (TypeCacheOffset >= byte.MaxValue) return;
            TypeCache[TypeCacheOffset] = type;
            TypeCacheOffset++;
        }

        /// <summary>
        /// Get a cached type
        /// </summary>
        /// <param name="index">Cache index</param>
        /// <returns>Type</returns>
        public Type GetCachedType(int index)
        {
            if (index >= TypeCacheOffset) throw new SerializerException($"Invalid type cache index #{index}", new InvalidDataException());
            return TypeCache[index];
        }

        /// <summary>
        /// Add a type
        /// </summary>
        /// <param name="obj">Object</param>
        public void AddObject<T>(T obj)
        {
            if (!UseObjectCache || ObjectCacheOffset >= byte.MaxValue) return;
            ObjectCache[ObjectCacheOffset] = obj!;
            ObjectCacheOffset++;
        }

        /// <summary>
        /// Get a cached object
        /// </summary>
        /// <param name="index">Cache index</param>
        /// <returns>Object</returns>
        public object GetCachedObject(int index)
        {
            if (!UseObjectCache) throw new SerializerException(message: null, new InvalidOperationException());
            if (index >= ObjectCacheOffset) throw new SerializerException($"Invalid object cache index #{index}", new InvalidDataException());
            return ObjectCache[index];
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            StreamSerializer.TypeCachePool.Return(TypeCache);
            if (UseObjectCache) StreamSerializer.ObjectCachePool.Return(ObjectCache);
        }

        /// <summary>
        /// Cast as object type
        /// </summary>
        /// <param name="context"><see cref="ItemDeserializerContext"/></param>
        public static implicit operator ObjectTypes(ItemDeserializerContext context) => context.ObjectType;

        /// <summary>
        /// Cast as serializer type
        /// </summary>
        /// <param name="context"><see cref="ItemDeserializerContext"/></param>
        public static implicit operator SerializerTypes(ItemDeserializerContext context) => context.ItemSerializer;

        /// <summary>
        /// Cast as item type
        /// </summary>
        /// <param name="context"><see cref="ItemDeserializerContext"/></param>
        public static implicit operator Type?(ItemDeserializerContext context) => context.ItemType;
    }
}
