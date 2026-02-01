namespace DingoAssetsLoadSystem.TextLoad
{
    public sealed class TextLoadHandle : AssetLoadHandle<TextCacheKey, TextFileAsset, TextLoadInfo>
    {
        public TextLoadHandle(string path, TextLoadInfo info = default, GlobalAssetCache<TextCacheKey, TextFileAsset, TextLoadInfo> cache = null) : base(path, info, cache ?? TextGlobal.Cache) { }
    }
}