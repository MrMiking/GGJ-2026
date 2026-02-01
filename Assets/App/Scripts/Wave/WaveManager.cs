using System;
using System.Linq;
using MVsToolkit.Utilities;
using NavMeshPlus.Components;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace GGJ2026
{
    public sealed class WaveManager: RegularSingleton<WaveManager>
    {
        [Header("Settings")]
        [SerializeField] private SSO_WaveConfig[] m_WavesConfig;
        [Space] 
        [SerializeField] private float m_RadiusSpawnOffset;
        [SerializeField] private float m_SpawnDelay = 2f;
        [Space]
        [SerializeField] private bool m_AutoStartOnAwake = false;

        private SSO_WaveConfig CurrentWaveConfig => m_WavesConfig[Mathf.Min(m_WavesConfig.Length - 1, IndexWave)];
        private int MaxEnemyAmount => m_WavesConfig[Mathf.Min(m_WavesConfig.Length - 1, IndexWave)].EnemiesCount;
        private int IndexWave => GameManager.Instance ? GameManager.Instance.Level -1: 0;

        private int CurrentEnemyAmount => EnemyManager.Instance.EnemyCount;
        
        private bool m_CanSpawn;
        
        private float m_TimerWave = 0f;
        private int m_BurstSpawned = 0;
    
        private void OnEnable()
        {
            if (m_AutoStartOnAwake) StartWaveSystem();
            GameTimeline.Instance.OnTimelineEventChange += CheckSystemUpdate;
        }

        private void CheckSystemUpdate(GameTimeline.TimelineEvent state)
        {
            if (state == GameTimeline.TimelineEvent.Wave)
            {
                StartWaveSystem();
            }
            else
            {
                StopWaveSystem();
            }
        }

        private void OnDisable()
        {
            GameTimeline.Instance.OnTimelineEventChange += CheckSystemUpdate;
            StopWaveSystem();
        }
        
        public void StartWaveSystem()
        {
            m_TimerWave = 0f;
            m_BurstSpawned = 0;
            this.Delay(()=>m_CanSpawn = true, m_SpawnDelay);
        }
        
        public void StopWaveSystem()
        {
            m_CanSpawn = false;
        }
        
        private void Update()
        {
            if (!m_CanSpawn || GameTimeline.Instance.IsPaused) return;
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
            GameObject enemySelected = RandomSelectEnemy();
            if (!enemySelected) return;
            Vector2 spawnPosition = RandomSpawnPosition();
            PoolManager.Instance.Spawn(enemySelected, spawnPosition,
                Quaternion.identity);
        }

        private GameObject RandomSelectEnemy()
        {
            float totalWeight = 0f;

            foreach (var enemy in CurrentWaveConfig.Enemies)
            {
                totalWeight += enemy.Percentage;
            }
            
            float randomWeight = Random.value * totalWeight;

            float cumulativeWeight = 0f;

            foreach (var enemy in CurrentWaveConfig.Enemies)
            {
                cumulativeWeight += enemy.Percentage;

                if (randomWeight <= cumulativeWeight) return enemy.Enemy;
            }

            return CurrentWaveConfig.Enemies.FirstOrDefault().Enemy;

        }

        private Vector2 RandomSpawnPosition()
        {
            Vector2 targetPosition = EnemyUtils.GetTarget().position;
            Vector2 spawnDirection = Random.insideUnitCircle.normalized;
            Vector2 position = targetPosition + spawnDirection * m_RadiusSpawnOffset;

            if (NavMesh.SamplePosition(position, out var hit, 100.0f, -1))
            {
                return hit.position;
            }

            return NavMeshSurface.activeSurfaces[0].navMeshData.sourceBounds.ClosestPoint(position);
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