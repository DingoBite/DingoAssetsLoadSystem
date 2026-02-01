namespace DingoAssetsLoadSystem.ParsedTextLoad
{
    public sealed class ParsedTextReleaser<T> : IAssetReleaser<T, ParsedTextLoadInfo<T>> where T : class
    {
        public void Release(T asset, string path, ParsedTextLoadInfo<T> info)
        {
            if (asset is System.IDisposable d)
                d.Dispose();
        }
    }
}