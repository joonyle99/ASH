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

        // 크기
        Size = SIZE.Small;

        // 종양 슬라임의 활동 종류
        ActionType = ACTION_TYPE.Floating;

        // 리젠
        // Response = RESPONE.None;

        // 선공
        IsAggressive = IS_AGGRESSIVE.Peace;

        // 추적
        IsChase = IS_CHASE.AllTerritory;

        // 도망
        IsRunaway = IS_RUNAWAY.Aggressive;
    }

    public override void OnDamage(int _damage)
    {
        base.OnDamage(_damage);

        if (CurHP <= 0)
        {
            CurHP = 0;
            Die();
        }
    }

    public override void KnockBack(Vector2 vec)
    {
        base.KnockBack(vec);

        //this.Rigidbody.AddForce(vec);
        this.Rigidbody.velocity = vec;
        this.Rigidbody.gravityScale = 1f;
    }

    public override void Die()
    {
        base.Die();
        this.gameObject.SetActive(false);
        //StartCoroutine(FadeOutObject());
    }

    private IEnumerator FadeOutObject()
    {
        // 오브젝트의 머티리얼 가져오기
        Renderer renderer = GetComponent<Renderer>();

        // 초기 알파값 저장
        float startAlpha = renderer.material.color.a;

        // 서서히 알파값 감소
        float t = 0;
        while (t < 2)
        {
            t += Time.deltaTime;
            float normalizedTime = t / 2;
            Color color = renderer.material.color;
            color.a = Mathf.Lerp(startAlpha, 0f, normalizedTime);
            renderer.material.color = color;
            yield return null;
        }

        // 오브젝트 비활성화
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 레이어가 Wall이면
        //if(collision.collider.gameObject.layer == LayerMask.NameToLayer("Wall"))
        //{
        //    this.Rigidbody.velocity = Vector2.zero;
        //    this.Rigidbody.gravityScale = 0f;
        //}

        // 플레이어 Attack Box에 피격되면
        // 임시 피격 코드
        //if(collision.gameObject.GetComponent<PlayerBasicAttackHitbox>() != null)
        //{
        //    KnockBack()
        //}


    }

    //public override void OnTriggerEnter2D(Collider2D collision)
    //{
    //    // 피격 collision에서 "PlayerBasicAttackHitbox" 컴포넌트를 찾음
    //    if (collision.GetComponent<PlayerBasicAttackHitbox>() != null)
    //    {
    //        Debug.Log("Hitted by basic attack");
    //    }
    //}
}