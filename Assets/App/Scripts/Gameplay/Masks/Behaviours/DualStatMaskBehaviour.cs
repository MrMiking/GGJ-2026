using System;
using System.Linq;
using System.Text;
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

        [SerializeField] private StatEffect[] m_StatEffects = new StatEffect[0];

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

        public override string GetFormattedDescription()
        {
            static string FormatSigned(float value)
            {
                return value.ToString("+0.##;-0.##");
            }

            static string FormatEnumName(StatType value)
            {
                string text = value.ToString();
                var sb = new StringBuilder();

                for (int i = 0; i < text.Length; i++)
                {
                    char c = text[i];

                    if (i > 0 && char.IsUpper(c))
                        sb.Append(' ');

                    sb.Append(c);
                }

                return sb.ToString();
            }

            var args = m_StatEffects.Select(effect =>
            {
                var name = FormatEnumName(effect.stat);
                var value = "?";
                switch (effect.modifierType)
                {
                    case StatModifierType.Flat: 
                        value = FormatSigned(effect.statValuePerLevel[Level]);
                        break;
                    case StatModifierType.PercentAdd:
                        value = FormatSigned(effect.statValuePerLevel[Level] * 100f) + "%";
                        break;
                    case StatModifierType.PercentMult: 
                        value = FormatSigned(effect.statValuePerLevel[Level] * 100f) + "%";
                        break;
                }

                return $"{name} {value}";
            });

            return string.Join("\n", args);
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