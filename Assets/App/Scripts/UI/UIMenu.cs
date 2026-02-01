using System;
using DG.Tweening;
using GGJ2026;
using UnityEngine;

public class UIMenu : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float m_FadeDuration = 0.5f;

    [SerializeField] private float m_YoyoDuration = 0.5f;
    [SerializeField] private float m_YoyoOffset = 1f;
    
    [Header("References")]
    [SerializeField] private RectTransform m_IntroductionPanel;
    [SerializeField] private CanvasGroup m_CanvasGroup;
    [SerializeField] private RectTransform m_DefaultMessageStarting;

    private void Awake()
    {
        if (GameTimeline.Instance.CurrentTimelineEvent != GameTimeline.TimelineEvent.None)
        {
            m_IntroductionPanel.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        UpdateState(GameTimeline.Instance.CurrentTimelineEvent);
    }

    private void OnEnable()
    {
       GameTimeline.Instance.OnTimelineEventChange += UpdateState;
    }

    private void OnDisable()
    {
        GameTimeline.Instance.OnTimelineEventChange -= UpdateState;
        m_IntroductionPanel.DOKill();
        m_CanvasGroup.DOKill();
    }

    private void UpdateState(GameTimeline.TimelineEvent ev)
    {
        switch (ev)
        {
            case GameTimeline.TimelineEvent.None:
                m_IntroductionPanel.gameObject.SetActive(true);
                m_CanvasGroup.DOFade(1f, m_FadeDuration).ChangeStartValue(0).SetLoops(-1, LoopType.Yoyo);
                m_IntroductionPanel.DOLocalMoveY(m_YoyoOffset,m_YoyoDuration).SetLoops(-1, LoopType.Yoyo);
                break;
            case GameTimeline.TimelineEvent.Introduction:
                m_IntroductionPanel.DOKill();
                m_CanvasGroup.DOFade(0f, m_FadeDuration);
                break;
            default:
                m_CanvasGroup.alpha = 0f;
                m_IntroductionPanel.gameObject.SetActive(false);
                break;
        }
    }
}
