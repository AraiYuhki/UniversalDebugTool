using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Xeon.UniversalDebugTool.Statemachine
{
    public interface IState
    {
        void SetInput(DebugInputSystemActions input);
        void OnEnter();
        void OnExit();
        void Update();
        void InputUpdate();
    }

    public enum DebugMenuState
    {
        Wait,
        Dialog,
        Menu,
        Closed,
    }

    public class DebugMenuStateMachine : IDisposable
    {
        private DebugMenuState currentState = DebugMenuState.Wait;
        private DebugMenuState nextState = DebugMenuState.Wait;
        private Dictionary<DebugMenuState, IState> states = new();
        private DebugDialogManager dialogManager = null;
        private DebugDialog queuedDialog = null;

        public DebugMenuStateMachine(DebugDialogManager dialogManager)
            => this.dialogManager = dialogManager;

        public DebugInputSystemActions Input { get; private set; } = new DebugInputSystemActions();
        public IState Current => states.TryGetValue(currentState, out var instance) ? instance : null;
        public DebugMenuState CurrentState => currentState;

        public void AddState(DebugMenuState state, IState instance)
        {
            states[state] = instance;
            instance.SetInput(Input);
        }

        public void Goto(DebugMenuState state)
        {
            nextState = state;
        }

        public void Update()
        {
            if (nextState == currentState)
            {
                if (currentState is DebugMenuState.Dialog && queuedDialog != null)
                    OpenDialog();
                return;
            }

            Current?.OnExit();
            currentState = nextState;

            if (currentState is DebugMenuState.Dialog)
            {
                OpenDialog();
                return;
            }
            Current?.OnEnter();
        }

        public void Dispose()
        {
            Input?.Dispose();
            Input = null;
        }

        private void OpenDialog()
        {
            dialogManager.OpenDialog(queuedDialog);
            (Current as DialogState).OnEnter(queuedDialog);
            queuedDialog = null;
        }

        public async UniTask<bool> OpenYesNoDialog(string title, string message)
        {
            if (queuedDialog != null)
            {
                return false;
            }
            var waitResult = new UniTaskCompletionSource<bool>();
            DebugDialog dialog = null;
            try
            {
                dialog = dialogManager.CreateDialog(title, message, ("YES", () => waitResult.TrySetResult(true)), ("NO", () => waitResult.TrySetResult(false)));
                queuedDialog = dialog;
                Goto(DebugMenuState.Dialog);
                var result = await waitResult.Task;
                Goto(DebugMenuState.Menu);
                return result;
            }
            catch (Exception _)
            {
                return false;
            }
            finally
            {
                dialogManager.CloseDialog(dialog);
            }
        }

        public async UniTask OpenConfirmDialog(string title, string message)
        {
            if (queuedDialog != null)
            {
                return;
            }
            var waitResult = new UniTaskCompletionSource();
            DebugDialog dialog = null;
            try
            {
                dialog = dialogManager.CreateDialog(title, message, ("OK", () => waitResult.TrySetResult()));
                queuedDialog = dialog;
                Goto(DebugMenuState.Dialog);
                await waitResult.Task;
                Goto(DebugMenuState.Menu);
            }
            finally
            {
                dialogManager.CloseDialog(dialog);
            }
        }
    }
}
