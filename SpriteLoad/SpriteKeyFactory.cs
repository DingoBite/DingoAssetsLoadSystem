namespace DingoAssetsLoadSystem.SpriteLoad
{
    public sealed class SpriteKeyFactory : ICacheKeyFactory<SpriteCacheKey, SpriteLoadInfo>
    {
        public SpriteCacheKey CreateKey(string path, SpriteLoadInfo info) => new(path, info);
    }
}
