using DingoAssetsLoadSystem.MeshLoad;
using NaughtyAttributes;
using UnityEngine;

namespace DingoAssetsLoadSystem.Tests
{
    public class MeshLoadTest : MonoBehaviour
    {
        [SerializeField] private MeshLoadWrapper _wrapper;
        [SerializeField] private string _path;

        [Button]
        private void Load()
        {
            _wrapper.UpdateValueWithoutNotify(new MeshLoadHandle(_path, new MeshLoadInfo(markNoLongerReadable:false)));
        }

        [Button]
        private void Unload()
        {
            _wrapper.Unload();
        }
    }
}