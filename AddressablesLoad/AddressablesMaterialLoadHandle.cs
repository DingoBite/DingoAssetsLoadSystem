#if ADDRESSABLES_EXISTS
using UnityEngine;

namespace DingoAssetsLoadSystem.AddressablesLoad
{
    public sealed class AddressablesMaterialLoadHandle : AddressablesLoadHandle<Material>
    {
        public AddressablesMaterialLoadHandle(string path, AddressablesLoadInfo info = default)
            : base(path, info)
        {
        }
    }
}
#endif
