using System.Buffers;
using System.Collections;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime;
using wan24.Core;

//TODO char (de)serializer
//TODO Use ArgumentValidationHelper methods

namespace wan24.StreamSerializerExtensions
{
    /// <summary>
    /// Serializer
    /// </summary>
    public static class StreamSerializer
    {
        /// <summary>
        /// Version number
        /// </summary>
        public const int VERSION = 3;

        /// <summary>
        /// Initialized?
        /// </summary>
        private static bool Initialized = false;

        /// <summary>
        /// An object for thread locking
        /// </summary>
        public static readonly object SyncObject = new();
        /// <summary>
        /// Serializer
        /// </summary>
        public static readonly ConcurrentDictionary<Type, Serialize_Delegate> SyncSerializer;
        /// <summary>
        /// Serializer
        /// </summary>
        public static readonly ConcurrentDictionary<Type, AsyncSerialize_Delegate> AsyncSerializer;
        /// <summary>
        /// Deserializer
        /// </summary>
        public static readonly ConcurrentDictionary<Type, Deserialize_Delegate> SyncDeserializer;
        /// <summary>
        /// Deserializer
        /// </summary>
        public static readonly ConcurrentDictionary<Type, AsyncDeserialize_Delegate> AsyncDeserializer;
        /// <summary>
        /// Allowed (non-array) types
        /// </summary>
        public static readonly ConcurrentBag<Type> AllowedTypes;
        /// <summary>
        /// Type instance factories
        /// </summary>
        public static readonly ConcurrentDictionary<Type, InstanceFactory_Delegate> InstanceFactories = new();

