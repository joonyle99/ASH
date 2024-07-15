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

    [SerializeField] protected float effectDelay;
    // [SerializeField] protected SoundClipData colliderSound;

    [Header("_____ Destroy Condition: Collision _____")]
    [Space]

    [SerializeField] protected LayerMask destroyLayer;

    [Header("_____ Destroy Condition: LifeTime _____")]
    [Space]

    [SerializeField] protected float lifeTime;                  // ����ü�� ����ִ� �ð�

    protected Rigidbody2D rigid;

    protected override void Awake()
    {
        base.Awake();

        rigid = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        // ��ų�� �����ǰ� ���� �ð��� ������ �ڵ����� �ı�
        StartCoroutine(AutoDestroy(lifeTime));
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        // ��ų �ı� ���̾�� �浹
        var collisionLayerValue = 1 << collision.gameObject.layer;
        if ((collisionLayerValue & destroyLayer.value) > 0)
        {
            /*
            if (colliderSound != null)
                SoundManager.Instance.PlaySFX(colliderSound);
            */

            if (!materialController || !materialController.DisintegrateEffect || collision.GetComponent<PlayerBehaviour>())
            {
                DestroyIndependentSkill();
            }
            else
            {
                StartCoroutine(DisintegrateCoroutine(materialController.DisintegrateEffect));
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
        // AutoDestroy �ڷ�ƾ �ߴ�
        StopAllCoroutines();
    }
}
