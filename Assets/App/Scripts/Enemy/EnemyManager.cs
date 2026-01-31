using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace GGJ2026
{
    public sealed class EnemyManager : RegularSingleton<EnemyManager>
    {
        private readonly List<BaseEnemy> m_Enemies = new();
        private readonly Stopwatch m_Stopwatch = new();

        private int m_IndexEnemyUpdated = -1;
        private int m_EnemyUpdatedThisFrame;
        private int m_TimeSinceLastAutoPilotCheckMs;
        
        
        
        private const int k_MaxMillisecondsPerFrame = (int)(1f/60f/8f * 1000);
        private const int k_AutoPilotRefreshRateMs = 1000;
        
        public int EnemyCount => m_Enemies.Count;
        
        public void Register(BaseEnemy baseEnemy)
        {
            m_Enemies.Add(baseEnemy);
        }

        public void Unregister(BaseEnemy baseEnemy)
        {
            m_Enemies.Remove(baseEnemy);
        }
        
        private void Update()
        {
            RefreshEnemiesPath();
            UpdateEnemiesState();
            CheckEnemiesAutoPilot();
        }

        private void UpdateEnemiesState()
        {
            foreach (BaseEnemy enemy in m_Enemies)
            {
                enemy.UpdateState();
            }
        }

        private void RefreshEnemiesPath()
        {
            m_Stopwatch.Restart();
            m_EnemyUpdatedThisFrame = 0;

            while (m_Enemies.Count > 0 && m_Stopwatch.ElapsedMilliseconds < k_MaxMillisecondsPerFrame && 
                   m_EnemyUpdatedThisFrame < m_Enemies.Count)
            {
                m_IndexEnemyUpdated = (m_IndexEnemyUpdated + 1) % m_Enemies.Count;
                m_EnemyUpdatedThisFrame++;
                m_Enemies[m_IndexEnemyUpdated].RefreshPath();
            }
            m_Stopwatch.Stop();
        }
        
        private void CheckEnemiesAutoPilot()
        {
            if (m_TimeSinceLastAutoPilotCheckMs < k_AutoPilotRefreshRateMs)
            {
                m_TimeSinceLastAutoPilotCheckMs += (int)(Time.deltaTime * 1000);
                return;
            }
            
            foreach (BaseEnemy enemy in m_Enemies)
            {
                enemy.UpdateAutoPilot();
            }
            
            m_TimeSinceLastAutoPilotCheckMs = 0;
        }
    }
}