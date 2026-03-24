#if ADDRESSABLES_EXISTS
using UnityEngine;
using UnityEngine.UI;

namespace DingoAssetsLoadSystem.AddressablesLoad.UnityWrappers
{
    public sealed class AddressablesSpriteLoadWrapper : AddressablesLoadWrapper<AddressablesSpriteLoadHandle, Sprite>
    {
        [SerializeField] private Image _image;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private bool _clearSpriteOnUnload = true;
        [SerializeField] private bool _setNativeSizeOnLoaded;

        protected override void ApplyView(AssetLoadState state, Sprite asset, AssetLoadData<Sprite, AddressablesLoadInfo> data)
        {
            if (state != AssetLoadState.Loaded)
            {
                if (_clearSpriteOnUnload)
                    AssignSprite(null);

                return;
            }

            AssignSprite(asset);

            if (_setNativeSizeOnLoaded && asset != null && _image != null)
                _image.SetNativeSize();
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
#endif
