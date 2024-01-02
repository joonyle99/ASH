using UnityEngine;

/// <summary>
/// ������ Ÿ�� / �ǰ��� ���
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
    /// �÷��̾� Ÿ��
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        // �ǰ� �� �÷��̾ ���Ϳ� �پ��ִ� ����..?
        // �׷� ���� ����..?
        // �ƴϴ� �÷��̾ ���� ������ ��� ���Ϳ��� ���� �� �ִ�.

        // �÷��̾�� �浹
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            PlayerBehaviour player = collision.gameObject.GetComponent<PlayerBehaviour>();

            if (player == null)
            {
                Debug.LogError("�ǰ� ��� PlayerBehavior�� �پ����� �ʽ��ϴ�");
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