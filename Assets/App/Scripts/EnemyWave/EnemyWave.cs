using System;
using MVsToolkit.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GGJ2026
{
    public sealed class EnemyWave: RegularSingleton<EnemyWave>
    {
        [Header("Settings")]
        [SerializeField] private SSO_WaveConfig[] m_WavesConfig;
        [Space] 
        [SerializeField] private float m_RadiusSpawnOffset;
        [Space]
        [SerializeField] private bool m_AutoStartOnAwake = true;

        private SSO_WaveConfig CurrentWaveConfig => m_WavesConfig[Mathf.Min(m_WavesConfig.Length - 1, IndexWave)];
        private int MaxEnemyAmount => m_WavesConfig[Mathf.Min(m_WavesConfig.Length - 1, IndexWave)].EnemiesCount;
        private int IndexWave => GameManager.Instance ? GameManager.Instance.Level : 0;

        private int CurrentEnemyAmount => EnemyManager.Instance.EnemyCount;
        
        private bool m_CanSpawn = true;
        
        private float m_TimerWave = 0f;
        private int m_BurstSpawned = 0;
    
        private void OnEnable()
        {
            if (m_AutoStartOnAwake) StartWaveSystem();
        }
        
        private void OnDisable()
        {
            StopWaveSystem();
        }
        
        public void StartWaveSystem()
        {
            m_CanSpawn = true;
            m_TimerWave = 0f;
            m_BurstSpawned = 0;
        }
        
        public void StopWaveSystem()
        {
            m_CanSpawn = false;
        }
        
        private void Update()
        {
            if (!m_CanSpawn) return;
            if (CurrentEnemyAmount < MaxEnemyAmount) SpawnEnemies(MaxEnemyAmount - CurrentEnemyAmount);
            if (CanSpawnBurst())
            {
                SpawnEnemies(CurrentWaveConfig.Bursts[m_BurstSpawned].BurstCount);
                m_BurstSpawned++;
            }
        }

        private bool CanSpawnBurst()
        {
            if (CurrentWaveConfig.Bursts.Length == 0) return false;
            if (CurrentWaveConfig.Bursts.Length <= m_BurstSpawned) return false;
            
            m_TimerWave += Time.deltaTime;
            if (m_TimerWave >= CurrentWaveConfig.Bursts[m_BurstSpawned].TimestampStart)
            {
                m_TimerWave = 0f;
                return true;
            }
            return false;
        }

        private void SpawnEnemies(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                SpawnEnemy();
            }
        }

        private void SpawnEnemy()
        {
            Vector2 spawnPosition = RandomSpawnPosition();
            PoolManager.Instance.Spawn(CurrentWaveConfig.EnemiesPrefabs.GetRandom(), spawnPosition,
                Quaternion.identity);
        }

        private Vector2 RandomSpawnPosition()
        {
            Vector2 targetPosition = EnemyUtils.GetTarget().position;
            Vector2 spawnDirection = Random.insideUnitCircle.normalized;
            Vector2 position = targetPosition + spawnDirection * m_RadiusSpawnOffset;
            return position;
        }


        private void OnDrawGizmosSelected()
        {

            Transform target = EnemyUtils.GetTarget() ?? transform;
            Vector2 targetPosition = target.position;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(targetPosition, m_RadiusSpawnOffset);
        }
    }
}