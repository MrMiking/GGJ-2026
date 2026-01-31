using UnityEngine;

namespace GGJ2026
{
    public sealed class LootCollectorRadiusBinder : MonoBehaviour
    {
        [SerializeField] private CharacterStats m_CharacterStats;
        [SerializeField] private LootCollector m_LootCollector;

        private void Update()
        {
            m_LootCollector.Radius = m_CharacterStats.GoldLootRange.Value;
        }
    }
}