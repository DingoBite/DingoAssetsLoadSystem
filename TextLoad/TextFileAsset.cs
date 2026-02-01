namespace DingoAssetsLoadSystem.TextLoad
{
    public sealed class TextFileAsset
    {
        public readonly string Path;
        public readonly string Text;
        public readonly byte[] Bytes;
        public readonly string EncodingName;
        public readonly long Length;
        public readonly long LastWriteTicksUtc;
        public readonly bool HasStamp;

        public TextFileAsset(string path, string text, byte[] bytes, string encodingName, bool hasStamp, long length, long lastWriteTicksUtc)
        {
            Path = path;
            Text = text;
            Bytes = bytes;
            EncodingName = encodingName;
            HasStamp = hasStamp;
            Length = length;
            LastWriteTicksUtc = lastWriteTicksUtc;
        }
    }
}