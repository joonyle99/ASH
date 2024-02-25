using UnityEngine;

public class Monster_HideState : Monster_StateBase, IAttackableState
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        Monster.IsHide = true;

        // ²®Áú ¹ö¼¸
        if (Monster.MonsterName.Contains(MonsterDefine.MonsterName.²®Áú¹ö¼¸.ToString()))
        {
            if (Monster.AttackEvaluator)
            {
                Monster.AttackEvaluator.CanWorking = false;
            }
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        // °ÅºÏÀÌ
        if (Monster.MonsterName.Contains(MonsterDefine.MonsterName.°¡½Ã°ÅºÏ.ToString()))
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

        // ²®Áú ¹ö¼¸
        if (Monster.MonsterName.Contains(MonsterDefine.MonsterName.²®Áú¹ö¼¸.ToString()))
        {
            if (Monster.AttackEvaluator)
            {
                Monster.AttackEvaluator.CanWorking = true;
            }
        }
    }
}
