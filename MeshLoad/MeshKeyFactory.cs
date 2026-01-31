namespace DingoAssetsLoadSystem.MeshLoad
{
    public sealed class MeshKeyFactory : ICacheKeyFactory<MeshCacheKey, MeshLoadInfo>
    {
        public MeshCacheKey CreateKey(string path, MeshLoadInfo info) => new(path, info);
    }
}