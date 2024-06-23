using UnityEngine;

[CreateAssetMenu(fileName = "New Move Straight InputSetter", menuName = "InputSetters/Move Straight")]
public class MoveStraightInputSetter : InputSetterScriptableObject
{
    public enum Direction { Left, Right}

    [SerializeField] private Direction _direction = Direction.Right;
    public Direction direction { get => _direction; }

    public override InputState GetState()
    {
        InputState state = new InputState();
        state.Movement = new Vector2(_direction == Direction.Left ? -1 : 1, 0);
        return state;
    }
}
