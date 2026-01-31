using UnityEngine;
using GGJ2026;
using MVsToolkit.Utilities;

public class Bullet : MonoBehaviour, IPooledObject
{
    [SerializeField] private float bulletLifetime = 5f;
    [SerializeField] private Rigidbody2D rb;

    public int PoolKey { get; set; }

    public GameObject GameObject { get; }

    private CharacterStats m_CharacterStats;

    private float bulletBounce;

    public void Initialize(int poolKey)
    {
        PoolKey = poolKey;
    }

    public void Release()
    {
        PoolManager.Instance.ReturnToPool(GameObject, PoolKey);
    }

    public void Fire(CharacterStats characterStats)
    {
        bulletBounce = m_CharacterStats.BulletBounce.Value;
        m_CharacterStats = characterStats;
        rb.AddForce(rb.transform.up * m_CharacterStats.BulletSpeed.Value, ForceMode2D.Impulse);

        this.Delay(this.Release, bulletLifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<Health>(out Health health))
        {
            health.Apply(new Damage(DamageType.Physical, m_CharacterStats.BulletDamage.Value));
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (bulletBounce < 1)
        {
            Release();
            return;
        }

        bulletBounce--;
        Vector2 direction = transform.up.normalized;
        Vector2 normal = collision.contacts[0].normal.normalized;
        Vector2 reflectDirection = Vector2.Reflect(direction, normal);
    }
}
