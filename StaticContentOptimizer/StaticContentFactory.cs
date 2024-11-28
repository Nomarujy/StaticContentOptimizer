using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using StaticContentOptimizer.Abstract;

namespace StaticContentOptimizer
{
    internal class StaticContentFactory
    {
        private readonly IServiceProvider _services;
        private readonly string webRootPath;
        private readonly FileExtensionContentTypeProvider _contentTypeProvider;

        public StaticContentFactory(IServiceProvider serviceProvider)
        {
            _services = serviceProvider;
            webRootPath = serviceProvider.GetService<IWebHostEnvironment>()!.WebRootPath;
            _contentTypeProvider = serviceProvider.GetService<FileExtensionContentTypeProvider>()!;

        }
        public Dictionary<string, OptimizedStaticContent> Build()
        {
            var optimyzersByExtension = GetSortedOptimizers();
            var files = Directory.GetFiles(webRootPath, "*.*", SearchOption.AllDirectories);

            Dictionary<string, OptimizedStaticContent> result = [];

            foreach (var filePath in files)
            {
                if (_contentTypeProvider.TryGetContentType(filePath, out var contentType) && optimyzersByExtension.TryGetValue(contentType, out var optimizer))
                {
                    var content = optimizer.GetOptimizedData(filePath);

                    foreach (var file in content)
                    {
                        result[file.UriPath] = file;
                    }
                }
            }
            return result;
        }

        private Dictionary<string, OptimizedDataProvider> GetSortedOptimizers()
        {
            var optimizers = _services.GetServices<OptimizedDataProvider>();

            Dictionary<string, OptimizedDataProvider> optimyzersByContentType = [];
            foreach (var optimizer in optimizers)
            {
                foreach (var suportedContentType in optimizer.SuportedContentTypes)
                {
                    optimyzersByContentType[suportedContentType] = optimizer;
                }
            }

            return optimyzersByContentType;
        }
    }
}
