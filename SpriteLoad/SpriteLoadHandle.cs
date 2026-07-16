using UnityEngine;

namespace DingoAssetsLoadSystem.SpriteLoad
{
    public sealed class SpriteLoadHandle : AssetLoadHandle<SpriteCacheKey, Sprite, SpriteLoadInfo>
    {
        public SpriteLoadHandle(
            string path,
            SpriteLoadInfo info = default,
            GlobalAssetCache<SpriteCacheKey, Sprite, SpriteLoadInfo> cache = null)
            : base(path, info, cache ?? SpriteGlobal.Cache)
        {
        }
    }
}
