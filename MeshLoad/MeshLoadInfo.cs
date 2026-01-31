using System;

namespace DingoAssetsLoadSystem.MeshLoad
{
    [Serializable]
    public readonly struct MeshLoadInfo : IEquatable<MeshLoadInfo>
    {
        public readonly int MeshIndex;
        public readonly string MeshName;

        public readonly bool CombineAllMeshes;

        public readonly bool RecalculateNormals;
        public readonly bool RecalculateBounds;
        public readonly bool MarkNoLongerReadable;

        public MeshLoadInfo(int meshIndex = -1, string meshName = null, bool combineAllMeshes = false, bool recalculateNormals = false, bool recalculateBounds = true, bool markNoLongerReadable = true)
        {
            MeshIndex = meshIndex;
            MeshName = meshName;
            CombineAllMeshes = combineAllMeshes;
            RecalculateNormals = recalculateNormals;
            RecalculateBounds = recalculateBounds;
            MarkNoLongerReadable = markNoLongerReadable;
        }

        public bool Equals(MeshLoadInfo other) =>
            MeshIndex == other.MeshIndex && MeshName == other.MeshName && CombineAllMeshes == other.CombineAllMeshes && RecalculateNormals == other.RecalculateNormals && RecalculateBounds == other.RecalculateBounds && MarkNoLongerReadable == other.MarkNoLongerReadable;

        public override bool Equals(object obj) => obj is MeshLoadInfo other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                var h = 17;
                h = (h * 31) ^ MeshIndex;
                h = (h * 31) ^ (MeshName != null ? MeshName.GetHashCode() : 0);
                h = (h * 31) ^ CombineAllMeshes.GetHashCode();
                h = (h * 31) ^ RecalculateNormals.GetHashCode();
                h = (h * 31) ^ RecalculateBounds.GetHashCode();
                h = (h * 31) ^ MarkNoLongerReadable.GetHashCode();
                return h;
            }
        }
    }
}