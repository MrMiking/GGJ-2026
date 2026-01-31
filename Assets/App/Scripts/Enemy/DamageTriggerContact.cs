using UnityEngine;

namespace GGJ2026
{
    public class DamageTriggerContact : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out Health health))
            {
                health.Apply(new Damage(DamageType.Physical,10));
            }
        }
    }
}