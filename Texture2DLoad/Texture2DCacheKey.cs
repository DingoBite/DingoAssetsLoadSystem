using System;

namespace DingoAssetsLoadSystem.Texture2DLoad
{
    public readonly struct Texture2DCacheKey : IEquatable<Texture2DCacheKey>
    {
        public readonly string Path;
        public readonly Texture2DLoadInfo Info;

        public Texture2DCacheKey(string path, Texture2DLoadInfo info)
        {
            Path = path;
            Info = info;
        }

        public bool Equals(Texture2DCacheKey other) => string.Equals(Path, other.Path, StringComparison.Ordinal) && Info.Equals(other.Info);

        public override bool Equals(object obj) => obj is Texture2DCacheKey other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                int h = 17;
                h = (h * 31) ^ (Path != null ? StringComparer.Ordinal.GetHashCode(Path) : 0);
                h = (h * 31) ^ Info.GetHashCode();
                return h;
            }
        }
    }
}