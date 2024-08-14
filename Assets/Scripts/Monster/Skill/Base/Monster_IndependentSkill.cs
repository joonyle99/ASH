using System.Collections;
using UnityEditor.Rendering;
using UnityEngine;

/// <summary>
/// ������ ���������� �����Ǿ� �����ϴ� ��ų�̴�. 
/// �ߵ� ��, �ð��� ������ ������ų� Ÿ�� �ݶ��̴��� �浹 �� �ı��Ǵ� ��ų
/// </summary>
public class Monster_IndependentSkill : Monster_Skill
{
    [Header("Independent Skill")]
    [Space]

    [SerializeField] protected float lifeTime;                  // ����ü�� ����ִ� �ð�
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
        // ��ų�� �����ǰ� ���� �ð��� ������ �ڵ����� �ı�
        _autoDestroyCoroutine = StartCoroutine(AutoDestroy(lifeTime));
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        var collisionLayerValue = 1 << collision.gameObject.layer;
        if ((collisionLayerValue & targetLayer.value) > 0) return;            // Ÿ�� ���̾�� �浹 �� ����
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
