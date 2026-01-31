using UnityEngine;

namespace DingoAssetsLoadSystem.Texture2DLoad
{
    public sealed class Texture2DLoadHandle : AssetLoadHandle<Texture2DCacheKey, Texture2D, Texture2DLoadInfo>
    {
        public Texture2DLoadHandle(string path, Texture2DLoadInfo info = default, GlobalAssetCache<Texture2DCacheKey, Texture2D, Texture2DLoadInfo> cache = null) 
            : base(path, info, cache ?? Texture2DGlobal.Cache) 
        {
        }
    }
}