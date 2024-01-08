using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Turtle : NormalMonster
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
    public override void OnHit(int damage, Vector2 forceVector)
    {
        if (IsDead)
            return;

        // Hit
        StartIsHitTimer();
        KnockBack(forceVector);
        GetComponent<SoundList>().PlaySFX("SE_Hurt");

        if (CurrentStateIs<Monster_IdleState>() || CurrentStateIs<GroundPatrolState>())
            Animator.SetTrigger("Hurt");
    }
    public override void Die()
    {
        IsDead = true;

        // hitbox collider :  Trigger -> Collision
        SetTriggerHitBox(false);

        // disable monster collider
        GetComponent<Collider2D>().enabled = false;
    }
}
