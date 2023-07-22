namespace wan24.StreamSerializerExtensions
{
    /// <summary>
    /// Context recursion
    /// </summary>
    public readonly record struct ContextRecursion : IDisposable
    {
        /// <summary>
        /// Dispose action
        /// </summary>
        private readonly Action DisposeAction;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">Context</param>
        public ContextRecursion(ISerializerContext context)
        {
            context.RecursionLevel++;
            bool disposed = false;
            DisposeAction = () =>
            {
                if (disposed) return;
                disposed = true;
                context.RecursionLevel--;
            };
        }

        /// <inheritdoc/>
        public void Dispose() => DisposeAction();
    }
}
