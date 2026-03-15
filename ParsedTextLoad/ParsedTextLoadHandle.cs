#if NEWTONSOFT_EXISTS
namespace DingoAssetsLoadSystem.ParsedTextLoad
{
    public sealed class ParsedTextLoadHandle<T> : AssetLoadHandle<ParsedTextCacheKey, T, ParsedTextLoadInfo<T>> where T : class
    {
        public ParsedTextLoadHandle(string path, ParsedTextLoadInfo<T> info, GlobalAssetCache<ParsedTextCacheKey, T, ParsedTextLoadInfo<T>> cache = null) : base(path, info, cache ?? ParsedTextGlobal<T>.Cache) { }
    }
}
#endif