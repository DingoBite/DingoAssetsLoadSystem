using Object = UnityEngine.Object;

namespace DingoAssetsLoadSystem.MeshGOLoad
{
    public sealed class MeshGOReleaser : IAssetReleaser<MeshGOAsset, MeshGOLoadInfo>
    {
        public void Release(MeshGOAsset asset, string path, MeshGOLoadInfo info)
        {
            if (asset == null)
                return;

            if (asset.PrototypeRoot != null)
                Object.Destroy(asset.PrototypeRoot);

            asset.Importer?.Dispose();
            asset.Importer = null;

            if (asset.OwnedObjects != null)
            {
                foreach (var o in asset.OwnedObjects)
                {
                    if (o != null)
                        Object.Destroy(o);
                }
            }
        }
    }
}