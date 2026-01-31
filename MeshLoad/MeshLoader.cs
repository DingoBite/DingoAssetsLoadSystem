using System;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using GLTFast;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace DingoAssetsLoadSystem.MeshLoad
{
    public sealed class MeshLoader : IAssetLoader<Mesh, MeshLoadInfo>
    {
        public async UniTask<Mesh> LoadAsync(string path, MeshLoadInfo info, CancellationToken ct)
        {
            var uri = ResolveToUri(path);

            var ext = Path.GetExtension(uri.IsFile ? uri.LocalPath : uri.AbsolutePath).ToLowerInvariant();
            if (ext != ".gltf" && ext != ".glb")
                throw new NotSupportedException($"MeshLoader supports only .gltf/.glb. Got: {ext}");

            var gltf = new GltfImport();
            try
            {
                var loaded = await gltf.Load(uri, importSettings: null, cancellationToken: ct);
                if (!loaded)
                    return null;

#pragma warning disable CS0618
                var importedMeshes = gltf.GetMeshes();
#pragma warning restore CS0618
                if (importedMeshes == null || importedMeshes.Length == 0)
                    return null;

                await UniTask.SwitchToMainThread(ct);

                Mesh ownedMesh = null;

                if (info.CombineAllMeshes)
                {
                    var combined = CombineMeshes(importedMeshes);
                    if (combined != null)
                        ownedMesh = combined;
                }
                else
                {
                    var selected = SelectMesh(importedMeshes, info);
                    if (selected != null)
                    {
                        ownedMesh = Object.Instantiate(selected);
                        ownedMesh.name = selected.name;
                    }
                }

                foreach (var m in importedMeshes)
                {
                    if (m != null)
                        Object.Destroy(m);
                }

                gltf.Dispose();

                if (ownedMesh == null)
                    return null;

                PostProcess(ownedMesh, info);
                return ownedMesh;
            }
            finally
            {
                try
                {
                    gltf.Dispose();
                }
                catch { }
            }
        }

        private static Mesh SelectMesh(Mesh[] meshes, MeshLoadInfo info)
        {
            if (!string.IsNullOrEmpty(info.MeshName))
            {
                foreach (var m in meshes)
                {
                    if (m != null && m.name == info.MeshName)
                        return m;
                }
            }

            if (info.MeshIndex >= 0 && info.MeshIndex < meshes.Length)
                return meshes[info.MeshIndex];

            return meshes[0];
        }

        private static Mesh CombineMeshes(Mesh[] meshes)
        {
            long totalVertices = 0;
            var count = 0;
            foreach (var m in meshes)
            {
                if (m == null)
                    continue;
                count++;
                totalVertices += m.vertexCount;
            }

            if (count == 0)
                return null;

            var combine = new CombineInstance[count];
            var ci = 0;
            foreach (var m in meshes)
            {
                if (m == null)
                    continue;
                combine[ci++] = new CombineInstance { mesh = m, subMeshIndex = 0, transform = Matrix4x4.identity };
            }

            var outMesh = new Mesh();
            if (totalVertices > 65535)
                outMesh.indexFormat = IndexFormat.UInt32;
            outMesh.CombineMeshes(combine, mergeSubMeshes: true, useMatrices: true, hasLightmapData: false);
            return outMesh;
        }

        private static void PostProcess(Mesh mesh, MeshLoadInfo info)
        {
            if (info.RecalculateNormals)
                mesh.RecalculateNormals();
            if (info.RecalculateBounds)
                mesh.RecalculateBounds();

            if (info.MarkNoLongerReadable)
                mesh.UploadMeshData(true);
        }

        private static Uri ResolveToUri(string path)
        {
            if (Uri.TryCreate(path, UriKind.Absolute, out var uri))
                return uri;

            var fullPath = Path.GetFullPath(path);
            return new Uri(fullPath);
        }
    }
}