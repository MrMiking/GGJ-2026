using UnityEngine;

namespace GGJ2026
{
    public interface IPooledObject
    {
        int PoolKey { get; set; }
        GameObject GameObject { get; }

        public void Initialize(int poolKey)
        {
            PoolKey = poolKey;
        }
        public void Release()
        {
            PoolManager.Instance.ReturnToPool(GameObject, PoolKey);
        }
    }
}