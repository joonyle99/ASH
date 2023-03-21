using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct InputState
{
    public bool IsPressingJump;
    public Vector2 Movement;
}
public class InputManager : HappyTools.SingletonBehaviourFixed<InputManager>, IInputSetter
{
    List<IInputSetter.InputEventHandler> JumpPressedEventHandlers = new List<IInputSetter.InputEventHandler>();
    public event IInputSetter.InputEventHandler JumpPressedEvent
    {
        add
        {
            JumpPressedEventHandlers.Add(value);
            
        }
        remove
        {
            JumpPressedEventHandlers.Remove(value);
        }
    }
    public delegate void InputEventHandler();
    IInputSetter _currentSetter;

    public void ChangeInputSetter(IInputSetter setter)
    {
        Debug.Log(JumpPressedEventHandlers.Count);

        foreach(var handler in JumpPressedEventHandlers)
            _currentSetter.JumpPressedEvent -= handler;
        _currentSetter = setter;

        foreach (var handler in JumpPressedEventHandlers)
            _currentSetter.JumpPressedEvent += handler;
    }

    

    public InputState GetState()
    {
        return _currentSetter.GetState();
    }
}
