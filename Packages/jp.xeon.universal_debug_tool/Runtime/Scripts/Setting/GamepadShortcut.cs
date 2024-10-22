using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace Xeon.UniversalDebugTool.Model
{
    [Serializable]
    public class GamepadShortcut
    {
        [SerializeField]
        private bool enabled = true;
        [SerializeField]
        private GamepadButton[] gamepadShortcuts = new GamepadButton[0];

        public bool Judge()
        {
            if (!enabled) return false;
            var gamepad = Gamepad.current;
            if (gamepad == null) return false;

            foreach (var button in gamepadShortcuts)
            {
                if (!gamepad[button].isPressed)
                    return false;
            }

            return true;
        }
    }
}
