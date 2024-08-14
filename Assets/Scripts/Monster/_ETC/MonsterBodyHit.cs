using UnityEngine;

/// <summary>
/// ������ �ٵ� Ÿ���� ���
/// </summary>
public class MonsterBodyHit : MonoBehaviour
{
    [Header("Monster Body Hit")]
    [Space]

    [SerializeField] private LayerMask _targetLayer;
    [SerializeField] private GameObject _hitEffect;

    [Space]

    [SerializeField] private int _damage = 1;
    [SerializeField] private float _forceX = 7f;
    [SerializeField] private float _forceY = 10f;

    [Space]

    [SerializeField] private bool _isAttackable = true;
    public bool IsAttackable
    {
        get => _isAttackable;
        set => _isAttackable = value;
    }

    // ����� �ݶ��̴��� ��� �������� ��츦 ����� TriggerStay ���
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (IsAttackable)
        {
            // Ÿ�� ���̾�� �浹
            if ((1 << collision.gameObject.layer & _targetLayer.value) > 0)
            {
                // Ÿ���� �÷��̾��� ���
                PlayerBehaviour player = collision.GetComponent<PlayerBehaviour>();
                if (player)
                {
                    // Debug.Log($"{this.transform.root.gameObject.name}�ȿ� Player�� �����ִ� !");

                    // �÷��̾ Ÿ�� ������ ������ ���
                    if (!player.IsHurt && !player.IsGodMode && !player.IsDead)
                    {
                        // �÷��̾ Ÿ��
                        Vector2 forceVector = new Vector2(_forceX * Mathf.Sign(player.transform.position.x - this.transform.position.x), _forceY);
                        var attackInfo = new AttackInfo(_damage, forceVector, AttackType.Monster_BodyAttack);
                        IAttackListener.AttackResult attackResult = player.OnHit(attackInfo);

                        // Ÿ�� ���� �� ��Ʈ ����Ʈ ����
                        if (attackResult == IAttackListener.AttackResult.Success)
                        {
                            Vector2 playerPos = player.transform.position;
                            Instantiate(_hitEffect, playerPos + Random.insideUnitCircle * 0.3f, Quaternion.identity);
                        }
                    }
                }
            }
        }
    }
}