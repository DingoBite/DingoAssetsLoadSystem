#if GLTFAST
using System;

namespace DingoAssetsLoadSystem.MeshGOLoad
{
    public readonly struct MeshGOCacheKey : IEquatable<MeshGOCacheKey>
    {
        public readonly string Path;
        public readonly MeshGOLoadInfo Info;

        public MeshGOCacheKey(string path, MeshGOLoadInfo info)
        {
            Path = path;
            Info = info;
        }

        public bool Equals(MeshGOCacheKey other) =>
            string.Equals(Path, other.Path, StringComparison.Ordinal) && Info.Equals(other.Info);

        public override bool Equals(object obj) => obj is MeshGOCacheKey other && Equals(other);

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
#endif