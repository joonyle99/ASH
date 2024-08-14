using UnityEngine;

[CreateAssetMenu(fileName = "New Target Focus InputSetter", menuName = "InputSetters/Target Focus")]
public class TargetFocusInputSetter : InputSetterScriptableObject
{
    public Transform targetTrans;
    public Transform thisTrans;

    public override InputState GetState()
    {
        InputState state = base.GetState();

        // 1. 타겟을 기준으로 어느 방향에 있는지를 계산

        // 2. 타겟으로부터 N 만큼 떨어진 위치로 이동

        // 3. 반대 방향으로 1 만큼 떨어진 위치로 이동 (방향 전환을 위함)

        return state;
    }
}

// InputManager.Instance.ChangeInputSetter(effect.InputSetter);