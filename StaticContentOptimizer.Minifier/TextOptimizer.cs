using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using StaticContentOptimizer.Abstract;
using System.Text;

namespace StaticContentOptimizer.Minifier
{
    public sealed class TextOptimizer(FileExtensionContentTypeProvider contentTypeProvider, IWebHostEnvironment environment) : DataOptimizer
    {
        private readonly FileExtensionContentTypeProvider _contentTypeProvider = contentTypeProvider;
        private readonly string _webRootPath = environment.WebRootPath;

        private readonly string[] suportedContentTypes =
        [
            "text/plain",
            "text/plain",
            "text/css",
            "text/csv",
            "text/html",
            "text/javascript",
            "text/xml"
        ];

        public override string[] SuportedContentTypes => suportedContentTypes;

        public override OptimizedStaticContent[] Minify(string filePath)
        {
            if (_contentTypeProvider.TryGetContentType(filePath, out var contentType))
            {
                var uri = CreateUri(filePath);

                var bytes = GetBytes(filePath);

                var lastModifed = File.GetLastWriteTime(filePath);

                return [new OptimizedStaticContent(uri, contentType, lastModifed, bytes)];
            }

            return [];
        }

        private string CreateUri(string filePath)
        {
            return filePath.Replace(_webRootPath, string.Empty).Replace("\\", "/");
        }

        private static byte[] GetBytes(string filePath)
        {
            if (File.Exists(filePath) is false)
                ThrowFileNotFound(filePath);

            using FileStream file = File.OpenRead(filePath);
            using StreamReader reader = new(file);
            using MemoryStream memoryStream = new();

            CopyMinifiedStream(reader, memoryStream);

            return memoryStream.ToArray();
        }

        private static void ThrowFileNotFound(string path)
            => throw new FileNotFoundException(path);

        private static void CopyMinifiedStream(StreamReader source, MemoryStream output)
        {
            string? line;
            while ((line = source.ReadLine()) is not null)
            {

                var optimizedString = RemoveSpace(line);
                var bytes = Encoding.UTF8.GetBytes(optimizedString);

                output.Write(bytes);
            }
        }

        private static char[] NextCharToScip = ['{', '(', ',', '>'];
        private static char[] PrevCharToScip = [':', ',', '>'];

        private static string RemoveSpace(string line)
        {
            bool fastScip = true;
            StringBuilder stringBuilder = new();

            var lineLength = line.Length;
            for (int i = 0; i < lineLength; i++)
            {
                char character = line[i];

                if (character == ' ')
                {
                    if (fastScip)
                        continue;

                    char? nextChar = i + 1 < lineLength ? line[i + 1] : null;
                    char? prevChar = i - 1 > 0 ? line[i - 1] : null;

                    if (nextChar.HasValue && NextCharToScip.Contains(nextChar.Value) || prevChar.HasValue && PrevCharToScip.Contains(prevChar.Value))
                    {
                        continue;
                    }
                }

                if (character == ':')
                    fastScip = false;

                if (character == ';' || character == ')')
                    fastScip = true;

                stringBuilder.Append(character);
            }

            return stringBuilder.ToString();
        }
    }
}
