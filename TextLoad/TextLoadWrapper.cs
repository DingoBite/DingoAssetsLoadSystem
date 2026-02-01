using System.IO;
using DingoUnityExtensions.Tweens;
using DingoUnityExtensions.UnityViewProviders.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace DingoAssetsLoadSystem.TextLoad
{
    public sealed class TextLoadWrapper : AssetLoadDataWrapper<TextLoadHandle, TextFileAsset, TextLoadInfo>
    {
        [SerializeField] private RevealBehaviour _content;
        [SerializeField] private RevealBehaviour _preloader;
        [SerializeField] private RevealBehaviour _notFound;

        [SerializeField] private TMP_Text _text;
        [SerializeField] private string _nameTemplate = "{0}";
        [SerializeField] private UnityEvent<string> _onTextLoaded;

        protected override void SetHandle(TextLoadHandle value)
        {
            if (value?.Path == null)
                name = "not found";
            else
                name = SingleKeyText.ReplaceKeyBy(Path.GetFileNameWithoutExtension(value.Path), _nameTemplate);

            base.SetHandle(value);
        }

        protected override void ApplyView(AssetLoadState state, TextFileAsset asset, AssetLoadData<TextFileAsset, TextLoadInfo> data)
        {
            switch (state)
            {
                case AssetLoadState.None:
                case AssetLoadState.Loading:
                    SetActive(_content, false);
                    SetActive(_preloader, true);
                    SetActive(_notFound, false);
                    if (_text != null)
                        _text.text = string.Empty;
                    break;

                case AssetLoadState.NotFound:
                case AssetLoadState.Failed:
                    SetActive(_content, false);
                    SetActive(_preloader, false);
                    SetActive(_notFound, true);
                    if (_text != null)
                        _text.text = string.Empty;
                    break;

                case AssetLoadState.Loaded:
                    SetActive(_content, true);
                    SetActive(_preloader, false);
                    SetActive(_notFound, false);

                    var text = asset?.Text ?? string.Empty;
                    if (_text != null)
                        _text.text = text;
                    _onTextLoaded?.Invoke(text);
                    break;
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