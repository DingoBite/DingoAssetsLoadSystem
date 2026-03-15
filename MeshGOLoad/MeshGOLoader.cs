#if GLTFAST
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using GLTFast;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DingoAssetsLoadSystem.MeshGOLoad
{
    public sealed class MeshGOLoader : IAssetLoader<MeshGOAsset, MeshGOLoadInfo>
    {
        public async UniTask<MeshGOAsset> LoadAsync(string path, MeshGOLoadInfo info, CancellationToken ct)
        {
            var uri = ResolveToUri(path);

            var ext = Path.GetExtension(uri.IsFile ? uri.LocalPath : uri.AbsolutePath).ToLowerInvariant();

            if (ext != ".gltf" && ext != ".glb")
                throw new NotSupportedException($"MeshGOLoader supports only .gltf/.glb. Got: {ext}");

#if !(GLTFAST || COM_UNITY_CLOUD_GLTFast)
            throw new InvalidOperationException("glTFast not available. Install com.unity.cloud.gltfast and define GLTFAST.");
#else
            var asset = new MeshGOAsset();

            await UniTask.SwitchToMainThread(ct);

            asset.PrototypeRoot = new GameObject($"gltf_proto::{Path.GetFileNameWithoutExtension(uri.IsFile ? uri.LocalPath : uri.AbsolutePath)}");
            asset.PrototypeRoot.SetActive(false);

            var importSettings = new ImportSettings
            {
                GenerateMipMaps = info.GenerateMipMaps
            };

            var gltf = new GltfImport();
            asset.Importer = gltf;

            var loaded = await gltf.Load(uri, importSettings, ct);
            if (!loaded)
            {
                SafeDispose(asset);
                Object.Destroy(asset.PrototypeRoot);
                return null;
            }

            var instSettings = new InstantiationSettings
            {
                Layer = info.Layer,
                LightIntensityFactor = info.LightIntensityFactor,
                SkinUpdateWhenOffscreen = info.SkinUpdateWhenOffscreen,
                Mask = info.Mask,
                SceneObjectCreation = info.SceneObjectCreation
            };

            var instantiator = new GameObjectInstantiator(gltf, asset.PrototypeRoot.transform, logger: null, settings: instSettings);

            var instantiated = await gltf.InstantiateMainSceneAsync(instantiator, ct);
            if (!instantiated)
            {
                SafeDispose(asset);
                Object.Destroy(asset.PrototypeRoot);
                return null;
            }

            CollectOwnedObjects(asset);

            return asset;
#endif
        }

        private static void CollectOwnedObjects(MeshGOAsset asset)
        {
            var root = asset.PrototypeRoot;
            var set = new HashSet<Object>();

            foreach (var mf in root.GetComponentsInChildren<MeshFilter>(true))
            {
                if (mf.sharedMesh != null)
                    set.Add(mf.sharedMesh);
            }

            foreach (var smr in root.GetComponentsInChildren<SkinnedMeshRenderer>(true))
            {
                if (smr.sharedMesh != null)
                    set.Add(smr.sharedMesh);
            }

            foreach (var mc in root.GetComponentsInChildren<MeshCollider>(true))
            {
                if (mc.sharedMesh != null)
                    set.Add(mc.sharedMesh);
            }

            foreach (var r in root.GetComponentsInChildren<Renderer>(true))
            {
                var mats = r.sharedMaterials;
                if (mats == null)
                    continue;

                foreach (var m in mats)
                {
                    if (m == null)
                        continue;

                    set.Add(m);

                    var props = m.GetTexturePropertyNames();
                    foreach (var p in props)
                    {
                        var tex = m.GetTexture(p);
                        if (tex != null)
                            set.Add(tex);
                    }
                }
            }

            asset.OwnedObjects.AddRange(set);
        }

        private static Uri ResolveToUri(string path)
        {
            if (Uri.TryCreate(path, UriKind.Absolute, out var uri))
                return uri;

            var fullPath = Path.GetFullPath(path);
            return new Uri(fullPath);
        }

        private static void SafeDispose(MeshGOAsset a)
        {
            try
            {
                a.Importer?.Dispose();
            }
            catch
            {
                /* ignore */
            }

            a.Importer = null;
        }
    }
}
#endif