using UnityEngine;

public abstract class Monster_SkillObject : MonoBehaviour
{
    [Header("Monster_SkillObject")]
    [Space]

    [SerializeField] protected LayerMask _skillTargetLayer;
    [SerializeField] protected LayerMask _skillDestroyLayer;

    [Space]

    [SerializeField] protected int _skillDamage;
    [SerializeField] protected float _skillForceX;
    [SerializeField] protected float _skillForceY;

    [Space]

    [SerializeField] protected GameObject _hitEffectPrefab;

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        // ��ų Ÿ�� ���̾�� �浹
        if ((1 << collision.gameObject.layer & _skillTargetLayer.value) > 0)
        {
            // Ÿ���� �÷��̾��� ���
            PlayerBehaviour player = collision.GetComponent<PlayerBehaviour>();
            if (player)
            {
                // �÷��̾ Ÿ�� ������ ������ ���
                if (!player.IsHurt && !player.IsGodMode && !player.IsDead)
                {
                    // �÷��̾ Ÿ��
                    Vector2 forceVector = new Vector2(_skillForceX * Mathf.Sign(player.transform.position.x - this.transform.position.x), _skillForceY);
                    IAttackListener.AttackResult attackResult = player.OnHit(new AttackInfo(_skillDamage, forceVector, AttackType.Monster_SkillAttack));

                    // Ÿ�� ���� �� ��Ʈ ����Ʈ ����
                    if (attackResult == IAttackListener.AttackResult.Success)
                    {
                        Vector2 playerPos = player.transform.position;
                        Instantiate(_hitEffectPrefab, playerPos + Random.insideUnitCircle * 0.3f, Quaternion.identity);
                    }
                }
            }
        }

        Debug.Log(this.gameObject.name + " : " + collision.gameObject.name);

        // ��ų �ı� ���̾�� �浹
        if ((1 << collision.gameObject.layer & _skillDestroyLayer.value) > 0)
            Destroy(this.gameObject);
    }
}
