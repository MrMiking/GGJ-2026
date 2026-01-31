using MVsToolkit.Dev;
using UnityEngine;
using UnityEngine.Events;

namespace GGJ2026
{
    [RequireComponent(typeof(ParticleSystem))]
    public sealed class GoldCoinSpawner : RegularSingleton<GoldCoinSpawner>
    {
        private static ParticleSystem.Particle[] s_ParticleBuffer = new ParticleSystem.Particle[65536];

        public UnityEvent<Vector2> OnCoinCollected;

        private ParticleSystem m_ParticleSystem;

        protected override void Awake()
        {
            base.Awake();
            m_ParticleSystem = GetComponent<ParticleSystem>();
        }

        public void SpawnCoins(Vector2 position, int amount)
        {
            m_ParticleSystem.transform.position = position;
            m_ParticleSystem.Emit(amount);
        }

        public void AttractCoins(Vector2 position, float attractionRadius, float collectRadius, float attractionSpeed)
        {
            int count = m_ParticleSystem.GetParticles(s_ParticleBuffer);
            float attractionRadiusSq = attractionRadius * attractionRadius;
            float collectRadiusSq = collectRadius * collectRadius;

            for (int i = 0; i < count; i++)
            {
                var particlePos = (Vector2) s_ParticleBuffer[i].position;
                float distSq = (particlePos - position).sqrMagnitude;

                if (distSq < attractionRadiusSq)
                {
                    s_ParticleBuffer[i].position = Vector2.MoveTowards(
                        particlePos,
                        position,
                        attractionSpeed * Time.deltaTime
                    );

                    if (distSq < collectRadiusSq)
                    {
                        GameManager.Instance.CurrentGold++;
                        OnCoinCollected.Invoke(particlePos);
                        Debug.Log(GameManager.Instance.CurrentGold);
                        s_ParticleBuffer[i].remainingLifetime = 0;
                    }
                }
            }

            m_ParticleSystem.SetParticles(s_ParticleBuffer, count);
        }

        [Button]
        private void SpawnRandomGoldCoins()
        {
            var pos = new Vector2(-10f + Random.value * 20f, -10f + Random.value * 20f);
            SpawnCoins(pos, Random.Range(3, 10));
        }
    }
}