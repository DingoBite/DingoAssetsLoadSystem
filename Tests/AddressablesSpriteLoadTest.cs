#if ADDRESSABLES_EXISTS
using DingoAssetsLoadSystem.AddressablesLoad;
using DingoAssetsLoadSystem.AddressablesLoad.UnityWrappers;
using NaughtyAttributes;
using UnityEngine;

namespace DingoAssetsLoadSystem.Tests
{
    public class AddressablesSpriteLoadTest : MonoBehaviour
    {
        [SerializeField] private AddressablesSpriteLoadWrapper _wrapper;
        [SerializeField] private string _path;

        [Button]
        private void Load()
        {
            _wrapper.UpdateValueWithoutNotify(new AddressablesSpriteLoadHandle(_path));
        }

        [Button]
        private void Unload()
        {
            _wrapper.Unload();
        }
    }
}
#endif
