namespace DingoAssetsLoadSystem.AudioClipLoad
{
    public static class AudioClipGlobal
    {
        public static readonly GlobalAssetCache<AudioClipCacheKey, UnityEngine.AudioClip, AudioClipLoadInfo> Cache = new(new AudioClipKeyFactory(), new AudioClipLoader(), new AudioClipReleaser(), null, UnityReceiverLiveness.IsUnityAlive);
    }
}