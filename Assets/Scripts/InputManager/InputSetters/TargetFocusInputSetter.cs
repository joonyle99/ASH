using UnityEngine;

[CreateAssetMenu(fileName = "New Target Focus InputSetter", menuName = "InputSetters/Target Focus")]
public class TargetFocusInputSetter : InputSetterScriptableObject
{
    public Transform targetTrans;
    public Transform thisTrans;

    public override InputState GetState()
    {
        InputState state = base.GetState();

        // 1. Ÿ���� �������� ��� ���⿡ �ִ����� ���

        // 2. Ÿ�����κ��� N ��ŭ ������ ��ġ�� �̵�

        // 3. �ݴ� �������� 1 ��ŭ ������ ��ġ�� �̵� (���� ��ȯ�� ����)

        return state;
    }
}

// InputManager.Instance.ChangeInputSetter(effect.InputSetter);