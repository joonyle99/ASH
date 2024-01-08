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

    // 대상이 콜라이더에 계속 들어와있을 경우를 고려해 TriggerStay 사용
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (_isDisableHitBox)
            return;

        // 플레이어와 충돌
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