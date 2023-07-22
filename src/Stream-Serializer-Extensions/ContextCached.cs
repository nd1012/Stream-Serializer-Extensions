namespace wan24.StreamSerializerExtensions
{
    /// <summary>
    /// Enable caching in context temporary (if not caching)
    /// </summary>
    public readonly record struct ContextCached : IDisposable
    {
        /// <summary>
        /// Dispose action
        /// </summary>
        private readonly Action DisposeAction;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">Context</param>
        public ContextCached(ISerializerContext context)
        {
            bool disable = context.EnableCache(),
                disposed = false;
            DisposeAction = () =>
            {
                if (disposed) return;
                disposed = true;
                if (disable) context.DisableCache();
            };
        }

        /// <inheritdoc/>
        public void Dispose() => DisposeAction();
    }
}
