#if ADDRESSABLES_EXISTS
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DingoAssetsLoadSystem.AddressablesLoad.UnityWrappers
{
    public sealed class AddressablesMaterialLoadWrapper : AddressablesLoadWrapper<AddressablesMaterialLoadHandle, Material>
    {
        [SerializeField] private Renderer _renderer;
        [SerializeField] private Graphic _graphic;
        [SerializeField] private TMP_Text _tmpText;
        [SerializeField] private bool _clearMaterialOnUnload = true;

        protected override void ApplyView(AssetLoadState state, Material asset, AssetLoadData<Material, AddressablesLoadInfo> data)
        {
            if (state != AssetLoadState.Loaded)
            {
                if (_clearMaterialOnUnload)
                    AssignMaterial(null);

                return;
            }

            AssignMaterial(asset);
        }

        private void AssignMaterial(Material material)
        {
            if (_renderer != null)
                _renderer.sharedMaterial = material;

            if (_graphic != null)
                _graphic.material = material;

            if (_tmpText != null)
                _tmpText.fontSharedMaterial = material;
        }
    }
}
#endif
