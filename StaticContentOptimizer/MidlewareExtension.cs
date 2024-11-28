using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using StaticContentOptimizer.Abstract;
using StaticContentOptimizer.OptimizedDataProviders;

namespace StaticContentOptimizer
{
    public static class MidlewareExtension
    {

        public static IServiceCollection AddStaticContentOptimizer(this IServiceCollection services)
        {
            services.AddScoped<OptimizedDataProvider, TextFilesProvider>();
            services.AddScoped<FileExtensionContentTypeProvider>();
            services.AddScoped<StaticContentFactory>();

            return services
                .AddStaticContent()
                .AddScoped<StaticContentOptimizerMidleware>();
        }

        public static IApplicationBuilder UseStaticContentOptimizer(this IApplicationBuilder app)
        {
            app.UseMiddleware<StaticContentOptimizerMidleware>();

            return app;
        }

        private static IServiceCollection AddStaticContent(this IServiceCollection services)
        {
            using var provider = services.BuildServiceProvider();
            using var scope = provider.CreateScope();
            var factory = scope.ServiceProvider.GetService<StaticContentFactory>()!;

            return services.AddSingleton(factory.Build());
        }
    }
}
