using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace GGJ2026
{
    [RequireComponent(typeof(NavMeshAgent), typeof(Health))]
    public sealed class MinibossAI : BaseEnemy
    {
        private enum State
        {
            Chasing,
            PreparingDash,
            Dashing
        }

        [Header("Settings")]
        [SerializeField] private float m_MoveSpeed = 5.0f;
        [SerializeField] private float m_DashRange = 3.0f;
        [SerializeField] private float m_DashDistance = 6.0f;
        [SerializeField] private float m_DashSpeed = 30.0f;
        [SerializeField] private float m_DashCooldown = 3.0f;
        [SerializeField] private float m_DashPreparationTime = 1.5f;
        [SerializeField] private float m_ContactDamage = 20.0f;
        [SerializeField] private float m_ContactKnockbackForce = 40.0f;
        [SerializeField] private float m_DashDamage = 60.0f;
        [SerializeField] private float m_DashKnockbackForce = 30.0f;
        [SerializeField] private float m_Radius = 1.5f;

        [Header("Visual")]
        [SerializeField] private LineRenderer m_DashLineRenderer;

        private State m_State = State.Chasing;
        private NavMeshAgent m_NavMeshAgent;
        private float m_LastPlayerDamageTime = float.NegativeInfinity;
        private float m_LastDashTime = float.NegativeInfinity;

        private void Awake()
        {
            m_NavMeshAgent = GetComponent<NavMeshAgent>();
            m_Health = GetComponent<Health>();
        }

        public override void Initialize(int poolKey)
        {
            base.Initialize(poolKey);
            m_NavMeshAgent.speed = m_MoveSpeed;
            m_NavMeshAgent.updateRotation = false;
            m_NavMeshAgent.autoRepath = false;
            m_DashLineRenderer.enabled = false;
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
            if (m_NavMeshAgent.isOnNavMesh) 
                m_NavMeshAgent.SetDestination(EnemyUtils.GetTarget().position);
        }

        public override void UpdateAutoPilot() { }

        public override void UpdateState()
        {
            switch (m_State)
            {
                case State.Chasing:
                    UpdateChasing();
                    break;
            }

            if (Time.time - m_LastPlayerDamageTime > 0.5f &&
                Vector2.Distance(EnemyUtils.GetTarget().position, transform.position) < m_Radius)
            {
                Attack();
            }
        }

        private void UpdateChasing()
        {
            var targetPos = EnemyUtils.GetTarget().position;
            if (Time.time - m_LastDashTime > m_DashCooldown &&
                Vector2.Distance(targetPos, transform.position) < m_DashRange)
            {
                StartCoroutine(DashCoroutine());
            }
        }

        private IEnumerator DashCoroutine()
        {
            m_NavMeshAgent.enabled = false;
            m_State = State.PreparingDash;
            var enemyPos = (Vector2) EnemyUtils.GetTarget().position;
            var startPos = (Vector2) transform.position;
            var targetPos = (enemyPos - startPos).normalized * m_DashDistance + startPos;

            m_DashLineRenderer.enabled = true;
            m_DashLineRenderer.positionCount = 2;
            m_DashLineRenderer.SetPosition(0, transform.InverseTransformPoint(startPos));
            m_DashLineRenderer.SetPosition(1, transform.InverseTransformPoint(targetPos));

            yield return new WaitForSeconds(m_DashPreparationTime);

            m_State = State.Dashing;
            // Reste last player damage time to enable double attack.
            m_LastPlayerDamageTime = float.NegativeInfinity;


            var dashDuration = m_DashDistance / m_DashSpeed;
            float t = 0;
            while (t < dashDuration)
            {
                t += Time.deltaTime;
                float progress = t / dashDuration;
                transform.position = Vector3.Lerp(startPos, targetPos, progress);

                m_DashLineRenderer.SetPosition(0, transform.InverseTransformPoint(transform.position));
                m_DashLineRenderer.SetPosition(1, transform.InverseTransformPoint(targetPos));

                yield return null;
            }
            m_LastDashTime = Time.time;
            m_DashLineRenderer.enabled = false;

            m_State = State.Chasing;
            m_NavMeshAgent.enabled = true;
            m_NavMeshAgent.Warp(transform.position);
        }

        private void Attack()
        {
            var dmg = m_State == State.Dashing ? m_DashDamage : m_ContactDamage;
            var knockbackForce = m_State == State.Dashing ? m_DashKnockbackForce : m_ContactKnockbackForce;

            var target = EnemyUtils.GetTarget();
            if (target.TryGetComponent(out Health health))
            {
                var damage = new Damage(this, DamageType.Physical, dmg);
                health.Apply(damage);
            }

            if (target.TryGetComponent(out PlayerController playerController))
            {
                var knockbackDirection = ((Vector2)(target.position - transform.position)).normalized;
                playerController.AddForce(knockbackDirection * knockbackForce);
            }

            m_LastPlayerDamageTime = Time.time;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, m_Radius);
        }
    }
}