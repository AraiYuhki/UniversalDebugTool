using UnityEngine;
using Xeon.UniversalDebugTool.Model;

namespace Xeon.UniversalDebugTool.Statemachine
{
    public class ClosedState : IState
    {
        private DebugMenu menu;
        private KeyboardShortcut keyboardShortcut;
        private GamepadShortcut gamepadShortcut;

        private DebugInputSystemActions input;

        public ClosedState(DebugMenu menu, KeyboardShortcut keyboardShortcut, GamepadShortcut gamepadShortcut)
        {
            this.menu = menu;
            this.keyboardShortcut = keyboardShortcut;
            this.gamepadShortcut = gamepadShortcut;
        }

        public void InputUpdate()
        {
            if (keyboardShortcut.Judge() || gamepadShortcut.Judge())
            {
                menu.Show();
            }
        }

        public void OnEnter()
        {
            input.Disable();
            input.UI.Enable();
        }

        public void OnExit()
        {
            input.Disable();
        }

        public void SetInput(DebugInputSystemActions input) => this.input = input;

        public void Update()
        {
        }
    }
}
