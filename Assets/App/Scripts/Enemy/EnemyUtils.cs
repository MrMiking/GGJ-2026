using UnityEngine;

namespace GGJ2026
{
    public static class EnemyUtils
    {
        public static Transform GetTarget()
        {
            if (PlayerController.Instance) return PlayerController.Instance.transform;
            if (DummyTarget.Instance) return DummyTarget.Instance.transform;
            return null;
        }
    }
}