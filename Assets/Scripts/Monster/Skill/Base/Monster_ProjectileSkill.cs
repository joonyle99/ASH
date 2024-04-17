using System.Collections;
using UnityEngine;

/// <summary>
/// ������ ���������� �����Ǿ� �����ϸ� Ÿ�� �ݶ��̴��� �浹 �� �ı��Ǵ� '����ü'�� ����� ��ų
/// ex) ����� ������
/// </summary>
public class Monster_ProjectileSkill : Monster_Skill
{
    [Header("Projectile Skill")]
    [Space]

    [SerializeField] protected LayerMask destroyLayer;
    [SerializeField] protected float effectDelay;
    [SerializeField] protected SoundClipData colliderSound;

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        // ��ų �ı� ���̾�� �浹
        if ((1 << collision.gameObject.layer & destroyLayer.value) > 0)
        {
            if (colliderSound != null)
                SoundManager.Instance.PlaySFX(colliderSound);

            if (materialController && materialController.DisintegrateEffect)
                StartCoroutine(DeathProcessCoroutine(materialController.DisintegrateEffect));
            else
                Destroy(gameObject);
        }
    }

    private IEnumerator DeathProcessCoroutine(DisintegrateEffect effect)
    {
        GetComponent<Rigidbody2D>().simulated = false;

        yield return StartCoroutine(DeathEffectCoroutine(effect));

        Destroy(transform.root ? transform.root.gameObject : gameObject);
    }
    private IEnumerator DeathEffectCoroutine(DisintegrateEffect effect)
    {
        effect.Play(effectDelay);
        yield return new WaitUntil(() => effect.IsEffectDone);
    }
}
