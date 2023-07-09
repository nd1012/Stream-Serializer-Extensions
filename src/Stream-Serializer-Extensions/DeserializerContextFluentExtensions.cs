using System.Runtime;
using System.Runtime.CompilerServices;

namespace wan24.StreamSerializerExtensions
{
    /// <summary>
    /// <see cref="IDeserializationContext"/> fluent extensions
    /// </summary>
    public static class DeserializerContextFluentExtensions
    {
        /// <summary>
        /// Configure serializer options
        /// </summary>
        /// <typeparam name="tContext">Context type</typeparam>
        /// <typeparam name="tOptions">Options type</typeparam>
        /// <param name="context">Context</param>
        /// <param name="options">Options</param>
        /// <returns>Context</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static tContext WithOptions<tContext, tOptions>(this tContext context, tOptions? options)
            where tContext : IDeserializationContext
            where tOptions : ISerializerOptions
        {
            context.Options = options;
            return context;
        }

        /// <summary>
        /// Remove serializer options
        /// </summary>
        /// <typeparam name="T">Context type</typeparam>
        /// <param name="context">Context</param>
        /// <returns>Context</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T WithoutOptions<T>(this T context) where T : IDeserializationContext
        {
            context.Options = null;
            return context;
        }
    }
}
