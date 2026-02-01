using GGJ2026;
using MoreMountains.Tools;
using UnityEngine;

public class UIHealth : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject m_Container;
    [SerializeField] private MMHealthBar m_HealthBar;
    
   private void OnEnable()
   {
       m_Container.SetActive(false);
       PlayerController.Instance.PlayerHealth.OnHeal += UpdateHealthUI;
       PlayerController.Instance.PlayerHealth.OnDamage += UpdateHealthUI;
       GameTimeline.Instance.OnTimelineEventChange += UpdateState;
   }
   
    private void OnDisable()
    {
         PlayerController.Instance.PlayerHealth.OnHeal -= UpdateHealthUI;
         PlayerController.Instance.PlayerHealth.OnDamage -= UpdateHealthUI;
         GameTimeline.Instance.OnTimelineEventChange -= UpdateState;
    }

    private void UpdateState(GameTimeline.TimelineEvent timelineEvent)
    {
        if (timelineEvent == GameTimeline.TimelineEvent.Wave)
        {
            m_Container.SetActive(true);
            UpdateHealthUI();
        }
        else
        {
            m_Container.SetActive(false);
        }
    }
    
   private void UpdateHealthUI(float previousHealth, float newHealth, in Heal heal) => UpdateHealthUI();

   private void UpdateHealthUI(float previousHealth, float newHealth, in Damage damage) => UpdateHealthUI();

   private void UpdateHealthUI() => m_HealthBar.UpdateBar(PlayerController.Instance.PlayerHealth.CurrentHealth, 0.0f,
       PlayerController.Instance.PlayerHealth.MaxHealth, true);
}
