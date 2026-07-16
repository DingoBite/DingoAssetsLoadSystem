using System;
using UnityEngine;
using UnityEngine.UI;

namespace DingoAssetsLoadSystem.SpriteLoad
{
    public sealed class SpriteLoadWrapper : AssetLoadDataWrapper<SpriteLoadHandle, Sprite, SpriteLoadInfo>
    {
        [SerializeField] private Image _image;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private bool _clearSpriteOnUnload = true;
        [SerializeField] private bool _setNativeSizeOnLoaded;

        public event Action<AssetLoadState, Sprite> Applied;

        public void Configure(
            SpriteRenderer spriteRenderer = null,
            Image image = null,
            bool clearSpriteOnUnload = true,
            bool setNativeSizeOnLoaded = false)
        {
            _spriteRenderer = spriteRenderer;
            _image = image;
            _clearSpriteOnUnload = clearSpriteOnUnload;
            _setNativeSizeOnLoaded = setNativeSizeOnLoaded;
        }

        protected override void ApplyView(AssetLoadState state, Sprite asset, AssetLoadData<Sprite, SpriteLoadInfo> data)
        {
            if (state == AssetLoadState.Loaded)
            {
                AssignSprite(asset);
                if (_setNativeSizeOnLoaded && asset != null && _image != null)
                    _image.SetNativeSize();
            }
            else if (_clearSpriteOnUnload)
            {
                AssignSprite(null);
            }

            Applied?.Invoke(state, asset);
        }

        private void AssignSprite(Sprite sprite)
        {
            if (_image != null)
                _image.sprite = sprite;
            if (_spriteRenderer != null)
                _spriteRenderer.sprite = sprite;
        }
    }
}
