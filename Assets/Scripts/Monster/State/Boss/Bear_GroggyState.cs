using UnityEngine;

public class Bear_GroggyState : Monster_StateBase
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        var bear = Monster as Bear;
        if (bear != null)
        {
            bear.isGroggy = true;
            bear.IsGodMode = false; // bear�� GodMode�� Ǯ���鼭 OnHit()�� ȣ��ȴ�.
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
    }
}
