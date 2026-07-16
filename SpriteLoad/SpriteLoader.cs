using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using DingoAssetsLoadSystem.Texture2DLoad;
using UnityEngine;

namespace DingoAssetsLoadSystem.SpriteLoad
{
    public sealed class SpriteLoader : IAssetLoader<Sprite, SpriteLoadInfo>
    {
        private readonly IAssetLoader<Texture2D, Texture2DLoadInfo> _textureLoader;

        public SpriteLoader(IAssetLoader<Texture2D, Texture2DLoadInfo> textureLoader = null)
        {
            _textureLoader = textureLoader ?? new Texture2DLoader();
        }

        public async UniTask<Sprite> LoadAsync(string path, SpriteLoadInfo info, CancellationToken ct)
        {
            Texture2D texture = null;
            Sprite sprite = null;

            try
            {
                texture = await _textureLoader.LoadAsync(path, info.TextureInfo, ct);
                if (texture == null)
                    return null;

                ct.ThrowIfCancellationRequested();
                texture.filterMode = info.FilterMode;
                texture.wrapMode = info.WrapMode;

                sprite = Sprite.Create(
                    texture,
                    new Rect(0f, 0f, texture.width, texture.height),
                    info.Pivot,
                    info.PixelsPerUnit,
                    info.Extrude,
                    info.MeshType,
                    info.Border,
                    info.GenerateFallbackPhysicsShape);

                if (sprite == null)
                    return null;

                sprite.name = Path.GetFileNameWithoutExtension(path);
                texture = null;
                return sprite;
            }
            finally
            {
                if (sprite == null && texture != null)
                    Object.Destroy(texture);
            }
        }
    }
}
