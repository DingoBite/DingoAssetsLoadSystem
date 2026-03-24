#if ADDRESSABLES_EXISTS
using Object = UnityEngine.Object;

namespace DingoAssetsLoadSystem.AddressablesLoad
{
    public static class AddressablesGlobal<TAsset> where TAsset : Object
    {
        public static readonly GlobalAssetCache<AddressablesCacheKey, TAsset, AddressablesLoadInfo> Cache = new(new AddressablesKeyFactory(), new AddressablesLoader<TAsset>(), new AddressablesReleaser<TAsset>(), null, UnityReceiverLiveness.IsUnityAlive);
    }
}
#endif