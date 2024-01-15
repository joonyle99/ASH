using UnityEngine;

public class Monster_AttackState : Monster_StateBase
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        if (Monster.MonsterName == MonsterDefine.MonsterName.흑곰.ToString())
        {
            // 흑곰의 다음 공격에 대한 변수를 설정한다.
            var bear = Monster as Bear;
            bear.AttackProcess();
        }

        // 공격이 끊기지 않기 위한 슈퍼아머
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