using System;
using GGJ2026;
using UnityEngine;
using MVsToolkit.Dev;
using UnityEngine.UI;

public class GameManager : RegularSingleton<GameManager>
{
    [Header("Settings")]
    [SerializeField] private int m_StartGold = 0;
    [Space(10)]
    [SerializeField][ReadOnly] private int m_CurrentGold;
    
    [Header("Timer")]
    [SerializeField] private float m_TimerBeforeShop;
    [SerializeField] private Image m_SliderVisual;
    
    private float m_CurrentTime = 0f;

    public int CurrentGold
    {
        get => m_CurrentGold;
        set
        {
            m_CurrentGold = value;
            OnGoldChange?.Invoke(m_CurrentGold);
        }
    }
    
    public event Action<int> OnGoldChange;

    private void Start()
    {
        CurrentGold = m_StartGold;
        m_CurrentTime = m_TimerBeforeShop;
    }

    private void Update()
    {
        if (Time.timeScale == 0) return;
        
        m_CurrentTime -= Time.deltaTime;
        
        m_SliderVisual.fillAmount = m_CurrentTime /  m_TimerBeforeShop;

        if (m_CurrentTime <= 0)
        {
            ShopManager.Instance.OpenShop();
            m_CurrentTime = m_TimerBeforeShop;
        }
    }
}