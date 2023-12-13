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

    [SerializeField] private Transform _wayPointBox;
    [SerializeField] private List<Transform> _wayPoints;
    [SerializeField] private Transform _curTargetPosition;
    [SerializeField] private Transform _nextTargetPosition;
    [SerializeField] private int _curWayPointIndex = 0;
    [SerializeField] private float _distanceWithTarget = 1f;
    [SerializeField] private Vector3 _moveDir;
    [SerializeField] private float _targetWaitTime = 2f;
    [SerializeField] private float _elapsedWaitTime;
    [SerializeField] private bool _isWaiting;
    [SerializeField] private Vector2 _attackBoxSize;
    [SerializeField] private bool _isAttack;
    [SerializeField] private float _targetAttackTime = 1.5f;
    [SerializeField] private float _elapsedAttackTime;
    [SerializeField] private float _elapsedFadeOutTime;
    [SerializeField] private float _targetFadeOutTime = 3f;

    [SerializeField] private BatSkillParticle _batSkillPrefab;
    [SerializeField] private Sprite[] _skillSprites;
    [SerializeField] private int _particleCount = 3;
    [SerializeField] private Transform _shootPosition;
    [SerializeField] private float _shootingPower;
    [SerializeField] private float _shootingAngle;
    [SerializeField] private float _shootingVariant;

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

        for (int i = 0; i < _wayPointBox.childCount; ++i)
            _wayPoints.Add(_wayPointBox.GetChild(i));

        // 초기 목적지 설정
        _curTargetPosition = _wayPoints[_curWayPointIndex];
        _nextTargetPosition = _wayPoints[(_curWayPointIndex + 1) % _wayPoints.Count];

        Debug.Log("OnDamage");
        OnDamage(2000);
    }

    protected override void Update()
    {
        base.Update();

        // 대기 상태
        if (_isWaiting)
        {
            _elapsedWaitTime += Time.deltaTime;

            if (_elapsedWaitTime > _targetWaitTime)
            {
                _elapsedWaitTime = 0f;
                _isWaiting = false;
            }
        }
        // 이동중
        else
        {
            // 목적지에 도착
            if (Vector3.Distance(_curTargetPosition.position,
                    this.transform.position) < _distanceWithTarget)
            {
                _isWaiting = true;

                Rigidbody.velocity = Vector2.zero;

                _curWayPointIndex++;
                _curTargetPosition = _nextTargetPosition;
                _nextTargetPosition = _wayPoints[(_curWayPointIndex + 1) % _wayPoints.Count];
            }
            // 목적지에 도착하지 못함
            else
            {
                // 이동하면서 목적지를 향한 방향을 계속해서 탐지
                _moveDir = (_curTargetPosition.position - transform.position).normalized;

                // 목적지로 등속 이동
                Rigidbody.velocity = _moveDir * MoveSpeed;
            }
        }

        // 공격 상태 아니라면
        if (!_isAttack)
        {
            // 탐지 범위 안에 들어왔는지 확인
            Collider2D targetCollider = Physics2D.OverlapBox(transform.position, _attackBoxSize, 0f, LayerMask.GetMask("Player"));
            if (targetCollider != null)
            {
                // Debug.Log("탐지 범위에 플레이어가 들어왔으니 공격한다.");
                // Debug.Log(targetCollider.gameObject.name);

                // 공격한다
                _isAttack = true;

                Animator.SetTrigger("Attack");

                // SprinkleParticle();
                // PlaySound_SE_Bat();
            }
        }
        // 공격 쿨타임 계산
        else
        {
            _elapsedAttackTime += Time.deltaTime;

            if (_elapsedAttackTime > _targetAttackTime)
            {
                // Debug.Log("다시 공격이 가능해진다");

                _elapsedAttackTime = 0f;
                _isAttack = false;
            }
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
        MonsterSize = MONSTER_SIZE.Small;

        // 박쥐의 활동 종류
        ActionType = ACTION_TYPE.Floating;

        // 리젠
        ResponseType = RESPONE_TYPE.None;

        // 선공
        AggressiveType = AGGRESSIVE_TYPE.TerritoryAggressive;

        // 추적
        ChaseType = CHASE_TYPE.Territory;

        // 도망
        RunawayType = RUNAWAY_TYPE.Aggressive;
    }

    public override void OnDamage(int damage)
    {
        Debug.Log("Bat의 OnDamage()");
        base.OnDamage(damage);
    }

    public override void KnockBack(Vector2 vec)
    {
        base.KnockBack(vec);

        Rigidbody.velocity = vec;
    }

    public override void Die()
    {
        Debug.Log("Bat의 Die()");
        base.Die();

        // 사라지기 시작
        StartCoroutine(FadeOutObject());
    }

    public void SprinkleParticle()
    {
        for (int i = 0; i < _particleCount; i++)
        {
            var particle = Instantiate(_batSkillPrefab, _shootPosition.position, Quaternion.identity);
            particle.SetSprite(_skillSprites[i % (_skillSprites.Length)]);
            float angle = i % 2 == 0 ? _shootingAngle : -_shootingAngle;
            particle.Shoot(Random.Range(-_shootingVariant, _shootingVariant) + angle, _shootingPower);
        }

        // TODO : 소리가 가끔 씹히는데 사운드가 길어서 그런가?
        PlaySound_SE_Bat();
    }

    public IEnumerator FadeOutObject()
    {
        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        // 초기 알파값 저장
        float[] startAlphaArray = new float[spriteRenderers.Length];
        for (int i = 0; i < spriteRenderers.Length; i++)
            startAlphaArray[i] = spriteRenderers[i].color.a;

        // 모든 렌더 컴포넌트를 돌면서 Fade Out
        while (_elapsedFadeOutTime < _targetFadeOutTime)
        {
            _elapsedFadeOutTime += Time.deltaTime;
            float normalizedTime = _elapsedFadeOutTime / 2;

            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                // 현재 스프라이트 렌더러의 알파값을 변경
                Color targetColor = spriteRenderers[i].color;
                targetColor.a = Mathf.Lerp(startAlphaArray[i], 0f, normalizedTime);
                spriteRenderers[i].color = targetColor;
            }

            yield return null;
        }

        // 오브젝트 삭제
        Destroy(gameObject);

        yield return null;
    }

    public void PlaySound_SE_Bat()
    {
        Debug.Log("박쥐 공격 사운드 재생");

        GetComponent<SoundList>().PlaySFX("SE_Bat");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        /*
        // 플레이어와 충돌했을 때
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (collision.collider.gameObject.GetComponent<PlayerBehaviour>().CurHp == 0)
                return;

            Debug.Log("플레이어와 충돌");

            float dir = Mathf.Sign(collision.transform.position.x - transform.position.x);
            Vector2 vec = new Vector2(_power * dir, _power);
            // collision.gameObject.GetComponent<PlayerBehaviour>().OnHit(damage, vec);
        }
        */
    }

    private void OnDrawGizmosSelected()
    {
        // 공격 범위
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(this.transform.position, _attackBoxSize);

        // 이동 방향
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(this.transform.position, this.transform.position + _moveDir * 2f);
    }

    #endregion
}
