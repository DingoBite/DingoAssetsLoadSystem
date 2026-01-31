using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace DingoAssetsLoadSystem.Texture2DLoad
{
    public sealed class Texture2DLoader : IAssetLoader<Texture2D, Texture2DLoadInfo>
    {
        public async UniTask<Texture2D> LoadAsync(string path, Texture2DLoadInfo info, CancellationToken ct)
        {
            var uri = ResolveToUri(path);

            using var uwr = CreateRequest(uri, info);
            var op = uwr.SendWebRequest();
            await op.ToUniTask(cancellationToken: ct);

            if (uwr.result == UnityWebRequest.Result.Success)
                return DownloadHandlerTexture.GetContent(uwr);

            if (uwr.result == UnityWebRequest.Result.ProtocolError && uwr.responseCode == 404)
                return null;

            if (uwr.result == UnityWebRequest.Result.ConnectionError)
                throw new Exception($"Texture load failed (connection error): {uwr.error}");

            if (uwr.result == UnityWebRequest.Result.ProtocolError)
                throw new Exception($"Texture load failed (HTTP {(int)uwr.responseCode}): {uwr.error}");

            throw new Exception($"Texture load failed ({uwr.result}): {uwr.error}");
        }

        private static UnityWebRequest CreateRequest(Uri uri, Texture2DLoadInfo info)
        {
#if UNITY_6000_0_OR_NEWER
            var p = DownloadedTextureParams.Default;
            p.readable = info.Readable;
            p.mipmapChain = info.MipmapChain;
            p.linearColorSpace = info.LinearColorSpace;
            if (info.MipmapCount > 0)
                p.mipmapCount = info.MipmapCount;

            return UnityWebRequestTexture.GetTexture(uri, p);
#else
            return UnityWebRequestTexture.GetTexture(uri, nonReadable: !info.Readable);
#endif
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