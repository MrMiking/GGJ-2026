using GGJ2026;
using MVsToolkit.Utilities;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class Bullet : MonoBehaviour, IPooledObject
{
    [SerializeField] private float bulletLifetime = 5f;
    [SerializeField] private float m_DefaultRadius = 0.2f;
    [SerializeField] private LayerMask m_DamageLayerMask;
    [SerializeField] private LayerMask m_BounceLayerMask;

    private static Collider2D[] s_ColliderBuffer = new Collider2D[32];

    public int PoolKey { get; set; }

    public GameObject GameObject => gameObject;

    private CharacterStats m_CharacterStats;

    private float m_RemainingBulletBounce;
    private float m_RemainingBulletPierce;
    private bool m_Released;

    private Vector2 m_Velocity;
    private float m_Radius;

    private List<Health> m_EnnemyHits = new (8);

    public void Initialize(int poolKey)
    {
        PoolKey = poolKey;
        m_Velocity = Vector2.zero;
        ResetEnnemyHits();
    }

    public void Fire(CharacterStats characterStats)
    {
        m_CharacterStats = characterStats;
        m_RemainingBulletBounce = characterStats.BulletBounce.Value;
        m_RemainingBulletPierce = characterStats.BulletPierce.Value;
        m_Velocity = transform.up * characterStats.BulletSpeed.Value;
        m_Released = false;
        transform.localScale = Vector3.one * m_CharacterStats.BulletSize.Value;
        m_Radius = m_DefaultRadius * m_CharacterStats.BulletSize.Value;

        this.Delay(() => {
            m_Released = true;
            ((IPooledObject)this).Release();
        }, bulletLifetime);
    }

    private void Update()
    {
        if (m_Released)
            return;

        CastDamage();
        CastBounce();

        transform.position += (Vector3) m_Velocity * Time.deltaTime;
    }

    public void ResetEnnemyHits()
    {
        m_EnnemyHits.Clear();
    }

    private void CastDamage()
    {
        var contactFilter = new ContactFilter2D()
        {
            layerMask = m_DamageLayerMask,
            useTriggers = true,
            useLayerMask = true
        };

        int count = Physics2D.OverlapCircle(transform.position, m_Radius, contactFilter, s_ColliderBuffer);
        for (int i = 0; i < count; ++i)
        {
            var collider = s_ColliderBuffer[i];

            if (collider.TryGetComponent(out Health health) && m_EnnemyHits.Contains(health) == false)
            {
                health.Apply(new Damage(DamageType.Physical, m_CharacterStats.BulletDamage.Value));
                m_EnnemyHits.Add(health);

                if (m_RemainingBulletPierce <= 0)
                {
                    StopAllCoroutines();
                    m_Released = true;
                    ((IPooledObject)this).Release();
                    break;
                }
                else
                {
                    m_RemainingBulletPierce--;
                }
            }
        }
    }

    private void CastBounce()
    {
        var hit = Physics2D.CircleCast(transform.position, m_Radius, transform.up, 0.5f, m_BounceLayerMask);
        if (hit.collider)
        {
            Vector2 direction = transform.up.normalized;
            Vector2 normal = hit.normal;
            Vector2 reflectDirection = Vector2.Reflect(direction, normal);

            m_Velocity = reflectDirection * m_CharacterStats.BulletSpeed.Value;
            transform.up = reflectDirection;
            ResetEnnemyHits();

            if (m_RemainingBulletBounce <= 0)
            {
                StopAllCoroutines();
                m_Released = true;
                ((IPooledObject)this).Release();
                return;
            }
            else
            {
                m_RemainingBulletBounce--;
            }
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, m_Radius);
    }
}
