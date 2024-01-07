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
    public bool KeyDown { get { return State == KeyState.KeyDown; } }
    public bool Pressing { get { return State == KeyState.Pressing || State == KeyState.KeyDown; } }
    public bool KeyUp { get { return State == KeyState.KeyUp; } }
    public bool None { get { return State == KeyState.None || State == KeyState.KeyUp; } }

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
