using UnityEngine;

namespace GGJ2026
{
    public sealed class Food : MonoBehaviour, IPooledObject
    {
        [SerializeField, Range(0, 1)] private float m_HealAmount = 0.15f;

        public int PoolKey { get; set; }
        public GameObject GameObject => gameObject;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out Health health))
            {
                var amount = m_HealAmount * health.MaxHealth;
                health.Apply(new Heal(this, amount));
                ((IPooledObject) this).Release();
            }
        }
    }
}