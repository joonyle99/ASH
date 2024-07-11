using System.Collections;
using UnityEditor.Rendering;
using UnityEngine;

/// <summary>
/// 몬스터의 독립적으로 생성되어 동작하는 스킬이다. 
/// 발동 후, 시간이 지나면 사라지거나 타겟 콜라이더와 충돌 시 파괴되는 스킬
/// </summary>
public class Monster_IndependentSkill : Monster_Skill
{
    [Header("Independent Skill")]
    [Space]

    [SerializeField] protected float effectDelay;
    // [SerializeField] protected SoundClipData colliderSound;

    [Header("_____ Destroy Condition: Collision _____")]
    [Space]

    [SerializeField] protected bool isDestroyWhenPlayer;        // 플레이어와 충돌 시 파괴할지의 여부
    [SerializeField] protected LayerMask destroyLayer;


    [Header("_____ Destroy Condition: LifeTime _____")]
    [Space]

    [SerializeField] protected float lifeTime;                  // 투사체가 살아있는 시간

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
        // 스킬이 생성되고 일정 시간이 지나면 자동으로 파괴
        StartCoroutine(AutoDestroy(lifeTime));
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        // 스킬 파괴 레이어와 충돌
        if ((1 << collision.gameObject.layer & destroyLayer.value) > 0)
        {
            /*
            if (colliderSound != null)
                SoundManager.Instance.PlaySFX(colliderSound);
            */

            if (materialController && materialController.DisintegrateEffect)
                StartCoroutine(DisintegrateCoroutine(materialController.DisintegrateEffect));
            else
            {
                // DisintegrateEffect가 없는 경우, 즉시 삭제한다.
                DestroyIndependentSkill();
            }
        }
    }

    private IEnumerator AutoDestroy(float time)
    {
        if (time < 0.1f) yield break;

        yield return new WaitForSeconds(time);

        DestroyIndependentSkill();
    }
    public void DestroyIndependentSkill()
    {
        Destroy(this.gameObject);
    }

    private IEnumerator DisintegrateCoroutine(DisintegrateEffect effect)
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
        // AutoDestroy 코루틴 중단
        StopAllCoroutines();

        if (monsterSkillEvent != null)
        {
            monsterSkillEvent -= DestroyIndependentSkill;
        }
    }
}
