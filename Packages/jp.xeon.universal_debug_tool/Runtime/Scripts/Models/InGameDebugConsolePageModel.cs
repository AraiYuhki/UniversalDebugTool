#if UNIVERSAL_DEBUG_SUPPORT_IN_GAME_CONSOLE
using IngameDebugConsole;
using System;
using UnityEngine;

[Serializable]
public sealed class InGameDebugConsolePageModel : DebugPageModel, IDisposable
{
    private enum State
    {
        Open,
        Minimized,
        Closed
    }

    private DebugLogManager debugLogManager;
    private State logWindowState = State.Open;
    private DebugEnumPickerModel<State> consoleState;

    public InGameDebugConsolePageModel()
    {
        title = "In game console";
    }

    protected override void Initialize()
    {
        debugLogManager = DebugLogManager.Instance;
        debugLogManager.OnLogWindowShown = (Action)Delegate.Combine(debugLogManager.OnLogWindowShown, new Action(OnLogWindowShown));
        debugLogManager.OnLogWindowHidden = (Action)Delegate.Combine(debugLogManager.OnLogWindowHidden, new Action(OnLogWindowHidden));

        consoleState = new("Display State", GetState(), value => SetState(value));
        AddPicker(consoleState);

        AddButton("Copy all logs to Clipboard", () => GUIUtility.systemCopyBuffer = debugLogManager.GetAllLogs());
    }

    public void Dispose()
    {
        debugLogManager.OnLogWindowShown = (Action)Delegate.Remove(debugLogManager.OnLogWindowShown, new Action(OnLogWindowShown));
        debugLogManager.OnLogWindowHidden = (Action)Delegate.Remove(debugLogManager.OnLogWindowHidden, new Action(OnLogWindowHidden));
    }

    private void OnLogWindowShown()
    {
        logWindowState = State.Open;
        consoleState.Value = logWindowState;
    }

    private void OnLogWindowHidden()
    {
        if (debugLogManager.PopupEnabled)
            logWindowState = State.Minimized;
        else
            logWindowState = State.Closed;
        consoleState.Value = logWindowState;
    }

    private void SetState(State state)
    {
        if (GetState() == state) return;
        switch (state)
        {
            case State.Closed:
                debugLogManager.PopupEnabled = false;
                debugLogManager.HideLogWindow();
                break;
            case State.Minimized:
                debugLogManager.PopupEnabled = true;
                debugLogManager.HideLogWindow();
                break;
            case State.Open:
                debugLogManager.ShowLogWindow();
                break;
            default:
                throw new ArgumentOutOfRangeException("state", state, null);
        }
    }

    private State GetState()
    {
        if (debugLogManager.IsLogWindowVisible)
            return State.Open;
        if (debugLogManager.PopupEnabled)
            return State.Minimized;
        return State.Closed;
    }
}
#endif
