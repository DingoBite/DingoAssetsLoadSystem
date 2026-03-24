#if ADDRESSABLES_EXISTS
using Object = UnityEngine.Object;

namespace DingoAssetsLoadSystem.AddressablesLoad
{
    public class AddressablesLoadHandle<TAsset> : AssetLoadHandle<AddressablesCacheKey, TAsset, AddressablesLoadInfo>
        where TAsset : Object
    {
        public AddressablesLoadHandle(string path, AddressablesLoadInfo info = default, GlobalAssetCache<AddressablesCacheKey, TAsset, AddressablesLoadInfo> cache = null)
            : base(path, info, cache ?? AddressablesGlobal<TAsset>.Cache)
        {
        }
    }
}
#endif
