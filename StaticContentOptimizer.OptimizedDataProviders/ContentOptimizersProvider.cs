using StaticContentOptimizer.Abstract;
using System.Net.Mime;

namespace StaticContentOptimizer.ContentOptimizers
{
    public sealed class ContentOptimizersProvider
    {
        private readonly Lazy<Dictionary<string, ContentOptimizer>> _sortedOptimizers;

        public ContentOptimizersProvider(IEnumerable<ContentOptimizer> optimizers)
        {
            _sortedOptimizers = new(() => SortOptimizers(optimizers));
        }

        private static Dictionary<string, ContentOptimizer> SortOptimizers(IEnumerable<ContentOptimizer> optimizers)
        {
            Dictionary<string, ContentOptimizer> optimyzersByContentType = [];

            foreach (var optimizer in optimizers)
            {
                foreach (var suportedContentType in optimizer.SuportedContentTypes)
                {
                    optimyzersByContentType[suportedContentType] = optimizer;
                }
            }

            return optimyzersByContentType;
        }

        public bool TryGet(string contentType, out ContentOptimizer value)
        {
            if (_sortedOptimizers.Value.TryGetValue(contentType, out value!))
            {
                return true;
            }

            return false;
        }
    }
}
