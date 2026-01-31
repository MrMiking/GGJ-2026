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

        private float m_TimerAttack;
        
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
                    if (CanAttack())
                    {
                        Attack();
                    }
                    break;
                case State.AutoPiloted:
                    transform.position = Vector3.MoveTowards(transform.position, EnemyUtils.GetTarget().position,
                        m_ContactEnemySettings.Speed * Time.deltaTime);
                    break;
            }
        }

        private void Attack()
        {
            if (!Physics.SphereCast(transform.position, m_ContactEnemySettings.AttackRange, Vector3.forward,
                    out RaycastHit hitInfo, Mathf.Infinity, LayerMask.GetMask("Player"))) return;
            if (hitInfo.collider.TryGetComponent(out Health health))
            {
                health.Apply(m_ContactEnemySettings.AttackDamage);
            }
        }

        private bool CanAttack()
        {
            m_TimerAttack += Time.deltaTime;
            if (m_TimerAttack >= m_ContactEnemySettings.AttackCooldown && 
                Vector3.Distance(transform.position, EnemyUtils.GetTarget().position) <= m_ContactEnemySettings.AttackRange)
            {
                m_TimerAttack = 0f;
                return true;
            }
            return false;
        }
        
        private enum State
        {
            Default,
            AutoPiloted
        }


        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, m_ContactEnemySettings.AttackRange);
        }
    }
}