        /// <summary>
        /// Constructor
        /// </summary>
        static StreamSerializer()
        {
            SyncSerializer = new(
                new KeyValuePair<Type, Serialize_Delegate>[]
                {
                    new KeyValuePair<Type, Serialize_Delegate>(typeof(bool),(s, v) => s.Write((bool)SerializerHelper.EnsureNotNull(v))),
                    new KeyValuePair<Type, Serialize_Delegate>(typeof(sbyte),(s, v) => s.Write((sbyte)SerializerHelper.EnsureNotNull(v))),
                    new KeyValuePair<Type, Serialize_Delegate>(typeof(byte),(s, v) => s.Write((byte)SerializerHelper.EnsureNotNull(v))),
                    new KeyValuePair<Type, Serialize_Delegate>(typeof(short),(s, v) => s.Write((int)(short)SerializerHelper.EnsureNotNull(v))),
                    new KeyValuePair<Type, Serialize_Delegate>(typeof(ushort),(s, v) => s.Write((ushort)SerializerHelper.EnsureNotNull(v))),
                    new KeyValuePair<Type, Serialize_Delegate>(typeof(int),(s, v) => s.Write((int)SerializerHelper.EnsureNotNull(v))),
                    new KeyValuePair<Type, Serialize_Delegate>(typeof(uint),(s, v) => s.Write((uint)SerializerHelper.EnsureNotNull(v))),
                    new KeyValuePair<Type, Serialize_Delegate>(typeof(long),(s, v) => s.Write((long)SerializerHelper.EnsureNotNull(v))),
                    new KeyValuePair<Type, Serialize_Delegate>(typeof(ulong),(s, v) => s.Write((ulong)SerializerHelper.EnsureNotNull(v))),
                    new KeyValuePair<Type, Serialize_Delegate>(typeof(float),(s, v) => s.Write((float)SerializerHelper.EnsureNotNull(v))),
                    new KeyValuePair<Type, Serialize_Delegate>(typeof(double),(s, v) => s.Write((double)SerializerHelper.EnsureNotNull(v))),
                    new KeyValuePair<Type, Serialize_Delegate>(typeof(decimal),(s, v) => s.Write((decimal)SerializerHelper.EnsureNotNull(v))),
                    new KeyValuePair<Type, Serialize_Delegate>(typeof(byte[]),(s, v) => s.WriteBytes((byte[])SerializerHelper.EnsureNotNull(v))),
                    new KeyValuePair<Type, Serialize_Delegate>(typeof(string),(s, v) => s.WriteString((string)SerializerHelper.EnsureNotNull(v))),
                    new KeyValuePair<Type, Serialize_Delegate>(typeof(Array),(s, v) => s.WriteArray((Array)SerializerHelper.EnsureNotNull(v))),
                    new KeyValuePair<Type, Serialize_Delegate>(typeof(IList),(s, v) => s.WriteList((IList)SerializerHelper.EnsureNotNull(v))),
                    new KeyValuePair<Type, Serialize_Delegate>(typeof(IDictionary),(s, v) => s.WriteDict((IDictionary)SerializerHelper.EnsureNotNull(v))),
                    new KeyValuePair<Type, Serialize_Delegate>(typeof(Enum),(s, v) => s.WriteEnum((Enum)SerializerHelper.EnsureNotNull(v))),
                    new KeyValuePair<Type, Serialize_Delegate>(typeof(Stream),(s, v) => s.WriteStream((Stream)SerializerHelper.EnsureNotNull(v)))
                }
            );
            AsyncSerializer = new(
                new KeyValuePair<Type, AsyncSerialize_Delegate>[]
                {
                    new KeyValuePair<Type, AsyncSerialize_Delegate>(typeof(bool),(s, v, ct) => s.WriteAsync((bool)SerializerHelper.EnsureNotNull(v), ct)),
                    new KeyValuePair<Type, AsyncSerialize_Delegate>(typeof(sbyte),(s, v, ct) => s.WriteAsync((sbyte)SerializerHelper.EnsureNotNull(v), ct)),
                    new KeyValuePair<Type, AsyncSerialize_Delegate>(typeof(byte),(s, v, ct) => s.WriteAsync((byte)SerializerHelper.EnsureNotNull(v), ct)),
                    new KeyValuePair<Type, AsyncSerialize_Delegate>(typeof(short),(s, v, ct) => s.WriteAsync((short)SerializerHelper.EnsureNotNull(v), ct)),
                    new KeyValuePair<Type, AsyncSerialize_Delegate>(typeof(ushort),(s, v, ct) => s.WriteAsync((ushort)SerializerHelper.EnsureNotNull(v), ct)),
                    new KeyValuePair<Type, AsyncSerialize_Delegate>(typeof(int),(s, v, ct) => s.WriteAsync((int)SerializerHelper.EnsureNotNull(v), ct)),
                    new KeyValuePair<Type, AsyncSerialize_Delegate>(typeof(uint),(s, v, ct) => s.WriteAsync((uint)SerializerHelper.EnsureNotNull(v), ct)),
                    new KeyValuePair<Type, AsyncSerialize_Delegate>(typeof(long),(s, v, ct) => s.WriteAsync((long)SerializerHelper.EnsureNotNull(v), ct)),
                    new KeyValuePair<Type, AsyncSerialize_Delegate>(typeof(ulong),(s, v, ct) => s.WriteAsync((ulong)SerializerHelper.EnsureNotNull(v), ct)),
                    new KeyValuePair<Type, AsyncSerialize_Delegate>(typeof(float),(s, v, ct) => s.WriteAsync((float)SerializerHelper.EnsureNotNull(v), ct)),
                    new KeyValuePair<Type, AsyncSerialize_Delegate>(typeof(double),(s, v, ct) => s.WriteAsync((double)SerializerHelper.EnsureNotNull(v), ct)),
                    new KeyValuePair<Type, AsyncSerialize_Delegate>(typeof(decimal),(s, v, ct) => s.WriteAsync((decimal)SerializerHelper.EnsureNotNull(v), ct)),
                    new KeyValuePair<Type, AsyncSerialize_Delegate>(typeof(byte[]),(s, v, ct) => s.WriteBytesAsync((byte[])SerializerHelper.EnsureNotNull(v), ct)),
                    new KeyValuePair<Type, AsyncSerialize_Delegate>(typeof(string),(s, v, ct) => s.WriteStringAsync((string)SerializerHelper.EnsureNotNull(v), ct)),
                    new KeyValuePair<Type, AsyncSerialize_Delegate>(typeof(Array),(s, v, ct) => s.WriteArrayAsync((Array)SerializerHelper.EnsureNotNull(v), ct)),
                    new KeyValuePair<Type, AsyncSerialize_Delegate>(typeof(IList),(s, v, ct) => s.WriteListAsync((IList)SerializerHelper.EnsureNotNull(v), ct)),
                    new KeyValuePair<Type, AsyncSerialize_Delegate>(typeof(IDictionary),(s, v, ct) => s.WriteDictAsync((IDictionary)SerializerHelper.EnsureNotNull(v), ct)),
                    new KeyValuePair<Type, AsyncSerialize_Delegate>(typeof(Enum),(s, v, ct) => s.WriteEnumAsync((Enum)SerializerHelper.EnsureNotNull(v), ct)),
                    new KeyValuePair<Type, AsyncSerialize_Delegate>(typeof(Stream),(s, v, ct) => s.WriteStreamAsync((Stream)SerializerHelper.EnsureNotNull(v), cancellationToken: ct))
                }
            );
            SyncDeserializer = new(new KeyValuePair<Type, Deserialize_Delegate>[]
            {
                    new KeyValuePair<Type, Deserialize_Delegate>(typeof(bool),(s,t,v,o) => s.ReadBool(v)),
                    new KeyValuePair<Type, Deserialize_Delegate>(typeof(sbyte),(s,t,v,o) => s.ReadOneSByte(v)),
                    new KeyValuePair<Type, Deserialize_Delegate>(typeof(byte),(s,t,v,o) => s.ReadOneByte(v)),
                    new KeyValuePair<Type, Deserialize_Delegate>(typeof(short),(s,t,v,o) => s.ReadShort(v)),
                    new KeyValuePair<Type, Deserialize_Delegate>(typeof(ushort),(s,t,v,o) => s.ReadUShort(v)),
                    new KeyValuePair<Type, Deserialize_Delegate>(typeof(int),(s,t,v,o) => s.ReadInt(v)),
                    new KeyValuePair<Type, Deserialize_Delegate>(typeof(uint),(s,t,v,o) => s.ReadUInt(v)),
                    new KeyValuePair<Type, Deserialize_Delegate>(typeof(long),(s,t,v,o) => s.ReadLong(v)),
                    new KeyValuePair<Type, Deserialize_Delegate>(typeof(ulong),(s,t,v,o) => s.ReadULong(v)),
                    new KeyValuePair<Type, Deserialize_Delegate>(typeof(float),(s,t,v,o) => s.ReadFloat(v)),
                    new KeyValuePair<Type, Deserialize_Delegate>(typeof(double),(s,t,v,o) => s.ReadDouble(v)),
                    new KeyValuePair<Type, Deserialize_Delegate>(typeof(decimal),(s,t,v,o) => s.ReadDecimal(v)),
                    new KeyValuePair<Type, Deserialize_Delegate>(typeof(byte[]),(s,t,v,o) =>
                        s.ReadBytes(v, minLen: o?.GetMinLen(0)??0, maxLen: o?.GetMaxLen(int.MaxValue)??int.MaxValue)
                        ),
                    new KeyValuePair<Type, Deserialize_Delegate>(typeof(string),(s,t,v,o) =>
                        s.ReadString(v, minLen: o?.GetMinLen(0)??0, maxLen: o?.GetMaxLen(int.MaxValue)??int.MaxValue)
                        ),
                    new KeyValuePair<Type, Deserialize_Delegate>(typeof(Array),(s,t,v,o) =>
                        s.ReadArray(
                            t,
                            v,
                            null,
                            o?.GetMinLen(0)??0,
                            o?.GetMaxLen(int.MaxValue)??int.MaxValue,
                            o?.Attribute.GetValueSerializerOptions(property: null, s, v)
                            )
                    ),
                    new KeyValuePair<Type, Deserialize_Delegate>(typeof(IList),(s,t,v,o) =>
                        s.ReadList(
                            t,
                            v,
                            null,
                            o?.GetMinLen(0)??0,
                            o?.GetMaxLen(int.MaxValue)??int.MaxValue,
                            o?.Attribute.GetValueSerializerOptions(property: null, s, v)
                            )
                    ),
                    new KeyValuePair<Type, Deserialize_Delegate>(typeof(IDictionary),(s,t,v,o) =>
                        s.ReadDict(
                            t,
                            v,
                            null,
                            o?.GetMinLen(0)??0,
                            o?.GetMaxLen(int.MaxValue)??int.MaxValue,
                            o?.Attribute.GetKeySerializerOptions(property: null, s, v),
                            o?.Attribute.GetValueSerializerOptions(property: null, s, v)
                            )
                    ),
                    new KeyValuePair<Type, Deserialize_Delegate>(typeof(Enum),(s,t,v,o) => s.ReadEnum(t,v)),
                    new KeyValuePair<Type, Deserialize_Delegate>(typeof(Stream),(s,t,v,o) =>
                    {
                        Stream res = o?.Attribute.GetStream(obj:null,property:null,s,v)?? new FileStream(
                            Path.GetTempFileName(),
                            FileMode.OpenOrCreate,
                            FileAccess.ReadWrite,
                            FileShare.None,
                            bufferSize: Settings.BufferSize,
                            FileOptions.RandomAccess | FileOptions.DeleteOnClose
                            );
                        try
                        {
                            return s.ReadStream(res,v,minLen:o?.GetMinLen(0L)??0, maxLen:o?.GetMaxLen(long.MaxValue)??long.MaxValue);
                        }
                        catch
                        {
                            res.Dispose();
                            throw;
                        }
                    })
            });
            AsyncDeserializer = new(new KeyValuePair<Type, AsyncDeserialize_Delegate>[]
            {
                    new KeyValuePair<Type, AsyncDeserialize_Delegate>(typeof(bool),(s,t,v,o,ct) => s.ReadBoolAsync(v, cancellationToken: ct)),
                    new KeyValuePair<Type, AsyncDeserialize_Delegate>(typeof(sbyte),(s,t,v,o,ct) => s.ReadOneSByteAsync(v, ct)),
                    new KeyValuePair<Type, AsyncDeserialize_Delegate>(typeof(byte),(s,t,v,o,ct) => s.ReadOneByteAsync(v, ct)),
                    new KeyValuePair<Type, AsyncDeserialize_Delegate>(typeof(short),(s,t,v,o,ct) => s.ReadShortAsync(v, cancellationToken: ct)),
                    new KeyValuePair<Type, AsyncDeserialize_Delegate>(typeof(ushort),(s,t,v,o,ct) => s.ReadUShortAsync(v, cancellationToken: ct)),
                    new KeyValuePair<Type, AsyncDeserialize_Delegate>(typeof(int),(s,t,v,o,ct) => s.ReadIntAsync(v, cancellationToken: ct)),
                    new KeyValuePair<Type, AsyncDeserialize_Delegate>(typeof(uint),(s,t,v,o,ct) => s.ReadUIntAsync(v, cancellationToken: ct)),
                    new KeyValuePair<Type, AsyncDeserialize_Delegate>(typeof(long),(s,t,v,o,ct) => s.ReadLongAsync(v, cancellationToken: ct)),
                    new KeyValuePair<Type, AsyncDeserialize_Delegate>(typeof(ulong),(s,t,v,o,ct) => s.ReadULongAsync(v, cancellationToken: ct)),
                    new KeyValuePair<Type, AsyncDeserialize_Delegate>(typeof(float),(s,t,v,o,ct) => s.ReadFloatAsync(v, cancellationToken: ct)),
                    new KeyValuePair<Type, AsyncDeserialize_Delegate>(typeof(double),(s,t,v,o,ct) => s.ReadDoubleAsync(v, cancellationToken: ct)),
                    new KeyValuePair<Type, AsyncDeserialize_Delegate>(typeof(decimal),(s,t,v,o,ct) => s.ReadDecimalAsync(v, cancellationToken: ct)),
                    new KeyValuePair<Type, AsyncDeserialize_Delegate>(typeof(byte[]),(s,t,v,o,ct) => 
                        s.ReadBytesAsync(v, minLen: o?.GetMinLen(0)??0, maxLen: o?.GetMaxLen(int.MaxValue)??int.MaxValue, cancellationToken: ct)
                        ),
                    new KeyValuePair<Type, AsyncDeserialize_Delegate>(typeof(string),(s,t,v,o,ct) => 
                        s.ReadStringAsync(v, minLen: o?.GetMinLen(0)??0, maxLen: o?.GetMaxLen(int.MaxValue)??int.MaxValue, cancellationToken: ct)
                        ),
                    new KeyValuePair<Type, AsyncDeserialize_Delegate>(typeof(Array),(s,t,v,o,ct) =>
                        s.ReadArrayAsync(
                            t,
                            v,
                            null,
                            o?.GetMinLen(0)??0,
                            o?.GetMaxLen(int.MaxValue)??int.MaxValue,
                            o?.Attribute.GetValueSerializerOptions(property: null, s, v, ct),
                            ct
                            )
                    ),
                    new KeyValuePair<Type, AsyncDeserialize_Delegate>(typeof(IList),(s,t,v,o,ct) =>
                        s.ReadListAsync(
                            t,
                            v,
                            null,
                            o?.GetMinLen(0)??0,
                            o?.GetMaxLen(int.MaxValue)??int.MaxValue,
                            o?.Attribute.GetValueSerializerOptions(property: null, s, v, ct),
                            ct
                            )
                    ),
                    new KeyValuePair<Type, AsyncDeserialize_Delegate>(typeof(IDictionary),(s,t,v,o,ct) =>
                        s.ReadDictAsync(
                            t,
                            v,
                            null,
                            o?.GetMinLen(0)??0,
                            o?.GetMaxLen(int.MaxValue)??int.MaxValue,
                            o?.Attribute.GetKeySerializerOptions(property: null, s, v, ct),
                            o?.Attribute.GetValueSerializerOptions(property: null, s, v, ct),
                            ct
                            )
                        ),
                    new KeyValuePair<Type, AsyncDeserialize_Delegate>(typeof(Enum),(s,t,v,o,ct) => s.ReadEnumAsync(t,v,cancellationToken: ct)),
                    new KeyValuePair<Type, AsyncDeserialize_Delegate>(typeof(Stream),(s,t,v,o,ct) =>
                    {
                        //TODO Should be asynchronous
                        Stream res = o?.Attribute.GetStream(obj:null,property:null,s,v,ct)?? new FileStream(
                            Path.GetTempFileName(),
                            FileMode.OpenOrCreate,
                            FileAccess.ReadWrite,
                            FileShare.None,
                            bufferSize: Settings.BufferSize,
                            FileOptions.RandomAccess | FileOptions.DeleteOnClose
                            );
                        try
                        {
                            return s.ReadStreamAsync(res,v,minLen:o?.GetMinLen(0L)??0, maxLen:o?.GetMaxLen(long.MaxValue)??long.MaxValue, cancellationToken: ct);
                        }
                        catch
                        {
                            res.Dispose();
                            throw;
                        }
                    })
            });
            AllowedTypes = new(new Type[]
            {
                typeof(bool),
                typeof(sbyte),
                typeof(byte),
                typeof(short),
                typeof(ushort),
                typeof(int),
                typeof(uint),
                typeof(long),
                typeof(ulong),
                typeof(float),
                typeof(double),
                typeof(decimal),
                typeof(string),
                typeof(char),
                typeof(IDictionary),
                typeof(IList),
                typeof(Stream)
            });
        }

