using UnityEngine;

/// <summary>
/// 몬스터 스킬의 베이스 클래스로 
/// Actor, TargetLayer, Hit Info, 등
/// 몬스터 스킬 사용에 필요한 기본 데이터를 담는다.
/// </summary>
public abstract class Monster_Skill : MonoBehaviour
{
    [Header("Monster Skill")]
    [Space]

    [SerializeField] protected MonsterBehaviour actor;
    [SerializeField] protected LayerMask targetLayer;
    [SerializeField] protected LayerMask destroyLayer;

    [Space]

    [SerializeField] protected int damage;
    [SerializeField] protected float forceX;
    [SerializeField] protected float forceY;

    [Space]

    [SerializeField] protected GameObject hitEffect;

    protected MaterialController materialController;

    protected event System.Action monsterSkillEvent;

    protected virtual void Awake()
    {
        materialController = GetComponent<MaterialController>();
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        // 스킬 타겟 레이어와 충돌
        var collisionLayerValue = 1 << collision.gameObject.layer;
        if ((collisionLayerValue & targetLayer.value) > 0)
        {
            // 타겟이 플레이어인 경우
            PlayerBehaviour player = collision.GetComponent<PlayerBehaviour>();
            if (player)
            {
                // 플레이어가 타격 가능한 상태인 경우
                if (!player.IsHurt && !player.IsGodMode && !player.IsDead)
                {
                    // Actor가 있으면 Actor를 기준으로 방향 설정
                    Vector2 forceVector = (actor == null)
                        ? new Vector2(forceX * Mathf.Sign(player.transform.position.x - this.transform.position.x),
                            forceY)
                        : new Vector2(forceX * Mathf.Sign(player.transform.position.x - actor.transform.position.x),
                            forceY);

                    // 플레이어를 타격
                    var attackInfo = new AttackInfo(damage, forceVector, AttackType.Monster_SkillAttack);
                    IAttackListener.AttackResult attackResult = player.OnHit(attackInfo);

                    // 타격 성공
                    if (attackResult == IAttackListener.AttackResult.Success)
                    {
                        // 피격 이펙트 생성
                        Vector2 playerPos = player.transform.position;
                        var effect = Instantiate(hitEffect, playerPos + Random.insideUnitCircle * 0.3f, Quaternion.identity);

                        // 스킬이 플레이어를 타격한 경우 발생하는 이벤트
                        monsterSkillEvent?.Invoke();

                        // 타겟 레이어를 히트와 동시에 파괴
                        if ((collisionLayerValue & destroyLayer.value) > 0)
                        {
                            DestroyImmediately();
                        }
                    }
                }
            }
        }
    }

    public void SetActor(MonsterBehaviour act)
    {
        this.actor = act;
    }

    protected void DestroyImmediately()
    {
        Destroy(this.gameObject);
    }
}
