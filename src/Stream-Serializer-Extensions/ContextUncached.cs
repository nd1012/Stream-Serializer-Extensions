namespace wan24.StreamSerializerExtensions
{
    /// <summary>
    /// Disable caching in context temporary (if caching)
    /// </summary>
    public readonly record struct ContextUncached : IDisposable
    {
        /// <summary>
        /// Dispose action
        /// </summary>
        private readonly Action DisposeAction;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">Context</param>
        public ContextUncached(ISerializerContext context)
        {
            bool enable = context.DisableCache(),
                disposed = false;
            DisposeAction = () =>
            {
                if (disposed) return;
                disposed = true;
                if (enable) context.EnableCache();
            };
        }

        /// <inheritdoc/>
        public void Dispose() => DisposeAction();
    }
}
