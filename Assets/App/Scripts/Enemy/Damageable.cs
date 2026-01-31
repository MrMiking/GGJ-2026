using System;
using UnityEngine;

namespace GGJ2026
{
    public class Damageable : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out Health health))
            {
                Debug.Log("Health: " + health);
                health.Apply(new Damage(DamageType.Physical,10));
            }
        }
    }
}