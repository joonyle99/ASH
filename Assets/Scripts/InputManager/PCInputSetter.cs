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
        state.Movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        return state;
    }
    void Update()
    {
        if (Input.GetKeyDown(jumpKey))
        {
            Debug.Log(JumpPressedEvent.GetInvocationList().Length);
            JumpPressedEvent?.Invoke();
        }
    }

}
