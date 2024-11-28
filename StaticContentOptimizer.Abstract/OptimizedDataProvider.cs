using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;

namespace StaticContentOptimizer.Abstract
{
    public abstract class OptimizedDataProvider
    {
        protected OptimizedDataProvider(FileExtensionContentTypeProvider contentTypeProvider, IWebHostEnvironment environment)
        {
            _contentTypeProvider = contentTypeProvider;
            WebRoot = environment.WebRootPath;
        }

        private readonly FileExtensionContentTypeProvider _contentTypeProvider;
        protected readonly string WebRoot;

        public abstract string[] SuportedContentTypes { get; }

        public abstract OptimizedStaticContent[] GetOptimizedData(string filePath);

        protected string GetRelativePath(string filePath)
        {
            return filePath.Replace(WebRoot, string.Empty).Replace("\\", "/");
        }

        protected string? GetContentType(string filePath)
        {
            if (_contentTypeProvider.TryGetContentType(filePath, out var contentType))
            {
                return contentType;
            }

            return null;
        }
    }
}
