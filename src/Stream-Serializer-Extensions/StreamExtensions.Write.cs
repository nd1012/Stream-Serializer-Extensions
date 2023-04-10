using System.Buffers;
using System.Data;
using System.Reflection;
using System.Text;
using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    /// <summary>
    /// Stream extensions
    /// </summary>
    public static partial class StreamExtensions
    {
        /// <summary>
        /// Write object method
        /// </summary>
        public static readonly MethodInfo WriteObjectMethod;
        /// <summary>
        /// Write object method
        /// </summary>
        public static readonly MethodInfo WriteObjectAsyncMethod;
        /// <summary>
        /// Write any object method
        /// </summary>
        public static readonly MethodInfo WriteAnyObjectMethod;
        /// <summary>
        /// Write any object method
        /// </summary>
        public static readonly MethodInfo WriteAnyObjectAsyncMethod;
        /// <summary>
        /// Write number method
        /// </summary>
        public static readonly MethodInfo WriteNumberMethod;
        /// <summary>
        /// Write number method
        /// </summary>
        public static readonly MethodInfo WriteNumberAsyncMethod;
        /// <summary>
        /// Write enumeration method
        /// </summary>
        public static readonly MethodInfo WriteEnumMethod;
        /// <summary>
        /// Write enumeration method
        /// </summary>
        public static readonly MethodInfo WriteEnumAsyncMethod;
        /// <summary>
        /// Write array method
        /// </summary>
        public static readonly MethodInfo WriteArrayMethod;
        /// <summary>
        /// Write array method
        /// </summary>
        public static readonly MethodInfo WriteArrayAsyncMethod;
        /// <summary>
        /// Write list method
        /// </summary>
        public static readonly MethodInfo WriteListMethod;
        /// <summary>
        /// Write list method
        /// </summary>
        public static readonly MethodInfo WriteListAsyncMethod;
        /// <summary>
        /// Write dictionary method
        /// </summary>
        public static readonly MethodInfo WriteDictMethod;
        /// <summary>
        /// Write dictionary method
        /// </summary>
        public static readonly MethodInfo WriteDictAsyncMethod;

        /// <summary>
        /// Constructor
        /// </summary>
        static StreamExtensions()
        {
            Type type = typeof(StreamExtensions);
            WriteObjectMethod = type.GetMethod(nameof(WriteObject), BindingFlags.Static | BindingFlags.Public) ?? throw new TypeLoadException($"Failed to reflect {nameof(WriteObject)}");
            WriteObjectAsyncMethod = type.GetMethod(nameof(WriteObjectAsync), BindingFlags.Static | BindingFlags.Public) ?? throw new TypeLoadException($"Failed to reflect {nameof(WriteObjectAsync)}");
            WriteAnyObjectMethod = type.GetMethod(nameof(WriteAnyObject), BindingFlags.Static | BindingFlags.Public) ?? throw new TypeLoadException($"Failed to reflect {nameof(WriteAnyObject)}");
            WriteAnyObjectAsyncMethod = type.GetMethod(nameof(WriteAnyObjectAsync), BindingFlags.Static | BindingFlags.Public) ?? throw new TypeLoadException($"Failed to reflect {nameof(WriteAnyObjectAsync)}");
            WriteNumberMethod = type.GetMethod(nameof(WriteNumber), BindingFlags.Static | BindingFlags.Public) ?? throw new TypeLoadException($"Failed to reflect {nameof(WriteNumber)}");
            WriteNumberAsyncMethod = type.GetMethod(nameof(WriteNumberAsync), BindingFlags.Static | BindingFlags.Public) ?? throw new TypeLoadException($"Failed to reflect {nameof(WriteNumberAsync)}");
            WriteEnumMethod = type.GetMethod(nameof(WriteEnum), BindingFlags.Static | BindingFlags.Public) ?? throw new TypeLoadException($"Failed to reflect {nameof(WriteEnum)}");
            WriteEnumAsyncMethod = type.GetMethod(nameof(WriteEnumAsync), BindingFlags.Static | BindingFlags.Public) ?? throw new TypeLoadException($"Failed to reflect {nameof(WriteEnumAsync)}");
            WriteArrayMethod = type.GetMethod(nameof(WriteArray), BindingFlags.Static | BindingFlags.Public) ?? throw new TypeLoadException($"Failed to reflect {nameof(WriteArray)}");
            WriteArrayAsyncMethod = type.GetMethod(nameof(WriteArrayAsync), BindingFlags.Static | BindingFlags.Public) ?? throw new TypeLoadException($"Failed to reflect {nameof(WriteArrayAsync)}");
            WriteListMethod = type.GetMethod(nameof(WriteList), BindingFlags.Static | BindingFlags.Public) ?? throw new TypeLoadException($"Failed to reflect {nameof(WriteList)}");
            WriteListAsyncMethod = type.GetMethod(nameof(WriteListAsync), BindingFlags.Static | BindingFlags.Public) ?? throw new TypeLoadException($"Failed to reflect {nameof(WriteListAsync)}");
            WriteDictMethod = type.GetMethod(nameof(WriteDict), BindingFlags.Static | BindingFlags.Public) ?? throw new TypeLoadException($"Failed to reflect {nameof(WriteDict)}");
            WriteDictAsyncMethod = type.GetMethod(nameof(WriteDictAsync), BindingFlags.Static | BindingFlags.Public) ?? throw new TypeLoadException($"Failed to reflect {nameof(WriteDictAsync)}");
            ReadObjectMethod = type.GetMethod(nameof(ReadObject), BindingFlags.Static | BindingFlags.Public) ?? throw new TypeLoadException($"Failed to reflect {nameof(ReadObject)}");
            ReadObjectAsyncMethod = type.GetMethod(nameof(ReadObjectAsync), BindingFlags.Static | BindingFlags.Public) ?? throw new TypeLoadException($"Failed to reflect {nameof(ReadObjectAsync)}");
            ReadAnyMethod = type.GetMethod(nameof(ReadAny), BindingFlags.Static | BindingFlags.Public) ?? throw new TypeLoadException($"Failed to reflect {nameof(ReadAny)}");
            ReadAnyAsyncMethod = type.GetMethod(nameof(ReadAnyAsync), BindingFlags.Static | BindingFlags.Public) ?? throw new TypeLoadException($"Failed to reflect {nameof(ReadAnyAsync)}");
            ReadAnyNullableMethod = type.GetMethod(nameof(ReadAnyNullable), BindingFlags.Static | BindingFlags.Public) ?? throw new TypeLoadException($"Failed to reflect {nameof(ReadAnyNullable)}");
            ReadAnyNullableAsyncMethod = type.GetMethod(nameof(ReadAnyNullableAsync), BindingFlags.Static | BindingFlags.Public) ?? throw new TypeLoadException($"Failed to reflect {nameof(ReadAnyNullableAsync)}");
            ReadSerializedMethod = type.GetMethod(nameof(ReadSerialized), BindingFlags.Static | BindingFlags.Public) ?? throw new TypeLoadException($"Failed to reflect {nameof(ReadSerialized)}");
            ReadSerializedAsyncMethod = type.GetMethod(nameof(ReadSerializedAsync), BindingFlags.Static | BindingFlags.Public) ?? throw new TypeLoadException($"Failed to reflect {nameof(ReadSerializedAsync)}");
            ReadNumberMethod = type.GetMethod(nameof(ReadNumber), BindingFlags.Static | BindingFlags.Public) ?? throw new TypeLoadException($"Failed to reflect {nameof(ReadNumber)}");
            ReadNumberAsyncMethod = type.GetMethod(nameof(ReadNumberAsync), BindingFlags.Static | BindingFlags.Public) ?? throw new TypeLoadException($"Failed to reflect {nameof(ReadNumberAsync)}");
            ReadEnumMethod = type.GetMethod(nameof(ReadEnum), BindingFlags.Static | BindingFlags.Public) ?? throw new TypeLoadException($"Failed to reflect {nameof(ReadEnum)}");
            ReadEnumAsyncMethod = type.GetMethod(nameof(ReadEnumAsync), BindingFlags.Static | BindingFlags.Public) ?? throw new TypeLoadException($"Failed to reflect {nameof(ReadEnumAsync)}");
            ReadArrayMethod = type.GetMethod(nameof(ReadArray), BindingFlags.Static | BindingFlags.Public) ?? throw new TypeLoadException($"Failed to reflect {nameof(ReadArray)}");
            ReadArrayAsyncMethod = type.GetMethod(nameof(ReadArrayAsync), BindingFlags.Static | BindingFlags.Public) ?? throw new TypeLoadException($"Failed to reflect {nameof(ReadArrayAsync)}");
            ReadListMethod = type.GetMethod(nameof(ReadList), BindingFlags.Static | BindingFlags.Public) ?? throw new TypeLoadException($"Failed to reflect {nameof(ReadList)}");
            ReadListAsyncMethod = type.GetMethod(nameof(ReadListAsync), BindingFlags.Static | BindingFlags.Public) ?? throw new TypeLoadException($"Failed to reflect {nameof(ReadListAsync)}");
            ReadDictMethod = type.GetMethod(nameof(ReadDict), BindingFlags.Static | BindingFlags.Public) ?? throw new TypeLoadException($"Failed to reflect {nameof(ReadDict)}");
            ReadDictAsyncMethod = type.GetMethod(nameof(ReadDictAsync), BindingFlags.Static | BindingFlags.Public) ?? throw new TypeLoadException($"Failed to reflect {nameof(ReadDictAsync)}");
            ArrayEmptyMethod = typeof(Array).GetMethod(nameof(Array.Empty), BindingFlags.Static | BindingFlags.Public)!;
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="tStream">Stream type</typeparam>
        /// <typeparam name="tObj">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object to write</param>
        /// <returns>Stream</returns>
        public static tStream WriteObject<tStream, tObj>(this tStream stream, tObj obj) where tStream : Stream
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (typeof(IStreamSerializer).IsAssignableFrom(obj.GetType())) return WriteSerialized(stream, (IStreamSerializer)obj);
            if (StreamSerializer.FindSerializer(obj.GetType()) is not StreamSerializer.Serialize_Delegate serializer)
            {
                WriteAnyObjectMethod.MakeGenericMethod(typeof(tStream), typeof(tObj)).InvokeAuto(obj: null, stream, obj);
                return stream;
            }
            try
            {
                serializer(stream, obj);
            }
            catch (SerializerException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
            return stream;
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static async Task WriteObjectAsync<T>(this Stream stream, T obj, CancellationToken cancellationToken = default)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (typeof(IStreamSerializer).IsAssignableFrom(obj.GetType()))
            {
                await WriteSerializedAsync(stream, (IStreamSerializer)obj, cancellationToken).DynamicContext();
                return;
            }
            if (StreamSerializer.FindAsyncSerializer(obj.GetType()) is not StreamSerializer.AsyncSerialize_Delegate serializer)
            {
                Task task = (Task)WriteAnyObjectAsyncMethod.MakeGenericMethod(typeof(T)).InvokeAuto(obj: null, stream, obj, cancellationToken)!;
                await task.DynamicContext();
                return;
            }
            try
            {
                await serializer(stream, obj, cancellationToken).DynamicContext();
            }
            catch (SerializerException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="tStream">Stream type</typeparam>
        /// <typeparam name="tObj">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object to write</param>
        /// <returns>Stream</returns>
        public static tStream WriteObjectNullable<tStream, tObj>(this tStream stream, tObj? obj) where tStream : Stream
        {
            Write(stream, obj != null);
            if (obj != null) WriteObject(stream, obj);
            return stream;
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static async Task WriteObjectNullableAsync<T>(this Stream stream, T? obj, CancellationToken cancellationToken = default)
        {
            await WriteAsync(stream, obj != null, cancellationToken).DynamicContext();
            if (obj != null) await WriteObjectAsync(stream, obj, cancellationToken).DynamicContext();
        }

        /// <summary>
        /// Write any object
        /// </summary>
        /// <typeparam name="T">Stream type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <returns>Stream</returns>
        public static T WriteAny<T>(this T stream, object obj) where T : Stream
        {
            try
            {
                (Type type, ObjectTypes objType, bool writeType, bool writeObject) = obj.GetObjectSerializerInfo();
                using(RentedArray<byte> poolData = new(1))
                {
                    poolData[0] = (byte)objType;
                    stream.Write(poolData.Span);
                }
                if (writeType) WriteString(stream, type.ToString());
                if (writeObject)
                    if (objType.IsNumber())
                    {
                        WriteNumberMethod.MakeGenericMethod(typeof(T), type).InvokeAuto(obj: null, stream, obj);
                    }
                    else
                    {
                        WriteObjectMethod.MakeGenericMethod(typeof(T), type).InvokeAuto(obj: null, stream, obj);
                    }
                return stream;
            }
            catch (SerializerException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Write any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static async Task WriteAnyAsync(this Stream stream, object obj, CancellationToken cancellationToken = default)
        {
            try
            {
                (Type type, ObjectTypes objType, bool writeType, bool writeObject) = obj.GetObjectSerializerInfo();
                using (RentedArray<byte> poolData = new(1))
                {
                    poolData[0] = (byte)objType;
                    await stream.WriteAsync(poolData.Memory, cancellationToken).DynamicContext();
                }
                if (writeType) await WriteStringAsync(stream, type.ToString(), cancellationToken).DynamicContext();
                if (writeObject)
                {
                    Task task;
                    if (objType.IsNumber())
                    {
                        task = (Task)WriteNumberAsyncMethod.MakeGenericMethod(type).InvokeAuto(obj: null, stream, obj, cancellationToken)!;
                    }
                    else
                    {
                        task = (Task)WriteObjectAsyncMethod.MakeGenericMethod(type).InvokeAuto(obj: null, stream, obj, cancellationToken)!;
                    }
                    await task.DynamicContext();
                }
            }
            catch (SerializerException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Write any object
        /// </summary>
        /// <typeparam name="T">Stream type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <returns>Stream</returns>
        public static T WriteAnyNullable<T>(this T stream, object? obj) where T : Stream
        {
            try
            {
                if (obj == null)
                {
                    using (RentedArray<byte> poolData = new(1))
                    {
                        poolData[0] = (byte)ObjectTypes.Null;
                        stream.Write(poolData.Span);
                    }
                }
                else
                {
                    WriteAny(stream, obj);
                }
                return stream;
            }
            catch (SerializerException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Write any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static async Task WriteAnyNullableAsync(this Stream stream, object? obj, CancellationToken cancellationToken = default)
        {
            try
            {
                if (obj == null)
                {
                    using (RentedArray<byte> poolData = new(1))
                    {
                        poolData[0] = (byte)ObjectTypes.Null;
                        await stream.WriteAsync(poolData.Memory, cancellationToken).DynamicContext();
                    }
                }
                else
                {
                    await WriteAnyAsync(stream, obj, cancellationToken).DynamicContext();
                }
            }
            catch (SerializerException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Stream type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        public static T Write<T>(this T stream, bool value) where T : Stream
        {
            using (RentedArray<byte> poolData = new(1))
            {
                poolData[0] = (byte)(value ? 1 : 0);
                stream.Write(poolData.Span);
            }
            return stream;
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static async Task WriteAsync(this Stream stream, bool value, CancellationToken cancellationToken = default)
        {
            using (RentedArray<byte> poolData = new(1))
            {
                poolData[0] = (byte)(value ? 1 : 0);
                await stream.WriteAsync(poolData.Memory, cancellationToken).DynamicContext();
            }
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Stream type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        public static T WriteNullable<T>(this T stream, bool? value) where T : Stream
        {
            Write(stream, value != null);
            if (value != null) Write(stream, value.Value);
            return stream;
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static async Task WriteNullableAsync(this Stream stream, bool? value, CancellationToken cancellationToken = default)
        {
            await WriteAsync(stream, value != null, cancellationToken).DynamicContext();
            if (value != null) await WriteAsync(stream, value.Value, cancellationToken).DynamicContext();
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Stream type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        public static T Write<T>(this T stream, sbyte value) where T : Stream
        {
            try
            {
                stream.Write((byte)value);
                return stream;
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static async Task WriteAsync(this Stream stream, sbyte value, CancellationToken cancellationToken = default)
        {
            try
            {
                await stream.WriteAsync((byte)value, cancellationToken).DynamicContext();
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Stream type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        public static T WriteNullable<T>(this T stream, sbyte? value) where T : Stream
        {
            Write(stream, value != null);
            if (value != null) Write(stream, value.Value);
            return stream;
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static async Task WriteNullableAsync(this Stream stream, sbyte? value, CancellationToken cancellationToken = default)
        {
            await WriteAsync(stream, value != null, cancellationToken).DynamicContext();
            if (value != null) await WriteAsync(stream, value.Value, cancellationToken).DynamicContext();
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Stream type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        public static T Write<T>(this T stream, byte value) where T : Stream
        {
            try
            {
                stream.WriteByte(value);
                return stream;
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static async Task WriteAsync(this Stream stream, byte value, CancellationToken cancellationToken = default)
        {
            await Task.Yield();
            Write(stream, value);
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Stream type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        public static T WriteNullable<T>(this T stream, byte? value) where T : Stream
        {
            Write(stream, value != null);
            if (value != null) Write(stream, value.Value);
            return stream;
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static async Task WriteNullableAsync(this Stream stream, byte? value, CancellationToken cancellationToken = default)
        {
            await WriteAsync(stream, value != null, cancellationToken).DynamicContext();
            if (value != null) await WriteAsync(stream, value.Value, cancellationToken).DynamicContext();
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Stream type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        public static T Write<T>(this T stream, short value) where T : Stream
        {
            try
            {
                stream.Write(value.GetBytes());
                return stream;
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static async Task WriteAsync(this Stream stream, short value, CancellationToken cancellationToken = default)
        {
            try
            {
                await stream.WriteAsync(value.GetBytes(), cancellationToken).DynamicContext();
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Stream type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        public static T WriteNullable<T>(this T stream, short? value) where T : Stream
        {
            Write(stream, value != null);
            if (value != null) Write(stream, value.Value);
            return stream;
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static async Task WriteNullableAsync(this Stream stream, short? value, CancellationToken cancellationToken = default)
        {
            await WriteAsync(stream, value != null, cancellationToken).DynamicContext();
            if (value != null) await WriteAsync(stream, value.Value, cancellationToken).DynamicContext();
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Stream type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        public static T Write<T>(this T stream, ushort value) where T : Stream
        {
            try
            {
                stream.Write(value.GetBytes());
                return stream;
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static async Task WriteAsync(this Stream stream, ushort value, CancellationToken cancellationToken = default)
        {
            try
            {
                await stream.WriteAsync(value.GetBytes(), cancellationToken).DynamicContext();
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Stream type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        public static T WriteNullable<T>(this T stream, ushort? value) where T : Stream
        {
            Write(stream, value != null);
            if (value != null) Write(stream, value.Value);
            return stream;
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static async Task WriteNullableAsync(this Stream stream, ushort? value, CancellationToken cancellationToken = default)
        {
            await WriteAsync(stream, value != null, cancellationToken).DynamicContext();
            if (value != null) await WriteAsync(stream, value.Value, cancellationToken).DynamicContext();
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Stream type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        public static T Write<T>(this T stream, int value) where T : Stream
        {
            try
            {
                stream.Write(value.GetBytes());
                return stream;
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static async Task WriteAsync(this Stream stream, int value, CancellationToken cancellationToken = default)
        {
            try
            {
                await stream.WriteAsync(value.GetBytes(), cancellationToken).DynamicContext();
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Stream type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        public static T WriteNullable<T>(this T stream, int? value) where T : Stream
        {
            Write(stream, value != null);
            if (value != null) Write(stream, value.Value);
            return stream;
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static async Task WriteNullableAsync(this Stream stream, int? value, CancellationToken cancellationToken = default)
        {
            await WriteAsync(stream, value != null, cancellationToken).DynamicContext();
            if (value != null) await WriteAsync(stream, value.Value, cancellationToken).DynamicContext();
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Stream type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        public static T Write<T>(this T stream, uint value) where T : Stream
        {
            try
            {
                stream.Write(value.GetBytes());
                return stream;
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static async Task WriteAsync(this Stream stream, uint value, CancellationToken cancellationToken = default)
        {
            try
            {
                await stream.WriteAsync(value.GetBytes(), cancellationToken).DynamicContext();
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Stream type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        public static T WriteNullable<T>(this T stream, uint? value) where T : Stream
        {
            Write(stream, value != null);
            if (value != null) Write(stream, value.Value);
            return stream;
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static async Task WriteNullableAsync(this Stream stream, uint? value, CancellationToken cancellationToken = default)
        {
            await WriteAsync(stream, value != null, cancellationToken).DynamicContext();
            if (value != null) await WriteAsync(stream, value.Value, cancellationToken).DynamicContext();
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Stream type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        public static T Write<T>(this T stream, long value) where T : Stream
        {
            try
            {
                stream.Write(value.GetBytes());
                return stream;
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static async Task WriteAsync(this Stream stream, long value, CancellationToken cancellationToken = default)
        {
            try
            {
                await stream.WriteAsync(value.GetBytes(), cancellationToken).DynamicContext();
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Stream type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        public static T WriteNullable<T>(this T stream, long? value) where T : Stream
        {
            Write(stream, value != null);
            if (value != null) Write(stream, value.Value);
            return stream;
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static async Task WriteNullableAsync(this Stream stream, long? value, CancellationToken cancellationToken = default)
        {
            await WriteAsync(stream, value != null, cancellationToken).DynamicContext();
            if (value != null) await WriteAsync(stream, value.Value, cancellationToken).DynamicContext();
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Stream type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        public static T Write<T>(this T stream, ulong value) where T : Stream
        {
            try
            {
                stream.Write(value.GetBytes());
                return stream;
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static async Task WriteAsync(this Stream stream, ulong value, CancellationToken cancellationToken = default)
        {
            try
            {
                await stream.WriteAsync(value.GetBytes(), cancellationToken).DynamicContext();
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Stream type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        public static T WriteNullable<T>(this T stream, ulong? value) where T : Stream
        {
            Write(stream, value != null);
            if (value != null) Write(stream, value.Value);
            return stream;
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static async Task WriteNullableAsync(this Stream stream, ulong? value, CancellationToken cancellationToken = default)
        {
            await WriteAsync(stream, value != null, cancellationToken).DynamicContext();
            if (value != null) await WriteAsync(stream, value.Value, cancellationToken).DynamicContext();
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Stream type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        public static T Write<T>(this T stream, float value) where T : Stream
        {
            try
            {
                stream.Write(value.GetBytes());
                return stream;
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static async Task WriteAsync(this Stream stream, float value, CancellationToken cancellationToken = default)
        {
            try
            {
                await stream.WriteAsync(value.GetBytes(), cancellationToken).DynamicContext();
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Stream type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        public static T WriteNullable<T>(this T stream, float? value) where T : Stream
        {
            Write(stream, value != null);
            if (value != null) Write(stream, value.Value);
            return stream;
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static async Task WriteNullableAsync(this Stream stream, float? value, CancellationToken cancellationToken = default)
        {
            await WriteAsync(stream, value != null, cancellationToken).DynamicContext();
            if (value != null) await WriteAsync(stream, value.Value, cancellationToken).DynamicContext();
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Stream type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        public static T Write<T>(this T stream, double value) where T : Stream
        {
            try
            {
                stream.Write(value.GetBytes());
                return stream;
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static async Task WriteAsync(this Stream stream, double value, CancellationToken cancellationToken = default)
        {
            try
            {
                await stream.WriteAsync(value.GetBytes(), cancellationToken).DynamicContext();
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Stream type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        public static T WriteNullable<T>(this T stream, double? value) where T : Stream
        {
            Write(stream, value != null);
            if (value != null) Write(stream, value.Value);
            return stream;
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static async Task WriteNullableAsync(this Stream stream, double? value, CancellationToken cancellationToken = default)
        {
            await WriteAsync(stream, value != null, cancellationToken).DynamicContext();
            if (value != null) await WriteAsync(stream, value.Value, cancellationToken).DynamicContext();
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="tStream">Stream type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        public static tStream Write<tStream>(this tStream stream, decimal value) where tStream : Stream
        {
            try
            {
                stream.Write(value.GetBytes());
                return stream;
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static async Task WriteAsync(this Stream stream, decimal value, CancellationToken cancellationToken = default)
        {
            try
            {
                await stream.WriteAsync(value.GetBytes(), cancellationToken).DynamicContext();
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Stream type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        public static T WriteNullable<T>(this T stream, decimal? value) where T : Stream
        {
            Write(stream, value != null);
            if (value != null) Write(stream, value.Value);
            return stream;
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static async Task WriteNullableAsync(this Stream stream, decimal? value, CancellationToken cancellationToken = default)
        {
            await WriteAsync(stream, value != null, cancellationToken).DynamicContext();
            if (value != null) await WriteAsync(stream, value.Value, cancellationToken).DynamicContext();
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="tStream">Stream type</typeparam>
        /// <typeparam name="tNumber">Number type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        public static tStream WriteNumber<tStream, tNumber>(this tStream stream, tNumber value)
            where tStream : Stream
            where tNumber : struct, IConvertible
        {
            (object number, NumberTypes type) = value.GetNumberAndType();
            using RentedArray<byte> poolData = new(1);
            poolData[0] = (byte)type;
            stream.Write(poolData.Span);
            if(!type.IsZero() && !type.HasValueFlags())
                switch (type)
                {
                    case NumberTypes.Byte:
                    case NumberTypes.Byte | NumberTypes.Unsigned:
                        poolData[0] = number.ConvertType<byte>();
                        stream.Write(poolData.Span);
                        break;
                    case NumberTypes.Short:
                        Write(stream, number.ConvertType<short>());
                        break;
                    case NumberTypes.Short | NumberTypes.Unsigned:
                        Write(stream, number.ConvertType<ushort>());
                        break;
                    case NumberTypes.Int:
                        Write(stream, number.ConvertType<int>());
                        break;
                    case NumberTypes.Int | NumberTypes.Unsigned:
                        Write(stream, number.ConvertType<uint>());
                        break;
                    case NumberTypes.Long:
                        Write(stream, number.ConvertType<long>());
                        break;
                    case NumberTypes.Long | NumberTypes.Unsigned:
                        Write(stream, number.ConvertType<ulong>());
                        break;
                    case NumberTypes.Float:
                        Write(stream, number.ConvertType<float>());
                        break;
                    case NumberTypes.Double:
                        Write(stream, number.ConvertType<double>());
                        break;
                    case NumberTypes.Decimal:
                        Write(stream, number.ConvertType<decimal>());
                        break;
                }
            return stream;
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Number type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static async Task WriteNumberAsync<T>(this Stream stream, T value, CancellationToken cancellationToken = default)
            where T : struct, IConvertible
        {
            (object number, NumberTypes type) = value.GetNumberAndType();
            using RentedArray<byte> poolData = new(1);
            poolData[0] = (byte)type;
            await stream.WriteAsync(poolData.Memory, cancellationToken).DynamicContext();
            if (!type.IsZero() && !type.HasValueFlags())
                switch (type)
                {
                    case NumberTypes.Byte:
                    case NumberTypes.Byte | NumberTypes.Unsigned:
                        poolData[0] = number.ConvertType<byte>();
                        await stream.WriteAsync(poolData.Memory, cancellationToken).DynamicContext();
                        break;
                    case NumberTypes.Short:
                        await WriteAsync(stream, number.ConvertType<short>(), cancellationToken).DynamicContext();
                        break;
                    case NumberTypes.Short | NumberTypes.Unsigned:
                        await WriteAsync(stream, number.ConvertType<ushort>(), cancellationToken).DynamicContext();
                        break;
                    case NumberTypes.Int:
                        await WriteAsync(stream, number.ConvertType<int>(), cancellationToken).DynamicContext();
                        break;
                    case NumberTypes.Int | NumberTypes.Unsigned:
                        await WriteAsync(stream, number.ConvertType<uint>(), cancellationToken).DynamicContext();
                        break;
                    case NumberTypes.Long:
                        await WriteAsync(stream, number.ConvertType<long>(), cancellationToken).DynamicContext();
                        break;
                    case NumberTypes.Long | NumberTypes.Unsigned:
                        await WriteAsync(stream, number.ConvertType<ulong>(), cancellationToken).DynamicContext();
                        break;
                    case NumberTypes.Float:
                        await WriteAsync(stream, number.ConvertType<float>(), cancellationToken).DynamicContext();
                        break;
                    case NumberTypes.Double:
                        await WriteAsync(stream, number.ConvertType<double>(), cancellationToken).DynamicContext();
                        break;
                    case NumberTypes.Decimal:
                        await WriteAsync(stream, number.ConvertType<decimal>(), cancellationToken).DynamicContext();
                        break;
                }
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="tStream">Stream type</typeparam>
        /// <typeparam name="tNumber">Number type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        public static tStream WriteNumberNullable<tStream, tNumber>(this tStream stream, tNumber? value)
            where tStream : Stream
            where tNumber : struct, IConvertible
        {
            Write(stream, value != null);
            if (value != null) WriteNumber(stream, value.Value);
            return stream;
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Number type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static async Task WriteNumberNullableAsync<T>(this Stream stream, T? value, CancellationToken cancellationToken = default)
            where T : struct, IConvertible
        {
            await WriteAsync(stream, value != null, cancellationToken).DynamicContext();
            if (value != null) await WriteNumberAsync(stream, value.Value, cancellationToken).DynamicContext();
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="tStream">Stream type</typeparam>
        /// <typeparam name="tEnum">Enumeration type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        public static tStream WriteEnum<tStream, tEnum>(this tStream stream, tEnum value)
            where tStream : Stream
            where tEnum : struct, Enum
        {
            try
            {
                Type type = typeof(tEnum).GetEnumUnderlyingType();
                WriteNumberMethod.MakeGenericMethod(typeof(tStream), type).InvokeAuto(obj: null, stream, Convert.ChangeType(value, type));
                return stream;
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Enumeration type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static async Task WriteEnumAsync<T>(this Stream stream, T value, CancellationToken cancellationToken = default)
            where T : struct, Enum
        {
            try
            {
                Type type = typeof(T).GetEnumUnderlyingType();
                Task task = (Task)WriteNumberAsyncMethod.MakeGenericMethod(type).InvokeAuto(obj: null, stream, Convert.ChangeType(value, type), cancellationToken)!;
                await task.DynamicContext();
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="tStream">Stream type</typeparam>
        /// <typeparam name="tEnum">Enumeration type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        public static tStream WriteEnumNullable<tStream, tEnum>(this tStream stream, tEnum? value)
            where tStream : Stream
            where tEnum : struct, Enum
        {
            Write(stream, value != null);
            if (value != null) WriteEnum(stream, value.Value);
            return stream;
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Enumeration type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static async Task WriteEnumNullableAsync<T>(this Stream stream, T? value, CancellationToken cancellationToken = default)
            where T : struct, Enum
        {
            await WriteAsync(stream, value != null, cancellationToken).DynamicContext();
            if (value != null) await WriteEnumAsync(stream, value.Value, cancellationToken).DynamicContext();
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Stream type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        public static T WriteBytes<T>(this T stream, Span<byte> value) where T : Stream
        {
            try
            {
                WriteNumber(stream, value.Length);
                if (value.Length > 0) stream.Write(value);
                return stream;
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static async Task WriteBytesAsync(this Stream stream, Memory<byte> value, CancellationToken cancellationToken = default)
        {
            try
            {
                await WriteNumberAsync(stream, value.Length, cancellationToken).DynamicContext();
                if (value.Length > 0) await stream.WriteAsync(value, cancellationToken).DynamicContext();
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Stream type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        public static T WriteBytesNullable<T>(this T stream, byte[]? value) where T : Stream
        {
            Write(stream, value != null);
            if (value != null) WriteBytes(stream, value);
            return stream;
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static async Task WriteBytesNullableAsync(this Stream stream, byte[]? value, CancellationToken cancellationToken = default)
        {
            await WriteAsync(stream, value != null, cancellationToken).DynamicContext();
            if (value != null) await WriteBytesAsync(stream, value, cancellationToken).DynamicContext();
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Stream type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        public static T WriteString<T>(this T stream, string value) where T : Stream
        {
            try
            {
                WriteBytes(stream, Encoding.UTF8.GetBytes(value));
                return stream;
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static async Task WriteStringAsync(this Stream stream, string value, CancellationToken cancellationToken = default)
        {
            try
            {
                await WriteBytesAsync(stream, Encoding.UTF8.GetBytes(value), cancellationToken).DynamicContext();
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Stream type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        public static T WriteStringNullable<T>(this T stream, string? value) where T : Stream
        {
            Write(stream, value != null);
            if (value != null) WriteString(stream, value);
            return stream;
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static async Task WriteStringNullableAsync(this Stream stream, string? value, CancellationToken cancellationToken = default)
        {
            await WriteAsync(stream, value != null, cancellationToken).DynamicContext();
            if (value != null) await WriteStringAsync(stream, value, cancellationToken).DynamicContext();
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="tStream">Stream type</typeparam>
        /// <typeparam name="tElement">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        public static tStream WriteArray<tStream, tElement>(this tStream stream, tElement[] value) where tStream : Stream
        {
            if (typeof(tElement) == typeof(byte)) return WriteBytes(stream, (value as byte[])!);
            try
            {
                WriteNumber(stream, value.Length);
                if (value.Length == 0) return stream;
                foreach (tElement element in value) WriteObject(stream, element);
                return stream;
            }
            catch (SerializerException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static async Task WriteArrayAsync<T>(this Stream stream, T[] value, CancellationToken cancellationToken = default)
        {
            if (typeof(T) == typeof(byte))
            {
                await WriteBytesAsync(stream, (value as byte[])!, cancellationToken).DynamicContext();
                return;
            }
            try
            {
                await WriteNumberAsync(stream, value.Length, cancellationToken).DynamicContext();
                if (value.Length == 0) return;
                foreach (T element in value) await WriteObjectAsync(stream, element, cancellationToken).DynamicContext();
            }
            catch (SerializerException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="tStream">Stream type</typeparam>
        /// <typeparam name="tElement">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        public static tStream WriteArrayNullable<tStream, tElement>(this tStream stream, tElement[]? value) where tStream : Stream
        {
            Write(stream, value != null);
            if (value != null) WriteArray(stream, value);
            return stream;
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static async Task WriteArrayNullableAsync<T>(this Stream stream, T[]? value, CancellationToken cancellationToken = default)
        {
            await WriteAsync(stream, value != null, cancellationToken).DynamicContext();
            if (value != null) await WriteArrayAsync(stream, value, cancellationToken).DynamicContext();
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="tStream">Stream type</typeparam>
        /// <typeparam name="tElement">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        public static tStream WriteList<tStream, tElement>(this tStream stream, List<tElement> value) where tStream : Stream
        {
            if (typeof(tElement) == typeof(byte)) return WriteBytes(stream, (value as byte[])!);
            try
            {
                WriteNumber(stream, value.Count);
                if (value.Count == 0) return stream;
                foreach (tElement element in value) WriteObject(stream, element);
                return stream;
            }
            catch (SerializerException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static async Task WriteListAsync<T>(this Stream stream, List<T> value, CancellationToken cancellationToken = default)
        {
            if (typeof(T) == typeof(byte))
            {
                await WriteBytesAsync(stream, (value as byte[])!, cancellationToken).DynamicContext();
                return;
            }
            try
            {
                await WriteNumberAsync(stream, value.Count, cancellationToken).DynamicContext();
                if (value.Count == 0) return;
                foreach (T element in value) await WriteObjectAsync(stream, element, cancellationToken).DynamicContext();
            }
            catch (SerializerException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="tStream">Stream type</typeparam>
        /// <typeparam name="tElement">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        public static tStream WriteListNullable<tStream, tElement>(this tStream stream, List<tElement>? value) where tStream : Stream
        {
            Write(stream, value != null);
            if (value != null) WriteList(stream, value);
            return stream;
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static async Task WriteListNullableAsync<T>(this Stream stream, List<T>? value, CancellationToken cancellationToken = default)
        {
            await WriteAsync(stream, value != null, cancellationToken).DynamicContext();
            if (value != null) await WriteListAsync(stream, value, cancellationToken).DynamicContext();
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="tStream">Stream type</typeparam>
        /// <typeparam name="tKey">Key type</typeparam>
        /// <typeparam name="tValue">Value type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        public static tStream WriteDict<tStream, tKey, tValue>(this tStream stream, Dictionary<tKey, tValue> value)
            where tStream : Stream
            where tKey : notnull
        {
            try
            {
                WriteNumber(stream, value.Count);
                if (value.Count == 0) return stream;
                foreach (KeyValuePair<tKey, tValue> kvp in value)
                {
                    WriteObject(stream, kvp.Key);
                    WriteObject(stream, kvp.Value);
                }
                return stream;
            }
            catch (SerializerException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="tKey">Key type</typeparam>
        /// <typeparam name="tValue">Value type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static async Task WriteDictAsync<tKey, tValue>(this Stream stream, Dictionary<tKey, tValue> value, CancellationToken cancellationToken = default)
            where tKey : notnull
        {
            try
            {
                await WriteNumberAsync(stream, value.Count, cancellationToken).DynamicContext();
                if (value.Count == 0) return;
                foreach (KeyValuePair<tKey, tValue> kvp in value)
                {
                    await WriteObjectAsync(stream, kvp.Key, cancellationToken).DynamicContext();
                    await WriteObjectAsync(stream, kvp.Value, cancellationToken).DynamicContext();
                }
            }
            catch (SerializerException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SerializerException(message: null, ex);
            }
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="tStream">Stream type</typeparam>
        /// <typeparam name="tKey">Key type</typeparam>
        /// <typeparam name="tValue">Value type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <returns>Stream</returns>
        public static tStream WriteDictNullable<tStream, tKey, tValue>(this tStream stream, Dictionary<tKey, tValue>? value)
            where tStream : Stream
            where tKey : notnull
        {
            Write(stream, value != null);
            if (value != null) WriteDict(stream, value);
            return stream;
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="tKey">Key type</typeparam>
        /// <typeparam name="tValue">Value type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static async Task WriteDictNullableAsync<tKey, tValue>(this Stream stream, Dictionary<tKey, tValue>? value, CancellationToken cancellationToken = default)
            where tKey : notnull
        {
            await WriteAsync(stream, value != null, cancellationToken).DynamicContext();
            if (value != null) await WriteDictAsync(stream, value, cancellationToken).DynamicContext();
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Stream type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <returns>Stream</returns>
        public static T WriteSerialized<T>(this T stream, IStreamSerializer obj) where T : Stream
        {
            obj.Serialize(stream);
            return stream;
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static async Task WriteSerializedAsync(this Stream stream, IStreamSerializer obj, CancellationToken cancellationToken = default)
            => await obj.SerializeAsync(stream, cancellationToken).DynamicContext();

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Stream type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <returns>Stream</returns>
        public static T WriteSerializedNullable<T>(this T stream, IStreamSerializer? obj) where T : Stream
        {
            Write(stream, obj != null);
            obj?.Serialize(stream);
            return stream;
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static async Task WriteSerializedNullableAsync(this Stream stream, IStreamSerializer? obj, CancellationToken cancellationToken = default)
        {
            await WriteAsync(stream, obj != null, cancellationToken).DynamicContext();
            if (obj != null) await obj.SerializeAsync(stream, cancellationToken).DynamicContext();
        }

        /// <summary>
        /// Write any object
        /// </summary>
        /// <typeparam name="tStream">Stream type</typeparam>
        /// <typeparam name="tObj">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <returns>Stream</returns>
        public static tStream WriteAnyObject<tStream, tObj>(this tStream stream, tObj obj)
            where tStream : Stream
            where tObj : class, new()
        {
            if (obj is IStreamSerializer serializable) return WriteSerialized(stream, serializable);
            Type type = obj.GetType();
            PropertyInfo[] pis = StreamSerializerAttribute.GetWriteProperties(type).ToArray();
            StreamSerializerAttribute? attr = type.GetCustomAttribute<StreamSerializerAttribute>(),
                objAttr;
            bool useChecksum = !(attr?.SkipPropertyNameChecksum ?? false);
            WriteNumberNullable(stream, attr?.Version);
            WriteNumber(stream, pis.Length);
            foreach (PropertyInfo pi in pis)
            {
                objAttr = pi.GetCustomAttribute<StreamSerializerAttribute>();
                if (useChecksum && !(objAttr?.SkipPropertyNameChecksum ?? false))
                    Write(stream, Encoding.UTF8.GetBytes(pi.Name).Aggregate((c, b) => (byte)(c ^ b)));
                if (Nullable.GetUnderlyingType(pi.PropertyType) == null)
                {
                    WriteAny(stream, pi.GetValue(obj)!);
                }
                else
                {
                    WriteAnyNullable(stream, pi.GetValue(obj));
                }
            }
            return stream;
        }

        /// <summary>
        /// Write any object
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static async Task WriteAnyObjectAsync<T>(this Stream stream, T obj, CancellationToken cancellationToken = default) where T : class, new()
        {
            if (obj is IStreamSerializer serializable)
            {
                await WriteSerializedAsync(stream, serializable, cancellationToken).DynamicContext();
                return;
            }
            Type type = obj.GetType();
            PropertyInfo[] pis = StreamSerializerAttribute.GetWriteProperties(type).ToArray();
            StreamSerializerAttribute? attr = type.GetCustomAttribute<StreamSerializerAttribute>(),
                objAttr;
            bool useChecksum = !(attr?.SkipPropertyNameChecksum ?? false);
            await WriteNumberNullableAsync(stream, attr?.Version, cancellationToken).DynamicContext();
            await WriteNumberAsync(stream, pis.Length, cancellationToken).DynamicContext();
            foreach (PropertyInfo pi in pis)
            {
                objAttr = pi.GetCustomAttribute<StreamSerializerAttribute>();
                if (useChecksum && !(objAttr?.SkipPropertyNameChecksum ?? false))
                    await WriteAsync(stream, Encoding.UTF8.GetBytes(pi.Name).Aggregate((c, b) => (byte)(c ^ b)), cancellationToken).DynamicContext();
                if (Nullable.GetUnderlyingType(pi.PropertyType) == null)
                {
                    await WriteAnyAsync(stream, pi.GetValue(obj)!, cancellationToken).DynamicContext();
                }
                else
                {
                    await WriteAnyNullableAsync(stream, pi.GetValue(obj)).DynamicContext();
                }
            }
        }

        /// <summary>
        /// Write any object
        /// </summary>
        /// <typeparam name="tStream">Stream type</typeparam>
        /// <typeparam name="tObj">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <returns>Stream</returns>
        public static tStream WriteAnyObjectNullable<tStream, tObj>(this tStream stream, tObj? obj)
            where tStream : Stream
            where tObj : class, new()
        {
            Write(stream, obj != null);
            if (obj != null) WriteAnyObject(stream, obj);
            return stream;
        }

        /// <summary>
        /// Write any object
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static async Task WriteAnyObjectNullableAsync<T>(this Stream stream, T? obj, CancellationToken cancellationToken = default) where T : class, new()
        {
            await WriteAsync(stream, obj != null, cancellationToken).DynamicContext();
            if (obj != null) await WriteAnyObjectAsync(stream, obj, cancellationToken).DynamicContext();
        }
    }
}
