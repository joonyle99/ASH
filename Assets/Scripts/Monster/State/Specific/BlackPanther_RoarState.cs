using UnityEngine;

public class BlackPanther_RoarState : Boss_StateBase
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        var blackPanther = (BlackPanther)Boss;

        // TODO: ��ǥ�� ��ȿ ������ �����Ų��
        Debug.Log("��ǥ�� ���� ����");

        // TODO: ��ǥ�� �� ������ ���� �ȿ� ���ԵǾ�� �ϴµ� �ӽ÷� ���⿡ �ۼ��Ѵ�
        if (!blackPanther.isActiveLuminescence)
            blackPanther.SetActiveLuminescence(true);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
    }
}
