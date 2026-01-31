using Unity.Cinemachine;
using UnityEngine;

namespace GGJ2026
{
    public sealed class Loot : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int m_MinGoldAmount = 1;
        [SerializeField] private int m_MaxGoldAmount = 3;
        [SerializeField, Range(0, 1)] private float m_FoodProbability = 0.01f;
        [SerializeField] private Food FoodPrefab;

        public void Drop()
        {
            var amount = Random.Range(m_MinGoldAmount, m_MaxGoldAmount);
            var stats = PlayerController.Instance.GetComponent<CharacterStats>();
            if (stats)
            {
                amount = (int) (amount * stats.GoldLootRate.Value);
            }
            GoldCoinSpawner.Instance.SpawnCoins(transform.position, amount);

            if (FoodPrefab != null && Random.value < m_FoodProbability)
            {
                PoolManager.Instance.Spawn(FoodPrefab, transform.position, Quaternion.identity);
            }
        }
    }
}
