#if ADDRESSABLES_EXISTS
using DingoAssetsLoadSystem.AddressablesLoad;
using DingoAssetsLoadSystem.AddressablesLoad.UnityWrappers;
using NaughtyAttributes;
using UnityEngine;

namespace DingoAssetsLoadSystem.Tests
{
    public class AddressablesMaterialLoadTest : MonoBehaviour
    {
        [SerializeField] private AddressablesMaterialLoadWrapper _wrapper;
        [SerializeField] private string _path;

        [Button]
        private void Load()
        {
            _wrapper.UpdateValueWithoutNotify(new AddressablesMaterialLoadHandle(_path));
        }

        [Button]
        private void Unload()
        {
            _wrapper.Unload();
        }
    }
}
#endif
