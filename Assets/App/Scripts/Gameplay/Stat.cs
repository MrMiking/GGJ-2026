using System.Collections.Generic;
using UnityEngine;

namespace GGJ2026
{
    public sealed class Stat
    {
        private float m_BaseValue;
        private float m_Value;
        private float m_MinValue;
        private float m_MaxValue;
        private List<StatModifier> m_Modifiers = new();
        private bool m_IsDirty;

        public float BaseValue
        {
            get => m_BaseValue;
            set
            {
                m_BaseValue = value;
                m_IsDirty = true;
            }
        }

        public float MinValue
        {
            get => m_MinValue;
            set
            {
                m_MinValue = value;
                m_IsDirty = true;
            }
        }

        public float MaxValue
        {
            get => m_MaxValue;
            set
            {
                m_MaxValue = value;
                m_IsDirty = true;
            }
        }

        public float Value
        {
            get
            {
                if (m_IsDirty)
                {
                    m_Value = CalculateFinalValue();
                    m_IsDirty = false;
                }
                return m_Value;
            }
        }

        public IReadOnlyCollection<StatModifier> Modifiers { get; }

        public Stat(float baseValue, float minValue = float.NegativeInfinity, float maxValue = float.PositiveInfinity)
        {
            BaseValue = baseValue;
            Modifiers = m_Modifiers.AsReadOnly();
            m_MinValue = minValue;
            m_MaxValue = maxValue;
        }

        public void AddModifier(StatModifier modifier)
        {
            m_IsDirty = true;
            m_Modifiers.Add(modifier);
        }

        public bool RemoveModifier(StatModifier modifier)
        {
            if (m_Modifiers.Remove(modifier))
            {
                m_IsDirty = true;
                return true;
            }
            return false;
        }

        public int RemoveAllModifiersFromSource(object source)
        {
            return m_Modifiers.RemoveAll(modifier => modifier.Source == source);
        }

        public float CalculateFinalValue()
        {
            float sumFlat = 0;
            float sumPercentAdd = 0;
            float productPercentMult = 1.0f;

            foreach (var modifier in m_Modifiers)
            {
                switch (modifier.Type)
                {
                    case StatModifierType.Flat:
                        sumFlat += modifier.Value;
                        break;
                    case StatModifierType.PercentAdd:
                        sumPercentAdd += modifier.Value;
                        break;
                    case StatModifierType.PercentMult:
                        productPercentMult *= 1 + modifier.Value;
                        break;
                }
            }

            var finalValue = (BaseValue + sumFlat) * (1 + sumPercentAdd) * productPercentMult;
            return Mathf.Clamp(finalValue, m_MinValue, m_MaxValue);
        }
    }
}