using UnityEngine;

namespace DingoAssetsLoadSystem.Texture2DLoad
{
    public sealed class Texture2DReleaser : IAssetReleaser<Texture2D, Texture2DLoadInfo>
    {
        public void Release(Texture2D asset, string path, Texture2DLoadInfo info)
        {
            if (asset != null)
                Object.Destroy(asset);
        }
    }
}