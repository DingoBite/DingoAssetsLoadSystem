#if GLTFAST || COM_UNITY_CLOUD_GLTFast
using DingoAssetsLoadSystem.MeshGOLoad;
using GLTFast;
using NaughtyAttributes;
using UnityEngine;

namespace DingoAssetsLoadSystem.Tests
{
    public class MeshGOLoadTest : MonoBehaviour
    {
        [SerializeField] private MeshGOLoadWrapper _wrapper;
        [SerializeField] private string _path;

        [Button]
        private void Load()
        {
            _wrapper.UpdateValueWithoutNotify(new MeshGOLoadHandle(_path, new MeshGOLoadInfo(mask: ComponentType.All)));
        }

        [Button]
        private void Unload()
        {
            _wrapper.Unload();
        }
    }
}
#endif