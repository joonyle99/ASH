using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Gizmos = UnityEngine.Gizmos;

public class PlayerAttackController : MonoBehaviour
{
    [Header("Attack Setting")]
    [Space]

    [SerializeField] private LayerMask _monsterLayerMask;
    [SerializeField] private LayerMask _attackableEntityLayerMask;
    [SerializeField] private Transform _attackHitBoxTrans;
    [SerializeField] private float _hitBoxRadius;

    [Space]

    [SerializeField] private int _attackDamage = 20;
    [SerializeField] private float _attackPowerX = 7f;
    [SerializeField] private float _attackPowerY = 10f;

    [Space]

    [SerializeField] private float _targetAttackTime = 1.5f;
    [SerializeField] private float _elapsedAttackTime;
    [SerializeField] private int _basicAttackCount;
    [SerializeField] private bool _isBasicAttacking;
    public bool IsBasicAttacking
    {
        get { return _isBasicAttacking; }
        set { _isBasicAttacking = value; }
    }

    private PlayerBehaviour _player;

    private void Awake()
    {
        _player = GetComponent<PlayerBehaviour>();
    }

    public void CastBasicAttack()
    {
        // you can attack when attack animation is done
        if (!IsBasicAttacking)
        {
            IsBasicAttacking = true;
            _elapsedAttackTime = 0f;
            _basicAttackCount++;

            _player.Animator.SetTrigger("Attack");
            _player.Animator.SetInteger("BasicAttackCount", _basicAttackCount);

            _player.PlaySound_SE_Attack();

            if (_basicAttackCount >= 3)
                _basicAttackCount = 0;

            MonsterAttackProcess();
            AttackableEntityProcess();
        }
    }

    public void MonsterAttackProcess()
    {
        _hitBoxRadius = _attackHitBoxTrans.GetComponent<CircleCollider2D>().radius;
        RaycastHit2D[] rayCastHits = Physics2D.CircleCastAll(_attackHitBoxTrans.position, _hitBoxRadius, Vector2.zero,
            0f, _monsterLayerMask);

        foreach (var rayCastHit in rayCastHits)
        {
            // check monster attack invalid
            MonsterBodyHit monsterBodyHit = rayCastHit.collider.GetComponent<MonsterBodyHit>();
            if (monsterBodyHit)
            {
                MonsterBehavior monsterBehavior = rayCastHit.collider.GetComponentInParent<MonsterBehavior>();
                if (monsterBehavior)
                {
                    Transform monsterTrans = rayCastHit.collider.transform;

                    // set forceVector
                    float dir = Mathf.Sign(monsterTrans.position.x - transform.position.x);
                    Vector2 forceVector = new Vector2(_attackPowerX * dir, _attackPowerY);

                    // message to monsterBehavior
                    monsterBehavior.OnHit(_attackDamage, forceVector);
                }
            }
        }
    }

    public void AttackableEntityProcess()
    {
        // TODO : attackableEntity의 레이어만 골라서 한다
        _hitBoxRadius = _attackHitBoxTrans.GetComponent<CircleCollider2D>().radius;
        RaycastHit2D[] rayCastHits = Physics2D.CircleCastAll(_attackHitBoxTrans.position, _hitBoxRadius, Vector2.zero,
            0f, _attackableEntityLayerMask);

        foreach (var rayCastHit in rayCastHits)
        {
            // check attackable entity invalid
            AttackableEntity attackableEntity = rayCastHit.collider.GetComponent<AttackableEntity>();
            if (attackableEntity)
                attackableEntity.OnHittedByBasicAttack();
        }
    }

    public void CastShootingAttack()
    {
        // _player.ChangeState<ShootingState>();
    }

    private void Update()
    {
        // reset attackCount
        if (_basicAttackCount > 0)
        {
            _elapsedAttackTime += Time.deltaTime;

            if (_elapsedAttackTime > _targetAttackTime)
            {
                _elapsedAttackTime = 0f;
                _basicAttackCount = 0;
                _player.Animator.SetInteger("BasicAttackCount", _basicAttackCount);
            }
        }
    }

    public void AnimEvent_FinishBaseAttackAnim()
    {
        // now you can cast attack
        IsBasicAttacking = false;

        _hitBoxRadius = 0f;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(_attackHitBoxTrans.position, _hitBoxRadius);
    }
}