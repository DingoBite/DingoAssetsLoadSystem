namespace DingoAssetsLoadSystem.MeshLoad
{
    public sealed class MeshLoadHandle : AssetLoadHandle<MeshCacheKey, UnityEngine.Mesh, MeshLoadInfo>
    {
        public MeshLoadHandle(string path, MeshLoadInfo info = default, GlobalAssetCache<MeshCacheKey, UnityEngine.Mesh, MeshLoadInfo> cache = null) : base(path, info, cache ?? MeshGlobal.Cache) { }
    }
}