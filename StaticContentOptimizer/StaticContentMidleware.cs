using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using StaticContentOptimizer.Abstract;

namespace StaticContentOptimizer
{
    internal class StaticContentMidleware(StaticContentProvider provider, ILogger<StaticContentMidleware> logger) : IMiddleware
    {
        private readonly StaticContentProvider _provider = provider;
        private readonly ILogger<StaticContentMidleware> _logger = logger;

        private const int NotModifedStatusCode = 304;
        private const int SuccessStatusCode = 200;

        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            string requestPath = context.Request.GetEncodedPathAndQuery();

            if (_provider.TryGetValue(requestPath, out var value))
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

        private static async Task WriteRespnose(HttpContext context, StaticContent staticContent)
        {
            context.Response.StatusCode = SuccessStatusCode;
            context.Response.ContentType = staticContent.ContentType;
            context.Response.Headers.LastModified = staticContent.LastModifed;

            //TODO: CacheControl by IOptions

            await context.Response.BodyWriter.WriteAsync(staticContent.ContentBytes);
        }
    }
}
