using UnityEngine;

public class Mushroom_DevourAttack : Monster_SkillAttack
{
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        // base.OnTriggerEnter2D(collision);

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
                    Transform mushroomTrans = this.transform.root;
                    Vector2 forceVector = new Vector2(_skillForceX * Mathf.Sign(player.transform.position.x - mushroomTrans.position.x), _skillForceY);
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

        // ��ų �ı� ���̾�� �浹
        if ((1 << collision.gameObject.layer & _skillDestroyLayer.value) > 0)
            Destroy(this.gameObject);
    }
}
