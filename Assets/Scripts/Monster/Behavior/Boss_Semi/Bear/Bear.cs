using UnityEngine;

public class Bear : SemiBossMonster
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

    public override void OnHitted(AttackInfo attackInfo)
    {
        base.OnHitted(attackInfo);
    }

    public override void Die()
    {
        base.Die();
    }
}