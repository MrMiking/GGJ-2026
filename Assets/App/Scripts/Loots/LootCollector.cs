using GGJ2026;
using UnityEngine;

public class LootCollector : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float m_AspirationSpeed = 5.0f;
    [SerializeField] private float m_CollectRadius = 0.5f;

    public float Radius = 10.0f;

    void LateUpdate()
    {
        var goldSystem = GoldCoinSpawner.Instance;
        goldSystem.AttractCoins(transform.position, Radius, m_CollectRadius, m_AspirationSpeed);
    }

    private void OnDrawGizmosSelected()
    {
        var color = Color.yellow;
        Gizmos.color = color;
        Gizmos.DrawWireSphere(transform.position, m_CollectRadius);
        color.a = 0.5f;
        Gizmos.color = color;
        Gizmos.DrawWireSphere(transform.position, Radius);
    }
}