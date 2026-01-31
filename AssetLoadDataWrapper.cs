using Bind;
using DingoUnityExtensions.UnityViewProviders.Core;
using UnityEngine;
using UnityEngine.Events;

namespace DingoAssetsLoadSystem
{
    public abstract class AssetLoadDataWrapper<THandle, TAsset, TInfo> : ValueContainer<THandle>
        where THandle : class, IAssetLoadHandle<TAsset, TInfo>
        where TAsset : class
    {
        [SerializeField] private bool _autoManageLifetime = true;
        [SerializeField] private UnityEvent<bool> _loadUnloadEvent;

        private bool _isLoadedForThis;
        private bool _isSubscribed;
        private AssetLoadData<TAsset, TInfo> _last;
        private bool _destroyed;
        private THandle _value;

        public bool AutoManageLifetime
        {
            get => _autoManageLifetime;
            set => _autoManageLifetime = value;
        }

        public void LoadOnly()
        {
            if (_destroyed)
                return;

            if (_isLoadedForThis)
                return;

            _loadUnloadEvent?.Invoke(true);
            _isLoadedForThis = true;

            EnsureSubscribed();
            Value?.LoadFor(this);
        }

        public void Unload()
        {
            if (_isLoadedForThis)
            {
                _loadUnloadEvent?.Invoke(false);
                _isLoadedForThis = false;

                Value?.UnloadFor(this);
                _last = AssetLoadData<TAsset, TInfo>.None;
            }

            ApplyData(AssetLoadData<TAsset, TInfo>.None);
            DetachSubscriptionIfNeeded();
        }
        
        protected virtual void SetHandle(THandle value)
        {
            if (_isLoadedForThis && _value != null && _value.Path == value.Path)
                return;
            _value = value;
            if (_destroyed)
                return;

            var wasLoaded = _isLoadedForThis;

            if (wasLoaded)
            {
                _loadUnloadEvent?.Invoke(false);
                _isLoadedForThis = false;
                Value?.UnloadFor(this);
            }

            DetachSubscription();
            
            if (value?.Path == null)
            {
                ApplyData(AssetLoadData<TAsset, TInfo>.NotFound);
                return;
            }

            if (isActiveAndEnabled)
                EnsureSubscribed();

            if (isActiveAndEnabled && (wasLoaded || _autoManageLifetime))
                LoadOnly();
            else
                ApplyData(AssetLoadData<TAsset, TInfo>.None);
        }

        protected sealed override void SetValueWithoutNotify(THandle value) => SetHandle(value);

        protected override void OnEnable()
        {
            base.OnEnable();

            if (_destroyed)
                return;

            if (Value?.Path != null)
                EnsureSubscribed();

            if (_autoManageLifetime)
                LoadOnly();
        }

        protected override void OnDisable()
        {
            if (_autoManageLifetime)
                Unload();
            else
                DetachSubscription();

            base.OnDisable();
        }

        protected virtual void OnDestroy()
        {
            Unload();
            DetachSubscription();
            _destroyed = true;
        }

        private void EnsureSubscribed()
        {
            if (_isSubscribed)
                return;
            if (Value == null)
                return;

            Value.Flow.SafeSubscribe(OnFlowChanged);
            _isSubscribed = true;

            OnFlowChanged(Value.Flow.V);
        }

        private void DetachSubscriptionIfNeeded()
        {
            if (!_autoManageLifetime)
                return;
            DetachSubscription();
        }

        private void DetachSubscription()
        {
            if (!_isSubscribed)
                return;

            Value?.Flow.UnSubscribe(OnFlowChanged);
            _isSubscribed = false;
        }

        private void OnFlowChanged(AssetLoadData<TAsset, TInfo> data)
        {
            if (_last.State == data.State && ReferenceEquals(_last.Asset, data.Asset))
                return;

            _last = data;
            ApplyData(data);
        }

        private void ApplyData(AssetLoadData<TAsset, TInfo> data) => ApplyView(data.State, data.Asset, data);
        protected abstract void ApplyView(AssetLoadState state, TAsset asset, AssetLoadData<TAsset, TInfo> data);
    }
}
