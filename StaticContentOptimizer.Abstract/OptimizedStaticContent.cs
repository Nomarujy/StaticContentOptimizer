namespace StaticContentOptimizer.Abstract
{
    public sealed record OptimizedStaticContent
    {
        public OptimizedStaticContent(string uriPath, string contentType, DateTime lastModifed, byte[] data)
        {
            UriPath = uriPath;
            ContentType = contentType;
            LastModifed = lastModifed.ToString("R");

            ContentBytes = data;
        }

        public string UriPath { get; init; }
        public string ContentType { get; init; }
        public string LastModifed { get; init; }

        public byte[] ContentBytes { get; init; }

    }
}
