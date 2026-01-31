using UnityEngine;

namespace DingoAssetsLoadSystem.AudioClipLoad
{
    public sealed class AudioClipLoadWrapper : AssetLoadDataWrapper<AudioClipLoadHandle, AudioClip, AudioClipLoadInfo>
    {
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private bool _autoPlayOnLoaded;
        [SerializeField] private bool _stopOnUnload = true;
        [SerializeField] private bool _clearClipOnUnload = true;

        protected override void ApplyView(AssetLoadState state, AudioClip asset, AssetLoadData<AudioClip, AudioClipLoadInfo> data)
        {
            if (_audioSource == null)
                return;

            if (state != AssetLoadState.Loaded)
            {
                if (_stopOnUnload)
                    _audioSource.Stop();

                if (_clearClipOnUnload)
                    _audioSource.clip = null;

                return;
            }

            _audioSource.clip = asset;

            if (_autoPlayOnLoaded && asset != null)
                _audioSource.Play();
        }
    }
}