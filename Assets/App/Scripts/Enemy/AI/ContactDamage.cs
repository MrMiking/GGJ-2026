using System;
using UnityEngine;

namespace GGJ2026
{
    public sealed class ContactDamage : MonoBehaviour
    {
        public float damage = 10;
        public float knockbackForce = 10;
        public float cooldown = 0.5f;
        public float radius = 0.5f;

        private float m_LastPlayerDamageTime = float.NegativeInfinity;

        public Action OnAttack;

        public void SkipCooldown()
        {
            m_LastPlayerDamageTime = float.NegativeInfinity;
        }

        private void Update()
        {
            var target = EnemyUtils.GetTarget();

            if (Time.time - m_LastPlayerDamageTime < cooldown || Vector2.Distance(target.position, transform.position) > radius)
                return;

            if (target.TryGetComponent(out Health health))
            {
                var damage = new Damage(this, DamageType.Physical, this.damage);
                health.Apply(damage);
            }

            if (target.TryGetComponent(out PlayerController playerController))
            {
                var knockbackDirection = ((Vector2)(target.position - transform.position)).normalized;
                playerController.AddForce(knockbackDirection * knockbackForce);
            }

            m_LastPlayerDamageTime = Time.time;
            OnAttack?.Invoke();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, radius);
        }

    }
}