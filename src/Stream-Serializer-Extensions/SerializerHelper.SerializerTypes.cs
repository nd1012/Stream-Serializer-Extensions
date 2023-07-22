using System.Collections;
using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    // SerializerTypes
    public static partial class SerializerHelper
    {
        /// <summary>
        /// Get item serialization informations from a type
        /// </summary>
        /// <param name="type">Item type</param>
        /// <param name="objType">Object type</param>
        /// <param name="isAsync">Is an asynchronous context?</param>
        /// <returns>Serializer type, synchronous serializer and asynchronous serializer</returns>
        public static (SerializerTypes Type, StreamSerializer.Serializer_Delegate? Serializer, StreamSerializer.AsyncSerializer_Delegate? AsyncSerializer) GetItemSerializerInfo(
            this Type type,
            ObjectTypes objType,
            bool isAsync
            )
        {
            if (objType.IsEmpty()) return (SerializerTypes.Any, null, null);
            int thc = type.GetHashCode();
            if (type.IsAbstract || type.IsInterface || thc == typeof(object).GetHashCode()) return (SerializerTypes.Any, null, null);
            if (thc == typeof(bool).GetHashCode()) return (SerializerTypes.Bool, null, null);
            if (type.IsNumeric()) return (SerializerTypes.Number, null, null);
            if (type.IsEnum) return (SerializerTypes.Enum, null, null);
            if (thc == typeof(string).GetHashCode()) return (SerializerTypes.String, null, null);
            if (thc == typeof(byte[]).GetHashCode()) return (SerializerTypes.Bytes, null, null);
            if (thc == typeof(Type).GetHashCode()) return (SerializerTypes.Type, null, null);
            if (type.IsArray) return (SerializerTypes.Array, null, null);
            if (typeof(IStreamSerializer).IsAssignableFrom(type)) return (SerializerTypes.List, null, null);
            (StreamSerializer.Serializer_Delegate? syncSerializer, StreamSerializer.AsyncSerializer_Delegate? asyncSerializer) = FindSerializer(type);
            if (syncSerializer != null || (isAsync && asyncSerializer != null)) return (SerializerTypes.Serializer, syncSerializer, asyncSerializer);
            if (typeof(Stream).IsAssignableFrom(type)) return (SerializerTypes.Stream, null, null);
            if (typeof(IDictionary).IsAssignableFrom(type)) return (SerializerTypes.Dictionary, null, null);
            if (typeof(IList).IsAssignableFrom(type)) return (SerializerTypes.List, null, null);
            if (type.IsValueType) return (SerializerTypes.Struct, null, null);
            return (SerializerTypes.AnyObject, null, null);
        }

        /// <summary>
        /// Get item deserialization informations from a type
        /// </summary>
        /// <param name="type">Item type</param>
        /// <param name="objType">Object type</param>
        /// <param name="isAsync">Is an asynchronous context?</param>
        /// <returns>Serializer type, synchronous deserializer and asynchronous deserializer</returns>
        public static (
            SerializerTypes Type,
            StreamSerializer.Deserializer_Delegate? Deserializer,
            StreamSerializer.AsyncDeserializer_Delegate? AsyncDeserializer
            ) GetItemDeserializerInfo(
            this Type type,
            ObjectTypes objType,
            bool isAsync
            )
        {
            if (objType.IsEmpty()) return (SerializerTypes.Any, null, null);
            int thc = type.GetHashCode();
            if (type.IsAbstract || type.IsInterface || thc == typeof(object).GetHashCode()) return (SerializerTypes.Any, null, null);
            if (thc == typeof(bool).GetHashCode()) return (SerializerTypes.Bool, null, null);
            if (type.IsNumeric()) return (SerializerTypes.Number, null, null);
            if (type.IsEnum) return (SerializerTypes.Enum, null, null);
            if (thc == typeof(string).GetHashCode()) return (SerializerTypes.String, null, null);
            if (thc == typeof(byte[]).GetHashCode()) return (SerializerTypes.Bytes, null, null);
            if (thc == typeof(Type).GetHashCode()) return (SerializerTypes.Type, null, null);
            if (type.IsArray) return (SerializerTypes.Array, null, null);
            if (typeof(IStreamSerializer).IsAssignableFrom(type)) return (SerializerTypes.List, null, null);
            (StreamSerializer.Deserializer_Delegate? syncDeserializer, StreamSerializer.AsyncDeserializer_Delegate? asyncDeserializer) = FindDeserializer(type);
            if (syncDeserializer != null || (isAsync && asyncDeserializer != null)) return (SerializerTypes.Serializer, syncDeserializer, asyncDeserializer);
            if (typeof(Stream).IsAssignableFrom(type)) return (SerializerTypes.Stream, null, null);
            if (typeof(IDictionary).IsAssignableFrom(type)) return (SerializerTypes.Dictionary, null, null);
            if (typeof(IList).IsAssignableFrom(type)) return (SerializerTypes.List, null, null);
            if (type.IsValueType) return (SerializerTypes.Struct, null, null);
            return (SerializerTypes.AnyObject, null, null);
        }
    }
}
