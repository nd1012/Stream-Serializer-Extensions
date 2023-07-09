namespace wan24.StreamSerializerExtensions
{
    // Type
    public static partial class SerializerHelper
    {
        /// <summary>
        /// Find type serializers
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Synchronous and asynchronous serializers</returns>
        public static (StreamSerializer.Serializer_Delegate? Serializer, StreamSerializer.AsyncSerializer_Delegate? AsyncSerializer) FindSerializer(this Type type)
            => (StreamSerializer.FindSerializer(type), StreamSerializer.FindAsyncSerializer(type));

        /// <summary>
        /// Find type deserializers
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Synchronous and asynchronous deserializers</returns>
        public static (StreamSerializer.Deserializer_Delegate? Deserializer, StreamSerializer.AsyncDeserializer_Delegate? AsyncDeserializer) FindDeserializer(this Type type)
            => (StreamSerializer.FindDeserializer(type), StreamSerializer.FindAsyncDeserializer(type));

        /// <summary>
        /// Get the object type of a type
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Object type</returns>
        public static ObjectTypes GetObjectType(this Type type)
        {
            ObjectTypes objType;
            int thc = type.GetHashCode();
            if (thc == typeof(bool).GetHashCode()) objType = ObjectTypes.Bool;
            else if (thc == typeof(sbyte).GetHashCode()) objType = ObjectTypes.Byte;
            else if (thc == typeof(byte).GetHashCode()) objType = ObjectTypes.Byte | ObjectTypes.Unsigned;
            else if (thc == typeof(short).GetHashCode()) objType = ObjectTypes.Short;
            else if (thc == typeof(ushort).GetHashCode()) objType = ObjectTypes.Short | ObjectTypes.Unsigned;
            else if (thc == typeof(int).GetHashCode()) objType = ObjectTypes.Int;
            else if (thc == typeof(uint).GetHashCode()) objType = ObjectTypes.Int | ObjectTypes.Unsigned;
            else if (thc == typeof(long).GetHashCode()) objType = ObjectTypes.Long;
            else if (thc == typeof(ulong).GetHashCode()) objType = ObjectTypes.Long | ObjectTypes.Unsigned;
            else if (thc == typeof(float).GetHashCode()) objType = ObjectTypes.Float;
            else if (thc == typeof(double).GetHashCode()) objType = ObjectTypes.Double;
            else if (thc == typeof(decimal).GetHashCode()) objType = ObjectTypes.Decimal;
            else if (thc == typeof(byte[]).GetHashCode()) objType = ObjectTypes.Bytes;
            else if (thc == typeof(string).GetHashCode()) objType = ObjectTypes.String;
            else if (typeof(IStreamSerializer).IsAssignableFrom(type))
                objType = StreamSerializer.TypeCacheEnabled ? ObjectTypes.Serializable | ObjectTypes.CachedSerializable : ObjectTypes.Serializable;
            else if (thc == typeof(Stream).GetHashCode()) objType = ObjectTypes.Stream;
            else if (thc == typeof(Type).GetHashCode()) objType = ObjectTypes.ClrType;
            else if (type.IsArray) objType = type.GetArrayRank() == 1 ? ObjectTypes.Array | ObjectTypes.NoRank : ObjectTypes.Array;
            else if (type.IsGenericType && typeof(Dictionary<,>).IsAssignableFrom(type.IsGenericTypeDefinition ? type : type.GetGenericTypeDefinition())) objType = ObjectTypes.Dict;
            else if (type.IsGenericType && typeof(List<>).IsAssignableFrom(type.IsGenericTypeDefinition ? type : type.GetGenericTypeDefinition())) objType = ObjectTypes.List;
            else if (type.IsValueType) objType = ObjectTypes.Struct;
            else objType = ObjectTypes.Object;
            if (type.IsGenericType) objType |= ObjectTypes.Generic;
            return objType;
        }
    }
}
