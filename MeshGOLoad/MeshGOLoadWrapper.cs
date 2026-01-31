using UnityEngine;

namespace DingoAssetsLoadSystem.MeshGOLoad
{
    public sealed class MeshGOLoadWrapper : AssetLoadDataWrapper<MeshGOLoadHandle, MeshGOAsset, MeshGOLoadInfo>
    {
        [SerializeField] private Transform _spawnParent;
        private GameObject _instance;

        protected override void ApplyView(AssetLoadState state, MeshGOAsset asset, AssetLoadData<MeshGOAsset, MeshGOLoadInfo> data)
        {
            if (state != AssetLoadState.Loaded)
            {
                DestroyInstance();
                return;
            }

            if (asset?.PrototypeRoot == null)
            {
                DestroyInstance();
                return;
            }

            if (_spawnParent == null)
                _spawnParent = transform;

            if (_instance == null)
            {
                _instance = Instantiate(asset.PrototypeRoot, _spawnParent);
                _instance.SetActive(true);
            }
        }

        protected override void SetHandle(MeshGOLoadHandle value)
        {
            DestroyInstance();
            base.SetHandle(value);
        }

        private void DestroyInstance()
        {
            if (_instance == null)
                return;
            Destroy(_instance);
            _instance = null;
        }
    }
}