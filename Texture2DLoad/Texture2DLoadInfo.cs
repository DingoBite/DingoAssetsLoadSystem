using System;

namespace DingoAssetsLoadSystem.Texture2DLoad
{
    public readonly struct Texture2DLoadInfo : IEquatable<Texture2DLoadInfo>
    {
        public readonly bool Readable;
        public readonly bool MipmapChain;
        public readonly bool LinearColorSpace;

#if UNITY_6000_0_OR_NEWER
        public readonly int MipmapCount;
#endif

        public Texture2DLoadInfo(bool readable = false, bool mipmapChain = false, bool linearColorSpace = false
#if UNITY_6000_0_OR_NEWER
            , int mipmapCount = 0
#endif
        )
        {
            Readable = readable;
            MipmapChain = mipmapChain;
            LinearColorSpace = linearColorSpace;
#if UNITY_6000_0_OR_NEWER
            MipmapCount = mipmapCount;
#endif
        }

        public bool Equals(Texture2DLoadInfo other)
        {
#if UNITY_6000_0_OR_NEWER
            return Readable == other.Readable && MipmapChain == other.MipmapChain && LinearColorSpace == other.LinearColorSpace && MipmapCount == other.MipmapCount;
#else
            return Readable == other.Readable &&
                   MipmapChain == other.MipmapChain &&
                   LinearColorSpace == other.LinearColorSpace;
#endif
        }

        public override bool Equals(object obj) => obj is Texture2DLoadInfo other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                int h = 17;
                h = (h * 31) ^ Readable.GetHashCode();
                h = (h * 31) ^ MipmapChain.GetHashCode();
                h = (h * 31) ^ LinearColorSpace.GetHashCode();
#if UNITY_6000_0_OR_NEWER
                h = (h * 31) ^ MipmapCount.GetHashCode();
#endif
                return h;
            }
        }
    }
}