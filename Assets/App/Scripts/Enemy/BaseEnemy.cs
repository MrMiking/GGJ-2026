using UnityEngine;

namespace GGJ2026
{
    public abstract class BaseEnemy : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] protected Health m_Health;
        
        public Health Health => m_Health;
        
        public abstract void  RefreshPath();

        public virtual void UpdateState(){}

        public virtual void SetAutoPilot(bool autoPiloted){}
        
        protected abstract void Die();
    }
}