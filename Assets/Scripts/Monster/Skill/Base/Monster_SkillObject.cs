using System.Collections;
using UnityEngine;

public abstract class Monster_SkillObject : MonoBehaviour
{
    [Header("Monster_SkillObject")]
    [Space]

    [SerializeField] protected LayerMask targetLayer;
    [SerializeField] protected LayerMask destroyLayer;

    [Space]

    [SerializeField] protected int damage;
    [SerializeField] protected float forceX;
    [SerializeField] protected float forceY;

    [Space]

    [SerializeField] protected GameObject hitEffect;
    [SerializeField] protected SoundClipData colideSound;

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        // ��ų Ÿ�� ���̾�� �浹
        if ((1 << collision.gameObject.layer & targetLayer.value) > 0)
        {
            // Ÿ���� �÷��̾��� ���
            PlayerBehaviour player = collision.GetComponent<PlayerBehaviour>();
            if (player)
            {
                // �÷��̾ Ÿ�� ������ ������ ���
                if (!player.IsHurt && !player.IsGodMode && !player.IsDead)
                {
                    // �÷��̾ Ÿ��
                    Vector2 forceVector = new Vector2(forceX * Mathf.Sign(player.transform.position.x - this.transform.position.x), forceY);
                    IAttackListener.AttackResult attackResult = player.OnHit(new AttackInfo(damage, forceVector, AttackType.Monster_SkillAttack));

                    // Ÿ�� ���� �� ��Ʈ ����Ʈ ����
                    if (attackResult == IAttackListener.AttackResult.Success)
                    {
                        Vector2 playerPos = player.transform.position;
                        Instantiate(hitEffect, playerPos + Random.insideUnitCircle * 0.3f, Quaternion.identity);
                    }
                }
            }
        }

        // ��ų �ı� ���̾�� �浹
        if ((1 << collision.gameObject.layer & destroyLayer.value) > 0)
        {
            if (colideSound != null)
                SoundManager.Instance.PlaySFX(colideSound);

            var effect = GetComponent<DisintegrateEffect>();
            if (effect)
                StartCoroutine(DeathEffectCoroutine(effect));
            else
                Destroy(this.gameObject);
        }
    }

    public IEnumerator DeathEffectCoroutine(DisintegrateEffect effect)
    {
        GetComponent<Rigidbody2D>().simulated = false;

        effect.Play();

        yield return new WaitUntil(() => effect.IsEffectDone);

        Destroy(gameObject);
    }
}
