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
        Type = TYPE.Normal;
    }

    public override void OnDamage(int _damage)
    {
        CurHP -= _damage;

        if (CurHP <= 0)
        {
            CurHP = 0;
            Die();
        }
    }

    public override void KnockBack(Vector2 vec)
    {

    }

    public override void Die()
    {
        Dead = true;
    }
}
