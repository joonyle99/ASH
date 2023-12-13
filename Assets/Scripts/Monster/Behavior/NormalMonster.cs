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
        // 타입 노말
        MonsterType = MONSTER_TYPE.Normal;
    }

    public override void OnDamage(int damage)
    {
        Debug.Log("Normal의 OnDamage()");
        base.OnDamage(damage);
    }

    public override void KnockBack(Vector2 vec)
    {
        base.KnockBack(vec);
    }

    public override void Die()
    {
        Debug.Log("Normal의 Die()");
        base.Die();
    }
}
