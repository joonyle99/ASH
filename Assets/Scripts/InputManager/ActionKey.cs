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
    public bool KeyDown { get { return Input.GetKeyDown(_keyCode); } }
    public bool Pressing { get { return Input.GetKey(_keyCode); } }
    public bool KeyUp { get { return Input.GetKeyUp(_keyCode); } }
    public bool None { get { return !Input.GetKey(_keyCode); } }
    public KeyState State
    {
        get
        {
            if (KeyDown)
                return KeyState.KeyDown;
            if (Pressing)
                return KeyState.Pressing;
            if (KeyUp)
                return KeyState.KeyUp;
            return KeyState.None;
        }
    }
}
