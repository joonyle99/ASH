using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCInputSetter : MonoBehaviour, IInputSetter
{
    public event IInputSetter.InputEventHandler JumpPressedEvent;
    public event IInputSetter.InputEventHandler DashPressedEvent;
    public event IInputSetter.InputEventHandler BasicAttackPressedEvent;
    public event IInputSetter.InputEventHandler HealingPressedEvent;
    public event IInputSetter.InputEventHandler ShootingAttackPressedEvent;

    [SerializeField] private KeyCode _jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode _dashKey = KeyCode.F;
    [SerializeField] private KeyCode _basicAttackKey = KeyCode.A;
    [SerializeField] private KeyCode _healingKey = KeyCode.Alpha2;
    [SerializeField] private KeyCode _shootingAttackKey = KeyCode.S;
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
        if (Input.GetKeyDown(_healingKey))
        {
            // 힐링 이벤트
            HealingPressedEvent?.Invoke();
        }
        if (Input.GetKeyDown(_shootingAttackKey))
        {
            // 발사 이벤트
            ShootingAttackPressedEvent?.Invoke();
        }
    }

}
