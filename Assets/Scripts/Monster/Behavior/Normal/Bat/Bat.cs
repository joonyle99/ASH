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

    [Space]

    [SerializeField] private Vector2 _attackBoxSize;
    [SerializeField] private bool _isAttack;
    [SerializeField] private float _targetAttackTime = 1.5f;
    [SerializeField] private float _elapsedAttackTime;

    [Space]

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

        for (int i = 0; i < _wayPointBox.childCount; ++i)
            _wayPoints.Add(_wayPointBox.GetChild(i));

        // 초기 목적지 설정
        _curTargetPosition = _wayPoints[_curWayPointIndex];
        _nextTargetPosition = _wayPoints[(_curWayPointIndex + 1) % _wayPoints.Count];
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
    }

    public override void OnDamage(int damage)
    {
        base.OnDamage(damage);
    }

    public override void KnockBack(Vector2 force)
    {
        base.KnockBack(force);
    }

    public override void Die()
    {
        base.Die();
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

        PlaySound_SE_Bat();
    }

    public void PlaySound_SE_Bat()
    {
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
