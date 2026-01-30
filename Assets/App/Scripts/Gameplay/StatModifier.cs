namespace GGJ2026
{
    public enum StatModifierType
    {
        Flat,
        PercentAdd,
        PercentMult
    }

    public struct StatModifier
    {
        public readonly StatModifierType Type;
        public readonly float Value;
        public readonly object Source;

        public StatModifier(float value, StatModifierType type, object source = null)
        {
            Value = value;
            Type = type;
            Source = source;
        }
    }
}