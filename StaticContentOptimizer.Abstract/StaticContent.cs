namespace StaticContentOptimizer.Abstract
{
    public class StaticContent
    {
        public StaticContent(string uriPath, string contentType, DateTime lastModifed, byte[] data)
        {
            UriPath = uriPath;
            ContentType = contentType;
            LastModifed = lastModifed.ToString("R");

            ContentBytes = data;
        }

        public string UriPath { get; set; }
        public string ContentType { get; set; }
        public string LastModifed { get; set; }

        public byte[] ContentBytes { get; set; }

    }
}
