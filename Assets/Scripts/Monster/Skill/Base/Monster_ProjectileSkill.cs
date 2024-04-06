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
            /*
            if (colliderSound != null)
                SoundManager.Instance.PlaySFX(colliderSound);
            */

            var effect = GetComponent<DisintegrateEffect_New>();
            if (effect)
                StartCoroutine(DeathEffectCoroutine(effect));
            else
                Destroy(this.gameObject);
        }
    }

    private IEnumerator DeathEffectCoroutine(DisintegrateEffect_New effect)
    {
        // ���� �ùķ��̼� ����
        GetComponent<Rigidbody2D>().simulated = false;

        // ����� ����Ʈ ����
        effect.Play(effectDelay);

        yield return new WaitUntil(() => effect.IsEffectDone);

        Destroy(gameObject);
    }
}
