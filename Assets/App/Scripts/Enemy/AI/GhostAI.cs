using System.Collections;
using UnityEngine;

namespace GGJ2026
{
    [RequireComponent(typeof(Health))]
    public sealed class GhostAI : BaseEnemy
    {
        [Header("Movement")]
        [SerializeField] private float m_MoveSpeed = 5.0f;
        [SerializeField] private float m_Frequency = 0.5f;
        [SerializeField] private float m_OffsetMovement = 1.0f;

        private Vector2 m_RefPosition = Vector2.zero;

        private void Awake()
        {
            m_Health = GetComponent<Health>();
        }

        public override void Initialize(int poolKey)
        {
            base.Initialize(poolKey);
            m_RefPosition = transform.position;
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
        }

        public override void UpdateAutoPilot() { }

        public override void UpdateState()
        {
            var targetPos = EnemyUtils.GetTarget().position;

            var delta = m_MoveSpeed * Time.deltaTime;
            var dir = ((Vector2) targetPos - m_RefPosition).normalized;
            var ortho = new Vector2(-dir.y, dir.x);

            m_RefPosition += dir * delta;
            transform.position = m_RefPosition + Mathf.Sin(Time.time * Mathf.PI * 2 * m_Frequency) * ortho * m_OffsetMovement;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(m_RefPosition, 0.1f);
        }
    }
}