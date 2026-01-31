using UnityEngine;

namespace DingoAssetsLoadSystem.Texture2DLoad
{
    public static class Texture2DGlobal
    {
        public static readonly GlobalAssetCache<Texture2DCacheKey, Texture2D, Texture2DLoadInfo> Cache = new(new Texture2DKeyFactory(), new Texture2DLoader(), new Texture2DReleaser(), null, UnityReceiverLiveness.IsUnityAlive);
    }
}