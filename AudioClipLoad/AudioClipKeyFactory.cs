namespace DingoAssetsLoadSystem.AudioClipLoad
{
    public sealed class AudioClipKeyFactory : ICacheKeyFactory<AudioClipCacheKey, AudioClipLoadInfo>
    {
        public AudioClipCacheKey CreateKey(string path, AudioClipLoadInfo info) => new(path, info);
    }
}