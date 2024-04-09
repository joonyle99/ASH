using UnityEngine;

public abstract class Monster_Skill : MonoBehaviour
{
    [Header("Monster Skill")]
    [Space]

    [SerializeField] protected MonsterBehavior actor;
    [SerializeField] protected LayerMask targetLayer;

    [Space]

    [SerializeField] protected int damage;
    [SerializeField] protected float forceX;
    [SerializeField] protected float forceY;

    [Space]

    [SerializeField] protected GameObject hitEffect;

    protected delegate void MosnterSkillEvent();
    protected MosnterSkillEvent monsterSkillEvent;

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        // ��ų Ÿ�� ���̾�� �浹
        if ((1 << collision.gameObject.layer & targetLayer.value) > 0)
        {
            // Ÿ���� �÷��̾��� ���
            PlayerBehaviour player = collision.GetComponent<PlayerBehaviour>();
            if (player)
            {
                // �÷��̾ Ÿ�� ������ ������ ���
                if (!player.IsHurt && !player.IsGodMode && !player.IsDead)
                {
                    // Actor�� ������ Actor�� �������� ���� ����
                    Vector2 forceVector = (actor == null)
                        ? new Vector2(forceX * Mathf.Sign(player.transform.position.x - this.transform.position.x),
                            forceY)
                        : new Vector2(forceX * Mathf.Sign(player.transform.position.x - actor.transform.position.x),
                            forceY);

                    // �÷��̾�� Ÿ��
                    IAttackListener.AttackResult attackResult = player.OnHit(new AttackInfo(damage, forceVector, AttackType.Monster_SkillAttack));

                    // Ÿ�� ���� �� ��Ʈ ����Ʈ ����
                    if (attackResult == IAttackListener.AttackResult.Success)
                    {
                        Vector2 playerPos = player.transform.position;
                        Instantiate(hitEffect, playerPos + Random.insideUnitCircle * 0.3f, Quaternion.identity);
                        monsterSkillEvent?.Invoke();
                    }
                }
            }
        }
    }

    public void SetActor(MonsterBehavior actor)
    {
        this.actor = actor;
    }
}
