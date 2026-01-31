using UnityEngine;

namespace GGJ2026
{
    public abstract class BaseEnemy : MonoBehaviour, IPooledObject
    {
        [Header("References")]
        [SerializeField] protected Health m_Health;
        
        public Health Health => m_Health;

        #region Update Methods

        public abstract void  RefreshPath();

        public virtual void UpdateState(){}

        public virtual void UpdateAutoPilot(){}

        #endregion

        #region State Methods

        protected virtual void Die()
        {
            m_Health.OnDeath -= Die;
            ((IPooledObject)this).Release();
        }

        #endregion

        #region IPooledObject Implementation

        public int PoolKey { get; set; }
        public GameObject GameObject => gameObject;

        public virtual void Initialize(int poolKey)
        {
            PoolKey = poolKey;
            m_Health.Apply(new Heal(m_Health.MaxHealth));
            m_Health.OnDeath += Die;
        }

        #endregion
    }
}