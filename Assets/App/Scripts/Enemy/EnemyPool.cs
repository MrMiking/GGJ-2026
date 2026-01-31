using System;
using MVsToolkit.Pool;
using UnityEngine;
using UnityEngine.Serialization;

namespace GGJ2026
{
    public class EnemyPool : RegularSingleton<EnemyPool>
    {
        [Header("References")]
        [SerializeField] private PoolObject<BaseEnemy> m_Pool;
        
        public PoolObject<BaseEnemy> Pool => m_Pool;

        protected override void Awake()
        {
            base.Awake();
            m_Pool.Init();
        }
    }
}