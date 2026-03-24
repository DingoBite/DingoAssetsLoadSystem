#if ADDRESSABLES_EXISTS
using UnityEngine;

namespace DingoAssetsLoadSystem.AddressablesLoad
{
    public sealed class AddressablesTexture2DLoadHandle : AddressablesLoadHandle<Texture2D>
    {
        public AddressablesTexture2DLoadHandle(string path, AddressablesLoadInfo info = default)
            : base(path, info)
        {
        }
    }
}
#endif
