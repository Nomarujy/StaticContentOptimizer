namespace StaticContentOptimizer.Abstract
{
    public abstract class DataOptimizer
    {
        public abstract string[] SuportedContentTypes { get; }

        public abstract OptimizedStaticContent[] Minify(string filePath);
    }
}
