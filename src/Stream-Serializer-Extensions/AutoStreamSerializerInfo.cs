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
        /// <param name="context">Context</param>
        public void Serialize<T>(IAutoStreamSerializerConfig config, T obj, ISerializationContext context) where T : IAutoStreamSerializer
        {
            if (Serializer == null)
            {
                if (IsNullable)
                {
                    context.Stream.WriteObjectNullable(Property.Getter!(obj), context);
                }
                else
                {
                    context.Stream.WriteObject(
                        Property.Getter!(obj)
                            ?? throw new SerializerException($"{Property.Property.DeclaringType}.{Property.Property.Name} value is NULL", new InvalidDataException()),
                        context
                        );
                }
            }
            else
            {
                Serializer(config, this, obj, Property.Getter!(obj), context);
            }
        }

        /// <summary>
        /// Serialize
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="config">Configuration</param>
        /// <param name="obj">Object</param>
        /// <param name="context">Context</param>
        public async Task SerializeAsync<T>(IAutoStreamSerializerConfig config, T obj, ISerializationContext context) where T : IAutoStreamSerializer
        {
            if (AsyncSerializer == null)
            {
                if (IsNullable)
                {
                    await context.Stream.WriteObjectNullableAsync(Property.Getter!(obj), context).DynamicContext();
                }
                else
                {
                    await context.Stream.WriteObjectAsync(
                        Property.Getter!(obj)
                            ?? throw new SerializerException($"{Property.Property.DeclaringType}.{Property.Property.Name} value is NULL", new InvalidDataException()),
                        context
                        ).DynamicContext();
                }
            }
            else
            {
                await AsyncSerializer(config, this, obj, Property.Getter!(obj), context).DynamicContext();
            }
        }

        /// <summary>
        /// Deserialize
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="config">Configuration</param>
        /// <param name="obj">Object</param>
        /// <param name="context">Context</param>
        public void Deserialize<T>(IAutoStreamSerializerConfig config, T obj, IDeserializationContext context) where T : IAutoStreamSerializer
        {
            if (Deserializer == null)
            {
                ISerializerOptions? options = Property.GetSerializerOptions(context);
                context.WithOptions(options);
                try
                {
                    Property.Setter!(obj, IsNullable
                        ? context.Stream.ReadObjectNullable(Property.Property.PropertyType, context)
                        : context.Stream.ReadObject(Property.Property.PropertyType, context));
                }
                finally
                {
                    context.WithoutOptions();
                }
            }
            else
            {
                Property.Setter!(obj, Deserializer(config, this, obj, context));
            }
        }

        /// <summary>
        /// Deserialize
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="config">Configuration</param>
        /// <param name="obj">Object</param>
        /// <param name="context">Context</param>
        public async Task DeserializeAsync<T>(IAutoStreamSerializerConfig config, T obj, IDeserializationContext context)
            where T : IAutoStreamSerializer
        {
            if (AsyncDeserializer == null)
            {
                ISerializerOptions? options = Property.GetSerializerOptions(context);
                context.WithOptions(options);
                try
                {
                    Property.Setter!(obj, IsNullable
                        ? await context.Stream.ReadObjectNullableAsync(Property.Property.PropertyType, context).DynamicContext()
                        : await context.Stream.ReadObjectAsync(Property.Property.PropertyType, context).DynamicContext());
                }
                finally
                {
                    context.WithoutOptions();
                }
            }
            else
            {
                Property.Setter!(obj, await AsyncDeserializer(config, this, obj, context).DynamicContext());
            }
        }

        /// <summary>
        /// Delegate for a serializer
        /// </summary>
        /// <param name="config">Configuration</param>
        /// <param name="info">Info</param>
        /// <param name="obj">Object</param>
        /// <param name="value">Value</param>
        /// <param name="context">Context</param>
        public delegate void Serializer_Delegate(
            IAutoStreamSerializerConfig config,
            AutoStreamSerializerInfo info,
            IAutoStreamSerializer obj,
            object? value,
            ISerializationContext context
            );

        /// <summary>
        /// Delegate for a serializer
        /// </summary>
        /// <param name="config">Configuration</param>
        /// <param name="info">Info</param>
        /// <param name="obj">Object</param>
        /// <param name="value">Value</param>
        /// <param name="context">Context</param>
        public delegate Task AsyncSerializer_Delegate(
            IAutoStreamSerializerConfig config,
            AutoStreamSerializerInfo info,
            IAutoStreamSerializer obj,
            object? value,
            ISerializationContext context
            );

        /// <summary>
        /// Delegate for a deserializer
        /// </summary>
        /// <param name="config">Configuration</param>
        /// <param name="info">Info</param>
        /// <param name="obj">Object</param>
        /// <param name="context">Context</param>
        /// <returns>Deserialized value</returns>
        public delegate object? Deserializer_Delegate(IAutoStreamSerializerConfig config, AutoStreamSerializerInfo info, IAutoStreamSerializer obj, IDeserializationContext context);

        /// <summary>
        /// Delegate for a deserializer
        /// </summary>
        /// <param name="config">Configuration</param>
        /// <param name="info">Info</param>
        /// <param name="obj">Object</param>
        /// <param name="context">Context</param>
        /// <returns>Deserialized value</returns>
        public delegate Task<object?> AsyncDeserializer_Delegate(
            IAutoStreamSerializerConfig config,
            AutoStreamSerializerInfo info,
            IAutoStreamSerializer obj,
            IDeserializationContext context
            );
    }
}
