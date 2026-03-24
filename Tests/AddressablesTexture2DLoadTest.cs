#if ADDRESSABLES_EXISTS
using DingoAssetsLoadSystem.AddressablesLoad;
using DingoAssetsLoadSystem.AddressablesLoad.UnityWrappers;
using NaughtyAttributes;
using UnityEngine;

namespace DingoAssetsLoadSystem.Tests
{
    public class AddressablesTexture2DLoadTest : MonoBehaviour
    {
        [SerializeField] private AddressablesTexture2DLoadWrapper _wrapper;
        [SerializeField] private string _path;

        [Button]
        private void Load()
        {
            _wrapper.UpdateValueWithoutNotify(new AddressablesTexture2DLoadHandle(_path));
        }

        [Button]
        private void Unload()
        {
            _wrapper.Unload();
        }
    }
}
#endif
