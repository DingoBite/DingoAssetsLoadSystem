namespace DingoAssetsLoadSystem.Texture2DLoad
{
    public sealed class Texture2DKeyFactory : ICacheKeyFactory<Texture2DCacheKey, Texture2DLoadInfo>
    {
        public Texture2DCacheKey CreateKey(string path, Texture2DLoadInfo info) => new(path, info);
    }
}