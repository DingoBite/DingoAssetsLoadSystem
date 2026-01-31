namespace DingoAssetsLoadSystem.AudioClipLoad
{
    public sealed class AudioClipLoadHandle : AssetLoadHandle<AudioClipCacheKey, UnityEngine.AudioClip, AudioClipLoadInfo>
    {
        public AudioClipLoadHandle(string path, AudioClipLoadInfo info = default, GlobalAssetCache<AudioClipCacheKey, UnityEngine.AudioClip, AudioClipLoadInfo> cache = null) : base(path, info, cache ?? AudioClipGlobal.Cache) { }
    }
}