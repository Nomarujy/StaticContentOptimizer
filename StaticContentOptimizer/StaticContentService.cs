using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace StaticContentOptimizer
{
    internal class StaticContentService(IServiceProvider serviceProvider) : IHostedService
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var services = scope.ServiceProvider;
            var contentProvider = services.GetService<StaticContentProvider>()!;
            var contentFactory = services.GetService<StaticContentFactory>()!;

            var webRootPath = GetWebRootPath(scope);

            var filePaths = Directory.GetFiles(webRootPath, "*.*", SearchOption.AllDirectories);

            foreach (var filePath in filePaths)
            {
                UpdateStaticContent(filePath, contentProvider, contentFactory);
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private static string GetWebRootPath(IServiceScope scope)
        {
            var enviroment = scope.ServiceProvider.GetService<IWebHostEnvironment>()!;

            return enviroment.WebRootPath;
        }

        private static void UpdateStaticContent(string filePath, StaticContentProvider contentProvider, StaticContentFactory contentFactory)
        {
            var contents = contentFactory.BuildStaticContentForFile(filePath);

            foreach (var content in contents)
            {
                contentProvider.AddOrUpdate(content);
            }
        }
    }
}
