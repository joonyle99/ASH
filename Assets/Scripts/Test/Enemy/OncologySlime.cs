using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OncologySlime : NormalMonster
    // 종양슬라임 몬스터 클래스
{
    // slime member
    // ...

    public override void SetUp(string name, int maxHp)
    {
        // 기본 초기화
        base.SetUp(name, maxHp);
        //Debug.Log("slime");

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

    private void Start()
    {
        Debug.Log(MonsterName + "가 생성되었습니다");
    }

    private void Update()
    {
        //Debug.Log(this.gameObject.name + "의 Update가 돌고있다");
    }
}
