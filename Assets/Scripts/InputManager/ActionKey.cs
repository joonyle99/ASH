using UnityEngine;

/// <summary>
/// KeyDown : 눌린 순간 (GetKeyDown)
/// KeyUp : 뗀 순간 (GetKeyUp)
/// Pressing : 누르고 있음 (GetKey)
/// None : 아무 입력 없음
/// </summary>
public enum KeyState
{
    KeyDown, Pressing, KeyUp, None
}
public class ActionKey
{
    KeyCode _keyCode;

    public ActionKey(KeyCode keyCode = KeyCode.None)
    {
        _keyCode = keyCode;
    }
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
}
