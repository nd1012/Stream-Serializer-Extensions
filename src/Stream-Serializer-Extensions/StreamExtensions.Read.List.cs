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
        /// Read
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static List<T> ReadList<T>(this Stream stream, IDeserializationContext context, int minLen = 0, int maxLen = int.MaxValue)
        {
            int len = ReadNumber<int>(stream, context);
            SerializerHelper.EnsureValidLength(len, minLen, maxLen);
            List<T> res = new(len);
            ReadListInt(context, res, typeof(T), len);
            return res;
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">List type</param>
        /// <param name="context">Context</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static IList ReadList(this Stream stream, Type type, IDeserializationContext context, int minLen = 0, int maxLen = int.MaxValue)
        {
            SerializerException.Wrap(() => ArgumentValidationHelper.EnsureValidArgument(
                nameof(type),
                type.IsGenericType || type.IsGenericTypeDefinition || !typeof(List<>).IsAssignableFrom(type.GetGenericTypeDefinition()),
                () => "Not a list type"
                ));
            int len = ReadNumber<int>(stream, context);
            SerializerHelper.EnsureValidLength(len, minLen, maxLen);
            Type itemType = type.GetGenericArgumentsCached()[0];
            IList res = (IList)(Activator.CreateInstance(type, len) ?? throw new SerializerException($"Failed to instance {type}"));
            ReadListInt(context, res, itemType, len);
            return res;
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<List<T>> ReadListAsync<T>(this Stream stream, IDeserializationContext context, int minLen = 0, int maxLen = int.MaxValue)
        {
            int len = await ReadNumberAsync<int>(stream, context).DynamicContext();
            SerializerHelper.EnsureValidLength(len, minLen, maxLen);
            List<T> res = new(len);
            await ReadListIntAsync(context, res, typeof(T), len).DynamicContext();
            return res;
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">List type</param>
        /// <param name="context">Context</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<IList> ReadListAsync(this Stream stream, Type type, IDeserializationContext context, int minLen = 0, int maxLen = int.MaxValue)
        {
            SerializerException.Wrap(() => ArgumentValidationHelper.EnsureValidArgument(
                nameof(type),
                type.IsGenericType || type.IsGenericTypeDefinition || !typeof(List<>).IsAssignableFrom(type.GetGenericTypeDefinition()),
                () => "Not a list type"
                ));
            int len = await ReadNumberAsync<int>(stream, context).DynamicContext();
            SerializerHelper.EnsureValidLength(len, minLen, maxLen);
            Type itemType = type.GetGenericArgumentsCached()[0];
            IList res = (IList)(Activator.CreateInstance(type, len) ?? throw new SerializerException($"Failed to instance {type}"));
            await ReadListIntAsync(context, res, type.GenericTypeArguments[0], len).DynamicContext();
            return res;
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static List<T>? ReadListNullable<T>(this Stream stream, IDeserializationContext context, int minLen = 0, int maxLen = int.MaxValue)
        {
            switch (context.SerializerVersion)// Serializer version switch
            {
                case 1:
                case 2:
                    {
                        return ReadBool(stream, context) ? ReadList<T>(stream, context, minLen, maxLen) : null;
                    }
                default:
                    {
                        if (ReadNumberNullable<int>(stream, context) is not int len) return null;
                        SerializerHelper.EnsureValidLength(len, minLen, maxLen);
                        List<T> res = new(len);
                        ReadListInt(context, res, typeof(T), len);
                        return res;
                    }
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">List type</param>
        /// <param name="context">Context</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static IList? ReadListNullable(this Stream stream, Type type, IDeserializationContext context, int minLen = 0, int maxLen = int.MaxValue)
        {
            SerializerException.Wrap(() => ArgumentValidationHelper.EnsureValidArgument(
                nameof(type),
                type.IsGenericType || type.IsGenericTypeDefinition || !typeof(List<>).IsAssignableFrom(type.GetGenericTypeDefinition()),
                () => "Not a list type"
                ));
            switch (context.SerializerVersion)// Serializer version switch
            {
                case 1:
                case 2:
                    {
                        return ReadBool(stream, context) ? ReadList(stream, type, context, minLen, maxLen) : null;
                    }
                default:
                    {
                        if (ReadNumberNullable<int>(stream, context) is not int len) return null;
                        SerializerHelper.EnsureValidLength(len, minLen, maxLen);
                        Type itemType = type.GetGenericArgumentsCached()[0];
                        IList res = (IList)(Activator.CreateInstance(type, len) ?? throw new SerializerException($"Failed to instance {type}"));
                        ReadListInt(context, res, itemType, len);
                        return res;
                    }
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="context">Context</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<List<T>?> ReadListNullableAsync<T>(this Stream stream, IDeserializationContext context, int minLen = 0, int maxLen = int.MaxValue)
        {
            switch (context.SerializerVersion)// Serializer version switch
            {
                case 1:
                case 2:
                    {
                        return await ReadBoolAsync(stream, context).DynamicContext()
                            ? await ReadListAsync<T>(stream, context, minLen, maxLen).DynamicContext()
                            : null;
                    }
                default:
                    {
                        if (await ReadNumberNullableAsync<int>(stream, context).DynamicContext() is not int len) return null;
                        SerializerHelper.EnsureValidLength(len, minLen, maxLen);
                        List<T> res = new(len);
                        await ReadListIntAsync(context, res, typeof(T), len).DynamicContext();
                        return res;
                    }
            }
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">List type</param>
        /// <param name="context">Context</param>
        /// <param name="minLen">Minimum length</param>
        /// <param name="maxLen">Maximum length</param>
        /// <returns>Value</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static async Task<IList?> ReadListNullableAsync(this Stream stream, Type type, IDeserializationContext context, int minLen = 0, int maxLen = int.MaxValue)
        {
            SerializerException.Wrap(() => ArgumentValidationHelper.EnsureValidArgument(
                nameof(type),
                type.IsGenericType || type.IsGenericTypeDefinition || !typeof(List<>).IsAssignableFrom(type.GetGenericTypeDefinition()),
                () => "Not a list type"
                ));
            switch (context.SerializerVersion)// Serializer version switch
            {
                case 1:
                case 2:
                    {
                        return await ReadBoolAsync(stream, context).DynamicContext()
                            ? await ReadListAsync(stream, type, context, minLen, maxLen).DynamicContext()
                            : null;
                    }
                default:
                    {
                        if (await ReadNumberNullableAsync<int>(stream, context).DynamicContext() is not int len) return null;
                        SerializerHelper.EnsureValidLength(len, minLen, maxLen);
                        Type itemType = type.GetGenericArgumentsCached()[0];
                        IList res = (IList)(Activator.CreateInstance(type, len) ?? throw new SerializerException($"Failed to instance {type}"));
                        await ReadListIntAsync(context, res, itemType, len).DynamicContext();
                        return res;
                    }
            }
        }

        /// <summary>
        /// Read list items
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="list">List</param>
        /// <param name="type">Item type</param>
        /// <param name="count">Number of items</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ReadListInt(IDeserializationContext context, IList list, Type type, int count)
        {
            using ContextRecursion cr = new(context);
            switch (context.SerializerVersion)// Serializer version switch
            {
                case 1:
                case 2:
                    {
                        for (int i = 0; i < count; list.Add(ReadObject(context.Stream, type, context)), i++) ;
                        break;
                    }
                default:
                    try
                    {
                        using ItemDeserializerContext itemContext = new(context)
                        {
                            Nullable = context.Options?.IsNullable ?? context.Nullable
                        };
                        (itemContext.ItemSerializer, itemContext.ItemSyncDeserializer, _) = type.GetItemDeserializerInfo(ObjectTypes.Null, isAsync: false);
                        if (itemContext.ItemSerializer == SerializerTypes.Any)
                        {
                            object? obj;
                            for (int i = 0; i < count; i++)
                            {
                                obj = ReadAnyItemHeader(itemContext, i, type);
                                if (obj == null && itemContext.ObjectType == ObjectTypes.Null)
                                {
                                    if (!itemContext.Nullable) throw new SerializerException($"Deserialized NULL value #{i}", new InvalidDataException());
                                    list.Add(obj);
                                }
                                else if (obj == null)
                                {
                                    list.Add(itemContext.ItemSerializer == SerializerTypes.Serializer
                                        ? obj = ReadItem(itemContext)
                                        : obj = ReadAnyInt(context, itemContext.ObjectType, itemContext.ItemType));
                                    if (itemContext.ObjectType.RequiresObjectWriting()) itemContext.AddObject(obj);
                                }
                                else
                                {
                                    list.Add(obj);
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < count; list.Add(ReadItem(itemContext)), i++) ;
                        }
                        break;
                    }
                    finally
                    {
                        context.WithoutOptions();
                    }
            }
        }

        /// <summary>
        /// Read list items
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="list">List</param>
        /// <param name="type">Item type</param>
        /// <param name="count">Number of items</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static async Task ReadListIntAsync(IDeserializationContext context, IList list, Type type, int count)
        {
            using ContextRecursion cr = new(context);
            switch (context.SerializerVersion)// Serializer version switch
            {
                case 1:
                case 2:
                    {
                        for (int i = 0; i < count; list.Add(await ReadObjectAsync(context.Stream, type, context).DynamicContext()), i++) ;
                        break;
                    }
                default:
                    try
                    {
                        using ItemDeserializerContext itemContext = new(context)
                        {
                            Nullable = context.Options?.IsNullable ?? context.Nullable
                        };
                        (itemContext.ItemSerializer, itemContext.ItemSyncDeserializer, itemContext.ItemAsyncDeserializer) = 
                            type.GetItemDeserializerInfo(ObjectTypes.Null, isAsync: true);
                        if (itemContext.ItemSerializer == SerializerTypes.Any)
                        {
                            object? obj;
                            for (int i = 0; i < count; i++)
                            {
                                obj = await ReadAnyItemHeaderAsync(itemContext, i, type).DynamicContext();
                                if (obj == null && itemContext.ObjectType == ObjectTypes.Null)
                                {
                                    if (!itemContext.Nullable) throw new SerializerException($"Deserialized NULL value #{i}", new InvalidDataException());
                                    list.Add(obj);
                                }
                                else if (obj == null)
                                {
                                    list.Add(itemContext.ItemSerializer == SerializerTypes.Serializer
                                        ? obj = await ReadItemAsync(itemContext).DynamicContext()
                                        : obj = await ReadAnyIntAsync(context, itemContext.ObjectType, itemContext.ItemType).DynamicContext());
                                    if (itemContext.ObjectType.RequiresObjectWriting()) itemContext.AddObject(obj);
                                }
                                else
                                {
                                    list.Add(obj);
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < count; list.Add(ReadItem(itemContext)), i++) ;
                        }
                        break;
                    }
                    finally
                    {
                        context.WithoutOptions();
                    }
            }
        }
    }
}
