using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NormalMonster : BasedMonster
    // �Ϲ� ���� Ŭ����
{
    // Noraml Monster members
    // ...

    public override void SetUp()
    {
        // Ÿ�� �븻
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
