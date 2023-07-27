using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

/// <summary>
/// 종양슬라임 몬스터 클래스
/// </summary>
public class OncologySlime : NormalMonster
{
    public SpriteRenderer renderer;                             // 렌더 정보
    [SerializeField] public List<Transform> wayPoints;          // 목적지 목록
    public Transform currTransform;                             // 목적지
    public Transform nextTransform;                             // 다음 목적지
    public int currentWaypointIndex;                            // 목적지 인덱스
    public float moveSpeed;                                     // 몬스터 이동 속도
    public float upPower;                                       // 튕기는 힘
    public GameObject player;                                   // 플레이어 정보
    [Range(0f, 50f)] public float volumeMul;                    // 볼륨 계수
    [Range(0f, 1000f)] public float power;                      // 넉백 파워
    [Range(0, 100)] public int damage;                          // 데미지

    protected override void Start()
    {
        base.Start();

        // 초기 세팅
        SetUp();

        // 플레이어 게임오브젝트
        // player = SceneContextController.Player.gameObject; -> 에러
        player = GameObject.Find("Player");

        // 초기 목적지
        currTransform = wayPoints[currentWaypointIndex];
        nextTransform = wayPoints[currentWaypointIndex + 1];

        renderer = GetComponentInChildren<SpriteRenderer>();
    }

    protected override void Update()
    {
        base.Update();

        // 목적지를 다음 지점으로 이동
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
        //Debug.Log("slime damage");
        base.OnDamage(_damage);
    }

    public override void KnockBack(Vector2 vec)
    {
        this.Rigidbody.AddForce(vec);
    }

    public override void Die()
    {
        base.Die();

        // 사라지기 시작
        StartCoroutine(FadeOutObject());
    }

    private IEnumerator FadeOutObject()
    {
        // 초기 알파값 저장
        float startAlpha = renderer.color.a;

        // 서서히 알파값 감소
        float t = 0;
        while (t < 3)
        {
            t += Time.deltaTime;
            float normalizedTime = t / 2;
            Color color = renderer.color;
            color.a = Mathf.Lerp(startAlpha, 0f, normalizedTime);
            renderer.color = color;
            yield return null;
        }

        // 오브젝트 삭제
        Destroy(gameObject);
        yield return null;
    }

    /// <summary>
    /// 땅이나 벽과 Collision 충돌
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 땅이나 벽과 충돌했을 때
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Wall") ||
            collision.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            // 거리에 비례해서 볼륨 소리를 키운다
            float finalMul = 1 / Vector3.Distance(player.transform.position, transform.position) * volumeMul;
            if (finalMul > 1f)
                finalMul = 1f;
            GetComponent<SoundList>().PlaySFX("SE_Slime", finalMul);

            // 땅에 닿았을 때 힘을 줘볼까?
            if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                Vector3 moveDirection = (currTransform.position - transform.position).normalized;
                Vector3 force = new Vector3(moveDirection.x * moveSpeed, upPower, 0f);
                Rigidbody.velocity = Vector2.zero;
                Rigidbody.AddForce(force);
            }
        }
        // 플레이어와 충돌했을 때
        else if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log("플레이어와 충돌");

            float dir = Mathf.Sign(collision.transform.position.x - transform.position.x);
            Vector2 vec = new Vector2(power * dir, power);
            collision.gameObject.GetComponent<PlayerBehaviour>().OnHit(damage, vec);
        }
    }
}