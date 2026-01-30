using UnityEngine;

namespace GGJ2026
{
    public sealed class CharacterStats : MonoBehaviour
    {
        [Header("Base Player Stats")]
        [SerializeField] private int m_HealthPoints = 50;
        [SerializeField] private float m_MovementSpeed = 3.0f;
        [SerializeField] private float m_GoldLootRange = 8.0f;

        [Header("Base Bullet Stats")]
        [SerializeField] private float m_BulletDamage = 5.0f;
        [SerializeField] private float m_FireRate = 2.0f;
        [SerializeField] private float m_BulletSpeed = 10.0f;
        [SerializeField] private float m_BulletSize = 1.0f;
        [SerializeField] private int m_BulletBounce = 0;
        [SerializeField] private int m_BulletPierce = 5;
        [SerializeField] private int m_BulletSpread = 1;

        public Stat HealthPoints { get; private set; }
        public Stat MovementSpeed { get; private set; }
        public Stat GoldLootRate { get; private set; }
        public Stat GoldLootRange { get; private set; }

        public Stat BulletDamage { get; private set; }
        public Stat FireRate { get; private set; }
        public Stat BulletSpeed { get; private set; }
        public Stat BulletSize { get; private set; }
        public Stat BulletBounce { get; private set; }
        public Stat BulletPierce { get; private set; }
        public Stat BulletSpread { get; private set; }

        private void Awake()
        {
            HealthPoints = new Stat(m_HealthPoints, 1);
            MovementSpeed = new Stat(m_MovementSpeed, 0.5f);
            GoldLootRate = new Stat(1);
            GoldLootRange = new Stat(m_GoldLootRange, 0.1f);

            FireRate = new Stat(m_FireRate, 0.1f);
            BulletDamage = new Stat(m_BulletDamage, 0.0f);
            BulletSpeed = new Stat(m_BulletSpeed, 0.0f);
            BulletBounce = new Stat(m_BulletBounce, 0.0f);
            BulletSize = new Stat(m_BulletSize, 0.1f);
            BulletPierce = new Stat(m_BulletPierce, 0.0f);
            BulletSpread = new Stat(m_BulletSpread, 1.0f);
        }

        private void OnValidate()
        {
            if (HealthPoints == null)
                return;

            HealthPoints.BaseValue = m_HealthPoints;
            MovementSpeed.BaseValue = m_MovementSpeed;
            GoldLootRate.BaseValue = 1;
            GoldLootRange.BaseValue = m_GoldLootRange;

            FireRate.BaseValue = m_FireRate;
            BulletDamage.BaseValue = m_BulletDamage;
            BulletSpeed.BaseValue = m_BulletSpeed;
            BulletBounce.BaseValue = m_BulletBounce;
            BulletSize.BaseValue = m_BulletSize;
            BulletPierce.BaseValue = m_BulletPierce;
            BulletSpread.BaseValue = m_BulletSpread;
        }
    }
}