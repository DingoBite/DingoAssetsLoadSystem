#if NEWTONSOFT_EXISTS
using System.IO;
using System.Text;
using DingoAssetsLoadSystem.TextLoad;
using Newtonsoft.Json;

namespace DingoAssetsLoadSystem.ParsedTextLoad
{
    public interface ITextParser<T>
    {
        string Id { get; }
        int Version { get; }
        T Parse(TextFileAsset asset);
    }

    public sealed class NewtonsoftJsonParser<T> : ITextParser<T>
    {
        public string Id { get; }
        public int Version { get; }
        private readonly JsonSerializerSettings _settings;

        public NewtonsoftJsonParser(string id, int version = 1, JsonSerializerSettings settings = null)
        {
            Id = id;
            Version = version;
            _settings = settings;
        }

        public T Parse(TextFileAsset asset)
        {
            if (asset == null)
                return default;

            if (asset.Text != null)
                return _settings == null ? JsonConvert.DeserializeObject<T>(asset.Text) : JsonConvert.DeserializeObject<T>(asset.Text, _settings);

            if (asset.Bytes == null)
                return default;

            using var ms = new MemoryStream(asset.Bytes, writable: false);
            using var sr = new StreamReader(ms, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, bufferSize: 16 * 1024);
            using var jr = new JsonTextReader(sr);

            var serializer = _settings == null ? JsonSerializer.CreateDefault() : JsonSerializer.Create(_settings);
            return serializer.Deserialize<T>(jr);
        }
    }
}
#endif