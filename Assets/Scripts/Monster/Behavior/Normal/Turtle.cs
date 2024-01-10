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
        base.FixedUpdate();
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
        if (IsDead)
            return;

        // Hit
        StartHitTimer();
        KnockBack(attackInfo.Force);
        GetComponent<SoundList>().PlaySFX("SE_Hurt");

        if (CurrentStateIs<Monster_IdleState>() || CurrentStateIs<GroundPatrolState>())
            Animator.SetTrigger("Hurt");
    }
    public override void Die()
    {
        IsDead = true;

        // Trigger -> Collision
        SetTriggerHitBox(false);

        // disable monster collider
        GetComponent<Collider2D>().enabled = false;
    }
}
