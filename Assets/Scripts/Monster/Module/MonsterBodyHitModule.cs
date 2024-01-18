using UnityEngine;

public class MonsterBodyHitModule : MonoBehaviour
{
    [Header("Monster BodyHit Module")]
    [Space]

    [SerializeField] private LayerMask _targetLayer;
    [SerializeField] private int _bodyAttackDamage = 5;
    [SerializeField] private float _forceXPower = 7f;
    [SerializeField] private float _forceYPower = 9f;

    [Space]

    [SerializeField] private bool _isAttackable = true;
    public bool IsAttackable
    {
        get { return _isAttackable; }
        set { _isAttackable = value; }
    }
    [SerializeField] private bool _isHurtable = true;
    public bool IsHurtable
    {
        get { return _isHurtable; }
        set { _isHurtable = value; }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsHurtable)
        {
            var spike = collision.GetComponent<Spikes>();
            if (spike)
            {
                var monster = GetComponentInParent<MonsterBehavior>();
                monster.Animator.SetTrigger("Die");
            }
        }
    }

    // ����� �ݶ��̴��� ��� �������� ��츦 ����� TriggerStay ���
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(IsAttackable)
        {
            // ������ �浹
            if ((collision.gameObject.layer | _targetLayer) > 0)
            {
                // �÷��̾�� �浹
                PlayerBehaviour player = collision.gameObject.GetComponent<PlayerBehaviour>();

                if (!player)
                    return;

                if (player.IsHurt || player.IsGodMode || player.IsDead)
                    return;

                // set force vector
                float dir = Mathf.Sign(player.transform.position.x - transform.position.x);
                Vector2 forceVector = new Vector2(_forceXPower * dir, _forceYPower);

                player.OnHit(_bodyAttackDamage, forceVector);
            }
        }
    }
}