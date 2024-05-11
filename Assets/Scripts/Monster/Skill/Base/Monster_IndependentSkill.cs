using System.Collections;
using UnityEngine;

/// <summary>
/// ������ ���������� �����Ǿ� �����ϴ� ��ų�̴�. 
/// �ߵ� ��, �ð��� ������ ������ų� Ÿ�� �ݶ��̴��� �浹 �� �ı��Ǵ� ��ų
/// </summary>
public class Monster_IndependentSkill : Monster_Skill
{
    [Header("Independent Skill")]
    [Space]

    [SerializeField] protected LayerMask destroyLayer;
    [SerializeField] protected float effectDelay;
    [SerializeField] protected SoundClipData colliderSound;

    [Space]

    [SerializeField] protected float lifeTime;                  // ����ü�� ����ִ� �ð�
    [SerializeField] protected bool isDestroyWhenPlayer;        // �÷��̾�� �浹 �� �ı������� ����

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

        // ��ų �ı� ���̾�� �浹
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
            Debug.LogWarning($"lifeTime�� {lifeTime}�̱� ������ Auto Destroy ����� ������� �ʽ��ϴ�");
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
            Debug.LogError("�ı��� ����� �����ϴ�");
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
