using System.Threading;
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
        public AutoStreamSerializerInfo(PropertyInfoExt property)
        {
            Property = property;
            IsNullable = property.Property.IsNullable();
            Attribute = property.Property.GetCustomAttributeCached<StreamSerializerAttribute>() 
                ?? throw new ArgumentException($"Missing {typeof(StreamSerializerAttribute)}", nameof(property));
        }

        /// <summary>
        /// Property
        /// </summary>
        public PropertyInfoExt Property { get; }

        /// <summary>
        /// Is the property value nullable?
        /// </summary>
        public bool IsNullable { get; }

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
                if (IsNullable)
                {
                    stream.WriteObjectNullable(Property.Getter!(obj));
                }
                else
                {
                    stream.WriteObject(
                        Property.Getter!(obj) 
                        ?? throw new SerializerException($"{Property.Property.DeclaringType}.{Property.Property.Name} value is NULL", new InvalidDataException())
                        );
                }
            }
            else
            {
                Serializer(config, this, obj, Property.Getter!(obj), stream);
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
                if (IsNullable)
                {
                    await stream.WriteObjectNullableAsync(Property.Getter!(obj), cancellationToken).DynamicContext();
                }
                else
                {
                    await stream.WriteObjectAsync(
                        Property.Getter!(obj) ?? throw new SerializerException($"{Property.Property.DeclaringType}.{Property.Property.Name} value is NULL", new InvalidDataException()),
                        cancellationToken
                        ).DynamicContext();
                }
            }
            else
            {
                await AsyncSerializer(config, this, obj, Property.Getter!(obj), stream, cancellationToken).DynamicContext();
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
                ISerializerOptions? options = Property.Property.GetSerializerOptions(stream, version, default);
                Property.Setter!(obj, IsNullable
                    ? stream.ReadObjectNullable(Property.Property.PropertyType, version, options)
                    : stream.ReadObject(Property.Property.PropertyType, version, options));
            }
            else
            {
                Property.Setter!(obj, Deserializer(config, this, obj, stream, version));
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
        public async Task DeserializeAsync<T>(IAutoStreamSerializerConfig config, T obj, Stream stream, int version, CancellationToken cancellationToken)
            where T : IAutoStreamSerializer
        {
            if (AsyncDeserializer == null)
            {
                ISerializerOptions? options = Property.Property.GetSerializerOptions(stream, version, cancellationToken);
                Property.Setter!(obj, IsNullable
                    ? await stream.ReadObjectNullableAsync(Property.Property.PropertyType, version, options, cancellationToken).DynamicContext()
                    : await stream.ReadObjectAsync(Property.Property.PropertyType, version, options, cancellationToken).DynamicContext());
            }
            else
            {
                Property.Setter!(obj, await AsyncDeserializer(config, this, obj, stream, version, cancellationToken).DynamicContext());
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