        /// <summary>
        /// Version number (the first 8 bits are used internal, while other bits can be used for customization)
        /// </summary>
        public static int Version { get; set; } = VERSION;

        /// <summary>
        /// Byte array buffer pool to use
        /// </summary>
        public static ArrayPool<byte> BufferPool { get; set; } = ArrayPool<byte>.Shared;

        /// <summary>
        /// Find a serializer
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Serializer</returns>
        public static Serialize_Delegate? FindSerializer(Type type)
        {
            Type? serializer = FindDelegateType(type, SyncSerializer.Keys);
            if (serializer == null || !SyncSerializer.TryGetValue(serializer, out Serialize_Delegate? res))
            {
                SerializerEventArgs<Serialize_Delegate> e = new(type);
                OnFindSyncSerializer?.Invoke(e);
                res = e.Delegate;
            }
            return res;
        }

        /// <summary>
        /// Find a serializer
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Serializer</returns>
        public static AsyncSerialize_Delegate? FindAsyncSerializer(Type type)
        {
            Type? serializer = FindDelegateType(type, AsyncSerializer.Keys);
            if (serializer == null || !AsyncSerializer.TryGetValue(serializer, out AsyncSerialize_Delegate? res))
            {
                SerializerEventArgs<AsyncSerialize_Delegate> e = new(type);
                OnFindAsyncSerializer?.Invoke(e);
                res = e.Delegate;
            }
            return res;
        }

