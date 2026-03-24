#if ADDRESSABLES_EXISTS
using UnityEngine;
using UnityEngine.UI;

namespace DingoAssetsLoadSystem.AddressablesLoad.UnityWrappers
{
    public sealed class AddressablesTexture2DLoadWrapper : AddressablesLoadWrapper<AddressablesTexture2DLoadHandle, Texture2D>
    {
        [SerializeField] private RawImage _rawImage;
        [SerializeField] private AspectRatioFitter _aspectRatioFitter;
        [SerializeField] private bool _clearTextureOnUnload = true;

        protected override void ApplyView(AssetLoadState state, Texture2D asset, AssetLoadData<Texture2D, AddressablesLoadInfo> data)
        {
            if (state != AssetLoadState.Loaded)
            {
                if (_clearTextureOnUnload && _rawImage != null)
                    _rawImage.texture = null;

                return;
            }

            if (_rawImage != null)
                _rawImage.texture = asset;

            if (_aspectRatioFitter != null && asset != null && asset.height > 0)
                _aspectRatioFitter.aspectRatio = (float)asset.width / asset.height;
        }
    }
}
#endif
