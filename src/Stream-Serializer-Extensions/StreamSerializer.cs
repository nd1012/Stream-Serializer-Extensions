using System.Collections.Concurrent;
using wan24.Core;

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
        public const int VERSION = 1;

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
                    new KeyValuePair<Type, Serialize_Delegate>(typeof(short),(s, v) => s.Write((short)SerializerHelper.EnsureNotNull(v))),
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
                    new KeyValuePair<Type, Serialize_Delegate>(typeof(Array),(s, v) =>
                    {
                        SerializerHelper.EnsureNotNull(v);
                        StreamExtensions.WriteArrayMethod.MakeGenericMethod(typeof(Stream),v!.GetType().GetElementType()!).InvokeAuto(obj: null, s,v);
                    }),
                    new KeyValuePair<Type, Serialize_Delegate>(typeof(List<>),(s, v) =>
                    {
                        SerializerHelper.EnsureNotNull(v);
                        StreamExtensions.WriteListMethod.MakeGenericMethod(typeof(Stream),v!.GetType().GetGenericArguments()[0]).InvokeAuto(obj: null, s,v);
                    }),
                    new KeyValuePair<Type, Serialize_Delegate>(typeof(Dictionary<,>),(s, v) =>
                    {
                        SerializerHelper.EnsureNotNull(v);
                        Type[] genericArgs=v!.GetType().GetGenericArguments();
                        StreamExtensions.WriteDictMethod.MakeGenericMethod(typeof(Stream),genericArgs[0], genericArgs[1]).InvokeAuto(obj: null, s,v);
                    }),
                    new KeyValuePair<Type, Serialize_Delegate>(typeof(Enum),(s, v) =>
                    {
                        SerializerHelper.EnsureNotNull(v);
                        StreamExtensions.WriteEnumMethod.MakeGenericMethod(typeof(Stream),v!.GetType()).InvokeAuto(obj: null, s, v);
                    })
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
                    new KeyValuePair<Type, AsyncSerialize_Delegate>(typeof(Array),(s, v, ct) =>
                    {
                        SerializerHelper.EnsureNotNull(v);
                        return (Task)StreamExtensions.WriteArrayAsyncMethod.MakeGenericMethod(v!.GetType().GetElementType()!).InvokeAuto(obj: null, s,v,ct)!;
                    }),
                    new KeyValuePair<Type, AsyncSerialize_Delegate>(typeof(List<>),(s, v, ct) =>
                    {
                        SerializerHelper.EnsureNotNull(v);
                        return (Task)StreamExtensions.WriteListAsyncMethod.MakeGenericMethod(v!.GetType().GetGenericArguments()[0]).InvokeAuto(obj: null, s,v,ct)!;
                    }),
                    new KeyValuePair<Type, AsyncSerialize_Delegate>(typeof(Dictionary<,>),(s, v, ct) =>
                    {
                        SerializerHelper.EnsureNotNull(v);
                        Type[] genericArgs=v!.GetType().GetGenericArguments();
                        return (Task)StreamExtensions.WriteDictAsyncMethod.MakeGenericMethod(genericArgs[0], genericArgs[1]).InvokeAuto(obj: null, s,v,ct)!;
                    }),
                    new KeyValuePair<Type, AsyncSerialize_Delegate>(typeof(Enum),(s, v, ct) =>
                    {
                        SerializerHelper.EnsureNotNull(v);
                        return (Task)StreamExtensions.WriteEnumAsyncMethod.MakeGenericMethod(v !.GetType()).InvokeAuto(obj : null, s, v, ct)!;
                    })
                }
            );
            SyncDeserializer = new(new KeyValuePair<Type, Deserialize_Delegate>[]
            {
                    new KeyValuePair<Type, Deserialize_Delegate>(typeof(bool),(s,t,v) => s.ReadBool(v)),
                    new KeyValuePair<Type, Deserialize_Delegate>(typeof(sbyte),(s,t,v) => s.ReadOneSByte(v)),
                    new KeyValuePair<Type, Deserialize_Delegate>(typeof(byte),(s,t,v) => s.ReadOneByte(v)),
                    new KeyValuePair<Type, Deserialize_Delegate>(typeof(short),(s,t,v) => s.ReadShort(v)),
                    new KeyValuePair<Type, Deserialize_Delegate>(typeof(ushort),(s,t,v) => s.ReadUShort(v)),
                    new KeyValuePair<Type, Deserialize_Delegate>(typeof(int),(s,t,v) => s.ReadInt(v)),
                    new KeyValuePair<Type, Deserialize_Delegate>(typeof(uint),(s,t,v) => s.ReadUInt(v)),
                    new KeyValuePair<Type, Deserialize_Delegate>(typeof(long),(s,t,v) => s.ReadLong(v)),
                    new KeyValuePair<Type, Deserialize_Delegate>(typeof(ulong),(s,t,v) => s.ReadULong(v)),
                    new KeyValuePair<Type, Deserialize_Delegate>(typeof(float),(s,t,v) => s.ReadFloat(v)),
                    new KeyValuePair<Type, Deserialize_Delegate>(typeof(double),(s,t,v) => s.ReadDouble(v)),
                    new KeyValuePair<Type, Deserialize_Delegate>(typeof(decimal),(s,t,v) => s.ReadDecimal(v)),
                    new KeyValuePair<Type, Deserialize_Delegate>(typeof(byte[]),(s,t,v) => s.ReadBytes(v)),
                    new KeyValuePair<Type, Deserialize_Delegate>(typeof(string),(s,t,v) => s.ReadString(v)),
                    new KeyValuePair<Type, Deserialize_Delegate>(typeof(Array),(s,t,v) =>
                    {
                        SerializerHelper.EnsureNotNull(v);
                        return StreamExtensions.ReadArrayMethod.MakeGenericMethod(t.GetElementType()!).InvokeAuto(obj: null, s,v);
                    }),
                    new KeyValuePair<Type, Deserialize_Delegate>(typeof(List<>),(s,t,v) =>
                        StreamExtensions.ReadListMethod.MakeGenericMethod(t.GetGenericArguments()[0]).InvokeAuto(obj : null, s, v)
                    ),
                    new KeyValuePair<Type, Deserialize_Delegate>(typeof(Dictionary<,>),(s,t,v) =>
                    {
                        Type[] genericArgs=t.GetGenericArguments();
                        return StreamExtensions.ReadDictMethod.MakeGenericMethod(genericArgs[0], genericArgs[1]).InvokeAuto(obj : null, s, v);
                    }),
                    new KeyValuePair<Type, Deserialize_Delegate>(typeof(Enum),(s,t,v) =>
                        StreamExtensions.ReadEnumMethod.MakeGenericMethod(t).InvokeAuto(obj : null, s, v)
                    )
            });
            AsyncDeserializer = new(new KeyValuePair<Type, AsyncDeserialize_Delegate>[]
            {
                    new KeyValuePair<Type, AsyncDeserialize_Delegate>(typeof(bool),(s,t,v,ct) => s.ReadBoolAsync(v, cancellationToken: ct)),
                    new KeyValuePair<Type, AsyncDeserialize_Delegate>(typeof(sbyte),(s,t,v,ct) => s.ReadOneSByteAsync(v, ct)),
                    new KeyValuePair<Type, AsyncDeserialize_Delegate>(typeof(byte),(s,t,v,ct) => s.ReadOneByteAsync(v, ct)),
                    new KeyValuePair<Type, AsyncDeserialize_Delegate>(typeof(short),(s,t,v,ct) => s.ReadShortAsync(v, cancellationToken: ct)),
                    new KeyValuePair<Type, AsyncDeserialize_Delegate>(typeof(ushort),(s,t,v,ct) => s.ReadUShortAsync(v, cancellationToken: ct)),
                    new KeyValuePair<Type, AsyncDeserialize_Delegate>(typeof(int),(s,t,v,ct) => s.ReadIntAsync(v, cancellationToken: ct)),
                    new KeyValuePair<Type, AsyncDeserialize_Delegate>(typeof(uint),(s,t,v,ct) => s.ReadUIntAsync(v, cancellationToken: ct)),
                    new KeyValuePair<Type, AsyncDeserialize_Delegate>(typeof(long),(s,t,v,ct) => s.ReadLongAsync(v, cancellationToken: ct)),
                    new KeyValuePair<Type, AsyncDeserialize_Delegate>(typeof(ulong),(s,t,v,ct) => s.ReadULongAsync(v, cancellationToken: ct)),
                    new KeyValuePair<Type, AsyncDeserialize_Delegate>(typeof(float),(s,t,v,ct) => s.ReadFloatAsync(v, cancellationToken: ct)),
                    new KeyValuePair<Type, AsyncDeserialize_Delegate>(typeof(double),(s,t,v,ct) => s.ReadDoubleAsync(v, cancellationToken: ct)),
                    new KeyValuePair<Type, AsyncDeserialize_Delegate>(typeof(decimal),(s,t,v,ct) => s.ReadDecimalAsync(v, cancellationToken: ct)),
                    new KeyValuePair<Type, AsyncDeserialize_Delegate>(typeof(byte[]),(s,t,v,ct) => s.ReadBytesAsync(v, cancellationToken: ct)),
                    new KeyValuePair<Type, AsyncDeserialize_Delegate>(typeof(string),(s,t,v,ct) => s.ReadStringAsync(v, cancellationToken: ct)),
                    new KeyValuePair<Type, AsyncDeserialize_Delegate>(typeof(Array),(s,t,v,ct) =>
                        (Task)StreamExtensions.ReadArrayAsyncMethod.MakeGenericMethod(t.GetElementType()!).InvokeAuto(obj: null, s,v,null,0,int.MaxValue,ct)!
                    ),
                    new KeyValuePair<Type, AsyncDeserialize_Delegate>(typeof(List<>),(s,t,v,ct) =>
                        (Task)StreamExtensions.ReadListAsyncMethod.MakeGenericMethod(t.GetGenericArguments()[0]).InvokeAuto(obj: null, s,v,null,0,int.MaxValue,ct)!
                    ),
                    new KeyValuePair<Type, AsyncDeserialize_Delegate>(typeof(Dictionary<,>),(s,t,v,ct) =>
                    {
                        Type[] genericArgs=t.GetGenericArguments();
                        return (Task)StreamExtensions.ReadDictAsyncMethod.MakeGenericMethod(genericArgs[0], genericArgs[1]).InvokeAuto(obj : null, s, v,null,0,int.MaxValue,ct)!;
                    }),
                    new KeyValuePair<Type, AsyncDeserialize_Delegate>(typeof(Enum),(s,t,v,ct) =>
                        (Task)StreamExtensions.ReadEnumAsyncMethod.MakeGenericMethod(t).InvokeAuto(obj : null, s, v, null, ct)!
                    )
            });
        }

        /// <summary>
        /// Version number
        /// </summary>
        public static int Version { get; set; } = VERSION;

        /// <summary>
        /// Find a serializer
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Serializer</returns>
        public static Serialize_Delegate? FindSerializer(Type type)
        {
            Type? serializer = FindDelegateType(type, SyncSerializer.Keys);
            Serialize_Delegate? res = null;
            if (serializer == null || !SyncSerializer.TryGetValue(serializer, out res))
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
            AsyncSerialize_Delegate? res = null;
            if (serializer == null || !AsyncSerializer.TryGetValue(serializer, out res))
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
            Deserialize_Delegate? res = null;
            if (serializer == null || !SyncDeserializer.TryGetValue(serializer, out res))
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
            AsyncDeserialize_Delegate? res = null;
            if (serializer == null || !AsyncDeserializer.TryGetValue(serializer, out res))
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
                Type? res = Type.GetType(name);
                if (res == null)
                {
                    TypeLoadEventArgs e = new(name);
                    OnLoadType?.Invoke(e);
                    if (e.Type == null) throw new SerializerException($"Failed to load type \"{name}\"");
                    res = e.Type;
                }
                return res;
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
        /// <returns>Deserialized value</returns>
        public delegate object? Deserialize_Delegate(Stream stream, Type type, int version);

        /// <summary>
        /// Deserializer delegate
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Type</param>
        /// <param name="version">Version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Deserialized value (task will be handled as <c>Task&lt;T&gt;</c>, a result is required!)</returns>
        public delegate Task AsyncDeserialize_Delegate(Stream stream, Type type, int version, CancellationToken cancellationToken);

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
