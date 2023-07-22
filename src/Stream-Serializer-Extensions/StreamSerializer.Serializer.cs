using System.Collections;
using System.Collections.Concurrent;
using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    // Serializer
    public static partial class StreamSerializer
    {
        /// <summary>
        /// Version number
        /// </summary>
        public const int VERSION = 3;

        /// <summary>
        /// Serializer
        /// </summary>
        public static readonly ConcurrentDictionary<Type, Serializer_Delegate> SyncSerializer;
        /// <summary>
        /// Serializer
        /// </summary>
        public static readonly ConcurrentDictionary<Type, AsyncSerializer_Delegate> AsyncSerializer;
        /// <summary>
        /// Deserializer
        /// </summary>
        public static readonly ConcurrentDictionary<Type, Deserializer_Delegate> SyncDeserializer;
        /// <summary>
        /// Deserializer
        /// </summary>
        public static readonly ConcurrentDictionary<Type, AsyncDeserializer_Delegate> AsyncDeserializer;

        /// <summary>
        /// Constructor
        /// </summary>
        static StreamSerializer()
        {
            SyncSerializer = new(
                new KeyValuePair<Type, Serializer_Delegate>[]
                {
                    new(typeof(bool),(c, v) => c.Stream.Write((bool)SerializerHelper.EnsureNotNull(v),c)),
                    new(typeof(sbyte),(c, v) => c.Stream.Write((sbyte)SerializerHelper.EnsureNotNull(v),c)),
                    new(typeof(byte),(c, v) => c.Stream.Write((byte)SerializerHelper.EnsureNotNull(v),c)),
                    new(typeof(short),(c, v) => c.Stream.Write((int)(short)SerializerHelper.EnsureNotNull(v),c)),
                    new(typeof(ushort),(c, v) => c.Stream.Write((ushort)SerializerHelper.EnsureNotNull(v),c)),
                    new(typeof(int),(c, v) => c.Stream.Write((int)SerializerHelper.EnsureNotNull(v),c)),
                    new(typeof(uint),(c, v) => c.Stream.Write((uint)SerializerHelper.EnsureNotNull(v),c)),
                    new(typeof(long),(c, v) => c.Stream.Write((long)SerializerHelper.EnsureNotNull(v),c)),
                    new(typeof(ulong),(c, v) => c.Stream.Write((ulong)SerializerHelper.EnsureNotNull(v),c)),
                    new(typeof(float),(c, v) => c.Stream.Write((float)SerializerHelper.EnsureNotNull(v),c)),
                    new(typeof(double),(c, v) => c.Stream.Write((double)SerializerHelper.EnsureNotNull(v),c)),
                    new(typeof(decimal),(c, v) => c.Stream.Write((decimal)SerializerHelper.EnsureNotNull(v),c)),
                    new(typeof(byte[]),(c, v) => c.Stream.WriteBytes((byte[])SerializerHelper.EnsureNotNull(v),c)),
                    new(typeof(string),(c, v) => c.Stream.WriteString((string)SerializerHelper.EnsureNotNull(v),c)),
                    new(typeof(Array),(c, v) => c.Stream.WriteArray((Array)SerializerHelper.EnsureNotNull(v),c)),
                    new(typeof(IList),(c, v) => c.Stream.WriteList((IList)SerializerHelper.EnsureNotNull(v),c)),
                    new(typeof(IDictionary),(c, v) => c.Stream.WriteDict((IDictionary)SerializerHelper.EnsureNotNull(v),c)),
                    new(typeof(Enum),(c, v) => c.Stream.WriteEnum((Enum)SerializerHelper.EnsureNotNull(v),c)),
                    new(typeof(Stream),(c, v) => c.Stream.WriteStream((Stream)SerializerHelper.EnsureNotNull(v),c)),
                    new(typeof(Type),(c, v) => c.Stream.Write((Type)SerializerHelper.EnsureNotNull(v),c))
                }
            );
            AsyncSerializer = new(
                new KeyValuePair<Type, AsyncSerializer_Delegate>[]
                {
                    new(typeof(bool),(c, v) => c.Stream.WriteAsync((bool)SerializerHelper.EnsureNotNull(v), c)),
                    new(typeof(sbyte),(c, v) => c.Stream.WriteAsync((sbyte)SerializerHelper.EnsureNotNull(v), c)),
                    new(typeof(byte),(c, v) => c.Stream.WriteAsync((byte)SerializerHelper.EnsureNotNull(v), c)),
                    new(typeof(short),(c, v) => c.Stream.WriteAsync((short)SerializerHelper.EnsureNotNull(v), c)),
                    new(typeof(ushort),(c, v) => c.Stream.WriteAsync((ushort)SerializerHelper.EnsureNotNull(v), c)),
                    new(typeof(int),(c, v) => c.Stream.WriteAsync((int)SerializerHelper.EnsureNotNull(v), c)),
                    new(typeof(uint),(c, v) => c.Stream.WriteAsync((uint)SerializerHelper.EnsureNotNull(v), c)),
                    new(typeof(long),(c, v) => c.Stream.WriteAsync((long)SerializerHelper.EnsureNotNull(v), c)),
                    new(typeof(ulong),(c, v) => c.Stream.WriteAsync((ulong)SerializerHelper.EnsureNotNull(v), c)),
                    new(typeof(float),(c, v) => c.Stream.WriteAsync((float)SerializerHelper.EnsureNotNull(v), c)),
                    new(typeof(double),(c, v) => c.Stream.WriteAsync((double)SerializerHelper.EnsureNotNull(v), c)),
                    new(typeof(decimal),(c, v) => c.Stream.WriteAsync((decimal)SerializerHelper.EnsureNotNull(v), c)),
                    new(typeof(byte[]),(c, v) => c.Stream.WriteBytesAsync((byte[])SerializerHelper.EnsureNotNull(v), c)),
                    new(typeof(string),(c, v) => c.Stream.WriteStringAsync((string)SerializerHelper.EnsureNotNull(v), c)),
                    new(typeof(Array),(c, v) => c.Stream.WriteArrayAsync((Array)SerializerHelper.EnsureNotNull(v), c)),
                    new(typeof(IList),(c, v) => c.Stream.WriteListAsync((IList)SerializerHelper.EnsureNotNull(v), c)),
                    new(typeof(IDictionary),(c, v) => c.Stream.WriteDictAsync((IDictionary)SerializerHelper.EnsureNotNull(v), c)),
                    new(typeof(Enum),(c, v) => c.Stream.WriteEnumAsync((Enum)SerializerHelper.EnsureNotNull(v), c)),
                    new(typeof(Stream),(c, v) => c.Stream.WriteStreamAsync((Stream)SerializerHelper.EnsureNotNull(v), c)),
                    new(typeof(Type),(c, v) => c.Stream.WriteAsync((Type)SerializerHelper.EnsureNotNull(v), c))
                }
            );
            SyncDeserializer = new(new KeyValuePair<Type, Deserializer_Delegate>[]
            {
                    new(typeof(bool),(c,t) => c.Stream.ReadBool(c)),
                    new(typeof(sbyte),(c,t) => c.Stream.ReadOneSByte(c)),
                    new(typeof(byte),(c,t) => c.Stream.ReadOneByte(c)),
                    new(typeof(short),(c,t) => c.Stream.ReadShort(c)),
                    new(typeof(ushort),(c,t) => c.Stream.ReadUShort(c)),
                    new(typeof(int),(c,t) => c.Stream.ReadInt(c)),
                    new(typeof(uint),(c,t) => c.Stream.ReadUInt(c)),
                    new(typeof(long),(c,t) => c.Stream.ReadLong(c)),
                    new(typeof(ulong),(c,t) => c.Stream.ReadULong(c)),
                    new(typeof(float),(c,t) => c.Stream.ReadFloat(c)),
                    new(typeof(double),(c,t) => c.Stream.ReadDouble(c)),
                    new(typeof(decimal),(c,t) => c.Stream.ReadDecimal(c)),
                    new(typeof(byte[]),(c,t) =>
                        c.Stream.ReadBytes(c, minLen: c.Options?.GetMinLen(0)??0, maxLen: c.Options?.GetMaxLen(int.MaxValue)??int.MaxValue)
                        ),
                    new(typeof(string),(c,t) =>
                        c.Stream.ReadString(c, minLen: c.Options?.GetMinLen(0)??0, maxLen: c.Options?.GetMaxLen(int.MaxValue)??int.MaxValue)
                        ),
                    new(typeof(Array),(c,t) =>
                        c.Stream.ReadArray(
                            t,
                            c,
                            c.Options?.GetMinLen(0)??0,
                            c.Options?.GetMaxLen(int.MaxValue)??int.MaxValue
                            )
                    ),
                    new(typeof(IList),(c,t) =>
                        c.Stream.ReadList(
                            t,
                            c,
                            c.Options?.GetMinLen(0)??0,
                            c.Options?.GetMaxLen(int.MaxValue)??int.MaxValue
                            )
                    ),
                    new(typeof(IDictionary),(c,t) =>
                        c.Stream.ReadDict(
                            t,
                            c,
                            c.Options?.GetMinLen(0)??0,
                            c.Options?.GetMaxLen(int.MaxValue)??int.MaxValue
                            )
                    ),
                    new(typeof(Enum),(c,t) => c.Stream.ReadEnum(t,c)),
                    new(typeof(Stream),(c,t) =>
                    {
                        Stream res = c.Options?.Attribute.GetStream(obj:null,property:null,c) ?? new PooledTempStream();
                        try
                        {
                            return c.Stream.ReadStream(res,c,minLen:c.Options?.GetMinLen(0L)??0, maxLen:c.Options?.GetMaxLen(long.MaxValue)??long.MaxValue);
                        }
                        catch
                        {
                            res.Dispose();
                            throw;
                        }
                    }),
                    new(typeof(Type),(c,t) => c.Stream.ReadType(c))
            });
            AsyncDeserializer = new(new KeyValuePair<Type, AsyncDeserializer_Delegate>[]
            {
                    new(typeof(bool),(c,t) => c.Stream.ReadBoolAsync(c)),
                    new(typeof(sbyte),(c,t) => c.Stream.ReadOneSByteAsync(c)),
                    new(typeof(byte),(c,t) => c.Stream.ReadOneByteAsync(c)),
                    new(typeof(short),(c,t) => c.Stream.ReadShortAsync(c)),
                    new(typeof(ushort),(c,t) => c.Stream.ReadUShortAsync(c)),
                    new(typeof(int),(c,t) => c.Stream.ReadIntAsync(c)),
                    new(typeof(uint),(c,t) => c.Stream.ReadUIntAsync(c)),
                    new(typeof(long),(c,t) => c.Stream.ReadLongAsync(c)),
                    new(typeof(ulong),(c,t) => c.Stream.ReadULongAsync(c)),
                    new(typeof(float),(c,t) => c.Stream.ReadFloatAsync(c)),
                    new(typeof(double),(c,t) => c.Stream.ReadDoubleAsync(c)),
                    new(typeof(decimal),(c,t) => c.Stream.ReadDecimalAsync(c)),
                    new(typeof(byte[]),(c,t) =>
                        c.Stream.ReadBytesAsync(c, minLen: c.Options?.GetMinLen(0)??0, maxLen: c.Options?.GetMaxLen(int.MaxValue)??int.MaxValue)
                        ),
                    new(typeof(string),(c,t) =>
                        c.Stream.ReadStringAsync(c, minLen: c.Options?.GetMinLen(0)??0, maxLen: c.Options?.GetMaxLen(int.MaxValue)??int.MaxValue)
                        ),
                    new(typeof(Array),(c,t) =>
                        c.Stream.ReadArrayAsync(
                            t,
                            c,
                            c.Options?.GetMinLen(0)??0,
                            c.Options?.GetMaxLen(int.MaxValue)??int.MaxValue
                            )
                    ),
                    new(typeof(IList),(c,t) =>
                        c.Stream.ReadListAsync(
                            t,
                            c,
                            c.Options?.GetMinLen(0)??0,
                            c.Options?.GetMaxLen(int.MaxValue)??int.MaxValue
                            )
                    ),
                    new(typeof(IDictionary),(c,t) =>
                        c.Stream.ReadDictAsync(
                            t,
                            c,
                            c.Options?.GetMinLen(0)??0,
                            c.Options?.GetMaxLen(int.MaxValue)??int.MaxValue
                            )
                        ),
                    new(typeof(Enum),(c,t) => c.Stream.ReadEnumAsync(t,c)),
                    new(typeof(Stream),(c,t) =>
                    {
                        Stream res = c.Options?.Attribute.GetStream(obj:null,property:null,c) ?? new PooledTempStream();
                        try
                        {
                            return c.Stream.ReadStreamAsync(res,c,minLen:c.Options?.GetMinLen(0L)??0, maxLen:c.Options?.GetMaxLen(long.MaxValue)??long.MaxValue);
                        }
                        catch
                        {
                            res.Dispose();//TODO Should be DisposeAsync
                            throw;
                        }
                    }),
                    new(typeof(Type),(c,t) => c.Stream.ReadTypeAsync(c))
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
                typeof(IDictionary),
                typeof(IList),
                typeof(Stream),
                typeof(Type)
            });
        }

        /// <summary>
        /// Version number (the first 8 bits are used internal, while other bits can be used for customization)
        /// </summary>
        public static int Version { get; set; } = VERSION;

        /// <summary>
        /// Find a serializer
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Serializer</returns>
        public static Serializer_Delegate? FindSerializer(Type type)
        {
            Type? serializer = FindDelegateType(type, SyncSerializer.Keys);
            if (serializer == null || !SyncSerializer.TryGetValue(serializer, out Serializer_Delegate? res))
            {
                SerializerEventArgs<Serializer_Delegate> e = new(type);
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
        public static AsyncSerializer_Delegate? FindAsyncSerializer(Type type)
        {
            Type? serializer = FindDelegateType(type, AsyncSerializer.Keys);
            if (serializer == null || !AsyncSerializer.TryGetValue(serializer, out AsyncSerializer_Delegate? res))
            {
                SerializerEventArgs<AsyncSerializer_Delegate> e = new(type);
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
        public static Deserializer_Delegate? FindDeserializer(Type type)
        {
            Type? serializer = FindDelegateType(type, SyncSerializer.Keys);
            if (serializer == null || !SyncDeserializer.TryGetValue(serializer, out Deserializer_Delegate? res))
            {
                SerializerEventArgs<Deserializer_Delegate> e = new(type);
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
        public static AsyncDeserializer_Delegate? FindAsyncDeserializer(Type type)
        {
            Type? serializer = FindDelegateType(type, AsyncSerializer.Keys);
            if (serializer == null || !AsyncDeserializer.TryGetValue(serializer, out AsyncDeserializer_Delegate? res))
            {
                SerializerEventArgs<AsyncDeserializer_Delegate> e = new(type);
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
    }
}
