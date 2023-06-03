using System.Reflection;
using System.Text;
using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    // Object
    public static partial class StreamExtensions
    {
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
            if (obj is IStreamSerializer streamSerializer) return WriteSerialized(stream, streamSerializer);
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
            if (obj is IStreamSerializer streamSerializer)
            {
                await WriteSerializedAsync(stream, streamSerializer, cancellationToken).DynamicContext();
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
                    await WriteAnyNullableAsync(stream, pi.GetValue(obj), cancellationToken).DynamicContext();
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
