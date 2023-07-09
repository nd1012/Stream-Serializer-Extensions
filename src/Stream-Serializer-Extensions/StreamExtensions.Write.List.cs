using System.Collections;
using System.Runtime;
using System.Runtime.CompilerServices;
using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    // List
    public static partial class StreamExtensions
    {
        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream WriteList(this Stream stream, IList value, ISerializationContext context)
        {
            Type type = value.GetType();
            SerializerException.Wrap(() => ArgumentValidationHelper.EnsureValidArgument(
                nameof(value),
                type.IsGenericType || type.IsGenericTypeDefinition || !typeof(List<>).IsAssignableFrom(type.GetGenericTypeDefinition()),
                () => "Not a list type"
                ));
            WriteNumber(stream, value.Count, context);
            if (value.Count == 0) return stream;
            WriteListInt(stream, value, context);
            return stream;
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<Stream> WriteListAsync(this Stream stream, IList value, ISerializationContext context)
        {
            Type type = value.GetType();
            SerializerException.Wrap(() => ArgumentValidationHelper.EnsureValidArgument(
                nameof(value),
                type.IsGenericType || type.IsGenericTypeDefinition || !typeof(List<>).IsAssignableFrom(type.GetGenericTypeDefinition()),
                () => "Not a list type"
                ));
            await WriteNumberAsync(stream, value.Count, context).DynamicContext();
            if (value.Count == 0) return stream;
            await WriteListIntAsync(stream, value, context).DynamicContext();
            return stream;
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> WriteListAsync(this Task<Stream> stream, IList value, ISerializationContext context)
            => AsyncHelper.FluentAsync(stream, value, context, WriteListAsync);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Stream WriteListNullable(this Stream stream, IList? value, ISerializationContext context)
            => WriteNullableCount(context, value?.Count, () =>
            {
                Type type = value!.GetType();
                SerializerException.Wrap(() => ArgumentValidationHelper.EnsureValidArgument(
                    nameof(value),
                    type.IsGenericType || type.IsGenericTypeDefinition || !typeof(List<>).IsAssignableFrom(type.GetGenericTypeDefinition()),
                    () => "Not a list type"
                    ));
                if (value!.Count == 0) return;
                WriteListInt(stream, value, context);
            });

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<Stream> WriteListNullableAsync(this Stream stream, IList? value, ISerializationContext context)
            => WriteNullableCountAsync(context, value?.Count, async () =>
            {
                Type type = value!.GetType();
                SerializerException.Wrap(() => ArgumentValidationHelper.EnsureValidArgument(
                    nameof(value),
                    type.IsGenericType || type.IsGenericTypeDefinition || !typeof(List<>).IsAssignableFrom(type.GetGenericTypeDefinition()),
                    () => "Not a list type"
                    ));
                if (value!.Count == 0) return;
                await WriteListIntAsync(stream, value, context).DynamicContext();
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
        public static Task<Stream> WriteListNullableAsync(this Task<Stream> stream, IList? value, ISerializationContext context)
            => AsyncHelper.FluentAsync(stream, value, context, WriteListNullableAsync);

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void WriteListInt(Stream stream, IList value, ISerializationContext context)
        {
            using ContextRecursion cr = new(context);
            Type valueType = value.GetType(),
                elementType = valueType.IsGenericType && valueType.GenericTypeArguments.Length == 1
                    ? valueType.GenericTypeArguments[0]
                    : typeof(object);
            using ItemSerializerContext itemContext = new(context);
            (itemContext.ItemSerializer, itemContext.ItemSyncSerializer, _) = elementType.GetItemSerializerInfo(ObjectTypes.Null, isAsync: false);
            if (itemContext.ItemSerializer == SerializerTypes.Any)
            {
                object? item;
                Type itemType;
                for (int i = 0, len = value.Count; i < len; i++)
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
                for (int i = 0, len = value.Count; i < len; WriteItem(itemContext, value[i]!), i++) ;
            }
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">Value to write</param>
        /// <param name="context">Context</param>
        /// <returns>Stream</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static async Task WriteListIntAsync(Stream stream, IList value, ISerializationContext context)
        {
            using ContextRecursion cr = new(context);
            Type valueType = value.GetType(),
                elementType = valueType.IsGenericType && valueType.GenericTypeArguments.Length == 1
                    ? valueType.GenericTypeArguments[0]
                    : typeof(object);
            using ItemSerializerContext itemContext = new(context);
            (itemContext.ItemSerializer, itemContext.ItemSyncSerializer, itemContext.ItemAsyncSerializer) = 
                elementType.GetItemSerializerInfo(ObjectTypes.Null, isAsync: true);
            if (itemContext.ItemSerializer == SerializerTypes.Any)
            {
                object? item;
                Type itemType;
                for (int i = 0, len = value.Count; i < len; i++)
                {
                    item = value[i];
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
                for (int i = 0, len = value.Count; i < len; await WriteItemAsync(itemContext, value[i]!).DynamicContext(), i++) ;
            }
        }
    }
}
