using System.Collections;
using UnityEngine;

/// <summary>
/// 몬스터의 독립적으로 생성되어 동작하는 스킬이다. 
/// 발동 후, 시간이 지나면 사라지거나 타겟 콜라이더와 충돌 시 파괴되는 스킬
/// </summary>
public class Monster_IndependentSkill : Monster_Skill
{
    [Header("Independent Skill")]
    [Space]

    [SerializeField] protected LayerMask destroyLayer;
    [SerializeField] protected float effectDelay;
    [SerializeField] protected SoundClipData colliderSound;

    [Space]

    [SerializeField] protected float lifeTime;                  // 투사체가 살아있는 시간
    [SerializeField] protected bool isDestroyWhenPlayer;        // 플레이어와 충돌 시 파괴할지의 여부

    protected Rigidbody2D rigid;

    protected override void Awake()
    {
        base.Awake();

        rigid = GetComponent<Rigidbody2D>();

        if (isDestroyWhenPlayer)
        {
            monsterSkillEvent -= DestroyIndependentSkill;
            monsterSkillEvent += DestroyIndependentSkill;
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
                StartCoroutine(DestroyCoroutine(materialController.DisintegrateEffect));
            else
                DestroyIndependentSkill();
        }
    }

    private IEnumerator AutoDestroy()
    {
        if (lifeTime < 0.1f)
        {
            Debug.LogWarning($"lifeTime이 {lifeTime}이기 때문에 Auto Destroy 기능은 실행되지 않습니다");
            yield break;
        }

        yield return new WaitForSeconds(lifeTime);

        DestroyIndependentSkill();
    }
    public void DestroyIndependentSkill()
    {
        if (transform.root != null)
        {
            Destroy(transform.root.gameObject);
        }
        else if (this.gameObject != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Debug.LogError("파괴할 대상이 없습니다");
        }
    }

    private IEnumerator DestroyCoroutine(DisintegrateEffect effect)
    {
        rigid.simulated = false;

        yield return StartCoroutine(DestroyEffectCoroutine(effect));

        DestroyIndependentSkill();
    }
    private IEnumerator DestroyEffectCoroutine(DisintegrateEffect effect)
    {
        effect.Play(effectDelay);
        yield return new WaitUntil(() => effect.IsEffectDone);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();

        if (monsterSkillEvent != null)
        {
            monsterSkillEvent -= DestroyIndependentSkill;
        }
    }
}
