using System;
using DG.Tweening;
using MVsToolkit.Utilities;
using TMPro;
using UnityEngine;

namespace GGJ2026
{
    public class UIDeath : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject m_Container;
        [SerializeField] private GameObject m_ScoreContainer;
        [SerializeField] private CanvasGroup m_CanvasGroup;
        [SerializeField] private float m_FadeDuration = 1f;
        [SerializeField] private TextMeshProUGUI m_ScoreText;
    

        private void OnEnable()
        {
            m_Container.SetActive(false);
            m_ScoreContainer.SetActive(false);
            GameTimeline.Instance.OnTimelineEventChange += UpdateState;
        }

        private void UpdateState(GameTimeline.TimelineEvent ev)
        {
            if (ev == GameTimeline.TimelineEvent.Defeat)
            {
                m_Container.SetActive(true);
                m_CanvasGroup.DOFade(1f, m_FadeDuration).SetUpdate(true).ChangeStartValue(0);
                this.Delay( () =>
                {
                    m_ScoreContainer.SetActive(true);
                    int score = GameManager.Instance.Score;
                    m_ScoreText.text = score.ToString();
                }, m_FadeDuration, false);
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