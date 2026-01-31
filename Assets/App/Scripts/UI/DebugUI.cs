using UnityEngine;
using UnityEngine.InputSystem;

namespace GGJ2026
{
    public sealed class DebugUI : MonoBehaviour
    {
        [SerializeField] private bool m_ShowMenu = false;

        private void Update()
        {
            if (Keyboard.current.f1Key.wasPressedThisFrame)
            {
                m_ShowMenu = !m_ShowMenu;
            }
        }

        private void OnGUI()
        {
            if (m_ShowMenu)
            {
                var playerStats = PlayerController.Instance.GetComponent<CharacterStats>();
                var health = playerStats.GetComponent<Health>();

                GUI.Box(new Rect(10, 10, 300, 600), "DEBUG MENU (F1)");

                GUILayout.BeginArea(new Rect(20, 40, 280, 540));
                GUILayout.Label($"Health: {health.CurrentHealth}");
                GUILayout.Label($"Health Points : {playerStats.HealthPoints.Value}");
                GUILayout.Label($"Movement Speed : {playerStats.MovementSpeed.Value}");
                GUILayout.Label($"Gold Loot Rate : {playerStats.GoldLootRate.Value}");
                GUILayout.Label($"Gold Loot Range : {playerStats.GoldLootRange.Value}");
                GUILayout.Label($"Bullet Damage : {playerStats.BulletDamage.Value}");
                GUILayout.Label($"Fire Rate : {playerStats.FireRate.Value}");
                GUILayout.Label($"Bullet Speed : {playerStats.BulletSpeed.Value}");
                GUILayout.Label($"Bullet Size : {playerStats.BulletSize.Value}");
                GUILayout.Label($"Bullet Bounce : {playerStats.BulletBounce.Value}");
                GUILayout.Label($"Bullet Pierce : {playerStats.BulletPierce.Value}");
                GUILayout.Label($"Bullet Spread : {playerStats.BulletSpread.Value}");


                GUILayout.Space(10);

                if (GUILayout.Button("Give +100 Gold"))
                {
                    GameManager.Instance.CurrentGold += 100;
                }

                if (GUILayout.Button("Heal Full"))
                {
                    health.Apply(new Heal(health.MaxHealth));
                }

                GUILayout.EndArea();
            }
        }
    }
}