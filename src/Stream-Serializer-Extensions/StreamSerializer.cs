using System.Buffers;
using System.Collections.Concurrent;
using wan24.Core;
using static wan24.Core.TranslationHelper;

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
        public const int VERSION = 2;
        /// <summary>
        /// Binary serializer name (see <see cref="ObjectSerializer"/>)
        /// </summary>
        public const string BINARY_SERIALIZER_NAME = "BIN";

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
        /// Constructor
        /// </summary>
        static StreamSerializer()
        {
            SyncSerializer = new(
                [
                    new(typeof(bool),(s, v) => s.Write((bool)SerializerHelper.EnsureNotNull(v))),
                    new(typeof(sbyte),(s, v) => s.Write((sbyte)SerializerHelper.EnsureNotNull(v))),
                    new(typeof(byte),(s, v) => s.Write((byte)SerializerHelper.EnsureNotNull(v))),
                    new(typeof(short),(s, v) => s.Write((short)SerializerHelper.EnsureNotNull(v))),
                    new(typeof(ushort),(s, v) => s.Write((ushort)SerializerHelper.EnsureNotNull(v))),
                    new(typeof(int),(s, v) => s.Write((int)SerializerHelper.EnsureNotNull(v))),
                    new(typeof(uint),(s, v) => s.Write((uint)SerializerHelper.EnsureNotNull(v))),
                    new(typeof(long),(s, v) => s.Write((long)SerializerHelper.EnsureNotNull(v))),
                    new(typeof(ulong),(s, v) => s.Write((ulong)SerializerHelper.EnsureNotNull(v))),
                    new(typeof(float),(s, v) => s.Write((float)SerializerHelper.EnsureNotNull(v))),
                    new(typeof(double),(s, v) => s.Write((double)SerializerHelper.EnsureNotNull(v))),
                    new(typeof(decimal),(s, v) => s.Write((decimal)SerializerHelper.EnsureNotNull(v))),
                    new(typeof(byte[]),(s, v) => s.WriteBytes((byte[])SerializerHelper.EnsureNotNull(v))),
                    new(typeof(string),(s, v) => s.WriteString((string)SerializerHelper.EnsureNotNull(v))),
                    new(typeof(Array),(s, v) =>
                    {
                        SerializerHelper.EnsureNotNull(v);
                        StreamExtensions.WriteArrayMethod.MakeGenericMethod(typeof(Stream),v!.GetType().GetElementType()!).InvokeAuto(obj: null, s,v);
                    }),
                    new(typeof(List<>),(s, v) =>
                    {
                        SerializerHelper.EnsureNotNull(v);
                        StreamExtensions.WriteListMethod.MakeGenericMethod(typeof(Stream),v!.GetType().GetGenericArguments()[0]).InvokeAuto(obj: null, s,v);
                    }),
                    new(typeof(Dictionary<,>),(s, v) =>
                    {
                        SerializerHelper.EnsureNotNull(v);
                        Type[] genericArgs=v!.GetType().GetGenericArguments();
                        StreamExtensions.WriteDictMethod.MakeGenericMethod(typeof(Stream),genericArgs[0], genericArgs[1]).InvokeAuto(obj: null, s,v);
                    }),
                    new(typeof(Enum),(s, v) =>
                    {
                        SerializerHelper.EnsureNotNull(v);
                        StreamExtensions.WriteEnumMethod.MakeGenericMethod(typeof(Stream),v!.GetType()).InvokeAuto(obj: null, s, v);
                    }),
                    new(typeof(Stream),(s, v) => s.WriteStream((Stream)SerializerHelper.EnsureNotNull(v)))
                ]
            );
            AsyncSerializer = new(
                [
                    new(typeof(bool),(s, v, ct) => s.WriteAsync((bool)SerializerHelper.EnsureNotNull(v), ct)),
                    new(typeof(sbyte),(s, v, ct) => s.WriteAsync((sbyte)SerializerHelper.EnsureNotNull(v), ct)),
                    new(typeof(byte),(s, v, ct) => s.WriteAsync((byte)SerializerHelper.EnsureNotNull(v), ct)),
                    new(typeof(short),(s, v, ct) => s.WriteAsync((short)SerializerHelper.EnsureNotNull(v), ct)),
                    new(typeof(ushort),(s, v, ct) => s.WriteAsync((ushort)SerializerHelper.EnsureNotNull(v), ct)),
                    new(typeof(int),(s, v, ct) => s.WriteAsync((int)SerializerHelper.EnsureNotNull(v), ct)),
                    new(typeof(uint),(s, v, ct) => s.WriteAsync((uint)SerializerHelper.EnsureNotNull(v), ct)),
                    new(typeof(long),(s, v, ct) => s.WriteAsync((long)SerializerHelper.EnsureNotNull(v), ct)),
                    new(typeof(ulong),(s, v, ct) => s.WriteAsync((ulong)SerializerHelper.EnsureNotNull(v), ct)),
                    new(typeof(float),(s, v, ct) => s.WriteAsync((float)SerializerHelper.EnsureNotNull(v), ct)),
                    new(typeof(double),(s, v, ct) => s.WriteAsync((double)SerializerHelper.EnsureNotNull(v), ct)),
                    new(typeof(decimal),(s, v, ct) => s.WriteAsync((decimal)SerializerHelper.EnsureNotNull(v), ct)),
                    new(typeof(byte[]),(s, v, ct) => s.WriteBytesAsync((byte[])SerializerHelper.EnsureNotNull(v), ct)),
                    new(typeof(string),(s, v, ct) => s.WriteStringAsync((string)SerializerHelper.EnsureNotNull(v), ct)),
                    new(typeof(Array),(s, v, ct) =>
                    {
                        SerializerHelper.EnsureNotNull(v);
                        return (Task)StreamExtensions.WriteArrayAsyncMethod.MakeGenericMethod(v!.GetType().GetElementType()!).InvokeAuto(obj: null, s,v,ct)!;
                    }),
                    new(typeof(List<>),(s, v, ct) =>
                    {
                        SerializerHelper.EnsureNotNull(v);
                        return (Task)StreamExtensions.WriteListAsyncMethod.MakeGenericMethod(v!.GetType().GetGenericArguments()[0]).InvokeAuto(obj: null, s,v,ct)!;
                    }),
                    new(typeof(Dictionary<,>),(s, v, ct) =>
                    {
                        SerializerHelper.EnsureNotNull(v);
                        Type[] genericArgs=v!.GetType().GetGenericArguments();
                        return (Task)StreamExtensions.WriteDictAsyncMethod.MakeGenericMethod(genericArgs[0], genericArgs[1]).InvokeAuto(obj: null, s,v,ct)!;
                    }),
                    new(typeof(Enum),(s, v, ct) =>
                    {
                        SerializerHelper.EnsureNotNull(v);
                        return (Task)StreamExtensions.WriteEnumAsyncMethod.MakeGenericMethod(v !.GetType()).InvokeAuto(obj : null, s, v, ct)!;
                    }),
                    new(typeof(Stream),(s, v, ct) => s.WriteStreamAsync((Stream)SerializerHelper.EnsureNotNull(v), cancellationToken: ct)),
                ]
            );
            SyncDeserializer = new(
                [
                    new(typeof(bool),(s,t,v,o) => s.ReadBool(v)),
                    new(typeof(sbyte),(s,t,v,o) => s.ReadOneSByte(v)),
                    new(typeof(byte),(s,t,v,o) => s.ReadOneByte(v)),
                    new(typeof(short),(s,t,v,o) => s.ReadShort(v)),
                    new(typeof(ushort),(s,t,v,o) => s.ReadUShort(v)),
                    new(typeof(int),(s,t,v,o) => s.ReadInt(v)),
                    new(typeof(uint),(s,t,v,o) => s.ReadUInt(v)),
                    new(typeof(long),(s,t,v,o) => s.ReadLong(v)),
                    new(typeof(ulong),(s,t,v,o) => s.ReadULong(v)),
                    new(typeof(float),(s,t,v,o) => s.ReadFloat(v)),
                    new(typeof(double),(s,t,v,o) => s.ReadDouble(v)),
                    new(typeof(decimal),(s,t,v,o) => s.ReadDecimal(v)),
                    new(typeof(byte[]),(s,t,v,o) => 
                        s.ReadBytes(v, minLen: o?.GetMinLen(0)??0, maxLen: o?.GetMaxLen(int.MaxValue)??int.MaxValue)
                        ),
                    new(typeof(string),(s,t,v,o) => 
                        s.ReadString(v, minLen: o?.GetMinLen(0)??0, maxLen: o?.GetMaxLen(int.MaxValue)??int.MaxValue)
                        ),
                    new(typeof(Array),(s,t,v,o) =>
                    {
                        SerializerHelper.EnsureNotNull(v);
                        return StreamExtensions.ReadArrayMethod.MakeGenericMethod(t.GetElementType()!).InvokeAuto(
                            obj: null, 
                            s,
                            v,
                            o?.GetMinLen(0)??0,
                            o?.GetMaxLen(int.MaxValue)??int.MaxValue,
                            o?.Attribute.GetValueSerializerOptions(property: null, s, v, default)
                            );
                    }),
                    new(typeof(List<>),(s,t,v,o) =>
                        StreamExtensions.ReadListMethod.MakeGenericMethod(t.GetGenericArguments()[0]).InvokeAuto(
                            obj : null, 
                            s, 
                            v,
                            o?.GetMinLen(0)??0,
                            o?.GetMaxLen(int.MaxValue)??int.MaxValue,
                            o?.Attribute.GetValueSerializerOptions(property: null, s, v, default)
                            )
                    ),
                    new(typeof(Dictionary<,>),(s,t,v,o) =>
                    {
                        Type[] genericArgs=t.GetGenericArguments();
                        return StreamExtensions.ReadDictMethod.MakeGenericMethod(genericArgs[0], genericArgs[1]).InvokeAuto(
                            obj : null, 
                            s, 
                            v,
                            o?.GetMinLen(0)??0,
                            o?.GetMaxLen(int.MaxValue)??int.MaxValue,
                            o?.Attribute.GetKeySerializerOptions(property: null, s, v, default),
                            o?.Attribute.GetValueSerializerOptions(property: null, s, v, default)
                            );
                    }),
                    new(typeof(Enum),(s,t,v,o) =>
                        StreamExtensions.ReadEnumMethod.MakeGenericMethod(t).InvokeAuto(obj : null, s, v)
                    ),
                    new(typeof(Stream),(s,t,v,o) =>
                    {
                        Stream res = o?.Attribute.GetStream(obj:null,property:null,s,v,default)?? new FileStream(
                            Path.GetTempFileName(),
                            FileMode.OpenOrCreate,
                            FileAccess.ReadWrite,
                            FileShare.None,
                            bufferSize: Settings.BufferSize,
                            FileOptions.RandomAccess | FileOptions.DeleteOnClose
                            );
                        try
                        {
                            s.ReadStream(res,v,minLen:o?.GetMinLen(0L)??0, maxLen:o?.GetMaxLen(long.MaxValue)??long.MaxValue);
                            return res;
                        }
                        catch
                        {
                            res.Dispose();
                            throw;
                        }
                    })
                ]);
            AsyncDeserializer = new(
                [
                    new(typeof(bool),(s,t,v,o,ct) => s.ReadBoolAsync(v, cancellationToken: ct)),
                    new(typeof(sbyte),(s,t,v,o,ct) => s.ReadOneSByteAsync(v, ct)),
                    new(typeof(byte),(s,t,v,o,ct) => s.ReadOneByteAsync(v, ct)),
                    new(typeof(short),(s,t,v,o,ct) => s.ReadShortAsync(v, cancellationToken: ct)),
                    new(typeof(ushort),(s,t,v,o,ct) => s.ReadUShortAsync(v, cancellationToken: ct)),
                    new(typeof(int),(s,t,v,o,ct) => s.ReadIntAsync(v, cancellationToken: ct)),
                    new(typeof(uint),(s,t,v,o,ct) => s.ReadUIntAsync(v, cancellationToken: ct)),
                    new(typeof(long),(s,t,v,o,ct) => s.ReadLongAsync(v, cancellationToken: ct)),
                    new(typeof(ulong),(s,t,v,o,ct) => s.ReadULongAsync(v, cancellationToken: ct)),
                    new(typeof(float),(s,t,v,o,ct) => s.ReadFloatAsync(v, cancellationToken: ct)),
                    new(typeof(double),(s,t,v,o,ct) => s.ReadDoubleAsync(v, cancellationToken: ct)),
                    new(typeof(decimal),(s,t,v,o,ct) => s.ReadDecimalAsync(v, cancellationToken: ct)),
                    new(typeof(byte[]),(s,t,v,o,ct) => 
                        s.ReadBytesAsync(v, minLen: o?.GetMinLen(0)??0, maxLen: o?.GetMaxLen(int.MaxValue)??int.MaxValue, cancellationToken: ct)
                        ),
                    new(typeof(string),(s,t,v,o,ct) => 
                        s.ReadStringAsync(v, minLen: o?.GetMinLen(0)??0, maxLen: o?.GetMaxLen(int.MaxValue)??int.MaxValue, cancellationToken: ct)
                        ),
                    new(typeof(Array),(s,t,v,o,ct) =>
                        (Task)StreamExtensions.ReadArrayAsyncMethod.MakeGenericMethod(t.GetElementType()!).InvokeAuto(
                            obj: null, 
                            s,
                            v,
                            o?.GetMinLen(0)??0,
                            o?.GetMaxLen(int.MaxValue)??int.MaxValue,
                            o?.Attribute.GetValueSerializerOptions(property: null, s, v, default),
                            ct
                            )!
                    ),
                    new(typeof(List<>),(s,t,v,o,ct) =>
                        (Task)StreamExtensions.ReadListAsyncMethod.MakeGenericMethod(t.GetGenericArguments()[0]).InvokeAuto(
                            obj: null, 
                            s,
                            v,
                            o?.GetMinLen(0)??0,
                            o?.GetMaxLen(int.MaxValue)??int.MaxValue,
                            o?.Attribute.GetValueSerializerOptions(property: null, s, v, default),
                            ct
                            )!
                    ),
                    new(typeof(Dictionary<,>),(s,t,v,o,ct) =>
                    {
                        Type[] genericArgs=t.GetGenericArguments();
                        return (Task)StreamExtensions.ReadDictAsyncMethod.MakeGenericMethod(genericArgs[0], genericArgs[1]).InvokeAuto(
                            obj : null, 
                            s, 
                            v,
                            o?.GetMinLen(0)??0,
                            o?.GetMaxLen(int.MaxValue)??int.MaxValue,
                            o?.Attribute.GetKeySerializerOptions(property: null, s, v, default),
                            o?.Attribute.GetValueSerializerOptions(property: null, s, v, default),
                            ct
                            )!;
                    }),
                    new(typeof(Enum),(s,t,v,o,ct) =>
                        (Task)StreamExtensions.ReadEnumAsyncMethod.MakeGenericMethod(t).InvokeAuto(obj : null, s, v, ct)!
                    ),
                    new(typeof(Stream),(s,t,v,o,ct) => StreamDeserializer(s,t,v,o,ct))
                ]);
            AllowedTypes = new(
            [
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
                typeof(char)
            ]);
        }

        /// <summary>
        /// Version number
        /// </summary>
        public static int Version { get; set; } = VERSION;

        /// <summary>
        /// Byte array buffer pool to use
        /// </summary>
        public static ArrayPool<byte> BufferPool { get; set; } = ArrayPool<byte>.Shared;

        /// <summary>
        /// State
        /// </summary>
        public static IEnumerable<Status> State
        {
            get
            {
                yield return new(__("Version"), VERSION, __("Serializer version number"));
                yield return new(__("Effective version"), Version, __("Effective serializer version number including optional custom version informations"));
                yield return new(__("Any attribute required"), StreamExtensions.AnyObjectAttributeRequired, __("If a 'StreamSerializerAttribute' is required when deserializing using 'ReadAnyType'"));
                foreach (Type type in AllowedTypes)
                    yield return new(__("Allowed"), type, __("Non-array CLR type which is allowed for (de)serialization"), __("Allowed types"));
                foreach (Type type in SyncSerializer.Keys.Concat(AsyncSerializer.Keys).Distinct())
                    yield return new(__("Serializer"), type, __("CLR type which has a serializer defined"), __("Serializers"));
                foreach (Type type in SyncDeserializer.Keys.Concat(AsyncDeserializer.Keys).Distinct())
                    yield return new(__("Deserializer"), type, __("CLR type which has a deserializer defined"), __("Deserializers"));
            }
        }

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
        {
            try
            {
                if ((Type.GetType(name, throwOnError: false) ?? TypeHelper.Instance.GetType(name)) is Type type && IsTypeAllowed(type)) return type;
                TypeLoadEventArgs e = new(name);
                OnLoadType?.Invoke(e);
                if (e.Type == null) throw new SerializerException($"Failed to load type \"{name}\"");
                return e.Type;
            }
            catch (SerializerException)
            {
                throw;
            }
            catch(Exception ex)
            {
                throw new SerializerException($"Failed to load type \"{name}\"", ex);
            }
        }

        /// <summary>
        /// Determine if a type is allowed per default for deserializing
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Is allowed?</returns>
        public static bool IsTypeAllowed(Type type)
        {
            // Allow all enum types
            if (type.IsEnum) return true;
            // Get the final element type of an array type
            Type finalType = type;
            Type? elementType;
            while (finalType.IsArray)
            {
                elementType = finalType.GetElementType();
                if (elementType == null) break;
                finalType = elementType;
            }
            // Allow registered allowed types
            if (AllowedTypes.Contains(finalType) || typeof(IStreamSerializer).IsAssignableFrom(finalType)) return true;
            // Allow supported generic types
            if (finalType.IsGenericType)
                if (typeof(Dictionary<,>).IsAssignableFrom(finalType.GetGenericTypeDefinition()))
                {
                    Type[] gp = finalType.GetGenericArguments();
                    return IsTypeAllowed(gp[0]) && IsTypeAllowed(gp[1]);
                }
                else if (typeof(List<>).IsAssignableFrom(finalType.GetGenericTypeDefinition()))
                {
                    return IsTypeAllowed(finalType.GetGenericArguments()[0]);
                }
                else if (AllowedTypes.Contains(finalType.GetGenericTypeDefinition()))
                {
                    return true;
                }
            // Deny
            return false;
        }

        /// <summary>
        /// Asynchronous stream deserializer
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Requested return type</param>
        /// <param name="version">Serializer version</param>
        /// <param name="options">Options</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>File stream</returns>
        public static async Task<Stream> StreamDeserializer(Stream stream, Type type, int version, ISerializerOptions? options, CancellationToken cancellationToken)
        {
            Stream res = options?.Attribute.GetStream(obj: null, property: null, stream, version, cancellationToken) ?? new FileStream(
                Path.Combine(Settings.TempFolder, Guid.NewGuid().ToString()),
                FileMode.OpenOrCreate,
                FileAccess.ReadWrite,
                FileShare.None,
                bufferSize: Settings.BufferSize,
                FileOptions.RandomAccess | FileOptions.DeleteOnClose
                );
            if (!type.IsAssignableFrom(res.GetType())) throw new SerializerException($"Requested type {type} isn't compatible with {res.GetType()}");
            try
            {
                return await stream.ReadStreamAsync(res, version, cancellationToken: cancellationToken).DynamicContext();
            }
            catch
            {
                res.Dispose();
                throw;
            }
        }

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
