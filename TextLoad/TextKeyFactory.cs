using System;
using System.IO;

namespace DingoAssetsLoadSystem.TextLoad
{
    public sealed class TextKeyFactory : ICacheKeyFactory<TextCacheKey, TextLoadInfo>
    {
        public TextCacheKey CreateKey(string path, TextLoadInfo info)
        {
            var normalized = NormalizePathKey(path);

            if (info.UseFileStampInKey && IsLocalFilePath(normalized))
            {
                try
                {
                    var fi = new FileInfo(normalized);
                    if (fi.Exists)
                        return new TextCacheKey(normalized, info, true, fi.Length, fi.LastWriteTimeUtc.Ticks);
                }
                catch { }
            }

            return new TextCacheKey(normalized, info, false, 0, 0);
        }

        private static bool IsLocalFilePath(string path)
        {
            if (Uri.TryCreate(path, UriKind.Absolute, out var uri))
                return uri.IsFile;
            return true;
        }

        private static string NormalizePathKey(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return string.Empty;

            if (Uri.TryCreate(path, UriKind.Absolute, out var uri) && !uri.IsFile)
                return path;

            try
            {
                return Path.GetFullPath(path);
            }
            catch
            {
                return path;
            }
        }
    }
}