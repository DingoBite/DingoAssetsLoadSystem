#if GLTFAST
namespace DingoAssetsLoadSystem.MeshLoad
{
    public static class MeshGlobal
    {
        public static readonly GlobalAssetCache<MeshCacheKey, UnityEngine.Mesh, MeshLoadInfo> Cache = new (new MeshKeyFactory(), new MeshLoader(), new MeshReleaser(), null, UnityReceiverLiveness.IsUnityAlive);
    }
}
#endif