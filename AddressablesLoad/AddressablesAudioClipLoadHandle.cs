#if ADDRESSABLES_EXISTS
using UnityEngine;

namespace DingoAssetsLoadSystem.AddressablesLoad
{
    public sealed class AddressablesAudioClipLoadHandle : AddressablesLoadHandle<AudioClip>
    {
        public AddressablesAudioClipLoadHandle(string path, AddressablesLoadInfo info = default)
            : base(path, info)
        {
        }
    }
}
#endif
