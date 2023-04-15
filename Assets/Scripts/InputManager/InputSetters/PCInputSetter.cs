using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCInputSetter : MonoBehaviour, IInputSetter
{
    public event IInputSetter.InputEventHandler JumpPressedEvent;
    public event IInputSetter.InputEventHandler DashPressedEvent;
    public event IInputSetter.InputEventHandler BasicAttackPressedEvent;

    [SerializeField] KeyCode _jumpKey = KeyCode.Space;
    [SerializeField] KeyCode _dashKey = KeyCode.LeftShift;
    [SerializeField] KeyCode _basicAttackKey = KeyCode.Alpha1;
    public InputState GetState()
    {
        InputState state = new InputState();
        state.IsPressingJump = Input.GetKey(_jumpKey);
        if (Input.GetKey(KeyCode.LeftArrow))
            state.Movement.x -= 1;
        if (Input.GetKey(KeyCode.RightArrow))
            state.Movement.x += 1;
        if (Input.GetKey(KeyCode.UpArrow))
            state.Movement.y += 1;
        if (Input.GetKey(KeyCode.DownArrow))
            state.Movement.y -= 1;
        return state;
    }
    void Update()
    {
        if (Input.GetKeyDown(_jumpKey))
        {
            JumpPressedEvent?.Invoke();
        }
        if (Input.GetKeyDown(_dashKey))
        {
            DashPressedEvent?.Invoke();
        }
        if (Input.GetKeyDown(_basicAttackKey))
        {
            BasicAttackPressedEvent?.Invoke();
        }
    }

}
