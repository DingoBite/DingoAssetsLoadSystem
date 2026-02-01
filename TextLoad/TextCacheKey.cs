using System;

namespace DingoAssetsLoadSystem.TextLoad
{
    public readonly struct TextCacheKey : IEquatable<TextCacheKey>
    {
        public readonly string Path;
        public readonly TextLoadInfo Info;
        public readonly long Length;
        public readonly long LastWriteTicksUtc;
        public readonly bool HasStamp;

        public TextCacheKey(string path, TextLoadInfo info, bool hasStamp, long length, long lastWriteTicksUtc)
        {
            Path = path;
            Info = info;
            HasStamp = hasStamp;
            Length = length;
            LastWriteTicksUtc = lastWriteTicksUtc;
        }

        public bool Equals(TextCacheKey other) =>
            string.Equals(Path, other.Path, StringComparison.Ordinal) && Info.Equals(other.Info) && HasStamp == other.HasStamp && Length == other.Length && LastWriteTicksUtc == other.LastWriteTicksUtc;

        public override bool Equals(object obj) => obj is TextCacheKey other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                int h = 17;
                h = (h * 31) ^ (Path != null ? StringComparer.Ordinal.GetHashCode(Path) : 0);
                h = (h * 31) ^ Info.GetHashCode();
                h = (h * 31) ^ HasStamp.GetHashCode();
                h = (h * 31) ^ Length.GetHashCode();
                h = (h * 31) ^ LastWriteTicksUtc.GetHashCode();
                return h;
            }
        }
    }
}