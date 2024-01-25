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
        // 거북이는 Dead 상태에서도 Hit가 가능하다

        // Turtle Hit Process
        HitProcess(attackInfo, false, true);

        // Change to Hurt State
        if (CurrentStateIs<Monster_IdleState>() || CurrentStateIs<GroundMoveState>())
            Animator.SetTrigger("Hurt");

        return IAttackListener.AttackResult.Success;
    }
    public override void Die()
    {
        IsDead = true;

        Animator.SetTrigger("Die");

        // Trigger -> Collision
        TurnToCollisionHitBox();

        // disable monster main collider
        GetComponent<Collider2D>().enabled = false;
    }
}
