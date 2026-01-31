using DingoAssetsLoadSystem.AudioClipLoad;
using NaughtyAttributes;
using UnityEngine;

namespace DingoAssetsLoadSystem.Tests
{
    public class AudioClipLoadTest : MonoBehaviour
    {
        [SerializeField] private AudioClipLoadWrapper _wrapper;
        [SerializeField] private string _path;

        [Button]
        private void Load()
        {
            _wrapper.UpdateValueWithoutNotify(new AudioClipLoadHandle(_path));
        }

        [Button]
        private void Unload()
        {
            _wrapper.Unload();
        }
    }
}