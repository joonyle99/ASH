using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class OncologySlime : NormalMonster
    // 종양슬라임 몬스터 클래스
{
    // slime member
    // ...

    public SpriteRenderer renderer;         // 렌더 정보

    [SerializeField]
    public List<Transform> wayPoints;       // 목적지 목록
    public Transform currTransform;         // 목적지
    public Transform nextTransform;         // 다음 목적지
    public int currentWaypointIndex;        // 목적지 인덱스
    public float moveSpeed;                 // 몬스터 이동 속도
    public float time;

    protected override void Start()
    {
        base.Start();

        // 초기 세팅
        SetUp();

        // 초기 목적지
        currTransform = wayPoints[currentWaypointIndex];
        nextTransform = wayPoints[currentWaypointIndex + 1];
    }

    protected override void Update()
    {
        base.Update();

        Debug.Log(Vector3.Distance(currTransform.position,
            transform.position));

        // 몬스터를 다음 지점으로 이동시킵니다.
        if (Vector3.Distance(currTransform.position,
                transform.position) < 2f)
        {
            currentWaypointIndex++;
            currentWaypointIndex %= wayPoints.Count;
            currTransform = wayPoints[currentWaypointIndex];
            nextTransform = wayPoints[(currentWaypointIndex + 1) % wayPoints.Count];
        }
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
        Response = RESPONE.None;

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
    }

    public override void KnockBack(Vector2 vec)
    {
        base.KnockBack(vec);

        this.Rigidbody.AddForce(vec);
    }

    public override void Die()
    {
        base.Die();

        // 충돌 비활성화
        gameObject.GetComponent<CircleCollider2D>().enabled = false;

        // 사라지기 시작
        StartCoroutine(FadeOutObject());
    }

    private IEnumerator FadeOutObject()
    {
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
        Destroy(gameObject);
    }

    /// <summary>
    /// 땅이나 벽과 Collision 충돌
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Wall") || collision.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
            GetComponent<SoundList>().PlaySFX("SE_Slime");

        // 땅에 닿았을 때 힘을 줘볼까?
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Debug.Log("Push");

            Vector3 moveDirection = (currTransform.position - transform.position).normalized;
            Vector3 force = new Vector3(moveDirection.x * moveSpeed, 200f, 0f);
            Rigidbody.AddForce(force);
        }
    }
}