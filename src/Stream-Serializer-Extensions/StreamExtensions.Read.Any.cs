using System.Runtime;
using System.Runtime.CompilerServices;
using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    // Any
    public static partial class StreamExtensions
    {
        /// <summary>
        /// Read any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Object</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object ReadAny(this Stream stream, IDeserializationContext context)
            => ReadAnyInt(context, (ObjectTypes)ReadOneByte(stream, context), type: null);

        /// <summary>
        /// Read any object
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="objType">Object type</param>
        /// <param name="type">CLR type</param>
        /// <returns>Object</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static object ReadAnyInt(IDeserializationContext context, ObjectTypes objType, Type? type)
            => SerializerException.Wrap(() =>
            {
                using ContextRecursion cr = new(context);
                if (objType == ObjectTypes.Null) throw new SerializerException("NULL object type is not supported by this method");
                bool isEmpty = objType.IsEmpty(),
                    isUnsigned = objType.IsUnsigned(),
                    readType = objType.RequiresType();
                switch (objType.RemoveFlags())
                {
                    case ObjectTypes.Bool:
                        return !isEmpty;
                    case ObjectTypes.Byte:
                        switch (context.SerializerVersion)// Serializer version switch
                        {
                            case 1:
                            case 2:
                                type = isUnsigned ? typeof(byte) : typeof(sbyte);
                                break;
                            default:
                                return isEmpty
                                    ? Convert.ChangeType(0, isUnsigned ? typeof(byte) : typeof(sbyte))
                                    : isUnsigned
                                        ? ReadOneByte(context.Stream, context)
                                        : ReadOneSByte(context.Stream, context);
                        }
                        break;
                    case ObjectTypes.Short:
                        switch (context.SerializerVersion)// Serializer version switch
                        {
                            case 1:
                            case 2:
                                type = isUnsigned ? typeof(ushort) : typeof(short);
                                break;
                            default:
                                return isEmpty
                                    ? Convert.ChangeType(0, isUnsigned ? typeof(ushort) : typeof(short))
                                    : isUnsigned
                                        ? ReadUShort(context.Stream, context)
                                        : ReadShort(context.Stream, context);
                        }
                        break;
                    case ObjectTypes.Int:
                    case ObjectTypes.Long:
                    case ObjectTypes.Float:
                    case ObjectTypes.Double:
                    case ObjectTypes.Decimal:
                        type = objType.RemoveFlags() switch
                        {
                            ObjectTypes.Int => isUnsigned ? typeof(uint) : typeof(int),
                            ObjectTypes.Long => isUnsigned ? typeof(ulong) : typeof(long),
                            ObjectTypes.Float => typeof(float),
                            ObjectTypes.Double => typeof(double),
                            ObjectTypes.Decimal => typeof(decimal),
                            _ => throw new InvalidProgramException()
                        };
                        break;
                    case ObjectTypes.String:
                        return isEmpty
                            ? string.Empty
                            : ReadString(
                                context.Stream,
                                context,
                                minLen: context.Options?.GetMinLen(0) ?? 0,
                                maxLen: context.Options?.GetMaxLen(int.MaxValue) ?? int.MaxValue
                                );
                    case ObjectTypes.String16:
                        return isEmpty
                            ? string.Empty
                            : ReadString16(
                                context.Stream,
                                context,
                                minLen: context.Options?.GetMinLen(0) ?? 0,
                                maxLen: context.Options?.GetMaxLen(int.MaxValue) ?? int.MaxValue
                                );
                    case ObjectTypes.String32:
                        return isEmpty
                            ? string.Empty
                            : ReadString32(
                                context.Stream,
                                context,
                                minLen: context.Options?.GetMinLen(0) ?? 0,
                                maxLen: context.Options?.GetMaxLen(int.MaxValue) ?? int.MaxValue
                                );
                    case ObjectTypes.Bytes:
                        return isEmpty
                            ? Array.Empty<byte>()
                            : ReadBytes(
                                context.Stream,
                                context,
                                minLen: context.Options?.GetMinLen(0) ?? 0,
                                maxLen: context.Options?.GetMaxLen(int.MaxValue) ?? int.MaxValue
                                ).Value;
                    case ObjectTypes.Stream:
                        readType = false;
                        break;
                    case ObjectTypes.ClrType:
                        if (isUnsigned)
                        {
                            if (context.SerializerVersion < 3)
                                throw new SerializerException($"CLR type reading isn't available in version {context.SerializerVersion}", new InvalidDataException());
                            if (!StreamSerializer.TypeCacheEnabled)
                                throw new SerializerException(
                                    $"The type cache needs to be enabled in order to be able to deserialize this type information",
                                    new InvalidOperationException()
                                    );
                            int thc = context.Stream.ReadInt(context);
                            if (!TypeCache.Types.TryGetValue(thc, out Type? type)) throw new SerializerException($"Unknown type #{thc}", new InvalidDataException());
                            if (!typeof(IStreamSerializer).IsAssignableFrom(type))
                                throw new SerializerException($"Invalid type {type} (possibly manipulated byte sequence)", new InvalidDataException());
                            return type;
                        }
                        return ReadType(context.Stream, context);
                }
                if (readType && type == null)
#pragma warning disable IDE0066 // Use switch expression
                    switch (context.SerializerVersion)// Serializer version switch
                    {
                        case 1:
                        case 2:
                            type = StreamSerializer.LoadType(ReadString(context.Stream, context, minLen: 1, maxLen: short.MaxValue));
                            break;
                        default:
                            type = ReadType(context.Stream, context);
                            break;
                    }
#pragma warning restore IDE0066 // Use switch expression
                switch (objType.RemoveFlags())
                {
                    case ObjectTypes.Byte:// Compatibility with serializer version 2
                    case ObjectTypes.Short:// Compatibility with serializer version 2
                    case ObjectTypes.Int:
                    case ObjectTypes.Long:
                    case ObjectTypes.Float:
                    case ObjectTypes.Double:
                    case ObjectTypes.Decimal:
                        return isEmpty ? Activator.CreateInstance(type!)! : ReadNumber(context.Stream, type!, context)!;
                    case ObjectTypes.Array:
                        return isEmpty ? ArrayEmptyMethod.MakeGenericMethod(type!.GetElementType()!).InvokeAuto(obj: null)! : ReadArray(
                            context.Stream,
                            type!,
                            context,
                            context.Options?.GetMinLen(0) ?? 0,
                            context.Options?.GetMaxLen(int.MaxValue) ?? int.MaxValue
                            );
                    case ObjectTypes.List:
                        return isEmpty ? Activator.CreateInstance(type!)! : ReadList(
                            context.Stream,
                            type!,
                            context,
                            context.Options?.GetMinLen(0) ?? 0,
                            context.Options?.GetMaxLen(int.MaxValue) ?? int.MaxValue
                            );
                    case ObjectTypes.Dict:
                        return isEmpty ? Activator.CreateInstance(type!)! : ReadDict(
                            context.Stream,
                            type!,
                            context,
                            context.Options?.GetMinLen(0) ?? 0,
                            context.Options?.GetMaxLen(int.MaxValue) ?? int.MaxValue
                            );
                    case ObjectTypes.Object:
                        return ReadObject(context.Stream, type!, context);
                    case ObjectTypes.Struct:
                        return ReadStructMethod.MakeGenericMethod(type!).InvokeAuto(obj: null, context.Stream, context)!;
                    case ObjectTypes.Serializable:
                        return ReadSerializedObject(context.Stream, type!, context);
                    case ObjectTypes.Stream:
                        Stream res = context.Options?.Attribute.GetStream(obj: null, property: null, context) ?? new PooledTempStream();
                        if (objType.IsEmpty()) return res;
                        try
                        {
                            return ReadStream(context.Stream, res, context, minLen: context.Options?.GetMinLen(0L) ?? 0, maxLen: context.Options?.GetMaxLen(long.MaxValue) ?? long.MaxValue);
                        }
                        catch
                        {
                            res.Dispose();
                            throw;
                        }
                    default:
                        throw new InvalidProgramException();
                }
            });

        /// <summary>
        /// Read any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Object</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<object> ReadAnyAsync(this Stream stream, IDeserializationContext context)
            => await ReadAnyIntAsync(context, (ObjectTypes)await ReadOneByteAsync(stream, context).DynamicContext(), type: null).DynamicContext();

        /// <summary>
        /// Read any object
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="objType">Object type</param>
        /// <param name="type">CLR type</param>
        /// <returns>Object</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Task<object> ReadAnyIntAsync(IDeserializationContext context, ObjectTypes objType, Type? type)
            => SerializerException.WrapAsync(async () =>
            {
                using ContextRecursion cr = new(context);
                if (objType == ObjectTypes.Null) throw new SerializerException("NULL object type is not supported by this method");
                bool isEmpty = objType.IsEmpty(),
                    isUnsigned = objType.IsUnsigned(),
                    readType = true;
                Task task;
                switch (objType.RemoveFlags())
                {
                    case ObjectTypes.Bool:
                        return !isEmpty;
                    case ObjectTypes.Byte:
                        switch (context.SerializerVersion)// Serializer version switch
                        {
                            case 1:
                            case 2:
                                type = isUnsigned ? typeof(byte) : typeof(sbyte);
                                break;
                            default:
                                return isEmpty
                                    ? Convert.ChangeType(0, isUnsigned ? typeof(byte) : typeof(sbyte))
                                    : isUnsigned
                                        ? await ReadOneByteAsync(context.Stream, context).DynamicContext()
                                        : await ReadOneSByteAsync(context.Stream, context).DynamicContext();
                        }
                        break;
                    case ObjectTypes.Short:
                        switch (context.SerializerVersion)// Serializer version switch
                        {
                            case 1:
                            case 2:
                                type = isUnsigned ? typeof(ushort) : typeof(short);
                                break;
                            default:
                                return isEmpty
                                    ? Convert.ChangeType(0, isUnsigned ? typeof(ushort) : typeof(short))
                                    : isUnsigned
                                        ? await ReadUShortAsync(context.Stream, context).DynamicContext()
                                        : await ReadShortAsync(context.Stream, context).DynamicContext();
                        }
                        break;
                    case ObjectTypes.Int:
                    case ObjectTypes.Long:
                    case ObjectTypes.Float:
                    case ObjectTypes.Double:
                    case ObjectTypes.Decimal:
                        type = objType.RemoveFlags() switch
                        {
                            ObjectTypes.Int => isUnsigned ? typeof(uint) : typeof(int),
                            ObjectTypes.Long => isUnsigned ? typeof(ulong) : typeof(long),
                            ObjectTypes.Float => typeof(float),
                            ObjectTypes.Double => typeof(double),
                            ObjectTypes.Decimal => typeof(decimal),
                            _ => throw new InvalidProgramException()
                        };
                        break;
                    case ObjectTypes.String:
                        return isEmpty
                            ? string.Empty
                            : await ReadStringAsync(
                                context.Stream,
                                context,
                                minLen: context.Options?.GetMinLen(0) ?? 0,
                                maxLen: context.Options?.GetMaxLen(int.MaxValue) ?? int.MaxValue
                                ).DynamicContext();
                    case ObjectTypes.String16:
                        return isEmpty
                            ? string.Empty
                            : await ReadString16Async(
                                context.Stream,
                                context,
                                minLen: context.Options?.GetMinLen(0) ?? 0,
                                maxLen: context.Options?.GetMaxLen(int.MaxValue) ?? int.MaxValue
                                ).DynamicContext();
                    case ObjectTypes.String32:
                        return isEmpty
                            ? string.Empty
                            : await ReadString32Async(
                                context.Stream,
                                context,
                                minLen: context.Options?.GetMinLen(0) ?? 0,
                                maxLen: context.Options?.GetMaxLen(int.MaxValue) ?? int.MaxValue
                                ).DynamicContext();
                    case ObjectTypes.Bytes:
                        return isEmpty
                            ? Array.Empty<byte>()
                            : (await ReadBytesAsync(
                                context.Stream,
                                context,
                                minLen: context.Options?.GetMinLen(0) ?? 0,
                                maxLen: context.Options?.GetMaxLen(int.MaxValue) ?? int.MaxValue
                                ).DynamicContext()).Value;
                    case ObjectTypes.Stream:
                        readType = false;
                        break;
                    case ObjectTypes.ClrType:
                        if (isUnsigned)
                        {
                            if (context.SerializerVersion < 3)
                                throw new SerializerException($"CLR type reading isn't available in version {context.SerializerVersion}", new InvalidDataException());
                            if (!StreamSerializer.TypeCacheEnabled)
                                throw new SerializerException(
                                    $"The type cache needs to be enabled in order to be able to deserialize this type information",
                                    new InvalidOperationException()
                                    );
                            int thc = await context.Stream.ReadIntAsync(context).DynamicContext();
                            if (!TypeCache.Types.TryGetValue(thc, out Type? type)) throw new SerializerException($"Unknown type #{thc}", new InvalidDataException());
                            if (!typeof(IStreamSerializer).IsAssignableFrom(type))
                                throw new SerializerException($"Invalid type {type} (possibly manipulated byte sequence)", new InvalidDataException());
                            return type;
                        }
                        return await ReadTypeAsync(context.Stream, context).DynamicContext();
                }
                if (readType && type == null)
#pragma warning disable IDE0066 // Use switch expression
                    switch (context.SerializerVersion)// Serializer version switch
                    {
                        case 1:
                        case 2:
                            type = StreamSerializer.LoadType(await ReadStringAsync(context.Stream, context, minLen: 1, maxLen: short.MaxValue).DynamicContext());
                            break;
                        default:
                            type = await ReadTypeAsync(context.Stream, context).DynamicContext();
                            break;
                    }
#pragma warning restore IDE0066 // Use switch expression
                switch (objType.RemoveFlags())
                {
                    case ObjectTypes.Byte:// Compatibility with serializer version 2
                    case ObjectTypes.Short:// Compatibility with serializer version 2
                    case ObjectTypes.Int:
                    case ObjectTypes.Long:
                    case ObjectTypes.Float:
                    case ObjectTypes.Double:
                    case ObjectTypes.Decimal:
                        if (isEmpty) return Activator.CreateInstance(type!)!;
                        return await ReadNumberAsync(context.Stream, type!, context).DynamicContext();
                    case ObjectTypes.Array:
                        return isEmpty ? ArrayEmptyMethod.MakeGenericMethod(type!.GetElementType()!).InvokeAuto(obj: null)! : await ReadArrayAsync(
                            context.Stream,
                            type!,
                            context,
                            context.Options?.GetMinLen(0) ?? 0,
                            context.Options?.GetMaxLen(int.MaxValue) ?? int.MaxValue
                            ).DynamicContext();
                    case ObjectTypes.List:
                        return isEmpty ? Activator.CreateInstance(type!)! : await ReadListAsync(
                            context.Stream,
                            type!,
                            context,
                            context.Options?.GetMinLen(0) ?? 0,
                            context.Options?.GetMaxLen(int.MaxValue) ?? int.MaxValue
                            ).DynamicContext();
                    case ObjectTypes.Dict:
                        return isEmpty ? Activator.CreateInstance(type!)! : await ReadDictAsync(
                            context.Stream,
                            type!,
                            context,
                            context.Options?.GetMinLen(0) ?? 0,
                            context.Options?.GetMaxLen(int.MaxValue) ?? int.MaxValue
                            ).DynamicContext();
                    case ObjectTypes.Object:
                        return await ReadObjectAsync(context.Stream, type!, context).DynamicContext();
                    case ObjectTypes.Struct:
                        task = (Task)ReadStructAsyncMethod.MakeGenericMethod(type!).InvokeAuto(obj: null, context.Stream, context)!;
                        break;
                    case ObjectTypes.Serializable:
                        return await ReadSerializedObjectAsync(context.Stream, type!, context)!.DynamicContext();
                    case ObjectTypes.Stream:
                        Stream res = context.Options?.Attribute.GetStream(obj: null, property: null, context) ?? new PooledTempStream();
                        if (objType.IsEmpty()) return res;
                        try
                        {
                            return await ReadStreamAsync(
                                context.Stream,
                                res,
                                context,
                                minLen: context.Options?.GetMinLen(0L) ?? 0,
                                maxLen: context.Options?.GetMaxLen(long.MaxValue) ?? long.MaxValue
                                ).DynamicContext();
                        }
                        catch
                        {
                            res.Dispose();
                            throw;
                        }
                    default:
                        throw new InvalidProgramException();
                }
                await task.DynamicContext();
                return task.GetResult(type!);
            });

        /// <summary>
        /// Read any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Object</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object? ReadAnyNullable(this Stream stream, IDeserializationContext context)
        {
            ObjectTypes objType = (ObjectTypes)ReadOneByte(stream, context);
            return objType == ObjectTypes.Null ? null : ReadAnyInt(context, objType, type: null);
        }

        /// <summary>
        /// Read any object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <returns>Object</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<object?> ReadAnyNullableAsync(this Stream stream, IDeserializationContext context)
        {
            ObjectTypes objType = (ObjectTypes)await ReadOneByteAsync(stream, context).DynamicContext();
            return objType == ObjectTypes.Null ? null : await ReadAnyIntAsync(context, objType, type: null).DynamicContext();
        }
    }
}
