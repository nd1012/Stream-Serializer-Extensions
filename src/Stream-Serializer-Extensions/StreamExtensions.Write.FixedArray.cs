using System.Runtime;
using System.Runtime.CompilerServices;
using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    // Fixed array
    public static partial class StreamExtensions
    {
        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Just a method adapter")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Stream WriteFixedArray<T>(this Stream stream, Span<T> value, ISerializationContext context)
            => WriteFixedArray(stream, (ReadOnlySpan<T>)value, context);

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream WriteFixedArray<T>(this Stream stream, ReadOnlySpan<T> value, ISerializationContext context)
        {
            try
            {
                using ContextRecursion cr = new(context);
                Type elementType = typeof(T);
                using ItemSerializerContext itemContext = new(context);
                (itemContext.ItemSerializer, itemContext.ItemSyncSerializer, _) = elementType.GetItemSerializerInfo(ObjectTypes.Null, isAsync: false);
                if (itemContext.ItemSerializer == SerializerTypes.Any)
                {
                    T? item;
                    Type itemType;
                    for (int i = 0, len = value.Length; i < len; i++)
                    {
                        item = value[i];
                        if (item == null)
                        {
                            if (!context.Nullable) throw new SerializerException($"Found NULL item at #{i}", new InvalidDataException());
                            Write(context.Stream, (byte)ObjectTypes.Null, context);
                            continue;
                        }
                        itemType = item.GetType();
                        WriteAnyItemHeader(itemContext, item, itemType);
                        if (itemContext.WriteObject && itemContext.ObjectType != ObjectTypes.Cached)
                            if (itemContext.ItemSerializer == SerializerTypes.Serializer)
                            {
                                WriteItem(itemContext, item);
                            }
                            else
                            {
                                WriteAny(stream, item, itemContext.ObjectType, itemContext.WriteObject, context);
                            }
                    }
                }
                else
                {
                    for (int i = 0, len = value.Length; i < len; WriteItem(itemContext, value[i]!), i++) ;
                }
                return stream;
            }
            catch (SerializerException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw SerializerException.From(ex);
            }
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream WriteFixedArray(this Stream stream, Array value, ISerializationContext context)
            => SerializerException.Wrap(() =>
            {
                using ContextRecursion cr = new(context);
                Type elementType = value.GetType().GetElementType()!;
                using ItemSerializerContext itemContext = new(context);
                (itemContext.ItemSerializer, itemContext.ItemSyncSerializer, _) = elementType.GetItemSerializerInfo(ObjectTypes.Null, isAsync: false);
                if (itemContext.ItemSerializer == SerializerTypes.Any)
                {
                    object? item;
                    Type itemType;
                    for (int i = 0, len = value.Length; i < len; i++)
                    {
                        item = value.GetValue(i);
                        if (item == null)
                        {
                            if (!context.Nullable) throw new SerializerException($"Found NULL item at #{i}", new InvalidDataException());
                            Write(context.Stream, (byte)ObjectTypes.Null, context);
                            continue;
                        }
                        itemType = item.GetType();
                        WriteAnyItemHeader(itemContext, item, itemType);
                        if (itemContext.WriteObject && itemContext.ObjectType != ObjectTypes.Cached)
                            if (itemContext.ItemSerializer == SerializerTypes.Serializer)
                            {
                                WriteItem(itemContext, item);
                            }
                            else
                            {
                                WriteAny(stream, item, itemContext.ObjectType, itemContext.WriteObject, context);
                            }
                    }
                }
                else
                {
                    for (int i = 0, len = value.Length; i < len; WriteItem(itemContext, value.GetValue(i)!), i++) ;
                }
                return stream;
            });

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Just a method adapter")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteFixedArrayAsync<T>(this Stream stream, Memory<T> value, ISerializationContext context)
            => WriteFixedArrayAsync(stream, (ReadOnlyMemory<T>)value, context);

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Just a method adapter")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteFixedArrayAsync<T>(this Task<Stream> stream, Memory<T> value, ISerializationContext context)
            => AsyncHelper.FluentAsync(stream, value, context, WriteFixedArrayAsync);

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<Stream> WriteFixedArrayAsync<T>(this Stream stream, ReadOnlyMemory<T> value, ISerializationContext context)
            => SerializerException.WrapAsync(async () =>
            {
                using ContextRecursion cr = new(context);
                Type elementType = typeof(T);
                using ItemSerializerContext itemContext = new(context);
                (itemContext.ItemSerializer, itemContext.ItemSyncSerializer, itemContext.ItemAsyncSerializer) = 
                    elementType.GetItemSerializerInfo(ObjectTypes.Null, isAsync: true);
                if (itemContext.ItemSerializer == SerializerTypes.Any)
                {
                    T? item;
                    Type itemType;
                    for (int i = 0, len = value.Length; i < len; i++)
                    {
                        item = value.Span[i];
                        if (item == null)
                        {
                            if (!context.Nullable) throw new SerializerException($"Found NULL item at #{i}", new InvalidDataException());
                            await WriteAsync(context.Stream, (byte)ObjectTypes.Null, context).DynamicContext();
                            continue;
                        }
                        itemType = item.GetType();
                        await WriteAnyItemHeaderAsync(itemContext, item, itemType).DynamicContext();
                        if (itemContext.WriteObject && itemContext.ObjectType != ObjectTypes.Cached)
                            if (itemContext.ItemSerializer == SerializerTypes.Serializer)
                            {
                                await WriteItemAsync(itemContext, item).DynamicContext();
                            }
                            else
                            {
                                await WriteAnyAsync(stream, item, itemContext.ObjectType, itemContext.WriteObject, context).DynamicContext();
                            }
                    }
                }
                else
                {
                    for (int i = 0, len = value.Length; i < len; await WriteItemAsync(itemContext, value.Span[i]!).DynamicContext(), i++) ;
                }
                return stream;
            });

        /// <summary>
        /// Write
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteFixedArrayAsync<T>(
            this Task<Stream> stream,
            ReadOnlyMemory<T> value,
            ISerializationContext context
            )
            => AsyncHelper.FluentAsync(stream, value, context, WriteFixedArrayAsync);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<Stream> WriteFixedArrayAsync(this Stream stream, Array value, ISerializationContext context)
            => SerializerException.WrapAsync(async () =>
            {
                using ContextRecursion cr = new(context);
                Type elementType = value.GetType().GetElementType()!;
                using ItemSerializerContext itemContext = new(context);
                (itemContext.ItemSerializer, itemContext.ItemSyncSerializer, itemContext.ItemAsyncSerializer) = 
                    elementType.GetItemSerializerInfo(ObjectTypes.Null, isAsync: true);
                if (itemContext.ItemSerializer == SerializerTypes.Any)
                {
                    object? item;
                    Type itemType;
                    for (int i = 0, len = value.Length; i < len; i++)
                    {
                        item = value.GetValue(i);
                        if (item == null)
                        {
                            if (!context.Nullable) throw new SerializerException($"Found NULL item at #{i}", new InvalidDataException());
                            await WriteAsync(context.Stream, (byte)ObjectTypes.Null, context).DynamicContext();
                            continue;
                        }
                        itemType = item.GetType();
                        await WriteAnyItemHeaderAsync(itemContext, item, itemType).DynamicContext();
                        if (itemContext.WriteObject && itemContext.ObjectType != ObjectTypes.Cached)
                            if (itemContext.ItemSerializer == SerializerTypes.Serializer)
                            {
                                await WriteItemAsync(itemContext, item).DynamicContext();
                            }
                            else
                            {
                                await WriteAnyAsync(stream, item, itemContext.ObjectType, itemContext.WriteObject, context).DynamicContext();
                            }
                    }
                }
                else
                {
                    for (int i = 0, len = value.Length; i < len; await WriteItemAsync(itemContext, value.GetValue(i)!).DynamicContext(), i++) ;
                }
                return stream;
            });

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteFixedArrayAsync(this Task<Stream> stream, Array value, ISerializationContext context)
            => AsyncHelper.FluentAsync(stream, value, context, WriteFixedArrayAsync);
    }
}
