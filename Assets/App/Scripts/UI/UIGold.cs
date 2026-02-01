using GGJ2026;
using TMPro;
using UnityEngine;

public class UIGold : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject m_Container;
    [SerializeField] private TextMeshProUGUI m_GoldText;

    private void OnEnable()
    {
        m_Container.SetActive(false);
        GameManager.Instance.OnGoldChange += SetGoldText;
        GameTimeline.Instance.OnTimelineEventChange += UpdateState;
    }

    private void UpdateState(GameTimeline.TimelineEvent ev)
    {
        m_Container.SetActive(ev is GameTimeline.TimelineEvent.Shop or GameTimeline.TimelineEvent.Wave);
    }

    private void OnDisable()
    {
        GameManager.Instance.OnGoldChange -= SetGoldText;
        GameManager.Instance.OnGoldChange -= SetGoldText;
    }

    private void SetGoldText(int amount)
    {
        m_GoldText.text = amount.ToString("F0");
    }
}
