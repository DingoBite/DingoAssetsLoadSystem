#if ADDRESSABLES_EXISTS
using TMPro;
using UnityEngine;

namespace DingoAssetsLoadSystem.AddressablesLoad.UnityWrappers
{
    public sealed class AddressablesTMPFontAssetLoadWrapper : AddressablesLoadWrapper<AddressablesTMPFontAssetLoadHandle, TMP_FontAsset>
    {
        [SerializeField] private TMP_Text _tmpText;
        [SerializeField] private bool _clearFontOnUnload = true;
        [SerializeField] private bool _clearSharedMaterialOnUnload = true;
        [SerializeField] private bool _applyFontMaterial = true;

        protected override void ApplyView(AssetLoadState state, TMP_FontAsset asset, AssetLoadData<TMP_FontAsset, AddressablesLoadInfo> data)
        {
            if (_tmpText == null)
                return;

            if (state != AssetLoadState.Loaded)
            {
                if (_clearFontOnUnload)
                    _tmpText.font = null;

                if (_clearSharedMaterialOnUnload)
                    _tmpText.fontSharedMaterial = null;

                return;
            }

            _tmpText.font = asset;

            if (_applyFontMaterial)
                _tmpText.fontSharedMaterial = asset != null ? asset.material : null;
        }
    }
}
#endif
