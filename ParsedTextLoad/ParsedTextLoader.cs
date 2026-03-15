#if NEWTONSOFT_EXISTS
using System.Threading;
using Cysharp.Threading.Tasks;
using DingoAssetsLoadSystem.TextLoad;

namespace DingoAssetsLoadSystem.ParsedTextLoad
{
    public sealed class ParsedTextLoader<T> : IAssetLoader<T, ParsedTextLoadInfo<T>> where T : class
    {
        private readonly TextLoader _textLoader = new();

        public async UniTask<T> LoadAsync(string path, ParsedTextLoadInfo<T> info, CancellationToken ct)
        {
            var raw = await _textLoader.LoadAsync(path, info.TextInfo, ct);
            if (raw == null)
                return null;

            if (info.Parser == null)
                return null;

            if (!info.ParseOnThreadPool)
                return info.Parser.Parse(raw);

            return await UniTask.RunOnThreadPool(() => info.Parser.Parse(raw), cancellationToken: ct);
        }
    }
}
#endif