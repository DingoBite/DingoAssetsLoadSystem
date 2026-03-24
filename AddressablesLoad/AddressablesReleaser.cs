#if ADDRESSABLES_EXISTS
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace DingoAssetsLoadSystem.AddressablesLoad
{
    public sealed class AddressablesReleaser<TAsset> : IAssetReleaser<TAsset, AddressablesLoadInfo> where TAsset : Object
    {
        public void Release(TAsset asset, string path, AddressablesLoadInfo info)
        {
            if (asset != null)
                Addressables.Release(asset);
        }
    }
}
#endif