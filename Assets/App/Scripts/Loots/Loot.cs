using UnityEngine;

namespace GGJ2026
{
    public sealed class Loot : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int m_MinGoldAmount = 1;
        [SerializeField] private int m_MaxGoldAmount = 3;

        public void SpawnLoot()
        {
            var amount = Random.Range(m_MinGoldAmount, m_MaxGoldAmount);
            GoldCoinSpawner.Instance.SpawnCoins(transform.position, amount);
        }
    }
}
