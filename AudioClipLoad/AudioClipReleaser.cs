using UnityEngine;
using Object = UnityEngine.Object;

namespace DingoAssetsLoadSystem.AudioClipLoad
{
    public enum AudioClipReleaseMode
    {
        Destroy,
        UnloadAudioDataThenDestroy,
        UnloadAudioDataOnly
    }

    public sealed class AudioClipReleaser : IAssetReleaser<AudioClip, AudioClipLoadInfo>
    {
        private readonly AudioClipReleaseMode _mode;

        public AudioClipReleaser(AudioClipReleaseMode mode = AudioClipReleaseMode.Destroy)
        {
            _mode = mode;
        }

        public void Release(AudioClip asset, string path, AudioClipLoadInfo info)
        {
            if (asset == null)
                return;

            if (_mode is AudioClipReleaseMode.UnloadAudioDataOnly or AudioClipReleaseMode.UnloadAudioDataThenDestroy)
                asset.UnloadAudioData();

            if (_mode is AudioClipReleaseMode.Destroy or AudioClipReleaseMode.UnloadAudioDataThenDestroy)
                Object.Destroy(asset);
        }
    }
}