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
