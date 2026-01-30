using UnityEngine;

namespace GGJ2026
{
    public sealed class CharacterStats : MonoBehaviour
    {
        public Stat AttackSpeed { get; } = new Stat(1.0f, 0.1f, 4.0f);
        public Stat CriticalChance { get; } = new Stat(0.0f, 0.0f, 1.0f);
        public Stat CriticalDamage { get; } = new Stat(1.5f, 1.0f);
    }
}