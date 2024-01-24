using UnityEngine;

/// <summary>
/// ������ �ٵ� Ÿ���� ����ϴ� ���
/// </summary>
public class MonsterBodyHitModule : MonoBehaviour
{
    [Header("Monster BodyHit Module")]
    [Space]

    [SerializeField] private LayerMask _bodyHitTargetLayer;
    [SerializeField] private GameObject _toHitEffectPrefab;

    [Space]

    [SerializeField] private int _bodyHitDamage = 5;
    [SerializeField] private float _bodyHitForceX = 7f;
    [SerializeField] private float _bodyHitForceY = 9f;

    [Space]

    [SerializeField] private bool _isAttackable = true;
    public bool IsAttackable
    {
        get { return _isAttackable; }
        set { _isAttackable = value; }
    }

    // ����� �ݶ��̴��� ��� �������� ��츦 ����� TriggerStay ���
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(IsAttackable)
        {
            // Ÿ�� ���̾�� �浹
            if ((1 << collision.gameObject.layer & _bodyHitTargetLayer.value) > 0)
            {
                // Ÿ���� �÷��̾��� ���
                PlayerBehaviour player = collision.GetComponent<PlayerBehaviour>();
                if (player)
                {
                    // �÷��̾ Ÿ�� ������ ������ ���
                    if (!player.IsHurt && !player.IsGodMode && !player.IsDead)
                    {
                        // �÷��̾ Ÿ��
                        Vector2 forceVector = new Vector2(_bodyHitForceX * Mathf.Sign(player.transform.position.x - this.transform.position.x), _bodyHitForceY);
                        IAttackListener.AttackResult attackResult = player.OnHit(new AttackInfo(_bodyHitDamage, forceVector, AttackType.Monster_BodyAttack));

                        // Ÿ�� ���� �� ��Ʈ ����Ʈ ����
                        if (attackResult == IAttackListener.AttackResult.Success)
                        {
                            Vector2 playerPos = player.transform.position;
                            Instantiate(_toHitEffectPrefab, playerPos + Random.insideUnitCircle * 0.3f, Quaternion.identity);
                        }
                    }
                }
            }
        }
    }
}