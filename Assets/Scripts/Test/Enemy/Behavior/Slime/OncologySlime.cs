using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OncologySlime : NormalMonster
    // 종양슬라임 몬스터 클래스
{
    // slime member
    // ...

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void SetUp(string name, int maxHp, TYPE type, ACTION_TYPE aType)
    {
        // 기본 초기화
        base.SetUp(name, maxHp, type, aType);

        // 추가 초기화 옵션

        // ID 설정
        ID = 1001;

        // 활동 종류
        ActionType = aType;
    }

    public override void OnDamage(int _damage)
    {
        // 기본 피격
        base.OnDamage(_damage);

        // 추가 피격 옵션

    }

    public override void KnockBack(Vector2 vec)
    {
        base.KnockBack(vec);

        //Rigidbody.AddForce(vec);
    }

    public override void Die()
    {
        // 기본 사망
        base.Die();

        // 추가 사망 옵션

    }
}