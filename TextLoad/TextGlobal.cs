namespace DingoAssetsLoadSystem.TextLoad
{
    public static class TextGlobal
    {
        public static readonly GlobalAssetCache<TextCacheKey, TextFileAsset, TextLoadInfo> Cache = new(new TextKeyFactory(), new TextLoader(), new TextReleaser(), null, UnityReceiverLiveness.IsUnityAlive);
    }
}