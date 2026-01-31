using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace DingoAssetsLoadSystem.AudioClipLoad
{
    public sealed class AudioClipLoader : IAssetLoader<AudioClip, AudioClipLoadInfo>
    {
        public async UniTask<AudioClip> LoadAsync(string path, AudioClipLoadInfo info, CancellationToken ct)
        {
            var uri = ResolveToUri(path);

            using var uwr = UnityWebRequestMultimedia.GetAudioClip(uri, info.AudioType);

            if (uwr.downloadHandler is DownloadHandlerAudioClip dh)
            {
                dh.streamAudio = info.StreamAudio;
                dh.compressed = info.CompressedInMemory;
            }

            await uwr.SendWebRequest().ToUniTask(cancellationToken: ct);

            if (uwr.result == UnityWebRequest.Result.Success)
                return DownloadHandlerAudioClip.GetContent(uwr);

            if (uwr.result == UnityWebRequest.Result.ProtocolError && uwr.responseCode == 404)
                return null;

            throw new Exception($"AudioClip load failed ({uwr.result}, HTTP {(int)uwr.responseCode}): {uwr.error}");
        }

        private static Uri ResolveToUri(string path)
        {
            if (Uri.TryCreate(path, UriKind.Absolute, out var uri))
                return uri;

            var fullPath = System.IO.Path.GetFullPath(path);
            return new Uri(fullPath);
        }
    }
}