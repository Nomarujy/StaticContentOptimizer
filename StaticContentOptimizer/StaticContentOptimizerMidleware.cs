using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using StaticContentOptimizer.Abstract;

namespace StaticContentOptimizer
{
    public class StaticContentOptimizerMidleware(Dictionary<string, OptimizedStaticContent> staticContent, ILogger<StaticContentOptimizerMidleware> logger) : IMiddleware
    {
        private readonly Dictionary<string, OptimizedStaticContent> _staticContent = staticContent;
        private readonly ILogger<StaticContentOptimizerMidleware> _logger = logger;

        private const int NotModifedStatusCode = 304;
        private const int SuccessStatusCode = 200;

        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            string requestPath = context.Request.Path;

            if (_staticContent.TryGetValue(requestPath, out var value))
            {
                if (context.Request.Headers.IfModifiedSince == value.LastModifed)
                {
                    _logger.LogDebug("Sending 304 (File not midifed) for {path}", requestPath);

                    return WriteNotModifed(context);
                }
                else
                {
                    _logger.LogDebug("Sending file at path: {path}", requestPath);

                    return WriteRespnose(context, value);
                }
            }

            _logger.LogDebug("File not found at path {path}", requestPath);

            return next(context);
        }

        private static Task WriteNotModifed(HttpContext context)
        {
            context.Response.StatusCode = NotModifedStatusCode;
            return Task.CompletedTask;
        }

        private static async Task WriteRespnose(HttpContext context, OptimizedStaticContent staticContent)
        {
            context.Response.StatusCode = SuccessStatusCode;
            context.Response.ContentType = staticContent.ContentType;
            context.Response.Headers.LastModified = staticContent.LastModifed;

            //TODO: CacheControl by IOptions

            await context.Response.BodyWriter.WriteAsync(staticContent.ContentBytes);
        }
    }
}
