using UnityEngine;

public class MonsterBodyHit : MonoBehaviour
{
    [SerializeField] private int _bodyAttackDamage = 5;
    [SerializeField] private float _forceXPower = 7f;
    [SerializeField] private float _forceYPower = 9f;
    [SerializeField] private bool _isDisableHitBox;
    public bool IsDisableHitBox
    {
        get { return _isDisableHitBox;}
        set { _isDisableHitBox = value; }
    }

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
        if (_isDisableHitBox)
            return;

        // �÷��̾�� �浹
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            PlayerBehaviour player = collision.gameObject.GetComponent<PlayerBehaviour>();

            if (player == null)
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