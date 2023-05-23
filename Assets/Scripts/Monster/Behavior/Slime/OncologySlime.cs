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

    public override void SetUp()
    {
        // 기본 초기화
        base.SetUp();

        // 종양 슬라임의 최대 체력
        MaxHp = 100;

        // 종양 슬라임의 현재 체력
        CurHP = MaxHp;

        // 종양 슬라임의 ID 설정
        ID = 1001;

        // 종양 슬라임의 이름 설정
        MonsterName = "종양 슬라임";

        // 종양 슬라임의 활동 종류
        ActionType = ACTION_TYPE.Ground;
    }

    public override void OnDamage(int _damage)
    {
        base.OnDamage(_damage);
    }

    public override void KnockBack(Vector2 vec)
    {
        base.KnockBack(vec);

        this.Rigidbody.AddForce(vec);
        Debug.Log("종양슬라임 AddForce");
    }

    public override void Die()
    {
        base.Die();

    }
}