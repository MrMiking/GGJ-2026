using System;
using DG.Tweening;
using UnityEngine;

namespace GGJ2026
{
    public class UIDeath : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject m_Container;
        [SerializeField] private CanvasGroup m_CanvasGroup;
        [SerializeField] private float m_FadeDuration = 1f;
    

        private void OnEnable()
        {
            m_Container.SetActive(false);
            GameTimeline.Instance.OnTimelineEventChange += UpdateState;
        }

        private void UpdateState(GameTimeline.TimelineEvent ev)
        {
            if (ev == GameTimeline.TimelineEvent.Defeat)
            {
                m_Container.SetActive(true);
                m_CanvasGroup.DOFade(1f, m_FadeDuration).SetUpdate(true).ChangeStartValue(0);
            }
        }

        public void Restart()
        {
            GameSequencer.Instance.RestartGame();
        }

        private void OnDisable()
        {
            GameTimeline.Instance.OnTimelineEventChange -= UpdateState;
        }
    }
}