        /// <summary>
        /// Find a deserializer
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Deserializer</returns>
        public static Deserialize_Delegate? FindDeserializer(Type type)
        {
            Type? serializer = FindDelegateType(type, SyncSerializer.Keys);
            if (serializer == null || !SyncDeserializer.TryGetValue(serializer, out Deserialize_Delegate? res))
            {
                SerializerEventArgs<Deserialize_Delegate> e = new(type);
                OnFindSyncDeserializer?.Invoke(e);
                res = e.Delegate;
            }
            return res;
        }

        /// <summary>
        /// Find a deserializer
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Deserializer</returns>
        public static AsyncDeserialize_Delegate? FindAsyncDeserializer(Type type)
        {
            Type? serializer = FindDelegateType(type, AsyncSerializer.Keys);
            if (serializer == null || !AsyncDeserializer.TryGetValue(serializer, out AsyncDeserialize_Delegate? res))
            {
                SerializerEventArgs<AsyncDeserialize_Delegate> e = new(type);
                OnFindAsyncDeserializer?.Invoke(e);
                res = e.Delegate;
            }
            return res;
        }

        /// <summary>
        /// Find a delegate type
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="types">Types</param>
        /// <returns>Delegate type</returns>
        public static Type? FindDelegateType(Type type, IEnumerable<Type> types)
        {
            lock (SyncObject)
                if (!Initialized)
                {
                    Initialized = true;
                    OnInit?.Invoke(new());
                }
            Type? res = types.FirstOrDefault(t => t == type);
            if (res == null)
                if (type.IsGenericType && !type.IsGenericTypeDefinition)
                {
                    Type gtd = type.GetGenericTypeDefinition();
                    res = types.FirstOrDefault(t => t.IsGenericType && (t.IsGenericTypeDefinition ? t : t.GetGenericTypeDefinition()) == gtd);
                }
                else if (type.IsArray)
                    res = types.FirstOrDefault(t => t == typeof(Array));
                else if (type.IsEnum)
                    res = types.FirstOrDefault(t => t == typeof(Enum));
            return res ?? types.FirstOrDefault(t => t.IsAssignableFrom(type));
        }

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
        /// Determine if a type is allowed per default for deserializing
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Is allowed?</returns>
        public static bool IsTypeAllowed(Type type)
        {
            // Allow all enumeration types
            if (type.IsEnum) return true;
            // Get the final element type of an array type
            Type finalType = type;
            while (finalType.IsArray) finalType = finalType.GetElementType()!;
            // Allow registered allowed types or serializable types
            if (AllowedTypes.Contains(finalType) || typeof(IStreamSerializer).IsAssignableFrom(finalType)) return true;
            // Validate generic type arguments also (btw. no risk for an endless recursion here)
            if (finalType.IsGenericType) foreach (Type gta in finalType.GetGenericArguments()) if (IsTypeAllowed(gta)) return false;
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
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="options">Serializer options</param>
        /// <returns>Instance</returns>
        public static object CreateInstance(out ConstructorInfo? usedConstructor, Type type, Stream stream, int? version = null, ISerializerOptions? options = null)
        {
            usedConstructor = null;
            if (InstanceFactories.TryGetValue(type, out InstanceFactory_Delegate? factory)) return factory(type, stream, version ?? VERSION, options);
            if (type.IsGenericType && InstanceFactories.TryGetValue(type.GetGenericTypeDefinition(), out factory))
                return factory(type, stream, version ?? VERSION, options);
            if (
                InstanceFactories.Keys.FirstOrDefault(t => t.IsAssignableFrom(type)) is Type inheritedType &&
                InstanceFactories.TryGetValue(inheritedType, out factory)
                )
                return factory(type, stream, version ?? VERSION, options);
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
                    usedConstructor = ci;
                    return ci.GetParametersCached().Length == 0 ? ci.InvokeAuto() : ci.InvokeAuto(stream, version ?? Version);
                }
            }
            return type.ConstructAuto(out usedConstructor, usePrivate: true, stream, version, options)
                ?? throw new SerializerException($"Failed to instance {type}", new InvalidProgramException());
        }

