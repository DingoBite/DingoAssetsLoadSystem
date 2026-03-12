#if NEWTONSOFT_EXISTS
using System;
using DingoAssetsLoadSystem.TextLoad;

namespace DingoAssetsLoadSystem.ParsedTextLoad
{
    public readonly struct ParsedTextLoadInfo<T> where T : class
    {
        public readonly TextLoadInfo TextInfo;
        public readonly ITextParser<T> Parser;
        public readonly bool ParseOnThreadPool;

        public ParsedTextLoadInfo(TextLoadInfo textInfo, ITextParser<T> parser, bool parseOnThreadPool = true)
        {
            TextInfo = textInfo;
            Parser = parser;
            ParseOnThreadPool = parseOnThreadPool;
        }
    }

    public readonly struct ParsedTextCacheKey : IEquatable<ParsedTextCacheKey>
    {
        public readonly TextCacheKey RawKey;
        public readonly int ParserIdHash;
        public readonly int ParserVersion;

        public ParsedTextCacheKey(TextCacheKey rawKey, int parserIdHash, int parserVersion)
        {
            RawKey = rawKey;
            ParserIdHash = parserIdHash;
            ParserVersion = parserVersion;
        }

        public bool Equals(ParsedTextCacheKey other) =>
            RawKey.Equals(other.RawKey) && ParserIdHash == other.ParserIdHash && ParserVersion == other.ParserVersion;

        public override bool Equals(object obj) => obj is ParsedTextCacheKey other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                int h = 17;
                h = (h * 31) ^ RawKey.GetHashCode();
                h = (h * 31) ^ ParserIdHash;
                h = (h * 31) ^ ParserVersion;
                return h;
            }
        }
    }

    public sealed class ParsedTextKeyFactory<T> : ICacheKeyFactory<ParsedTextCacheKey, ParsedTextLoadInfo<T>> where T : class
    {
        private readonly TextKeyFactory _rawFactory = new();

        public ParsedTextCacheKey CreateKey(string path, ParsedTextLoadInfo<T> info)
        {
            var rawKey = _rawFactory.CreateKey(path, info.TextInfo);

            var parser = info.Parser;
            var id = parser?.Id ?? typeof(T).FullName ?? "parser";
            var ver = parser?.Version ?? 0;

            return new ParsedTextCacheKey(rawKey, StringComparer.Ordinal.GetHashCode(id), ver);
        }
    }
}
#endif