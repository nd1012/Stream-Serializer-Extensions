namespace wan24.StreamSerializerExtensions
{
    /// <summary>
    /// <see cref="ISerializerOptions"/> fluent extensions
    /// </summary>
    public static class SerializerOptionsFluentExtensions
    {
        /// <summary>
        /// Configure key serializer options
        /// </summary>
        /// <typeparam name="T">Options type</typeparam>
        /// <param name="options">Options</param>
        /// <param name="keyOptions">Key options</param>
        /// <returns>Options</returns>
        public static T WithKeyOptions<T>(this T options, ISerializerOptions? keyOptions) where T : ISerializerOptions
        {
            options.KeyOptions = keyOptions;
            return options;
        }

        /// <summary>
        /// Configure value serializer options
        /// </summary>
        /// <typeparam name="T">Options type</typeparam>
        /// <param name="options">Options</param>
        /// <param name="valueOptions">value options</param>
        /// <returns>Options</returns>
        public static T WithValueOptions<T>(this T options, ISerializerOptions? valueOptions) where T : ISerializerOptions
        {
            options.ValueOptions = valueOptions;
            return options;
        }
    }
}
