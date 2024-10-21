using System;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class KeyboardShortcut
{
    [SerializeField]
    private bool enabled = true;
    [SerializeField]
    private bool useShift = false;
    [SerializeField]
    private bool useCtrl = false;
    [SerializeField]
    private bool useAlt = false;
    [SerializeField]
    private Key keyCode = Key.None;

    public bool Judge()
    {
        if (!enabled) return false;

        var keyboard = Keyboard.current;
        if (useShift)
        {
            if (!keyboard[Key.LeftShift].isPressed && !keyboard[Key.RightShift].isPressed)
                return false;
        }
        if (useCtrl)
        {
            if (!keyboard[Key.LeftCtrl].isPressed && !keyboard[Key.RightCtrl].isPressed
                && !keyboard[Key.LeftCommand].isPressed && !keyboard[Key.RightCommand].isPressed)
                return false;
        }
        if (useAlt)
        {
            if (!keyboard[Key.RightAlt].isPressed && !keyboard[Key.LeftAlt].isPressed)
                return false;
        }
        return keyboard[keyCode].wasPressedThisFrame;
    }
}
