using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gizmos = UnityEngine.Gizmos;

/// <summary>
/// 박쥐 몬스터 클래스
/// </summary>
public class Bat : NormalMonster
{
    #region Attribute

    [Header("Bat")]
    [Space]

    [SerializeField] private bool _isTest;

    private List<Transform> _wayPoints;       // 목적지 목록
    private Transform _currTransform;         // 목적지
    private Transform _nextTransform;         // 다음 목적지
    private int _currentWaypointIndex;        // 목적지 인덱스
    private float _waitTime;                  // 기다리는 시간
    private bool _isWaiting;                  // 대기 여부
    private float _time;
    private Vector3 _boxSize;
    private LayerMask _layerMask;
    private bool _isAttack;
    private float _time2;

    private BatSkillParticle _batSkillPrefab;
    private Sprite [] _skillSprites;
    private int _particleCount  = 3;
    private float _shootAngle;
    private float _shootAngleVariant;
    private float _shootPower;
    private Transform _shootPosition;

    private int _damage;
    private float _power;

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

        // 초기 목적지 설정
        // _currTransform = _wayPoints[_currentWaypointIndex];
        // _nextTransform = _wayPoints[(_currentWaypointIndex + 1) % _wayPoints.Count];
    }

    protected override void Update()
    {
        base.Update();

        /*
        // 대기 상태
        if (_isWaiting)
        {
            _time += Time.deltaTime;

            if (_time > _waitTime)
            {
                _time = 0f;
                _isWaiting = false;
            }
        }
        // 이동중
        else
        {
            // 목적지에 도착
            if (Vector3.Distance(_currTransform.position,
                    transform.position) < 1f)
            {
                _isWaiting = true;
                Rigidbody.velocity = Vector2.zero;

                _currentWaypointIndex++;
                _currentWaypointIndex %= _wayPoints.Count;
                _currTransform = _wayPoints[_currentWaypointIndex];
                _nextTransform = _wayPoints[(_currentWaypointIndex + 1) % _wayPoints.Count];
            }
            // 목적지에 도착하지 못함
            else
            {
                // 이동하면서 목적지를 향한 방향을 계속해서 탐지
                Vector2 moveDirection = (_currTransform.position - transform.position).normalized;

                // 목적지로 등속 이동
                Rigidbody.velocity = moveDirection * MoveSpeed;
            }
        }

        // 공격 상태 아니라면
        if (!_isAttack)
        {
            // 탐지 범위 안에 들어왔는지 확인
            Collider2D targetCollider = Physics2D.OverlapBox(transform.position, _boxSize, 0f, _layerMask);
            if (targetCollider != null)
            {
                if (targetCollider.gameObject.tag == "Player")
                {
                    // 공격한다
                    _isAttack = true;

                    Animator.SetTrigger("Shaking");
                }
            }
        }
        else
        {
            _time2 += Time.deltaTime;

            if (_time2 > _waitTime)
            {
                _time2 = 0f;
                _isAttack = false;
            }
        }
        */
    }

    public void AnimEvent_SpawnParticles()
    {
        for (int i = 0; i < _particleCount; i++)
        {
            var particle = Instantiate(_batSkillPrefab, _shootPosition.position, Quaternion.identity);
            particle.SetSprite(_skillSprites[i % (_skillSprites.Length)]);
            float angle = i % 2 == 0 ? _shootAngle : -_shootAngle;
            particle.Shoot(Random.Range(-_shootAngleVariant, _shootAngleVariant) + angle, _shootPower);
        }
    }

    public override void SetUp()
    {
        // 기본 초기화
        base.SetUp();

        // 박쥐의 ID 설정
        ID = 1002;

        // 박쥐의 이름 설정
        MonsterName = "박쥐";

        // 박쥐의 최대 체력
        MaxHp = 100;

        // 박쥐의 현재 체력
        CurHp = MaxHp;

        // 박쥐의 이동속도
        MoveSpeed = 5;

        // 크기
        Size = SIZE.Small;

        // 박쥐의 활동 종류
        ActionType = ACTION_TYPE.Floating;

        // 리젠
        Response = RESPONE.None;

        // 선공
        IsAggressive = IS_AGGRESSIVE.TerritoryAggressive;

        // 추적
        IsChase = IS_CHASE.Territory;

        // 도망
        IsRunaway = IS_RUNAWAY.Aggressive;
    }

    public override void OnDamage(int _damage)
    {
        Debug.Log("bat _damage");
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

    public IEnumerator FadeOutObject()
    {
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();

        // 초기 알파값 저장
        float[] startAlphas = new float[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            startAlphas[i] = renderers[i].color.a;
        }

        // 모든 렌더 컴포넌트를 돌면서 Fade Out
        float t = 0;
        while (t < 3)
        {
            t += Time.deltaTime;
            float normalizedTime = t / 2;

            for (int i = 0; i < renderers.Length; i++)
            {
                Color color = renderers[i].color;
                color.a = Mathf.Lerp(startAlphas[i], 0f, normalizedTime);
                renderers[i].color = color;
            }

            yield return null;
        }

        // 오브젝트 삭제
        Destroy(gameObject);
        yield return null;
    }

    // TODO : 박쥐 몸털기 사운드 Once 재생
    public void PlaySound_SE_Bat()
    {
        GetComponent<SoundList>().PlaySFX("SE_Bat");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 플레이어와 충돌했을 때
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (collision.collider.gameObject.GetComponent<PlayerBehaviour>().CurHp == 0)
                return;

            Debug.Log("플레이어와 충돌");

            float dir = Mathf.Sign(collision.transform.position.x - transform.position.x);
            Vector2 vec = new Vector2(_power * dir, _power);
            // collision.gameObject.GetComponent<PlayerBehaviour>().OnHit(_damage, vec);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, _boxSize);
    }

    #endregion
}
