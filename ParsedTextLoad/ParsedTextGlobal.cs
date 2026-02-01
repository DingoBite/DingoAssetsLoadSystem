namespace DingoAssetsLoadSystem.ParsedTextLoad
{
    public static class ParsedTextGlobal<T> where T : class
    {
        public static readonly GlobalAssetCache<ParsedTextCacheKey, T, ParsedTextLoadInfo<T>> Cache = new(new ParsedTextKeyFactory<T>(), new ParsedTextLoader<T>(), new ParsedTextReleaser<T>(), null, UnityReceiverLiveness.IsUnityAlive);
    }
}