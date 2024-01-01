using UnityEngine;

/// <summary>
/// 몬스터의 타격 / 피격을 담당
/// </summary>
public class MonsterBodyHit : MonoBehaviour
{
    [SerializeField] private MonsterBehavior _monster;

    [SerializeField] private int _damage;
    [SerializeField] private float _forceXPower;
    [SerializeField] private float _forceYPower;

    void Awake()
    {
        _monster = GetComponentInParent<MonsterBehavior>();
    }

    /// <summary>
    /// 플레이어 타격
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        // 피격 후 플레이어가 몬스터와 붙어있는 상태..?
        // 그런 경우는 없다..?
        // 아니다 플레이어가 무적 상태인 경우 몬스터에게 붙을 수 있다.

        // 플레이어와 충돌
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            PlayerBehaviour player = collision.gameObject.GetComponent<PlayerBehaviour>();

            if (player == null)
            {
                Debug.LogError("피격 대상에 PlayerBehavior가 붙어있지 않습니다");
                return;
            }

            if (player.IsGodMode || player.IsDead)
                return;

            if (!player.IsHurtable)
                return;

            float dir = Mathf.Sign(player.transform.position.x - transform.position.x);
            Vector2 forceVector = new Vector2(_forceXPower * dir, _forceYPower);

            player.OnHit(_damage, forceVector);
        }
    }
}