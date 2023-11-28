using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 종양슬라임 몬스터 클래스
/// </summary>
public class OncologySlime : NormalMonster
{
    #region Attribute

    private SpriteRenderer _renderer;                             // 렌더 정보
    [SerializeField] private List<Transform> _wayPoints;          // 목적지 목록
    private Transform _currTransform;                             // 목적지
    private Transform _nextTransform;                             // 다음 목적지
    private int _currentWaypointIndex;                            // 목적지 인덱스
    private float _moveSpeed;                                     // 몬스터 이동 속도
    private float _upPower;                                       // 튕기는 힘
    private GameObject _player;                                   // 플레이어 정보
    [Range(0f, 50f)] private float _volumeMul;                    // 볼륨 계수
    [Range(0f, 1000f)] private float _power;                      // 넉백 파워
    [Range(0, 100)] private int _damage;                          // 데미지

    #endregion

    #region Function

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        // 초기 세팅
        SetUp();

        // 플레이어 게임오브젝트
        // _player = SceneContextController.Player.gameObject; -> 에러
        _player = GameObject.Find("Player");

        // 초기 목적지
        _currTransform = _wayPoints[_currentWaypointIndex];
        _nextTransform = _wayPoints[(_currentWaypointIndex + 1) % _wayPoints.Count];

        _renderer = GetComponentInChildren<SpriteRenderer>();
    }

    protected override void Update()
    {
        base.Update();

        // 목적지를 다음 지점으로 이동
        if (Vector3.Distance(_currTransform.position,
                transform.position) < 2f)
        {
            _currentWaypointIndex++;
            _currentWaypointIndex %= _wayPoints.Count;
            _currTransform = _wayPoints[_currentWaypointIndex];
            _nextTransform = _wayPoints[(_currentWaypointIndex + 1) % _wayPoints.Count];
        }
    }

    public override void SetUp()
    {
        // 기본 초기화
        base.SetUp();

        // 종양 슬라임의 최대 체력
        MaxHp = 100;

        // 종양 슬라임의 현재 체력
        CurHp = MaxHp;

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
        //Debug.Log("slime _damage");
        base.OnDamage(_damage);
    }

    public override void KnockBack(Vector2 vec)
    {
        Rigidbody.velocity = vec;
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
        float startAlpha = _renderer.color.a;

        // 서서히 알파값 감소
        float t = 0;
        while (t < 3)
        {
            t += Time.deltaTime;
            float normalizedTime = t / 2;
            Color color = _renderer.color;
            color.a = Mathf.Lerp(startAlpha, 0f, normalizedTime);
            _renderer.color = color;
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
            float finalMul = 1 / Vector3.Distance(_player.transform.position, transform.position) * _volumeMul;
            if (finalMul > 1f)
                finalMul = 1f;
            GetComponent<SoundList>().PlaySFX("SE_Slime", finalMul);

            // 땅에 닿았을 때 힘을 줘볼까?
            if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                Vector3 moveDirection = (_currTransform.position - transform.position).normalized;
                Vector3 force = new Vector3(moveDirection.x * _moveSpeed, _upPower, 0f);
                Rigidbody.velocity = force;
                //Rigidbody.velocity = Vector2.zero;
                //Rigidbody.AddForce(force);
            }
        }
        // 플레이어와 충돌했을 때
        else if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (collision.collider.gameObject.GetComponent<PlayerBehaviour>().CurHp == 0)
                return;

            Debug.Log("플레이어와 충돌");

            float dir = Mathf.Sign(collision.transform.position.x - transform.position.x);
            Vector2 vec = new Vector2(_power * dir, _power);
            // collision.gameObject.GetComponent<PlayerBehaviour>().OnHit(_damage, vec);
        }
    }

    #endregion
}