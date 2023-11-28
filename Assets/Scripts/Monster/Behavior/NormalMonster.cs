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

        SetUp();
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

        // Ÿ�� �븻
        MonsterType = MONSTER_TYPE.Normal;
    }

    public override void OnDamage(int damage)
    {
        base.OnDamage(damage);
    }

    public override void KnockBack(Vector2 vec)
    {
        base.KnockBack(vec);
    }

    public override void Die()
    {
        base.Die();
    }
}
