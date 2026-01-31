using System;

namespace DingoAssetsLoadSystem.MeshLoad
{
    public readonly struct MeshCacheKey : IEquatable<MeshCacheKey>
    {
        public readonly string Path;
        public readonly MeshLoadInfo Info;

        public MeshCacheKey(string path, MeshLoadInfo info)
        {
            Path = path;
            Info = info;
        }

        public bool Equals(MeshCacheKey other) =>
            string.Equals(Path, other.Path, StringComparison.Ordinal) && Info.Equals(other.Info);

        public override bool Equals(object obj) => obj is MeshCacheKey other && Equals(other);

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