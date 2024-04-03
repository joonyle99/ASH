using System.Collections;
using UnityEngine;

/// <summary>
/// 몬스터의 독립적으로 생성되어 동작하며 타겟 콜라이더와 충돌 시 파괴되는 '투사체'와 비슷한 스킬
/// ex) 흑곰의 종유석
/// </summary>
public class Monster_ProjectileSkill : Monster_Skill
{
    [Header("Projectile Skill")]
    [Space]

    [SerializeField] protected LayerMask destroyLayer;
    [SerializeField] protected SoundClipData colliderSound;

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        // 스킬 파괴 레이어와 충돌
        if ((1 << collision.gameObject.layer & destroyLayer.value) > 0)
        {
            if (colliderSound != null)
                SoundManager.Instance.PlaySFX(colliderSound);

            var effect = GetComponent<DisintegrateEffect>();
            if (effect)
                StartCoroutine(DeathEffectCoroutine(effect));
            else
                Destroy(this.gameObject);
        }
    }

    private IEnumerator DeathEffectCoroutine(DisintegrateEffect effect)
    {
        GetComponent<Rigidbody2D>().simulated = false;

        effect.Play();

        yield return new WaitUntil(() => effect.IsEffectDone);

        Destroy(gameObject);
    }
}
