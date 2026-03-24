#if ADDRESSABLES_EXISTS
using Object = UnityEngine.Object;

namespace DingoAssetsLoadSystem.AddressablesLoad
{
    public abstract class AddressablesLoadWrapper<THandle, TAsset> : AssetLoadDataWrapper<THandle, TAsset, AddressablesLoadInfo>
        where THandle : AddressablesLoadHandle<TAsset>
        where TAsset : Object
    {
    }
}
#endif
