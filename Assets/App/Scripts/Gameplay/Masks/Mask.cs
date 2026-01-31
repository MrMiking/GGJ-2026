using UnityEngine;

namespace GGJ2026
{
    [CreateAssetMenu(fileName = "New Mask", menuName = "GGJ2026/Mask")]
    public sealed class Mask : ScriptableObject
    {
        public const int MaximumMaskLevel = 5;

        [Header("Properties")]
        [SerializeField] private MaskRarity m_Rarity;
        [SerializeField, Range(1, MaximumMaskLevel)] private int m_MaximumLevel = 5;
        [SerializeField] private MaskBehaviour m_BehaviourPrefab;

        [Space, Header("Visual")]
        [SerializeField] private Sprite m_Sprite;

        public MaskRarity Rarity => m_Rarity;
        public int MaximumLevel => m_MaximumLevel;
        public MaskBehaviour BehaviourPrefab => m_BehaviourPrefab;
        public Sprite Sprite => m_Sprite;
    }

    public enum MaskRarity
    {
        Common,
        Rare,
        Epic
    }
}