using System.Reflection;
using System.Runtime;

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
            ReadObjectNullableMethod = type.GetMethod(nameof(ReadObjectNullable), BindingFlags.Static | BindingFlags.Public) ?? throw new TypeLoadException($"Failed to reflect {nameof(ReadObjectNullable)}");
            ReadObjectNullableAsyncMethod = type.GetMethod(nameof(ReadObjectNullableAsync), BindingFlags.Static | BindingFlags.Public) ?? throw new TypeLoadException($"Failed to reflect {nameof(ReadObjectNullableAsync)}");
            ReadStructMethod = type.GetMethod(nameof(ReadStruct), BindingFlags.Static | BindingFlags.Public) ?? throw new TypeLoadException($"Failed to reflect {nameof(ReadStruct)}");
            ReadStructAsyncMethod = type.GetMethod(nameof(ReadStructAsync), BindingFlags.Static | BindingFlags.Public) ?? throw new TypeLoadException($"Failed to reflect {nameof(ReadStructAsync)}");
            ReadAnyMethod = type.GetMethod(nameof(ReadAny), BindingFlags.Static | BindingFlags.Public) ?? throw new TypeLoadException($"Failed to reflect {nameof(ReadAny)}");
            ReadAnyAsyncMethod = type.GetMethod(nameof(ReadAnyAsync), BindingFlags.Static | BindingFlags.Public) ?? throw new TypeLoadException($"Failed to reflect {nameof(ReadAnyAsync)}");
            ReadAnyNullableMethod = type.GetMethod(nameof(ReadAnyNullable), BindingFlags.Static | BindingFlags.Public) ?? throw new TypeLoadException($"Failed to reflect {nameof(ReadAnyNullable)}");
            ReadAnyNullableAsyncMethod = type.GetMethod(nameof(ReadAnyNullableAsync), BindingFlags.Static | BindingFlags.Public) ?? throw new TypeLoadException($"Failed to reflect {nameof(ReadAnyNullableAsync)}");
            ReadSerializedMethod = type.GetMethod(nameof(ReadSerialized), BindingFlags.Static | BindingFlags.Public) ?? throw new TypeLoadException($"Failed to reflect {nameof(ReadSerialized)}");
            ReadSerializedAsyncMethod = type.GetMethod(nameof(ReadSerializedAsync), BindingFlags.Static | BindingFlags.Public) ?? throw new TypeLoadException($"Failed to reflect {nameof(ReadSerializedAsync)}");
            ReadNumberMethod = type.GetMethod(nameof(ReadNumber), BindingFlags.Static | BindingFlags.Public) ?? throw new TypeLoadException($"Failed to reflect {nameof(ReadNumber)}");
            ReadNumberAsyncMethod = type.GetMethod(nameof(ReadNumberAsync), BindingFlags.Static | BindingFlags.Public) ?? throw new TypeLoadException($"Failed to reflect {nameof(ReadNumberAsync)}");
            ReadNumberIntMethod = type.GetMethod(nameof(ReadNumberInt), BindingFlags.Static | BindingFlags.NonPublic) ?? throw new TypeLoadException($"Failed to reflect {nameof(ReadNumberInt)}");
            ReadNumberIntAsyncMethod = type.GetMethod(nameof(ReadNumberIntAsync), BindingFlags.Static | BindingFlags.NonPublic) ?? throw new TypeLoadException($"Failed to reflect {nameof(ReadNumberIntAsync)}");
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
        /// Write the serializer version
        /// </summary>
        /// <typeparam name="T">Stream type</typeparam>
        /// <param name="stream">Stream</param>
        /// <returns>Stream</returns>
        [TargetedPatchingOptOut("Tiny method")]
        public static T WriteSerializerVersion<T>(this T stream) where T : Stream => WriteNumber(stream, StreamSerializer.VERSION);

        /// <summary>
        /// Write the serializer version
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="cancellationToken">Cancellation token</param>
        [TargetedPatchingOptOut("Tiny method")]
        public static Task WriteSerializerVersionAsync(this Stream stream, CancellationToken cancellationToken = default)
            => WriteNumberAsync(stream, StreamSerializer.VERSION, cancellationToken);
    }
}
