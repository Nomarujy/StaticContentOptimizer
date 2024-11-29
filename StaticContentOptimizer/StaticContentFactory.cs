using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using StaticContentOptimizer.Abstract;
using StaticContentOptimizer.ContentOptimizers;

namespace StaticContentOptimizer
{
    internal class StaticContentFactory(IFileVersionProvider versionProvider,
        IContentTypeProvider contentTypeProvider, ILogger<StaticContentFactory> logger,
        ContentOptimizersProvider optimizersProvider, IWebHostEnvironment environment)
    {
        private readonly IFileVersionProvider _versionProvider = versionProvider;
        private readonly IContentTypeProvider _contentTypeProvider = contentTypeProvider;
        private readonly ILogger<StaticContentFactory> _logger = logger;
        private readonly ContentOptimizersProvider _optimizersProvider = optimizersProvider;
        private readonly string _webRootPath = environment.WebRootPath;

        public StaticContent[] BuildStaticContentForFile(string filePath)
        {
            Dictionary<QueryString, byte[]> optimizedData;

            if (_optimizersProvider.TryGet(Path.GetExtension(filePath), out var optimizer))
            {
                optimizedData = optimizer.GetOptimizedData(filePath);
            }
            else
            {
                optimizedData = GetDefaultData(filePath);
            }

            //TODO: Exlude static by options;
            return CreateStaticContent(filePath, optimizedData);
        }

        private static Dictionary<QueryString, byte[]> GetDefaultData(string filePath)
        {
            using var fileStream = File.OpenRead(filePath);
            var buffer = new byte[fileStream.Length];

            fileStream.ReadExactly(buffer);

            return new() { { QueryString.Empty, buffer } };
        }

        private StaticContent[] CreateStaticContent(string filePath, Dictionary<QueryString, byte[]> data)
        {
            string baseUri = GetBaseUri(filePath);
            string uriWithVersion = _versionProvider.AddFileVersionToPath(baseUri, baseUri);
            string contentType = GetContentType(filePath);
            DateTime lastModifed = File.GetLastWriteTimeUtc(filePath);

            LogCreating(filePath, baseUri);
            LogCreating(filePath, uriWithVersion);

            StaticContent[] result = new StaticContent[data.Count * 2];

            for (int i = 0; i < data.Count; i++)
            {
                var keyPair = data.ElementAt(i);
                var query = keyPair.Key;
                var fileData = keyPair.Value;

                result[i * 2] = new StaticContent(AppendQuery(baseUri, query), contentType, lastModifed, fileData);
                result[i * 2 + 1] = new StaticContent(AppendQuery(uriWithVersion, query), contentType, lastModifed, fileData);
            }

            return result;
        }

        private string GetBaseUri(string filePath)
        {
            return filePath.Replace(_webRootPath, string.Empty)
                .Replace("\\", "/");
        }

        private string GetContentType(string filePath)
        {
            if (_contentTypeProvider.TryGetContentType(filePath, out var contentType))
                return contentType;
            return "application/octet-stream";
        }

        private void LogCreating(string filePath, string uri)
        {
            _logger.LogDebug("Creating {file} at path: {path}", Path.GetFileName(filePath), uri);
        }

        private static string AppendQuery(string uri, QueryString query)
        {
            string queryString = query.ToString();

            if (string.IsNullOrEmpty(queryString))
                return uri;

            if (uri.Contains('?'))
            {
                return $"{uri}&{queryString[1..]}";
            }

            return $"{uri}{queryString}";
        }
    }
}
