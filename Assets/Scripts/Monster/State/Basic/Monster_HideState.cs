using UnityEngine;

public class Monster_HideState : Monster_StateBase, IAttackableState
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        Monster.IsHide = true;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        // �ź����� ��쿡��
        if (Monster.MonsterName.Contains(MonsterDefine.MonsterName.���ðź�.ToString()))
        {
            // reset stay time
            if (Monster.IsHit)
                _elapsedStayTime = 0f;
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);

        Monster.IsHide = false;
    }
}
