using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace GGJ2026
{
    [RequireComponent(typeof(NavMeshAgent), typeof(Health), typeof(ContactDamage))]
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
        private ContactDamage m_Contact;
        private float m_LastDashTime = float.NegativeInfinity;

        private void Awake()
        {
            m_NavMeshAgent = GetComponent<NavMeshAgent>();
            m_Health = GetComponent<Health>();
            m_Contact = GetComponent<ContactDamage>();
        }

        public override void Initialize(int poolKey)
        {
            base.Initialize(poolKey);
            m_NavMeshAgent.speed = m_MoveSpeed;
            m_NavMeshAgent.updateRotation = false;
            m_NavMeshAgent.autoRepath = false;
            m_DashLineRenderer.enabled = false;
            m_State = State.Chasing;
            m_Contact.damage = m_ContactDamage;
            m_Contact.knockbackForce = m_ContactKnockbackForce;
            m_Contact.radius = m_Radius;
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
            m_Contact.SkipCooldown();
            m_Contact.damage = m_DashDamage;
            m_Contact.knockbackForce = m_DashKnockbackForce;

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
            m_Contact.damage = m_ContactDamage;
            m_Contact.knockbackForce = m_DashKnockbackForce;
            m_NavMeshAgent.enabled = true;
            m_NavMeshAgent.Warp(transform.position);
        }
    }
}