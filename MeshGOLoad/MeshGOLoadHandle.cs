#if GLTFAST
namespace DingoAssetsLoadSystem.MeshGOLoad
{
    public sealed class MeshGOLoadHandle : AssetLoadHandle<MeshGOCacheKey, MeshGOAsset, MeshGOLoadInfo>
    {
        public MeshGOLoadHandle(string path, MeshGOLoadInfo info = default, GlobalAssetCache<MeshGOCacheKey, MeshGOAsset, MeshGOLoadInfo> cache = null) : base(path, info, cache ?? MeshGOGlobal.Cache) { }
    }
}
#endif