using UnityEngine;

namespace DingoAssetsLoadSystem.SpriteLoad
{
    public sealed class SpriteReleaser : IAssetReleaser<Sprite, SpriteLoadInfo>
    {
        public void Release(Sprite asset, string path, SpriteLoadInfo info)
        {
            if (asset == null)
                return;

            var texture = asset.texture;
            Object.Destroy(asset);
            if (texture != null)
                Object.Destroy(texture);
        }
    }
}
