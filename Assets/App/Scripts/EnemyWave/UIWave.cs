using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GGJ2026
{
    public class UIWave : MonoBehaviour
    {
        
        [Header("References")]
        [SerializeField] private GameObject m_Container;
        [SerializeField] private Image m_Slider;
        
        private void OnEnable()
        {
            m_Container.SetActive(false);
            GameTimeline.Instance.OnTimelineEventChange += UpdateState;
        }
        
        private void OnDisable()
        {
            GameTimeline.Instance.OnTimelineEventChange -= UpdateState;
        }

        private void UpdateState(GameTimeline.TimelineEvent obj)
        {
            if (obj == GameTimeline.TimelineEvent.Wave)
            {
                StartCoroutine(WaveAnimation());
            }
            else
            {
                StopAllCoroutines();
                m_Container.SetActive(false);
            }
        }

        private IEnumerator WaveAnimation()
        {
            m_Container.SetActive(true);
            m_Slider.fillAmount = 0f;
            float timer = 0f;
            float duration = GameSequencer.Instance.TimerBeforeShop;

            while (timer < duration)
            {
                timer += Time.deltaTime;
                m_Slider.fillAmount = Mathf.Clamp01(1f - timer / duration);
                yield return null;
            }
        }
    }
}