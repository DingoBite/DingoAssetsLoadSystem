#if ADDRESSABLES_EXISTS
using TMPro;

namespace DingoAssetsLoadSystem.AddressablesLoad
{
    public sealed class AddressablesTMPFontAssetLoadHandle : AddressablesLoadHandle<TMP_FontAsset>
    {
        public AddressablesTMPFontAssetLoadHandle(string path, AddressablesLoadInfo info = default)
            : base(path, info)
        {
        }
    }
}
#endif
