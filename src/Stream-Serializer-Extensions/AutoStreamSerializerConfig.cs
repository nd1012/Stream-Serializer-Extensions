using System.Reflection;
using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    /// <summary>
    /// Auto stream serializer configuration
    /// </summary>
    /// <typeparam name="T">Serialized object type</typeparam>
    public sealed class AutoStreamSerializerConfig<T> : IAutoStreamSerializerConfig where T : class, IAutoStreamSerializer
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="initDefaultValues">Initialize default values?</param>
        public AutoStreamSerializerConfig(bool initDefaultValues = true)
        {
            Type = typeof(T);
            // Stream serializer attribute required
            Attribute = Type.GetCustomAttribute<StreamSerializerAttribute>() ?? throw new InvalidProgramException($"{typeof(T)} is missing a {typeof(StreamSerializerAttribute)}");
            // Object version required
            if (Attribute.Version == null)
                throw new InvalidProgramException($"{typeof(StreamSerializerAttribute)}.{nameof(StreamSerializerAttribute.Version)} of {typeof(T)} requires a value");
            // Mode must be opt-in
            if (Attribute.Mode != StreamSerializerModes.OptIn)
                throw new InvalidProgramException($"{typeof(StreamSerializerAttribute)}.{nameof(StreamSerializerAttribute.Mode)} ({Attribute.Mode}) of {typeof(T)} not supported");
            // Collect property informations
            Infos = new(from pi in StreamSerializerAttribute.GetWriteProperties(Type, version: 0)
                        select new KeyValuePair<string, AutoStreamSerializerInfo>(pi.Property.Name, new AutoStreamSerializerInfo(pi)));
            // Initialize the default values
            if (Attribute.UseDefaultValues)
            {
                DefaultValues = new();
                if (initDefaultValues) InitDefaultValues();
            }
            else
            {
                DefaultValues = null;
            }
        }

        /// <inheritdoc/>
        public Type Type { get; }

        /// <inheritdoc/>
        public StreamSerializerAttribute Attribute { get; }

        /// <inheritdoc/>
        public OrderedDictionary<string, AutoStreamSerializerInfo> Infos { get; }

        /// <inheritdoc/>
        public Dictionary<string, object?>? DefaultValues { get; }

        /// <summary>
        /// Serialize
        /// </summary>
        /// <param name="obj">Object</param>
        /// <param name="stream">Stream</param>
        public void Serialize(T obj, Stream stream)
        {
            (PropertyInfoExt[] properties, List<string>? usedDefaultValue, byte[]? defaultValueBits, int? defaultValueBitsLength) = PrepareSerialization(obj);
            if (defaultValueBits != null)
                try
                {
                    stream.Write(defaultValueBits.AsSpan()[..defaultValueBitsLength!.Value]);
                }
                finally
                {
                    StreamSerializer.BufferPool.Return(defaultValueBits);
                }
            foreach (PropertyInfoExt pi in properties)
            {
                if (usedDefaultValue?.Contains(pi.Property.Name) ?? false) continue;
                if (!Infos.TryGetValue(pi.Property.Name, out AutoStreamSerializerInfo? info))
                    throw new SerializerException($"Missing auto stream serializer information for {obj.GetType()}.{pi.Property.Name}", new InvalidProgramException());
                info.Serialize(this, obj, stream);
            }
        }

        /// <summary>
        /// Serialize
        /// </summary>
        /// <param name="obj">Object</param>
        /// <param name="stream">Stream</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public async Task SerializeAsync(T obj, Stream stream, CancellationToken cancellationToken)
        {
            (PropertyInfoExt[] properties, List<string>? usedDefaultValue, byte[]? defaultValueBits, int? defaultValueBitsLength) = PrepareSerialization(obj);
            if (defaultValueBits != null)
                try
                {
                    await stream.WriteAsync(defaultValueBits.AsMemory()[..defaultValueBitsLength!.Value], cancellationToken).DynamicContext();
                }
                finally
                {
                    StreamSerializer.BufferPool.Return(defaultValueBits);
                }
            foreach (PropertyInfoExt pi in properties)
            {
                if (usedDefaultValue?.Contains(pi.Property.Name) ?? false) continue;
                if (!Infos.TryGetValue(pi.Property.Name, out AutoStreamSerializerInfo? info))
                    throw new SerializerException($"Missing auto stream serializer information for {obj.GetType()}.{pi.Property.Name}", new InvalidProgramException());
                await info.SerializeAsync(this, obj, stream, cancellationToken).DynamicContext();
            }
        }

        /// <summary>
        /// Deserialize
        /// </summary>
        /// <param name="obj">Object</param>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        public void Deserialize(T obj, Stream stream, int version)
        {
            PropertyInfoExt[] properties = StreamSerializerAttribute.GetReadProperties(obj.GetType(), version).ToArray();
            List<string>? usedDefaultValue = null;
            if (DefaultValues != null)
            {
                PropertyInfoExt[] props = properties
                    .Where(p => DefaultValues.ContainsKey(p.Property.Name) && Infos[p.Property.Name].Attribute.GetUseDefaultValue(Attribute.Version!.Value))
                    .ToArray();
                usedDefaultValue = PrepareDeserialization(
                    obj, 
                    props, 
                    StreamExtensions.ReadSerializedData(stream, (int)Math.Ceiling((decimal)props.Length / 8), StreamSerializer.BufferPool)
                    );
            }
            foreach (PropertyInfoExt pi in properties)
            {
                if (usedDefaultValue?.Contains(pi.Property.Name) ?? false) continue;
                if (!Infos.TryGetValue(pi.Property.Name, out AutoStreamSerializerInfo? info))
                    throw new SerializerException($"Missing auto stream serializer information for {obj.GetType()}.{pi.Property.Name}", new InvalidProgramException());
                info.Deserialize(this, obj, stream, version);
            }
        }

        /// <summary>
        /// Deserialize
        /// </summary>
        /// <param name="obj">Object</param>
        /// <param name="stream">Stream</param>
        /// <param name="version">Serializer version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public async Task DeserializeAsync(T obj, Stream stream, int version, CancellationToken cancellationToken)
        {
            PropertyInfoExt[] properties = StreamSerializerAttribute.GetReadProperties(obj.GetType(), version).ToArray();
            List<string>? usedDefaultValue = null;
            if (DefaultValues != null)
            {
                PropertyInfoExt[] props = properties
                    .Where(p => DefaultValues.ContainsKey(p.Property.Name) && Infos[p.Property.Name].Attribute.GetUseDefaultValue(Attribute.Version!.Value))
                    .ToArray();
                usedDefaultValue = PrepareDeserialization(
                    obj,
                    props,
                    await StreamExtensions.ReadSerializedDataAsync(stream, (int)Math.Ceiling((decimal)props.Length / 8), StreamSerializer.BufferPool, cancellationToken)
                        .DynamicContext()
                    );
            }
            foreach (PropertyInfo pi in properties)
            {
                if (usedDefaultValue?.Contains(pi.Name) ?? false) continue;
                if (!Infos.TryGetValue(pi.Name, out AutoStreamSerializerInfo? info))
                    throw new SerializerException($"Missing auto stream serializer information for {obj.GetType()}.{pi.Name}", new InvalidProgramException());
                await info.DeserializeAsync(this, obj, stream, version, cancellationToken).DynamicContext();
            }
        }

        /// <summary>
        /// Initialize the default values
        /// </summary>
        public void InitDefaultValues()
        {
            if (DefaultValues == null) return;
            object instance = Type.ConstructAuto(usePrivate: true)
                ?? throw new SerializerException($"Can't instance {Type} for getting the default values", new InvalidProgramException());
            foreach (AutoStreamSerializerInfo info in Infos.Values)
                if (info.Attribute.UseDefaultValues)
                    DefaultValues[info.Property.Property.Name] = info.Property.Getter!(instance);
        }

        /// <summary>
        /// Prepare the serialization
        /// </summary>
        /// <param name="obj">Serialized object</param>
        /// <returns>Properties, used default value property names, default value bits (needs to be returned to the <see cref="StreamSerializer.BufferPool"/>) and their length 
        /// in bytes</returns>
        private (PropertyInfoExt[] Properties, List<string>? UsedDefaultValue, byte[]? DefaultValueBits, int? DefaultBitsLength) PrepareSerialization(T obj)
        {
            PropertyInfoExt[] properties = StreamSerializerAttribute.GetWriteProperties(obj.GetType()).ToArray();
            List<string>? usedDefaultValue = null;
            byte[]? defaultValueBits = null;
            int? defaultValueBitsLength = 0;
            if (DefaultValues == null) return (properties, usedDefaultValue, defaultValueBits, defaultValueBitsLength);
            usedDefaultValue = new();
            bool usedDefault;
            PropertyInfoExt[] props = properties
                .Where(p => DefaultValues.ContainsKey(p.Property.Name) && Infos[p.Property.Name].Attribute.GetUseDefaultValue(Attribute.Version!.Value))
                .ToArray();
            defaultValueBitsLength = (int)Math.Ceiling((decimal)props.Length / 8);
            defaultValueBits = StreamSerializer.BufferPool.RentClean(defaultValueBitsLength.Value);
            try
            {
                Bitmap bits = new(defaultValueBits, bitCount: 0);
                foreach (PropertyInfoExt pi in props)
                {
                    bits.AddBits(usedDefault = ObjectHelper.AreEqual(pi.Getter!(obj), DefaultValues[pi.Property.Name]));
                    if (usedDefault || (!pi.Property.IsNullable() && pi.Property.PropertyType == typeof(bool)))
                        usedDefaultValue.Add(pi.Property.Name);
                }
            }
            catch
            {
                StreamSerializer.BufferPool.Return(defaultValueBits);
                throw;
            }
            return (properties, usedDefaultValue, defaultValueBits, defaultValueBitsLength);
        }

        /// <summary>
        /// Prepare the deserialization
        /// </summary>
        /// <param name="obj">Serialized object</param>
        /// <param name="properties">Properties</param>
        /// <param name="defaultValueBits">Default value bits (needs to be returned to the <see cref="StreamSerializer.BufferPool"/>)</param>
        /// <returns>Properties, used default value property names and default value bits</returns>
        private List<string>? PrepareDeserialization(T obj, PropertyInfoExt[] properties, byte[]? defaultValueBits)
        {
            List<string>? usedDefaultValue = null;
            if (DefaultValues == null) return usedDefaultValue;
            ArgumentValidationHelper.EnsureValidArgument(nameof(defaultValueBits), defaultValueBits);
            usedDefaultValue = new();
            try
            {
                Bitmap bits = new(defaultValueBits);
                PropertyInfoExt pi;
                for (int i = 0, len = properties.Length; i < len; i++)
                {
                    pi = properties[i];
                    if (bits[i])
                    {
                        usedDefaultValue.Add(pi.Property.Name);
                    }
                    else if (!pi.Property.IsNullable() && pi.Property.PropertyType == typeof(bool))
                    {
                        usedDefaultValue.Add(pi.Property.Name);
                        pi.Setter!(obj, !(bool)pi.Getter!(obj)!);
                    }
                }
            }
            finally
            {
                StreamSerializer.BufferPool.Return(defaultValueBits);
            }
            return usedDefaultValue;
        }
    }
}
