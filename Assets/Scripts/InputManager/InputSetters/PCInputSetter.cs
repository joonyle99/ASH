using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Default PC InputSetter", menuName = "InputSetters/PCDefault")]
public class PCInputSetter : InputSetterScriptableObject, IInputSetter
{
    public override event IInputSetter.InputEventHandler JumpPressedEvent;
    public override event IInputSetter.InputEventHandler DashPressedEvent;
    public override event IInputSetter.InputEventHandler BasicAttackPressedEvent;
    public override event IInputSetter.InputEventHandler ShootingAttackPressedEvent;

    [SerializeField] KeyCode _jumpKey = KeyCode.Space;
    [SerializeField] KeyCode _dashKey = KeyCode.F;
    [SerializeField] KeyCode _basicAttackKey = KeyCode.A;
    [SerializeField] KeyCode _shootingAttackKey = KeyCode.S;

    public override InputState GetState()
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
