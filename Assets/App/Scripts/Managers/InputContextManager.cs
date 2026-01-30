using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public enum UIContext
{
    Game,
    Shop,
    Menu,
}

public class UIContextManager : PersistantSingleton<UIContextManager>
{
    [Header("References")]
    [SerializeField] private InputActionAsset inputAsset;

    private readonly Stack<UIContext> contextStack = new Stack<UIContext>();
    private InputActionMap playerMap;
    private InputActionMap uiMap;

    protected override void Awake()
    {
        base.Awake();
        
        playerMap = inputAsset.FindActionMap("Player");
        uiMap = inputAsset.FindActionMap("UI");

        PushContext(UIContext.Game);
    }

    public void PushContext(UIContext context)
    {
        contextStack.Push(context);
        ApplyContext(context);
    }

    public void PopContext(UIContext context)
    {
        if (contextStack.Count == 0) return;
        if (contextStack.Peek() != context) return;

        contextStack.Pop();
        ApplyContext(contextStack.Peek());
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

        playerMap.Enable();
        uiMap.Disable();

        foreach (var action in playerMap.actions)
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

        playerMap.Disable();
        uiMap.Enable();

        foreach (var action in uiMap.actions)
        {
            action.Disable();
            action.Enable();
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}