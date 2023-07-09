namespace wan24.StreamSerializerExtensions
{
    /// <summary>
    /// Context recursion (should only be disposed once!)
    /// </summary>
    public readonly record struct ContextRecursion : IDisposable
    {
        /// <summary>
        /// Dispose action
        /// </summary>
        private readonly ISerializerContext Context;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">Context</param>
        public ContextRecursion(ISerializerContext context)
        {
            context.RecursionLevel++;
            Context = context;
        }

        /// <inheritdoc/>
        public void Dispose() => Context.RecursionLevel--;
    }
}
