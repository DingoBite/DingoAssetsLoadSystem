namespace DingoAssetsLoadSystem.TextLoad
{
    public sealed class TextReleaser : IAssetReleaser<TextFileAsset, TextLoadInfo>
    {
        public void Release(TextFileAsset asset, string path, TextLoadInfo info) { }
    }
}