using Microsoft.AspNetCore.Http;

namespace StaticContentOptimizer.Abstract
{
    public abstract class ContentOptimizer
    {
        public abstract string[] SuportedExtensions { get; }

        public abstract Dictionary<QueryString, byte[]> GetOptimizedData(string filePath);
    }
}
