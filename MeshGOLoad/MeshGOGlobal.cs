namespace DingoAssetsLoadSystem.MeshGOLoad
{
    public static class MeshGOGlobal
    {
        public static readonly GlobalAssetCache<MeshGOCacheKey, MeshGOAsset, MeshGOLoadInfo> Cache = new(new MeshGOKeyFactory(), new MeshGOLoader(), new MeshGOReleaser(), null, UnityReceiverLiveness.IsUnityAlive);
    }
}