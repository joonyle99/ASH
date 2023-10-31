using UnityEngine;

public enum KeyState
{
    KeyDown, Pressing, KeyUp, None
}
public class ActionKey
{
    public delegate void InputEventHandler();

    KeyCode _keyCode;

    public KeyState State
    {
        get
        {
            if (Input.GetKeyDown(_keyCode))
                return KeyState.KeyDown;
            if (Input.GetKey(_keyCode))
                return KeyState.Pressing;
            if (Input.GetKeyUp(_keyCode))
                return KeyState.KeyUp;
            return KeyState.None;
        }
    }

    public static implicit operator KeyState(ActionKey key)
    {
        return key.State;
    }
}
