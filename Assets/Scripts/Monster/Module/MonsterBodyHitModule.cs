using UnityEngine;

public class MonsterBodyHitModule : MonoBehaviour
{
    [Header("Monster BodyHit Module")]
    [Space]

    [SerializeField] private LayerMask _targetLayer;
    [SerializeField] private int _bodyAttackDamage = 5;
    [SerializeField] private float _forceXPower = 7f;
    [SerializeField] private float _forceYPower = 9f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var spike = collision.GetComponent<Spikes>();
        if (spike)
        {
            var monster = GetComponentInParent<MonsterBehavior>();
            monster.Animator.SetTrigger("Die");
        }
    }

    // ����� �ݶ��̴��� ��� �������� ��츦 ����� TriggerStay ���
    private void OnTriggerStay2D(Collider2D collision)
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