using System;
using UnityEngine;
using UnityEngine.AI;

namespace GGJ2026
{
    public sealed class ContactEnemy : BaseEnemy
    {
        [Header("Settings")]
        [SerializeField] private SSO_ContactEnemySettings m_ContactEnemySettings;
        
        [Header("References")]
        [SerializeField] private NavMeshAgent m_NavMeshAgent;
        
        private State m_State = State.Default;
        
        private void Awake()
        {
            m_NavMeshAgent.speed = m_ContactEnemySettings.Speed;
            m_NavMeshAgent.updateRotation = false;
            m_NavMeshAgent.autoRepath = false;
            m_Health.OnDeath += Die;
        }

        private void OnEnable()
        {
            EnemyManager.Instance?.Register(this);
        }
        
        private void OnDisable()
        {
            EnemyManager.Instance?.Unregister(this);
        }

        public override void RefreshPath()
        {
            if (m_State == State.AutoPiloted) return;
            if (m_NavMeshAgent.isOnNavMesh) m_NavMeshAgent.SetDestination(EnemyUtils.GetTarget().position);
        }

        public override void UpdateAutoPilot()
        {
            switch (m_State)
            {
                case State.Default:
                    if (!CameraUtils.IsWorldPositionVisible(transform.position, CameraUtils.K_MarginExit))
                    {
                        m_State = State.AutoPiloted;
                        m_NavMeshAgent.enabled = false;
                        m_NavMeshAgent.Warp(transform.position);
                    }
                    break;
                case State.AutoPiloted:
                    if (CameraUtils.IsWorldPositionVisible(transform.position, CameraUtils.K_KMarginEnter))
                    {
                        m_State = State.Default;
                        m_NavMeshAgent.enabled = true;
                    }
                    break;
            }
        }

        public override void UpdateState()
        {
            switch (m_State)
            {
                case State.Default:
                    break;
                case State.AutoPiloted:
                    transform.position = Vector3.MoveTowards(transform.position, EnemyUtils.GetTarget().position,
                        m_ContactEnemySettings.Speed * Time.deltaTime);
                    break;
            }
        }

        private enum State
        {
            Default,
            AutoPiloted
        }
        
    }
}