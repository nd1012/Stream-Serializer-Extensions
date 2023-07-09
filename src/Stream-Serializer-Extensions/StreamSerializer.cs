using System.Buffers;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime;
using System.Runtime.CompilerServices;
using wan24.Core;

//TODO Test type cache
//TODO Use SerializerContext
//TODO SerializerStream which manages a SerializerContext and adopts to extension methods to get rid of the SerializerContext parameter
//TODO Use ItemSerializerContext
//TODO ReadUntil for reading until any byte sequence
//TODO WriteAll/ReadAll for writing/reading a type sequence
//TODO WriteJson/ReadJson for writing/reading JSON encoded data
//TODO WriteAnonymous/ReadAnonymous for writing/reading anonymous objects (reading by interface or class)
//TODO CSV parsing

namespace wan24.StreamSerializerExtensions
{
    /// <summary>
    /// Serializer
    /// </summary>
    public static partial class StreamSerializer
    {
        /// <summary>
        /// Initialized?
        /// </summary>
        private static bool Initialized = false;

        /// <summary>
        /// An object for thread locking
        /// </summary>
        public static readonly object SyncObject = new();
        /// <summary>
        /// Allowed (non-array) types
        /// </summary>
        public static readonly ConcurrentBag<Type> AllowedTypes;
        /// <summary>
        /// Type instance factories
        /// </summary>
        public static readonly ConcurrentDictionary<Type, InstanceFactory_Delegate> InstanceFactories = new();

        /// <summary>
        /// Byte array buffer pool to use
        /// </summary>
        public static ArrayPool<byte> BufferPool { get; set; } = ArrayPool<byte>.Shared;

        /// <summary>
        /// Object cache pool to use
        /// </summary>
        public static ArrayPool<object> ObjectCachePool { get; set; } = ArrayPool<object>.Shared;

        /// <summary>
        /// Type cache pool to use
        /// </summary>
        public static ArrayPool<Type> TypeCachePool { get; set; } = ArrayPool<Type>.Shared;

        /// <summary>
        /// Object hash code cache pool to use
        /// </summary>
        public static ArrayPool<int> HashCodeCachePool { get; set; } = ArrayPool<int>.Shared;

        /// <summary>
        /// Is the type cache enabled?
        /// </summary>
        public static bool TypeCacheEnabled { get; private set; }

        /// <summary>
        /// Load a type
        /// </summary>
        /// <param name="name">Type name</param>
        /// <returns>Type</returns>
        public static Type LoadType(string name)
            => SerializerException.Wrap(() =>
            {
                if ((Type.GetType(name, throwOnError: false) ?? TypeHelper.Instance.GetType(name)) is Type type && IsTypeAllowed(type)) return type;
                TypeLoadEventArgs e = new(name);
                OnLoadType?.Invoke(e);
                if (e.Type == null) throw new SerializerException($"Failed to load type \"{name}\"");
                return e.Type;
            }, $"Failed to load type \"{name}\"");

        /// <summary>
        /// Determine if a type is allowed for deserializing per default
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Is allowed?</returns>
        public static bool IsTypeAllowed(Type type)
        {
            // Deny abstract types
            if (type.IsAbstract || type.IsInterface) return false;
            // Allow all enumeration types
            if (type.IsEnum) return true;
            // Get the final element type of an array type
            Type finalType = type.IsArray ? type.GetFinalElementType() : type;
            // Allow registered allowed types or serializable types
            if (
                AllowedTypes.Contains(finalType) || 
                typeof(IStreamSerializer).IsAssignableFrom(finalType) || 
                finalType.GetCustomAttributeCached<StreamSerializerAttribute>() != null
                )
                return true;
            // Validate generic type arguments also (btw. no risk for an endless recursion here)
            if (finalType.IsGenericType) foreach (Type gta in finalType.GetGenericArguments()) if (!IsTypeAllowed(gta)) return false;
            // Allow inheriting types
            if (AllowedTypes.Any(t => t.IsAssignableFrom(finalType))) return true;
            // Deny
            return false;
        }

        /// <summary>
        /// Create an instance
        /// </summary>
        /// <param name="usedConstructor">Used constructor</param>
        /// <param name="type">Requested type</param>
        /// <param name="context">Context</param>
        /// <returns>Instance</returns>
        public static object CreateInstance(out ConstructorInfo? usedConstructor, Type type, IDeserializationContext context)
        {
            usedConstructor = null;
            // Try factories
            if (InstanceFactories.TryGetValue(type, out InstanceFactory_Delegate? factory)) return factory(type, context);
            if (type.IsGenericType && InstanceFactories.TryGetValue(type.GetGenericTypeDefinition(), out factory))
                return factory(type, context);
            if (
                InstanceFactories.Keys.FirstOrDefault(t => t.IsAssignableFrom(type)) is Type inheritedType &&
                InstanceFactories.TryGetValue(inheritedType, out factory)
                )
                return factory(type, context);
            // Try reflection
            if (typeof(IStreamSerializer).IsAssignableFrom(type))
            {
                ConstructorInfo? ci = (from c in type.GetConstructorsCached()
                                       where c.GetParametersCached().Length == 0 ||
                                           c.IsSerializerConstructor()
                                       orderby c.GetCustomAttributeCached<StreamSerializerAttribute>() is not null descending
                                       orderby c.GetParametersCached().Length descending
                                       select c)
                                      .FirstOrDefault();
                if (ci != null)
                {
                    // Use the found (possibly) serializer constructor
                    usedConstructor = ci;
                    return ci.GetParametersCached().Length == 0 ? ci.InvokeAuto() : ci.InvokeAuto(context);
                }
            }
            // Try automatic construction from an unknown constructor
            return type.ConstructAuto(out usedConstructor, usePrivate: true, context)
                ?? throw new SerializerException($"Failed to instance {type}", new InvalidProgramException());
        }

