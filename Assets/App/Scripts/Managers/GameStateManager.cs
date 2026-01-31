using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public enum GameState { Playing, Shop, Pause }

public class GameStateManager : RegularSingleton<GameStateManager>
{
    [Header("Inputs")]
    [SerializeField] private bool m_HideCursor = false;
    [SerializeField] private InputActionAsset m_InputAsset;

    private readonly Stack<GameState> m_ContextStack = new Stack<GameState>();
    private InputActionMap m_PlayerMap;
    private InputActionMap m_UIMap;
    
    public GameState CurrentState => m_ContextStack.Peek();

    protected override void Awake()
    {
        base.Awake();
        
        m_PlayerMap = m_InputAsset.FindActionMap("Player");
        m_UIMap = m_InputAsset.FindActionMap("UI");
        
        PushContext(GameState.Playing);
    }

    // Set new State
    public void PushContext(GameState context)
    {
        Debug.Log("Push Context " + context);
        m_ContextStack.Push(context);
        ApplyContext(context);
    }

    
    // Pop State
    public void PopContext(GameState context)
    {
        if (m_ContextStack.Count == 0) return;
        if (m_ContextStack.Peek() != context) return;

        Debug.Log("Pop Context " + context);
        m_ContextStack.Pop();
        ApplyContext(m_ContextStack.Peek());
    }

    private void ApplyContext(GameState context)
    {
        switch (context)
        {
            case GameState.Playing:
                EnableGame();
                break;
            default:
                EnableUI();
                break;
        }
    }

    private void EnableGame()
    {
        Time.timeScale = 1;
        
        m_PlayerMap.Enable();
        m_UIMap.Disable();

        foreach (var action in m_PlayerMap.actions)
        {
            action.Disable();
            action.Enable();
        }

        if (m_HideCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void EnableUI()
    {
        Time.timeScale = 0;
        
        m_PlayerMap.Disable();
        m_UIMap.Enable();

        foreach (var action in m_UIMap.actions)
        {
            action.Disable();
            action.Enable();
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}