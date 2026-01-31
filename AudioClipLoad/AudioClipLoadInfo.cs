using System;
using UnityEngine;

namespace DingoAssetsLoadSystem.AudioClipLoad
{
    [Serializable]
    public readonly struct AudioClipLoadInfo : IEquatable<AudioClipLoadInfo>
    {
        public readonly AudioType AudioType;
        public readonly bool StreamAudio;
        public readonly bool CompressedInMemory;

        public AudioClipLoadInfo(AudioType audioType, bool streamAudio = false, bool compressedInMemory = false)
        {
            AudioType = audioType;
            StreamAudio = streamAudio;
            CompressedInMemory = compressedInMemory;
        }

        public bool Equals(AudioClipLoadInfo other) =>
            AudioType == other.AudioType && StreamAudio == other.StreamAudio && CompressedInMemory == other.CompressedInMemory;

        public override bool Equals(object obj) => obj is AudioClipLoadInfo other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                var h = 17;
                h = (h * 31) ^ (int)AudioType;
                h = (h * 31) ^ StreamAudio.GetHashCode();
                h = (h * 31) ^ CompressedInMemory.GetHashCode();
                return h;
            }
        }
    }
}