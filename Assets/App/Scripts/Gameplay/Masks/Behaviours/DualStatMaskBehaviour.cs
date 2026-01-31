using System;
using UnityEngine;

namespace GGJ2026
{
    public sealed class DualStatMaskBehaviour : MaskBehaviour
    {
        public enum StatType
        {
            HealthPoints,
            MovementSpeed,
            GoldLootRate,
            GoldLootRange,

            BulletDamage,
            FireRate,
            BulletSpeed,
            BulletSize,
            BulletBounce,
            BulletPierce,
            BulletSpread,
        }

        [Serializable]
        public struct StatEffect
        {
            public StatType stat;
            public StatModifierType modifierType;
            public float[] statValuePerLevel;
        }

        [SerializeField] private StatEffect[] m_StatEffects;

        public override void OnMaskAttached(in MaskAttachContext context)
        {
            var characterStats = GetComponentInParent<CharacterStats>();
            if (characterStats != null)
            {
                for (int i = 0; i < m_StatEffects.Length; i++)
                {
                    ref var statEffect = ref m_StatEffects[i];
                    var stat = GetCharacterStat(characterStats, statEffect.stat);
                    var value = statEffect.statValuePerLevel[Level - 1];
                    stat.AddModifier(new StatModifier(value, statEffect.modifierType, this));
                }
            }
        }

        public override void OnLevelChange()
        {
            var characterStats = GetComponentInParent<CharacterStats>();
            if (characterStats != null)
            {
                for (int i = 0; i < m_StatEffects.Length; i++)
                {
                    ref var statEffect = ref m_StatEffects[i];
                    var stat = GetCharacterStat(characterStats, statEffect.stat);
                    var value = statEffect.statValuePerLevel[Level - 1];
                    stat.RemoveAllModifiersFromSource(this);
                    stat.AddModifier(new StatModifier(value, statEffect.modifierType, this));
                }
            }
        }

        private void OnDestroy()
        {
            var characterStats = GetComponentInParent<CharacterStats>();
            if (characterStats != null)
            {
                for (int i = 0; i < m_StatEffects.Length; i++)
                {
                    ref var statEffect = ref m_StatEffects[i];
                    var stat = GetCharacterStat(characterStats, statEffect.stat);
                    stat.RemoveAllModifiersFromSource(this);
                }
            }
        }

        private void OnValidate()
        {
            for (int i = 0; i < m_StatEffects.Length; i++)
            {
                Array.Resize(ref m_StatEffects[i].statValuePerLevel, Mask.MaximumMaskLevel);
            }
        }

        private Stat GetCharacterStat(CharacterStats stats, StatType stat)
        {
            switch (stat)
            {
                case StatType.HealthPoints: return stats.HealthPoints;
                case StatType.MovementSpeed: return stats.MovementSpeed;
                case StatType.GoldLootRate: return stats.GoldLootRate;
                case StatType.GoldLootRange: return stats.GoldLootRange;

                case StatType.BulletDamage: return stats.BulletDamage;
                case StatType.FireRate: return stats.FireRate;
                case StatType.BulletSpeed: return stats.BulletSpeed;
                case StatType.BulletSize: return stats.BulletSize;
                case StatType.BulletBounce: return stats.BulletBounce;
                case StatType.BulletSpread: return stats.BulletSpread;
                case StatType.BulletPierce: return stats.BulletPierce;

            }
            return null;
        }
    }
}