using System.Threading;
using UnityEngine;

public class Turtle : MonsterBehavior
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

    }
    protected override void SetUp()
    {
        base.SetUp();
    }

    public override void KnockBack(Vector2 forceVector)
    {
        base.KnockBack(forceVector);
    }
    public override IAttackListener.AttackResult OnHit(AttackInfo attackInfo)
    {
        if (IsDead)
            return IAttackListener.AttackResult.Fail;

        // Hit Process
        StartHitTimer();
        KnockBack(attackInfo.Force);
        GetComponent<SoundList>().PlaySFX("SE_Hurt");

        // Change to Hurt State
        if (CurrentStateIs<Monster_IdleState>() || CurrentStateIs<GroundPatrolState>())
            Animator.SetTrigger("Hurt");
        return IAttackListener.AttackResult.Success;
    }
    public override void Die()
    {
        // Trigger -> Collision
        TurnToCollisionHitBox();

        // disable monster collider
        GetComponent<Collider2D>().enabled = false;
    }
}
