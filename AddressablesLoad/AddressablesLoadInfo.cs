#if ADDRESSABLES_EXISTS
using System;

namespace DingoAssetsLoadSystem.AddressablesLoad
{
    [Serializable]
    public readonly struct AddressablesLoadInfo : IEquatable<AddressablesLoadInfo>
    {
        public bool Equals(AddressablesLoadInfo other) => true;

        public override bool Equals(object obj) => obj is AddressablesLoadInfo;

        public override int GetHashCode() => 0;
    }
}
#endif
