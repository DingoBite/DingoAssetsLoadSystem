#if ADDRESSABLES_EXISTS
using System;

namespace DingoAssetsLoadSystem.AddressablesLoad
{
    public readonly struct AddressablesCacheKey : IEquatable<AddressablesCacheKey>
    {
        public readonly string Path;
        public readonly AddressablesLoadInfo Info;

        public AddressablesCacheKey(string path, AddressablesLoadInfo info)
        {
            Path = path;
            Info = info;
        }

        public bool Equals(AddressablesCacheKey other) => string.Equals(Path, other.Path, StringComparison.Ordinal) && Info.Equals(other.Info);

        public override bool Equals(object obj) => obj is AddressablesCacheKey other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                var h = 17;
                h = (h * 31) ^ (Path != null ? StringComparer.Ordinal.GetHashCode(Path) : 0);
                h = (h * 31) ^ Info.GetHashCode();
                return h;
            }
        }
    }
}
#endif