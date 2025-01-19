using UnityEngine;

public class DealDamageOnFastContact : MonoBehaviour, ICollisionWithPlayerListener
{
    [Header("Common")]
    [Space]

    /// <summary>데미지를 주기 위한 최소 속도 임계값</summary>
    [SerializeField] protected float _threatVelocityThreshold = 3;

    [Header("Player")]
    [Space]

    /// <summary>플레이어에게 데미지를 가할 수 있는 오브젝트인지 여부</summary>
    [SerializeField] private bool _isDamagableToPlayer = true;
    /// <summary>플레이어에게 가할 데미지 양</summary>
    [SerializeField] protected float _damageToPlayer = 1f;
    /// <summary>플레이어에게 가할 넉백 힘</summary>
    [SerializeField] protected float _knockbackPowerXToPlayer = 7f;
    [SerializeField] protected float _knockbackPowerYToPlayer = 10f;

    [Header("Monster")]
    [Space]

    /// <summary>몬스터에게 데미지를 가할 수 있는 오브젝트인지 여부</summary>
    [SerializeField] private bool _isDamagableToMonster = true;
    /// <summary>몬스터에게 가할 데미지 양</summary>
    [SerializeField] protected float _damageToMonster = 1f;
    /// <summary>몬스터에게 가할 넉백 힘</summary>
    [SerializeField] protected float _knockbackPowerXToMonster = 7f;
    [SerializeField] protected float _knockbackPowerYToMonster = 10f;

    [Space]

    /// <summary>상호작용 가능한 오브젝트 컴포넌트 참조</summary>
    [SerializeField] private InteractableObject _interactableComponent;

    private Rigidbody2D _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the object is a player
        PlayerBehaviour player = collision.transform.GetComponent<PlayerBehaviour>();

        if (player != null && _isDamagableToPlayer)
        {
            // Debug.Log("player ~");
            OnPlayerEnter(player);

            return;
        }

        // Check if the object is a monster
        MonsterBehaviour monster = collision.transform.GetComponent<MonsterBehaviour>();

        if (monster != null && _isDamagableToMonster)
        {
            // Debug.Log("monster ~");
            OnMonsterEnter(monster);
        }
    }

    protected virtual bool CanDealDamage(PlayerBehaviour player)
    {
        // 상호작용 가능한 오브젝트인 경우
        if (_interactableComponent)
        {
            // 상호작용 중인 경우 데미지를 주지 않음
            if (_interactableComponent.IsInteracting)
            {
                return false;
            }
        }

        return true;
    }

    public void OnPlayerEnter(PlayerBehaviour player)
    {
        Debug.Log("OnPlayerEnter Spike");

        // 속도가 임계 값보다 높은 경우
        if (_rigidbody.velocity.sqrMagnitude > Mathf.Pow(_threatVelocityThreshold, 2))
        {
            Debug.Log("_threatVelocityThreshold Spike");

            // 데미지를 줄 수 있는 경우
            if (CanDealDamage(player))
            {
                if (!player.IsHurt && !player.IsGodMode && !player.IsDead)
                {
                    Vector2 knockbackForce = new Vector2(_knockbackPowerXToPlayer * Mathf.Sign(player.transform.position.x - this.transform.position.x), _knockbackPowerYToPlayer);
                    AttackInfo attackInfo = new AttackInfo(_damageToPlayer, knockbackForce, AttackType.GimmickAttack);
                    var result = player.OnHit(attackInfo);

                    Debug.Log("OnHit From Spike");
                }
            }
        }
    }
    public void OnMonsterEnter(MonsterBehaviour monster)
    {
        // 속도가 임계 값보다 높은 경우
        if (_rigidbody.velocity.sqrMagnitude > Mathf.Pow(_threatVelocityThreshold, 2))
        {
            if (!monster.IsHurt && !monster.IsGodMode && !monster.IsDead)
            {
                Vector2 knockbackForce = new Vector2(_knockbackPowerXToMonster * Mathf.Sign(monster.transform.position.x - this.transform.position.x), _knockbackPowerYToMonster);
                AttackInfo attackInfo = new AttackInfo(_damageToMonster, knockbackForce, AttackType.GimmickAttack);
                var result = monster.OnHit(attackInfo);
            }
        }
    }
}
