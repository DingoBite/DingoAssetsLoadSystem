using UnityEngine;

namespace DingoAssetsLoadSystem
{
    public static class UnityReceiverLiveness
    {
        public static bool IsUnityAlive(this object receiver)
        {
            if (receiver == null)
                return false;
            if (receiver is Object uo)
                return uo != null;
            return true;
        }
    }
}