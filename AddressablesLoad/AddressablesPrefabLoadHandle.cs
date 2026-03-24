#if ADDRESSABLES_EXISTS
using UnityEngine;

namespace DingoAssetsLoadSystem.AddressablesLoad
{
    public sealed class AddressablesPrefabLoadHandle : AddressablesLoadHandle<GameObject>
    {
        public AddressablesPrefabLoadHandle(string path, AddressablesLoadInfo info = default)
            : base(path, info)
        {
        }
    }
}
#endif
