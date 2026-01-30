using UnityEngine;

namespace GGJ2026
{
    public sealed class CriticalStrikeMaskBehaviour : MaskBehaviour
    {
        [SerializeField] private float CriticalChancePerLevel = 0.2f;

        public override void OnMaskAttached(in MaskAttachContext context)
        {
            // context.player.Stats.CriticalChance.AddModifier(...)
        }

        public override void OnLevelChange()
        {
            // context.player.Stats.CriticalChance.RemoveModifier(...)
            // context.player.Stats.CriticalChance.AddModifier(...)
        }
    }
}