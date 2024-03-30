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
    protected override void FixedUpdate()
    {
        if (IsDead)
            return;

        if(CurrentStateIs<GroundMoveState>())
            monsterMovement.GroundWalking();
    }
    public override void SetUp()
    {
        base.SetUp();
    }

    public override void KnockBack(Vector2 forceVector)
    {
        base.KnockBack(forceVector);
    }
    public override IAttackListener.AttackResult OnHit(AttackInfo attackInfo)
    {
        // �ź��̴� Dead ���¿����� Hit�� �����ϴ�

        // Turtle Hit Process
        if (IsHiding || IsDead)
            HitProcess(attackInfo, false, true, false);
        else
            HitProcess(attackInfo, false, true, true);

        // Change to Hurt State
        if (CurrentState is IHurtableState)
            Animator.SetTrigger("Hurt");

        return IAttackListener.AttackResult.Success;
    }
    public override void Die()
    {
        IsDead = true;

        Animator.SetTrigger("Die");

        // Trigger -> Collision
        GroundizeHitBox();

        // disable monster main collider
        GetComponent<Collider2D>().enabled = false;
    }
}