        /// <summary>
        /// Create an instance
        /// </summary>
        /// <typeparam name="T">Requested type</typeparam>
        /// <param name="usedConstructor">Used constructor</param>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="options">Serializer options</param>
        /// <returns>Instance</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T CreateInstance<T>(out ConstructorInfo? usedConstructor, Stream stream, int? version = null, ISerializerOptions? options = null)
            => (T)CreateInstance(out usedConstructor, typeof(T), stream, version, options);

        /// <summary>
        /// Serializer delegate
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to serialize</param>
        public delegate void Serialize_Delegate(Stream stream, object? value);

        /// <summary>
        /// Serializer delegate
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to serialize</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public delegate Task AsyncSerialize_Delegate(Stream stream, object? value, CancellationToken cancellationToken);

        /// <summary>
        /// Deserializer delegate
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Type</param>
        /// <param name="version">Version</param>
        /// <param name="options">Options</param>
        /// <returns>Deserialized value</returns>
        public delegate object? Deserialize_Delegate(Stream stream, Type type, int version, ISerializerOptions? options);

        /// <summary>
        /// Deserializer delegate
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Type</param>
        /// <param name="version">Version</param>
        /// <param name="options">Options</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Deserialized value (task will be handled as <c>Task&lt;T&gt;</c>, a result is required!)</returns>
        public delegate Task AsyncDeserialize_Delegate(Stream stream, Type type, int version, ISerializerOptions? options, CancellationToken cancellationToken);

