using Unity.VisualScripting;
using UnityEngine;

public class LootCollector : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float m_AspirationSpeed = 5.0f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Loot loot))
        {
            loot.Collect(this, m_AspirationSpeed);
        }
    }
}