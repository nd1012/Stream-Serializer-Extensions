using System.Reflection;
using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    /// <summary>
    /// Autostream serializer information
    /// </summary>
    public sealed class AutoStreamSerializerInfo
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="property">Property</param>
        public AutoStreamSerializerInfo(PropertyInfo property)
        {
            Property = property;
            Attribute = Property.GetCustomAttribute<StreamSerializerAttribute>() ?? throw new ArgumentException($"Missing {typeof(StreamSerializerAttribute)}", nameof(property));
        }

        /// <summary>
        /// Property
        /// </summary>
        public PropertyInfo Property { get; }

        /// <summary>
        /// Attribute
        /// </summary>
        public StreamSerializerAttribute Attribute { get; }

        /// <summary>
        /// Serializer
        /// </summary>
        public Serializer_Delegate? Serializer { get; set; }

        /// <summary>
        /// Serializer
        /// </summary>
        public AsyncSerializer_Delegate? AsyncSerializer { get; set; }

        /// <summary>
        /// Deserializer
        /// </summary>
        public Deserializer_Delegate? Deserializer { get; set; }

        /// <summary>
        /// Deserializer
        /// </summary>
        public AsyncDeserializer_Delegate? AsyncDeserializer { get; set; }

        /// <summary>
        /// Serialize
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="config">Configuration</param>
        /// <param name="obj">Object</param>
        /// <param name="stream">Stream</param>
        public void Serialize<T>(IAutoStreamSerializerConfig config, T obj, Stream stream) where T : IAutoStreamSerializer
        {
            if (Serializer == null)
            {
                stream.WriteObject(Property.GetValue(obj));
            }
            else
            {
                Serializer(config, this, obj, Property.GetValue(obj), stream);
            }
        }

        /// <summary>
        /// Serialize
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="config">Configuration</param>
        /// <param name="obj">Object</param>
        /// <param name="stream">Stream</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public async Task SerializeAsync<T>(IAutoStreamSerializerConfig config, T obj, Stream stream, CancellationToken cancellationToken) where T : IAutoStreamSerializer
        {
            if (AsyncSerializer == null)
            {
                await stream.WriteObjectAsync(obj, cancellationToken).DynamicContext();
            }
            else
            {
                await AsyncSerializer(config, this, obj, Property.GetValue(obj), stream, cancellationToken).DynamicContext();
            }
        }

        /// <summary>
        /// Deserialize
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="config">Configuration</param>
        /// <param name="obj">Object</param>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        public void Deserialize<T>(IAutoStreamSerializerConfig config, T obj, Stream stream, int version) where T : IAutoStreamSerializer
        {
            if (Deserializer == null)
            {
                Property.SetValue(obj, StreamExtensions.ReadObjectMethod.MakeGenericMethod(Property.PropertyType).InvokeAuto(obj: null, stream, version));
            }
            else
            {
                Property.SetValue(obj, Deserializer(config, this, obj, stream, version));
            }
        }

        /// <summary>
        /// Deserialize
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="config">Configuration</param>
        /// <param name="obj">Object</param>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public async Task DeserializeAsync<T>(IAutoStreamSerializerConfig config, T obj, Stream stream, int version, CancellationToken cancellationToken) where T : IAutoStreamSerializer
        {
            if (AsyncDeserializer == null)
            {
                Property.SetValue(
                    obj, 
                    await StreamExtensions.ReadObjectAsyncMethod.MakeGenericMethod(Property.PropertyType).InvokeAutoAsync(obj: null, stream, version, cancellationToken).DynamicContext()
                    );
            }
            else
            {
                Property.SetValue(obj, await AsyncDeserializer(config, this, obj, stream, version, cancellationToken).DynamicContext());
            }
        }

        /// <summary>
        /// Delegate for a serializer
        /// </summary>
        /// <param name="config">Configuration</param>
        /// <param name="info">Info</param>
        /// <param name="obj">Object</param>
        /// <param name="value">Value to serialize</param>
        /// <param name="stream">Stream</param>
        public delegate void Serializer_Delegate(IAutoStreamSerializerConfig config, AutoStreamSerializerInfo info, IAutoStreamSerializer obj, object? value, Stream stream);

        /// <summary>
        /// Delegate for a serializer
        /// </summary>
        /// <param name="config">Configuration</param>
        /// <param name="info">Info</param>
        /// <param name="obj">Object</param>
        /// <param name="value">Value to serialize</param>
        /// <param name="stream">Stream</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public delegate Task AsyncSerializer_Delegate(
            IAutoStreamSerializerConfig config, 
            AutoStreamSerializerInfo info, 
            IAutoStreamSerializer obj, 
            object? value, 
            Stream stream, 
            CancellationToken cancellationToken
            );

        /// <summary>
        /// Delegate for a deserializer
        /// </summary>
        /// <param name="config">Configuration</param>
        /// <param name="info">Info</param>
        /// <param name="obj">Object</param>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <returns>Deserialized value</returns>
        public delegate object? Deserializer_Delegate(IAutoStreamSerializerConfig config, AutoStreamSerializerInfo info, IAutoStreamSerializer obj, Stream stream, int version);

        /// <summary>
        /// Delegate for a deserializer
        /// </summary>
        /// <param name="config">Configuration</param>
        /// <param name="info">Info</param>
        /// <param name="obj">Object</param>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Deserialized value</returns>
        public delegate Task<object?> AsyncDeserializer_Delegate(
            IAutoStreamSerializerConfig config, 
            AutoStreamSerializerInfo info, 
            IAutoStreamSerializer obj, 
            Stream stream, 
            int version, 
            CancellationToken cancellationToken
            );
    }
}
