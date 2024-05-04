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
    [SerializeField] protected float effectDelay;
    [SerializeField] protected SoundClipData colliderSound;

    [SerializeField] protected float lifeTime;
    [SerializeField] protected bool isDestroyWhenPlayer;

    protected Rigidbody2D rigidBody2D;

    protected override void Awake()
    {
        base.Awake();

        rigidBody2D = GetComponent<Rigidbody2D>();

        if (isDestroyWhenPlayer)
        {
            monsterSkillEvent -= DestroyProjectile;
            monsterSkillEvent += DestroyProjectile;
        }
    }
    private void Start()
    {
        StartCoroutine(AutoDestroy());
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        // 스킬 파괴 레이어와 충돌
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

    private IEnumerator AutoDestroy()
    {
        yield return new WaitForSeconds(lifeTime);

        DestroyProjectile();
    }
    public void DestroyProjectile()
    {
        Destroy(transform.root ? transform.root.gameObject : gameObject);
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

    private void OnDestroy()
    {
        StopAllCoroutines();

        if (monsterSkillEvent != null)
        {
            monsterSkillEvent -= DestroyProjectile;
        }
    }
}
