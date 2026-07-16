using UnityEngine;

namespace DingoAssetsLoadSystem.SpriteLoad
{
    public static class SpriteGlobal
    {
        public static readonly GlobalAssetCache<SpriteCacheKey, Sprite, SpriteLoadInfo> Cache =
            new(new SpriteKeyFactory(), new SpriteLoader(), new SpriteReleaser(), null, UnityReceiverLiveness.IsUnityAlive);
    }
}
