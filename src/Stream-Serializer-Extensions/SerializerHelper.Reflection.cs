using System.Reflection;
using System.Runtime;
using System.Runtime.CompilerServices;
using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    // Reflection
    public static partial class SerializerHelper
    {
        /// <summary>
        /// Get the serializer options
        /// </summary>
        /// <param name="pi">Property</param>
        /// <param name="context">Context</param>
        /// <returns>Serializer options</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ISerializerOptions? GetSerializerOptions(this PropertyInfoExt pi, ISerializerContext context)
            => pi.GetCustomAttributeCached<StreamSerializerAttribute>()?.GetSerializerOptions(pi, context);

        /// <summary>
        /// Get the key serializer options
        /// </summary>
        /// <param name="pi">Property</param>
        /// <param name="context">Context</param>
        /// <returns>Serializer options</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ISerializerOptions? GetKeySerializerOptions(this PropertyInfoExt pi, ISerializerContext context)
            => pi.GetCustomAttributeCached<StreamSerializerAttribute>()?.GetKeySerializerOptions(pi, context);

        /// <summary>
        /// Get the value serializer options
        /// </summary>
        /// <param name="pi">Property</param>
        /// <param name="context">Context</param>
        /// <returns>Serializer options</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ISerializerOptions? GetValueSerializerOptions(this PropertyInfoExt pi, ISerializerContext context)
            => pi.GetCustomAttributeCached<StreamSerializerAttribute>()?.GetValueSerializerOptions(pi, context);

        /// <summary>
        /// Determine if the constructor is the serializer constructor
        /// </summary>
        /// <param name="ci">Constructor</param>
        /// <param name="requireAttribute">Require the <see cref="StreamSerializerAttribute"/>?</param>
        /// <returns>Is the serializer constructor?</returns>
        [TargetedPatchingOptOut("Tiny method")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSerializerConstructor(this ConstructorInfo ci, bool requireAttribute = false)
        {
            ParameterInfo[] pis = ci.GetParametersCached();
            return pis.Select(p => p.ParameterType).ContainsAll(typeof(Stream), typeof(int)) &&
                pis[0].ParameterType == typeof(Stream) &&
                pis[1].ParameterType == typeof(int) &&
                (!requireAttribute || ci.GetCustomAttributeCached<StreamSerializerAttribute>() is not null);
        }
    }
}
