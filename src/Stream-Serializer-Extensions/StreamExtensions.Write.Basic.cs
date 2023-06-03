using System.Text;
using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    // Basic types
    public static partial class StreamExtensions
    {
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
            using RentedArray<byte> poolData = new(1, clean: false);
            poolData[0] = (byte)(value ? 1 : 0);
            await stream.WriteAsync(poolData.Memory, cancellationToken).DynamicContext();
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
#pragma warning disable IDE0060 // Remove unused parameter
        public static async Task WriteAsync(this Stream stream, byte value, CancellationToken cancellationToken = default)
#pragma warning restore IDE0060
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
                using RentedArray<byte> buffer = new(sizeof(short), StreamSerializer.BufferPool, clean: false);
                stream.Write(value.GetBytes(buffer.Span));
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
                using RentedArray<byte> buffer = new(sizeof(short), StreamSerializer.BufferPool, clean: false);
                await stream.WriteAsync(value.GetBytes(buffer.Memory), cancellationToken).DynamicContext();
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
                using RentedArray<byte> buffer = new(sizeof(ushort), StreamSerializer.BufferPool, clean: false);
                stream.Write(value.GetBytes(buffer.Span));
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
                using RentedArray<byte> buffer = new(sizeof(ushort), StreamSerializer.BufferPool, clean: false);
                await stream.WriteAsync(value.GetBytes(buffer.Memory), cancellationToken).DynamicContext();
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
                using RentedArray<byte> buffer = new(sizeof(int), StreamSerializer.BufferPool, clean: false);
                stream.Write(value.GetBytes(buffer.Span));
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
                using RentedArray<byte> buffer = new(sizeof(int), StreamSerializer.BufferPool, clean: false);
                await stream.WriteAsync(value.GetBytes(buffer.Memory), cancellationToken).DynamicContext();
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
                using RentedArray<byte> buffer = new(sizeof(uint), StreamSerializer.BufferPool, clean: false);
                stream.Write(value.GetBytes(buffer.Span));
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
                using RentedArray<byte> buffer = new(sizeof(uint), StreamSerializer.BufferPool, clean: false);
                await stream.WriteAsync(value.GetBytes(buffer.Memory), cancellationToken).DynamicContext();
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
                using RentedArray<byte> buffer = new(sizeof(long), StreamSerializer.BufferPool, clean: false);
                stream.Write(value.GetBytes(buffer.Span));
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
                using RentedArray<byte> buffer = new(sizeof(long), StreamSerializer.BufferPool, clean: false);
                await stream.WriteAsync(value.GetBytes(buffer.Memory), cancellationToken).DynamicContext();
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
                using RentedArray<byte> buffer = new(sizeof(ulong), StreamSerializer.BufferPool, clean: false);
                stream.Write(value.GetBytes(buffer.Span));
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
                using RentedArray<byte> buffer = new(sizeof(ulong), StreamSerializer.BufferPool, clean: false);
                await stream.WriteAsync(value.GetBytes(buffer.Memory), cancellationToken).DynamicContext();
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
                using RentedArray<byte> buffer = new(sizeof(float), StreamSerializer.BufferPool, clean: false);
                stream.Write(value.GetBytes(buffer.Span));
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
                using RentedArray<byte> buffer = new(sizeof(float), StreamSerializer.BufferPool, clean: false);
                await stream.WriteAsync(value.GetBytes(buffer.Memory), cancellationToken).DynamicContext();
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
                using RentedArray<byte> buffer = new(sizeof(double), StreamSerializer.BufferPool, clean: false);
                stream.Write(value.GetBytes(buffer.Span));
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
                using RentedArray<byte> buffer = new(sizeof(double), StreamSerializer.BufferPool, clean: false);
                await stream.WriteAsync(value.GetBytes(buffer.Memory), cancellationToken).DynamicContext();
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
                using RentedArray<byte> buffer = new(sizeof(int) << 2, StreamSerializer.BufferPool, clean: false);
                stream.Write(value.GetBytes(buffer.Span));
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
                using RentedArray<byte> buffer = new(sizeof(int) << 2, StreamSerializer.BufferPool, clean: false);
                await stream.WriteAsync(value.GetBytes(buffer.Memory), cancellationToken).DynamicContext();
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
            using RentedArray<byte> poolData = new(1, clean: false);
            poolData[0] = (byte)type;
            stream.Write(poolData.Span);
            if (!type.IsZero() && !type.HasValueFlags())
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
            using RentedArray<byte> poolData = new(1, clean: false);
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
    }
}
