using UnityEngine;

namespace GGJ2026
{
    [CreateAssetMenu(menuName = "SSO/Enemy/ContactEnemy", fileName = "SSO_ContactEnemySettings")]
    public class SSO_ContactEnemySettings : ScriptableObject
    {
        [Header("Settings")]
        public float Speed = 3f;
        [Space]
        public float AttackRange = 0.5f;
        public float AttackCooldown = 1.0f;
        public int AttackDamage = 10;
        public float KnockbackForce = 10f;
    }
}