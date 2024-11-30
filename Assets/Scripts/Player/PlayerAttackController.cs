using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackController : MonoBehaviour
{
    [Header("Attack Setting")]
    [Space]

    [SerializeField] private LayerMask _attackableEntityLayer;
    [SerializeField] private Collider2D _attackCollider;

    [Space]

    [SerializeField] private int _attackDamage = 1;
    [SerializeField] private float _attackPowerX = 7f;
    [SerializeField] private float _attackPowerY = 10f;

    [Space]

    [SerializeField] private float _targetAttackCoolTime;
    private Coroutine _attackCooldownCoroutine;

    [Header("Effects")]
    [Space]

    [SerializeField] ParticleHelper _attackEffects;
    [SerializeField] GameObject _basicAttackImpactPrefab;

    private PlayerBehaviour _player;

    private void Awake()
    {
        _player = GetComponent<PlayerBehaviour>();
    }

    public void CastAttack()
    {
        _player.CanAttack = false;

        if (_attackCooldownCoroutine != null)
        {
            StopCoroutine(_attackCooldownCoroutine);
        }

        _attackCooldownCoroutine = StartCoroutine(AttackCooldown());

        _player.Animator.SetTrigger("Attack");
        _player.PlaySound_SE_Attack();
        _attackEffects.Play();
    }
    public void AttackProcess_AnimEvent()
    {
        // cast parameter
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(_attackableEntityLayer);
        List<RaycastHit2D> rayCastHits = new List<RaycastHit2D>();

        // do cast
        _attackCollider.Cast(_player.PlayerLookDir2D, filter, rayCastHits, 0);

        List<Rigidbody2D> handledBodies = new List<Rigidbody2D>();

        foreach (var rayCastHit in rayCastHits)
        {
            // rayCastHit에 RigidBody가 없거나 이미 리스트업 했다면 continue
            if (!rayCastHit.rigidbody || handledBodies.Contains(rayCastHit.rigidbody))
                continue;

            // rayCastHit의 모든 IAttackListener를 가져옴
            var listeners = rayCastHit.rigidbody.GetComponents<IAttackListener>();

            handledBodies.Add(rayCastHit.rigidbody);

            IAttackListener.AttackResult attackResult = IAttackListener.AttackResult.Fail;
            foreach (var listener in listeners)
            {
                Vector2 forceVector = new Vector2(_attackPowerX * Mathf.Sign(rayCastHit.transform.position.x - transform.position.x), _attackPowerY);
                var attackInfo = new AttackInfo(_attackDamage, forceVector, AttackType.Player_BasicAttack);
                var result = listener.OnHit(attackInfo);
                if (result == IAttackListener.AttackResult.Success)
                    attackResult = IAttackListener.AttackResult.Success;
            }

            if (attackResult == IAttackListener.AttackResult.Success)
            {
                Instantiate(_basicAttackImpactPrefab, rayCastHit.point + Random.insideUnitCircle * 0.2f, Quaternion.identity);
            }
        }
    }
    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(_targetAttackCoolTime);
        _player.CanAttack = true;
    }
}