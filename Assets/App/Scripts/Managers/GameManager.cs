using System;
using UnityEngine;
using MVsToolkit.Dev;

public class GameManager : RegularSingleton<GameManager>
{
    [Header("Settings")]
    [SerializeField] private int m_StartGold = 0;
    [Space(10)]
    [SerializeField][ReadOnly] private int m_CurrentGold;

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
    }
}