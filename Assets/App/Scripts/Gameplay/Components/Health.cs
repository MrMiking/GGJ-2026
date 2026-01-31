using MVsToolkit.Dev;
using UnityEngine;
using UnityEngine.Events;

namespace GGJ2026
{
    public class Health : MonoBehaviour
    {
        public delegate void OnDamageDelegate(float previousHealth, float newHealth, in Damage damage);
        public delegate void OnHealDelegate(float previousHealth, float newHealth, in Heal heal);
        public delegate void OnMaxHealthChangedDelegate(float previousValue, float newValue);
        public delegate void OnDeathDelegate();

        [SerializeField] private float m_MaxHealth = 100.0f;
        [SerializeField] private float m_CurrentHealth = 100.0f;

        [SerializeField] private UnityEvent OnDamageEvent;
        [SerializeField] private UnityEvent OnHealEvent;
        [SerializeField] private UnityEvent OnMaxHealthChangeEvent;
        [SerializeField] private UnityEvent OnDeathEvent;

        public event OnDamageDelegate OnDamage;
        public event OnHealDelegate OnHeal;
        public event OnMaxHealthChangedDelegate OnMaxHealthChange;
        public event OnDeathDelegate OnDeath;

        public float CurrentHealth
        {
            get => m_CurrentHealth;
            private set => m_CurrentHealth = Mathf.Clamp(value, 0.0f, m_MaxHealth);
        }

        public float MaxHealth
        {
            get => m_MaxHealth;
            set
            {
                m_MaxHealth = value;
                CurrentHealth = Mathf.Clamp(CurrentHealth, 0.0f, m_MaxHealth);
                OnMaxHealthChange?.Invoke(CurrentHealth, m_MaxHealth);
            }
        }

        public bool IsAlive => CurrentHealth > 0.0f;
        public bool IsDead => CurrentHealth == 0.0f;

        public void ResetComponent()
        {
            CurrentHealth = MaxHealth;
            OnDeath = null;
        }
        
        public void Apply(in Damage damage)
        {
            var lastHealth = CurrentHealth;
            CurrentHealth -= damage.Value;
            OnDamage?.Invoke(lastHealth, CurrentHealth, in damage);

            if (CurrentHealth == 0.0f)
            {
                OnDeath?.Invoke();
            }
        }

        public void Apply(in Heal heal)
        {
            var lastHealth = CurrentHealth;
            CurrentHealth += heal.Value;
            OnHeal?.Invoke(lastHealth, CurrentHealth, in heal);
        }
    }

    public struct Heal
    {
        public object Source { get; }
        public float Value { get; }
        public bool IsCritical { get; }

        private Heal(object source, float value, bool isCritical)
        {
            Source = source;
            Value = value;
            IsCritical = isCritical;
        }

        public Heal(object source, float value) : this(source, value, false) { }

        public Heal(float value) : this(null, value) { }

        public Heal AsCritical(float criticalMultiplier = 1.0f)
        {
            return new Heal(Source, Value * criticalMultiplier, true);
        }
    }

    public enum DamageType
    {
        Physical,
        Magical,
        Passthrough
    }

    public struct Damage
    {
        public object Source { get; }
        public DamageType DamageType { get; }
        public float Value { get; }
        public bool IsCritical { get; }

        private Damage(object source, DamageType damageType, float damage, bool isCritical)
        {
            Source = source;
            DamageType = damageType;
            Value = damage;
            IsCritical = isCritical;
        }

        public Damage(object source, DamageType damageType, float damage)
            : this(source, damageType, damage, false)
        {
        }

        public Damage(DamageType damageType, float damage)
            : this(null, damageType, damage, false)
        {
        }

        public Damage AsCritical(float criticalMultiplier = 1.0f)
        {
            return new Damage(Source, DamageType, Value * criticalMultiplier, true);
        }
    }
}