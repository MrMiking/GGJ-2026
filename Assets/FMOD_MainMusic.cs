using System;
using FMOD.Studio;
using FMODUnity;
using MVsToolkit.Utilities;
using UnityEngine;

namespace GGJ2026
{
    public class FMOD_MainMusic : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private StudioEventEmitter m_Emitter;

        [Header("Settings")]
        [SerializeField] private string m_ParameterIDStart;
        [SerializeField] private string m_ParameterIDShop;
        [SerializeField] private string m_ParameterIDDefeat;
        [SerializeField] private string m_ParameterIDLife;
        [Space]
        [SerializeField] private float m_DurationStinger;

        private void OnEnable()
        {
            GameTimeline.Instance.OnTimelineEventChange += UpdateState;
            PlayerController.Instance.PlayerHealth.OnHeal += UpdateSoundHealth;
            PlayerController.Instance.PlayerHealth.OnDamage += UpdateSoundHealth;
        }

        private void OnDisable()
        {
            GameTimeline.Instance.OnTimelineEventChange -= UpdateState;
            PlayerController.Instance.PlayerHealth.OnHeal += UpdateSoundHealth;
            PlayerController.Instance.PlayerHealth.OnDamage += UpdateSoundHealth;
        }

        private void UpdateState(GameTimeline.TimelineEvent ev)
        {
            if (ev == GameTimeline.TimelineEvent.Introduction)
            {
                m_Emitter.SetParameter(m_ParameterIDStart,1f);
                this.Delay(()=>m_Emitter.SetParameter(m_ParameterIDStart,0f),m_DurationStinger);
            }

            if (ev == GameTimeline.TimelineEvent.Shop)
            {
                m_Emitter.SetParameter(m_ParameterIDShop,1f);
                this.Delay(()=> m_Emitter.SetParameter(m_ParameterIDShop,0f),m_DurationStinger);
            }

            if (ev == GameTimeline.TimelineEvent.Defeat)
            {
                m_Emitter.SetParameter(m_ParameterIDDefeat,1f);
                this.Delay(()=> m_Emitter.SetParameter(m_ParameterIDDefeat,0f),m_DurationStinger);
            }
        }

        private void UpdateSoundHealth(float previousHealth, float newHealth, in Damage damage) => UpdateSoundHealth();
        private void UpdateSoundHealth(float previousHealth, float newHealth, in Heal heal) => UpdateSoundHealth();

        private void UpdateSoundHealth()
        {
            if (GameTimeline.Instance.CurrentTimelineEvent != GameTimeline.TimelineEvent.Wave) return;
            float healthRatio = PlayerController.Instance.PlayerHealth.CurrentHealth / PlayerController.Instance.PlayerHealth.MaxHealth;
            m_Emitter.SetParameter(m_ParameterIDLife, healthRatio);
        }
    }
}