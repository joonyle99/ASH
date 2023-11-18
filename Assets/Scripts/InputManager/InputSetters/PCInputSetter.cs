using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCInputSetter : MonoBehaviour, IInputSetter
{
    public event IInputSetter.InputEventHandler JumpPressedEvent;
    public event IInputSetter.InputEventHandler DashPressedEvent;
    public event IInputSetter.InputEventHandler BasicAttackPressedEvent;
    public event IInputSetter.InputEventHandler ShootingAttackPressedEvent;

    [SerializeField] KeyCode _jumpKey = KeyCode.Space;
    [SerializeField] KeyCode _dashKey = KeyCode.F;
    [SerializeField] KeyCode _basicAttackKey = KeyCode.A;
    [SerializeField] KeyCode _shootingAttackKey = KeyCode.S;

    public InputState GetState()
    {
        // "매 프레임" 새로운 Input Data를 생성
        InputState state = new InputState();

        state.IsPressingJump = Input.GetKey(_jumpKey);

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            state.Movement.x -= 1;
            state.Horizontal -= 1;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            state.Movement.x += 1;
            state.Horizontal += 1;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            state.Movement.y += 1;
            state.Vertical += 1;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            state.Movement.y -= 1;
            state.Vertical -= 1;
        }

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
        if (Input.GetKeyDown(_shootingAttackKey))
        {
            ShootingAttackPressedEvent?.Invoke();
        }
    }

}
