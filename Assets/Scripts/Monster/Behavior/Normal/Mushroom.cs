using UnityEngine;

public class Mushroom : MonsterBehavior
{
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

        if (CautionEvaluator)
        {
            if (!CautionEvaluator.IsTargetWithinCautionRange())
            {
                if (CurrentStateIs<Monster_HideState>())
                {
                    Debug.Log("SetTrigger Idle");

                    Animator.SetTrigger("Idle");
                    return;
                }
            }
            else
            {
                if (CurrentStateIs<Monster_IdleState>())
                {
                    Debug.Log("SetTrigger Hide");

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
}
