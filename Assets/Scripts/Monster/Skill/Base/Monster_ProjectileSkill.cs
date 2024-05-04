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
