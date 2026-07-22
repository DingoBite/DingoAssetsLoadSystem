using UnityEngine;

namespace DingoAssetsLoadSystem.SpriteLoad
{
    public static class SpriteLoadUnityUtils
    {
        public static void DestroyObject(Object value)
        {
            if (value == null)
            {
                return;
            }

            if (Application.IsPlaying(value))
            {
                Object.Destroy(value);
            }
            else
            {
                Object.DestroyImmediate(value);
            }
        }
    }
}
