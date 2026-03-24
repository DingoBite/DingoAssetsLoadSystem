#if ADDRESSABLES_EXISTS
namespace DingoAssetsLoadSystem.AddressablesLoad
{
    public sealed class AddressablesKeyFactory : ICacheKeyFactory<AddressablesCacheKey, AddressablesLoadInfo>
    {
        public AddressablesCacheKey CreateKey(string path, AddressablesLoadInfo info) => new(path, info);
    }
}
#endif
