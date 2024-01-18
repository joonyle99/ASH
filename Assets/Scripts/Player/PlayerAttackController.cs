using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Gizmos = UnityEngine.Gizmos;

public class PlayerAttackController : MonoBehaviour
{
    [Header("Attack Setting")]
    [Space]

    [SerializeField] private LayerMask _attackableEntityLayer;
    [SerializeField] private Transform _attackHitBoxTrans;
    [SerializeField] private float _hitBoxRadius = 3f;
    private float _currentHitBoxRadius;

    [Space]

    [SerializeField] private int _attackDamage = 20;
    [SerializeField] private float _attackPowerX = 7f;
    [SerializeField] private float _attackPowerY = 10f;

    [Space]

    [SerializeField] private float _targetNextBasicAttackTime = 1.5f;
    [SerializeField] private float _elapsedNextBasicAttackTime;
    [SerializeField] private int _basicAttackCount;
    [SerializeField] private bool _isBasicAttacking;

    [Header("Effects")]
    [SerializeField] ParticleHelper[] _attackEffects;
    [SerializeField] GameObject _basicAttackImpactPrefab;
    public bool IsBasicAttacking
    {
        get => _isBasicAttacking;
        private set => _isBasicAttacking = value;
    }

    private PlayerBehaviour _player;

    private void Awake()
    {
        _player = GetComponent<PlayerBehaviour>();
    }
    private void Update()
    {
        // reset attackCount
        if (_basicAttackCount > 0)
        {
            _elapsedNextBasicAttackTime += Time.deltaTime;

            if (_elapsedNextBasicAttackTime > _targetNextBasicAttackTime)
            {
                _elapsedNextBasicAttackTime = 0f;
                _basicAttackCount = 0;
                _player.Animator.SetInteger("BasicAttackCount", _basicAttackCount);
            }
        }
    }

    public void CastBasicAttack()
    {
        // you can attack when attack animation is done
        if (!IsBasicAttacking)
        {
            IsBasicAttacking = true;
            _elapsedNextBasicAttackTime = 0f;
            _basicAttackCount++;


            _player.Animator.SetTrigger("Attack");
            _player.Animator.SetInteger("BasicAttackCount", _basicAttackCount);

            _player.PlaySound_SE_Attack();
            _attackEffects[_basicAttackCount-1].Play();

            if (_basicAttackCount >= 3)
                _basicAttackCount = 0;
        }
    }
    public void CastShootingAttack()
    {
        // _player.ChangeState<ShootingState>();
    }
    public void AttackableEntityProcess_AnimEvent()
    {
        _currentHitBoxRadius = _hitBoxRadius;
        RaycastHit2D[] rayCastHits = Physics2D.CircleCastAll(_attackHitBoxTrans.position, _currentHitBoxRadius, Vector2.zero,
            0f, _attackableEntityLayer);
        List<Rigidbody2D> handledBodies = new List<Rigidbody2D>();

        foreach (var rayCastHit in rayCastHits)
        {
            if (!rayCastHit.rigidbody || handledBodies.Contains(rayCastHit.rigidbody))
                continue;

            var listeners = rayCastHit.rigidbody.GetComponents<IAttackListener>();
            handledBodies.Add(rayCastHit.rigidbody);

            IAttackListener.AttackResult attackResult = IAttackListener.AttackResult.Fail;
            foreach (var listener in listeners)
            {
                Vector2 forceVector = new Vector2(_attackPowerX * Mathf.Sign(rayCastHit.transform.position.x - transform.position.x), _attackPowerY);
                var result = listener.OnHit(new AttackInfo(_attackDamage, forceVector, AttackType.BasicAttack));
                if (result == IAttackListener.AttackResult.Success)
                    attackResult = IAttackListener.AttackResult.Success;
            }

            if (attackResult == IAttackListener.AttackResult.Success)
            {
                Instantiate(_basicAttackImpactPrefab, rayCastHit.point + Random.insideUnitCircle * 0.3f, Quaternion.identity);
                // TODO : Play impact sound
            }
        }
    }
    public void FinishBasicAttack_AnimEvent()
    {
        // now you can cast attack
        IsBasicAttacking = false;

        _currentHitBoxRadius = 0f;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(_attackHitBoxTrans.position, _currentHitBoxRadius);
    }
}