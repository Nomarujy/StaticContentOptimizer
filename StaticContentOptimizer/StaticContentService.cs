using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StaticContentOptimizer.Abstract;
using StaticContentOptimizer.ContentOptimizers;

namespace StaticContentOptimizer
{
    internal class StaticContentService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly string _webRootPath;
        private readonly StaticContentProvider _contentProvider;

        public StaticContentService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _webRootPath = serviceProvider.GetService<IWebHostEnvironment>()!.WebRootPath!;
            _contentProvider = serviceProvider.GetService<StaticContentProvider>()!;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var filePaths = Directory.GetFiles(_webRootPath, "*.*", SearchOption.AllDirectories);

            foreach (var filePath in filePaths)
            {
                UpdateStaticContent(filePath);
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private void UpdateStaticContent(string filePath)
        {
            var contents = GetStaticContent(filePath);

            foreach (var content in contents)
            {
                _contentProvider.AddOrUpdate(content);
            };
        }

        private StaticContent[] GetStaticContent(string filePath)
        {
            using var scope = _serviceProvider.CreateScope();
            var contentTypeProvider = scope.ServiceProvider.GetService<FileExtensionContentTypeProvider>()!;
            var optimizerProvider = scope.ServiceProvider.GetService<ContentOptimizersProvider>()!;

            if (contentTypeProvider.TryGetContentType(filePath, out var contentType)
                && optimizerProvider.TryGet(contentType, out var optimizer))
            {
                return optimizer.GetOptimizedData(filePath);
            }

            return [];
        }
    }
}
