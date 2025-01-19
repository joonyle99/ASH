using UnityEngine;

public class DealDamageOnFastContact : MonoBehaviour, ICollisionWithPlayerListener
{
    [Header("Common")]
    [Space]

    /// <summary>�������� �ֱ� ���� �ּ� �ӵ� �Ӱ谪</summary>
    [SerializeField] protected float _threatVelocityThreshold = 3;

    [Header("Player")]
    [Space]

    /// <summary>�÷��̾�� �������� ���� �� �ִ� ������Ʈ���� ����</summary>
    [SerializeField] private bool _isDamagableToPlayer = true;
    /// <summary>�÷��̾�� ���� ������ ��</summary>
    [SerializeField] protected float _damageToPlayer = 1f;
    /// <summary>�÷��̾�� ���� �˹� ��</summary>
    [SerializeField] protected float _knockbackPowerXToPlayer = 7f;
    [SerializeField] protected float _knockbackPowerYToPlayer = 10f;

    [Header("Monster")]
    [Space]

    /// <summary>���Ϳ��� �������� ���� �� �ִ� ������Ʈ���� ����</summary>
    [SerializeField] private bool _isDamagableToMonster = true;
    /// <summary>���Ϳ��� ���� ������ ��</summary>
    [SerializeField] protected float _damageToMonster = 1f;
    /// <summary>���Ϳ��� ���� �˹� ��</summary>
    [SerializeField] protected float _knockbackPowerXToMonster = 7f;
    [SerializeField] protected float _knockbackPowerYToMonster = 10f;

    [Space]

    /// <summary>��ȣ�ۿ� ������ ������Ʈ ������Ʈ ����</summary>
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
        // ��ȣ�ۿ� ������ ������Ʈ�� ���
        if (_interactableComponent)
        {
            // ��ȣ�ۿ� ���� ��� �������� ���� ����
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

        // �ӵ��� �Ӱ� ������ ���� ���
        if (_rigidbody.velocity.sqrMagnitude > Mathf.Pow(_threatVelocityThreshold, 2))
        {
            Debug.Log("_threatVelocityThreshold Spike");

            // �������� �� �� �ִ� ���
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
        // �ӵ��� �Ӱ� ������ ���� ���
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