        /// <summary>
        /// Delegate for an instance factory
        /// </summary>
        /// <param name="type">Requested type</param>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="options">Serializer options</param>
        /// <returns>Instance</returns>
        public delegate object InstanceFactory_Delegate(Type type, Stream stream, int version, ISerializerOptions? options);

        /// <summary>
        /// Delegate for finding a serializer
        /// </summary>
        /// <param name="e">Event arguments</param>
        public delegate void FindSyncSerializer_Delegate(SerializerEventArgs<Serialize_Delegate> e);
        /// <summary>
        /// Raised when finding a serializer
        /// </summary>
        public static event FindSyncSerializer_Delegate? OnFindSyncSerializer;

        /// <summary>
        /// Delegate for finding a serializer
        /// </summary>
        /// <param name="e">Event arguments</param>
        public delegate void FindAsyncSerializer_Delegate(SerializerEventArgs<AsyncSerialize_Delegate> e);
        /// <summary>
        /// Raised when finding a serializer
        /// </summary>
        public static event FindAsyncSerializer_Delegate? OnFindAsyncSerializer;

        /// <summary>
        /// Delegate for finding a deserializer
        /// </summary>
        /// <param name="e">Event arguments</param>
        public delegate void FindSyncDeserializer_Delegate(SerializerEventArgs<Deserialize_Delegate> e);
        /// <summary>
        /// Raised when finding a deserializer
        /// </summary>
        public static event FindSyncDeserializer_Delegate? OnFindSyncDeserializer;

        /// <summary>
        /// Delegate for finding a deserializer
        /// </summary>
        /// <param name="e">Event arguments</param>
        public delegate void FindAsyncDeserializer_Delegate(SerializerEventArgs<AsyncDeserialize_Delegate> e);
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
