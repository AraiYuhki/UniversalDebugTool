using UnityEngine;
using Xeon.UniversalDebugTool.Model;

namespace Xeon.UniversalDebugTool.Statemachine
{
    public class MenuState : IState
    {
        private DebugMenu menu;
        private KeyboardShortcut keyboardShortcut;
        private GamepadShortcut gamepadShortcut;
        private DebugInputSystemActions input;

        public MenuState(DebugMenu menu, KeyboardShortcut keyboardShortcut, GamepadShortcut gamepadShortcut)
        {
            this.menu = menu;
            this.keyboardShortcut = keyboardShortcut;
            this.gamepadShortcut = gamepadShortcut;
        }

        public void InputUpdate()
        {
            if (keyboardShortcut.Judge() || gamepadShortcut.Judge())
            {
                menu.Hide();
                return;
            }

            if (input.UI.Up.WasPressedThisFrame())
                menu.Up();
            else if (input.UI.Down.WasPressedThisFrame())
                menu.Down();
            if (input.UI.Right.WasPressedThisFrame())
                menu.Right();
            else if (input.UI.Left.WasPressedThisFrame())
                menu.Left();

            if (input.UI.Submit.WasPressedThisFrame())
                menu.Submit();
            else if (input.UI.Cancel.WasPressedThisFrame())
                menu.Cancel();
        }

        public void OnEnter()
        {
            input.Disable();
            input.UI.Enable();
        }

        public void OnExit()
        {
        }

        public void SetInput(DebugInputSystemActions input) => this.input = input;

        public void Update()
        {
        }
    }
}
