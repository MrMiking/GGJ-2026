using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public enum UIContext
{
    Game,
    Shop,
    Menu,
}

public class UIContextManager : RegularSingleton<UIContextManager>
{
    [Header("References")]
    [SerializeField] private InputActionAsset m_InputAsset;

    private readonly Stack<UIContext> m_ContextStack = new Stack<UIContext>();
    private InputActionMap m_PlayerMap;
    private InputActionMap m_UIMap;

    protected override void Awake()
    {
        base.Awake();
        
        m_PlayerMap = m_InputAsset.FindActionMap("Player");
        m_UIMap = m_InputAsset.FindActionMap("UI");

        PushContext(UIContext.Game);
    }

    public void PushContext(UIContext context)
    {
        m_ContextStack.Push(context);
        ApplyContext(context);
    }

    public void PopContext(UIContext context)
    {
        if (m_ContextStack.Count == 0) return;
        if (m_ContextStack.Peek() != context) return;

        m_ContextStack.Pop();
        ApplyContext(m_ContextStack.Peek());
    }

    private void ApplyContext(UIContext context)
    {
        switch (context)
        {
            case UIContext.Game:
                EnableGame();
                break;
            default:
                EnableUI();
                break;
        }
    }

    private void EnableGame()
    {
        Debug.Log("Enable Game Context");

        m_PlayerMap.Enable();
        m_UIMap.Disable();

        foreach (var action in m_PlayerMap.actions)
        {
            action.Disable();
            action.Enable();
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void EnableUI()
    {
        Debug.Log("Enable UI Context");

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