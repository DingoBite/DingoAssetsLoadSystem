#if GLTFAST
using System;
using GLTFast;

namespace DingoAssetsLoadSystem.MeshGOLoad
{
    [Serializable]
    public readonly struct MeshGOLoadInfo : IEquatable<MeshGOLoadInfo>
    {
        public readonly bool GenerateMipMaps;

        public readonly int Layer;
#if GLTFAST || COM_UNITY_CLOUD_GLTFast
        public readonly ComponentType Mask;
        public readonly SceneObjectCreation SceneObjectCreation;
#endif
        public readonly float LightIntensityFactor;
        public readonly bool SkinUpdateWhenOffscreen;

        public MeshGOLoadInfo(bool generateMipMaps = false, int layer = 0,
#if GLTFAST || COM_UNITY_CLOUD_GLTFast
            ComponentType mask = ComponentType.All, SceneObjectCreation sceneObjectCreation = SceneObjectCreation.Always,
#endif
            float lightIntensityFactor = 1f, bool skinUpdateWhenOffscreen = false)
        {
            GenerateMipMaps = generateMipMaps;
            Layer = layer;
#if GLTFAST || COM_UNITY_CLOUD_GLTFast
            Mask = mask;
            SceneObjectCreation = sceneObjectCreation;
#endif
            LightIntensityFactor = lightIntensityFactor;
            SkinUpdateWhenOffscreen = skinUpdateWhenOffscreen;
        }

        public bool Equals(MeshGOLoadInfo other)
        {
#if GLTFAST || COM_UNITY_CLOUD_GLTFast
            return GenerateMipMaps == other.GenerateMipMaps && Layer == other.Layer && Mask == other.Mask && SceneObjectCreation == other.SceneObjectCreation && LightIntensityFactor.Equals(other.LightIntensityFactor) && SkinUpdateWhenOffscreen == other.SkinUpdateWhenOffscreen;
#else
            return GenerateMipMaps == other.GenerateMipMaps
                   && Layer == other.Layer
                   && LightIntensityFactor.Equals(other.LightIntensityFactor)
                   && SkinUpdateWhenOffscreen == other.SkinUpdateWhenOffscreen;
#endif
        }

        public override bool Equals(object obj) => obj is MeshGOLoadInfo other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                var h = 17;
                h = h * 31 + GenerateMipMaps.GetHashCode();
                h = h * 31 + Layer;
#if GLTFAST || COM_UNITY_CLOUD_GLTFast
                h = h * 31 + (int)Mask;
                h = h * 31 + (int)SceneObjectCreation;
#endif
                h = h * 31 + LightIntensityFactor.GetHashCode();
                h = h * 31 + SkinUpdateWhenOffscreen.GetHashCode();
                return h;
            }
        }
    }
}
#endif