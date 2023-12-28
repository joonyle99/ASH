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

    [SerializeField] private WayPointPatrol _wayPointPatrol;
    [SerializeField] private AttackEvaluator _attackEvaluator;

    [Space]

    [SerializeField] private BatSkillParticle _batSkillPrefab;
    [SerializeField] private Sprite[] _skillSprites;
    [SerializeField] private int _particleCount = 3;
    [SerializeField] private Transform _shootPosition;
    [SerializeField] private float _shootingPower;
    [SerializeField] private float _shootingAngle;
    [SerializeField] private float _shootingVariant;

    #endregion

    #region Function

    protected override void Awake()
    {
        base.Awake();

        _wayPointPatrol = GetComponent<WayPointPatrol>();
        _attackEvaluator = GetComponent<AttackEvaluator>();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        /////////////////////
        // WayPoint Patrol //
        /////////////////////
        if (!_wayPointPatrol.IsWaiting)
        {
            // 목적지에 도착
            if (Vector3.Distance(_wayPointPatrol.CurTargetPosition.position,
                    this.transform.position) < _wayPointPatrol.DistanceWithTarget)
            {
                _wayPointPatrol.IsWaiting = true;
                Rigidbody.velocity = Vector2.zero;

                _wayPointPatrol.ChangeWayPoint();
            }
            else
            {
                // 이동하면서 목적지를 향한 방향을 계속해서 탐지
                _wayPointPatrol.MoveDir = (_wayPointPatrol.CurTargetPosition.position - transform.position).normalized;

                // 목적지로 등속 이동
                Rigidbody.velocity = _wayPointPatrol.MoveDir * MoveSpeed;
            }
        }

        //////////////////////
        // Attack Evaluator //
        //////////////////////
        if (_attackEvaluator.IsAttackable)
        {
            // 탐지 범위 안에 들어왔는지 확인
            Collider2D targetCollider = Physics2D.OverlapBox(transform.position, _attackEvaluator.AttackBoxSize, 0f, _attackEvaluator.TargetLayer);

            if (targetCollider != null)
            {
                Animator.SetTrigger("Attack");
                _attackEvaluator.IsAttackable = false;
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

    #endregion
}
