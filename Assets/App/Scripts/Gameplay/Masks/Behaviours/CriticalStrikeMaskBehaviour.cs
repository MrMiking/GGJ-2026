using UnityEngine;

namespace GGJ2026
{
    public sealed class CriticalStrikeMaskBehaviour : MaskBehaviour
    {
        [SerializeField] private float CriticalChancePerLevel = 0.2f;

        public override void OnMaskAttached(in MaskAttachContext context)
        {
            var characterStats = GetComponentInParent<CharacterStats>();
            if (characterStats != null) 
            {
                var value = CriticalChancePerLevel * (Level + 1);
                characterStats.CriticalChance.AddModifier(new StatModifier(value, StatModifierType.Flat, this));
            }
        }

        public override void OnLevelChange()
        {
            var characterStats = GetComponentInParent<CharacterStats>();
            if (characterStats != null)
            {
                var value = CriticalChancePerLevel * (Level + 1);
                characterStats.CriticalChance.RemoveAllModifiersFromSource(this);
                characterStats.CriticalChance.AddModifier(new StatModifier(value, StatModifierType.Flat, this));
            }
        }
    }
}