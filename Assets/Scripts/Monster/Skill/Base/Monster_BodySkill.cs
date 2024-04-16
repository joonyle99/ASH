using UnityEngine;

public class Monster_BodySkill : Monster_Skill
{
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (monster.IsDead) return;

        base.OnTriggerEnter2D(collision);
    }

    /// <summary>
    /// 스킬 사용 도중에만 Active되는 BodySkill은 몬스터가 갑자기 죽어서 미처 끄지 못했을 때를 대비하여
    /// 해당 게임 오브젝트를 비활성화해줘야 한다.
    /// </summary>
    private void OnDisable()
    {
        gameObject.SetActive(false);
    }
}
