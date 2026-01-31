using System.IO;
using DingoUnityExtensions.Tweens;
using DingoUnityExtensions.UnityViewProviders.Text;
using UnityEngine;
using UnityEngine.UI;

namespace DingoAssetsLoadSystem.Texture2DLoad
{
    public sealed class Texture2DLoadWrapper : AssetLoadDataWrapper<Texture2DLoadHandle, Texture2D, Texture2DLoadInfo>
    {
        [SerializeField] private RevealBehaviour _imageParent;
        [SerializeField] private RawImage _rawImage;
        [SerializeField] private AspectRatioFitter _aspectRatioFitter;
        [SerializeField] private RevealBehaviour _preloader;
        [SerializeField] private RevealBehaviour _notFound;
        [SerializeField] private LayoutElement _layoutElement;
        [SerializeField] private Vector2 _defaultLayoutElementSizes;
        [SerializeField] private string _nameTemplate = "{0}";

        protected override void SetHandle(Texture2DLoadHandle value)
        {
            if (value?.Path == null)
                name = "not found";
            else
                name = SingleKeyText.ReplaceKeyBy(Path.GetFileNameWithoutExtension(value.Path), _nameTemplate);

            base.SetHandle(value);
        }

        protected override void ApplyView(AssetLoadState state, Texture2D tex, AssetLoadData<Texture2D, Texture2DLoadInfo> data)
        {
            if (state != AssetLoadState.Loaded)
                _rawImage.texture = null;

            switch (state)
            {
                case AssetLoadState.None:
                case AssetLoadState.Loading:
                    SetActive(_imageParent, false);
                    SetActive(_preloader, true);
                    SetActive(_notFound, false);
                    break;

                case AssetLoadState.NotFound or AssetLoadState.Failed:
                    SetActive(_imageParent, false);
                    SetActive(_preloader, false);
                    SetActive(_notFound, true);
                    break;

                case AssetLoadState.Loaded:
                    SetActive(_imageParent, true);
                    SetActive(_preloader, false);
                    SetActive(_notFound, false);

                    _rawImage.texture = tex;
                    if (tex != null)
                        ApplyAspect(tex.width, tex.height);
                    break;
            }
        }

        private void ApplyAspect(int w, int h)
        {
            if (w <= 0 || h <= 0)
                return;

            var aspect = (float)w / h;

            if (_aspectRatioFitter != null)
                _aspectRatioFitter.aspectRatio = aspect;

            if (_layoutElement != null && _defaultLayoutElementSizes.y != 0)
            {
                var defaultAspect = _defaultLayoutElementSizes.x / _defaultLayoutElementSizes.y;
                var scale = aspect / defaultAspect;

                _layoutElement.minWidth = _defaultLayoutElementSizes.x * scale;
                _layoutElement.preferredWidth = _defaultLayoutElementSizes.x * scale;

                _layoutElement.minHeight = _defaultLayoutElementSizes.y * scale;
                _layoutElement.preferredHeight = _defaultLayoutElementSizes.y * scale;
            }
        }

        private void SetActive(RevealBehaviour anim, bool value, bool immediately = false)
        {
            if (anim == null)
                return;

            if (!gameObject.activeInHierarchy)
                anim.SetActiveImmediately(value);
            else
                anim.SetActive(value, immediately);
        }
    }
}