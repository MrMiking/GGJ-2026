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
    
    private int m_Level;
    private int m_EnemiesDefeated;
    
    
    public int CurrentGold
    {
        get => m_CurrentGold;
        set
        {
            m_CurrentGold = value;
            OnGoldChange?.Invoke(m_CurrentGold);
        }
    }

    public int Score => m_EnemiesDefeated * m_Level;

    public int Level
    {
        get => m_Level;
        set => m_Level = value;
    }
    
    public int EnemiesDefeated
    {
        get => m_EnemiesDefeated;
        set => m_EnemiesDefeated = value;
    }
    
    public event Action<int> OnGoldChange;

    private void Start()
    {
        CurrentGold = m_StartGold;
    }
}