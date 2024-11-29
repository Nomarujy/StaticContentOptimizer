using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using StaticContentOptimizer.Abstract;
using StaticContentOptimizer.ContentOptimizers;

namespace StaticContentOptimizer
{
    public static class MidlewareExtension
    {
        public static IServiceCollection AddStaticContentOptimizer(this IServiceCollection services)
        {
            services.AddScoped<ContentOptimizersProvider>()
                .AddScoped<ContentOptimizer, TextContentOptimizer>();


            return services
                .AddSingleton<IContentTypeProvider, FileExtensionContentTypeProvider>()
                .AddSingleton<StaticContentProvider>()
                .AddScoped<StaticContentFactory>()
                .AddScoped<StaticContentMidleware>()
                .AddHostedService<StaticContentService>();
        }

        public static IApplicationBuilder UseStaticContentOptimizer(this IApplicationBuilder app)
        {

            app.UseMiddleware<StaticContentMidleware>();

            return app;
        }
    }
}
