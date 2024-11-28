using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;

namespace StaticContentOptimizer.Abstract
{
    public abstract class ContentOptimizer(FileExtensionContentTypeProvider contentTypeProvider, IWebHostEnvironment environment)
    {
        private readonly FileExtensionContentTypeProvider _contentTypeProvider = contentTypeProvider;
        protected readonly string WebRoot = environment.WebRootPath;

        public abstract string[] SuportedContentTypes { get; }

        public abstract StaticContent[] GetOptimizedData(string filePath);

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
