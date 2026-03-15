#if GLTFAST
using UnityEngine;

namespace DingoAssetsLoadSystem.MeshLoad
{
    public sealed class MeshLoadWrapper : AssetLoadDataWrapper<MeshLoadHandle, Mesh, MeshLoadInfo>
    {
        [SerializeField] private MeshFilter _meshFilter;
        [SerializeField] private SkinnedMeshRenderer _skinnedMeshRenderer;
        [SerializeField] private MeshCollider _meshCollider;
        [SerializeField] private bool _clearMeshOnUnload = true;

        protected override void ApplyView(AssetLoadState state, Mesh asset, AssetLoadData<Mesh, MeshLoadInfo> data)
        {
            if (state != AssetLoadState.Loaded)
            {
                if (_clearMeshOnUnload)
                    AssignMesh(null);
                return;
            }

            AssignMesh(asset);
        }

        private void AssignMesh(Mesh mesh)
        {
            if (_meshFilter != null)
                _meshFilter.sharedMesh = mesh;
            if (_skinnedMeshRenderer != null)
                _skinnedMeshRenderer.sharedMesh = mesh;
            if (_meshCollider != null)
                _meshCollider.sharedMesh = mesh;
        }
    }
}
#endif