using UnityEngine;

namespace GGJ2026
{
    [CreateAssetMenu(menuName = "SSO/Enemy/ContactEnemy", fileName = "SSO_ContactEnemySettings")]
    public class SSO_ContactEnemySettings : ScriptableObject
    {
        [Header("Settings")]
        public float Speed = 3f;
    }
}