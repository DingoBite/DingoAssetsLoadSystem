using System;

namespace DingoAssetsLoadSystem.SpriteLoad
{
    public readonly struct SpriteCacheKey : IEquatable<SpriteCacheKey>
    {
        public readonly string Path;
        public readonly SpriteLoadInfo Info;

        public SpriteCacheKey(string path, SpriteLoadInfo info)
        {
            Path = path;
            Info = info;
        }

        public bool Equals(SpriteCacheKey other) =>
            string.Equals(Path, other.Path, StringComparison.Ordinal) && Info.Equals(other.Info);

        public override bool Equals(object obj) => obj is SpriteCacheKey other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = (hash * 31) ^ (Path != null ? StringComparer.Ordinal.GetHashCode(Path) : 0);
                hash = (hash * 31) ^ Info.GetHashCode();
                return hash;
            }
        }
    }
}
