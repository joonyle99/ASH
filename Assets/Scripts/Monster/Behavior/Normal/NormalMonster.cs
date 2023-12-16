using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 일반 몬스터 클래스
/// </summary>
public abstract class NormalMonster : MonsterBehavior
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

    public override void SetUp()
    {
        base.SetUp();
    }

    public override void OnDamage(int damage)
    {
        base.OnDamage(damage);
    }

    public override void KnockBack(Vector2 force)
    {
        base.KnockBack(force);
    }

    public override void Die()
    {
        base.Die();
    }
}
