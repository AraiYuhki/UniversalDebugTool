using System;
using UnityEngine;

namespace Xeon.UniversalDebugTool.Statemachine
{
    public class DialogState : IState
    {
        private DebugDialog dialog;
        private DebugInputSystemActions input;

        public void SetInput(DebugInputSystemActions input) => this.input = input;

        public void OnEnter(DebugDialog dialog)
        {
            input.Disable();
            this.dialog = dialog;
            input.UI.Enable();
        }

        public void OnEnter()
        {
            throw new NotImplementedException();
        }

        public void OnExit()
        {
        }

        public void Update() { }
        public void InputUpdate()
        {
            Debug.Log(dialog);
            if (input.UI.Up.WasPressedThisFrame()) dialog.Up();
            else if (input.UI.Down.WasPressedThisFrame()) dialog.Down();

            if (input.UI.Left.WasPressedThisFrame()) dialog.Left();
            else if (input.UI.Right.WasPressedThisFrame()) dialog.Right();

            if (input.UI.Submit.WasPressedThisFrame()) dialog.Submit();
            else if (input.UI.Cancel.WasPressedThisFrame()) dialog.Cancel();
        }
    }
}
