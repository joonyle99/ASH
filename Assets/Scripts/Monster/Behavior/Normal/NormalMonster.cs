using UnityEngine;

/// <summary>
/// �Ϲ� ���� Ŭ����
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
