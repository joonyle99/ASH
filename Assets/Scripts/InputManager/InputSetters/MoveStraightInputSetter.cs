using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Move Straight InputSetter", menuName = "InputSetters/Move Straight")]
public class MoveStraightInputSetter : InputSetterScriptableObject
{
    public enum Direction { Left, Right}
    [SerializeField] Direction _direction = Direction.Right;

    public override InputState GetState()
    {
        InputState state = new InputState();
        state.IsPressingJump = false;
        state.Movement = new Vector2(_direction == Direction.Left ? -1 : 1, 0);
        return state;
    }

}
