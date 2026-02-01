#if NEWTONSOFT_EXISTS
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace DingoAssetsLoadSystem.TextLoad
{
    public static class TextAssetExtensions
    {
        public static T Deserialize<T>(this TextFileAsset asset, JsonSerializerSettings settings = null)
        {
            if (asset == null)
                return default;

            if (asset.Text != null)
                return settings == null ? JsonConvert.DeserializeObject<T>(asset.Text) : JsonConvert.DeserializeObject<T>(asset.Text, settings);

            if (asset.Bytes == null)
                return default;

            using var ms = new MemoryStream(asset.Bytes, writable: false);
            using var sr = new StreamReader(ms, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, bufferSize: 16 * 1024, leaveOpen: false);
            using var jr = new JsonTextReader(sr);

            var serializer = settings == null ? JsonSerializer.CreateDefault() : JsonSerializer.Create(settings);
            return serializer.Deserialize<T>(jr);
        }
    }
}
#endif