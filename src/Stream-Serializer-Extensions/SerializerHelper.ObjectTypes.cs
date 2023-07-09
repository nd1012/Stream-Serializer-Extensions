using System.Collections;
using System.Diagnostics;
using System.Runtime;
using System.Runtime.CompilerServices;
using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    // ObjectTypes
    public static partial class SerializerHelper
    {
        /// <summary>
        /// Remove flags
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Type without flags</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ObjectTypes RemoveFlags(this ObjectTypes type) => type & ~ObjectTypes.FLAGS;

        /// <summary>
        /// Is empty?
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Empty?</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEmpty(this ObjectTypes type) => type.ContainsAllFlags(ObjectTypes.Empty);

        /// <summary>
        /// Is zero?
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Zero?</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsZero(this ObjectTypes type) => type.IsNumber() && type.ContainsAllFlags(ObjectTypes.Zero);

        /// <summary>
        /// Is unsigned?
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Unsigned?</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsUnsigned(this ObjectTypes type) => type.ContainsAllFlags(ObjectTypes.Unsigned);

        /// <summary>
        /// Is cached?
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Cached?</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsCached(this ObjectTypes type) => type.ContainsAllFlags(ObjectTypes.Cached);

        /// <summary>
        /// Is generic?
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Generic?</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsGeneric(this ObjectTypes type) => type.ContainsAllFlags(ObjectTypes.Generic);

        /// <summary>
        /// Is enumeration break?
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Break?</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBreak(this ObjectTypes type) => type == ObjectTypes.Break;

        /// <summary>
        /// Is an array?
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>An array?</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsArray(this ObjectTypes type) => type.RemoveFlags() == ObjectTypes.Array;

        /// <summary>
        /// Is not ranked?
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Not ranked?</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotRanked(this ObjectTypes type) => type.ContainsAllFlags(ObjectTypes.NoRank);

        /// <summary>
        /// Is a cached serializable type?
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Is cached serializable?</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsCachedSerializable(this ObjectTypes type) => type.ContainsAllFlags(ObjectTypes.CachedSerializable);

        /// <summary>
        /// Is a basic type info?
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Is a basic type info?</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBasicTypeInfo(this ObjectTypes type) => type.ContainsAllFlags(ObjectTypes.BasicTypeInfo);

        /// <summary>
        /// Is a number?
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>A number?</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool IsNumber(this ObjectTypes type) => type.RemoveFlags() switch
        {
            ObjectTypes.Byte => true,
            ObjectTypes.Short => true,
            ObjectTypes.Int => true,
            ObjectTypes.Long => true,
            ObjectTypes.Float => true,
            ObjectTypes.Double => true,
            ObjectTypes.Decimal => true,
            _ => false
        };

        /// <summary>
        /// Determine if a type is required
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Is required?</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool RequiresType(this ObjectTypes type) => type.RemoveFlags() switch
        {
            ObjectTypes.Array => true,
            ObjectTypes.List => true,
            ObjectTypes.Dict => true,
            ObjectTypes.Object => true,
            ObjectTypes.Struct => true,
            ObjectTypes.Serializable => true,
            _ => false
        };

        /// <summary>
        /// Does the type require to write the serialized object?
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Is required?</returns>
        [TargetedPatchingOptOut("Tiny method")]
#if !NO_INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool RequiresObjectWriting(this ObjectTypes type) => type switch
        {
            ObjectTypes.Null => false,
            ObjectTypes.Bool => false,
            ObjectTypes.Cached => false,
            _ => !type.ContainsAnyFlag(ObjectTypes.Empty)
        };

        /// <summary>
        /// Get object serializer informations
        /// </summary>
        /// <param name="obj">Object</param>
        /// <returns>Informations</returns>
        public static (Type Type, ObjectTypes ObjectType, bool WriteType, bool WriteObject) GetObjectSerializerInfo(this object obj)
        {
            ArgumentValidationHelper.EnsureValidArgument(nameof(obj), obj);
#if DEBUG
            Debug.Assert(obj != null);
#endif
            Type type = obj.GetType();
            ObjectTypes objType = obj switch
            {
                bool => ObjectTypes.Bool,
                sbyte => ObjectTypes.Byte,
                byte => ObjectTypes.Byte | ObjectTypes.Unsigned,
                short => ObjectTypes.Short,
                ushort => ObjectTypes.Short | ObjectTypes.Unsigned,
                int => ObjectTypes.Int,
                uint => ObjectTypes.Int | ObjectTypes.Unsigned,
                long => ObjectTypes.Long,
                ulong => ObjectTypes.Long | ObjectTypes.Unsigned,
                float => ObjectTypes.Float,
                double => ObjectTypes.Double,
                decimal => ObjectTypes.Decimal,
                string => ObjectTypes.String,
                byte[] => ObjectTypes.Bytes,
                Stream => ObjectTypes.Stream,
                Type => ObjectTypes.ClrType,
                IStreamSerializer => ObjectTypes.Serializable,
                _ => ObjectTypes.Null
            };
            if (objType == ObjectTypes.Null)
                if (type.IsArray)
                {
                    objType = ObjectTypes.Array;
                }
                else if (typeof(IList).IsAssignableFrom(type))
                {
                    objType = ObjectTypes.List;
                }
                else if (typeof(IDictionary).IsAssignableFrom(type))
                {
                    objType = ObjectTypes.Dict;
                }
                else if (type.IsValueType && !type.IsEnum)
                {
                    objType = ObjectTypes.Struct;
                }
                else
                {
                    objType = ObjectTypes.Object;
                }
            bool writeType = false,
                writeObject = true;
            switch (objType.RemoveFlags())
            {
                case ObjectTypes.Bool:
                    if (!(bool)obj) objType |= ObjectTypes.False;
                    writeObject = false;
                    break;
                case ObjectTypes.Byte:
                case ObjectTypes.Short:
                case ObjectTypes.Int:
                case ObjectTypes.Long:
                case ObjectTypes.Float:
                case ObjectTypes.Double:
                case ObjectTypes.Decimal:
                    if (Activator.CreateInstance(type)!.Equals(obj))
                    {
                        objType |= ObjectTypes.Zero;
                        writeObject = false;
                    }
                    break;
                case ObjectTypes.String:
                    if (((string)obj).Length == 0)
                    {
                        objType |= ObjectTypes.Empty;
                        writeObject = false;
                    }
                    break;
                case ObjectTypes.Array:
                    if (((Array)obj).Length == 0)
                    {
                        objType |= ObjectTypes.Empty;
                        writeObject = false;
                    }
                    writeType = true;
                    break;
                case ObjectTypes.List:
                    if (((IList)obj).Count == 0)
                    {
                        objType |= ObjectTypes.Empty;
                        writeObject = false;
                    }
                    writeType = true;
                    break;
                case ObjectTypes.Dict:
                    if (((IDictionary)obj).Count == 0)
                    {
                        objType |= ObjectTypes.Empty;
                        writeObject = false;
                    }
                    writeType = true;
                    break;
                case ObjectTypes.Object:
                case ObjectTypes.Struct:
                case ObjectTypes.Serializable:
                    writeType = true;
                    break;
                case ObjectTypes.ClrType:
                    break;
                case ObjectTypes.Bytes:
                    if (((byte[])obj).Length == 0)
                    {
                        objType |= ObjectTypes.Empty;
                        writeObject = false;
                    }
                    break;
                case ObjectTypes.Stream:
                    Stream stream = (Stream)obj;
                    if (stream.CanSeek && stream.Length == 0)
                    {
                        objType |= ObjectTypes.Empty;
                        writeObject = false;
                    }
                    break;
                default:
                    throw new InvalidProgramException();
            }
            return (type, objType, writeType, writeObject);
        }

        /// <summary>
        /// Ensure a correct object type, based on additional serializer options for a property
        /// </summary>
        /// <param name="objType">Object type</param>
        /// <param name="pi">Property</param>
        /// <returns>Object type</returns>
        public static ObjectTypes EnsureCorrectObjectType(this ObjectTypes objType, PropertyInfoExt pi)
            => pi.GetCustomAttributeCached<StreamSerializerAttribute>() is StreamSerializerAttribute attr
                ? EnsureCorrectObjectType(objType, attr)
                : objType;

        /// <summary>
        /// Ensure a correct object type, based on additional serializer options
        /// </summary>
        /// <param name="objType">Object type</param>
        /// <param name="options">Options</param>
        /// <returns>Object type</returns>
        public static ObjectTypes EnsureCorrectObjectType(this ObjectTypes objType, ISerializerOptions options)
        {
            switch (objType)
            {
                case ObjectTypes.String:
                    if (options.Serializer != null)
                        switch (options.Serializer.Value)
                        {
                            case SerializerTypes.String16:
                                objType = ObjectTypes.String16;
                                break;
                            case SerializerTypes.String32:
                                objType = ObjectTypes.String32;
                                break;
                        }
                    break;
            }
            return objType;
        }

        /// <summary>
        /// Ensure a correct object type, based on additional serializer options 
        /// </summary>
        /// <param name="objType">Object type</param>
        /// <param name="attr">Attribute</param>
        /// <returns>Object type</returns>
        public static ObjectTypes EnsureCorrectObjectType(this ObjectTypes objType, StreamSerializerAttribute attr)
        {
            switch (objType)
            {
                case ObjectTypes.String:
                    if (attr.Serializer != null)
                        switch (attr.Serializer.Value)
                        {
                            case SerializerTypes.String16:
                                objType = ObjectTypes.String16;
                                break;
                            case SerializerTypes.String32:
                                objType = ObjectTypes.String32;
                                break;
                        }
                    break;
            }
            return objType;
        }
    }
}
