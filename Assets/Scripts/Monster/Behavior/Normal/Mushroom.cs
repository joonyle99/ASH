using UnityEngine;

public class Mushroom : MonsterBehavior
{
    // Etc
    protected int countOfUpdate = 0;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        // TODO : temp code
        countOfUpdate++;

        if (countOfUpdate == 1)
            return;

        if (CautionEvaluator)
        {
            if (!CautionEvaluator.IsTargetWithinRange())
            {
                if (CurrentStateIs<Monster_HideState>())
                {
                    //Debug.Log("SetTrigger Idle");
                    Animator.SetTrigger("Idle");
                    return;
                }
            }
            // 타겟이 범위 안에 있는 경우
            else
            {
                if (CurrentStateIs<Monster_IdleState>())
                {
                    //Debug.Log("Update - SetTrigger() Hide " + "/ countOfUpdate : " + countOfUpdate.ToString());
                    //Debug.Log("SetTrigger Hide" + " " + countOfUpdate.ToString());
                    Animator.SetTrigger("Hide");
                    return;
                }
            }
        }
    }

    protected override void SetUp()
    {
        base.SetUp();
    }

    public override void KnockBack(Vector2 forceVector)
    {
        base.KnockBack(forceVector);
    }

    public override void OnHit(AttackInfo attackInfo)
    {
        base.OnHit(attackInfo);
    }

    public override void Die()
    {
        base.Die();
    }

    public void IdleAnimationStart_AnimEvent()
    {
        // TODO : temp code
        //Debug.Log("Fire animation events - Idle Animation Start");
    }

    public void HideAnimationStart_AnimEvent()
    {
        // TODO : temp code
        //Debug.Log("Fire animation events - Hide Animation Start");
    }
}
