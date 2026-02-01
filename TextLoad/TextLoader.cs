using System;
using System.IO;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;

namespace DingoAssetsLoadSystem.TextLoad
{
    public sealed class TextLoader : IAssetLoader<TextFileAsset, TextLoadInfo>
    {
        public async UniTask<TextFileAsset> LoadAsync(string path, TextLoadInfo info, CancellationToken ct)
        {
            var uri = ResolveToUri(path);
            var encoding = info.GetEncoding();

            if (uri.IsFile)
            {
                var fullPath = uri.LocalPath;

                if (!File.Exists(fullPath))
                    return null;

                byte[] bytes;
                try
                {
                    bytes = await File.ReadAllBytesAsync(fullPath, ct);
                }
                catch (FileNotFoundException)
                {
                    return null;
                }

                var text = info.DecodeText ? Decode(bytes, encoding, info.StripBom, info.NormalizeNewLines) : null;
                var keptBytes = info.KeepBytes ? bytes : null;

                var (hasStamp, length, ticks) = TryGetStamp(fullPath);

                return new TextFileAsset(fullPath, text, keptBytes, string.IsNullOrWhiteSpace(info.EncodingName) ? "utf-8" : info.EncodingName, hasStamp, length, ticks);
            }

            await UniTask.SwitchToMainThread(ct);

            using var uwr = UnityWebRequest.Get(uri);
            var op = uwr.SendWebRequest();
            await op.ToUniTask(cancellationToken: ct);

            if (uwr.result == UnityWebRequest.Result.Success)
            {
                var bytes = uwr.downloadHandler.data;
                var text = info.DecodeText ? Decode(bytes, encoding, info.StripBom, info.NormalizeNewLines) : null;
                var keptBytes = info.KeepBytes ? bytes : null;

                return new TextFileAsset(uri.ToString(), text, keptBytes, string.IsNullOrWhiteSpace(info.EncodingName) ? "utf-8" : info.EncodingName, false, 0, 0);
            }

            if (uwr.result == UnityWebRequest.Result.ProtocolError && uwr.responseCode == 404)
                return null;

            if (uwr.result == UnityWebRequest.Result.ConnectionError)
                throw new Exception($"Text load failed (connection error): {uwr.error}");

            if (uwr.result == UnityWebRequest.Result.ProtocolError)
                throw new Exception($"Text load failed (HTTP {(int)uwr.responseCode}): {uwr.error}");

            throw new Exception($"Text load failed ({uwr.result}): {uwr.error}");
        }

        private static (bool hasStamp, long length, long ticksUtc) TryGetStamp(string fullPath)
        {
            try
            {
                var fi = new FileInfo(fullPath);
                if (!fi.Exists)
                    return (false, 0, 0);
                return (true, fi.Length, fi.LastWriteTimeUtc.Ticks);
            }
            catch
            {
                return (false, 0, 0);
            }
        }

        private static string Decode(byte[] bytes, Encoding encoding, bool stripBom, bool normalizeNewLines)
        {
            if (bytes == null || bytes.Length == 0)
                return string.Empty;

            int offset = 0;

            if (stripBom)
            {
                if (bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
                    offset = 3;
                else if (bytes.Length >= 2 && bytes[0] == 0xFF && bytes[1] == 0xFE)
                    offset = 2;
                else if (bytes.Length >= 2 && bytes[0] == 0xFE && bytes[1] == 0xFF)
                    offset = 2;
            }

            var text = offset == 0 ? encoding.GetString(bytes) : encoding.GetString(bytes, offset, bytes.Length - offset);

            if (normalizeNewLines)
                text = text.Replace("\r\n", "\n");

            return text;
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