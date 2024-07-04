using UnityEngine;

public class DealDamageOnContact : MonoBehaviour, ICollisionWithPlayerListener
{
    [SerializeField] protected bool _isDamagableToPlayer = true;
    [SerializeField] protected float _damage = 1f;
    [SerializeField] protected float _knockbackPowerX = 7f;
    [SerializeField] protected float _knockbackPowerY = 10f;

    private Rigidbody2D _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerBehaviour player = collision.transform.GetComponent<PlayerBehaviour>();

        if (player != null && _isDamagableToPlayer)
        {
            OnPlayerEnter(player);
        }
    }

    protected virtual bool CanDealDamage(PlayerBehaviour player)
    {
        return true;
    }

    public void OnPlayerEnter(PlayerBehaviour player)
    {
        if (CanDealDamage(player))
        {
            if (!player.IsHurt && !player.IsGodMode && !player.IsDead)
            {
                Vector2 knockbackForce = new Vector2(_knockbackPowerX * Mathf.Sign(player.transform.position.x - this.transform.position.x), _knockbackPowerY);
                AttackInfo attackInfo = new AttackInfo(_damage, knockbackForce, AttackType.GimmickAttack);
                var result = player.OnHit(attackInfo);
            }
        }
    }
}
