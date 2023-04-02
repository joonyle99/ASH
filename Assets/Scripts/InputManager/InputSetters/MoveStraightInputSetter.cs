using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Move Straight InputSetter", menuName = "InputSetters/Move Straight")]
public class MoveStraightInputSetter : InputSetterScriptableObject
{
    public enum Direction { Left = -1, Right = 1 }
    [SerializeField] Direction _direction = Direction.Right;

    public override InputState GetState()
    {
        InputState state = new InputState();
        state.IsPressingJump = false;
        state.Movement = new Vector2((int)_direction, 0);
        return state;
    }

}
