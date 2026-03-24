#if ADDRESSABLES_EXISTS
using UnityEngine;

namespace DingoAssetsLoadSystem.AddressablesLoad.UnityWrappers
{
    public sealed class AddressablesPrefabLoadWrapper : AddressablesLoadWrapper<AddressablesPrefabLoadHandle, GameObject>
    {
        [SerializeField] private Transform _spawnParent;
        [SerializeField] private bool _worldPositionStays;

        private GameObject _instance;
        private GameObject _prototype;

        protected override void SetHandle(AddressablesPrefabLoadHandle value)
        {
            var currentPath = Value?.Path;
            var nextPath = value?.Path;

            if (currentPath != nextPath)
                DestroyInstance();

            base.SetHandle(value);
        }

        protected override void ApplyView(AssetLoadState state, GameObject asset, AssetLoadData<GameObject, AddressablesLoadInfo> data)
        {
            if (state != AssetLoadState.Loaded || asset == null)
            {
                DestroyInstance();
                return;
            }

            if (_spawnParent == null)
                _spawnParent = transform;

            if (_instance != null && _prototype == asset)
                return;

            DestroyInstance();

            _instance = Instantiate(asset, _spawnParent, _worldPositionStays);
            _prototype = asset;
        }

        private void DestroyInstance()
        {
            if (_instance == null)
                return;

            Destroy(_instance);
            _instance = null;
            _prototype = null;
        }
    }
}
#endif
