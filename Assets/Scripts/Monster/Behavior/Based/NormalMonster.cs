using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NormalMonster : BasedMonster
    // 일반 몬스터 클래스
{
    // Noraml Monster members
    // ...

    public override void SetUp()
    {
        // 타입 노말
        Type = TYPE.Normal;
    }

    public override void OnDamage(int _damage)
    {
        //Debug.Log("normal ondamage");
        base.OnDamage(_damage);
    }

    public override void KnockBack(Vector2 vec)
    {

    }

    public override void Die()
    {
        base.Die();
    }
}
