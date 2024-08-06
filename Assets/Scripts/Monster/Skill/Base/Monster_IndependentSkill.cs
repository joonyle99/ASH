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

    [SerializeField] protected float lifeTime;                  // 투사체가 살아있는 시간
    [SerializeField] protected float effectDelay;

    protected MaterialController materialController;

    private Coroutine _autoDestroyCoroutine;

    protected override void Awake()
    {
        base.Awake();

        materialController = GetComponent<MaterialController>();
    }
    private void Start()
    {
        // 스킬이 생성되고 일정 시간이 지나면 자동으로 파괴
        _autoDestroyCoroutine = StartCoroutine(AutoDestroy(lifeTime));
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        var collisionLayerValue = 1 << collision.gameObject.layer;
        if ((collisionLayerValue & targetLayer.value) > 0) return;            // 타겟 레이어와 충돌 시 무시
        if ((collisionLayerValue & destroyLayer.value) > 0)
        {
            if (materialController == null || materialController.DisintegrateEffect == null)
            {
                DestroyImmediately();
            }
            else
            {
                StopAutoDestroy();
                StartCoroutine(DisintegrateCoroutine(materialController.DisintegrateEffect));
            }
        }
    }

    public void SetLifeTime(float time)
    {
        lifeTime = time;
    }
    public void SetEffectDelay(float delay)
    {
        effectDelay = delay;
    }

    private IEnumerator AutoDestroy(float time)
    {
        if (time < 0.1f) yield break;

        yield return new WaitForSeconds(time);

        DestroyImmediately();
    }
    private void StopAutoDestroy()
    {
        if (_autoDestroyCoroutine != null)
        {
            StopCoroutine(_autoDestroyCoroutine);
            _autoDestroyCoroutine = null;
        }
    }

    private IEnumerator DisintegrateCoroutine(DisintegrateEffect effect)
    {
        var rigid = GetComponent<Rigidbody2D>();
        if (rigid) rigid.simulated = false;

        yield return DestroyEffectCoroutine(effect);

        DestroyImmediately();
    }
    private IEnumerator DestroyEffectCoroutine(DisintegrateEffect effect)
    {
        effect.Play(effectDelay);
        yield return new WaitUntil(() => effect.IsEffectDone);
    }
}
