using UnityEngine;
using Object = UnityEngine.Object;

namespace DingoAssetsLoadSystem.MeshLoad
{
    public sealed class MeshReleaser : IAssetReleaser<Mesh, MeshLoadInfo>
    {
        public void Release(Mesh asset, string path, MeshLoadInfo info)
        {
            if (asset != null)
            {
                if (Application.isPlaying)
                    Object.Destroy(asset);
                else
                    Object.DestroyImmediate(asset);
            }
        }
    }
}
