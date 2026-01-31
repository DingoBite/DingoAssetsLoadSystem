using System;
using System.Collections.Generic;
using Bind;
using UnityEngine;

namespace DingoAssetsLoadSystem
{
    public interface IAssetLoadHandle<TAsset, TInfo> where TAsset : class
    {
        string Path { get; }
        IReadonlyBind<AssetLoadData<TAsset, TInfo>> Flow { get; }
        void LoadFor(object receiver);
        void UnloadFor(object receiver);
    }

    public abstract class AssetLoadHandle<TKey, TAsset, TInfo> : IAssetLoadHandle<TAsset, TInfo> where TAsset : class
    {
        public string Path { get; private set; }
        public TInfo Info { get; private set; }

        private readonly GlobalAssetCache<TKey, TAsset, TInfo> _cache;

        private readonly Bind<AssetLoadData<TAsset, TInfo>> _proxyFlow = new(AssetLoadData<TAsset, TInfo>.None);

        private IReadonlyBind<AssetLoadData<TAsset, TInfo>> _sourceFlow;
        private readonly HashSet<object> _activeReceivers = new();

        private readonly Action<AssetLoadData<TAsset, TInfo>> _forward;

        public IReadonlyBind<AssetLoadData<TAsset, TInfo>> Flow => _proxyFlow;

        protected AssetLoadHandle(string path, TInfo info, GlobalAssetCache<TKey, TAsset, TInfo> cache)
        {
            Path = path;
            Info = info;
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _forward = _proxyFlow.SetValue;
        }

        public void LoadFor(object receiver) => LoadFor(receiver, false);

        public void LoadFor(object receiver, bool forceReload)
        {
            if (receiver == null)
            {
                Debug.LogException(new NullReferenceException(nameof(receiver)));
                return;
            }

            if (string.IsNullOrWhiteSpace(Path))
            {
                Debug.LogException(new ArgumentException("Path is null/empty.", nameof(Path)));
                return;
            }

            if (_activeReceivers.Contains(receiver))
            {
                if (!forceReload)
                {
                    if (_sourceFlow != null)
                        _proxyFlow.V = _sourceFlow.V;
                    return;
                }

                UnloadFor(receiver);
            }

            _activeReceivers.Add(receiver);

            var srcReadonly = _cache.Acquire(Path, Info, receiver, forceReload);

            if (!ReferenceEquals(_sourceFlow, srcReadonly))
            {
                DetachFromSource();
                _sourceFlow = srcReadonly;
                _sourceFlow.AddListener(_forward);
            }

            _proxyFlow.V = _sourceFlow.V;
        }

        public void UnloadFor(object receiver)
        {
            if (receiver == null)
                return;

            if (!_activeReceivers.Remove(receiver))
                return;

            _cache.Release(receiver);

            if (_activeReceivers.Count == 0)
            {
                DetachFromSource();
                _proxyFlow.V = AssetLoadData<TAsset, TInfo>.None;
            }
        }

        public void Invalidate()
        {
            if (string.IsNullOrWhiteSpace(Path))
                return;

            _cache.Invalidate(Path, Info);
        }

        public void Set(string newPath, TInfo newInfo = default, bool dropToNone = true)
        {
            Path = newPath;
            Info = newInfo;

            if (dropToNone)
                _proxyFlow.V = AssetLoadData<TAsset, TInfo>.None;
        }

        private void DetachFromSource()
        {
            _sourceFlow?.RemoveListener(_forward);

            _sourceFlow = null;
        }
    }
}