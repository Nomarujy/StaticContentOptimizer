using StaticContentOptimizer.Abstract;

namespace StaticContentOptimizer
{
    internal sealed class StaticContentProvider
    {
        private readonly Dictionary<string, StaticContent> _content = [];

        public bool TryGetValue(string uriPath, out StaticContent value)
            => _content.TryGetValue(uriPath, out value!);

        public void AddOrUpdate(StaticContent content)
        {
            _content[content.UriPath] = content;
        }
    }
}
