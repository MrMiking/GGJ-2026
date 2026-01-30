using UnityEngine;

namespace GGJ2026
{
    public sealed class StatModifierMaskBehaviour : MaskBehaviour
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

        [SerializeField] private StatModifierType m_ModifierType = StatModifierType.Flat;
        [SerializeField] private StatType m_Stat = StatType.HealthPoints;
        [SerializeField] private float m_StatValuePerLevel = 0.2f;

        public override void OnMaskAttached(in MaskAttachContext context)
        {
            var stat = GetCharacterStat(m_Stat);
            if (stat != null)
            {
                var value = m_StatValuePerLevel * (Level + 1);
                stat.AddModifier(new StatModifier(value, m_ModifierType, this));
            }
        }

        public override void OnLevelChange()
        {
            var stat = GetCharacterStat(m_Stat);
            if (stat != null)
            {
                var value = m_StatValuePerLevel * (Level + 1);
                stat.RemoveAllModifiersFromSource(this);
                stat.AddModifier(new StatModifier(value, m_ModifierType, this));
            }
        }

        private void OnDestroy()
        {
            var stat = GetCharacterStat(m_Stat);
            if (stat != null)
            {
                stat.RemoveAllModifiersFromSource(this);
            }
        }

        private Stat GetCharacterStat(StatType stat)
        {
            var characterStats = GetComponentInParent<CharacterStats>();
            if (characterStats != null)
            {
                switch (stat)
                {
                    case StatType.HealthPoints: return characterStats.HealthPoints;
                    case StatType.MovementSpeed: return characterStats.MovementSpeed;
                    case StatType.GoldLootRate: return characterStats.GoldLootRate;
                    case StatType.GoldLootRange: return characterStats.GoldLootRange;

                    case StatType.BulletDamage: return characterStats.BulletDamage;
                    case StatType.FireRate: return characterStats.FireRate;
                    case StatType.BulletSpeed: return characterStats.BulletSpeed;
                    case StatType.BulletSize: return characterStats.BulletSize;
                    case StatType.BulletBounce: return characterStats.BulletBounce;
                    case StatType.BulletSpread: return characterStats.BulletSpread;
                    case StatType.BulletPierce: return characterStats.BulletPierce;

                }
            }
            return null;
        }
    }
}