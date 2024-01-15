using UnityEngine;

public class Bear : MonsterBehavior
{
    public float attackCycleTime = 10f;
    public int normalAttackCount = 0;

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

    public override void OnHit(AttackInfo attackInfo)
    {
        //
    }

    public override void Die()
    {
        // Disable Hit Box
        TurnOffHitBox();
    }
}