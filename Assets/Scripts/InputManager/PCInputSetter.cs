using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCInputSetter : MonoBehaviour, IInputSetter
{
    public event IInputSetter.InputEventHandler JumpPressedEvent;

    KeyCode jumpKey = KeyCode.Space;
    public InputState GetState()
    {
        InputState state = new InputState();
        state.IsPressingJump = Input.GetKey(jumpKey);
        if (Input.GetKey(KeyCode.LeftArrow))
            state.Movement.x -= 1;
        if (Input.GetKey(KeyCode.RightArrow))
            state.Movement.x += 1;
        return state;
    }
    void Update()
    {
        if (Input.GetKeyDown(jumpKey))
        {
            JumpPressedEvent?.Invoke();
        }
    }

}
