using System;
using DingoAssetsLoadSystem.Texture2DLoad;
using UnityEngine;

namespace DingoAssetsLoadSystem.SpriteLoad
{
    public readonly struct SpriteLoadInfo : IEquatable<SpriteLoadInfo>
    {
        private readonly bool _initialized;
        private readonly Vector2 _pivot;
        private readonly float _pixelsPerUnit;
        private readonly uint _extrude;
        private readonly SpriteMeshType _meshType;
        private readonly Vector4 _border;
        private readonly bool _generateFallbackPhysicsShape;
        private readonly FilterMode _filterMode;
        private readonly TextureWrapMode _wrapMode;

        public readonly Texture2DLoadInfo TextureInfo;

        public Vector2 Pivot => _initialized ? _pivot : Vector2.one * 0.5f;
        public float PixelsPerUnit => _pixelsPerUnit > 0f ? _pixelsPerUnit : 100f;
        public uint Extrude => _extrude;
        public SpriteMeshType MeshType => _initialized ? _meshType : SpriteMeshType.FullRect;
        public Vector4 Border => _border;
        public bool GenerateFallbackPhysicsShape => _generateFallbackPhysicsShape;
        public FilterMode FilterMode => _initialized ? _filterMode : UnityEngine.FilterMode.Point;
        public TextureWrapMode WrapMode => _initialized ? _wrapMode : UnityEngine.TextureWrapMode.Clamp;

        public static SpriteLoadInfo PixelArt => default;

        public SpriteLoadInfo(
            Texture2DLoadInfo textureInfo = default,
            Vector2? pivot = null,
            float pixelsPerUnit = 100f,
            uint extrude = 0,
            SpriteMeshType meshType = SpriteMeshType.FullRect,
            Vector4 border = default,
            bool generateFallbackPhysicsShape = false,
            FilterMode filterMode = FilterMode.Point,
            TextureWrapMode wrapMode = TextureWrapMode.Clamp)
        {
            _initialized = true;
            TextureInfo = textureInfo;
            _pivot = pivot ?? Vector2.one * 0.5f;
            _pixelsPerUnit = pixelsPerUnit;
            _extrude = extrude;
            _meshType = meshType;
            _border = border;
            _generateFallbackPhysicsShape = generateFallbackPhysicsShape;
            _filterMode = filterMode;
            _wrapMode = wrapMode;
        }

        public bool Equals(SpriteLoadInfo other) =>
            TextureInfo.Equals(other.TextureInfo) &&
            Pivot.Equals(other.Pivot) &&
            PixelsPerUnit.Equals(other.PixelsPerUnit) &&
            Extrude == other.Extrude &&
            MeshType == other.MeshType &&
            Border.Equals(other.Border) &&
            GenerateFallbackPhysicsShape == other.GenerateFallbackPhysicsShape &&
            FilterMode == other.FilterMode &&
            WrapMode == other.WrapMode;

        public override bool Equals(object obj) => obj is SpriteLoadInfo other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = (hash * 31) ^ TextureInfo.GetHashCode();
                hash = (hash * 31) ^ Pivot.GetHashCode();
                hash = (hash * 31) ^ PixelsPerUnit.GetHashCode();
                hash = (hash * 31) ^ Extrude.GetHashCode();
                hash = (hash * 31) ^ MeshType.GetHashCode();
                hash = (hash * 31) ^ Border.GetHashCode();
                hash = (hash * 31) ^ GenerateFallbackPhysicsShape.GetHashCode();
                hash = (hash * 31) ^ FilterMode.GetHashCode();
                hash = (hash * 31) ^ WrapMode.GetHashCode();
                return hash;
            }
        }
    }
}
