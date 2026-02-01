using System;
using System.Text;

namespace DingoAssetsLoadSystem.TextLoad
{
    public readonly struct TextLoadInfo : IEquatable<TextLoadInfo>
    {
        public readonly bool UseFileStampInKey;
        public readonly bool KeepBytes;
        public readonly bool DecodeText;
        public readonly bool NormalizeNewLines;
        public readonly bool StripBom;
        public readonly string EncodingName;

        public TextLoadInfo(bool useFileStampInKey = true, bool keepBytes = false, bool decodeText = true, bool normalizeNewLines = false, bool stripBom = true, string encodingName = "utf-8")
        {
            UseFileStampInKey = useFileStampInKey;
            KeepBytes = keepBytes;
            DecodeText = decodeText;
            NormalizeNewLines = normalizeNewLines;
            StripBom = stripBom;
            EncodingName = encodingName;
        }

        public Encoding GetEncoding()
        {
            if (string.IsNullOrWhiteSpace(EncodingName))
                return Encoding.UTF8;

            try
            {
                return Encoding.GetEncoding(EncodingName);
            }
            catch
            {
                return Encoding.UTF8;
            }
        }

        public bool Equals(TextLoadInfo other) =>
            UseFileStampInKey == other.UseFileStampInKey && KeepBytes == other.KeepBytes && DecodeText == other.DecodeText && NormalizeNewLines == other.NormalizeNewLines && StripBom == other.StripBom && string.Equals(EncodingName, other.EncodingName, StringComparison.OrdinalIgnoreCase);

        public override bool Equals(object obj) => obj is TextLoadInfo other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                int h = 17;
                h = (h * 31) ^ UseFileStampInKey.GetHashCode();
                h = (h * 31) ^ KeepBytes.GetHashCode();
                h = (h * 31) ^ DecodeText.GetHashCode();
                h = (h * 31) ^ NormalizeNewLines.GetHashCode();
                h = (h * 31) ^ StripBom.GetHashCode();
                h = (h * 31) ^ (EncodingName != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(EncodingName) : 0);
                return h;
            }
        }
    }
}