using UnityEngine;

/// <summary>
/// None : 아무 입력 없음
/// KeyDown : 눌린 순간 (GetKeyDown)
/// KeyUp : 뗀 순간 (GetKeyUp)
/// Pressing : 누르고 있음 (GetKey)
/// </summary>
public enum KeyState
{
    None, KeyDown, Pressing, KeyUp
}
public struct ActionKey
{
    public KeyState State;
    public bool KeyDown => State == KeyState.KeyDown;
    public bool Pressing => State is KeyState.Pressing or KeyState.KeyDown;
    public bool KeyUp => State == KeyState.KeyUp;
    public bool None => State is KeyState.None or KeyState.KeyUp;

    public void Update(KeyCode code)
    {
        State = KeyState.None;

        if (Input.GetKey(code))
            State = KeyState.Pressing;
        if (Input.GetKeyDown(code))
            State = KeyState.KeyDown;
        if (Input.GetKeyUp(code))
            State = KeyState.KeyUp;
    }
}
