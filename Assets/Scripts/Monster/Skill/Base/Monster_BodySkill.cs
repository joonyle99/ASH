using UnityEngine;

/// <summary>
/// 몬스터의 신체 중 일부로 공격하는 스킬이다.
/// 발동 후, 삭제되지 않으며 비활성화 되는 재사용되는 스킬
/// </summary>
public class Monster_BodySkill : Monster_Skill
{
    protected override void Awake()
    {
        base.Awake();

        // BodySkill의 경우 Actor를 자동으로 설정해준다
        actor = GetComponent<MonsterBehaviour>() ?? GetComponentInParent<MonsterBehaviour>();
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (actor == null)
        {
            Debug.LogError("BodySkill에 Actor가 없습니다");
            return;
        }

        if (actor.IsDead)
        {
            Debug.Log("BodySkill의 Actor가 사망 상태입니다");
            return;
        }

        base.OnTriggerEnter2D(collision);
    }
}
