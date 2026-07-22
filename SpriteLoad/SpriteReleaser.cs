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
            SpriteLoadUnityUtils.DestroyObject(asset);
            if (texture != null)
                SpriteLoadUnityUtils.DestroyObject(texture);
        }
    }
}
