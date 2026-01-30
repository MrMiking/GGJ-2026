using System;
using UnityEngine;
using MVsToolkit.Dev;

public enum GameState { Playing, Shop, Pause }

public class GameManager : RegularSingleton<GameManager>
{
    [Header("Settings")]
    [SerializeField] private int m_StartGold = 0;
    [SerializeField] private GameState m_StartingGameState = GameState.Playing;
    [Space(10)]
    [SerializeField][ReadOnly] private GameState m_GameState;
    [SerializeField][ReadOnly] private int m_CurrentGold;

    public GameState CurrentState
    {
        get => m_GameState;
        set
        {
            m_GameState = value;
            HandleStateLogic(m_GameState);
            OnStateChanged.Invoke(m_GameState);
        }
    }
    
    public static event Action<GameState> OnStateChanged;

    public int CurrentGold
    {
        get => m_CurrentGold;
        set
        {
            m_CurrentGold = value;
            OnGoldChange.Invoke(m_CurrentGold);
        }
    }
    
    public static event Action<int> OnGoldChange;

    private void Start()
    {
        CurrentGold = m_StartGold;
        CurrentState = m_StartingGameState;
    }

    private void HandleStateLogic(GameState state)
    {
        switch (state)
        {
            case GameState.Playing:
                Time.timeScale = 1;
                break;
            case GameState.Shop:
                Time.timeScale = 0;
                break;
            case GameState.Pause:
                Time.timeScale = 0;
                break;
        }
    }
    
    public void StartGame() => CurrentState = GameState.Playing;
}