        /// <summary>
        /// Create an instance
        /// </summary>
        /// <typeparam name="T">Requested type</typeparam>
        /// <param name="usedConstructor">Used constructor</param>
        /// <param name="context">Context</param>
        /// <returns>Instance</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T CreateInstance<T>(out ConstructorInfo? usedConstructor, IDeserializationContext context)
            => (T)CreateInstance(out usedConstructor, typeof(T), context);

        /// <summary>
        /// Enable the type cache (CAUTION: Calling this method will ensure that the <c>wan24-Core</c> bootstrapper did run! A call during bootstrapping will cause an exception)
        /// </summary>
        /// <exception cref="InvalidOperationException">Can't be called during bootstrapping</exception>
        public static void EnableTypeCache()
        {
            if (TypeCacheEnabled) return;
            TypeCacheEnabled = true;
            // Ensure the bootstrapper did run
            if (!Bootstrap.DidBoot)
            {
                if (Bootstrap.IsBooting) throw new InvalidOperationException("Can't be called during bootstrapping");
                Bootstrap.Async().Wait();
            }
            // Will the type cache with IStreamSerializer types
            Type streamSerializer = typeof(IStreamSerializer);
            foreach (Type type in from ass in TypeHelper.Instance.Assemblies
                                  from t in ass.GetTypes()
                                  where !t.IsInterface &&
                                     !t.IsAbstract &&
                                     streamSerializer.IsAssignableFrom(t) && 
                                     !TypeCache.Types.ContainsKey(t.GetHashCode())
                                  select t)
                TypeCache.Types.TryAdd(type.GetHashCode(), type);
        }

        /// <summary>
        /// Serializer delegate
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="obj">Object</param>
        public delegate void Serializer_Delegate(ISerializationContext context, object? obj);

        /// <summary>
        /// Serializer delegate
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="obj">Object</param>
        public delegate Task AsyncSerializer_Delegate(ISerializationContext context, object? obj);

        /// <summary>
        /// Deserializer delegate
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="type">Type</param>
        /// <returns>Object</returns>
        public delegate object? Deserializer_Delegate(IDeserializationContext context, Type type);

        /// <summary>
        /// Deserializer delegate
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="type">Type</param>
        /// <returns>Object (the task will be used as <see cref="Task{TResult}"/>)</returns>
        public delegate Task AsyncDeserializer_Delegate(IDeserializationContext context, Type type);

        /// <summary>
        /// Delegate for an instance factory
        /// </summary>
        /// <param name="type">Requested type</param>
        /// <param name="context">Context</param>
        /// <returns>Instance</returns>
        public delegate object InstanceFactory_Delegate(Type type, IDeserializationContext context);

        /// <summary>
        /// Delegate for finding a serializer
        /// </summary>
        /// <param name="e">Event arguments</param>
        public delegate void FindSyncSerializer_Delegate(SerializerEventArgs<Serializer_Delegate> e);
        /// <summary>
        /// Raised when finding a serializer
        /// </summary>
        public static event FindSyncSerializer_Delegate? OnFindSyncSerializer;

        /// <summary>
        /// Delegate for finding a serializer
        /// </summary>
        /// <param name="e">Event arguments</param>
        public delegate void FindAsyncSerializer_Delegate(SerializerEventArgs<AsyncSerializer_Delegate> e);
        /// <summary>
        /// Raised when finding a serializer
        /// </summary>
        public static event FindAsyncSerializer_Delegate? OnFindAsyncSerializer;

        /// <summary>
        /// Delegate for finding a deserializer
        /// </summary>
        /// <param name="e">Event arguments</param>
        public delegate void FindSyncDeserializer_Delegate(SerializerEventArgs<Deserializer_Delegate> e);
        /// <summary>
        /// Raised when finding a deserializer
        /// </summary>
        public static event FindSyncDeserializer_Delegate? OnFindSyncDeserializer;

        /// <summary>
        /// Delegate for finding a deserializer
        /// </summary>
        /// <param name="e">Event arguments</param>
        public delegate void FindAsyncDeserializer_Delegate(SerializerEventArgs<AsyncDeserializer_Delegate> e);
        /// <summary>
        /// Raised when finding a deserializer
        /// </summary>
        public static event FindAsyncDeserializer_Delegate? OnFindAsyncDeserializer;

        /// <summary>
        /// Delegate for the <see cref="OnLoadType"/> event
        /// </summary>
        /// <param name="e">Event arguments</param>
        public delegate void LoadType_Delegate(TypeLoadEventArgs e);
        /// <summary>
        /// Raised on loading a type
        /// </summary>
        public static event LoadType_Delegate? OnLoadType;

        /// <summary>
        /// Delegate for the <see cref="OnInit"/> event
        /// </summary>
        /// <param name="e">Event arguments</param>
        public delegate void Init_Delegate(EventArgs e);
        /// <summary>
        /// Raised on initialization
        /// </summary>
        public static event Init_Delegate? OnInit;
    }
}
