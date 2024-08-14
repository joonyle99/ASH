using UnityEngine;

[CreateAssetMenu(fileName = "New Move Straight InputSetter", menuName = "InputSetters/Move Straight")]
public class MoveStraightInputSetter : InputSetterScriptableObject
{
    public enum Direction { Left, Right}

    [SerializeField] private Direction _direction = Direction.Right;
    public Direction direction { get => _direction; }

    public override InputState GetState()
    {
        // Move Input Setter인 경우, state.Movement만 설정
        // ** state.[KEY].Update([KEYCODE]); 를 해주지 않음 **
        // i.e) 상호작용 키의 입력이 불가능하다.

        InputState state = base.GetState();
        state.Movement = new Vector2(_direction == Direction.Left ? -1 : 1, 0);
        return state;
    }
}
