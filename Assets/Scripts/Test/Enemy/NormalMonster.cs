using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NormalMonster : BasedMonster
    // 일반 몬스터 클래스 (추상 클래스)
{
    // Noraml Monster members
    // ...

    public override void SetUp(string name, int maxHp)
    {
        // 기본 초기화
        base.SetUp(name, maxHp);
        //Debug.Log("normal");

        // 추가 초기화 옵션

    }

    public override void OnDamage(int _damage)
    {
        // 기본 피격
        base.OnDamage(_damage);

        // 추가 피격 옵션

    }

    public override void Die()
    {
        // 기본 사망
        base.Die();

        // 추가 사망 옵션

    }
}
