using StaticContentOptimizer.Abstract;

namespace StaticContentOptimizer.ContentOptimizers
{
    public sealed class ContentOptimizersProvider(IEnumerable<ContentOptimizer> optimizers)
    {
        private readonly Lazy<Dictionary<string, ContentOptimizer>> _sortedOptimizers = new(() => SortOptimizers(optimizers));

        private static Dictionary<string, ContentOptimizer> SortOptimizers(IEnumerable<ContentOptimizer> optimizers)
        {
            Dictionary<string, ContentOptimizer> optimyzersByContentType = [];

            foreach (var optimizer in optimizers)
            {
                foreach (var extension in optimizer.SuportedExtensions)
                {
                    optimyzersByContentType[extension] = optimizer;
                }
            }

            return optimyzersByContentType;
        }

        public bool TryGet(string fileExtension, out ContentOptimizer value)
            => _sortedOptimizers.Value.TryGetValue(fileExtension, out value!);
    }
}
