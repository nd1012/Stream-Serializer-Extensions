using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    // Any
    public static partial class StreamExtensions
    {
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
                using (RentedArray<byte> poolData = new(1))
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
                    using RentedArray<byte> poolData = new(1, clean: false);
                    poolData[0] = (byte)ObjectTypes.Null;
                    stream.Write(poolData.Span);
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
                    using RentedArray<byte> poolData = new(1, clean: false);
                    poolData[0] = (byte)ObjectTypes.Null;
                    await stream.WriteAsync(poolData.Memory, cancellationToken).DynamicContext();
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
    }
}
