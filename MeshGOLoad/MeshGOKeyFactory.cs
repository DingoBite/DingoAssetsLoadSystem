namespace DingoAssetsLoadSystem.MeshGOLoad
{
    public sealed class MeshGOKeyFactory : ICacheKeyFactory<MeshGOCacheKey, MeshGOLoadInfo>
    {
        public MeshGOCacheKey CreateKey(string path, MeshGOLoadInfo info) => new(path, info);
    }
}