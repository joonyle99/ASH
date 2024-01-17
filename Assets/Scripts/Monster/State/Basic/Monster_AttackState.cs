using UnityEngine;

public class Monster_AttackState : Monster_StateBase
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        if (Monster.MonsterName == MonsterDefine.MonsterName.���.ToString())
        {
            // ����� ���� ���ݿ� ���� ������ �����Ѵ�.
            var bear = Monster as Bear;
            bear.AttackProcess();
        }

        // ������ ������ �ʱ� ���� ���۾Ƹ�
        Monster.IsSuperArmor = true;

        Monster.StartSuperArmorFlash();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);

        Monster.IsSuperArmor = false;
    }
}