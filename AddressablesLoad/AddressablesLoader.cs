#if ADDRESSABLES_EXISTS
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace DingoAssetsLoadSystem.AddressablesLoad
{
    public sealed class AddressablesLoader<TAsset> : IAssetLoader<TAsset, AddressablesLoadInfo> where TAsset : Object
    {
        public async UniTask<TAsset> LoadAsync(string path, AddressablesLoadInfo info, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(path))
                return null;

            var locationsHandle = Addressables.LoadResourceLocationsAsync(path, typeof(TAsset));
            try
            {
                await locationsHandle.ToUniTask(cancellationToken: ct);

                if (locationsHandle.Status != AsyncOperationStatus.Succeeded)
                    return null;

                if (locationsHandle.Result == null || locationsHandle.Result.Count == 0)
                    return null;
            }
            finally
            {
                if (locationsHandle.IsValid())
                    Addressables.Release(locationsHandle);
            }

            var handle = Addressables.LoadAssetAsync<TAsset>(path);
            try
            {
                await handle.ToUniTask(cancellationToken: ct);
            }
            catch (OperationCanceledException)
            {
                if (handle.IsValid())
                    Addressables.Release(handle);
                throw;
            }
            catch
            {
                if (handle.IsValid())
                    Addressables.Release(handle);
                throw;
            }

            if (handle.Status == AsyncOperationStatus.Succeeded)
                return handle.Result;

            var message = handle.OperationException?.Message ?? $"Addressables load failed: {path}";

            if (handle.IsValid())
                Addressables.Release(handle);

            throw new Exception(message);
        }
    }
}
#endif