using System.Reflection;
using System.Runtime;
using System.Runtime.InteropServices;
using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    // Structure
    public static partial class StreamExtensions
    {
        /// <summary>
        /// Write a struct
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Struct</param>
        /// <param name="forceLittleEndian">Force little endian encoding?</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static Stream WriteStruct(this Stream stream, object value, bool forceLittleEndian = true)
            => SerializerException.Wrap(() =>
            {
                Type structType = value.GetType();
                ArgumentValidationHelper.EnsureValidArgument(nameof(value), structType.IsValueType, "Not a structure");
                using RentedArray<byte> buffer = new(Marshal.SizeOf(value), StreamSerializer.BufferPool, clean: false);
                GCHandle gch = GCHandle.Alloc(buffer.Array, GCHandleType.Pinned);
                try
                {
                    Marshal.StructureToPtr(value, gch.AddrOfPinnedObject(), fDeleteOld: true);
                }
                finally
                {
                    gch.Free();
                }
                if (forceLittleEndian && !BitConverter.IsLittleEndian && structType.GetCustomAttributeCached<StreamSerializerAttribute>() is StreamSerializerAttribute attr)
                    ConvertStructureEndianess(structType, buffer.Memory, attr);
                return WriteBytes(stream, buffer.Span);
            });

        /// <summary>
        /// Write a struct
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Struct</param>
        /// <param name="forceLittleEndian">Force little endian encoding?</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static Stream WriteStructNullable(this Stream stream, object? value, bool forceLittleEndian = true)
            => WriteIfNull(stream, value, () => WriteStruct(stream, value!, forceLittleEndian));

        /// <summary>
        /// Write a struct
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Struct</param>
        /// <param name="forceLittleEndian">Force little endian encoding?</param>
        /// <param name="cancellationToken">Cancellation token</param>
        [TargetedPatchingOptOut("Tiny method")]
        public static Task WriteStructAsync(this Stream stream, object value, bool forceLittleEndian = true, CancellationToken cancellationToken = default)
            => SerializerException.WrapAsync(() =>
            {
                Type structType = value.GetType();
                ArgumentValidationHelper.EnsureValidArgument(nameof(value), structType.IsValueType, "Not a structure");
                using RentedArray<byte> buffer = new(Marshal.SizeOf(value), StreamSerializer.BufferPool, clean: false);
                GCHandle gch = GCHandle.Alloc(buffer.Array, GCHandleType.Pinned);
                try
                {
                    Marshal.StructureToPtr(value, gch.AddrOfPinnedObject(), fDeleteOld: true);
                }
                finally
                {
                    gch.Free();
                }
                if (forceLittleEndian && !BitConverter.IsLittleEndian && structType.GetCustomAttribute<StreamSerializerAttribute>() is StreamSerializerAttribute attr)
                    ConvertStructureEndianess(structType, buffer.Memory, attr);
                return WriteBytesAsync(stream, buffer.Memory, cancellationToken);
            });

        /// <summary>
        /// Write a struct
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Struct</param>
        /// <param name="forceLittleEndian">Force little endian encoding?</param>
        /// <param name="cancellationToken">Cancellation token</param>
        [TargetedPatchingOptOut("Tiny method")]
        public static Task WriteStructNullableAsync(
            this Stream stream,
            object? value,
            bool forceLittleEndian = true,
            CancellationToken cancellationToken = default
            )
            => WriteIfNullAsync(stream, value, () => WriteStructAsync(stream, value!, forceLittleEndian, cancellationToken), cancellationToken);

        /// <summary>
        /// Convert the endianess of structure fields
        /// </summary>
        /// <param name="type">Structure type</param>
        /// <param name="data">Data</param>
        /// <param name="attr">Stream serializer attribute of the structure type</param>
        public static void ConvertStructureEndianess(Type type, Memory<byte> data, StreamSerializerAttribute? attr = null)
        {
            if (!type.IsValueType) throw new ArgumentException("Structure type required", nameof(type));
            attr ??= type.GetCustomAttribute<StreamSerializerAttribute>()
                ?? throw new ArgumentException($"{type} requires a {typeof(StreamSerializerAttribute)}", nameof(type));
            Queue<(Type Type, Memory<byte> Data, StreamSerializerAttribute Attribute)> queue = new();
            queue.Enqueue((type, data, attr));
            Type t;
            while (queue.TryDequeue(out var item))
                foreach (FieldInfo fi in item.Attribute.GetStructureFields(item.Type))
                {
                    t = fi.FieldType;
                    if (t.IsEnum) t = t.GetEnumUnderlyingType();
                    if (t.IsNumeric())
                    {
                        item.Data.Span.Slice(Marshal.OffsetOf(item.Type, fi.Name).ToInt32(), Marshal.SizeOf(t)).Reverse();
                    }
                    else
                    {
                        if (item.Type.GetCustomAttribute<StreamSerializerAttribute>() is not StreamSerializerAttribute a)
                            throw new InvalidProgramException($"{item.Type}.{fi.Name} has a {nameof(StreamSerializerAttribute)}, but {t} has none");
                        queue.Enqueue((t, item.Data[Marshal.SizeOf(t)..], a));
                    }
                }
        }
    }
}
